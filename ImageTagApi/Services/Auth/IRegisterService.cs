using ImageTagApi.Domain.Models;

namespace ImageTagApi.Services.Auth
{
    public interface IRegisterService
    {
        Task<bool> UserExistsAsync(string email);
        Task<User> RegisterAsync(string email, string password);
    }
}
