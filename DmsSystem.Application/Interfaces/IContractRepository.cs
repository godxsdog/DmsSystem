using DmsSystem.Domain.Entities;
using System.Collections.Generic; // 為了 IEnumerable
using System.Threading.Tasks;

namespace DmsSystem.Application.Interfaces
{
    /// <summary>
    /// 提供「契約資料表 (DMS.CONTRACT)」的資料存取合約。
    /// </summary>
    public interface IContractRepository
    {
        /// <summary>
        /// (舊有功能) 取得所有契約資料。
        /// </summary>
        Task<IEnumerable<Contract>> GetAllAsync();

        /// <summary>
        /// (新增功能) 根據 PowerBuilder 程式中的 PCODE 轉換邏輯來查找合約。
        /// </summary>
        Task<Contract> FindByPcodeAsync(string pcode);
    }
}