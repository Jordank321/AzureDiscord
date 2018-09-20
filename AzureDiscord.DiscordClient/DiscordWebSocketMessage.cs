using Newtonsoft.Json;

namespace AzureDiscord.DiscordClient
{
    public class DiscordWebSocketMessage
    {
        [JsonProperty(PropertyName = "op")]
        public int OpCode { get; set; }
        [JsonProperty(PropertyName = "s", NullValueHandling=NullValueHandling.Ignore)]
        public int? SequenceNumber { get; set; }
        [JsonProperty(PropertyName = "t", NullValueHandling=NullValueHandling.Ignore)]
        public string EventName { get; set; }

        [JsonProperty(PropertyName = "d")]
        public object Data { get; set; }
    }
}