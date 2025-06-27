using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public enum ServerType
    {
        stNonBlocking,
        stThreadBlocking
    }

    [ToolboxItem(true)]
    [Description("Enhanced Asynchronous TCP Server Component - Delphi TServerSocket Style")]
    public partial class AsyncServer : Component
    {
        private TcpListener _listener;
        private bool _active;
        private readonly ConcurrentDictionary<int, ServerClientSocket> _connections = new ConcurrentDictionary<int, ServerClientSocket>();
        private readonly object _lockObject = new object();
        private CancellationTokenSource _cancellationTokenSource;
        private int _nextConnectionId = 0;

        // Events - Delphi style naming
        public event EventHandler<ServerListenEventArgs> OnListen;
        public event EventHandler<ClientConnectEventArgs> OnClientConnect;
        public event EventHandler<ClientDisconnectEventArgs> OnClientDisconnect;
        public event EventHandler<ClientReadEventArgs> OnClientRead;
        public event EventHandler<ClientWriteEventArgs> OnClientWrite;
        public event EventHandler<ServerErrorEventArgs> OnClientError;
        public event EventHandler<AcceptEventArgs> OnAccept;

        // Properties matching Delphi TServerSocket
        [Category("Socket")]
        [Description("Activates or deactivates the server socket")]
        [DefaultValue(false)]
        public bool Active
        {
            get { return _active; }
            set
            {
                if (_active != value)
                {
                    if (value)
                        Open();
                    else
                        Close();
                }
            }
        }

        [Category("Socket")]
        [Description("IP address to bind to (use 0.0.0.0 for all interfaces)")]
        [DefaultValue("0.0.0.0")]
        public string Address { get; set; } = "0.0.0.0";

        [Category("Socket")]
        [Description("Port number to listen on")]
        [DefaultValue(23)]
        public int Port { get; set; } = 23;

        [Category("Socket")]
        [Description("Server socket type")]
        [DefaultValue(ServerType.stNonBlocking)]
        public ServerType ServerType { get; set; } = ServerType.stNonBlocking;

        private Encoding _textEncoding = Encoding.UTF8;
        [Category("Data")]
        [Description("Text encoding for string operations")]
        public Encoding TextEncoding
        {
            get { return _textEncoding ?? Encoding.UTF8; }
            set { _textEncoding = value ?? Encoding.UTF8; }
        }

        [Category("Socket")]
        [Description("Maximum number of pending connections")]
        [DefaultValue(5)]
        public int MaxConnections { get; set; } = 5;

        [Category("Socket")]
        [Description("Enable Nagle algorithm for client connections")]
        [DefaultValue(true)]
        public bool NoDelay { get; set; } = true;

        [Category("Socket")]
        [Description("Keep client connections alive")]
        [DefaultValue(true)]
        public bool KeepAlive { get; set; } = true;

        // Delphi-style properties
        [Browsable(false)]
        public bool Listening
        {
            get { return _active && _listener != null; }
        }

        [Browsable(false)]
        public ServerSocket Socket { get; private set; }

        [Browsable(false)]
        public Socket ListenerSocket
        {
            get { return _listener?.Server; }
        }

        [Browsable(false)]
        public int ActiveConnections
        {
            get { return _connections.Count(c => c.Value != null && c.Value.Connected); }
        }

        public AsyncServer()
        {
            InitializeComponent();
            _textEncoding = Encoding.UTF8;
            Socket = new ServerSocket(this);
        }

        public AsyncServer(IContainer container)
        {
            if (container != null)
                container.Add(this);
            InitializeComponent();
            _textEncoding = Encoding.UTF8;
            Socket = new ServerSocket(this);
        }

        // Delphi-style methods
        public void Open()
        {
            if (_active)
                return;

            try
            {
                var ipAddress = Address == "0.0.0.0" ? IPAddress.Any : IPAddress.Parse(Address);
                _listener = new TcpListener(ipAddress, Port);
                _listener.Start(MaxConnections);

                _active = true;
                _cancellationTokenSource = new CancellationTokenSource();

                OnListen?.Invoke(this, new ServerListenEventArgs());

                // Start accepting clients
                if (ServerType == ServerType.stNonBlocking)
                    Task.Run(() => AcceptClientsAsync(_cancellationTokenSource.Token));
                else
                    Task.Run(() => AcceptClientsBlocking(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                _active = false;
                OnClientError?.Invoke(this, new ServerErrorEventArgs(ex, "Open"));
            }
        }

        public void Close()
        {
            if (!_active)
                return;

            try
            {
                _active = false;
                _cancellationTokenSource?.Cancel();

                // Disconnect all clients gracefully
                var clientsToDisconnect = new List<ServerClientSocket>();
                foreach (var connection in _connections.Values)
                {
                    if (connection != null)
                        clientsToDisconnect.Add(connection);
                }

                // Disconnect clients in parallel for faster shutdown
                Parallel.ForEach(clientsToDisconnect, client =>
                {
                    try
                    {
                        client.Disconnect();
                    }
                    catch { }
                });

                _connections.Clear();
                _listener?.Stop();
                _listener = null;
                _nextConnectionId = 0;
            }
            catch (Exception ex)
            {
                OnClientError?.Invoke(this, new ServerErrorEventArgs(ex, "Close"));
            }
        }

        // Delphi-style broadcast methods
        public int SendToAll(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            var data = TextEncoding.GetBytes(text);
            return SendBufToAll(data);
        }

        public int SendBufToAll(byte[] data)
        {
            if (data == null || data.Length == 0)
                return 0;

            int sentCount = 0;
            var activeClients = _connections.Values.Where(c => c != null && c.Connected).ToList();

            Parallel.ForEach(activeClients, client =>
            {
                try
                {
                    if (client.SendBuf(data) > 0)
                        Interlocked.Increment(ref sentCount);
                }
                catch { }
            });

            return sentCount;
        }

        public async Task<int> SendToAllAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            var data = TextEncoding.GetBytes(text);
            return await SendBufToAllAsync(data);
        }

        public async Task<int> SendBufToAllAsync(byte[] data)
        {
            if (data == null || data.Length == 0)
                return 0;

            var activeClients = _connections.Values.Where(c => c != null && c.Connected).ToList();
            var tasks = activeClients.Select(async client =>
            {
                try
                {
                    return await client.SendBufAsync(data) > 0 ? 1 : 0;
                }
                catch
                {
                    return 0;
                }
            });

            var results = await Task.WhenAll(tasks);
            return results.Sum();
        }

        // Delphi-style individual client methods
        public int SendToClient(int connectionId, string text)
        {
            if (_connections.TryGetValue(connectionId, out var client) && client != null)
                return client.SendText(text);
            return 0;
        }

        public int SendBufToClient(int connectionId, byte[] data)
        {
            if (_connections.TryGetValue(connectionId, out var client) && client != null)
                return client.SendBuf(data);
            return 0;
        }

        public async Task<int> SendToClientAsync(int connectionId, string text)
        {
            if (_connections.TryGetValue(connectionId, out var client) && client != null)
                return await client.SendTextAsync(text);
            return 0;
        }

        public async Task<int> SendBufToClientAsync(int connectionId, byte[] data)
        {
            if (_connections.TryGetValue(connectionId, out var client) && client != null)
                return await client.SendBufAsync(data);
            return 0;
        }

        public void DisconnectClient(int connectionId)
        {
            if (_connections.TryGetValue(connectionId, out var client) && client != null)
                client.Disconnect();
        }

        public ServerClientSocket GetClient(int connectionId)
        {
            _connections.TryGetValue(connectionId, out var client);
            return client;
        }

        public List<int> GetActiveConnectionIds()
        {
            return _connections.Where(c => c.Value != null && c.Value.Connected)
                              .Select(c => c.Key)
                              .ToList();
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            while (_active && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    var connectionId = Interlocked.Increment(ref _nextConnectionId);
                    var clientSocket = CreateClientSocket(tcpClient, connectionId);

                    _connections[connectionId] = clientSocket;

                    OnAccept?.Invoke(this, new AcceptEventArgs(clientSocket));
                    OnClientConnect?.Invoke(this, new ClientConnectEventArgs(clientSocket));

                    if (ServerType == ServerType.stNonBlocking)
                        _ = Task.Run(() => HandleClientAsync(clientSocket, cancellationToken));
                    else
                        _ = Task.Run(() => HandleClientBlocking(clientSocket, cancellationToken));
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (_active)
                        OnClientError?.Invoke(this, new ServerErrorEventArgs(ex, "AcceptClientsAsync"));
                }
            }
        }

        private void AcceptClientsBlocking(CancellationToken cancellationToken)
        {
            while (_active && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var tcpClient = _listener.AcceptTcpClient();
                    var connectionId = Interlocked.Increment(ref _nextConnectionId);
                    var clientSocket = CreateClientSocket(tcpClient, connectionId);

                    _connections[connectionId] = clientSocket;

                    OnAccept?.Invoke(this, new AcceptEventArgs(clientSocket));
                    OnClientConnect?.Invoke(this, new ClientConnectEventArgs(clientSocket));

                    _ = Task.Run(() => HandleClientBlocking(clientSocket, cancellationToken));
                }
                catch (Exception ex)
                {
                    if (_active)
                        OnClientError?.Invoke(this, new ServerErrorEventArgs(ex, "AcceptClientsBlocking"));
                }
            }
        }

        private ServerClientSocket CreateClientSocket(TcpClient tcpClient, int connectionId)
        {
            var clientSocket = new ServerClientSocket(tcpClient, this, connectionId);

            if (tcpClient.Client != null)
            {
                tcpClient.Client.NoDelay = NoDelay;
                if (KeepAlive)
                    tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            }

            return clientSocket;
        }

        private async Task HandleClientAsync(ServerClientSocket clientSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            try
            {
                while (_active && clientSocket.Connected && !cancellationToken.IsCancellationRequested)
                {
                    int bytesRead = await clientSocket.Stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                    if (bytesRead == 0)
                        break;

                    var data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead);

                    OnClientRead?.Invoke(this, new ClientReadEventArgs(clientSocket, data));
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation
            }
            catch (Exception ex)
            {
                OnClientError?.Invoke(this, new ServerErrorEventArgs(ex, $"HandleClientAsync - Client {clientSocket.ConnectionId}"));
            }
            finally
            {
                RemoveClient(clientSocket);
            }
        }

        private void HandleClientBlocking(ServerClientSocket clientSocket, CancellationToken cancellationToken)
        {
            var buffer = new byte[4096];

            try
            {
                while (_active && clientSocket.Connected && !cancellationToken.IsCancellationRequested)
                {
                    if (clientSocket.Stream.DataAvailable)
                    {
                        int bytesRead = clientSocket.Stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;

                        var data = new byte[bytesRead];
                        Array.Copy(buffer, data, bytesRead);

                        OnClientRead?.Invoke(this, new ClientReadEventArgs(clientSocket, data));
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            }
            catch (Exception ex)
            {
                OnClientError?.Invoke(this, new ServerErrorEventArgs(ex, $"HandleClientBlocking - Client {clientSocket.ConnectionId}"));
            }
            finally
            {
                RemoveClient(clientSocket);
            }
        }

        internal void RemoveClient(ServerClientSocket clientSocket)
        {
            try
            {
                _connections.TryRemove(clientSocket.ConnectionId, out _);
                clientSocket.InternalDisconnect();

                // Use Application.OpenForms to ensure thread safety for UI events
                if (OnClientDisconnect != null)
                {
                    var disconnectArgs = new ClientDisconnectEventArgs(clientSocket);
                    if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
                    {
                        Application.OpenForms[0].Invoke(new Action(() =>
                            OnClientDisconnect?.Invoke(this, disconnectArgs)));
                    }
                    else
                    {
                        OnClientDisconnect?.Invoke(this, disconnectArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                OnClientError?.Invoke(this, new ServerErrorEventArgs(ex, $"RemoveClient - Client {clientSocket.ConnectionId}"));
            }
        }

        internal void NotifyWrite(ServerClientSocket clientSocket, int byteCount)
        {
            OnClientWrite?.Invoke(this, new ClientWriteEventArgs(clientSocket, byteCount));
        }

        internal void NotifyError(ServerClientSocket clientSocket, Exception ex, string source)
        {
            OnClientError?.Invoke(this, new ServerErrorEventArgs(ex, source, clientSocket));
        }

        internal ConcurrentDictionary<int, ServerClientSocket> GetConnections()
        {
            return _connections;
        }
    }

    // Delphi-style server socket class
    public class ServerSocket
    {
        private readonly AsyncServer _server;

        internal ServerSocket(AsyncServer server)
        {
            _server = server;
        }

        public ServerClientSocket[] Connections
        {
            get { return _server.GetConnections().Values.Where(c => c != null).ToArray(); }
        }

        public ServerClientSocket this[int connectionId]
        {
            get { return _server.GetClient(connectionId); }
        }

        public int ConnectionCount => _server.ActiveConnections;

        public Socket Handle => _server.ListenerSocket;

        public List<int> ActiveConnectionIds => _server.GetActiveConnectionIds();
    }

    // ServerClientSocket
    public class ServerClientSocket
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        internal readonly AsyncServer _server;
        private readonly object _lockObject = new object();

        public int ConnectionId { get; internal set; }
        public NetworkStream Stream => _stream;
        public bool Connected => _tcpClient?.Connected == true;
        public Socket Socket => _tcpClient?.Client;

        public string RemoteAddress
        {
            get
            {
                try
                {
                    return _tcpClient?.Client?.RemoteEndPoint is IPEndPoint remoteEP ? remoteEP.Address.ToString() : "";
                }
                catch { return ""; }
            }
        }

        public int RemotePort
        {
            get
            {
                try
                {
                    return _tcpClient?.Client?.RemoteEndPoint is IPEndPoint remoteEP ? remoteEP.Port : 0;
                }
                catch { return 0; }
            }
        }

        public string LocalAddress
        {
            get
            {
                try
                {
                    return _tcpClient?.Client?.LocalEndPoint is IPEndPoint localEP ? localEP.Address.ToString() : "";
                }
                catch { return ""; }
            }
        }

        public int LocalPort
        {
            get
            {
                try
                {
                    return _tcpClient?.Client?.LocalEndPoint is IPEndPoint localEP ? localEP.Port : 0;
                }
                catch { return 0; }
            }
        }

        internal ServerClientSocket(TcpClient tcpClient, AsyncServer server, int connectionId)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            _server = server;
            ConnectionId = connectionId;
        }

        // Send methods
        public int SendText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            try
            {
                var encoding = _server?.TextEncoding ?? Encoding.UTF8;
                var data = encoding.GetBytes(text);
                return SendBuf(data);
            }
            catch (Exception ex)
            {
                _server?.NotifyError(this, ex, "SendText");
                return 0;
            }
        }

        public int SendBuf(byte[] data)
        {
            if (data == null || data.Length == 0 || !Connected)
                return 0;

            try
            {
                lock (_lockObject)
                {
                    _stream.Write(data, 0, data.Length);
                    _server?.NotifyWrite(this, data.Length);
                    return data.Length;
                }
            }
            catch (Exception ex)
            {
                _server?.NotifyError(this, ex, "SendBuf");
                return 0;
            }
        }

        public async Task<int> SendTextAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            try
            {
                var encoding = _server?.TextEncoding ?? Encoding.UTF8;
                var data = encoding.GetBytes(text);
                return await SendBufAsync(data);
            }
            catch (Exception ex)
            {
                _server?.NotifyError(this, ex, "SendTextAsync");
                return 0;
            }
        }

        public async Task<int> SendBufAsync(byte[] data)
        {
            if (data == null || data.Length == 0 || !Connected)
                return 0;

            try
            {
                await _stream.WriteAsync(data, 0, data.Length);
                _server?.NotifyWrite(this, data.Length);
                return data.Length;
            }
            catch (Exception ex)
            {
                _server?.NotifyError(this, ex, "SendBufAsync");
                return 0;
            }
        }

        // Receive methods
        public string ReceiveText()
        {
            var data = ReceiveBuf();
            if (data != null)
            {
                var encoding = _server?.TextEncoding ?? Encoding.UTF8;
                return encoding.GetString(data);
            }
            return "";
        }

        public byte[] ReceiveBuf()
        {
            if (!Connected)
                return null;

            try
            {
                if (_stream.DataAvailable)
                {
                    var buffer = new byte[4096];
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        var result = new byte[bytesRead];
                        Array.Copy(buffer, result, bytesRead);
                        return result;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _server?.NotifyError(this, ex, "ReceiveBuf");
                return null;
            }
        }

        public void Disconnect()
        {
            _server?.RemoveClient(this);
        }

        internal void InternalDisconnect()
        {
            try
            {
                _stream?.Close();
                _tcpClient?.Close();
            }
            catch { }
        }
    }

    // Event argument classes
    public class ServerListenEventArgs : EventArgs
    {
        public DateTime ListenAt { get; }
        public ServerListenEventArgs() { ListenAt = DateTime.Now; }
    }

    public class ClientConnectEventArgs : EventArgs
    {
        public ServerClientSocket Socket { get; }
        public DateTime ConnectedAt { get; }
        public ClientConnectEventArgs(ServerClientSocket socket) { Socket = socket; ConnectedAt = DateTime.Now; }
    }

    public class ClientDisconnectEventArgs : EventArgs
    {
        public ServerClientSocket Socket { get; }
        public DateTime DisconnectedAt { get; }
        public ClientDisconnectEventArgs(ServerClientSocket socket) { Socket = socket; DisconnectedAt = DateTime.Now; }
    }

    public class ClientReadEventArgs : EventArgs
    {
        public ServerClientSocket Socket { get; }
        public byte[] Data { get; }
        public string Text { get; }
        public int ByteCount => Data?.Length ?? 0;
        public DateTime ReceivedAt { get; }

        public ClientReadEventArgs(ServerClientSocket socket, byte[] data)
        {
            Socket = socket;
            Data = data;
            var encoding = socket?._server?.TextEncoding ?? Encoding.UTF8;
            Text = data != null ? encoding.GetString(data) : "";
            ReceivedAt = DateTime.Now;
        }
    }

    public class ClientWriteEventArgs : EventArgs
    {
        public ServerClientSocket Socket { get; }
        public int ByteCount { get; }
        public DateTime SentAt { get; }
        public ClientWriteEventArgs(ServerClientSocket socket, int byteCount) { Socket = socket; ByteCount = byteCount; SentAt = DateTime.Now; }
    }

    public class ServerErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public string Source { get; }
        public ServerClientSocket Socket { get; }
        public DateTime ErrorAt { get; }
        public ServerErrorEventArgs(Exception exception, string source = "", ServerClientSocket socket = null) { Exception = exception; Source = source; Socket = socket; ErrorAt = DateTime.Now; }
    }

    public class AcceptEventArgs : EventArgs
    {
        public ServerClientSocket Socket { get; }
        public DateTime AcceptedAt { get; }
        public AcceptEventArgs(ServerClientSocket socket) { Socket = socket; AcceptedAt = DateTime.Now; }
    }
}