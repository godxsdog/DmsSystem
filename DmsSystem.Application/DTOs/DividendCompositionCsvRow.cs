namespace DmsSystem.Application.DTOs;

public class DividendCompositionCsvRow
{
    public string FundMasterNo { get; set; } = default!;
    public string FundNo { get; set; } = default!;
    public DateTime DividendDate { get; set; }
    public string DividendType { get; set; } = default!;
    public decimal InterestRate { get; set; }
    public decimal CapitalRate { get; set; }
}
