// IExcelUploadService.cs
namespace DmsSystem.Application.Interfaces
{
    public interface ICompanyInfoUploadService
    {
        // 定義一個方法，接收檔案串流，並回傳處理結果
        // IExcelUploadService.cs
        Task<(bool Success, string Message, int RowsAdded)> ProcessShmtSource4UploadAsync(Stream fileStream, string fileName);
    }
}