using DmsSystem.Domain.Entities;

namespace DmsSystem.Application.Interfaces;

/// <summary>
    /// 配息參數設定 Repository 介面
    /// </summary>
public interface IFundDivSetRepository
{
    /// <summary>
    /// 根據基金代碼和配息頻率查詢配息參數設定
    /// </summary>
    Task<FundDivSet?> GetByFundNoAndDivTypeAsync(string fundNo, string divType);

    /// <summary>
    /// 新增配息參數設定
    /// </summary>
    Task AddAsync(FundDivSet fundDivSet);

    /// <summary>
    /// 更新配息參數設定
    /// </summary>
    Task UpdateAsync(FundDivSet fundDivSet);

    /// <summary>
    /// 儲存變更
    /// </summary>
    Task SaveChangesAsync();
}
