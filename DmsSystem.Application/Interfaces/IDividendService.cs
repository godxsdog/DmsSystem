using DmsSystem.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace DmsSystem.Application.Interfaces;

/// <summary>
/// 5A1 配息匯入與計算服務
/// </summary>
public interface IDividendService
{
    Task<DividendImportResult> ImportAsync(IFormFile file);

    Task<DividendConfirmResult> ConfirmAsync(string fundNo, DateOnly dividendDate, string dividendType);
}

