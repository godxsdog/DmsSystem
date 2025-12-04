using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DmsSystem.Infrastructure.Persistence.Repositories
{
    public class ShmtSource4Repository : IShmtSource4Repository
    {
        private readonly DmsDbContext _context;

        public ShmtSource4Repository(DmsDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<ShmtSource4> entities)
        {
            // 使用 EF Core 的 AddRangeAsync 來提高批次新增的效率
            await _context.ShmtSource4s.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
    }
}