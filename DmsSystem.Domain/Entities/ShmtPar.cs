using System;
using System.Collections.Generic;

namespace DmsSystem.Domain.Entities;

/// <summary>
/// 股東會基本資料表
/// </summary>
public partial class ShmtPar
{
    /// <summary>
    /// 股票代號
    /// </summary>
    public string StkCd { get; set; } = null!;

    /// <summary>
    /// 交易所代碼
    /// </summary>
    public string DealCtr { get; set; } = null!;

    /// <summary>
    /// 股東會開會日期
    /// </summary>
    public DateOnly? ShmtDate { get; set; }

    public string? ShmtTime { get; set; }

    public string? ShmtAddr { get; set; }

    public DateOnly? SsrgDate { get; set; }

    public DateOnly? LsrgDate { get; set; }

    public DateOnly? LsbyDate { get; set; }

    public int? Fund300Cnt { get; set; }

    public long? AllShares { get; set; }

    public string? AttendType { get; set; }

    public string? OuterType { get; set; }

    public string? ChfChgType { get; set; }

    public string? ChfChgYn { get; set; }

    public decimal? ChfChgRate1 { get; set; }

    public decimal? ChfChgRate2 { get; set; }

    public long? ChfChgValue { get; set; }

    public string? CompName { get; set; }

    public string? UpdUser { get; set; }

    public DateOnly? UpdDate { get; set; }

    public string? InputType { get; set; }

    public string? OutEmp { get; set; }

    public string? SeqNo { get; set; }

    public int? ChfChgCnt1 { get; set; }

    public int? ChfChgCnt2 { get; set; }

    public DateOnly? DataDate { get; set; }

    public byte[]? Par001 { get; set; }

    public byte[]? Par002 { get; set; }

    public byte[]? Par003 { get; set; }

    public string? ContactEmp1 { get; set; }

    public string? ContactEmp2 { get; set; }

    public string? ContactCheck { get; set; }

    public DateOnly? ContactDate { get; set; }

    public string? DocType1 { get; set; }

    public string? DocType2 { get; set; }

    public string? DocType3 { get; set; }

    public string? DocType4 { get; set; }

    public string? DocType5 { get; set; }

    public string? DocType6 { get; set; }

    public string? DocType7 { get; set; }

    public string? DocType8 { get; set; }

    public string? DocType9 { get; set; }

    public string? DelType { get; set; }

    public DateOnly? DelDate { get; set; }

    public string? Par001Status { get; set; }

    public string? Par002Status { get; set; }

    public string? Par003Status { get; set; }

    public DateOnly? Par001Date { get; set; }

    public DateOnly? Par002Date { get; set; }

    public DateOnly? Par003Date { get; set; }

    public string? ChfChgType1 { get; set; }

    public string? ChfChgType2 { get; set; }

    public string? DelReason { get; set; }

    public string? Type { get; set; }
}
