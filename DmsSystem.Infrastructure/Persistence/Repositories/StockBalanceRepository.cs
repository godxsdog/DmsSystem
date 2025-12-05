using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DmsSystem.Infrastructure.Persistence.Repositories
{
    public class StockBalanceRepository : IStockBalanceRepository
    {
        private readonly DmsDbContext _context;
        public StockBalanceRepository(DmsDbContext context) { _context = context; }

        public async Task<StockBalance> FindByKeyAsync(string id, string contractSeq, DateOnly acDate, string stockNo)
        {
            return await _context.StockBalances
                .FirstOrDefaultAsync(b => b.Id == id && b.ContractSeq == contractSeq && b.AcDate == acDate && b.StockNo == stockNo);
        }

        public async Task AddAsync(StockBalance entity)
        {
            await _context.StockBalances.AddAsync(entity);
        }

        public void Update(StockBalance entity)
        {
            _context.StockBalances.Update(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockBalance>> GetAllAsync()
        {
            return await Task.FromResult(_context.StockBalances.ToList());
        }
    }
}