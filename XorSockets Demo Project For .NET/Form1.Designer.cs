namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.asyncClient1 = new WindowsFormsApp1.AsyncClient(this.components);
            this.asyncServer1 = new WindowsFormsApp1.AsyncServer(this.components);
            this.serverGroup = new System.Windows.Forms.GroupBox();
            this.txtServerLog = new System.Windows.Forms.TextBox();
            this.lblServerLog = new System.Windows.Forms.Label();
            this.btnDisconnectClient = new System.Windows.Forms.Button();
            this.btnBroadcast = new System.Windows.Forms.Button();
            this.btnSendToClient = new System.Windows.Forms.Button();
            this.txtServerMessage = new System.Windows.Forms.TextBox();
            this.txtClientId = new System.Windows.Forms.TextBox();
            this.lblServerMsg = new System.Windows.Forms.Label();
            this.lstConnections = new System.Windows.Forms.ListBox();
            this.lblConnections = new System.Windows.Forms.Label();
            this.btnStopServer = new System.Windows.Forms.Button();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.lblServerPort = new System.Windows.Forms.Label();
            this.clientGroup = new System.Windows.Forms.GroupBox();
            this.txtClientLog = new System.Windows.Forms.TextBox();
            this.lblClientLog = new System.Windows.Forms.Label();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.txtClientMessage = new System.Windows.Forms.TextBox();
            this.lblClientMessage = new System.Windows.Forms.Label();
            this.lblConnectionInfo = new System.Windows.Forms.Label();
            this.lblClientStatus = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtClientPort = new System.Windows.Forms.TextBox();
            this.lblClientPort = new System.Windows.Forms.Label();
            this.txtClientHost = new System.Windows.Forms.TextBox();
            this.lblClientHost = new System.Windows.Forms.Label();
            this.serverGroup.SuspendLayout();
            this.clientGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // asyncClient1
            // 
            this.asyncClient1.TextEncoding = ((System.Text.Encoding)(resources.GetObject("asyncClient1.TextEncoding")));
            this.asyncClient1.OnConnect += new System.EventHandler<WindowsFormsApp1.ConnectEventArgs>(this.asyncClient1_OnConnect);
            this.asyncClient1.OnDisconnect += new System.EventHandler<WindowsFormsApp1.DisconnectEventArgs>(this.asyncClient1_OnDisconnect);
            this.asyncClient1.OnRead += new System.EventHandler<WindowsFormsApp1.ReadEventArgs>(this.asyncClient1_OnRead);
            this.asyncClient1.OnError += new System.EventHandler<WindowsFormsApp1.ErrorEventArgs>(this.asyncClient1_OnError);
            this.asyncClient1.OnLookup += new System.EventHandler<WindowsFormsApp1.LookupEventArgs>(this.asyncClient1_OnLookup);
            // 
            // asyncServer1
            // 
            this.asyncServer1.TextEncoding = ((System.Text.Encoding)(resources.GetObject("asyncServer1.TextEncoding")));
            this.asyncServer1.OnListen += new System.EventHandler<WindowsFormsApp1.ServerListenEventArgs>(this.asyncServer1_OnListen);
            this.asyncServer1.OnClientConnect += new System.EventHandler<WindowsFormsApp1.ClientConnectEventArgs>(this.asyncServer1_OnClientConnect);
            this.asyncServer1.OnClientDisconnect += new System.EventHandler<WindowsFormsApp1.ClientDisconnectEventArgs>(this.asyncServer1_OnClientDisconnect);
            this.asyncServer1.OnClientRead += new System.EventHandler<WindowsFormsApp1.ClientReadEventArgs>(this.asyncServer1_OnClientRead);
            this.asyncServer1.OnClientError += new System.EventHandler<WindowsFormsApp1.ServerErrorEventArgs>(this.asyncServer1_OnClientError);
            // 
            // serverGroup
            // 
            this.serverGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.serverGroup.Controls.Add(this.txtServerLog);
            this.serverGroup.Controls.Add(this.lblServerLog);
            this.serverGroup.Controls.Add(this.btnDisconnectClient);
            this.serverGroup.Controls.Add(this.btnBroadcast);
            this.serverGroup.Controls.Add(this.btnSendToClient);
            this.serverGroup.Controls.Add(this.txtServerMessage);
            this.serverGroup.Controls.Add(this.txtClientId);
            this.serverGroup.Controls.Add(this.lblServerMsg);
            this.serverGroup.Controls.Add(this.lstConnections);
            this.serverGroup.Controls.Add(this.lblConnections);
            this.serverGroup.Controls.Add(this.btnStopServer);
            this.serverGroup.Controls.Add(this.btnStartServer);
            this.serverGroup.Controls.Add(this.txtServerPort);
            this.serverGroup.Controls.Add(this.lblServerPort);
            this.serverGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverGroup.Location = new System.Drawing.Point(12, 12);
            this.serverGroup.Name = "serverGroup";
            this.serverGroup.Size = new System.Drawing.Size(480, 640);
            this.serverGroup.TabIndex = 0;
            this.serverGroup.TabStop = false;
            this.serverGroup.Text = "TCP Server";
            // 
            // txtServerLog
            // 
            this.txtServerLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerLog.Location = new System.Drawing.Point(10, 205);
            this.txtServerLog.Multiline = true;
            this.txtServerLog.Name = "txtServerLog";
            this.txtServerLog.ReadOnly = true;
            this.txtServerLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtServerLog.Size = new System.Drawing.Size(460, 420);
            this.txtServerLog.TabIndex = 13;
            // 
            // lblServerLog
            // 
            this.lblServerLog.AutoSize = true;
            this.lblServerLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServerLog.Location = new System.Drawing.Point(10, 185);
            this.lblServerLog.Name = "lblServerLog";
            this.lblServerLog.Size = new System.Drawing.Size(62, 13);
            this.lblServerLog.TabIndex = 12;
            this.lblServerLog.Text = "Server Log:";
            // 
            // btnDisconnectClient
            // 
            this.btnDisconnectClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisconnectClient.Location = new System.Drawing.Point(350, 125);
            this.btnDisconnectClient.Name = "btnDisconnectClient";
            this.btnDisconnectClient.Size = new System.Drawing.Size(80, 23);
            this.btnDisconnectClient.TabIndex = 11;
            this.btnDisconnectClient.Text = "Disconnect";
            this.btnDisconnectClient.UseVisualStyleBackColor = true;
            this.btnDisconnectClient.Click += new System.EventHandler(this.BtnDisconnectClient_Click);
            // 
            // btnBroadcast
            // 
            this.btnBroadcast.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBroadcast.Location = new System.Drawing.Point(260, 125);
            this.btnBroadcast.Name = "btnBroadcast";
            this.btnBroadcast.Size = new System.Drawing.Size(80, 23);
            this.btnBroadcast.TabIndex = 10;
            this.btnBroadcast.Text = "Broadcast";
            this.btnBroadcast.UseVisualStyleBackColor = true;
            this.btnBroadcast.Click += new System.EventHandler(this.BtnBroadcast_Click);
            // 
            // btnSendToClient
            // 
            this.btnSendToClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendToClient.Location = new System.Drawing.Point(160, 125);
            this.btnSendToClient.Name = "btnSendToClient";
            this.btnSendToClient.Size = new System.Drawing.Size(90, 23);
            this.btnSendToClient.TabIndex = 9;
            this.btnSendToClient.Text = "Send to Client";
            this.btnSendToClient.UseVisualStyleBackColor = true;
            this.btnSendToClient.Click += new System.EventHandler(this.BtnSendToClient_Click);
            // 
            // txtServerMessage
            // 
            this.txtServerMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerMessage.Location = new System.Drawing.Point(160, 100);
            this.txtServerMessage.Name = "txtServerMessage";
            this.txtServerMessage.Size = new System.Drawing.Size(200, 20);
            this.txtServerMessage.TabIndex = 8;
            this.txtServerMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtServerMessage_KeyPress);
            // 
            // txtClientId
            // 
            this.txtClientId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClientId.Location = new System.Drawing.Point(160, 75);
            this.txtClientId.Name = "txtClientId";
            this.txtClientId.Size = new System.Drawing.Size(50, 20);
            this.txtClientId.TabIndex = 7;
            // 
            // lblServerMsg
            // 
            this.lblServerMsg.AutoSize = true;
            this.lblServerMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServerMsg.Location = new System.Drawing.Point(160, 55);
            this.lblServerMsg.Name = "lblServerMsg";
            this.lblServerMsg.Size = new System.Drawing.Size(90, 13);
            this.lblServerMsg.TabIndex = 6;
            this.lblServerMsg.Text = "Send to Client ID:";
            // 
            // lstConnections
            // 
            this.lstConnections.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstConnections.FormattingEnabled = true;
            this.lstConnections.Location = new System.Drawing.Point(10, 75);
            this.lstConnections.Name = "lstConnections";
            this.lstConnections.Size = new System.Drawing.Size(140, 95);
            this.lstConnections.TabIndex = 5;
            this.lstConnections.SelectedIndexChanged += new System.EventHandler(this.LstConnections_SelectedIndexChanged);
            // 
            // lblConnections
            // 
            this.lblConnections.AutoSize = true;
            this.lblConnections.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnections.Location = new System.Drawing.Point(10, 55);
            this.lblConnections.Name = "lblConnections";
            this.lblConnections.Size = new System.Drawing.Size(102, 13);
            this.lblConnections.TabIndex = 4;
            this.lblConnections.Text = "Active Connections:";
            // 
            // btnStopServer
            // 
            this.btnStopServer.Enabled = false;
            this.btnStopServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStopServer.Location = new System.Drawing.Point(210, 22);
            this.btnStopServer.Name = "btnStopServer";
            this.btnStopServer.Size = new System.Drawing.Size(80, 23);
            this.btnStopServer.TabIndex = 3;
            this.btnStopServer.Text = "Stop Server";
            this.btnStopServer.UseVisualStyleBackColor = true;
            this.btnStopServer.Click += new System.EventHandler(this.BtnStopServer_Click);
            // 
            // btnStartServer
            // 
            this.btnStartServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartServer.Location = new System.Drawing.Point(125, 22);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(80, 23);
            this.btnStartServer.TabIndex = 2;
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.UseVisualStyleBackColor = true;
            this.btnStartServer.Click += new System.EventHandler(this.BtnStartServer_Click);
            // 
            // txtServerPort
            // 
            this.txtServerPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerPort.Location = new System.Drawing.Point(55, 23);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(60, 20);
            this.txtServerPort.TabIndex = 1;
            this.txtServerPort.Text = "8080";
            // 
            // lblServerPort
            // 
            this.lblServerPort.AutoSize = true;
            this.lblServerPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblServerPort.Location = new System.Drawing.Point(10, 25);
            this.lblServerPort.Name = "lblServerPort";
            this.lblServerPort.Size = new System.Drawing.Size(29, 13);
            this.lblServerPort.TabIndex = 0;
            this.lblServerPort.Text = "Port:";
            // 
            // clientGroup
            // 
            this.clientGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clientGroup.Controls.Add(this.txtClientLog);
            this.clientGroup.Controls.Add(this.lblClientLog);
            this.clientGroup.Controls.Add(this.btnSendMessage);
            this.clientGroup.Controls.Add(this.txtClientMessage);
            this.clientGroup.Controls.Add(this.lblClientMessage);
            this.clientGroup.Controls.Add(this.lblConnectionInfo);
            this.clientGroup.Controls.Add(this.lblClientStatus);
            this.clientGroup.Controls.Add(this.btnDisconnect);
            this.clientGroup.Controls.Add(this.btnConnect);
            this.clientGroup.Controls.Add(this.txtClientPort);
            this.clientGroup.Controls.Add(this.lblClientPort);
            this.clientGroup.Controls.Add(this.txtClientHost);
            this.clientGroup.Controls.Add(this.lblClientHost);
            this.clientGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clientGroup.Location = new System.Drawing.Point(500, 12);
            this.clientGroup.Name = "clientGroup";
            this.clientGroup.Size = new System.Drawing.Size(480, 640);
            this.clientGroup.TabIndex = 1;
            this.clientGroup.TabStop = false;
            this.clientGroup.Text = "TCP Client";
            // 
            // txtClientLog
            // 
            this.txtClientLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtClientLog.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClientLog.Location = new System.Drawing.Point(10, 155);
            this.txtClientLog.Multiline = true;
            this.txtClientLog.Name = "txtClientLog";
            this.txtClientLog.ReadOnly = true;
            this.txtClientLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtClientLog.Size = new System.Drawing.Size(460, 470);
            this.txtClientLog.TabIndex = 12;
            // 
            // lblClientLog
            // 
            this.lblClientLog.AutoSize = true;
            this.lblClientLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientLog.Location = new System.Drawing.Point(10, 135);
            this.lblClientLog.Name = "lblClientLog";
            this.lblClientLog.Size = new System.Drawing.Size(57, 13);
            this.lblClientLog.TabIndex = 11;
            this.lblClientLog.Text = "Client Log:";
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Enabled = false;
            this.btnSendMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendMessage.Location = new System.Drawing.Point(380, 102);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(60, 23);
            this.btnSendMessage.TabIndex = 10;
            this.btnSendMessage.Text = "Send";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.BtnSendMessage_Click);
            // 
            // txtClientMessage
            // 
            this.txtClientMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClientMessage.Location = new System.Drawing.Point(70, 103);
            this.txtClientMessage.Name = "txtClientMessage";
            this.txtClientMessage.Size = new System.Drawing.Size(300, 20);
            this.txtClientMessage.TabIndex = 9;
            this.txtClientMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtClientMessage_KeyPress);
            // 
            // lblClientMessage
            // 
            this.lblClientMessage.AutoSize = true;
            this.lblClientMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientMessage.Location = new System.Drawing.Point(10, 105);
            this.lblClientMessage.Name = "lblClientMessage";
            this.lblClientMessage.Size = new System.Drawing.Size(53, 13);
            this.lblClientMessage.TabIndex = 8;
            this.lblClientMessage.Text = "Message:";
            // 
            // lblConnectionInfo
            // 
            this.lblConnectionInfo.AutoSize = true;
            this.lblConnectionInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConnectionInfo.Location = new System.Drawing.Point(10, 75);
            this.lblConnectionInfo.Name = "lblConnectionInfo";
            this.lblConnectionInfo.Size = new System.Drawing.Size(0, 13);
            this.lblConnectionInfo.TabIndex = 7;
            // 
            // lblClientStatus
            // 
            this.lblClientStatus.AutoSize = true;
            this.lblClientStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientStatus.ForeColor = System.Drawing.Color.Red;
            this.lblClientStatus.Location = new System.Drawing.Point(10, 55);
            this.lblClientStatus.Name = "lblClientStatus";
            this.lblClientStatus.Size = new System.Drawing.Size(109, 13);
            this.lblClientStatus.TabIndex = 6;
            this.lblClientStatus.Text = "Status: Disconnected";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisconnect.Location = new System.Drawing.Point(330, 22);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(80, 23);
            this.btnDisconnect.TabIndex = 5;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.BtnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(250, 22);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(70, 23);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.BtnConnect_Click);
            // 
            // txtClientPort
            // 
            this.txtClientPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClientPort.Location = new System.Drawing.Point(180, 23);
            this.txtClientPort.Name = "txtClientPort";
            this.txtClientPort.Size = new System.Drawing.Size(60, 20);
            this.txtClientPort.TabIndex = 3;
            this.txtClientPort.Text = "8080";
            // 
            // lblClientPort
            // 
            this.lblClientPort.AutoSize = true;
            this.lblClientPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientPort.Location = new System.Drawing.Point(140, 25);
            this.lblClientPort.Name = "lblClientPort";
            this.lblClientPort.Size = new System.Drawing.Size(29, 13);
            this.lblClientPort.TabIndex = 2;
            this.lblClientPort.Text = "Port:";
            // 
            // txtClientHost
            // 
            this.txtClientHost.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtClientHost.Location = new System.Drawing.Point(50, 23);
            this.txtClientHost.Name = "txtClientHost";
            this.txtClientHost.Size = new System.Drawing.Size(80, 20);
            this.txtClientHost.TabIndex = 1;
            this.txtClientHost.Text = "localhost";
            // 
            // lblClientHost
            // 
            this.lblClientHost.AutoSize = true;
            this.lblClientHost.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClientHost.Location = new System.Drawing.Point(10, 25);
            this.lblClientHost.Name = "lblClientHost";
            this.lblClientHost.Size = new System.Drawing.Size(32, 13);
            this.lblClientHost.TabIndex = 0;
            this.lblClientHost.Text = "Host:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 664);
            this.Controls.Add(this.clientGroup);
            this.Controls.Add(this.serverGroup);
            this.MinimumSize = new System.Drawing.Size(1000, 700);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XorSockets Demo - Server & Client Chat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.serverGroup.ResumeLayout(false);
            this.serverGroup.PerformLayout();
            this.clientGroup.ResumeLayout(false);
            this.clientGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AsyncClient asyncClient1;
        private AsyncServer asyncServer1;
        private System.Windows.Forms.GroupBox serverGroup;
        private System.Windows.Forms.TextBox txtServerLog;
        private System.Windows.Forms.Label lblServerLog;
        private System.Windows.Forms.Button btnDisconnectClient;
        private System.Windows.Forms.Button btnBroadcast;
        private System.Windows.Forms.Button btnSendToClient;
        private System.Windows.Forms.TextBox txtServerMessage;
        private System.Windows.Forms.TextBox txtClientId;
        private System.Windows.Forms.Label lblServerMsg;
        private System.Windows.Forms.ListBox lstConnections;
        private System.Windows.Forms.Label lblConnections;
        private System.Windows.Forms.Button btnStopServer;
        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Label lblServerPort;
        private System.Windows.Forms.GroupBox clientGroup;
        private System.Windows.Forms.TextBox txtClientLog;
        private System.Windows.Forms.Label lblClientLog;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.TextBox txtClientMessage;
        private System.Windows.Forms.Label lblClientMessage;
        private System.Windows.Forms.Label lblConnectionInfo;
        private System.Windows.Forms.Label lblClientStatus;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtClientPort;
        private System.Windows.Forms.Label lblClientPort;
        private System.Windows.Forms.TextBox txtClientHost;
        private System.Windows.Forms.Label lblClientHost;
    }
}