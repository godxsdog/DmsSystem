using CsvHelper;
using CsvHelper.Configuration;
using DmsSystem.Application.Interfaces;
using DmsSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics; // 引用 Debug
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DmsSystem.Infrastructure.Services
{
    /// <summary>
    /// 用於 CsvHelper 讀取紀錄的 DTO
    /// 欄位依照 CSV 順序
    /// </summary>
    public class StockBalanceCsvRecord
    {
        public string? Pcode { get; set; }
        public string? AcDate { get; set; }
        public string? Isin { get; set; }
        public decimal Shares { get; set; }
    }

    /// <summary>
    /// 依照 PowerBuilder 邏輯，使用「欄位順序 (Index)」來對應 DTO
    /// </summary>
    public sealed class StockBalanceCsvRecordMap : ClassMap<StockBalanceCsvRecord>
    {
        public StockBalanceCsvRecordMap()
        {
            Map(m => m.Pcode).Index(0); // 第 1 欄
            Map(m => m.AcDate).Index(1); // 第 2 欄
            Map(m => m.Isin).Index(2); // 第 3 欄
            Map(m => m.Shares).Index(3); // 第 4 欄
        }
    }

    public class StockBalanceUploadService : IStockBalanceUploadService
    {
        private readonly IStockBalanceRepository _stockBalanceRepo;
        private readonly IContractRepository _contractRepo;
        private readonly IEquityRepository _equityRepo;

        // 【建議】注入 ILogger 以進行更正式的日誌記錄
        // private readonly ILogger<StockBalanceUploadService> _logger;
        // public StockBalanceUploadService(..., ILogger<StockBalanceUploadService> logger) { ... _logger = logger; }

        public StockBalanceUploadService(
            IStockBalanceRepository stockBalanceRepo,
            IContractRepository contractRepo,
            IEquityRepository equityRepo)
        {
            _stockBalanceRepo = stockBalanceRepo;
            _contractRepo = contractRepo;
            _equityRepo = equityRepo;
        }

        public async Task<(bool Success, string Message, int RowsAdded, int RowsUpdated, int RowsFailed)> ProcessUploadAsync(Stream fileStream, string fileName)
        {
            int successCount = 0;
            int updatedCount = 0;
            int failedCount = 0;
            var errorMessages = new List<string>();
            Stopwatch stopwatch = Stopwatch.StartNew(); // 計時開始
            Debug.WriteLine($"[StockBalanceUploadService] 開始處理檔案: {fileName}");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
                Encoding = Encoding.GetEncoding("Big5")
            };

            List<StockBalanceCsvRecord> csvRecords = new List<StockBalanceCsvRecord>();

            try // 將 CsvHelper 讀取也包在 try-catch 中
            {
                using (var reader = new StreamReader(fileStream, config.Encoding))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<StockBalanceCsvRecordMap>();

                    bool headerSkipped = csv.Read();
                    if (!headerSkipped)
                    {
                        Debug.WriteLine("[StockBalanceUploadService] 錯誤: CSV 檔案為空或只包含標頭。");
                        return (false, "CSV 檔案為空或只包含標頭。", 0, 0, 0);
                    }
                    Debug.WriteLine("[StockBalanceUploadService] 已跳過 CSV 標頭行。");

                    // 從第二行開始正式讀取所有紀錄
                    csvRecords = csv.GetRecords<StockBalanceCsvRecord>().ToList();
                    Debug.WriteLine($"[StockBalanceUploadService] 成功讀取 {csvRecords.Count} 筆 CSV 紀錄。");
                }
            }
            catch (Exception ex)
            {
                // 捕捉 CsvHelper 讀取/解析時的錯誤
                failedCount = 1; // 標記為失敗
                string errorMsg = $"[StockBalanceUploadService] 讀取或解析 CSV 檔案時發生錯誤: {ex.Message}";
                Debug.WriteLine(errorMsg);
                errorMessages.Add(ex.Message);
                // 直接回傳錯誤，因為無法繼續處理
                // 【已修改】調整錯誤訊息格式
                string finalMessageOnError = $"處理失敗：讀取 CSV 檔案時發生錯誤。\n失敗原因: {ex.Message}";
                return (false, finalMessageOnError, 0, 0, failedCount);
            }


            // 開始逐筆處理紀錄
            for (int i = 0; i < csvRecords.Count; i++)
            {
                // +2 是因為 List 從 0 開始，且我們跳過了標頭行
                int currentRowNum = i + 2;
                var record = csvRecords[i];
                Stopwatch rowStopwatch = Stopwatch.StartNew(); // 計時單行處理時間

                try
                {
                    Debug.WriteLine($"[StockBalanceUploadService] 開始處理第 {currentRowNum} 行: PCODE={record.Pcode}, ISIN={record.Isin}");

                    if (string.IsNullOrEmpty(record.Pcode) || string.IsNullOrEmpty(record.AcDate) || string.IsNullOrEmpty(record.Isin))
                    {
                        failedCount++;
                        string msg = $"資料行 {currentRowNum}: PCODE, AC_DATE 或 ISIN 欄位為空。";
                        errorMessages.Add(msg);
                        Debug.WriteLine($"[StockBalanceUploadService] {msg}");
                        continue;
                    }

                    string transformedPcode = TransformPcode(record.Pcode);
                    Debug.WriteLine($"[StockBalanceUploadService] 行 {currentRowNum}: PCODE 轉換為 {transformedPcode}");

                    var contract = await _contractRepo.FindByPcodeAsync(transformedPcode);
                    if (contract == null)
                    {
                        failedCount++;
                        string msg = $"資料行 {currentRowNum}: PCODE '{record.Pcode}' (轉換後為 '{transformedPcode}') 找不到對應的 Contract。";
                        errorMessages.Add(msg);
                        Debug.WriteLine($"[StockBalanceUploadService] {msg}");
                        continue;
                    }
                    Debug.WriteLine($"[StockBalanceUploadService] 行 {currentRowNum}: 找到 Contract ID={contract.Id}, Seq={contract.ContractSeq}");


                    var equity = await _equityRepo.FindByIsinAsync(record.Isin);
                    if (equity == null)
                    {
                        failedCount++;
                        string msg = $"資料行 {currentRowNum}: ISIN '{record.Isin}' 找不到對應的 Equity。";
                        errorMessages.Add(msg);
                        Debug.WriteLine($"[StockBalanceUploadService] {msg}");
                        continue;
                    }
                    Debug.WriteLine($"[StockBalanceUploadService] 行 {currentRowNum}: 找到 Equity StkCd={equity.StkCd}");


                    if (!DateTime.TryParseExact(record.AcDate, "yyyy/M/d", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
                    {
                        failedCount++;
                        string msg = $"資料行 {currentRowNum}: 日期格式錯誤 '{record.AcDate}' (應為 yyyy/M/d)。";
                        errorMessages.Add(msg);
                        Debug.WriteLine($"[StockBalanceUploadService] {msg}");
                        continue;
                    }
                    var acDateOnly = DateOnly.FromDateTime(parsedDateTime);
                    Debug.WriteLine($"[StockBalanceUploadService] 行 {currentRowNum}: 日期轉換成功 {acDateOnly}");


                    var existingBalance = await _stockBalanceRepo.FindByKeyAsync(contract.Id, contract.ContractSeq, acDateOnly, equity.StkCd);

                    if (existingBalance == null)
                    {
                        Debug.WriteLine($"[StockBalanceUploadService] 行 {currentRowNum}: 資料不存在，準備新增。");
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
                            Debug.WriteLine($"[StockBalanceUploadService] 行 {currentRowNum}: 新增操作已標記。");
                        }
                        catch (Exception dbEx)
                        {
                            failedCount++;
                            string msg = $"資料行 {currentRowNum}: 新增 PCODE '{record.Pcode}', ISIN '{record.Isin}' 時資料庫錯誤: {dbEx.InnerException?.Message ?? dbEx.Message}";
                            errorMessages.Add(msg);
                            Debug.WriteLine($"[StockBalanceUploadService] ERROR: {msg}");
                            // 可選擇記錄完整 dbEx
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"[StockBalanceUploadService] 行 {currentRowNum}: 資料已存在，準備更新 Shares。");
                        existingBalance.Shares = record.Shares;
                        //【修正建議】移除明確的 Update 呼叫，讓 EF Core 自動追蹤變更
                        //_stockBalanceRepo.Update(existingBalance); // EF Core tracks the update automatically if entity is tracked
                        updatedCount++;
                        Debug.WriteLine($"[StockBalanceUploadService] 行 {currentRowNum}: 更新 Shares 操作已標記。");
                    }
                }
                catch (Exception ex) // 捕捉處理單行時的未預期錯誤
                {
                    failedCount++;
                    string msg = $"處理 CSV 資料行 {currentRowNum} (PCODE: {record?.Pcode}, ISIN: {record?.Isin}) 時發生未預期錯誤: {ex.Message}";
                    errorMessages.Add(msg);
                    Debug.WriteLine($"[StockBalanceUploadService] ERROR: {msg}");
                    // 可選擇記錄完整 ex
                }
                finally
                {
                    rowStopwatch.Stop();
                    Debug.WriteLine($"[StockBalanceUploadService] 處理第 {currentRowNum} 行完成，耗時: {rowStopwatch.ElapsedMilliseconds} ms");
                }
            } // end foreach

            Debug.WriteLine("[StockBalanceUploadService] 所有 CSV 行處理完畢，準備儲存資料庫變更...");
            try
            {
                await _stockBalanceRepo.SaveChangesAsync();
                Debug.WriteLine("[StockBalanceUploadService] 資料庫變更儲存成功。");
            }
            catch (Exception dbEx)
            {
                failedCount += (successCount + updatedCount); // 將所有嘗試的操作標記為失敗
                successCount = 0;
                updatedCount = 0;
                string msg = $"儲存資料庫變更時發生錯誤 (可能是整體交易失敗): {dbEx.InnerException?.Message ?? dbEx.Message}";
                errorMessages.Add(msg);
                Debug.WriteLine($"[StockBalanceUploadService] ERROR: {msg}");
                // 可選擇記錄完整 dbEx
            }

            stopwatch.Stop(); // 計時結束
            Debug.WriteLine($"[StockBalanceUploadService] 全部處理完成，總耗時: {stopwatch.ElapsedMilliseconds} ms");

            // --- 👇👇👇 【已修改】依照您的要求調整最終訊息格式 👇👇👇 ---
            string finalMessage;
            if (failedCount == 0)
            {
                // 完全成功
                finalMessage = $"處理完成。\n結果：所有資料處理成功 (新增 {successCount} 筆，更新 {updatedCount} 筆)。";
            }
            else
            {
                // 有部分失敗
                finalMessage = $"處理完成：\n結果：有 {failedCount} 項更新失敗 (新增 {successCount} 筆，更新 {updatedCount} 筆)。\n失敗原因 (最多顯示 10 筆): {string.Join("; ", errorMessages.Take(10))}";
            }

            Debug.WriteLine($"[StockBalanceUploadService] 回傳結果: Success={failedCount == 0}, Message={finalMessage}");
            // Success 仍然表示是否 "完全" 成功，但 finalMessage 總是包含詳細結果
            return (failedCount == 0, finalMessage, successCount, updatedCount, failedCount);
            // --- 👆👆👆 【已修改】依照您的要求調整最終訊息格式 👆👆👆 ---
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
}

