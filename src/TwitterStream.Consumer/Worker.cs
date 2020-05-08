using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitterStream.Producer;

namespace TwitterStream.Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting the service: {time}", DateTimeOffset.Now);
            await Task.Delay(2000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumerConfig config = new ConsumerConfig()
                {
                    GroupId = "twittergroup",
                    BootstrapServers = "localhost:9092",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    consumer.Subscribe("twittertopic");
                    try
                    {
                        while (true)
                        {
                            ConsumeResult<Ignore, string> message = consumer.Consume(stoppingToken);
                            Tweet tweet = JsonSerializer.Deserialize<Tweet>(message.Message.Value);
                            _logger.LogInformation($"Tweet received at {DateTimeOffset.Now}:\n\nText: {tweet.Text}\nTweeted by: {tweet.TweetedBy} \nTweeted at: {tweet.TweetedAt} {new String('-',40)}");
                            await Task.Delay(10, stoppingToken);
                        }
                    }

                    catch (OperationCanceledException)
                    {
                        consumer.Close();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                }

                
            }
        }
    }
}
