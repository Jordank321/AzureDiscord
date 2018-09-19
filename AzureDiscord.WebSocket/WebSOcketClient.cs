using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AzureDiscord.WebSocket
{
    public class WebSocketClient : IDisposable
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;

        public WebSocketClient(string webSocketUrl, int port)
        {
            WebSocketUrl = webSocketUrl;
            Port = port;
        }

        public string WebSocketUrl { get; }
        public int Port { get; }

        public void Dispose()
        {
            _tcpClient.Dispose();
            _stream.Dispose();
        }

        public async Task Connect()
        {
            //var ip = await Dns.GetHostAddressesAsync(WebSocketUrl);
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(WebSocketUrl, Port);
            _stream = _tcpClient.GetStream();
            var updrageRequest = Encoding.ASCII.GetBytes(HttpUpgradeRequestString());
            await _stream.WriteAsync(updrageRequest, 0, updrageRequest.Length);
            using (var responseStream = new MemoryStream())
            {
                await _stream.CopyToAsync(responseStream);
                responseStream.Position = 0;
                var data = responseStream.ToArray();
                var response = Encoding.ASCII.GetString(data);
            }
        }

        private string HttpUpgradeRequestString()
        {
            var key = new byte[16];
            var rand = new Random();
            
            rand.NextBytes(key);
            var builder = new StringBuilder();
            builder.AppendLine($"GET {WebSocketUrl} HTTP/1.1");
            builder.AppendLine($"Host: {WebSocketUrl}");
            builder.AppendLine("Upgrade: websocket");
            builder.AppendLine("Connection: Upgrade");
            builder.AppendLine($"Sec-WebSocket-Key: {Convert.ToBase64String(key)}");
            builder.AppendLine("Sec-WebSocket-Version: 13");
            builder.AppendLine();
            return builder.ToString();
        }
    }
}
