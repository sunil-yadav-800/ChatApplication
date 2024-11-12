using ChatApi.Entities;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace ChatApi.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceProvider _serviceProvider;
        public ConsumerService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = "chatGroup",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
             {
                 await ConsumeMessage(stoppingToken);
             });
            //await ConsumeMessage(stoppingToken);
        }
        private async Task ConsumeMessage(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("message");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    var message = JsonConvert.DeserializeObject<Message>(consumeResult.Message.Value);
                    if (message != null)
                    {
                        await SaveMessageToDb(message);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            _consumer.Close();
        }
        private async Task SaveMessageToDb(Message message)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ContextDb>();
                    dbContext.Messages.Add(message);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to save message: " + ex.Message);
            }
        }
    }
}
