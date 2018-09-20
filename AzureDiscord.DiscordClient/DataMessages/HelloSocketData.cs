using Newtonsoft.Json;

namespace AzureDiscord.DiscordClient
{
    internal class HelloSocketData
    {
        [JsonProperty(PropertyName = "heartbeat_interval")]
        public int HeartbeatInterval { get; set; }
        [JsonProperty(PropertyName = "_trace")]
        public string[] Trace { get; set; }
    }
}