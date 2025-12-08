using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;

namespace DmsSystem.Application.Services;

/// <summary>
/// 股東會明細檔案上傳服務實作
/// </summary>
public class ShareholderMeetingDetailService : IShareholderMeetingDetailService
{
    private readonly IShmtSource1Repository _repository;
    private readonly IFileParser<ShmtSource1> _fileParser;

    public ShareholderMeetingDetailService(
        IShmtSource1Repository repository,
        IFileParser<ShmtSource1> fileParser)
    {
        _repository = repository;
        _fileParser = fileParser;
    }

    public async Task<(bool Success, string Message, int RowsAdded)> ProcessUploadAsync(Stream fileStream, string fileName)
    {
        try
        {
            var entitiesToInsert = await _fileParser.ParseAsync(fileStream, fileName);

            if (entitiesToInsert.Count > 0)
            {
                await _repository.AddRangeAsync(entitiesToInsert);
            }

            return (true, $"成功載入 {entitiesToInsert.Count} 筆資料到 ris.shmtsource1。", entitiesToInsert.Count);
        }
        catch (Exception ex)
        {
            return (false, $"處理檔案時發生錯誤: {ex.Message}", 0);
        }
    }
}

