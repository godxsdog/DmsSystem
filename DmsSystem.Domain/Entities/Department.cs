using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 部門資料表
/// </summary>
public partial class Department
{
    /// <summary>
    /// 部門代號
    /// </summary>
    public string DeptNo { get; set; } = null!;

    public string? DeptChName { get; set; }

    public string? DeptEnName { get; set; }

    public string? DeptShNm { get; set; }

    public string? DeptMgr { get; set; }

    public string? InputUser { get; set; }

    public DateOnly? InputDate { get; set; }

    public string? Category { get; set; }
}
