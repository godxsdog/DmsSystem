using DmsSystem.Domain.Entities;
using System.Threading.Tasks;

namespace DmsSystem.Application.Interfaces
{
    /// <summary>
    /// 提供「股票基本資料表 (RIS.EQUITY)」的資料存取合約。
    /// </summary>
    public interface IEquityRepository
    {
        Task<Equity> FindByIsinAsync(string isin);
    }
}