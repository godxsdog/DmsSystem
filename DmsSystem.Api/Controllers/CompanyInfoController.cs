using DmsSystem.Application.Interfaces;
using DmsSystem.Api.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DmsSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyInfoController : ControllerBase
    {
        private readonly ICompanyInfoUploadService _excelUploadService;
        private readonly IValidator<IFormFile> _fileValidator;

        public CompanyInfoController(
            ICompanyInfoUploadService excelUploadService,
            IValidator<IFormFile> fileValidator)
        {
            _excelUploadService = excelUploadService;
            _fileValidator = fileValidator;
        }

        [HttpPost("upload-shmtsource4")]
        public async Task<IActionResult> UploadShmtSource4(IFormFile file)
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
            var (success, message, rowsAdded) = await _excelUploadService.ProcessShmtSource4UploadAsync(stream, file.FileName);

            if (success)
            {
                return Ok(new { Message = message, RowsAdded = rowsAdded });
            }
            else
            {
                return StatusCode(500, new { Message = message });
            }
        }
    }
}