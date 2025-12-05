using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DmsSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// 「股東會明細 (RIS.SHMT_SOURCE1)」的資料存取實作。
    /// </summary>
    public class ShmtSource1Repository : IShmtSource1Repository
    {
        private readonly DmsDbContext _context;

        public ShmtSource1Repository(DmsDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<ShmtSource1> entities)
        {
            // 注意：EF Core 會根據您的類別名稱，自動找到對應的 DbSet，例如 _context.ShmtSource1s
            await _context.ShmtSource1s.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShmtSource1>> GetAllAsync()
        {
            return await Task.FromResult(_context.ShmtSource1s.ToList());
        }
    }
}