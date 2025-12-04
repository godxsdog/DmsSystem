using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 委託人基本資料檔
/// </summary>
public partial class Trustee
{
    /// <summary>
    /// 統一編號
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 中文名稱
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 英文名稱
    /// </summary>
    public string? Ename { get; set; }

    /// <summary>
    /// 公司簡稱
    /// </summary>
    public string? Sname { get; set; }

    /// <summary>
    /// 公司簡簡稱
    /// </summary>
    public string? Ssname { get; set; }
}
