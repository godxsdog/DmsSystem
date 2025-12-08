using DmsSystem.Domain.Entities;

namespace DmsSystem.Application.Interfaces;

/// <summary>
/// 目標配息率設定 Repository 介面
/// </summary>
public interface IFundDivObjRepository
{
    /// <summary>
    /// 查詢生效的目標配息率設定（依配息基準日向前回溯最近一筆）
    /// </summary>
    Task<FundDivObj?> GetEffectiveAsync(string fundNo, string divType, DateTime dividendDate);

    /// <summary>
    /// 新增目標配息率設定
    /// </summary>
    Task AddAsync(FundDivObj fundDivObj);

    /// <summary>
    /// 更新目標配息率設定
    /// </summary>
    Task UpdateAsync(FundDivObj fundDivObj);

    /// <summary>
    /// 儲存變更
    /// </summary>
    Task SaveChangesAsync();
}
