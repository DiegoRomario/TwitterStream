using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Streaming;

namespace TwitterStream.Producer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly TwitterAPIAccess _options;
        public Worker(ILogger<Worker> logger, IOptions<TwitterAPIAccess> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("What topic would you like to get tweets about?");
            string subject = Console.ReadLine();
            while (!stoppingToken.IsCancellationRequested)
            {
                ITwitterCredentials demo = Auth.SetUserCredentials(_options.ConsumerKey, _options.ConsumerSecret, _options.AccessToken, _options.AccessTokenSecret);

                IFilteredStream stream = Stream.CreateFilteredStream();

                stream.AddTrack(subject);

                stream.AddTweetLanguageFilter(LanguageFilter.Portuguese);

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("I'm listening to twitter");

                stream.MatchingTweetReceived += async (sender, arguments) =>
                {
                    await SendTweetsByKafka(new Tweet() { TweetedBy = arguments.Tweet.CreatedBy.ScreenName, TweetedAt = arguments.Tweet.CreatedAt, Text = arguments.Tweet.Text });
                };

                stream.StartStreamMatchingAllConditions();

                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task SendTweetsByKafka (Tweet tweet)
        {
            ProducerConfig config = new ProducerConfig() { BootstrapServers = "localhost:9092" };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    string al = JsonSerializer.ToJson<Tweet>(tweet);
                    var result = await producer.ProduceAsync("twittertopic", new Message<Null, string>() { Value = al });
                    _logger.LogInformation("Tweet sent at: {time}", DateTimeOffset.Now);
                }
                catch (ProduceException<Null, string> e)
                {

                    _logger.LogError($"{e.Error.Code}: Tweet failed: {e.Error.Reason}");
                }
            }
        }


    }
}
