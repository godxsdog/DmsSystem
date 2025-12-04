using System.IO;
using System.Threading.Tasks;

namespace DmsSystem.Application.Interfaces
{
    /// <summary>
    /// 定義處理「股東會明細」檔案上傳的服務合約。
    /// </summary>
    public interface IShareholderMeetingDetailService
    {
        Task<(bool Success, string Message, int RowsAdded)> ProcessUploadAsync(Stream fileStream, string fileName);
    }
}