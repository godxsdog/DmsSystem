using System.Collections.Generic;

namespace DmsSystem.Application.Interfaces
{
    public interface IExcelGenerator
    {
        // 我們設計一個通用的方法
        byte[] GenerateExcel<T>(IEnumerable<T> data, string sheetName = "Sheet1");
    }
}