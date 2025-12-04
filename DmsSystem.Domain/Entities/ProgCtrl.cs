using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 程式控制表
/// </summary>
public partial class ProgCtrl
{
    /// <summary>
    /// 應用程式代號
    /// </summary>
    public string ApCode { get; set; } = null!;

    public string MenuId { get; set; } = null!;

    public string ProgCtrlCode { get; set; } = null!;

    public DateOnly? ProgCtrlDate { get; set; }

    public string? ProgCtrlDes { get; set; }

    public string? Flag { get; set; }

    public string? Remark { get; set; }
}
