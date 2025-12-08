using System.Data.Common;

namespace DmsSystem.Application.Interfaces;

/// <summary>
/// 資料庫連接工廠介面，用於抽象資料庫連接的取得
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// 取得資料庫連接
    /// </summary>
    /// <returns>資料庫連接</returns>
    DbConnection GetConnection();
}
