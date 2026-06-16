
using ImageTagApi.DTOs.Files;
using ImageTagApi.Services.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ImageTagApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IFileService _fileService;
        public FilesController(ILogger<FilesController> logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FileListItemResponse>>> GetFiles()
        {
            _logger.LogInformation("GET: api/files");

            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userClaim == null)
            {
                return Unauthorized(new { message = "ユーザー情報が取得できません" });
            }

            int userId = int.Parse(userClaim.Value);

            var files = await _fileService.GetFilesByUserIdAsync(userId);

            return Ok(files);
        }

        [HttpPost("upload")]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            _logger.LogInformation("POST: api/files/upload");
            if(file == null)
            {
                _logger.LogWarning("ファイルがありません");
                return BadRequest(new { message = "ファイルがありません" });
            }

            _logger.LogInformation("JWTからuserIdの取得をします");
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userClaim == null)
            {
                _logger.LogWarning("ユーザー情報が取得できません");
                // フロント側でログイン画面に戻す処理を入れたらいいかもしれない
                return Unauthorized(new { message = "ユーザー情報が取得できません" });
            }

            int userId = int.Parse(userClaim.Value);

            try
            {
                var result = await _fileService.UploadAsync(file, userId);
                return Ok(result);
            }
            catch(ArgumentException ex)
            {
                _logger.LogWarning(ex.StackTrace);
                return BadRequest(new {message = ex.Message});
            }
            catch(Exception ex)
            {
                _logger.LogWarning(ex.StackTrace);
                return Problem("サーバーエラー");
            }
        }
    }
}
