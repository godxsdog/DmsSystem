namespace DmsSystem.Application.DTOs;

/// <summary>
/// 配息參數設定 DTO
/// </summary>
public record FundDivSetDto(
    string FundNo,
    string DivType,
    int? Item01Seq,
    int? Item02Seq,
    int? Item03Seq,
    int? Item04Seq,
    int? Item05Seq,
    int? Item06Seq,
    int? Item07Seq,
    int? Item08Seq,
    int? Item09Seq,
    int? Item10Seq,
    string? CapitalType,
    string? EmailList
);

/// <summary>
/// 目標配息率設定 DTO
/// </summary>
public record FundDivObjDto(
    string FundNo,
    string DivType,
    DateTime TxDate,
    decimal? DivObj,
    decimal? DivObjAmt
);
