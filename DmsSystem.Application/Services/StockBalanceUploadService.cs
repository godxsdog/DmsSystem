using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DmsSystem.Application.Services;

/// <summary>
/// 實作處理「股票餘額」檔案上傳的服務。
/// </summary>
public class StockBalanceUploadService : IStockBalanceUploadService
{
    private readonly IStockBalanceRepository _stockBalanceRepo;
    private readonly IContractRepository _contractRepo;
    private readonly IEquityRepository _equityRepo;
    private readonly IFileParser<StockBalanceCsvRecord> _fileParser;
    private readonly ILogger<StockBalanceUploadService> _logger;

    public StockBalanceUploadService(
        IStockBalanceRepository stockBalanceRepo,
        IContractRepository contractRepo,
        IEquityRepository equityRepo,
        IFileParser<StockBalanceCsvRecord> fileParser,
        ILogger<StockBalanceUploadService> logger)
    {
        _stockBalanceRepo = stockBalanceRepo;
        _contractRepo = contractRepo;
        _equityRepo = equityRepo;
        _fileParser = fileParser;
        _logger = logger;
    }

    public async Task<(bool Success, string Message, int RowsAdded, int RowsUpdated, int RowsFailed)> ProcessUploadAsync(Stream fileStream, string fileName)
    {
        int successCount = 0;
        int updatedCount = 0;
        int failedCount = 0;
        var errorMessages = new List<string>();
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("開始處理檔案: {FileName}", fileName);

        List<StockBalanceCsvRecord> csvRecords;

        try
        {
            csvRecords = await _fileParser.ParseAsync(fileStream, fileName);
            _logger.LogInformation("成功讀取 {Count} 筆 CSV 紀錄", csvRecords.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "讀取或解析 CSV 檔案時發生錯誤");
            return (false, $"處理失敗：讀取 CSV 檔案時發生錯誤。\n失敗原因: {ex.Message}", 0, 0, 1);
        }

        if (csvRecords.Count == 0)
        {
            _logger.LogWarning("CSV 檔案為空或只包含標頭");
            return (false, "CSV 檔案為空或只包含標頭。", 0, 0, 0);
        }

        // 開始逐筆處理紀錄
        for (int i = 0; i < csvRecords.Count; i++)
        {
            int currentRowNum = i + 2; // +2 是因為 List 從 0 開始，且我們跳過了標頭行
            var record = csvRecords[i];
            var rowStopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogDebug("開始處理第 {RowNum} 行: PCODE={Pcode}, ISIN={Isin}", currentRowNum, record.Pcode, record.Isin);

                if (string.IsNullOrEmpty(record.Pcode) || string.IsNullOrEmpty(record.AcDate) || string.IsNullOrEmpty(record.Isin))
                {
                    failedCount++;
                    string msg = $"資料行 {currentRowNum}: PCODE, AC_DATE 或 ISIN 欄位為空。";
                    errorMessages.Add(msg);
                    _logger.LogWarning(msg);
                    continue;
                }

                string transformedPcode = TransformPcode(record.Pcode);
                _logger.LogDebug("行 {RowNum}: PCODE 轉換為 {TransformedPcode}", currentRowNum, transformedPcode);

                var contract = await _contractRepo.FindByPcodeAsync(transformedPcode);
                if (contract == null)
                {
                    failedCount++;
                    string msg = $"資料行 {currentRowNum}: PCODE '{record.Pcode}' (轉換後為 '{transformedPcode}') 找不到對應的 Contract。";
                    errorMessages.Add(msg);
                    _logger.LogWarning(msg);
                    continue;
                }
                _logger.LogDebug("行 {RowNum}: 找到 Contract ID={Id}, Seq={Seq}", currentRowNum, contract.Id, contract.ContractSeq);

                var equity = await _equityRepo.FindByIsinAsync(record.Isin);
                if (equity == null)
                {
                    failedCount++;
                    string msg = $"資料行 {currentRowNum}: ISIN '{record.Isin}' 找不到對應的 Equity。";
                    errorMessages.Add(msg);
                    _logger.LogWarning(msg);
                    continue;
                }
                _logger.LogDebug("行 {RowNum}: 找到 Equity StkCd={StkCd}", currentRowNum, equity.StkCd);

                if (!DateTime.TryParseExact(record.AcDate, "yyyy/M/d", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
                {
                    failedCount++;
                    string msg = $"資料行 {currentRowNum}: 日期格式錯誤 '{record.AcDate}' (應為 yyyy/M/d)。";
                    errorMessages.Add(msg);
                    _logger.LogWarning(msg);
                    continue;
                }
                var acDateOnly = DateOnly.FromDateTime(parsedDateTime);
                _logger.LogDebug("行 {RowNum}: 日期轉換成功 {AcDate}", currentRowNum, acDateOnly);

                var existingBalance = await _stockBalanceRepo.FindByKeyAsync(contract.Id, contract.ContractSeq, acDateOnly, equity.StkCd);

                if (existingBalance == null)
                {
                    _logger.LogDebug("行 {RowNum}: 資料不存在，準備新增", currentRowNum);
                    var newBalance = new StockBalance
                    {
                        Id = contract.Id,
                        ContractSeq = contract.ContractSeq,
                        ImpDate = acDateOnly,
                        ImpId = "ADMIN",
                        AcDate = acDateOnly,
                        CurrencyNo = "NTD",
                        StockNo = equity.StkCd,
                        Shares = record.Shares,
                        AssetType = "1"
                    };

                    try
                    {
                        await _stockBalanceRepo.AddAsync(newBalance);
                        successCount++;
                        _logger.LogDebug("行 {RowNum}: 新增操作已標記", currentRowNum);
                    }
                    catch (Exception dbEx)
                    {
                        failedCount++;
                        string msg = $"資料行 {currentRowNum}: 新增 PCODE '{record.Pcode}', ISIN '{record.Isin}' 時資料庫錯誤: {dbEx.InnerException?.Message ?? dbEx.Message}";
                        errorMessages.Add(msg);
                        _logger.LogError(dbEx, msg);
                    }
                }
                else
                {
                    _logger.LogDebug("行 {RowNum}: 資料已存在，準備更新 Shares", currentRowNum);
                    existingBalance.Shares = record.Shares;
                    updatedCount++;
                    _logger.LogDebug("行 {RowNum}: 更新 Shares 操作已標記", currentRowNum);
                }
            }
            catch (Exception ex)
            {
                failedCount++;
                string msg = $"處理 CSV 資料行 {currentRowNum} (PCODE: {record?.Pcode}, ISIN: {record?.Isin}) 時發生未預期錯誤: {ex.Message}";
                errorMessages.Add(msg);
                _logger.LogError(ex, msg);
            }
            finally
            {
                rowStopwatch.Stop();
                _logger.LogDebug("處理第 {RowNum} 行完成，耗時: {ElapsedMs} ms", currentRowNum, rowStopwatch.ElapsedMilliseconds);
            }
        }

        _logger.LogInformation("所有 CSV 行處理完畢，準備儲存資料庫變更...");
        try
        {
            await _stockBalanceRepo.SaveChangesAsync();
            _logger.LogInformation("資料庫變更儲存成功");
        }
        catch (Exception dbEx)
        {
            failedCount += (successCount + updatedCount);
            successCount = 0;
            updatedCount = 0;
            string msg = $"儲存資料庫變更時發生錯誤 (可能是整體交易失敗): {dbEx.InnerException?.Message ?? dbEx.Message}";
            errorMessages.Add(msg);
            _logger.LogError(dbEx, msg);
        }

        stopwatch.Stop();
        _logger.LogInformation("全部處理完成，總耗時: {ElapsedMs} ms", stopwatch.ElapsedMilliseconds);

        string finalMessage;
        if (failedCount == 0)
        {
            finalMessage = $"處理完成。\n結果：所有資料處理成功 (新增 {successCount} 筆，更新 {updatedCount} 筆)。";
        }
        else
        {
            finalMessage = $"處理完成：\n結果：有 {failedCount} 項更新失敗 (新增 {successCount} 筆，更新 {updatedCount} 筆)。\n失敗原因 (最多顯示 10 筆): {string.Join("; ", errorMessages.Take(10))}";
        }

        _logger.LogInformation("回傳結果: Success={Success}, Message={Message}", failedCount == 0, finalMessage);
        return (failedCount == 0, finalMessage, successCount, updatedCount, failedCount);
    }

    private string TransformPcode(string? pcode)
    {
        if (pcode != null && pcode.Length == 4 && pcode.StartsWith("TT0") && char.IsDigit(pcode[3]))
        {
            return "TT" + pcode.Substring(3);
        }
        return pcode ?? string.Empty;
    }
}

