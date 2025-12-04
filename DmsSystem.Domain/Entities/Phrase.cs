using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 片語資料表
/// </summary>
public partial class Phrase
{
    /// <summary>
    /// 片語類別
    /// </summary>
    public string PhraseType { get; set; } = null!;

    public string? PhraseDes { get; set; }
}
