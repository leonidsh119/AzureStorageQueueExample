using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ExampleFunction
{
    public class ExampleFunction
    {
        private readonly ILogger _logger;
        private QueueClient _queue;
        private readonly IConfiguration _configuration;

        public ExampleFunction(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExampleFunction>();
            _configuration = configuration;
            string connectionString = _configuration["StoarageAccountConnectionString"];
            string queueName = _configuration["JobStorageQueueName"];
            _logger.LogInformation("Connecting to Azure Storage Queue [{}].", queueName);
            _queue = new QueueClient(connectionString, queueName);
            _logger.LogInformation("Connecting to Azure Storage Queue [{}].", queueName);
        }

        [Function("ExampleFunction")]
        public void Run([TimerTrigger("%TimerInterval%")] TimerInfo myTimer)
        {
            string timerInterval = _configuration["TimerInterval"];
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now} with interval: {timerInterval}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            var minBatchSize = int.Parse(_configuration["MinBatchSize"]);
            var maxBatchSize = int.Parse(_configuration["MaxBatchSize"]);
            var batchSize = new Random(DateTime.Now.Millisecond).Next(minBatchSize, maxBatchSize);
            GenerateBatch(batchSize);
        }

        private void GenerateBatch(int batchSize)
        {
            _logger.LogTrace($"Generating batch of {batchSize} messages.");
            var batchId = Guid.NewGuid().ToString();
            for (int index = 0; index < batchSize; index++)
            {
                _queue.SendMessage(string.Format($"Example Message [{index + 1}/{batchSize}] from batch [{batchId}]"));
            }
        }
    }
}
