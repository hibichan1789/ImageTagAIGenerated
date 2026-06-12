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
        private readonly IRegisterService _registerService;
        private readonly ILoginService _loginService;

        public AuthController(
            ILogger<AuthController> logger,
            IRegisterService registerService,
            ILoginService loginService
            )
        {
            _logger = logger;
            _registerService = registerService;
            _loginService = loginService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("リクエストが不適切な形式です\n{reqest}", request.ToString());
                return BadRequest(ModelState);
            }

            if (await _registerService.UserExistsAsync(request.Email))
            {
                return BadRequest(new { message = "このメールアドレスは既に登録されています" });
            }

            var registeredUser = await _registerService.RegisterAsync(request.Email, request.Password);

            _logger.LogInformation("email: {email}のユーザーが登録されました", registeredUser.Email);
            return Ok(new
            {
                id = registeredUser.Id,
                email = registeredUser.Email,
                message = "ユーザー登録が完了しました"
            });


        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("リクエストが不適切な形式です\n{reqest}", request.ToString());
                return BadRequest(ModelState);
            }



            var result = await _loginService.LoginAsync(request.Email, request.Password);
            if (result == null)
            {
                return Unauthorized(new { message = "ログインに失敗しました" });
            }

            _logger.LogInformation("email: {email}のログイン成功", request.Email);
            return Ok(result);

        }
    }
}
