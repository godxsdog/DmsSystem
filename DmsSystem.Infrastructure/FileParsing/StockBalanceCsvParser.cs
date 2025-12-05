using CsvHelper;
using CsvHelper.Configuration;
using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using System.Globalization;
using System.Text;

namespace DmsSystem.Infrastructure.FileParsing;

/// <summary>
/// StockBalance CSV 的 ClassMap
/// </summary>
public sealed class StockBalanceCsvRecordMap : ClassMap<StockBalanceCsvRecord>
{
    public StockBalanceCsvRecordMap()
    {
        Map(m => m.Pcode).Index(0);
        Map(m => m.AcDate).Index(1);
        Map(m => m.Isin).Index(2);
        Map(m => m.Shares).Index(3);
    }
}

/// <summary>
/// StockBalance CSV 檔案解析器
/// </summary>
public class StockBalanceCsvParser : IFileParser<StockBalanceCsvRecord>
{
    public Task<List<StockBalanceCsvRecord>> ParseAsync(Stream fileStream, string fileName)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Encoding = Encoding.GetEncoding("Big5")
        };

        var csvRecords = new List<StockBalanceCsvRecord>();

        using var reader = new StreamReader(fileStream, config.Encoding);
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<StockBalanceCsvRecordMap>();

        // 跳過標頭行
        csv.Read();

        csvRecords = csv.GetRecords<StockBalanceCsvRecord>().ToList();

        return Task.FromResult(csvRecords);
    }
}

