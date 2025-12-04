// 檔案: DmsSystem.Api\Controllers\StockBalanceController.cs
using DmsSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace DmsSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockBalanceController : ControllerBase
    {
        private readonly IStockBalanceUploadService _uploadService;
        public StockBalanceController(IStockBalanceUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("未上傳檔案。");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                var result = await _uploadService.ProcessUploadAsync(stream, file.FileName);

                if (result.Success) return Ok(result);
                else return BadRequest(result); // 回傳包含失敗訊息的 400 錯誤
            }
        }
    }
}