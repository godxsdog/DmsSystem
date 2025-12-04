using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DmsSystem.Infrastructure.Persistence.Repositories
{
    public class EquityRepository : IEquityRepository
    {
        private readonly DmsDbContext _context;
        public EquityRepository(DmsDbContext context) { _context = context; }

        public async Task<Equity> FindByIsinAsync(string isin)
        {
            // 這個查詢邏輯很單純，直接使用 LINQ 即可
            return await _context.Equities.FirstOrDefaultAsync(e => e.IsinCode == isin);
        }
    }
}