namespace DmsSystem.Application.Interfaces;

/// <summary>
/// 檔案解析器介面，用於解析 Excel 和 CSV 檔案
/// </summary>
/// <typeparam name="T">要解析的目標實體類型</typeparam>
public interface IFileParser<T> where T : class
{
    /// <summary>
    /// 從檔案串流解析資料
    /// </summary>
    /// <param name="fileStream">檔案串流</param>
    /// <param name="fileName">檔案名稱（用於判斷檔案類型）</param>
    /// <returns>解析後的實體列表</returns>
    Task<List<T>> ParseAsync(Stream fileStream, string fileName);
}

