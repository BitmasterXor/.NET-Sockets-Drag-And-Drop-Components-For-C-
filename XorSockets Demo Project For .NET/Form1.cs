using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private bool _isShuttingDown = false;

        public Form1()
        {
            InitializeComponent();
        }

        #region Helper Methods for Thread-Safe UI Updates

        private void SafeInvoke(Action action)
        {
            try
            {
                if (this.IsDisposed || _isShuttingDown)
                    return;

                if (this.InvokeRequired)
                    this.Invoke(action);
                else
                    action();
            }
            catch (ObjectDisposedException)
            {
                // Form is being disposed, ignore
            }
            catch (InvalidOperationException)
            {
                // Control handle not created or being destroyed, ignore
            }
        }

        private void AppendServerLog(string message)
        {
            SafeInvoke(() =>
            {
                txtServerLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\r\n");
                txtServerLog.ScrollToCaret();
            });
        }

        private void AppendClientLog(string message)
        {
            SafeInvoke(() =>
            {
                txtClientLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\r\n");
                txtClientLog.ScrollToCaret();
            });
        }

        private void UpdateConnectionsList()
        {
            SafeInvoke(() =>
            {
                try
                {
                    lstConnections.Items.Clear();

                    // Only update if server is active and has valid socket
                    if (asyncServer1.Active && asyncServer1.Socket != null)
                    {
                        var activeIds = asyncServer1.Socket.ActiveConnectionIds;
                        foreach (int id in activeIds)
                        {
                            var client = asyncServer1.Socket[id];
                            if (client != null)
                            {
                                lstConnections.Items.Add($"ID:{id} - {client.RemoteAddress}:{client.RemotePort}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Silently handle any errors during shutdown
                    System.Diagnostics.Debug.WriteLine($"UpdateConnectionsList error: {ex.Message}");
                }
            });
        }

        private void UpdateServerButtons()
        {
            SafeInvoke(() =>
            {
                btnStartServer.Enabled = !asyncServer1.Active;
                btnStopServer.Enabled = asyncServer1.Active;
            });
        }

        private void UpdateClientButtons()
        {
            SafeInvoke(() =>
            {
                bool connected = asyncClient1.Connected;

                btnConnect.Enabled = !connected;
                btnDisconnect.Enabled = connected;
                btnSendMessage.Enabled = connected;

                if (connected)
                {
                    lblClientStatus.Text = "Status: Connected";
                    lblClientStatus.ForeColor = Color.Green;
                    lblConnectionInfo.Text = $"Connected to {asyncClient1.RemoteHost}:{asyncClient1.RemotePort} | Local: {asyncClient1.LocalHost}:{asyncClient1.LocalPort}";
                }
                else
                {
                    lblClientStatus.Text = "Status: Disconnected";
                    lblClientStatus.ForeColor = Color.Red;
                    lblConnectionInfo.Text = "";
                }
            });
        }

        #endregion

        #region Server Event Handlers

        private void asyncServer1_OnListen(object sender, ServerListenEventArgs e)
        {
            AppendServerLog($"Server listening on port {asyncServer1.Port}");
            UpdateServerButtons();
        }

        private void asyncServer1_OnClientConnect(object sender, ClientConnectEventArgs e)
        {
            AppendServerLog($"Client {e.Socket.ConnectionId} connected from {e.Socket.RemoteAddress}:{e.Socket.RemotePort}");

            // Send welcome message
            asyncServer1.SendToClient(e.Socket.ConnectionId, $"Welcome! Your client ID is {e.Socket.ConnectionId}\r\n");

            // Broadcast join message
            asyncServer1.SendToAll($">>> Client {e.Socket.ConnectionId} joined the chat\r\n");

            UpdateConnectionsList();
        }

        private void asyncServer1_OnClientDisconnect(object sender, ClientDisconnectEventArgs e)
        {
            AppendServerLog($"Client {e.Socket.ConnectionId} disconnected");

            // Only broadcast if server is still active (not shutting down)
            if (asyncServer1.Active)
            {
                asyncServer1.SendToAll($">>> Client {e.Socket.ConnectionId} left the chat\r\n");
            }

            // Use a small delay to prevent UI flooding during mass disconnects
            Task.Delay(50).ContinueWith(_ => UpdateConnectionsList());
        }

        private void asyncServer1_OnClientRead(object sender, ClientReadEventArgs e)
        {
            string message = e.Text.Trim();
            AppendServerLog($"From Client {e.Socket.ConnectionId}: {message}");

            // Echo to all clients with sender ID
            asyncServer1.SendToAll($"[Client {e.Socket.ConnectionId}]: {message}\r\n");
        }

        private void asyncServer1_OnClientError(object sender, ServerErrorEventArgs e)
        {
            string clientInfo = e.Socket != null ? $" (Client {e.Socket.ConnectionId})" : "";
            AppendServerLog($"ERROR{clientInfo}: {e.Exception.Message}");
        }

        #endregion

        #region Client Event Handlers

        private void asyncClient1_OnConnect(object sender, ConnectEventArgs e)
        {
            AppendClientLog($"Connected to server at {e.RemoteAddress}:{e.RemotePort}");
            AppendClientLog($"Local endpoint: {e.LocalAddress}:{e.LocalPort}");
            UpdateClientButtons();
        }

        private void asyncClient1_OnDisconnect(object sender, DisconnectEventArgs e)
        {
            AppendClientLog($"Disconnected: {e.Reason}");
            UpdateClientButtons();
        }

        private void asyncClient1_OnRead(object sender, ReadEventArgs e)
        {
            AppendClientLog($"Received: {e.Text.Trim()}");
        }

        private void asyncClient1_OnError(object sender, ErrorEventArgs e)
        {
            AppendClientLog($"ERROR: {e.Exception.Message}");
        }

        private void asyncClient1_OnLookup(object sender, LookupEventArgs e)
        {
            AppendClientLog($"Looking up host: {e.HostName}");
        }

        #endregion

        #region Button Event Handlers

        private void BtnStartServer_Click(object sender, EventArgs e)
        {
            try
            {
                asyncServer1.Port = int.Parse(txtServerPort.Text);
                asyncServer1.Active = true;
                AppendServerLog("Starting server...");
            }
            catch (Exception ex)
            {
                AppendServerLog($"Failed to start server: {ex.Message}");
            }
        }

        private void BtnStopServer_Click(object sender, EventArgs e)
        {
            try
            {
                AppendServerLog("Stopping server...");

                // Disable the button immediately to prevent double-clicks
                btnStopServer.Enabled = false;

                // Stop the server on a background thread to prevent UI freezing
                Task.Run(() =>
                {
                    try
                    {
                        asyncServer1.Active = false;

                        // Update UI on main thread after server stops
                        SafeInvoke(() =>
                        {
                            AppendServerLog("Server stopped");
                            UpdateServerButtons();
                            UpdateConnectionsList();
                        });
                    }
                    catch (Exception ex)
                    {
                        SafeInvoke(() =>
                        {
                            AppendServerLog($"Error stopping server: {ex.Message}");
                            UpdateServerButtons();
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                AppendServerLog($"Error stopping server: {ex.Message}");
                UpdateServerButtons();
            }
        }

        private void BtnSendToClient_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtClientId.Text, out int clientId) && !string.IsNullOrEmpty(txtServerMessage.Text))
            {
                int sent = asyncServer1.SendToClient(clientId, txtServerMessage.Text + "\r\n");
                if (sent > 0)
                {
                    AppendServerLog($"Sent to Client {clientId}: {txtServerMessage.Text}");
                    txtServerMessage.Clear();
                }
                else
                {
                    AppendServerLog($"Failed to send to Client {clientId} (not connected)");
                }
            }
        }

        private void BtnBroadcast_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtServerMessage.Text))
            {
                int sentCount = asyncServer1.SendToAll($"[SERVER BROADCAST]: {txtServerMessage.Text}\r\n");
                AppendServerLog($"Broadcast sent to {sentCount} clients: {txtServerMessage.Text}");
                txtServerMessage.Clear();
            }
        }

        private void BtnDisconnectClient_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtClientId.Text, out int clientId))
            {
                asyncServer1.DisconnectClient(clientId);
                AppendServerLog($"Disconnected Client {clientId}");
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                asyncClient1.Host = txtClientHost.Text;
                asyncClient1.Port = int.Parse(txtClientPort.Text);
                asyncClient1.Active = true;

                AppendClientLog($"Connecting to {asyncClient1.Host}:{asyncClient1.Port}...");
                UpdateClientButtons();
            }
            catch (Exception ex)
            {
                AppendClientLog($"Connection failed: {ex.Message}");
            }
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            asyncClient1.Active = false;
            AppendClientLog("Disconnecting...");
        }

        private void BtnSendMessage_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtClientMessage.Text))
            {
                int sent = asyncClient1.SendText(txtClientMessage.Text + "\r\n");
                if (sent > 0)
                {
                    AppendClientLog($"Sent: {txtClientMessage.Text}");
                    txtClientMessage.Clear();
                }
                else
                {
                    AppendClientLog("Failed to send message (not connected)");
                }
            }
        }

        private void TxtClientMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnSendMessage_Click(sender, e);
                e.Handled = true;
            }
        }

        private void TxtServerMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnBroadcast_Click(sender, e);
                e.Handled = true;
            }
        }

        private void LstConnections_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Auto-fill client ID when selecting from connections list
            if (lstConnections.SelectedItem != null)
            {
                string selectedItem = lstConnections.SelectedItem.ToString();
                if (selectedItem.StartsWith("ID:"))
                {
                    string idPart = selectedItem.Substring(3, selectedItem.IndexOf(' ') - 3);
                    txtClientId.Text = idPart;
                }
            }
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            AppendServerLog("=== XorSockets Demo Server Ready ===");
            AppendClientLog("=== XorSockets Demo Client Ready ===");
            AppendServerLog("Click 'Start Server' to begin listening for connections");
            AppendClientLog("Enter server details and click 'Connect' to join");
        }

        private bool isClosing = false;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isShuttingDown)
            {
                // Prevent the form from closing immediately
                e.Cancel = true;
                _isShuttingDown = true;

                // Disable the form to show we're shutting down
                this.Enabled = false;
                this.Text = "XorSockets Demo - Shutting down...";

                // Shutdown connections on background thread
                Task.Run(() =>
                {
                    try
                    {
                        // FORCE stop server - no graceful bullshit
                        if (asyncServer1 != null)
                        {
                            asyncServer1.Active = false;
                            asyncServer1.Dispose();
                        }

                        // FORCE stop client - no graceful bullshit
                        if (asyncClient1 != null)
                        {
                            asyncClient1.Active = false;
                            asyncClient1.Dispose();
                        }

                        // Wait for components to die
                        Thread.Sleep(500);

                        // KILL THE APPLICATION COMPLETELY
                        Environment.Exit(0);
                    }
                    catch
                    {
                        // If ANYTHING fails, FORCE KILL THE PROCESS
                        Environment.Exit(0);
                    }
                });
            }
        }
    }
}