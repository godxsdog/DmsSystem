using DmsSystem.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace DmsSystem.Application.Interfaces;

/// <summary>
/// 5A1 配息匯入與計算服務介面
/// </summary>
public interface IDividendService
{
    /// <summary>
    /// 匯入可分配收益 CSV 檔案
    /// </summary>
    /// <param name="file">CSV 檔案</param>
    /// <returns>匯入結果</returns>
    Task<DividendImportResult> ImportAsync(IFormFile file);

    /// <summary>
    /// 執行配息計算與確認
    /// </summary>
    /// <param name="fundNo">基金代號</param>
    /// <param name="dividendDate">配息基準日</param>
    /// <param name="dividendType">配息頻率（M/Q/S/Y）</param>
    /// <returns>計算結果</returns>
    Task<DividendConfirmResult> ConfirmAsync(string fundNo, DateOnly dividendDate, string dividendType);
}

