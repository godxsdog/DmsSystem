using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 帳戶基本資料檔
/// </summary>
public partial class Contract
{
    /// <summary>
    /// 契約ID
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// 契約序號
    /// </summary>
    public string ContractSeq { get; set; } = null!;

    /// <summary>
    /// 生效日
    /// </summary>
    public DateOnly EffectiveDate { get; set; }

    public string EffectiveCode { get; set; } = null!;

    public string? OldContractSeq { get; set; }

    public DateOnly? MaturityDate { get; set; }

    public DateOnly? ActualMatDate { get; set; }

    public string? MaturityType { get; set; }

    public string? Remark { get; set; }

    public string? OpenEmpNo { get; set; }

    public DateOnly? OpenDate { get; set; }

    public string? EmpNo { get; set; }

    public DateOnly? LastModified { get; set; }

    public DateOnly? PrevDate { get; set; }

    public DateOnly? AcDate { get; set; }

    public string? IsPosted { get; set; }

    public string? IsTrial { get; set; }

    public decimal? TrustAmount { get; set; }

    public decimal? Damages { get; set; }

    public decimal? LostPoint1 { get; set; }

    public decimal? LostPoint2 { get; set; }

    public decimal? LowHoldRate { get; set; }

    public decimal? LowMfRate { get; set; }

    public string? IsReport { get; set; }

    public DateOnly? ReportDate { get; set; }

    public string SerNo { get; set; } = null!;

    public string? IsOpen { get; set; }

    public DateOnly? LastModified1 { get; set; }

    public string? EmpNo1 { get; set; }

    public string? AmortizeType { get; set; }

    public string? HolidayCalculateType { get; set; }

    public string? CalculateType { get; set; }

    public string? RoundType { get; set; }

    public string? AiCode { get; set; }

    public string? GainLostType { get; set; }

    public string? StockCost { get; set; }

    public string? AutoPaymc { get; set; }

    public string? Remark1 { get; set; }

    public string? Name { get; set; }

    public string? IsShowReport { get; set; }

    public string? AssetType { get; set; }

    public string? NavType { get; set; }

    public string? DdIntType { get; set; }

    public string? TaxType { get; set; }

    public string? IsShowReport1 { get; set; }

    public string? AssetType1 { get; set; }

    public string? IsBondSerNo { get; set; }

    public string? RealAccount { get; set; }

    public string? Type { get; set; }

    public string? SName { get; set; }

    public string? McCalculateType { get; set; }

    public string? ToCode { get; set; }

    public string? EName { get; set; }

    public string? CrncyCd { get; set; }

    public string? ExchCrncyCd { get; set; }

    public decimal? ExchRateDate { get; set; }

    public string? ExpHipType { get; set; }

    public string? FtFeeType { get; set; }

    public string? Benchmark { get; set; }

    public string? GbPriceType { get; set; }

    public string? FundPriceType { get; set; }

    public string? FundType { get; set; }

    public string? Ssname { get; set; }

    public string? CalendarCode { get; set; }

    public decimal? SelDays { get; set; }

    public decimal? IntIncludeType { get; set; }

    public string? BondBaseType { get; set; }

    public DateOnly? BondBaseDate { get; set; }

    public string? AbSharesType { get; set; }

    public decimal? NavPoint { get; set; }

    public string? InvType { get; set; }

    public string? SfbFundId { get; set; }

    public string? SfbFundType { get; set; }

    public string? RbType { get; set; }

    public string? JepunId { get; set; }

    public string? SwId { get; set; }

    public string? BShareId { get; set; }

    public string? StepCd { get; set; }

    public DateOnly? CloseDate { get; set; }

    public DateOnly? AccCloseDate { get; set; }

    public string? TaxId { get; set; }

    public string? DmsTrans { get; set; }

    public string? ExpHipAcc { get; set; }

    public decimal? PurDays { get; set; }

    public decimal? SelAccDays { get; set; }

    public decimal? PurAccDays { get; set; }

    public decimal? SelNavDays { get; set; }

    public decimal? PurNavDays { get; set; }

    public string? CalNavType { get; set; }

    public string? RedType { get; set; }

    public string? DivType { get; set; }

    public decimal? SharesPoint { get; set; }

    public string? NavRoundType { get; set; }

    public string? SharesRoundType { get; set; }

    public string? NavAllocateType { get; set; }

    public string? AreaType { get; set; }

    public string? MultiClassType { get; set; }

    public string? AShareId { get; set; }

    public string? OrgNavCrncy { get; set; }

    public decimal? OrgNav { get; set; }

    public string? FundCategory { get; set; }

    public DateOnly? AShareDate { get; set; }

    public string? DivStatus { get; set; }

    public string? DivM { get; set; }

    public string? DivQ { get; set; }

    public string? DivH { get; set; }

    public string? DivY { get; set; }

    public decimal? OrgFaceNav { get; set; }

    public string? MfPayCrncy { get; set; }

    public string? SfPayCrncy { get; set; }

    public string? OverseasFeeType { get; set; }

    public string? CalSharesType { get; set; }

    public string? ExchangeRateType { get; set; }

    public decimal? ExchangeRateSetDate { get; set; }

    public string? ExchangeRateSetTime { get; set; }

    public string? AdvisorType { get; set; }

    public string? DamIlpDepositAccType { get; set; }

    public DateOnly? RedNavChangeDate { get; set; }

    public string? OptionIncomeType { get; set; }

    public string? Adviser { get; set; }

    public string? LifeMcCalType { get; set; }

    public string? SubCustodianNo { get; set; }

    public string? AutoFx { get; set; }

    public string? AutoFxCrncy { get; set; }

    public string? NavPublish { get; set; }

    public string? SubpType { get; set; }

    public string? AccountType { get; set; }

    public DateOnly? ValueDate { get; set; }

    public string? CompMfType { get; set; }

    public string? CompContractSeq { get; set; }

    public string? DivInvType { get; set; }

    public decimal? DivInvNav { get; set; }

    public string? ExpAttrType { get; set; }

    public string? FpiAccountNo { get; set; }

    public string? FpiGlobalBank { get; set; }

    public string? FpiLocalBank { get; set; }

    public DateOnly? OrgNavDate { get; set; }

    public string? OutsourceType { get; set; }

    public DateOnly? OutsourceDate { get; set; }

    public string? SeparateFee { get; set; }

    public DateOnly? SeparateFeeDate { get; set; }

    public string? BenchmarkType { get; set; }

    public string? InvOutsourceType { get; set; }

    public DateOnly? InvOutsourceDate { get; set; }

    public string? AladdinId { get; set; }

    public string? JpMorganId { get; set; }

    public string? JpMorganBic { get; set; }
}
