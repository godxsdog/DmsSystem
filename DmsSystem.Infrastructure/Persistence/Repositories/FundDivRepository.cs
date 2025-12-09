using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DmsSystem.Infrastructure.Persistence.Repositories;

/// <summary>
/// 可分配收益資料 Repository 實作
/// </summary>
public class FundDivRepository : IFundDivRepository
{
    private readonly DmsDbContext _context;

    public FundDivRepository(DmsDbContext context)
    {
        _context = context;
    }

    public async Task<FundDiv?> GetByKeyAsync(string fundNo, DateTime dividendDate, string dividendType)
    {
        return await _context.Set<FundDiv>()
            .FirstOrDefaultAsync(f => f.FundNo == fundNo 
                && f.DividendDate.Date == dividendDate.Date 
                && f.DividendType == dividendType);
    }

    public async Task AddAsync(FundDiv fundDiv)
    {
        await _context.Set<FundDiv>().AddAsync(fundDiv);
    }

    public async Task UpdateAsync(FundDiv fundDiv)
    {
        _context.Set<FundDiv>().Update(fundDiv);
        await Task.CompletedTask;
    }

    public async Task<FundDiv?> GetPreviousAsync(string fundNo, string dividendType, DateTime dividendDate)
    {
        return await _context.Set<FundDiv>()
            .Where(f => f.FundNo == fundNo 
                && f.DividendType == dividendType 
                && f.DividendDate < dividendDate)
            .OrderByDescending(f => f.DividendDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<FundDiv>> GetAllAsync(string? fundNo = null, string? dividendType = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Set<FundDiv>().AsQueryable();

        if (!string.IsNullOrEmpty(fundNo))
        {
            query = query.Where(f => f.FundNo == fundNo);
        }

        if (!string.IsNullOrEmpty(dividendType))
        {
            query = query.Where(f => f.DividendType == dividendType);
        }

        if (startDate.HasValue)
        {
            query = query.Where(f => f.DividendDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(f => f.DividendDate <= endDate.Value);
        }

        return await query
            .OrderByDescending(f => f.DividendDate)
            .ThenBy(f => f.FundNo)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
