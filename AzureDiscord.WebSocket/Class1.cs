using System;
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

            
        }

        private string HttpUpgradeRequestString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("GET / ")
        }
    }
}
