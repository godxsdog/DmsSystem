using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;

namespace DmsSystem.Application.Services;

/// <summary>
/// 實作處理「公司資訊」檔案上傳的服務。
/// </summary>
public class CompanyInfoUploadService : ICompanyInfoUploadService
{
    private readonly IShmtSource4Repository _repository;
    private readonly IFileParser<ShmtSource4> _fileParser;

    public CompanyInfoUploadService(
        IShmtSource4Repository repository,
        IFileParser<ShmtSource4> fileParser)
    {
        _repository = repository;
        _fileParser = fileParser;
    }

    public async Task<(bool Success, string Message, int RowsAdded)> ProcessShmtSource4UploadAsync(Stream fileStream, string fileName)
    {
        try
        {
            var entitiesToInsert = await _fileParser.ParseAsync(fileStream, fileName);

            if (entitiesToInsert.Count > 0)
            {
                await _repository.AddRangeAsync(entitiesToInsert);
            }

            return (true, $"成功載入 {entitiesToInsert.Count} 筆資料。", entitiesToInsert.Count);
        }
        catch (Exception ex)
        {
            return (false, $"處理檔案時發生錯誤: {ex.Message}", 0);
        }
    }
}

