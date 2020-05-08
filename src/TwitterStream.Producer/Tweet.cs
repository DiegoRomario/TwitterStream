using System;

namespace TwitterStream.Producer
{
    [Serializable]
    public class Tweet
    {
        public Tweet(string tweetedBy, DateTime tweetedAt, string text)
        {
            TweetedBy = tweetedBy;
            TweetedAt = tweetedAt;
            Text = text;
        }

        public string TweetedBy { get; set; }
        public DateTime TweetedAt  { get; set; }
        public string Text { get; set; }
    }
}
