using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DmsSystem.Infrastructure.Persistence.Repositories;

/// <summary>
/// 目標配息率設定 Repository 實作
/// </summary>
public class FundDivObjRepository : IFundDivObjRepository
{
    private readonly DmsDbContext _context;

    public FundDivObjRepository(DmsDbContext context)
    {
        _context = context;
    }

    public async Task<FundDivObj?> GetEffectiveAsync(string fundNo, string divType, DateTime dividendDate)
    {
        return await _context.Set<FundDivObj>()
            .Where(f => f.FundNo == fundNo 
                && f.DivType == divType 
                && f.TxDate <= dividendDate)
            .OrderByDescending(f => f.TxDate)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(FundDivObj fundDivObj)
    {
        await _context.Set<FundDivObj>().AddAsync(fundDivObj);
    }

    public async Task UpdateAsync(FundDivObj fundDivObj)
    {
        _context.Set<FundDivObj>().Update(fundDivObj);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
