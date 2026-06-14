using ImageTagApi.Domain.Enums;
using ImageTagApi.Infrastructure.Repositories;
using ImageTagApi.Services.Ai;
using ImageTagApi.Services.Queue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ImageTagApi.Controllers
{
    [Route("api/files")]
    [ApiController]
    [Authorize]
    public class FileProcessingController : ControllerBase
    {
        private readonly ILogger<FileProcessingController> _logger;
        private readonly IFileRepository _fileRepo;
        private readonly ITagRepository _tagRepo;
        private readonly ICssStyleRepository _cssStyleRepo;
        private readonly IFileTagRepository _fileTagRepo;
        private readonly IAiFunctionService _aiFunctionService;
        private readonly IQueueService _queueService;

        public FileProcessingController(
            ILogger<FileProcessingController> logger,
            IFileRepository fileRepo,
            ITagRepository tagRepo,
            ICssStyleRepository cssStyleRepo,
            IFileTagRepository fileTagRepo,
            IAiFunctionService aiFunctionService,
            IQueueService queueService
            )
        {
            _logger = logger;
            _fileRepo = fileRepo;
            _tagRepo = tagRepo;
            _cssStyleRepo = cssStyleRepo;
            _fileTagRepo = fileTagRepo;
            _aiFunctionService = aiFunctionService;
            _queueService = queueService;
        }

        // タグ生成
        [HttpPost("{id}/generate-tags")]
        public async Task<ActionResult> GenerateTags(int id)
        {
            var file = await _fileRepo.GetByIdAsync(id);
            if(file == null)
            {
                return NotFound(new { message = "ファイルが存在しません" });
            }

            if (file.Status != FileStatus.ReadyForTag)
            {
                return BadRequest(new { message = "タグ生成できる状態ではありません" });
            }

            // Azure Functions HttpTrigger → 外部AIサービス
            var aiResult = await _aiFunctionService.GenerateTagAsync(file.AiProcessedFileUrl);
            if(aiResult == null)
            {
                // 一応DBのStatusをErrorにしておく
                await _fileRepo.UpdateStatusAsync(id, FileStatus.Error);
                return BadRequest(new {message="AIタグ生成に失敗しました"});
            }

            // DB保存
            foreach(var item in  aiResult.Items)
            {
                var tag = await _tagRepo.GetOrCreateAsync(item.Tag);
                var style = await _cssStyleRepo.GetOrCreateAsync(item.BgColor);

                await _fileTagRepo.AddAsync(file.Id, tag.Id, style.Id);
            }

            // ステータス更新
            await _fileRepo.UpdateStatusAsync(file.Id, FileStatus.Completed);
            _logger.LogInformation("AIの生成が完了しました");
            return Ok(aiResult);
        }

        [HttpPost("{id}/retry")]
        public async Task<ActionResult> Retry(int id)
        {
            var file = await _fileRepo.GetByIdAsync(id);
            if(file == null)
            {
                return NotFound(new { message = "ファイルが存在しません" });
            }

            if(file.Status != FileStatus.Error)
            {
                return BadRequest(new { message = "エラー状態のみ再処理できます" });
            }

            
            await _queueService.SendFileProcessRequestAsync(file.Id);

            await _fileRepo.UpdateStatusAsync(id, FileStatus.Processing);
            return Ok(new {message="再処理を開始しました", fileId=file.Id});
        }
    }
}
