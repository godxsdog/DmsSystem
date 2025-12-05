using DmsSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DmsSystem.Application.Interfaces
{
    /// <summary>
    /// 提供「公司基本資料 (RIS.SHMT_SOURCE4)」的資料存取合約。
    /// </summary>
    public interface IShmtSource4Repository
    {
        Task AddRangeAsync(IEnumerable<ShmtSource4> entities);
        Task<IEnumerable<ShmtSource4>> GetAllAsync();
    }
}