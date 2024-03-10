namespace ExampleService.Services
{
    public interface IAzureStorageQueueService
    {
        public int CountMessages();
        public bool ClearMessages();
        public IEnumerable<string> SendMessages(int batchSize);
        IEnumerable<string> ReceiveMessages(int batchSize);
    }
}
