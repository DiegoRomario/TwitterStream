using Newtonsoft.Json;
using System;

namespace TwitterStream.Producer
{
    [Serializable]
    public class Tweet
    {
        [JsonProperty]
        public string TweetedBy { get; set; }
        [JsonProperty]
        public DateTime TweetedAt  { get; set; }
        [JsonProperty]
        public string Text { get; set; }
    }
}
