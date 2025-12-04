using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// HSBC基金主檔表
/// </summary>
public partial class HsbcFundMaster
{
    /// <summary>
    /// 基金代號
    /// </summary>
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? Ename { get; set; }

    public string? SitcaPcaId { get; set; }

    public string? SfbFundId { get; set; }

    public string? JepunId { get; set; }

    public string? FasId { get; set; }

    public string? FacId { get; set; }

    public string? CrtsId { get; set; }

    public string? EdwId { get; set; }

    public string? HiportId { get; set; }

    public string? CalendarCode { get; set; }

    public string? ExpHipType { get; set; }
}
