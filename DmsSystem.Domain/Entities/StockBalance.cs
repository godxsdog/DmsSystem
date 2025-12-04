using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股票餘額檔
/// </summary>
public partial class StockBalance
{
    /// <summary>
    /// 基金代碼
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 契約序號
    /// </summary>
    public string ContractSeq { get; set; } = null!;

    public DateOnly? ImpDate { get; set; }

    public string? ImpId { get; set; }

    /// <summary>
    /// 記帳日
    /// </summary>
    public DateOnly AcDate { get; set; }

    public string? CurrencyNo { get; set; }

    /// <summary>
    /// 股票代號
    /// </summary>
    public string StockNo { get; set; } = null!;

    /// <summary>
    /// 庫存數量
    /// </summary>
    public decimal? Shares { get; set; }

    public decimal? AvgCost { get; set; }

    public decimal? Cost { get; set; }

    public decimal? Price { get; set; }

    public decimal? MktAmt { get; set; }

    public decimal? NavRate { get; set; }

    public decimal? UnlistBsshare { get; set; }

    public decimal? UnlistSsshare { get; set; }

    public decimal? UnlistOshare { get; set; }

    public decimal? UnlistS1share { get; set; }

    public decimal? UnlistS2share { get; set; }

    public decimal? UnlistRishare { get; set; }

    public decimal? UnlistBoshare { get; set; }

    public decimal? UnlistBishare { get; set; }

    public decimal? UnlistShares { get; set; }

    public decimal? UnlistCdamt { get; set; }

    public decimal? UnstkDiv { get; set; }

    public decimal? GainLost { get; set; }

    public decimal? ApprAmt { get; set; }

    public decimal? GainLostBas { get; set; }

    public string? EmpNo { get; set; }

    public DateOnly? ExcDate { get; set; }

    public string? CfmEmpNo { get; set; }

    public DateOnly? CfmDate { get; set; }

    public string? CfmCd { get; set; }

    /// <summary>
    /// 資產類型
    /// </summary>
    public string AssetType { get; set; } = null!;

    public decimal? AssetCost { get; set; }

    public decimal? AssetValue { get; set; }

    public decimal? ValueCost { get; set; }

    public decimal? CostBas { get; set; }

    public decimal? ApprAmtBas { get; set; }

    public decimal? MktAmtBas { get; set; }

    public decimal? UnlistCdamtBas { get; set; }

    public decimal? UnstkDivBas { get; set; }

    public decimal? ExgRate { get; set; }

    public string? OrgStockNo { get; set; }

    public decimal? PreShares { get; set; }

    public decimal? PreCost { get; set; }

    public decimal? UnsettleShares { get; set; }

    public decimal? NavRateSt { get; set; }
}
