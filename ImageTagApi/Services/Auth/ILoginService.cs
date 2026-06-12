using ImageTagApi.DTOs.Auth;

namespace ImageTagApi.Services.Auth
{
    public interface ILoginService
    {
        Task<LoginResponse> LoginAsync(string email, string password);
    }
}
