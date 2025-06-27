using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public enum SocketState
    {
        Inactive,
        Connecting,
        Connected,
        Disconnecting,
        Listening,
        Error
    }

    public enum ClientType
    {
        ctNonBlocking,
        ctBlocking
    }

    [ToolboxItem(true)]
    [Description("Enhanced Asynchronous TCP Client Component - Delphi TClientSocket Style")]
    public partial class AsyncClient : Component
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private bool _active;
        private SocketState _socketState = SocketState.Inactive;
        private byte[] _buffer = new byte[4096];
        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _lockObject = new object();
        private System.Threading.Timer _keepAliveTimer;
        private volatile bool _disconnecting = false;

        // Events - Delphi style naming
        public event EventHandler<ConnectEventArgs> OnConnect;
        public event EventHandler<DisconnectEventArgs> OnDisconnect;
        public event EventHandler<ReadEventArgs> OnRead;
        public event EventHandler<WriteEventArgs> OnWrite;
        public event EventHandler<ErrorEventArgs> OnError;
        public event EventHandler<LookupEventArgs> OnLookup;

        // Properties matching Delphi TClientSocket
        [Category("Socket")]
        [Description("Activates or deactivates the socket connection")]
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
        [Description("Socket connection state")]
        [Browsable(false)]
        public SocketState SocketState
        {
            get { return _socketState; }
            private set
            {
                if (_socketState != value)
                {
                    _socketState = value;
                }
            }
        }

        [Category("Connection")]
        [Description("Server hostname or IP address")]
        [DefaultValue("localhost")]
        public string Host { get; set; } = "localhost";

        [Category("Connection")]
        [Description("Server port number")]
        [DefaultValue(23)]
        public int Port { get; set; } = 23;

        [Category("Connection")]
        [Description("Connection timeout in milliseconds")]
        [DefaultValue(5000)]
        public int Timeout { get; set; } = 5000;

        [Category("Socket")]
        [Description("Client socket type")]
        [DefaultValue(ClientType.ctNonBlocking)]
        public ClientType ClientType { get; set; } = ClientType.ctNonBlocking;

        private Encoding _textEncoding = Encoding.UTF8;
        [Category("Data")]
        [Description("Text encoding for string operations")]
        public Encoding TextEncoding
        {
            get { return _textEncoding ?? Encoding.UTF8; }
            set { _textEncoding = value ?? Encoding.UTF8; }
        }

        [Category("Socket")]
        [Description("Keep connection alive")]
        [DefaultValue(true)]
        public bool KeepAlive { get; set; } = true;

        [Category("Socket")]
        [Description("Keep alive interval in milliseconds")]
        [DefaultValue(30000)]
        public int KeepAliveInterval { get; set; } = 30000;

        [Category("Socket")]
        [Description("Enable Nagle algorithm")]
        [DefaultValue(true)]
        public bool NoDelay { get; set; } = true;

        [Category("Socket")]
        [Description("Automatically reconnect on disconnect")]
        [DefaultValue(false)]
        public bool AutoReconnect { get; set; } = false;

        [Category("Socket")]
        [Description("Reconnect delay in milliseconds")]
        [DefaultValue(5000)]
        public int ReconnectDelay { get; set; } = 5000;

        // Delphi-style properties
        [Browsable(false)]
        public bool Connected
        {
            get { return _active && _tcpClient != null && _tcpClient.Connected && SocketState == SocketState.Connected && !_disconnecting; }
        }

        [Browsable(false)]
        public Socket Socket
        {
            get { return _tcpClient?.Client; }
        }

        [Browsable(false)]
        public string LocalHost
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

        [Browsable(false)]
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

        [Browsable(false)]
        public string RemoteHost
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

        [Browsable(false)]
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

        public AsyncClient()
        {
            InitializeComponent();
            _textEncoding = Encoding.UTF8;
        }

        public AsyncClient(IContainer container)
        {
            if (container != null)
                container.Add(this);
            InitializeComponent();
            _textEncoding = Encoding.UTF8;
        }

        // Delphi-style methods
        public void Open()
        {
            if (_active || SocketState == SocketState.Connecting)
                return;

            try
            {
                SocketState = SocketState.Connecting;
                _disconnecting = false;
                _cancellationTokenSource = new CancellationTokenSource();

                if (ClientType == ClientType.ctBlocking)
                    Task.Run(() => ConnectSync());
                else
                    Task.Run(() => ConnectAsync());
            }
            catch (Exception ex)
            {
                SocketState = SocketState.Error;
                OnError?.Invoke(this, new ErrorEventArgs(ex, "Open"));
            }
        }

        public void Close()
        {
            if (!_active && SocketState == SocketState.Inactive)
                return;

            try
            {
                SocketState = SocketState.Disconnecting;
                _disconnecting = true;
                _active = false;

                _cancellationTokenSource?.Cancel();
                _keepAliveTimer?.Dispose();
                _keepAliveTimer = null;

                // Graceful shutdown
                if (_stream != null)
                {
                    try
                    {
                        _stream.Close();
                    }
                    catch { }
                    _stream = null;
                }

                if (_tcpClient != null)
                {
                    try
                    {
                        _tcpClient.Close();
                    }
                    catch { }
                    _tcpClient = null;
                }

                SocketState = SocketState.Inactive;
                _disconnecting = false;

                // Use Application.OpenForms to ensure thread safety for UI events
                if (OnDisconnect != null)
                {
                    var disconnectArgs = new DisconnectEventArgs("Manual disconnect");
                    if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
                    {
                        Application.OpenForms[0].Invoke(new Action(() =>
                            OnDisconnect?.Invoke(this, disconnectArgs)));
                    }
                    else
                    {
                        OnDisconnect?.Invoke(this, disconnectArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                SocketState = SocketState.Error;
                OnError?.Invoke(this, new ErrorEventArgs(ex, "Close"));
            }
        }

        private async Task ConnectAsync()
        {
            try
            {
                OnLookup?.Invoke(this, new LookupEventArgs(Host));

                _tcpClient = new TcpClient();

                if (KeepAlive && _tcpClient.Client != null)
                {
                    _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                }

                var connectTask = _tcpClient.ConnectAsync(Host, Port);
                var timeoutTask = Task.Delay(Timeout, _cancellationTokenSource.Token);

                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    _tcpClient?.Close();
                    throw new TimeoutException($"Connection timeout after {Timeout}ms");
                }

                if (_tcpClient.Connected)
                {
                    _stream = _tcpClient.GetStream();
                    _active = true;
                    SocketState = SocketState.Connected;

                    if (_tcpClient.Client != null)
                    {
                        _tcpClient.Client.NoDelay = NoDelay;
                    }

                    if (KeepAlive && KeepAliveInterval > 0)
                    {
                        _keepAliveTimer = new System.Threading.Timer(SendKeepAlive, null, KeepAliveInterval, KeepAliveInterval);
                    }

                    // Use Application.OpenForms to ensure thread safety for UI events
                    if (OnConnect != null)
                    {
                        var connectArgs = new ConnectEventArgs(this);
                        if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
                        {
                            Application.OpenForms[0].Invoke(new Action(() =>
                                OnConnect?.Invoke(this, connectArgs)));
                        }
                        else
                        {
                            OnConnect?.Invoke(this, connectArgs);
                        }
                    }

                    _ = Task.Run(() => ReceiveDataAsync(_cancellationTokenSource.Token));
                }
            }
            catch (Exception ex)
            {
                _active = false;
                SocketState = SocketState.Error;

                if (AutoReconnect && !_disconnecting)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(ReconnectDelay);
                        if (!_disconnecting)
                            Open();
                    });
                }

                OnError?.Invoke(this, new ErrorEventArgs(ex, "ConnectAsync"));
            }
        }

        private void ConnectSync()
        {
            try
            {
                OnLookup?.Invoke(this, new LookupEventArgs(Host));

                _tcpClient = new TcpClient();

                if (KeepAlive && _tcpClient.Client != null)
                {
                    _tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                }

                var connectTask = _tcpClient.ConnectAsync(Host, Port);
                if (!connectTask.Wait(Timeout))
                {
                    _tcpClient?.Close();
                    throw new TimeoutException($"Connection timeout after {Timeout}ms");
                }

                if (_tcpClient.Connected)
                {
                    _stream = _tcpClient.GetStream();
                    _active = true;
                    SocketState = SocketState.Connected;

                    if (_tcpClient.Client != null)
                    {
                        _tcpClient.Client.NoDelay = NoDelay;
                    }

                    if (KeepAlive && KeepAliveInterval > 0)
                    {
                        _keepAliveTimer = new System.Threading.Timer(SendKeepAlive, null, KeepAliveInterval, KeepAliveInterval);
                    }

                    // Use Application.OpenForms to ensure thread safety for UI events
                    if (OnConnect != null)
                    {
                        var connectArgs = new ConnectEventArgs(this);
                        if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
                        {
                            Application.OpenForms[0].Invoke(new Action(() =>
                                OnConnect?.Invoke(this, connectArgs)));
                        }
                        else
                        {
                            OnConnect?.Invoke(this, connectArgs);
                        }
                    }

                    _ = Task.Run(() => ReceiveDataAsync(_cancellationTokenSource.Token));
                }
            }
            catch (Exception ex)
            {
                _active = false;
                SocketState = SocketState.Error;

                if (AutoReconnect && !_disconnecting)
                {
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(ReconnectDelay);
                        if (!_disconnecting)
                            Open();
                    });
                }

                OnError?.Invoke(this, new ErrorEventArgs(ex, "ConnectSync"));
            }
        }

        private void SendKeepAlive(object state)
        {
            if (Connected && _tcpClient?.Client != null)
            {
                try
                {
                    _tcpClient.Client.Send(new byte[0]);
                }
                catch
                {
                    // Keep-alive failed, connection might be dead
                }
            }
        }

        public int SendText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            try
            {
                var encoding = TextEncoding ?? Encoding.UTF8;
                var data = encoding.GetBytes(text);
                return SendBuf(data);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new ErrorEventArgs(ex, "SendText"));
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
                    OnWrite?.Invoke(this, new WriteEventArgs(data.Length));
                    return data.Length;
                }
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new ErrorEventArgs(ex, "SendBuf"));
                return 0;
            }
        }

        public async Task<int> SendTextAsync(string text)
        {
            if (string.IsNullOrEmpty(text))
                return 0;

            try
            {
                var encoding = TextEncoding ?? Encoding.UTF8;
                var data = encoding.GetBytes(text);
                return await SendBufAsync(data);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new ErrorEventArgs(ex, "SendTextAsync"));
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
                OnWrite?.Invoke(this, new WriteEventArgs(data.Length));
                return data.Length;
            }
            catch (Exception ex)
            {
                OnError?.Invoke(this, new ErrorEventArgs(ex, "SendBufAsync"));
                return 0;
            }
        }

        public string ReceiveText()
        {
            var data = ReceiveBuf();
            if (data != null)
            {
                var encoding = TextEncoding ?? Encoding.UTF8;
                return encoding.GetString(data);
            }
            return "";
        }

        public byte[] ReceiveBuf()
        {
            if (!Connected || _stream == null)
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
                OnError?.Invoke(this, new ErrorEventArgs(ex, "ReceiveBuf"));
                return null;
            }
        }

        private async Task ReceiveDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (Connected && !cancellationToken.IsCancellationRequested && !_disconnecting)
                {
                    int bytesRead = await _stream.ReadAsync(_buffer, 0, _buffer.Length, cancellationToken);

                    if (bytesRead == 0)
                    {
                        break;
                    }

                    var data = new byte[bytesRead];
                    Array.Copy(_buffer, data, bytesRead);

                    // Use Application.OpenForms to ensure thread safety for UI events
                    if (OnRead != null)
                    {
                        var readArgs = new ReadEventArgs(data);
                        if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
                        {
                            Application.OpenForms[0].Invoke(new Action(() =>
                                OnRead?.Invoke(this, readArgs)));
                        }
                        else
                        {
                            OnRead?.Invoke(this, readArgs);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation
            }
            catch (Exception ex)
            {
                if (!_disconnecting)
                    OnError?.Invoke(this, new ErrorEventArgs(ex, "ReceiveDataAsync"));
            }
            finally
            {
                if (_active && !_disconnecting)
                {
                    _active = false;
                    SocketState = SocketState.Inactive;

                    // Use Application.OpenForms to ensure thread safety for UI events
                    if (OnDisconnect != null)
                    {
                        var disconnectArgs = new DisconnectEventArgs("Connection lost");
                        if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
                        {
                            Application.OpenForms[0].Invoke(new Action(() =>
                                OnDisconnect?.Invoke(this, disconnectArgs)));
                        }
                        else
                        {
                            OnDisconnect?.Invoke(this, disconnectArgs);
                        }
                    }

                    if (AutoReconnect && !_disconnecting)
                    {
                        _ = Task.Run(async () =>
                        {
                            await Task.Delay(ReconnectDelay);
                            if (!_disconnecting)
                                Open();
                        });
                    }
                }
            }
        }
    }

    // Event argument classes - CORRECTED VERSION WITH ALL PROPERTIES
    public class ConnectEventArgs : EventArgs
    {
        public DateTime ConnectedAt { get; }
        public AsyncClient Socket { get; }
        public string RemoteAddress { get; }
        public int RemotePort { get; }
        public string LocalAddress { get; }
        public int LocalPort { get; }

        public ConnectEventArgs(AsyncClient socket)
        {
            ConnectedAt = DateTime.Now;
            Socket = socket;
            RemoteAddress = socket.RemoteHost;
            RemotePort = socket.RemotePort;
            LocalAddress = socket.LocalHost;
            LocalPort = socket.LocalPort;
        }
    }

    public class DisconnectEventArgs : EventArgs
    {
        public string Reason { get; }
        public DateTime DisconnectedAt { get; }
        public DisconnectEventArgs(string reason) { Reason = reason; DisconnectedAt = DateTime.Now; }
    }

    public class ReadEventArgs : EventArgs
    {
        public byte[] Data { get; }
        public string Text { get; }
        public int ByteCount => Data?.Length ?? 0;
        public DateTime ReceivedAt { get; }

        public ReadEventArgs(byte[] data)
        {
            Data = data;
            var encoding = Encoding.UTF8;
            Text = data != null ? encoding.GetString(data) : "";
            ReceivedAt = DateTime.Now;
        }
    }

    public class WriteEventArgs : EventArgs
    {
        public int ByteCount { get; }
        public DateTime SentAt { get; }
        public WriteEventArgs(int byteCount) { ByteCount = byteCount; SentAt = DateTime.Now; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public string Source { get; }
        public DateTime ErrorAt { get; }
        public ErrorEventArgs(Exception exception, string source = "") { Exception = exception; Source = source; ErrorAt = DateTime.Now; }
    }

    public class LookupEventArgs : EventArgs
    {
        public string HostName { get; }
        public DateTime LookupAt { get; }
        public LookupEventArgs(string hostName) { HostName = hostName; LookupAt = DateTime.Now; }
    }
}