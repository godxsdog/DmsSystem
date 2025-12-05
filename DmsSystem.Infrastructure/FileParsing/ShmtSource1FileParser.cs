using CsvHelper;
using CsvHelper.Configuration;
using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using NPOI.SS.UserModel;
using System.Globalization;
using System.Text;

namespace DmsSystem.Infrastructure.FileParsing;

/// <summary>
/// ShmtSource1 專用的檔案解析器
/// </summary>
public class ShmtSource1FileParser : IFileParser<ShmtSource1>
{
    public Task<List<ShmtSource1>> ParseAsync(Stream fileStream, string fileName)
    {
        string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

        if (fileExtension == ".xlsx")
        {
            return Task.FromResult(ParseXlsxStream(fileStream));
        }
        else if (fileExtension == ".csv")
        {
            return Task.FromResult(ParseCsvStream(fileStream));
        }
        else
        {
            throw new NotSupportedException($"不支援的檔案格式: {fileExtension}");
        }
    }

    private List<ShmtSource1> ParseCsvStream(Stream fileStream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Encoding = Encoding.GetEncoding("Big5")
        };

        using var reader = new StreamReader(fileStream, config.Encoding);
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<ShmtSource1Map>();

        csv.Read(); // 跳過標頭行

        var records = csv.GetRecords<ShmtSource1>().ToList();

        // 套用業務邏輯
        foreach (var record in records)
        {
            record.AcDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.EmpNo = "A00994";
            record.Status = "Y";
        }

        return records;
    }

    private List<ShmtSource1> ParseXlsxStream(Stream fileStream)
    {
        var entities = new List<ShmtSource1>();
        IWorkbook workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fileStream);
        ISheet worksheet = workbook.GetSheetAt(0);

        for (int row = 1; row <= worksheet.LastRowNum; row++)
        {
            IRow currentRow = worksheet.GetRow(row);
            if (currentRow == null) continue;

            var entity = new ShmtSource1
            {
                AcDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                EmpNo = "A00994",
                Status = "Y",
                StkCd = GetCellStringValue(currentRow.GetCell(0)),
                StkName = GetCellStringValue(currentRow.GetCell(1)),
                ShmtDate = GetCellStringValue(currentRow.GetCell(2)),
                SsrgDate = GetCellStringValue(currentRow.GetCell(3)),
                ChfChgYn = GetCellStringValue(currentRow.GetCell(4)),
                ShmtAddr = GetCellStringValue(currentRow.GetCell(5)),
                Type = GetCellStringValue(currentRow.GetCell(6))
            };
            entities.Add(entity);
        }

        return entities;
    }

    private string GetCellStringValue(ICell? cell)
    {
        if (cell == null) return string.Empty;
        return cell.ToString().Trim();
    }
}

/// <summary>
/// CsvHelper 的 ClassMap 定義
/// </summary>
public sealed class ShmtSource1Map : ClassMap<ShmtSource1>
{
    public ShmtSource1Map()
    {
        Map(m => m.StkCd).Index(0);
        Map(m => m.StkName).Index(1);
        Map(m => m.ShmtDate).Index(2);
        Map(m => m.SsrgDate).Index(3);
        Map(m => m.ChfChgYn).Index(4);
        Map(m => m.ShmtAddr).Index(5);
        Map(m => m.Type).Index(6);
    }
}

