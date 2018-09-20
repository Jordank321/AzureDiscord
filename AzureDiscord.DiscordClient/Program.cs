using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureDiscord.DiscordClient
{
    class Program
    {
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();
        static async Task MainAsync(string[] args)
        {
            var socket = new DiscordWebSocket("TOKEN");
            await socket.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
