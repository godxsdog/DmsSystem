namespace DmsSystem.Domain.Entities;

/// <summary>
/// 目標配息率設定表 (MDS.FUND_DIV_OBJ)
/// </summary>
public partial class FundDivObj
{
    /// <summary>
    /// 基金代碼
    /// </summary>
    public string FundNo { get; set; } = null!;

    /// <summary>
    /// 配息頻率 (M/Q/S/Y)
    /// </summary>
    public string DivType { get; set; } = null!;

    /// <summary>
    /// 生效日期
    /// </summary>
    public DateTime TxDate { get; set; }

    /// <summary>
    /// 目標配息率（年化）
    /// </summary>
    public decimal? DivObj { get; set; }

    /// <summary>
    /// 目標配息金額（每單位）
    /// </summary>
    public decimal? DivObjAmt { get; set; }

    public string? CreatedBy { get; set; }
    public DateTime? CreationDate { get; set; }
    public string? RevisedBy { get; set; }
    public DateTime? RevisionDate { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? Status { get; set; }
}
