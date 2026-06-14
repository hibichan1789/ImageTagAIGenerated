using ImageTagApi.Domain.Models;

namespace ImageTagApi.Infrastructure.Repositories
{
    public interface ITagRepository
    {
        Task<Tag> GetOrCreateAsync(string value);
    }
}
