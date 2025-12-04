using DmsSystem.Infrastructure.Services; // 引用 IReportService
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DmsSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        // 注入 Application 層的 IReportService
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("shareholder-excel")] // API 路徑: GET /api/reports/shareholder-excel
        public async Task<IActionResult> GetShareholderReportExcel()
        {
            try
            {
                byte[] fileBytes = await _reportService.GenerateShareholderReportExcelAsync();

                if (fileBytes == null || fileBytes.Length == 0)
                {
                    return NotFound("查無資料可匯出。");
                }

                string fileName = $"ShareholderReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // 回傳檔案
                return File(fileBytes, mimeType, fileName);
            }
            catch (Exception ex)
            {
                // 應加入 Log
                return StatusCode(500, $"產生 Excel 報表時發生內部錯誤: {ex.Message}");
            }
        }
    }
}