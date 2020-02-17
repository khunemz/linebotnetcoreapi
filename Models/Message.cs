using Newtonsoft.Json;

namespace LineMessageApi.Models
{
    public class Message
    {
        [JsonProperty("text")]
        public string Text { get; set; }
    }
}