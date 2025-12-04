using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 檔案轉換參數表
/// </summary>
public partial class FileCvPar
{
    /// <summary>
    /// 應用程式代號
    /// </summary>
    public string ApCode { get; set; } = null!;

    public string MenuId { get; set; } = null!;

    public int SourceSeq { get; set; }

    public string? ConverterType { get; set; }

    public string? FileType { get; set; }

    public string? SheetName { get; set; }

    public int? RecsLength { get; set; }

    public string? DefaultFileName { get; set; }

    public string? DefaultPath { get; set; }

    public int? ColumnsCount { get; set; }

    public string? TitleType { get; set; }

    public int? RowsType { get; set; }

    public string? OwnerName { get; set; }

    public string? TableName { get; set; }

    public string? SeparateCode { get; set; }
}
