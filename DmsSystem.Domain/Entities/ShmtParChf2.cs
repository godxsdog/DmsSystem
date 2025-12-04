using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股東會監察人名單表
/// </summary>
public partial class ShmtParChf2
{
    /// <summary>
    /// 股票代號
    /// </summary>
    public string StkCd { get; set; } = null!;

    public DateOnly ShmtDate { get; set; }

    public int Seq { get; set; }

    public string? ChfChgName { get; set; }

    public string? Type1 { get; set; }

    public string? Type2 { get; set; }
}
