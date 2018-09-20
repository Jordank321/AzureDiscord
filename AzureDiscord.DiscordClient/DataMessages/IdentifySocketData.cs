
using Newtonsoft.Json;

namespace AzureDiscord.DiscordClient.DataMessages
{
    internal class IdentifySocketData
    {
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "properties")]
        public ConnectionProperties Properties { get; } = new ConnectionProperties();
        [JsonProperty(PropertyName = "compress", NullValueHandling=NullValueHandling.Ignore)]
        public bool? Compress { get; set; }
        [JsonProperty(PropertyName = "large_threshold", NullValueHandling=NullValueHandling.Ignore)]
        public int? LargeThreshold { get; set; }
        [JsonProperty(PropertyName = "shard", NullValueHandling=NullValueHandling.Ignore)]
        public int[] Shard { get; set; }
        [JsonProperty(PropertyName = "presence", NullValueHandling=NullValueHandling.Ignore)]
        public UpdateStatus Presence { get; } = new UpdateStatus();
    }

    internal class UpdateStatus
    {
        [JsonProperty(PropertyName = "since")] public int? Since { get; set; }
        [JsonProperty(PropertyName = "game")] public Activity Game { get; set; }
        [JsonProperty(PropertyName = "status")] public string Status { get; set; }
        [JsonProperty(PropertyName = "afk")] public bool Afk { get; set; }
    }

    internal class Activity
    {
        //TODO: fill this in from https://discordapp.com/developers/docs/topics/gateway#activity-object
    }

    internal class ConnectionProperties
    {
        [JsonProperty(PropertyName = "$os")]
        public string Os { get; set; } = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        [JsonProperty(PropertyName = "$browser")]
        public string Browser { get; set; } = "AzureDiscord.Net";
        [JsonProperty(PropertyName = "$device")]
        public string Device { get; set; } = "AzureDiscord.Net";
    }
}
