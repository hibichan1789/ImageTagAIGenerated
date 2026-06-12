using ImageTagApi.DTOs.Auth;
using ImageTagApi.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageTagApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IRegisterService _registerServices;
        
        public AuthController(ILogger<AuthController> logger, IRegisterService registerServices)
        {
            _logger = logger;
            _registerServices = registerServices;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody]RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(await _registerServices.UserExistsAsync(request.Email))
            {
                return BadRequest(new { message = "このメールアドレスは既に登録されています" });
            }

            var registeredUser = await _registerServices.RegisterAsync(request.Email, request.Password);

            _logger.LogInformation("email: {email}のユーザーが登録されました", registeredUser.Email);
            return Ok(new
            {
                id = registeredUser.Id,
                email = registeredUser.Email,
                message = "ユーザー登録が完了しました"
            });
        }
    }
}
