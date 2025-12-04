using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股票研究員清單表
/// </summary>
public partial class EquitySpList
{
    /// <summary>
    /// 季度
    /// </summary>
    public string QuarterNo { get; set; } = null!;

    public string StkCd { get; set; } = null!;

    public DateOnly? AcDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? IdtName { get; set; }

    public string? StkName { get; set; }

    public string? ResEmpNo { get; set; }

    public string? EmpNo { get; set; }

    public string? UpdUser { get; set; }

    public DateOnly? UpdDate { get; set; }

    public string? InputType { get; set; }
}
