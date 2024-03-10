using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ExampleService.Services
{
    public class MessageProcessor : BackgroundService
    {
        private readonly ILogger<MessageProcessor> _logger;
        private readonly IConfiguration _configuration;
        private readonly QueueClient _queue;

        public MessageProcessor(IConfiguration configuration, ILogger<MessageProcessor> logger)
        {
            _logger = logger;
            _configuration = configuration;
            string connectionString = _configuration["AZURE:STORAGEACCOUNTCONNECTIONSTRING"];
            string queueName = _configuration["AZURE:JOBSTORAGEQUEUENAME"];
            _logger.LogTrace("Connecting to Azure Storage Queue [{}].", queueName);
            _queue = new QueueClient(connectionString, queueName);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var batchSize = int.Parse(_configuration["Application:BatchSize"]);
                var delay = int.Parse(_configuration["Application:BatchRateSec"]);
                int processedMessages = ProcessBatch(batchSize);
                if (processedMessages == 0) // no need to wait if still mssages are in queue
                {
                    await Task.Delay(1000 * delay, stoppingToken);
                }
            }
        }

        public int ProcessBatch(int batchSize)
        {
            _logger.LogTrace("Dequiuing batch of maximum [{}] messages.", batchSize);
            var messages = _queue.ReceiveMessages(batchSize);
            _logger.LogTrace("Dequiued batch of [{}] messages.", messages.Value.Length);
            Parallel.ForEach(messages.Value.ToList(), ProcessMessage);
            _logger.LogTrace("Finished processing batch of [{}] messages.", messages.Value.Length);
            return messages.Value.Length;
        }

        private void ProcessMessage(QueueMessage message)
        {
            _logger.LogInformation("Processing Message [{}]:  [{}].", message.MessageId, message.Body.ToString());
            _logger.LogTrace("Message [{}] processed successfully.", message.MessageId);

            _logger.LogTrace("Deleting message [{}].", message.MessageId);
            _queue.DeleteMessage(message.MessageId, message.PopReceipt);
            _logger.LogTrace("Message [{}] deleted successfully.", message.MessageId);

        }
    }
}
