namespace DmsSystem.Application.DTOs;

public record DividendImportResult(
    bool Success,
    int Inserted,
    int Updated,
    int Failed,
    List<string> Errors,
    List<ImportedRecordKey>? ImportedKeys = null);

public record ImportedRecordKey(
    string FundNo,
    DateTime DividendDate,
    string DividendType);


public record DividendConfirmResult(
    bool Success,
    string Message,
    decimal? Nav,
    decimal? Unit,
    decimal? DivTotal,
    decimal? DivRate,
    decimal? DivRateMonthly,
    decimal? CapitalRate);

public record BatchConfirmResult(
    int TotalCount,
    int SuccessCount,
    int FailureCount,
    List<string> Errors);
