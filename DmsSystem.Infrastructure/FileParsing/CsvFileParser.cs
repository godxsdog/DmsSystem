using CsvHelper;
using CsvHelper.Configuration;
using DmsSystem.Application.Interfaces;
using System.Globalization;
using System.Text;

namespace DmsSystem.Infrastructure.FileParsing;

/// <summary>
/// CSV 檔案解析器實作（使用 CsvHelper）
/// </summary>
public class CsvFileParser<T> : IFileParser<T> where T : class
{
    private readonly CsvConfiguration _config;
    private readonly bool _hasHeader;
    private readonly bool _skipFirstRow;

    public CsvFileParser(bool hasHeader = true, bool skipFirstRow = false)
    {
        _hasHeader = hasHeader;
        _skipFirstRow = skipFirstRow;
        _config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = hasHeader,
            Encoding = Encoding.GetEncoding("Big5")
        };
    }

    public Task<List<T>> ParseAsync(Stream fileStream, string fileName)
    {
        using var reader = new StreamReader(fileStream, _config.Encoding);
        using var csv = new CsvReader(reader, _config);

        // 註冊 ClassMap（如果有的話）
        RegisterClassMap(csv);

        // 跳過第一行（標頭）
        if (_skipFirstRow)
        {
            csv.Read();
        }

        var records = csv.GetRecords<T>().ToList();
        return Task.FromResult(records);
    }

    protected virtual void RegisterClassMap(CsvReader csv)
    {
        // 子類別可以覆寫此方法來註冊自訂的 ClassMap
    }
}

