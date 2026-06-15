using ImageTagApi.Context;
using ImageTagApi.Domain.Models;
using ImageTagApi.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace ImageTagApi.Services.Auth
{
    public class LoginService:ILoginService
    {
        private readonly ILogger<LoginService> _logger;
        private readonly MyContext _db;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _passwordHasher;

        public LoginService(ILogger<LoginService> logger, MyContext db, IConfiguration config)
        {
            _logger = logger;
            _db = db;
            _config = config;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user == null)
            {
                _logger.LogWarning("email: {email}のユーザーは存在しません", email);
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if(result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("passwordエラー");
                return null;
            }

            _logger.LogInformation("email: {email}のユーザーの認証成功",email);
            DateTime expiredAt = DateTime.UtcNow.AddMinutes(30);
            string token = GenerateJwtToken(user, expiredAt);

            return new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token,
                ExpiredAt = expiredAt
            };
        }

        private string GenerateJwtToken(User user, DateTime expiredAt)
        {
            // JWTの書名に使う秘密鍵
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email.ToString())
            };

            var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"]!,
                    audience: _config["Jwt:Audience"]!,
                    claims: claims,
                    expires: expiredAt,
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
