using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 員工基本資料表
/// </summary>
public partial class Employee
{
    /// <summary>
    /// 員工代號
    /// </summary>
    public string EmpNo { get; set; } = null!;

    public string? EmpIdNo { get; set; }

    public string? EmpName { get; set; }

    public string? EmpNameEng { get; set; }

    public string? DeptNo { get; set; }

    public string? MangrCode { get; set; }

    public string? EmpCd { get; set; }

    public string? Email { get; set; }

    public string? GroupEmail { get; set; }

    public string? DomainId { get; set; }

    public string? Remark { get; set; }

    public string? InputUser { get; set; }

    public DateOnly? InputDate { get; set; }

    public DateOnly? EntryDate { get; set; }

    public DateOnly? LeaveDate { get; set; }

    public string? Telo { get; set; }

    public string? Address { get; set; }

    public string? Title { get; set; }

    public int? Seq { get; set; }

    public string? ManagrType { get; set; }
}
