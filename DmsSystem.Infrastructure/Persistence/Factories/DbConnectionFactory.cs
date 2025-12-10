using System.Data.Common;
using DmsSystem.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DmsSystem.Infrastructure.Persistence.Factories;

/// <summary>
/// 資料庫連接工廠實作，透過 Configuration 建立新的資料庫連接
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("資料庫連接字串 'DefaultConnection' 未設定");
    }

    public DbConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
