# XorSockets

**Enhanced Asynchronous TCP Client & Server Components for C# .NET**

XorSockets provides powerful, easy-to-use TCP networking components inspired by Delphi's TClientSocket and TServerSocket. Built for .NET Framework 4.8+ and Windows Forms applications, these components offer both blocking and non-blocking operation modes with comprehensive event handling.

<div align="center">
  <img src="Preview.png" alt="XorSockets" />
</div>
<div align="center">
  <img src="Preview1.png" alt="XorSockets Demo Application" />
</div>


## 🚀 Features

### AsyncClient Component
- **Multiple Connection Types**: Non-blocking (async) and blocking modes
- **Auto-Reconnection**: Configurable automatic reconnection with custom delays
- **Keep-Alive Support**: Built-in TCP keep-alive functionality
- **Thread-Safe Events**: All events are automatically marshaled to the UI thread
- **Flexible Data Handling**: Send/receive both text and binary data
- **Connection Management**: Comprehensive connection state tracking
- **Timeout Control**: Configurable connection timeouts

### AsyncServer Component
- **Multi-Client Support**: Handle multiple simultaneous client connections
- **Broadcasting**: Send messages to all connected clients simultaneously
- **Individual Client Control**: Send to specific clients or disconnect them individually
- **Connection Tracking**: Monitor all active connections with detailed information
- **Scalable Architecture**: Efficient handling of multiple clients using async/await patterns
- **Event-Driven Design**: Rich event model for all server activities

### Common Features
- **Delphi-Style API**: Familiar interface for developers coming from Delphi
- **Visual Designer Support**: Drag-and-drop components in Visual Studio
- **Comprehensive Logging**: Detailed event information for debugging
- **Error Handling**: Robust error handling with detailed exception information
- **Encoding Support**: Configurable text encoding (UTF8, ASCII, etc.)

## 📦 Installation

### Method 1: Direct Integration
1. Download the source files from this repository
2. Add `AsyncClient.cs`, `AsyncServer.cs`, and their corresponding `.Designer.cs` files to your project
3. Build your project
4. The components will appear in your Visual Studio Toolbox

### Method 2: Template Installation
1. Copy the XorSockets template folder to:
   ```
   C:\Users\[YOUR_USERNAME]\Documents\Visual Studio 2022\Templates\ItemTemplates\
   ```
2. Restart Visual Studio
3. Add components via Project → Add New Item → XorSockets Template

## 🔧 Quick Start

### Basic TCP Client

```csharp
// Drag AsyncClient from toolbox or create programmatically
AsyncClient client = new AsyncClient();

// Configure connection
client.Host = "localhost";
client.Port = 8080;
client.ClientType = ClientType.ctNonBlocking;
client.AutoReconnect = true;

// Wire up events
client.OnConnect += (sender, e) => {
    Console.WriteLine($"Connected to {e.RemoteAddress}:{e.RemotePort}");
};

client.OnRead += (sender, e) => {
    Console.WriteLine($"Received: {e.Text}");
};

client.OnDisconnect += (sender, e) => {
    Console.WriteLine($"Disconnected: {e.Reason}");
};

// Connect and send data
client.Active = true;
client.SendText("Hello Server!");
```

### Basic TCP Server

```csharp
// Drag AsyncServer from toolbox or create programmatically
AsyncServer server = new AsyncServer();

// Configure server
server.Port = 8080;
server.MaxConnections = 100;
server.ServerType = ServerType.stNonBlocking;

// Wire up events
server.OnClientConnect += (sender, e) => {
    Console.WriteLine($"Client {e.Socket.ConnectionId} connected from {e.Socket.RemoteAddress}");
    server.SendToClient(e.Socket.ConnectionId, "Welcome to the server!");
};

server.OnClientRead += (sender, e) => {
    Console.WriteLine($"Client {e.Socket.ConnectionId}: {e.Text}");
    // Echo to all clients
    server.SendToAll($"[{e.Socket.ConnectionId}]: {e.Text}");
};

server.OnClientDisconnect += (sender, e) => {
    Console.WriteLine($"Client {e.Socket.ConnectionId} disconnected");
};

// Start server
server.Active = true;
```

## 📋 Component Properties

