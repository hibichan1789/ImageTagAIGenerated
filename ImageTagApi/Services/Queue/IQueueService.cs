namespace ImageTagApi.Services.Queue
{
    public interface IQueueService
    {
        Task SendFileProcessRequestAsync(int fileId);
    }
}
