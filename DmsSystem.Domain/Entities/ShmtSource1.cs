using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股東會來源資料表1
/// </summary>
public partial class ShmtSource1
{
    /// <summary>
    /// 處理日期字串
    /// </summary>
    public string AcDate { get; set; } = null!;

    public string EmpNo { get; set; } = null!;

    public string StkCd { get; set; } = null!;

    public string? StkName { get; set; }

    public string ShmtDate { get; set; } = null!;

    public string? SsrgDate { get; set; }

    public string? ChfChgYn { get; set; }

    public string? ShmtAddr { get; set; }

    public string? Status { get; set; }

    public string? Des { get; set; }

    public string Type { get; set; } = null!;
}
