using System.Data.Common;

namespace DmsSystem.Application.Interfaces;

/// <summary>
/// 資料庫連接工廠介面，提供資料庫連接的抽象化
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// 取得資料庫連接
    /// </summary>
    /// <returns>資料庫連接物件</returns>
    DbConnection GetConnection();
}
