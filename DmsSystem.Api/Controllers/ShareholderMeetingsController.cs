using DmsSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

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

        public ShareholderMeetingsController(IShareholderMeetingDetailService detailService)
        {
            _detailService = detailService;
        }

        /// <summary>
        /// 上傳「股東會明細」的 Excel 或 CSV 檔案，並將資料寫入 ris.shmtsource1 資料表。
        /// </summary>
        [HttpPost("upload-shmtsource1")]
        public async Task<IActionResult> UploadDetails(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("未上傳任何檔案。");
            }

            using (var stream = new MemoryStream())
            {
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
}