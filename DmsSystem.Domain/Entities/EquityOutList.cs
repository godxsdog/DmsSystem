using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

public partial class EquityOutList
{
    /// <summary>
    /// 股票代號
    /// </summary>
    public string StkCd { get; set; } = null!;

    public string? Name { get; set; }

    public string? CompName { get; set; }

    public string? UpdUser { get; set; }

    public DateOnly? UpdDate { get; set; }

    public int? ListCount { get; set; }
}
