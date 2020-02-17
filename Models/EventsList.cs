using Newtonsoft.Json;

namespace LineMessageApi.Models
{
    public class EventsList
    {
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }

        [JsonProperty("source")]
        public Source Source { get; set; }
    }
}