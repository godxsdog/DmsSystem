namespace DmsSystem.Domain.Entities;

/// <summary>
/// 可分配收益計算與暫存表 (MDS.FUND_DIV)
/// </summary>
public partial class FundDiv
{
    /// <summary>
    /// 基金代碼
    /// </summary>
    public string FundNo { get; set; } = null!;

    /// <summary>
    /// 配息年度
    /// </summary>
    public int? DividendYear { get; set; }

    /// <summary>
    /// 配息基準日
    /// </summary>
    public DateTime DividendDate { get; set; }

    /// <summary>
    /// 配息頻率 (M=月/Q=季/S=半年/Y=年)
    /// </summary>
    public string DividendType { get; set; } = null!;

    /// <summary>
    /// 付款日期
    /// </summary>
    public DateTime? PayDate { get; set; }

    /// <summary>
    /// 實際付款日期
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// 下次配息日期
    /// </summary>
    public DateTime? NextDividendDate { get; set; }

    /// <summary>
    /// 實際分配月份
    /// </summary>
    public DateTime? ReallyDivDate { get; set; }

    /// <summary>
    /// 基準日總單位數
    /// </summary>
    public decimal? Unit { get; set; }

    /// <summary>
    /// 基準日淨值
    /// </summary>
    public decimal? Nav { get; set; }

    // 期初收益
    public decimal? PreDiv1 { get; set; }
    public decimal? PreDiv1Rate { get; set; }
    public decimal? PreDiv1RateM { get; set; }
    public decimal? PreDiv1Adj { get; set; }
    public decimal? PreDiv1B { get; set; }

    public decimal? PreDiv2 { get; set; }
    public decimal? PreDiv2Rate { get; set; }
    public decimal? PreDiv2RateM { get; set; }
    public decimal? PreDiv2Adj { get; set; }

    public decimal? PreDiv3 { get; set; }
    public decimal? PreDiv3Rate { get; set; }
    public decimal? PreDiv3RateM { get; set; }
    public decimal? PreDiv3Adj { get; set; }

    public decimal? PreDiv4 { get; set; }
    public decimal? PreDiv4Rate { get; set; }
    public decimal? PreDiv4RateM { get; set; }
    public decimal? PreDiv4Adj { get; set; }

    public decimal? PreDiv5 { get; set; }
    public decimal? PreDiv5Rate { get; set; }
    public decimal? PreDiv5RateM { get; set; }
    public decimal? PreDiv5Adj { get; set; }

    // 當期收益
    public decimal? Div1 { get; set; }
    public decimal? Div1Rate { get; set; }
    public decimal? Div1RateM { get; set; }
    public decimal? Div1B { get; set; }

    public decimal? Div2 { get; set; }
    public decimal? Div2Rate { get; set; }
    public decimal? Div2RateM { get; set; }
    public decimal? Div2Adj { get; set; }

    public decimal? Div3 { get; set; }
    public decimal? Div3Rate { get; set; }
    public decimal? Div3RateM { get; set; }
    public decimal? Div3Adj { get; set; }

    public decimal? Div4 { get; set; }
    public decimal? Div4Rate { get; set; }
    public decimal? Div4RateM { get; set; }
    public decimal? Div4Adj { get; set; }

    public decimal? Div5 { get; set; }
    public decimal? Div5Rate { get; set; }
    public decimal? Div5RateM { get; set; }
    public decimal? Div5Adj { get; set; }

    /// <summary>
    /// 費用
    /// </summary>
    public decimal? Fee { get; set; }
    public decimal? FeeRate { get; set; }
    public decimal? FeeRateM { get; set; }

    /// <summary>
    /// 總可分配金額
    /// </summary>
    public decimal? DivTot { get; set; }

    /// <summary>
    /// 每單位可分配收益（年化）
    /// </summary>
    public decimal? DivRate { get; set; }

    /// <summary>
    /// 每單位配息金額（當期）
    /// </summary>
    public decimal? DivRateM { get; set; }

    /// <summary>
    /// 每單位配息金額（原始）
    /// </summary>
    public decimal? DivRateO { get; set; }

    /// <summary>
    /// 本金配息金額
    /// </summary>
    public decimal? Capital { get; set; }

    /// <summary>
    /// 每單位本金配息比率
    /// </summary>
    public decimal? CapitalRate { get; set; }
    public decimal? CapitalRateM { get; set; }
    public decimal? CapitalAdj { get; set; }

    /// <summary>
    /// 目標配息率（年化）
    /// </summary>
    public decimal? DivObj { get; set; }

    /// <summary>
    /// 上期配息率
    /// </summary>
    public decimal? DivPre { get; set; }

    // Step 1 狀態
    public string? Step1CreEmp { get; set; }
    public DateTime? Step1CreTime { get; set; }
    public string? Step1CofEmp { get; set; }
    public DateTime? Step1CofTime { get; set; }
    public string? Step1Status { get; set; }

    // Step 2 狀態
    public string? Step2CreEmp { get; set; }
    public DateTime? Step2CreTime { get; set; }
    public string? Step2CofEmp { get; set; }
    public DateTime? Step2CofTime { get; set; }
    public string? Step2Status { get; set; }

    // Step 3 狀態
    public string? Step3CreEmp { get; set; }
    public DateTime? Step3CreTime { get; set; }
    public string? Step3CofEmp { get; set; }
    public DateTime? Step3CofTime { get; set; }
    public string? Step3Status { get; set; }

    // Step 4 狀態
    public string? Step4CreEmp { get; set; }
    public DateTime? Step4CreTime { get; set; }
    public string? Step4CofEmp { get; set; }
    public DateTime? Step4CofTime { get; set; }
    public string? Step4Status { get; set; }

    // Step 5 狀態
    public string? Step5CreEmp { get; set; }
    public DateTime? Step5CreTime { get; set; }
    public string? Step5CofEmp { get; set; }
    public DateTime? Step5CofTime { get; set; }
    public string? Step5Status { get; set; }

    /// <summary>
    /// 利息比率
    /// </summary>
    public decimal? IRate { get; set; }

    /// <summary>
    /// 本金比率
    /// </summary>
    public decimal? CRate { get; set; }

    public string? Status { get; set; }
    public string? StatusC { get; set; }
    public string? UpdUser { get; set; }
    public DateTime? UpdDate { get; set; }
}
