using DmsSystem.Application.Interfaces;
using DmsSystem.Api.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DmsSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockBalanceController : ControllerBase
    {
        private readonly IStockBalanceUploadService _uploadService;
        private readonly IValidator<IFormFile> _fileValidator;

        public StockBalanceController(
            IStockBalanceUploadService uploadService,
            IValidator<IFormFile> fileValidator)
        {
            _uploadService = uploadService;
            _fileValidator = fileValidator;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
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
            var result = await _uploadService.ProcessUploadAsync(stream, file.FileName);

            if (result.Success) return Ok(result);
            else return BadRequest(result);
        }
    }
}