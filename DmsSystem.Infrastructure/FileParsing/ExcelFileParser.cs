using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace DmsSystem.Infrastructure.FileParsing;

/// <summary>
/// Excel 檔案解析器實作（使用 NPOI）
/// </summary>
public class ExcelFileParser<T> : IFileParser<T> where T : class
{
    public Task<List<T>> ParseAsync(Stream fileStream, string fileName)
    {
        var entities = new List<T>();
        IWorkbook workbook = new XSSFWorkbook(fileStream);
        ISheet worksheet = workbook.GetSheetAt(0);

        // 從第二行開始讀取資料（跳過標頭）
        for (int row = 1; row <= worksheet.LastRowNum; row++)
        {
            IRow currentRow = worksheet.GetRow(row);
            if (currentRow == null) continue;

            var entity = ParseRow(currentRow);
            if (entity != null)
            {
                entities.Add(entity);
            }
        }

        return Task.FromResult(entities);
    }

    protected virtual T? ParseRow(IRow row)
    {
        // 子類別需要實作此方法
        throw new NotImplementedException("子類別必須實作 ParseRow 方法");
    }

    protected string GetCellStringValue(ICell? cell)
    {
        if (cell == null) return string.Empty;
        return cell.ToString().Trim();
    }
}

