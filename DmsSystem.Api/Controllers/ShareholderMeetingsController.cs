using DmsSystem.Application.Interfaces;
using DmsSystem.Api.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DmsSystem.Api.Controllers
{
    /// <summary>
    /// 管理所有與「股東會」相關的 API 端點。
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ShareholderMeetingsController : ControllerBase
    {
        private readonly IShareholderMeetingDetailService _detailService;
        private readonly IValidator<IFormFile> _fileValidator;

        public ShareholderMeetingsController(
            IShareholderMeetingDetailService detailService,
            IValidator<IFormFile> fileValidator)
        {
            _detailService = detailService;
            _fileValidator = fileValidator;
        }

        /// <summary>
        /// 上傳「股東會明細」的 Excel 或 CSV 檔案，並將資料寫入 ris.shmtsource1 資料表。
        /// </summary>
        [HttpPost("upload-shmtsource1")]
        public async Task<IActionResult> UploadDetails(IFormFile file)
        {
            // 使用 FluentValidation 驗證檔案
            var validationResult = await _fileValidator.ValidateAsync(file);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { errors = validationResult.Errors.Select(e => e.ErrorMessage) });
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;
            var result = await _detailService.ProcessUploadAsync(stream, file.FileName);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return StatusCode(500, result);
            }
        }
    }
}