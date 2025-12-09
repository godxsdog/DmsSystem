using System;
using System.Collections.Generic;
using DmsSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DmsSystem.Infrastructure.Persistence.Contexts;

public partial class DmsDbContext : DbContext
{
    public DmsDbContext()
    {
    }

    public DmsDbContext(DbContextOptions<DmsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Equity> Equities { get; set; }

    public virtual DbSet<EquityOutList> EquityOutLists { get; set; }

    public virtual DbSet<EquitySpList> EquitySpLists { get; set; }

    public virtual DbSet<FileCvPar> FileCvPars { get; set; }

    public virtual DbSet<HsbcFundMaster> HsbcFundMasters { get; set; }

    public virtual DbSet<Phrase> Phrases { get; set; }

    public virtual DbSet<ProgCtrl> ProgCtrls { get; set; }

    public virtual DbSet<ShmtOutSource1> ShmtOutSource1s { get; set; }

    public virtual DbSet<ShmtPar> ShmtPars { get; set; }

    public virtual DbSet<ShmtParChf1> ShmtParChf1s { get; set; }

    public virtual DbSet<ShmtParChf2> ShmtParChf2s { get; set; }

    public virtual DbSet<ShmtParDe> ShmtParDes { get; set; }

    public virtual DbSet<ShmtParHolder> ShmtParHolders { get; set; }

    public virtual DbSet<ShmtParHolderAll> ShmtParHolderAlls { get; set; }

    public virtual DbSet<ShmtParHolderDam> ShmtParHolderDams { get; set; }

    public virtual DbSet<ShmtParReason> ShmtParReasons { get; set; }

    public virtual DbSet<ShmtSource1> ShmtSource1s { get; set; }

    public virtual DbSet<ShmtSource2> ShmtSource2s { get; set; }

    public virtual DbSet<ShmtSource3> ShmtSource3s { get; set; }

    public virtual DbSet<ShmtSource4> ShmtSource4s { get; set; }

    public virtual DbSet<StockBalance> StockBalances { get; set; }

    public virtual DbSet<Trustee> Trustees { get; set; }

    public virtual DbSet<FundDiv> FundDivs { get; set; }

    public virtual DbSet<FundDivSet> FundDivSets { get; set; }

    public virtual DbSet<FundDivObj> FundDivObjs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.ContractSeq });

            entity.ToTable("CONTRACT", "DMS", tb => tb.HasComment("帳戶基本資料檔"));

            entity.HasIndex(e => e.SerNo, "UK_CONTRACT_SER_NO").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasComment("契約ID")
                .HasColumnName("ID");
            entity.Property(e => e.ContractSeq)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasComment("契約序號")
                .HasColumnName("CONTRACT_SEQ");
            entity.Property(e => e.AShareDate).HasColumnName("A_SHARE_DATE");
            entity.Property(e => e.AShareId)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("A_SHARE_ID");
            entity.Property(e => e.AbSharesType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("AB_SHARES_TYPE");
            entity.Property(e => e.AcDate).HasColumnName("AC_DATE");
            entity.Property(e => e.AccCloseDate).HasColumnName("ACC_CLOSE_DATE");
            entity.Property(e => e.AccountType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("G")
                .HasColumnName("ACCOUNT_TYPE");
            entity.Property(e => e.ActualMatDate).HasColumnName("ACTUAL_MAT_DATE");
            entity.Property(e => e.Adviser)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("PPMS")
                .HasColumnName("ADVISER");
            entity.Property(e => e.AdvisorType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ADVISOR_TYPE");
            entity.Property(e => e.AiCode)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("AI_CODE");
            entity.Property(e => e.AladdinId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ALADDIN_ID");
            entity.Property(e => e.AmortizeType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AMORTIZE_TYPE");
            entity.Property(e => e.AreaType)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasDefaultValue("TW")
                .HasColumnName("AREA_TYPE");
            entity.Property(e => e.AssetType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASSET_TYPE");
            entity.Property(e => e.AssetType1)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ASSET_TYPE1");
            entity.Property(e => e.AutoFx)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("AUTO_FX");
            entity.Property(e => e.AutoFxCrncy)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("AUTO_FX_CRNCY");
            entity.Property(e => e.AutoPaymc)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("AUTO_PAYMC");
            entity.Property(e => e.BShareId)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("B_SHARE_ID");
            entity.Property(e => e.Benchmark)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("BENCHMARK");
            entity.Property(e => e.BenchmarkType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("BENCHMARK_TYPE");
            entity.Property(e => e.BondBaseDate).HasColumnName("BOND_BASE_DATE");
            entity.Property(e => e.BondBaseType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("BOND_BASE_TYPE");
            entity.Property(e => e.CalNavType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("CAL_NAV_TYPE");
            entity.Property(e => e.CalSharesType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("CAL_SHARES_TYPE");
            entity.Property(e => e.CalculateType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CALCULATE_TYPE");
            entity.Property(e => e.CalendarCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("E")
                .HasColumnName("CALENDAR_CODE");
            entity.Property(e => e.CloseDate).HasColumnName("CLOSE_DATE");
            entity.Property(e => e.CompContractSeq)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("COMP_CONTRACT_SEQ");
            entity.Property(e => e.CompMfType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("COMP_MF_TYPE");
            entity.Property(e => e.CrncyCd)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("NTD")
                .HasColumnName("CRNCY_CD");
            entity.Property(e => e.DamIlpDepositAccType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .HasColumnName("DAM_ILP_DEPOSIT_ACC_TYPE");
            entity.Property(e => e.Damages)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("DAMAGES");
            entity.Property(e => e.DdIntType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("DD_INT_TYPE");
            entity.Property(e => e.DivH)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("DIV_H");
            entity.Property(e => e.DivInvNav)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(14, 8)")
                .HasColumnName("DIV_INV_NAV");
            entity.Property(e => e.DivInvType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("DIV_INV_TYPE");
            entity.Property(e => e.DivM)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("DIV_M");
            entity.Property(e => e.DivQ)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("DIV_Q");
            entity.Property(e => e.DivStatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("DIV_STATUS");
            entity.Property(e => e.DivType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("DIV_TYPE");
            entity.Property(e => e.DivY)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("DIV_Y");
            entity.Property(e => e.DmsTrans)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .HasColumnName("DMS_TRANS");
            entity.Property(e => e.EName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("E_NAME");
            entity.Property(e => e.EffectiveCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("EFFECTIVE_CODE");
            entity.Property(e => e.EffectiveDate)
                .HasComment("生效日")
                .HasColumnName("EFFECTIVE_DATE");
            entity.Property(e => e.EmpNo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("EMP_NO");
            entity.Property(e => e.EmpNo1)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("EMP_NO1");
            entity.Property(e => e.ExchCrncyCd)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("EXCH_CRNCY_CD");
            entity.Property(e => e.ExchRateDate)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("EXCH_RATE_DATE");
            entity.Property(e => e.ExchangeRateSetDate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 0)")
                .HasColumnName("EXCHANGE_RATE_SET_DATE");
            entity.Property(e => e.ExchangeRateSetTime)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("16:30")
                .HasColumnName("EXCHANGE_RATE_SET_TIME");
            entity.Property(e => e.ExchangeRateType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .HasColumnName("EXCHANGE_RATE_TYPE");
            entity.Property(e => e.ExpAttrType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("00")
                .HasColumnName("EXP_ATTR_TYPE");
            entity.Property(e => e.ExpHipAcc)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("EXP_HIP_ACC");
            entity.Property(e => e.ExpHipType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EXP_HIP_TYPE");
            entity.Property(e => e.FpiAccountNo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FPI_ACCOUNT_NO");
            entity.Property(e => e.FpiGlobalBank)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FPI_GLOBAL_BANK");
            entity.Property(e => e.FpiLocalBank)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FPI_LOCAL_BANK");
            entity.Property(e => e.FtFeeType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("FT_FEE_TYPE");
            entity.Property(e => e.FundCategory)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("FUND_CATEGORY");
            entity.Property(e => e.FundPriceType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("2")
                .HasColumnName("FUND_PRICE_TYPE");
            entity.Property(e => e.FundType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("D")
                .HasColumnName("FUND_TYPE");
            entity.Property(e => e.GainLostType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("GAIN_LOST_TYPE");
            entity.Property(e => e.GbPriceType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("GB_PRICE_TYPE");
            entity.Property(e => e.HolidayCalculateType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("HOLIDAY_CALCULATE_TYPE");
            entity.Property(e => e.IntIncludeType)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(1, 0)")
                .HasColumnName("INT_INCLUDE_TYPE");
            entity.Property(e => e.InvOutsourceDate).HasColumnName("INV_OUTSOURCE_DATE");
            entity.Property(e => e.InvOutsourceType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("INV_OUTSOURCE_TYPE");
            entity.Property(e => e.InvType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("INV_TYPE");
            entity.Property(e => e.IsBondSerNo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_BOND_SER_NO");
            entity.Property(e => e.IsOpen)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_OPEN");
            entity.Property(e => e.IsPosted)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_POSTED");
            entity.Property(e => e.IsReport)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_REPORT");
            entity.Property(e => e.IsShowReport)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_SHOW_REPORT");
            entity.Property(e => e.IsShowReport1)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_SHOW_REPORT1");
            entity.Property(e => e.IsTrial)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("IS_TRIAL");
            entity.Property(e => e.JepunId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("JEPUN_ID");
            entity.Property(e => e.JpMorganBic)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("JP_MORGAN_BIC");
            entity.Property(e => e.JpMorganId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("JP_MORGAN_ID");
            entity.Property(e => e.LastModified).HasColumnName("LAST_MODIFIED");
            entity.Property(e => e.LastModified1).HasColumnName("LAST_MODIFIED1");
            entity.Property(e => e.LifeMcCalType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .HasColumnName("LIFE_MC_CAL_TYPE");
            entity.Property(e => e.LostPoint1)
                .HasColumnType("decimal(6, 4)")
                .HasColumnName("LOST_POINT1");
            entity.Property(e => e.LostPoint2)
                .HasColumnType("decimal(6, 4)")
                .HasColumnName("LOST_POINT2");
            entity.Property(e => e.LowHoldRate)
                .HasColumnType("decimal(6, 4)")
                .HasColumnName("LOW_HOLD_RATE");
            entity.Property(e => e.LowMfRate)
                .HasColumnType("decimal(7, 6)")
                .HasColumnName("LOW_MF_RATE");
            entity.Property(e => e.MaturityDate).HasColumnName("MATURITY_DATE");
            entity.Property(e => e.MaturityType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("MATURITY_TYPE");
            entity.Property(e => e.McCalculateType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("MC_CALCULATE_TYPE");
            entity.Property(e => e.MfPayCrncy)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("MF_PAY_CRNCY");
            entity.Property(e => e.MultiClassType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("A")
                .HasColumnName("MULTI_CLASS_TYPE");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.NavAllocateType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("NAV_ALLOCATE_TYPE");
            entity.Property(e => e.NavPoint)
                .HasDefaultValue(2m)
                .HasColumnType("decimal(1, 0)")
                .HasColumnName("NAV_POINT");
            entity.Property(e => e.NavPublish)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .HasColumnName("NAV_PUBLISH");
            entity.Property(e => e.NavRoundType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("NAV_ROUND_TYPE");
            entity.Property(e => e.NavType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("NAV_TYPE");
            entity.Property(e => e.OldContractSeq)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("OLD_CONTRACT_SEQ");
            entity.Property(e => e.OpenDate).HasColumnName("OPEN_DATE");
            entity.Property(e => e.OpenEmpNo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("OPEN_EMP_NO");
            entity.Property(e => e.OptionIncomeType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("OPTION_INCOME_TYPE");
            entity.Property(e => e.OrgFaceNav)
                .HasDefaultValue(10m)
                .HasColumnType("decimal(10, 4)")
                .HasColumnName("ORG_FACE_NAV");
            entity.Property(e => e.OrgNav)
                .HasDefaultValue(10m)
                .HasColumnType("decimal(14, 8)")
                .HasColumnName("ORG_NAV");
            entity.Property(e => e.OrgNavCrncy)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("ORG_NAV_CRNCY");
            entity.Property(e => e.OrgNavDate).HasColumnName("ORG_NAV_DATE");
            entity.Property(e => e.OutsourceDate).HasColumnName("OUTSOURCE_DATE");
            entity.Property(e => e.OutsourceType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("OUTSOURCE_TYPE");
            entity.Property(e => e.OverseasFeeType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("OVERSEAS_FEE_TYPE");
            entity.Property(e => e.PrevDate).HasColumnName("PREV_DATE");
            entity.Property(e => e.PurAccDays)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("PUR_ACC_DAYS");
            entity.Property(e => e.PurDays)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("PUR_DAYS");
            entity.Property(e => e.PurNavDays)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("PUR_NAV_DAYS");
            entity.Property(e => e.RbType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("RB_TYPE");
            entity.Property(e => e.RealAccount)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("REAL_ACCOUNT");
            entity.Property(e => e.RedNavChangeDate).HasColumnName("RED_NAV_CHANGE_DATE");
            entity.Property(e => e.RedType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("RED_TYPE");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("REMARK");
            entity.Property(e => e.Remark1)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("REMARK1");
            entity.Property(e => e.ReportDate).HasColumnName("REPORT_DATE");
            entity.Property(e => e.RoundType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("ROUND_TYPE");
            entity.Property(e => e.SName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("S_NAME");
            entity.Property(e => e.SelAccDays)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("SEL_ACC_DAYS");
            entity.Property(e => e.SelDays)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("SEL_DAYS");
            entity.Property(e => e.SelNavDays)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(2, 0)")
                .HasColumnName("SEL_NAV_DAYS");
            entity.Property(e => e.SeparateFee)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("SEPARATE_FEE");
            entity.Property(e => e.SeparateFeeDate).HasColumnName("SEPARATE_FEE_DATE");
            entity.Property(e => e.SerNo)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasColumnName("SER_NO");
            entity.Property(e => e.SfPayCrncy)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("SF_PAY_CRNCY");
            entity.Property(e => e.SfbFundId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SFB_FUND_ID");
            entity.Property(e => e.SfbFundType)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("SFB_FUND_TYPE");
            entity.Property(e => e.SharesPoint)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(1, 0)")
                .HasColumnName("SHARES_POINT");
            entity.Property(e => e.SharesRoundType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasColumnName("SHARES_ROUND_TYPE");
            entity.Property(e => e.Ssname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("SSNAME");
            entity.Property(e => e.StepCd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .HasColumnName("STEP_CD");
            entity.Property(e => e.StockCost)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("STOCK_COST");
            entity.Property(e => e.SubCustodianNo)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("SUB_CUSTODIAN_NO");
            entity.Property(e => e.SubpType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .HasColumnName("SUBP_TYPE");
            entity.Property(e => e.SwId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("SW_ID");
            entity.Property(e => e.TaxId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TAX_ID");
            entity.Property(e => e.TaxType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("TAX_TYPE");
            entity.Property(e => e.ToCode)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("TO_CODE");
            entity.Property(e => e.TrustAmount)
                .HasColumnType("decimal(12, 0)")
                .HasColumnName("TRUST_AMOUNT");
            entity.Property(e => e.Type)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("TYPE");
            entity.Property(e => e.ValueDate).HasColumnName("VALUE_DATE");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptNo);

            entity.ToTable("DEPARTMENT", "RIS", tb => tb.HasComment("部門資料表"));

            entity.Property(e => e.DeptNo)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasComment("部門代號")
                .HasColumnName("DEPT_NO");
            entity.Property(e => e.Category)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("CATEGORY");
            entity.Property(e => e.DeptChName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DEPT_CH_NAME");
            entity.Property(e => e.DeptEnName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DEPT_EN_NAME");
            entity.Property(e => e.DeptMgr)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DEPT_MGR");
            entity.Property(e => e.DeptShNm)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("DEPT_SH_NM");
            entity.Property(e => e.InputDate).HasColumnName("INPUT_DATE");
            entity.Property(e => e.InputUser)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("INPUT_USER");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmpNo);

            entity.ToTable("EMPLOYEE", "RIS", tb => tb.HasComment("員工基本資料表"));

            entity.Property(e => e.EmpNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("員工代號")
                .HasColumnName("EMP_NO");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.DeptNo)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("DEPT_NO");
            entity.Property(e => e.DomainId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DOMAIN_ID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.EmpCd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .IsFixedLength()
                .HasColumnName("EMP_CD");
            entity.Property(e => e.EmpIdNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMP_ID_NO");
            entity.Property(e => e.EmpName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("EMP_NAME");
            entity.Property(e => e.EmpNameEng)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("EMP_NAME_ENG");
            entity.Property(e => e.EntryDate).HasColumnName("ENTRY_DATE");
            entity.Property(e => e.GroupEmail)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("GROUP_EMAIL");
            entity.Property(e => e.InputDate).HasColumnName("INPUT_DATE");
            entity.Property(e => e.InputUser)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("INPUT_USER");
            entity.Property(e => e.LeaveDate).HasColumnName("LEAVE_DATE");
            entity.Property(e => e.ManagrType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("MANAGR_TYPE");
            entity.Property(e => e.MangrCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MANGR_CODE");
            entity.Property(e => e.Remark)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("REMARK");
            entity.Property(e => e.Seq).HasColumnName("SEQ");
            entity.Property(e => e.Telo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TELO");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("TITLE");
        });

        modelBuilder.Entity<Equity>(entity =>
        {
            entity.HasKey(e => new { e.StkCd, e.DealCtr });

            entity.ToTable("EQUITY", "RIS", tb => tb.HasComment("股票基本資料表"));

            entity.Property(e => e.StkCd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STK_CD");
            entity.Property(e => e.DealCtr)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasComment("交易所代碼")
                .HasColumnName("DEAL_CTR");
            entity.Property(e => e.CfmCd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CFM_CD");
            entity.Property(e => e.CfmDate).HasColumnName("CFM_DATE");
            entity.Property(e => e.CfmUser)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("CFM_USER");
            entity.Property(e => e.CloseDate).HasColumnName("CLOSE_DATE");
            entity.Property(e => e.ComTelNo)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("COM_TEL_NO");
            entity.Property(e => e.Comm)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("COMM");
            entity.Property(e => e.CompName)
                .HasMaxLength(2100)
                .IsUnicode(false)
                .HasColumnName("COMP_NAME");
            entity.Property(e => e.CountryCd)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("COUNTRY_CD");
            entity.Property(e => e.CptlAmt)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(15, 2)")
                .HasColumnName("CPTL_AMT");
            entity.Property(e => e.CrdDate).HasColumnName("CRD_DATE");
            entity.Property(e => e.CrdUser)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("CRD_USER");
            entity.Property(e => e.CrncyCd)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CRNCY_CD");
            entity.Property(e => e.CrtsCode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CRTS_CODE");
            entity.Property(e => e.CrtsId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CRTS_ID");
            entity.Property(e => e.CusipCode)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("CUSIP_CODE");
            entity.Property(e => e.Engnm)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("ENGNM");
            entity.Property(e => e.EtfAccountName)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ETF_ACCOUNT_NAME");
            entity.Property(e => e.EtfAccountNo)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ETF_ACCOUNT_NO");
            entity.Property(e => e.EtfBankBranch)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("ETF_BANK_BRANCH");
            entity.Property(e => e.EtfBankNo)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("ETF_BANK_NO");
            entity.Property(e => e.GtsalDate).HasColumnName("GTSAL_DATE");
            entity.Property(e => e.IdNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID_NO");
            entity.Property(e => e.IdtType)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("IDT_TYPE");
            entity.Property(e => e.IdtTypeBloomberg)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("IDT_TYPE_BLOOMBERG");
            entity.Property(e => e.IdtTypeSp)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("IDT_TYPE_SP");
            entity.Property(e => e.IsinCode)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasColumnName("ISIN_CODE");
            entity.Property(e => e.IssueBranch)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ISSUE_BRANCH");
            entity.Property(e => e.IssueBroker)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("ISSUE_BROKER");
            entity.Property(e => e.IssueType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("B")
                .IsFixedLength()
                .HasColumnName("ISSUE_TYPE");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.OldStkCd)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("OLD_STK_CD");
            entity.Property(e => e.OrgStkCd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("ORG_STK_CD");
            entity.Property(e => e.OsStk)
                .HasDefaultValue(0L)
                .HasColumnName("OS_STK");
            entity.Property(e => e.ProfType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("PROF_TYPE");
            entity.Property(e => e.SedolCode)
                .HasMaxLength(35)
                .IsUnicode(false)
                .HasColumnName("SEDOL_CODE");
            entity.Property(e => e.ShEngnm)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("SH_ENGNM");
            entity.Property(e => e.ShName)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("SH_NAME");
            entity.Property(e => e.StkChnm)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("STK_CHNM");
            entity.Property(e => e.StkSevBranch)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("STK_SEV_BRANCH");
            entity.Property(e => e.StkSevCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STK_SEV_CD");
            entity.Property(e => e.StkSevId)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("STK_SEV_ID");
            entity.Property(e => e.StkSevTel)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("STK_SEV_TEL");
            entity.Property(e => e.StkType)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("STK_TYPE");
            entity.Property(e => e.Symbol)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("SYMBOL");
            entity.Property(e => e.TCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("T_CODE");
            entity.Property(e => e.TrnDealCtr)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("TRN_DEAL_CTR");
            entity.Property(e => e.TrnStkDt).HasColumnName("TRN_STK_DT");
            entity.Property(e => e.TrnStkNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("TRN_STK_NO");
            entity.Property(e => e.UpdDate).HasColumnName("UPD_DATE");
            entity.Property(e => e.UpdUser)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("UPD_USER");
            entity.Property(e => e.WeitYn)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("WEIT_YN");
        });

        modelBuilder.Entity<EquityOutList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("EQUITY_OUT_LIST", "RIS");

            entity.Property(e => e.CompName)
                .HasMaxLength(2100)
                .IsUnicode(false)
                .HasColumnName("COMP_NAME");
            entity.Property(e => e.ListCount).HasColumnName("LIST_COUNT");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.StkCd)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("股票代號")
                .HasColumnName("STK_CD");
            entity.Property(e => e.UpdDate).HasColumnName("UPD_DATE");
            entity.Property(e => e.UpdUser)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("UPD_USER");
        });

        modelBuilder.Entity<EquitySpList>(entity =>
        {
            entity.HasKey(e => new { e.QuarterNo, e.StkCd });

            entity.ToTable("EQUITY_SP_LIST", "RIS", tb => tb.HasComment("股票研究員清單表"));

            entity.Property(e => e.QuarterNo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasComment("季度")
                .HasColumnName("QUARTER_NO");
            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STK_CD");
            entity.Property(e => e.AcDate).HasColumnName("AC_DATE");
            entity.Property(e => e.EmpNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMP_NO");
            entity.Property(e => e.EndDate).HasColumnName("END_DATE");
            entity.Property(e => e.IdtName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("IDT_NAME");
            entity.Property(e => e.InputType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("INPUT_TYPE");
            entity.Property(e => e.ResEmpNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RES_EMP_NO");
            entity.Property(e => e.StkName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STK_NAME");
            entity.Property(e => e.UpdDate).HasColumnName("UPD_DATE");
            entity.Property(e => e.UpdUser)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UPD_USER");
        });

        modelBuilder.Entity<FileCvPar>(entity =>
        {
            entity.HasKey(e => new { e.ApCode, e.MenuId, e.SourceSeq });

            entity.ToTable("FILE_CV_PAR", "RIS", tb => tb.HasComment("檔案轉換參數表"));

            entity.Property(e => e.ApCode)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasComment("應用程式代號")
                .HasColumnName("AP_CODE");
            entity.Property(e => e.MenuId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MENU_ID");
            entity.Property(e => e.SourceSeq).HasColumnName("SOURCE_SEQ");
            entity.Property(e => e.ColumnsCount).HasColumnName("COLUMNS_COUNT");
            entity.Property(e => e.ConverterType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CONVERTER_TYPE");
            entity.Property(e => e.DefaultFileName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("DEFAULT_FILE_NAME");
            entity.Property(e => e.DefaultPath)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("DEFAULT_PATH");
            entity.Property(e => e.FileType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("FILE_TYPE");
            entity.Property(e => e.OwnerName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("OWNER_NAME");
            entity.Property(e => e.RecsLength).HasColumnName("RECS_LENGTH");
            entity.Property(e => e.RowsType)
                .HasDefaultValue(1)
                .HasColumnName("ROWS_TYPE");
            entity.Property(e => e.SeparateCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue(",")
                .HasColumnName("SEPARATE_CODE");
            entity.Property(e => e.SheetName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("SHEET_NAME");
            entity.Property(e => e.TableName)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("TABLE_NAME");
            entity.Property(e => e.TitleType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("TITLE_TYPE");
        });

        modelBuilder.Entity<HsbcFundMaster>(entity =>
        {
            entity.ToTable("HSBC_FUND_MASTER", "RIS", tb => tb.HasComment("HSBC基金主檔表"));

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("基金代號")
                .HasColumnName("ID");
            entity.Property(e => e.CalendarCode)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("CALENDAR_CODE");
            entity.Property(e => e.CrtsId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CRTS_ID");
            entity.Property(e => e.EdwId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("EDW_ID");
            entity.Property(e => e.Ename)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ENAME");
            entity.Property(e => e.ExpHipType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EXP_HIP_TYPE");
            entity.Property(e => e.FacId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("FAC_ID");
            entity.Property(e => e.FasId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("FAS_ID");
            entity.Property(e => e.HiportId)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("HIPORT_ID");
            entity.Property(e => e.JepunId)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("JEPUN_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("NAME");
            entity.Property(e => e.SfbFundId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("SFB_FUND_ID");
            entity.Property(e => e.SitcaPcaId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("SITCA_PCA_ID");
        });

        modelBuilder.Entity<Phrase>(entity =>
        {
            entity.HasKey(e => e.PhraseType);

            entity.ToTable("PHRASE", "RIS", tb => tb.HasComment("片語資料表"));

            entity.Property(e => e.PhraseType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("片語類別")
                .HasColumnName("PHRASE_TYPE");
            entity.Property(e => e.PhraseDes)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("PHRASE_DES");
        });

        modelBuilder.Entity<ProgCtrl>(entity =>
        {
            entity.HasKey(e => new { e.ApCode, e.MenuId, e.ProgCtrlCode });

            entity.ToTable("PROG_CTRL", "RIS", tb => tb.HasComment("程式控制表"));

            entity.Property(e => e.ApCode)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasComment("應用程式代號")
                .HasColumnName("AP_CODE");
            entity.Property(e => e.MenuId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MENU_ID");
            entity.Property(e => e.ProgCtrlCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PROG_CTRL_CODE");
            entity.Property(e => e.Flag)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("FLAG");
            entity.Property(e => e.ProgCtrlDate).HasColumnName("PROG_CTRL_DATE");
            entity.Property(e => e.ProgCtrlDes)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PROG_CTRL_DES");
            entity.Property(e => e.Remark)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("REMARK");
        });

        modelBuilder.Entity<ShmtOutSource1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("SHMT_OUT_SOURCE1", "RIS", tb => tb.HasComment("股東會外部人員聯絡資料表"));

            entity.Property(e => e.AcDate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("建檔日期")
                .HasColumnName("AC_DATE");
            entity.Property(e => e.ContactAddr1)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CONTACT_ADDR1");
            entity.Property(e => e.ContactAddr2)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CONTACT_ADDR2");
            entity.Property(e => e.ContactAddr3)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("CONTACT_ADDR3");
            entity.Property(e => e.ContactArea1)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CONTACT_AREA1");
            entity.Property(e => e.ContactArea2)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CONTACT_AREA2");
            entity.Property(e => e.ContactArea3)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CONTACT_AREA3");
            entity.Property(e => e.ContactEmail1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CONTACT_EMAIL1");
            entity.Property(e => e.ContactEmail2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CONTACT_EMAIL2");
            entity.Property(e => e.ContactEmail3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CONTACT_EMAIL3");
            entity.Property(e => e.ContactExt1)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CONTACT_EXT1");
            entity.Property(e => e.ContactExt2)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CONTACT_EXT2");
            entity.Property(e => e.ContactExt3)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("CONTACT_EXT3");
            entity.Property(e => e.ContactId1)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CONTACT_ID1");
            entity.Property(e => e.ContactId2)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CONTACT_ID2");
            entity.Property(e => e.ContactId3)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CONTACT_ID3");
            entity.Property(e => e.ContactName1)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CONTACT_NAME1");
            entity.Property(e => e.ContactName2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CONTACT_NAME2");
            entity.Property(e => e.ContactName3)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CONTACT_NAME3");
            entity.Property(e => e.ContactPst1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CONTACT_PST1");
            entity.Property(e => e.ContactPst2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CONTACT_PST2");
            entity.Property(e => e.ContactPst3)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CONTACT_PST3");
            entity.Property(e => e.ContactTel1)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CONTACT_TEL1");
            entity.Property(e => e.ContactTel2)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CONTACT_TEL2");
            entity.Property(e => e.ContactTel3)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CONTACT_TEL3");
            entity.Property(e => e.EmpNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMP_NO");
            entity.Property(e => e.Remark1)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("REMARK1");
            entity.Property(e => e.Remark2)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("REMARK2");
            entity.Property(e => e.Remark3)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("REMARK3");
            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STK_CD");
            entity.Property(e => e.StkName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STK_NAME");
        });

        modelBuilder.Entity<ShmtPar>(entity =>
        {
            entity.HasKey(e => new { e.StkCd, e.DealCtr, e.ShmtDate });

            entity.ToTable("SHMT_PAR", "RIS", tb => tb.HasComment("股東會基本資料表"));

            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("股票代號")
                .HasColumnName("STK_CD");
            entity.Property(e => e.DealCtr)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasComment("交易所代碼")
                .HasColumnName("DEAL_CTR");
            entity.Property(e => e.ShmtDate)
                .HasComment("股東會開會日期")
                .HasColumnName("SHMT_DATE");
            entity.Property(e => e.AllShares)
                .HasDefaultValue(0L)
                .HasColumnName("ALL_SHARES");
            entity.Property(e => e.AttendType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .IsFixedLength()
                .HasColumnName("ATTEND_TYPE");
            entity.Property(e => e.ChfChgCnt1)
                .HasDefaultValue(0)
                .HasColumnName("CHF_CHG_CNT1");
            entity.Property(e => e.ChfChgCnt2)
                .HasDefaultValue(0)
                .HasColumnName("CHF_CHG_CNT2");
            entity.Property(e => e.ChfChgRate1)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("CHF_CHG_RATE1");
            entity.Property(e => e.ChfChgRate2)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("CHF_CHG_RATE2");
            entity.Property(e => e.ChfChgType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .IsFixedLength()
                .HasColumnName("CHF_CHG_TYPE");
            entity.Property(e => e.ChfChgType1)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .IsFixedLength()
                .HasColumnName("CHF_CHG_TYPE1");
            entity.Property(e => e.ChfChgType2)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .HasColumnName("CHF_CHG_TYPE2");
            entity.Property(e => e.ChfChgValue)
                .HasDefaultValue(0L)
                .HasColumnName("CHF_CHG_VALUE");
            entity.Property(e => e.ChfChgYn)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("CHF_CHG_YN");
            entity.Property(e => e.CompName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("COMP_NAME");
            entity.Property(e => e.ContactCheck)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("CONTACT_CHECK");
            entity.Property(e => e.ContactDate).HasColumnName("CONTACT_DATE");
            entity.Property(e => e.ContactEmp1)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CONTACT_EMP1");
            entity.Property(e => e.ContactEmp2)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CONTACT_EMP2");
            entity.Property(e => e.DataDate).HasColumnName("DATA_DATE");
            entity.Property(e => e.DelDate).HasColumnName("DEL_DATE");
            entity.Property(e => e.DelReason)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("DEL_REASON");
            entity.Property(e => e.DelType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("DEL_TYPE");
            entity.Property(e => e.DocType1)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .IsFixedLength()
                .HasColumnName("DOC_TYPE1");
            entity.Property(e => e.DocType2)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .IsFixedLength()
                .HasColumnName("DOC_TYPE2");
            entity.Property(e => e.DocType3)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .IsFixedLength()
                .HasColumnName("DOC_TYPE3");
            entity.Property(e => e.DocType4)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("DOC_TYPE4");
            entity.Property(e => e.DocType5)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("DOC_TYPE5");
            entity.Property(e => e.DocType6)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("DOC_TYPE6");
            entity.Property(e => e.DocType7)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("DOC_TYPE7");
            entity.Property(e => e.DocType8)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("DOC_TYPE8");
            entity.Property(e => e.DocType9)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("DOC_TYPE9");
            entity.Property(e => e.Fund300Cnt)
                .HasDefaultValue(0)
                .HasColumnName("FUND_300_CNT");
            entity.Property(e => e.InputType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("INPUT_TYPE");
            entity.Property(e => e.LsbyDate).HasColumnName("LSBY_DATE");
            entity.Property(e => e.LsrgDate).HasColumnName("LSRG_DATE");
            entity.Property(e => e.OutEmp)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("OUT_EMP");
            entity.Property(e => e.OuterType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("N")
                .IsFixedLength()
                .HasColumnName("OUTER_TYPE");
            entity.Property(e => e.Par001).HasColumnName("PAR001");
            entity.Property(e => e.Par001Date).HasColumnName("PAR001_DATE");
            entity.Property(e => e.Par001Status)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("PAR001_STATUS");
            entity.Property(e => e.Par002).HasColumnName("PAR002");
            entity.Property(e => e.Par002Date).HasColumnName("PAR002_DATE");
            entity.Property(e => e.Par002Status)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("PAR002_STATUS");
            entity.Property(e => e.Par003).HasColumnName("PAR003");
            entity.Property(e => e.Par003Date).HasColumnName("PAR003_DATE");
            entity.Property(e => e.Par003Status)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("PAR003_STATUS");
            entity.Property(e => e.SeqNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SEQ_NO");
            entity.Property(e => e.ShmtAddr)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SHMT_ADDR");
            entity.Property(e => e.ShmtTime)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("SHMT_TIME");
            entity.Property(e => e.SsrgDate).HasColumnName("SSRG_DATE");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TYPE");
            entity.Property(e => e.UpdDate).HasColumnName("UPD_DATE");
            entity.Property(e => e.UpdUser)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UPD_USER");
        });

        modelBuilder.Entity<ShmtParChf1>(entity =>
        {
            entity.HasKey(e => new { e.StkCd, e.ShmtDate, e.Seq });

            entity.ToTable("SHMT_PAR_CHF1", "RIS", tb => tb.HasComment("股東會董事名單表"));

            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("股票代號")
                .HasColumnName("STK_CD");
            entity.Property(e => e.ShmtDate).HasColumnName("SHMT_DATE");
            entity.Property(e => e.Seq).HasColumnName("SEQ");
            entity.Property(e => e.ChfChgName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CHF_CHG_NAME");
            entity.Property(e => e.Type1)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TYPE1");
            entity.Property(e => e.Type2)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TYPE2");
        });

        modelBuilder.Entity<ShmtParChf2>(entity =>
        {
            entity.HasKey(e => new { e.StkCd, e.ShmtDate, e.Seq });

            entity.ToTable("SHMT_PAR_CHF2", "RIS", tb => tb.HasComment("股東會監察人名單表"));

            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("股票代號")
                .HasColumnName("STK_CD");
            entity.Property(e => e.ShmtDate).HasColumnName("SHMT_DATE");
            entity.Property(e => e.Seq).HasColumnName("SEQ");
            entity.Property(e => e.ChfChgName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CHF_CHG_NAME");
            entity.Property(e => e.Type1)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TYPE1");
            entity.Property(e => e.Type2)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("TYPE2");
        });

        modelBuilder.Entity<ShmtParDe>(entity =>
        {
            entity.HasKey(e => new { e.StkCd, e.ShmtDate, e.Seq });

            entity.ToTable("SHMT_PAR_DES", "RIS", tb => tb.HasComment("股東會原因說明表"));

            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("股票代號")
                .HasColumnName("STK_CD");
            entity.Property(e => e.ShmtDate).HasColumnName("SHMT_DATE");
            entity.Property(e => e.Seq).HasColumnName("SEQ");
            entity.Property(e => e.Des)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("DES");
        });

        modelBuilder.Entity<ShmtParHolder>(entity =>
        {
            entity.HasKey(e => new { e.StkCd, e.ShmtDate, e.Id, e.ContractSeq });

            entity.ToTable("SHMT_PAR_HOLDER", "RIS", tb => tb.HasComment("股東會持股明細表"));

            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("股票代號")
                .HasColumnName("STK_CD");
            entity.Property(e => e.ShmtDate).HasColumnName("SHMT_DATE");
            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.ContractSeq)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CONTRACT_SEQ");
            entity.Property(e => e.AcDate).HasColumnName("AC_DATE");
            entity.Property(e => e.InputType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("INPUT_TYPE");
            entity.Property(e => e.Shares)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("SHARES");
            entity.Property(e => e.UpdDate).HasColumnName("UPD_DATE");
            entity.Property(e => e.UpdUser)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UPD_USER");
        });

        modelBuilder.Entity<ShmtParHolderAll>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("SHMT_PAR_HOLDER_ALL", "RIS");

            entity.Property(e => e.AcDate).HasColumnName("AC_DATE");
            entity.Property(e => e.ContractSeq)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CONTRACT_SEQ");
            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.InputType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("INPUT_TYPE");
            entity.Property(e => e.Shares)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("SHARES");
            entity.Property(e => e.ShmtDate).HasColumnName("SHMT_DATE");
            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STK_CD");
            entity.Property(e => e.Type)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasComment("類型")
                .HasColumnName("TYPE");
            entity.Property(e => e.UpdDate).HasColumnName("UPD_DATE");
            entity.Property(e => e.UpdUser)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UPD_USER");
        });

        modelBuilder.Entity<ShmtParHolderDam>(entity =>
        {
            entity.HasKey(e => new { e.StkCd, e.ShmtDate, e.Id, e.ContractSeq });

            entity.ToTable("SHMT_PAR_HOLDER_DAM", "RIS", tb => tb.HasComment("股東會DAM持股明細表"));

            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("股票代號")
                .HasColumnName("STK_CD");
            entity.Property(e => e.ShmtDate).HasColumnName("SHMT_DATE");
            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID");
            entity.Property(e => e.ContractSeq)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CONTRACT_SEQ");
            entity.Property(e => e.AcDate).HasColumnName("AC_DATE");
            entity.Property(e => e.InputType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("INPUT_TYPE");
            entity.Property(e => e.Shares)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("SHARES");
            entity.Property(e => e.UpdDate).HasColumnName("UPD_DATE");
            entity.Property(e => e.UpdUser)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("UPD_USER");
        });

        modelBuilder.Entity<ShmtParReason>(entity =>
        {
            entity.HasKey(e => new { e.StkCd, e.ShmtDate, e.Seq });

            entity.ToTable("SHMT_PAR_REASON", "RIS", tb => tb.HasComment("股東會原因說明表2"));

            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComment("股票代號")
                .HasColumnName("STK_CD");
            entity.Property(e => e.ShmtDate).HasColumnName("SHMT_DATE");
            entity.Property(e => e.Seq).HasColumnName("SEQ");
            entity.Property(e => e.Des)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("DES");
        });

        modelBuilder.Entity<ShmtSource1>(entity =>
        {
            entity.HasKey(e => new { e.AcDate, e.EmpNo, e.StkCd, e.Type, e.ShmtDate }).HasName("PK_shmtsource1");

            entity.ToTable("SHMT_SOURCE1", "RIS", tb => tb.HasComment("股東會來源資料表1"));

            entity.Property(e => e.AcDate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("處理日期字串")
                .HasColumnName("AC_DATE");
            entity.Property(e => e.EmpNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMP_NO");
            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STK_CD");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TYPE");
            entity.Property(e => e.ShmtDate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SHMT_DATE");
            entity.Property(e => e.ChfChgYn)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CHF_CHG_YN");
            entity.Property(e => e.Des)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("DES");
            entity.Property(e => e.ShmtAddr)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("SHMT_ADDR");
            entity.Property(e => e.SsrgDate)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SSRG_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("Y")
                .IsFixedLength()
                .HasColumnName("STATUS");
            entity.Property(e => e.StkName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STK_NAME");
        });

        modelBuilder.Entity<ShmtSource2>(entity =>
        {
            entity.HasKey(e => new { e.AcDate, e.EmpNo, e.StkCd });

            entity.ToTable("SHMT_SOURCE2", "RIS", tb => tb.HasComment("股東會董監改選資料表"));

            entity.Property(e => e.AcDate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("建檔日期")
                .HasColumnName("AC_DATE");
            entity.Property(e => e.EmpNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMP_NO");
            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STK_CD");
            entity.Property(e => e.DataDate).HasColumnName("DATA_DATE");
            entity.Property(e => e.StkName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STK_NAME");
        });

        modelBuilder.Entity<ShmtSource3>(entity =>
        {
            entity.HasKey(e => new { e.AcDate, e.EmpNo, e.StkCd });

            entity.ToTable("SHMT_SOURCE3", "RIS", tb => tb.HasComment("股東會董監改選資料表"));

            entity.Property(e => e.AcDate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("建檔日期")
                .HasColumnName("AC_DATE");
            entity.Property(e => e.EmpNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMP_NO");
            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STK_CD");
            entity.Property(e => e.DataDate).HasColumnName("DATA_DATE");
            entity.Property(e => e.StkName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STK_NAME");
        });

        modelBuilder.Entity<ShmtSource4>(entity =>
        {
            entity.HasKey(e => new { e.AcDate, e.EmpNo, e.StkCd });

            entity.ToTable("SHMT_SOURCE4", "RIS", tb => tb.HasComment("股東會公司基本資料表"));

            entity.Property(e => e.AcDate)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("建檔日期")
                .HasColumnName("AC_DATE");
            entity.Property(e => e.EmpNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("EMP_NO");
            entity.Property(e => e.StkCd)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STK_CD");
            entity.Property(e => e.Addr)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("ADDR");
            entity.Property(e => e.BrokerName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BROKER_NAME");
            entity.Property(e => e.BrokerTel)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("BROKER_TEL");
            entity.Property(e => e.Chairman)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CHAIRMAN");
            entity.Property(e => e.CompName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("COMP_NAME");
            entity.Property(e => e.IdNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ID_NO");
            entity.Property(e => e.President)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PRESIDENT");
            entity.Property(e => e.Spokesman)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SPOKESMAN");
            entity.Property(e => e.StkName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STK_NAME");
            entity.Property(e => e.Tel)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("TEL");
        });

        modelBuilder.Entity<StockBalance>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.ContractSeq, e.AcDate, e.StockNo, e.AssetType });

            entity.ToTable("STOCK_BALANCE", "DMS", tb => tb.HasComment("股票餘額檔"));

            entity.Property(e => e.Id)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("基金代碼")
                .HasColumnName("ID");
            entity.Property(e => e.ContractSeq)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasComment("契約序號")
                .HasColumnName("CONTRACT_SEQ");
            entity.Property(e => e.AcDate)
                .HasComment("記帳日")
                .HasColumnName("AC_DATE");
            entity.Property(e => e.StockNo)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasComment("股票代號")
                .HasColumnName("STOCK_NO");
            entity.Property(e => e.AssetType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .HasComment("資產類型")
                .HasColumnName("ASSET_TYPE");
            entity.Property(e => e.ApprAmt)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("APPR_AMT");
            entity.Property(e => e.ApprAmtBas)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("APPR_AMT_BAS");
            entity.Property(e => e.AssetCost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("ASSET_COST");
            entity.Property(e => e.AssetValue)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("ASSET_VALUE");
            entity.Property(e => e.AvgCost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 12)")
                .HasColumnName("AVG_COST");
            entity.Property(e => e.CfmCd)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CFM_CD");
            entity.Property(e => e.CfmDate).HasColumnName("CFM_DATE");
            entity.Property(e => e.CfmEmpNo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("CFM_EMP_NO");
            entity.Property(e => e.Cost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("COST");
            entity.Property(e => e.CostBas)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("COST_BAS");
            entity.Property(e => e.CurrencyNo)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("CURRENCY_NO");
            entity.Property(e => e.EmpNo)
                .HasMaxLength(6)
                .IsUnicode(false)
                .HasColumnName("EMP_NO");
            entity.Property(e => e.ExcDate).HasColumnName("EXC_DATE");
            entity.Property(e => e.ExgRate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(21, 6)")
                .HasColumnName("EXG_RATE");
            entity.Property(e => e.GainLost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("GAIN_LOST");
            entity.Property(e => e.GainLostBas)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("GAIN_LOST_BAS");
            entity.Property(e => e.ImpDate).HasColumnName("IMP_DATE");
            entity.Property(e => e.ImpId)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IMP_ID");
            entity.Property(e => e.MktAmt)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("MKT_AMT");
            entity.Property(e => e.MktAmtBas)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("MKT_AMT_BAS");
            entity.Property(e => e.NavRate)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 6)")
                .HasColumnName("NAV_RATE");
            entity.Property(e => e.NavRateSt)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 6)")
                .HasColumnName("NAV_RATE_ST");
            entity.Property(e => e.OrgStockNo)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("ORG_STOCK_NO");
            entity.Property(e => e.PreCost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("PRE_COST");
            entity.Property(e => e.PreShares)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("PRE_SHARES");
            entity.Property(e => e.Price)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 12)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Shares)
                .HasDefaultValue(0m)
                .HasComment("庫存數量")
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("SHARES");
            entity.Property(e => e.UnlistBishare)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_BISHARE");
            entity.Property(e => e.UnlistBoshare)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_BOSHARE");
            entity.Property(e => e.UnlistBsshare)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_BSSHARE");
            entity.Property(e => e.UnlistCdamt)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_CDAMT");
            entity.Property(e => e.UnlistCdamtBas)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_CDAMT_BAS");
            entity.Property(e => e.UnlistOshare)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_OSHARE");
            entity.Property(e => e.UnlistRishare)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_RISHARE");
            entity.Property(e => e.UnlistS1share)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_S1SHARE");
            entity.Property(e => e.UnlistS2share)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_S2SHARE");
            entity.Property(e => e.UnlistShares)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_SHARES");
            entity.Property(e => e.UnlistSsshare)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNLIST_SSSHARE");
            entity.Property(e => e.UnsettleShares)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNSETTLE_SHARES");
            entity.Property(e => e.UnstkDiv)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNSTK_DIV");
            entity.Property(e => e.UnstkDivBas)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNSTK_DIV_BAS");
            entity.Property(e => e.ValueCost)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("VALUE_COST");
        });

        modelBuilder.Entity<Trustee>(entity =>
        {
            entity.ToTable("TRUSTEE", "DMS", tb => tb.HasComment("委託人基本資料檔"));

            entity.Property(e => e.Id)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasComment("統一編號")
                .HasColumnName("ID");
            entity.Property(e => e.Ename)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasComment("英文名稱")
                .HasColumnName("ENAME");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasComment("中文名稱")
                .HasColumnName("NAME");
            entity.Property(e => e.Sname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("公司簡稱")
                .HasColumnName("SNAME");
            entity.Property(e => e.Ssname)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasComment("公司簡簡稱")
                .HasColumnName("SSNAME");
        });

        modelBuilder.Entity<FundDivSet>(entity =>
        {
            entity.HasKey(e => new { e.FundNo, e.DivType });

            entity.ToTable("FUND_DIV_SET", "MDS");

            entity.Property(e => e.FundNo)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("FUND_NO");
            entity.Property(e => e.DivType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("DIV_TYPE");
            entity.Property(e => e.Item01Seq).HasColumnName("ITEM01_SEQ");
            entity.Property(e => e.Item01SeqAdj).HasColumnName("ITEM01_SEQ_ADJ");
            entity.Property(e => e.Item01Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM01_TYPE");
            entity.Property(e => e.Item02Seq).HasColumnName("ITEM02_SEQ");
            entity.Property(e => e.Item02SeqAdj).HasColumnName("ITEM02_SEQ_ADJ");
            entity.Property(e => e.Item02Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM02_TYPE");
            entity.Property(e => e.Item03Seq).HasColumnName("ITEM03_SEQ");
            entity.Property(e => e.Item03SeqAdj).HasColumnName("ITEM03_SEQ_ADJ");
            entity.Property(e => e.Item03Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM03_TYPE");
            entity.Property(e => e.Item04Seq).HasColumnName("ITEM04_SEQ");
            entity.Property(e => e.Item04SeqAdj).HasColumnName("ITEM04_SEQ_ADJ");
            entity.Property(e => e.Item04Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM04_TYPE");
            entity.Property(e => e.Item05Seq).HasColumnName("ITEM05_SEQ");
            entity.Property(e => e.Item05SeqAdj).HasColumnName("ITEM05_SEQ_ADJ");
            entity.Property(e => e.Item05Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM05_TYPE");
            entity.Property(e => e.Item06Seq).HasColumnName("ITEM06_SEQ");
            entity.Property(e => e.Item06SeqAdj).HasColumnName("ITEM06_SEQ_ADJ");
            entity.Property(e => e.Item06Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM06_TYPE");
            entity.Property(e => e.Item07Seq).HasColumnName("ITEM07_SEQ");
            entity.Property(e => e.Item07SeqAdj).HasColumnName("ITEM07_SEQ_ADJ");
            entity.Property(e => e.Item07Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM07_TYPE");
            entity.Property(e => e.Item08Seq).HasColumnName("ITEM08_SEQ");
            entity.Property(e => e.Item08SeqAdj).HasColumnName("ITEM08_SEQ_ADJ");
            entity.Property(e => e.Item08Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM08_TYPE");
            entity.Property(e => e.Item09Seq).HasColumnName("ITEM09_SEQ");
            entity.Property(e => e.Item09SeqAdj).HasColumnName("ITEM09_SEQ_ADJ");
            entity.Property(e => e.Item09Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM09_TYPE");
            entity.Property(e => e.Item10Seq).HasColumnName("ITEM10_SEQ");
            entity.Property(e => e.Item10SeqAdj).HasColumnName("ITEM10_SEQ_ADJ");
            entity.Property(e => e.Item10Type)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ITEM10_TYPE");
            entity.Property(e => e.CapitalType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("CAPITAL_TYPE");
            entity.Property(e => e.EmailList)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("EMAIL_LIST");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreationDate).HasColumnName("CREATION_DATE");
            entity.Property(e => e.RevisedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REVISED_BY");
            entity.Property(e => e.RevisionDate).HasColumnName("REVISION_DATE");
            entity.Property(e => e.ReviewedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REVIEWED_BY");
            entity.Property(e => e.ReviewDate).HasColumnName("REVIEW_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.FirstDividendDate).HasColumnName("FIRST_DIVIDEND_DATE");
            entity.Property(e => e.FirstDividendCount).HasColumnName("FIRST_DIVIDEND_COUNT");
        });

        modelBuilder.Entity<FundDivObj>(entity =>
        {
            entity.HasKey(e => new { e.FundNo, e.DivType, e.TxDate });

            entity.ToTable("FUND_DIV_OBJ", "MDS");

            entity.Property(e => e.FundNo)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("FUND_NO");
            entity.Property(e => e.DivType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("DIV_TYPE");
            entity.Property(e => e.TxDate).HasColumnName("TX_DATE");
            entity.Property(e => e.DivObj)
                .HasColumnType("decimal(12, 6)")
                .HasColumnName("DIV_OBJ");
            entity.Property(e => e.DivObjAmt)
                .HasColumnType("decimal(12, 6)")
                .HasColumnName("DIV_OBJ_AMT");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CREATED_BY");
            entity.Property(e => e.CreationDate).HasColumnName("CREATION_DATE");
            entity.Property(e => e.RevisedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REVISED_BY");
            entity.Property(e => e.RevisionDate).HasColumnName("REVISION_DATE");
            entity.Property(e => e.ReviewedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REVIEWED_BY");
            entity.Property(e => e.ReviewDate).HasColumnName("REVIEW_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("STATUS");
        });

        modelBuilder.Entity<FundDiv>(entity =>
        {
            entity.HasKey(e => new { e.FundNo, e.DividendDate, e.DividendType });

            entity.ToTable("FUND_DIV", "MDS");

            entity.Property(e => e.FundNo)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("FUND_NO");
            entity.Property(e => e.DividendYear).HasColumnName("DIVIDEND_YEAR");
            entity.Property(e => e.DividendDate).HasColumnName("DIVIDEND_DATE");
            entity.Property(e => e.DividendType)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("DIVIDEND_TYPE");
            entity.Property(e => e.PayDate).HasColumnName("PAY_DATE");
            entity.Property(e => e.PaymentDate).HasColumnName("PAYMENT_DATE");
            entity.Property(e => e.NextDividendDate).HasColumnName("NEXT_DIVIDEND_DATE");
            entity.Property(e => e.ReallyDivDate).HasColumnName("REALLY_DIV_DATE");
            entity.Property(e => e.Unit)
                .HasColumnType("decimal(20, 5)")
                .HasColumnName("UNIT");
            entity.Property(e => e.Nav)
                .HasColumnType("decimal(14, 8)")
                .HasColumnName("NAV");
            entity.Property(e => e.PreDiv1).HasColumnName("PRE_DIV1");
            entity.Property(e => e.PreDiv1Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV1_RATE");
            entity.Property(e => e.PreDiv1RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV1_RATE_M");
            entity.Property(e => e.PreDiv1Adj).HasColumnName("PRE_DIV1_ADJ");
            entity.Property(e => e.PreDiv1B).HasColumnName("PRE_DIV1_B");
            entity.Property(e => e.PreDiv2).HasColumnName("PRE_DIV2");
            entity.Property(e => e.PreDiv2Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV2_RATE");
            entity.Property(e => e.PreDiv2RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV2_RATE_M");
            entity.Property(e => e.PreDiv2Adj).HasColumnName("PRE_DIV2_ADJ");
            entity.Property(e => e.PreDiv3).HasColumnName("PRE_DIV3");
            entity.Property(e => e.PreDiv3Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV3_RATE");
            entity.Property(e => e.PreDiv3RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV3_RATE_M");
            entity.Property(e => e.PreDiv3Adj).HasColumnName("PRE_DIV3_ADJ");
            entity.Property(e => e.PreDiv4).HasColumnName("PRE_DIV4");
            entity.Property(e => e.PreDiv4Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV4_RATE");
            entity.Property(e => e.PreDiv4RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV4_RATE_M");
            entity.Property(e => e.PreDiv4Adj).HasColumnName("PRE_DIV4_ADJ");
            entity.Property(e => e.PreDiv5).HasColumnName("PRE_DIV5");
            entity.Property(e => e.PreDiv5Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV5_RATE");
            entity.Property(e => e.PreDiv5RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("PRE_DIV5_RATE_M");
            entity.Property(e => e.PreDiv5Adj).HasColumnName("PRE_DIV5_ADJ");
            entity.Property(e => e.Div1).HasColumnName("DIV1");
            entity.Property(e => e.Div1Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV1_RATE");
            entity.Property(e => e.Div1RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV1_RATE_M");
            entity.Property(e => e.Div1B).HasColumnName("DIV1_B");
            entity.Property(e => e.Div2).HasColumnName("DIV2");
            entity.Property(e => e.Div2Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV2_RATE");
            entity.Property(e => e.Div2RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV2_RATE_M");
            entity.Property(e => e.Div2Adj).HasColumnName("DIV2_ADJ");
            entity.Property(e => e.Div3).HasColumnName("DIV3");
            entity.Property(e => e.Div3Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV3_RATE");
            entity.Property(e => e.Div3RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV3_RATE_M");
            entity.Property(e => e.Div3Adj).HasColumnName("DIV3_ADJ");
            entity.Property(e => e.Div4).HasColumnName("DIV4");
            entity.Property(e => e.Div4Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV4_RATE");
            entity.Property(e => e.Div4RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV4_RATE_M");
            entity.Property(e => e.Div4Adj).HasColumnName("DIV4_ADJ");
            entity.Property(e => e.Div5).HasColumnName("DIV5");
            entity.Property(e => e.Div5Rate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV5_RATE");
            entity.Property(e => e.Div5RateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV5_RATE_M");
            entity.Property(e => e.Div5Adj).HasColumnName("DIV5_ADJ");
            entity.Property(e => e.Fee).HasColumnName("FEE");
            entity.Property(e => e.FeeRate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("FEE_RATE");
            entity.Property(e => e.FeeRateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("FEE_RATE_M");
            entity.Property(e => e.DivTot).HasColumnName("DIV_TOT");
            entity.Property(e => e.DivRate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV_RATE");
            entity.Property(e => e.DivRateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV_RATE_M");
            entity.Property(e => e.DivRateO)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("DIV_RATE_O");
            entity.Property(e => e.Capital).HasColumnName("CAPITAL");
            entity.Property(e => e.CapitalRate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("CAPITAL_RATE");
            entity.Property(e => e.CapitalRateM)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("CAPITAL_RATE_M");
            entity.Property(e => e.CapitalAdj).HasColumnName("CAPITAL_ADJ");
            entity.Property(e => e.DivObj)
                .HasColumnType("decimal(12, 6)")
                .HasColumnName("DIV_OBJ");
            entity.Property(e => e.DivPre)
                .HasColumnType("decimal(12, 6)")
                .HasColumnName("DIV_PRE");
            entity.Property(e => e.Step1CreEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP1_CRE_EMP");
            entity.Property(e => e.Step1CreTime).HasColumnName("STEP1_CRE_TIME");
            entity.Property(e => e.Step1CofEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP1_COF_EMP");
            entity.Property(e => e.Step1CofTime).HasColumnName("STEP1_COF_TIME");
            entity.Property(e => e.Step1Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("STEP1_STATUS");
            entity.Property(e => e.Step2CreEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP2_CRE_EMP");
            entity.Property(e => e.Step2CreTime).HasColumnName("STEP2_CRE_TIME");
            entity.Property(e => e.Step2CofEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP2_COF_EMP");
            entity.Property(e => e.Step2CofTime).HasColumnName("STEP2_COF_TIME");
            entity.Property(e => e.Step2Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("STEP2_STATUS");
            entity.Property(e => e.Step3CreEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP3_CRE_EMP");
            entity.Property(e => e.Step3CreTime).HasColumnName("STEP3_CRE_TIME");
            entity.Property(e => e.Step3CofEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP3_COF_EMP");
            entity.Property(e => e.Step3CofTime).HasColumnName("STEP3_COF_TIME");
            entity.Property(e => e.Step3Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("STEP3_STATUS");
            entity.Property(e => e.Step4CreEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP4_CRE_EMP");
            entity.Property(e => e.Step4CreTime).HasColumnName("STEP4_CRE_TIME");
            entity.Property(e => e.Step4CofEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP4_COF_EMP");
            entity.Property(e => e.Step4CofTime).HasColumnName("STEP4_COF_TIME");
            entity.Property(e => e.Step4Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("STEP4_STATUS");
            entity.Property(e => e.Step5CreEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP5_CRE_EMP");
            entity.Property(e => e.Step5CreTime).HasColumnName("STEP5_CRE_TIME");
            entity.Property(e => e.Step5CofEmp)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("STEP5_COF_EMP");
            entity.Property(e => e.Step5CofTime).HasColumnName("STEP5_COF_TIME");
            entity.Property(e => e.Step5Status)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("STEP5_STATUS");
            entity.Property(e => e.IRate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("I_RATE");
            entity.Property(e => e.CRate)
                .HasColumnType("decimal(12, 10)")
                .HasColumnName("C_RATE");
            // 這些欄位在實際資料表中不存在，使用 Ignore 忽略
            entity.Ignore(e => e.Status);
            entity.Ignore(e => e.StatusC);
            entity.Ignore(e => e.UpdUser);
            entity.Ignore(e => e.UpdDate);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
