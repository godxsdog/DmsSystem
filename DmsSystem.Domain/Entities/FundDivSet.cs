namespace DmsSystem.Domain.Entities;

/// <summary>
/// 配息參數設定表 (MDS.FUND_DIV_SET)
/// </summary>
public partial class FundDivSet
{
    /// <summary>
    /// 基金代碼
    /// </summary>
    public string FundNo { get; set; } = null!;

    /// <summary>
    /// 配息頻率 (M/Q/S/Y)
    /// </summary>
    public string DivType { get; set; } = null!;

    // 分攤順序 (1-10)
    public int? Item01Seq { get; set; }
    public int? Item01SeqAdj { get; set; }
    public string? Item01Type { get; set; }

    public int? Item02Seq { get; set; }
    public int? Item02SeqAdj { get; set; }
    public string? Item02Type { get; set; }

    public int? Item03Seq { get; set; }
    public int? Item03SeqAdj { get; set; }
    public string? Item03Type { get; set; }

    public int? Item04Seq { get; set; }
    public int? Item04SeqAdj { get; set; }
    public string? Item04Type { get; set; }

    public int? Item05Seq { get; set; }
    public int? Item05SeqAdj { get; set; }
    public string? Item05Type { get; set; }

    public int? Item06Seq { get; set; }
    public int? Item06SeqAdj { get; set; }
    public string? Item06Type { get; set; }

    public int? Item07Seq { get; set; }
    public int? Item07SeqAdj { get; set; }
    public string? Item07Type { get; set; }

    public int? Item08Seq { get; set; }
    public int? Item08SeqAdj { get; set; }
    public string? Item08Type { get; set; }

    public int? Item09Seq { get; set; }
    public int? Item09SeqAdj { get; set; }
    public string? Item09Type { get; set; }

    public int? Item10Seq { get; set; }
    public int? Item10SeqAdj { get; set; }
    public string? Item10Type { get; set; }

    /// <summary>
    /// 是否有費用扣除 (Y/N)
    /// </summary>
    public string? FeeType { get; set; }

    /// <summary>
    /// 是否允許分配本金 (Y/N)
    /// </summary>
    public string? CapitalType { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisionDate { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? Status { get; set; }

    /// <summary>
    /// Email 清單
    /// </summary>
    public string? EmailList { get; set; }

    public DateTime? FirstDividendDate { get; set; }
    public int? FirstDividendCount { get; set; }
}
