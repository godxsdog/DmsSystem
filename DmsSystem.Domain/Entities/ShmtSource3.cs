using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股東會董監改選資料表
/// </summary>
public partial class ShmtSource3
{
    /// <summary>
    /// 建檔日期
    /// </summary>
    public string AcDate { get; set; } = null!;

    public string EmpNo { get; set; } = null!;

    public string StkCd { get; set; } = null!;

    public string? StkName { get; set; }

    public DateOnly? DataDate { get; set; }
}
