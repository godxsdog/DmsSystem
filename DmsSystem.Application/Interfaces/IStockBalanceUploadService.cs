using System.IO;
using System.Threading.Tasks;

namespace DmsSystem.Application.Interfaces
{
    public interface IStockBalanceUploadService
    {
        Task<(bool Success, string Message, int RowsAdded, int RowsUpdated, int RowsFailed)> ProcessUploadAsync(Stream fileStream, string fileName);
    }
}