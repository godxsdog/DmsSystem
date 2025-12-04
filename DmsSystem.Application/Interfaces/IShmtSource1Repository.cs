using DmsSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DmsSystem.Application.Interfaces
{
    /// <summary>
    /// 提供「股東會明細 (RIS.SHMT_SOURCE1)」的資料存取合約。
    /// </summary>
    public interface IShmtSource1Repository
    {
        Task AddRangeAsync(IEnumerable<ShmtSource1> entities);
    }
}
