using ImageTagApi.Domain.Models;

namespace ImageTagApi.Infrastructure.Repositories
{
    public interface ICssStyleRepository
    {
        Task<CssStyle> GetOrCreateAsync(string bgColor);
    }
}
