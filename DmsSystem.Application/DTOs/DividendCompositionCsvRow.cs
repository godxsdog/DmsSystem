namespace DmsSystem.Application.DTOs;

public class DividendCompositionCsvRow
{
    public string FundMasterNo { get; set; } = default!;
    public string FundNo { get; set; } = default!;
    public string DividendDateStr { get; set; } = default!;
    public string DividendType { get; set; } = default!;
    public string InterestRateStr { get; set; } = default!;
    public string CapitalRateStr { get; set; } = default!;
}