### AsyncClient Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Active` | bool | false | Activates/deactivates the connection |
| `Host` | string | "localhost" | Server hostname or IP address |
| `Port` | int | 23 | Server port number |
| `ClientType` | ClientType | ctNonBlocking | Connection type (blocking/non-blocking) |
| `Timeout` | int | 5000 | Connection timeout in milliseconds |
| `AutoReconnect` | bool | false | Automatically reconnect on disconnect |
| `ReconnectDelay` | int | 5000 | Delay between reconnection attempts |
| `KeepAlive` | bool | true | Enable TCP keep-alive |
| `KeepAliveInterval` | int | 30000 | Keep-alive interval in milliseconds |
| `TextEncoding` | Encoding | UTF8 | Text encoding for string operations |

### AsyncServer Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Active` | bool | false | Activates/deactivates the server |
| `Address` | string | "0.0.0.0" | IP address to bind to |
| `Port` | int | 23 | Port number to listen on |
| `ServerType` | ServerType | stNonBlocking | Server operation mode |
| `MaxConnections` | int | 5 | Maximum pending connections |
| `NoDelay` | bool | true | Disable Nagle algorithm |
| `KeepAlive` | bool | true | Enable keep-alive for clients |
| `TextEncoding` | Encoding | UTF8 | Text encoding for string operations |

## 🎯 Events

### AsyncClient Events

- **OnConnect**: Fired when connection is established
- **OnDisconnect**: Fired when connection is lost
- **OnRead**: Fired when data is received
- **OnWrite**: Fired when data is sent
- **OnError**: Fired when an error occurs
- **OnLookup**: Fired during hostname resolution

### AsyncServer Events

- **OnListen**: Fired when server starts listening
- **OnClientConnect**: Fired when a client connects
- **OnClientDisconnect**: Fired when a client disconnects
- **OnClientRead**: Fired when data is received from a client
- **OnClientWrite**: Fired when data is sent to a client
- **OnClientError**: Fired when a client error occurs
- **OnAccept**: Fired when a client connection is accepted

## 🌟 Advanced Features

### Broadcasting Messages
```csharp
// Send to all connected clients
int clientCount = server.SendToAll("Server announcement!");

// Send to specific client
server.SendToClient(clientId, "Private message");

// Async broadcasting
await server.SendToAllAsync("Async broadcast message");
```

### Connection Management
```csharp
// Get active connection IDs
var activeConnections = server.Socket.ActiveConnectionIds;

// Access specific client
var client = server.Socket[connectionId];
Console.WriteLine($"Client IP: {client.RemoteAddress}");

// Disconnect specific client
server.DisconnectClient(connectionId);
```

### Binary Data Handling
```csharp
// Send binary data
byte[] binaryData = File.ReadAllBytes("data.bin");
client.SendBuf(binaryData);

// Receive binary data
client.OnRead += (sender, e) => {
    byte[] receivedData = e.Data;
    // Process binary data
};
```

## 🔍 Demo Application

The repository includes a comprehensive demo application (`Form1.cs`) that showcases:

- **Side-by-side client and server** in a single application
- **Real-time connection monitoring** with active client lists
- **Interactive message exchange** between clients and server
- **Broadcasting capabilities** with server announcements
- **Individual client management** with disconnect functionality
- **Comprehensive logging** of all socket activities

To run the demo:
1. Build the project in Visual Studio
2. Run the application
3. Start the server by clicking "Start Server"
4. Connect clients by clicking "Connect"
5. Exchange messages and observe the real-time logging

## ⚡ Performance Tips

1. **Use Non-Blocking Mode**: For better scalability, use `ClientType.ctNonBlocking` and `ServerType.stNonBlocking`
2. **Optimize Buffer Sizes**: The default 4KB buffer works well for most scenarios
3. **Enable Keep-Alive**: For long-lived connections, enable keep-alive to detect dead connections
4. **Handle Events Efficiently**: Keep event handlers lightweight to avoid blocking the socket operations
5. **Use Async Methods**: For high-throughput scenarios, prefer `SendTextAsync` and `SendBufAsync`

## 🛠️ Requirements

- **.NET Framework 4.8** or higher
- **Windows Forms** application
- **Visual Studio 2019** or higher (recommended)
- **Windows OS** (tested on Windows 10/11)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## 📞 Support

If you encounter any issues or have questions:
1. Check the demo application for implementation examples
2. Review the comprehensive event logging for debugging
3. Open an issue on GitHub with detailed information

## 🙏 Acknowledgments

- Inspired by Delphi's TClientSocket and TServerSocket components
- Built with modern async/await patterns for optimal performance
- Designed with Windows Forms developers in mind

---

**Made with ❤️ for the .NET community**
