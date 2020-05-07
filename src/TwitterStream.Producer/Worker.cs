using System;
using System.Threading;
using System.Threading.Tasks;
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

                stream.MatchingTweetReceived += (sender, arguments) =>
                {
                    _logger.LogInformation($"Tweet: {arguments.Tweet.Text}");
                };

                stream.StartStreamMatchingAllConditions();

                await Task.Delay(15000, stoppingToken);
            }
        }



    }
}
