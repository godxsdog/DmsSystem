using System.Data.Common;
using DmsSystem.Application.Interfaces;
using DmsSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DmsSystem.Infrastructure.Persistence.Factories;

/// <summary>
/// 資料庫連接工廠實作，使用 DmsDbContext 取得資料庫連接
/// </summary>
public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly DmsDbContext _context;

    public DbConnectionFactory(DmsDbContext context)
    {
        _context = context;
    }

    public DbConnection GetConnection()
    {
        return _context.Database.GetDbConnection();
    }
}
