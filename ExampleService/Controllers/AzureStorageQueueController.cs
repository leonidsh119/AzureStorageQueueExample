using ExampleService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExampleService.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AzureStorageQueueController(IAzureStorageQueueService azureStorageQueueService, ILogger<AzureStorageQueueController> logger)
    {
        private readonly IAzureStorageQueueService _azureStorageQueueService = azureStorageQueueService;
        private readonly ILogger<AzureStorageQueueController> _logger = logger;

        [HttpGet(Name = "CountMessages")]
        public int CountMessages()
        {
            _logger.LogTrace("Counting peekable messages in Azure Storage Queue.");
            return _azureStorageQueueService.CountMessages();
        }

        [HttpDelete(Name = "CleanMessages")]
        public int CleanMessages()
        {
            _logger.LogTrace("Emptying queue.");
            return _azureStorageQueueService.CountMessages();
        }

        [HttpPost(Name = "PushMessages")]
        public IEnumerable<string> SendMessages(int batchSize = 5)
        {
            _logger.LogTrace("Generating a batch of [{}] messages.", batchSize);
            return _azureStorageQueueService.SendMessages(batchSize);
        }

        [HttpGet(Name = "PopMessages")]
        public IEnumerable<string> PopMessages(int batchSize = 5)
        {
            _logger.LogTrace("Receiving a batch of [{}] messages.", batchSize);
            return _azureStorageQueueService.ReceiveMessages(batchSize);
        }
    }
}
