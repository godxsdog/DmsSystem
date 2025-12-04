using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using DmsSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DmsSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// 「契約資料表 (DMS.CONTRACT)」的資料存取實作。
    /// </summary>
    public class ContractRepository : IContractRepository
    {
        private readonly DmsDbContext _context;

        public ContractRepository(DmsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// (舊有功能) 取得所有契約資料。
        /// </summary>
        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await _context.Contracts.ToListAsync();
        }


        /// <summary>
        /// (新增功能) 根據 PowerBuilder 程式中的 PCODE 轉換邏輯來查找合約。
        /// </summary>
        public async Task<Contract?> FindByPcodeAsync(string pcode) // 使用 Contract? 來表示可能回傳 null
        {
            // 因為查詢邏輯複雜，我們使用 FromSqlInterpolated 來執行原生 SQL
            var query = _context.Contracts
                .FromSqlInterpolated($"SELECT * FROM DMS.CONTRACT WHERE (CASE WHEN FUND_TYPE='D' THEN 'TD' + SUBSTRING(SER_NO,2,LEN(SER_NO)) WHEN FUND_TYPE <> 'D' THEN 'TT' + SUBSTRING(SER_NO,2,LEN(SER_NO)) END) = {pcode}");

            return await query.FirstOrDefaultAsync();
        }
    }
}