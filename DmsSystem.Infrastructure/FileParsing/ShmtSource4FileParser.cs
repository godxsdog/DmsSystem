using CsvHelper;
using CsvHelper.Configuration;
using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using NPOI.SS.UserModel;
using System.Globalization;
using System.Text;

namespace DmsSystem.Infrastructure.FileParsing;

/// <summary>
/// ShmtSource4 專用的檔案解析器
/// </summary>
public class ShmtSource4FileParser : IFileParser<ShmtSource4>
{
    public async Task<List<ShmtSource4>> ParseAsync(Stream fileStream, string fileName)
    {
        string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

        if (fileExtension == ".xlsx")
        {
            return ParseXlsxStream(fileStream);
        }
        else if (fileExtension == ".csv")
        {
            return ParseCsvStream(fileStream);
        }
        else
        {
            throw new NotSupportedException($"不支援的檔案格式: {fileExtension}");
        }
    }

    private List<ShmtSource4> ParseCsvStream(Stream fileStream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Encoding = Encoding.GetEncoding("Big5")
        };

        using var reader = new StreamReader(fileStream, config.Encoding);
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<ShmtSource4Map>();

        var records = csv.GetRecords<ShmtSource4>().ToList();

        // 套用業務邏輯
        foreach (var record in records)
        {
            record.AcDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            record.EmpNo = "A00994";
        }

        return records;
    }

    private List<ShmtSource4> ParseXlsxStream(Stream fileStream)
    {
        var entities = new List<ShmtSource4>();
        IWorkbook workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fileStream);
        ISheet worksheet = workbook.GetSheetAt(0);

        for (int row = 1; row <= worksheet.LastRowNum; row++)
        {
            IRow currentRow = worksheet.GetRow(row);
            if (currentRow == null) continue;

            var entity = new ShmtSource4
            {
                AcDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                EmpNo = "A00994",
                StkCd = GetCellStringValue(currentRow.GetCell(0)),
                StkName = GetCellStringValue(currentRow.GetCell(1)),
                CompName = GetCellStringValue(currentRow.GetCell(2)),
                Tel = GetCellStringValue(currentRow.GetCell(3)),
                Addr = GetCellStringValue(currentRow.GetCell(4)),
                BrokerName = GetCellStringValue(currentRow.GetCell(5)),
                BrokerTel = GetCellStringValue(currentRow.GetCell(6)),
                Spokesman = GetCellStringValue(currentRow.GetCell(7)),
                President = GetCellStringValue(currentRow.GetCell(8)),
                Chairman = GetCellStringValue(currentRow.GetCell(9)),
                IdNo = GetCellStringValue(currentRow.GetCell(10))
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
public sealed class ShmtSource4Map : ClassMap<ShmtSource4>
{
    public ShmtSource4Map()
    {
        Map(m => m.StkCd).Name("股票代號");
        Map(m => m.StkName).Name("股票名稱");
        Map(m => m.CompName).Name("公司名稱");
        Map(m => m.Tel).Name("電話");
        Map(m => m.Addr).Name("地址");
        Map(m => m.BrokerName).Name("股票過戶機構");
        Map(m => m.BrokerTel).Name("股務代理電話");
        Map(m => m.Spokesman).Name("發言人");
        Map(m => m.President).Name("總經理");
        Map(m => m.Chairman).Name("董事長");
        Map(m => m.IdNo).Name("統一編號");
    }
}

