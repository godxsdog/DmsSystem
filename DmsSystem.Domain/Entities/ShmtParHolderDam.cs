using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股東會DAM持股明細表
/// </summary>
public partial class ShmtParHolderDam
{
    /// <summary>
    /// 股票代號
    /// </summary>
    public string StkCd { get; set; } = null!;

    public DateOnly ShmtDate { get; set; }

    public string Id { get; set; } = null!;

    public string ContractSeq { get; set; } = null!;

    public DateOnly? AcDate { get; set; }

    public decimal? Shares { get; set; }

    public string? UpdUser { get; set; }

    public DateOnly? UpdDate { get; set; }

    public string? InputType { get; set; }
}
