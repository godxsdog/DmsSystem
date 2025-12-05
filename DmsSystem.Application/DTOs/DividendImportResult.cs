namespace DmsSystem.Application.DTOs;

public record DividendImportResult(
    bool Success,
    int Inserted,
    int Updated,
    int Failed,
    List<string> Errors);

public record DividendConfirmResult(
    bool Success,
    string Message,
    decimal? Nav,
    decimal? Unit,
    decimal? DivTotal,
    decimal? DivRate,
    decimal? DivRateMonthly,
    decimal? CapitalRate);

