using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DmsSystem.Application.Services;

/// <summary>
/// 報表產生服務實作，提供股東會報表資料查詢與 Excel 產生功能
/// </summary>
public class ReportService : IReportService
{
    private readonly IShareholderReportRepository _reportRepo;
    private readonly IExcelGenerator _excelGenerator;

    public ReportService(
        IShareholderReportRepository reportRepo,
        IExcelGenerator excelGenerator)
    {
        _reportRepo = reportRepo;
        _excelGenerator = excelGenerator;
    }

    public async Task<List<ShareholderReportDto>> GetShareholderReportDataAsync()
    {
        var data = await _reportRepo.GetReportDataAsync();
        return data.ToList();
    }

    public MemoryStream GenerateShareholderReportExcel(List<ShareholderReportDto> data)
    {
        var fileBytes = _excelGenerator.GenerateExcel(data, "ShareholderReport");
        return new MemoryStream(fileBytes);
    }
}

