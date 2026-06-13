
using Azure.Storage.Queues;
using System.Text.Json;
using ImageTagApi.DTOs.Files;

namespace ImageTagApi.Services.Queue
{
    public class LocalQueueService : IQueueService
    {
        private readonly ILogger<LocalQueueService> _logger;
        private readonly QueueClient _queueClient;

        public LocalQueueService(
            ILogger<LocalQueueService> logger,
            IConfiguration config
            )
        {
            _logger = logger;
            var connectionString = config["LocalAzureStorage:ConnectionString"]!;
            var queueName = config["LocalAzureStorage:QueueName"];
            
            var options = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };
            _queueClient = new QueueClient(connectionString, queueName, options);
            _queueClient.CreateIfNotExists();
        }

        public async Task SendFileProcessRequestAsync(int fileId)
        {
            var message = JsonSerializer.Serialize(new FileProcessRequest {FileId = fileId});

            _logger.LogInformation("LocalQueueService: fileId={fileId}をAzure Queueに送信します", fileId);

            await _queueClient.SendMessageAsync(message);
        }
    }
}
