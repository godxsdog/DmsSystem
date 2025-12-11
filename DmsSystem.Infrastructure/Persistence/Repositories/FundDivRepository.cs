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
        var query = from fd in _context.Set<FundDiv>()
                    join f in _context.Set<Fund>() on fd.FundNo equals f.FundNo into fundGroup
                    from f in fundGroup.DefaultIfEmpty()
                    select new { fd, FundName = f != null ? f.FundName : null };

        if (!string.IsNullOrEmpty(fundNo))
        {
            query = query.Where(x => x.fd.FundNo == fundNo);
        }

        if (!string.IsNullOrEmpty(dividendType))
        {
            query = query.Where(x => x.fd.DividendType == dividendType);
        }

        if (startDate.HasValue)
        {
            query = query.Where(x => x.fd.DividendDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(x => x.fd.DividendDate <= endDate.Value);
        }

        var result = await query
            .OrderByDescending(x => x.fd.DividendDate)
            .ThenBy(x => x.fd.FundNo)
            .ToListAsync();

        return result.Select(x => 
        {
            x.fd.FundName = x.FundName;
            return x.fd;
        });
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
