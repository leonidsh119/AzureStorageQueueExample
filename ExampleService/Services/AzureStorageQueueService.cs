using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace ExampleService.Services
{
    public class AzureStorageQueueService : IAzureStorageQueueService
    {
        private readonly ILogger<AzureStorageQueueService> _logger;
        private readonly QueueClient _queue;

        public AzureStorageQueueService(IConfiguration configuration, ILogger<AzureStorageQueueService> logger)
        {
            _logger = logger;
            string connectionString = configuration["AZURE:STORAGEACCOUNTCONNECTIONSTRING"];
            string queueName = configuration["AZURE:JOBSTORAGEQUEUENAME"];
            _logger.LogTrace("Connecting to Azure Storage Queue [{}].", queueName);
            _queue = new QueueClient(connectionString, queueName);
        }

        public int CountMessages()
        {
            return _queue.MaxPeekableMessages;
        }

        public bool ClearMessages()
        {
            Azure.Response response = _queue.ClearMessages();
            return response.IsError;
        }

        public IEnumerable<string> SendMessages(int batchSize)
        {
            _logger.LogTrace($"Generating batch of {batchSize} messages.");
            var batchId = Guid.NewGuid().ToString();
            for (int index = 0; index < batchSize; index++)
            {
                Azure.Response<SendReceipt> response = _queue.SendMessage(string.Format("Example Message [{0}/{1}] from batch [{2}]", index + 1, batchSize, batchId));
                yield return response.Value.MessageId;
            }
        }

        public IEnumerable<string> ReceiveMessages(int batchSize)
        {
            foreach(var message in _queue.ReceiveMessages(batchSize).Value)
            {
                _queue.DeleteMessage(message.MessageId, message.PopReceipt);
                yield return message.Body.ToString();
            }
        }
    }
}
