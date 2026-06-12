using ImageTagApi.Context;
using ImageTagApi.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ImageTagApi.Services.Auth
{
    public class RegisterService:IRegisterService
    {
        private readonly ILogger<RegisterService> _logger;
        private readonly MyContext _db;
        private readonly PasswordHasher<User> _passwordHasher; 
        
        public RegisterService(ILogger<RegisterService> logger, MyContext db)
        {
            _logger = logger;
            _db = db;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            bool isExist = await _db.Users.AnyAsync(u => u.Email == email);

            if (isExist)
            {
                _logger.LogWarning("email: {email}はすでに存在しています", email);
            }
            return isExist;
        }

        public async Task<User> RegisterAsync(string email, string password)
        {
            var user = new User
            {
                Email = email,
                CreatedAt = DateTimeOffset.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _db.Add(user);
            await _db.SaveChangesAsync();

            return user;
        }
    }
}
