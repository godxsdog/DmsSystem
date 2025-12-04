using DmsSystem.Application.Interfaces;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DmsSystem.Infrastructure.FileGeneration
{
    public class NpoiExcelGenerator : IExcelGenerator
    {
        public byte[] GenerateExcel<T>(IEnumerable<T> data, string sheetName = "Sheet1")
        {
            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(sheetName);

            PropertyInfo[] properties = typeof(T).GetProperties();

            // 建立標頭
            IRow headerRow = sheet.CreateRow(0);
            for (int i = 0; i < properties.Length; i++)
            {
                headerRow.CreateCell(i).SetCellValue(properties[i].Name);
            }

            // 填充資料
            int rowNum = 1;
            foreach (var item in data)
            {
                IRow row = sheet.CreateRow(rowNum++);
                for (int i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(item);
                    row.CreateCell(i).SetCellValue(value?.ToString() ?? string.Empty);
                }
            }

            using (var ms = new MemoryStream())
            {
                workbook.Write(ms);
                return ms.ToArray();
            }
        }
    }
}