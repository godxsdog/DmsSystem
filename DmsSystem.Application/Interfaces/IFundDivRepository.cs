using DmsSystem.Domain.Entities;

namespace DmsSystem.Application.Interfaces;

/// <summary>
/// 可分配收益資料 Repository 介面
/// </summary>
public interface IFundDivRepository
{
    /// <summary>
    /// 根據主鍵查詢配息資料
    /// </summary>
    Task<FundDiv?> GetByKeyAsync(string fundNo, DateTime dividendDate, string dividendType);

    /// <summary>
    /// 新增配息資料
    /// </summary>
    Task AddAsync(FundDiv fundDiv);

    /// <summary>
    /// 更新配息資料
    /// </summary>
    Task UpdateAsync(FundDiv fundDiv);

    /// <summary>
    /// 查詢上期配息資料
    /// </summary>
    Task<FundDiv?> GetPreviousAsync(string fundNo, string dividendType, DateTime dividendDate);
}
