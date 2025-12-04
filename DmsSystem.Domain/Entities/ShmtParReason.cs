using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股東會原因說明表2
/// </summary>
public partial class ShmtParReason
{
    /// <summary>
    /// 股票代號
    /// </summary>
    public string StkCd { get; set; } = null!;

    public DateOnly ShmtDate { get; set; }

    public int Seq { get; set; }

    public string? Des { get; set; }
}
