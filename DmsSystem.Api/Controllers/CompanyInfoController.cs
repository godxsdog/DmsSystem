using DmsSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DmsSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly ICompanyInfoUploadService _excelUploadService;

        public FileUploadController(ICompanyInfoUploadService excelUploadService)
        {
            _excelUploadService = excelUploadService;
        }

        [HttpPost("upload-shmtsource4")]
        public async Task<IActionResult> UploadShmtSource4(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("未上傳任何檔案。");
            }

            // 將上傳檔案的內容讀取為記憶體串流
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0; // 重設串流位置到開頭
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
}