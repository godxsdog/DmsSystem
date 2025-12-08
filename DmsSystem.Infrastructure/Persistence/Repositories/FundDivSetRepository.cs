using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DmsSystem.Infrastructure.Persistence.Repositories;

/// <summary>
/// 配息參數設定 Repository 實作
/// </summary>
public class FundDivSetRepository : IFundDivSetRepository
{
    private readonly DmsDbContext _context;

    public FundDivSetRepository(DmsDbContext context)
    {
        _context = context;
    }

    public async Task<FundDivSet?> GetByFundNoAndDivTypeAsync(string fundNo, string divType)
    {
        return await _context.Set<FundDivSet>()
            .FirstOrDefaultAsync(f => f.FundNo == fundNo && f.DivType == divType);
    }

    public async Task AddAsync(FundDivSet fundDivSet)
    {
        await _context.Set<FundDivSet>().AddAsync(fundDivSet);
    }

    public async Task UpdateAsync(FundDivSet fundDivSet)
    {
        _context.Set<FundDivSet>().Update(fundDivSet);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
