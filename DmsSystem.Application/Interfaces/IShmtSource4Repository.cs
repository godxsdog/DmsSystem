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
        // 定義一個可以一次新增多筆資料的方法
        Task AddRangeAsync(IEnumerable<ShmtSource4> entities);
    }
}