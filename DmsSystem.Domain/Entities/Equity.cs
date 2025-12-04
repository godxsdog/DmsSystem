using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股票基本資料表
/// </summary>
public partial class Equity
{
    [DisplayName("股票代號")]
    public string StkCd { get; set; } = null!;
    [DisplayName("中文名稱")]
    public string? Name { get; set; }
    [DisplayName("中文簡稱")]
    public string? ShName { get; set; }
    [DisplayName("英文名稱")]
    public string? Engnm { get; set; }

    public string? ShEngnm { get; set; }

    public string? SedolCode { get; set; }

    public string? TCode { get; set; }

    public string? IsinCode { get; set; }

    public string? CrtsCode { get; set; }

    public string? CountryCd { get; set; }

    public string? CrncyCd { get; set; }

    /// <summary>
    /// 交易所代碼
    /// </summary>
    public string DealCtr { get; set; } = null!;

    public string? StkType { get; set; }

    public DateOnly? GtsalDate { get; set; }

    public long? OsStk { get; set; }

    public decimal? CptlAmt { get; set; }

    public string? IdtType { get; set; }

    public string? IdNo { get; set; }

    public string? ComTelNo { get; set; }

    public string? StkSevId { get; set; }

    public string? StkSevCd { get; set; }

    public string? StkSevBranch { get; set; }

    public string? StkSevTel { get; set; }

    public string? TrnStkNo { get; set; }

    public DateOnly? TrnStkDt { get; set; }

    public string? WeitYn { get; set; }

    public string? TrnDealCtr { get; set; }

    public DateOnly? CloseDate { get; set; }

    public string? UpdUser { get; set; }

    public DateOnly? UpdDate { get; set; }

    public string? CfmUser { get; set; }

    public DateOnly? CfmDate { get; set; }

    public string? CfmCd { get; set; }

    public string? CrdUser { get; set; }

    public DateOnly? CrdDate { get; set; }

    public string? Comm { get; set; }

    public string? IdtTypeSp { get; set; }

    public string? StkChnm { get; set; }

    public string? Symbol { get; set; }

    public string? CrtsId { get; set; }

    public string? CompName { get; set; }

    public string? IdtTypeBloomberg { get; set; }

    public string? OrgStkCd { get; set; }

    public string? OldStkCd { get; set; }

    public string? IssueType { get; set; }

    public string? IssueBroker { get; set; }

    public string? IssueBranch { get; set; }

    public string? ProfType { get; set; }

    public string? CusipCode { get; set; }

    public string? EtfBankNo { get; set; }

    public string? EtfBankBranch { get; set; }

    public string? EtfAccountNo { get; set; }

    public string? EtfAccountName { get; set; }
}
