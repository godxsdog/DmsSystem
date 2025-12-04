using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

public partial class ShmtParHolderAll
{
    /// <summary>
    /// 類型
    /// </summary>
    public string Type { get; set; } = null!;

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
