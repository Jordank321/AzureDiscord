using System.Threading.Tasks;
using AzureDiscord.WebSocket;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();
        static async Task MainAsync(string[] args)
        {
            using (var client = new WebSocketClient("gateway.discord.gg", 80))
            {
                await client.Connect();
            }
        }
    }
}
