using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using System.Threading.Tasks;

namespace DmsSystem.Infrastructure.Services
{
    // 定義介面
    public interface IReportService
    {
        Task<byte[]> GenerateShareholderReportExcelAsync();
    }

    // 實作介面
    public class ReportService : IReportService
    {
        private readonly IShareholderReportRepository _reportRepo;
        private readonly IExcelGenerator _excelGenerator;

        // 透過 DI 注入「合約」
        public ReportService(IShareholderReportRepository reportRepo, IExcelGenerator excelGenerator)
        {
            _reportRepo = reportRepo;
            _excelGenerator = excelGenerator;
        }

        public async Task<byte[]> GenerateShareholderReportExcelAsync()
        {
            // 1. 呼叫 Repository 取得資料
            var data = await _reportRepo.GetReportDataAsync();

            // 2. 呼叫 Generator 產出 Excel
            // (您可以在這裡添加檢查 data 是否為 null 或 empty)
            var fileBytes = _excelGenerator.GenerateExcel(data, "ShareholderReport");

            // 3. 回傳 Excel 的 byte[]
            return fileBytes;
        }
    }
}