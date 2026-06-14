using Azure.Storage.Queues;
using ImageTagApi.DTOs.Files;
using System.Text.Json;

namespace ImageTagApi.Services.Queue
{
    public class AzureQueueService:IQueueService
    {
        private readonly ILogger<AzureQueueService> _logger;
        private readonly QueueClient _queueClient;

        public AzureQueueService(ILogger<AzureQueueService> logger, IConfiguration config)
        {
            _logger = logger;

            var connectionString = config["AzureStorage:ConnectionString"];
            var queueName = config["AzureStorage:QueueName"];

            var options = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            _queueClient = new QueueClient(connectionString, queueName, options);
            _queueClient.CreateIfNotExists();
        }

        public async Task SendFileProcessRequestAsync(int fileId)
        {
            var message = JsonSerializer.Serialize(new FileProcessRequest { FileId = fileId });

            _logger.LogInformation("AzureQueueService: fileId={fileId} を Azure Queue に送信します", fileId);

            await _queueClient.SendMessageAsync(message);
        }
    }
}
