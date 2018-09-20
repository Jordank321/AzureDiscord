using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureDiscord.DiscordClient.DataMessages;
using Newtonsoft.Json;

namespace AzureDiscord.DiscordClient
{
    class DiscordWebSocket
    {
        private readonly string _botToken;
        private ClientWebSocket _webSocket;
        private HttpClient _httpClient = new HttpClient();
        private Encoding _encoding = Encoding.UTF8;

        private int _heartbeatInterval = 5000;
        private int? _sequenceNumber;
        private volatile bool _heartbeatAcked;

        public DiscordWebSocket(string botToken)
        {
            _botToken = botToken;
        }

        public async Task ConnectAsync()
        {
            var gatewayUri = new Uri(await GetDiscordWebSocketGatewayUriAsync() + "/?v=6&encoding=json");
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(gatewayUri, CancellationToken.None);
            var handleTask = HandleMessagesAsync();
        }

        private async Task HandleMessagesAsync()
        {
            byte[] buffer = new byte[1024];
            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty,
                            CancellationToken.None);
                    }
                    else
                    {
                        var messageString = _encoding.GetString(buffer.Take(result.Count).ToArray());
                        while (!result.EndOfMessage)
                        {
                            result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                            messageString += _encoding.GetString(buffer.Take(result.Count).ToArray());
                        }
                        Console.WriteLine($"RECIEVED: {messageString}");
                        var message = JsonConvert.DeserializeObject<DiscordWebSocketMessage>(messageString);
                        if (message.SequenceNumber.HasValue) _sequenceNumber = message.SequenceNumber;
                        switch (message.OpCode)
                        {
                            case 10:
                                HelloHanshake(message);
                                break;
                            case 11:
                                Console.WriteLine("HEARTBEAT ACK RECIEVED");
                                _heartbeatAcked = true;
                                break;
                            default:
                                Console.WriteLine($"Unknown message: {messageString}");
                                break;
                        }
                    }
                }
                catch(Exception e)
                {
                }
            }
        }

        private void HelloHanshake(DiscordWebSocketMessage message)
        {
            var hello = GetData<HelloSocketData>(message);
            _heartbeatInterval = hello.HeartbeatInterval;
            _ = HeartbeatAsync();
            _ = SendAsync(new DiscordWebSocketMessage
            {
                OpCode = 2,
                Data = new IdentifySocketData
                {
                    //Compress = false,
                    Token = _botToken
                }
            });
        }

        private async Task HeartbeatAsync()
        {
            while (true)
            {
                if (_heartbeatAcked == false) await ZombiedConnection();

                Console.WriteLine($"SENDING HEARTBEAT {_sequenceNumber}");
                _heartbeatAcked = false;
                await SendAsync(new DiscordWebSocketMessage
                {
                    OpCode = 1,
                    Data = _sequenceNumber
                });

                await Task.Delay(_heartbeatInterval);
            }
        }

        private Task ZombiedConnection()
        {
            //TODO: reconnect!
            return Task.CompletedTask;
        }

        private async Task SendAsync(DiscordWebSocketMessage message)
        {
            var messageString = JsonConvert.SerializeObject(message, Formatting.Indented);

            Console.WriteLine($"SENDING MESSAGE: {messageString}");
            var buffer = _encoding.GetBytes(messageString);
            await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private static T GetData<T>(DiscordWebSocketMessage message)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(message.Data));
        }

        private async Task<string> GetDiscordWebSocketGatewayUriAsync()
        {
            var gatewayResponse = await _httpClient.GetAsync("https://discordapp.com/api/gateway");
            gatewayResponse.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GatewayResponse>(await gatewayResponse.Content.ReadAsStringAsync()).Url;
        }

        private class GatewayResponse
        {
            public string Url { get; set; }
        }
    }
}
