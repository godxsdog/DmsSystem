using DmsSystem.Application.DTOs;
using System.Collections.Generic;
using System.IO; // 為了 Stream
using System.Threading.Tasks;

namespace DmsSystem.Application.Interfaces
{
    /// <summary>
    /// 提供報表產生相關功能的服務合約。
    /// </summary>
    public interface IReportService
    {
        /// <summary>
        /// 取得股東會報表資料。
        /// </summary>
        Task<List<ShareholderReportDto>> GetShareholderReportDataAsync();

        /// <summary>
        /// 將股東會報表資料產生為 Excel 檔案串流。
        /// </summary>
        /// <param name="data">要匯出的資料。</param>
        /// <returns>包含 Excel 檔案內容的記憶體串流。</returns>
        MemoryStream GenerateShareholderReportExcel(List<ShareholderReportDto> data); // NPOI 非同步方法較少，改為同步
    }
}