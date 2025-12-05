using DmsSystem.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace DmsSystem.Application.Interfaces
{
    /// <summary>
    /// 提供「股票餘額表 (DMS.STOCK_BALANCE)」的資料存取合約。
    /// </summary>
    public interface IStockBalanceRepository
    {
        // 根據主鍵查找單一筆餘額紀錄
        Task<StockBalance> FindByKeyAsync(string id, string contractSeq, DateOnly acDate, string stockNo);

        // 新增一筆紀錄 (但不儲存)
        Task AddAsync(StockBalance entity);
        // 更新一筆紀錄 (但不儲fen)
        void Update(StockBalance entity);
        // 將所有變更一次性儲存到資料庫
        Task<int> SaveChangesAsync();
        // 取得所有股票餘額資料
        Task<IEnumerable<StockBalance>> GetAllAsync();
    }
}