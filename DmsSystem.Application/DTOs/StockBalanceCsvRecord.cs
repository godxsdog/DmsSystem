namespace DmsSystem.Application.DTOs;

/// <summary>
/// StockBalance CSV 記錄的 DTO
/// </summary>
public class StockBalanceCsvRecord
{
    public string? Pcode { get; set; }
    public string? AcDate { get; set; }
    public string? Isin { get; set; }
    public decimal Shares { get; set; }
}

