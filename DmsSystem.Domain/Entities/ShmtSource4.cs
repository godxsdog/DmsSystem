using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股東會公司基本資料表
/// </summary>
public partial class ShmtSource4
{
    /// <summary>
    /// 建檔日期
    /// </summary>
    public string AcDate { get; set; } = null!;

    public string EmpNo { get; set; } = null!;

    public string StkCd { get; set; } = null!;

    public string? StkName { get; set; }

    public string? CompName { get; set; }

    public string? Tel { get; set; }

    public string? Addr { get; set; }

    public string? BrokerName { get; set; }

    public string? BrokerTel { get; set; }

    public string? Spokesman { get; set; }

    public string? President { get; set; }

    public string? Chairman { get; set; }

    public string? IdNo { get; set; }
}
