using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.Persistence.Contexts; // 引用 DbContext
using Microsoft.EntityFrameworkCore;

namespace DmsSystem.Infrastructure.Persistence.Repositories
{
    public class ShmtParRepository : IShmtParRepository
    {
        private readonly DmsDbContext _context;

        // 透過建構函式注入 DmsDbContext
        public ShmtParRepository(DmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ShmtPar>> GetAllAsync()
        {
            // 使用 EF Core 的 LINQ 語法來查詢資料
            return await _context.ShmtPars.ToListAsync();
        }
    }
}