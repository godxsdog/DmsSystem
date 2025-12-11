using System.Globalization;
using System.Data;
using System.Data.Common;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using DmsSystem.Infrastructure.Persistence.SqlQueries;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace DmsSystem.Application.Services;

public class DividendService : IDividendService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DividendService> _logger;

    public DividendService(IDbConnectionFactory connectionFactory, ILogger<DividendService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<DividendImportResult> ImportAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return new DividendImportResult(false, 0, 0, 1, new() { "檔案為空" });
        }

        var inserted = 0;
        var updated = 0;
        var failed = 0;
        var errors = new List<string>();

        await using var connection = _connectionFactory.GetConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding("Big5"));
        
        // 修正：不跳過行，因為 CSV 第一行就是 Header
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Encoding = Encoding.GetEncoding("Big5")
        };
        
        using var csv = new CsvReader(reader, config);
        
        var map = new DividendCsvMap();
        csv.Context.RegisterClassMap(map);

        var records = csv.GetRecords<DividendCsvRow>().ToList();

        foreach (var record in records)
        {
            try
            {
                var dividendDate = DateOnly.FromDateTime(record.DividendDate);
                var dividendYear = dividendDate.Year;
                var now = DateTime.UtcNow;

                var action = await connection.ExecuteScalarAsync<string>(
                    DividendSqlQueries.UpsertFundDiv,
                    new
                    {
                        record.FundNo,
                        DividendYear = dividendYear,
                        DividendDate = dividendDate.ToDateTime(TimeOnly.MinValue),
                        record.DividendType,
                        record.Nav,
                        record.PreDiv1,
                        record.PreDiv2,
                        record.PreDiv3,
                        record.PreDiv4,
                        record.PreDiv5,
                        record.Div1,
                        record.Div2,
                        record.Div3,
                        record.Div4,
                        record.Div5,
                        PreDiv1B = record.PreDiv1B ?? 0,
                        Div1B = record.Div1B ?? 0,
                        record.Fee,
                        record.DivTot,
                        Now = now
                    });

                if (action?.Equals("UPDATE", StringComparison.OrdinalIgnoreCase) == true)
                {
                    updated++;
                }
                else
                {
                    inserted++;
                }
            }
            catch (Exception ex)
            {
                failed++;
                errors.Add($"基金 {record.FundNo} 日期 {record.DividendDate:yyyy/MM/dd}: {ex.Message}");
                _logger.LogError(ex, "匯入基金 {FundNo} 失敗", record.FundNo);
            }
        }

        var success = failed == 0;
        return new DividendImportResult(success, inserted, updated, failed, errors);
    }

    public async Task<DividendConfirmResult> ConfirmAsync(string fundNo, DateOnly dividendDate, string dividendType)
    {
        // ... (保持原樣，為了確保檔案完整性，這裡不省略)
        await using var connection = _connectionFactory.GetConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var transaction = connection.BeginTransaction();
        try
        {
            var nav = await connection.ExecuteScalarAsync<decimal?>(
                DividendSqlQueries.GetNav,
                new { FundNo = fundNo, Date = dividendDate.ToDateTime(TimeOnly.MinValue) },
                transaction);

            var unit = await connection.ExecuteScalarAsync<decimal?>(
                DividendSqlQueries.GetUnit,
                new { FundNo = fundNo, Date = dividendDate.ToDateTime(TimeOnly.MinValue) },
                transaction);

            if (!nav.HasValue || nav.Value == 0)
            {
                transaction.Rollback();
                return new DividendConfirmResult(false, "此配息基準淨值尚未公告，不可進行確認作業！", null, null, null, null, null, null);
            }

            if (!unit.HasValue || unit.Value == 0)
            {
                transaction.Rollback();
                return new DividendConfirmResult(false, "基準日單位數為零或未找到", null, null, null, null, null, null);
            }

            var fundDiv = await connection.QuerySingleOrDefaultAsync<dynamic>(
                DividendSqlQueries.GetFundDiv,
                new { FundNo = fundNo, Date = dividendDate.ToDateTime(TimeOnly.MinValue), Type = dividendType },
                transaction);

            if (fundDiv == null)
            {
                transaction.Rollback();
                return new DividendConfirmResult(false, "找不到對應的 FUND_DIV 記錄", null, null, null, null, null, null);
            }

            var divSet = await connection.QuerySingleOrDefaultAsync<dynamic>(
                DividendSqlQueries.GetFundDivSet,
                new { FundNo = fundNo, Type = dividendType },
                transaction);

            if (divSet == null)
            {
                transaction.Rollback();
                return new DividendConfirmResult(false, "找不到配息參數設定", null, null, null, null, null, null);
            }

            var divObj = await connection.QuerySingleOrDefaultAsync<dynamic>(
                DividendSqlQueries.GetFundDivObj,
                new { FundNo = fundNo, Type = dividendType, Date = dividendDate.ToDateTime(TimeOnly.MinValue) },
                transaction);

            var previousDiv = await connection.QuerySingleOrDefaultAsync<dynamic>(
                DividendSqlQueries.GetPreviousFundDiv,
                new { FundNo = fundNo, Type = dividendType, Date = dividendDate.ToDateTime(TimeOnly.MinValue) },
                transaction);

            decimal pre1 = fundDiv.PRE_DIV1 ?? 0m;
            decimal pre2 = fundDiv.PRE_DIV2 ?? 0m;
            decimal pre3 = fundDiv.PRE_DIV3 ?? 0m;
            decimal pre4 = fundDiv.PRE_DIV4 ?? 0m;
            decimal pre5 = fundDiv.PRE_DIV5 ?? 0m;
            decimal div1 = fundDiv.DIV1 ?? 0m;
            decimal div2 = fundDiv.DIV2 ?? 0m;
            decimal div3 = fundDiv.DIV3 ?? 0m;
            decimal div4 = fundDiv.DIV4 ?? 0m;
            decimal div5 = fundDiv.DIV5 ?? 0m;
            decimal fee = fundDiv.FEE ?? 0m;

            decimal divTot = pre1 + pre2 + pre3 + pre4 + pre5 + div1 + div2 + div3 + div4 + div5 - fee;
            if (divTot < 0) divTot = 0m;

            decimal divRate = unit.Value > 0 ? Math.Round(divTot / unit.Value, 6) : 0m;

            decimal navValue = nav!.Value;
            decimal rate = dividendType switch
            {
                "M" => divRate * 12 / navValue,
                "Q" => divRate * 4 / navValue,
                "S" => divRate * 2 / navValue,
                "Y" => divRate / navValue,
                _ => throw new ArgumentException($"不支援的配息頻率: {dividendType}")
            };

            decimal divPre = 0m;
            if (previousDiv != null && previousDiv.NAV != null)
            {
                decimal prevNav = (decimal)previousDiv!.NAV;
                if (prevNav > 0)
                {
                    decimal prevDivRateM = (decimal?)(previousDiv!.DIV_RATE_M) ?? 0m;
                    divPre = dividendType switch
                    {
                        "M" => Math.Round(prevDivRateM * 12 / prevNav, 4),
                        "Q" => Math.Round(prevDivRateM * 4 / prevNav, 4),
                        "S" => Math.Round(prevDivRateM * 2 / prevNav, 4),
                        "Y" => Math.Round(prevDivRateM / prevNav, 4),
                        _ => 0m
                    };
                }
            }

            decimal divObjRate = divObj?.DIV_OBJ ?? rate;
            decimal divObjAmt = divObj?.DIV_OBJ_AMT ?? 0m;
            decimal divRateObj = 0m;

            if (divObjAmt > 0)
            {
                divRateObj = divObjAmt;
                divObjRate = dividendType switch
                {
                    "M" => Math.Truncate(divRateObj / nav.Value * 12 * 1000000) / 1000000,
                    "Q" => Math.Truncate(divRateObj / nav.Value * 4 * 1000000) / 1000000,
                    "S" => Math.Truncate(divRateObj / nav.Value * 2 * 1000000) / 1000000,
                    "Y" => Math.Truncate(divRateObj / nav.Value * 1000000) / 1000000,
                    _ => divObjRate
                };
            }
            else
            {
                decimal temp = dividendType switch
                {
                    "M" => divObjRate / 12 * nav.Value,
                    "Q" => divObjRate / 4 * nav.Value,
                    "S" => divObjRate / 2 * nav.Value,
                    "Y" => divObjRate * nav.Value,
                    _ => 0m
                };
                divRateObj = Math.Truncate(temp * 1000000) / 1000000;
                if (Math.Truncate(temp * 1000000) > temp * 1000000)
                {
                    divRateObj -= 0.000001m;
                }
            }

            string capitalType = divSet.CAPITAL_TYPE ?? "N";
            decimal divRatePerUnit = divRate;
            decimal feeRate = unit.Value > 0 ? Math.Round(fee / unit.Value, 6) : 0m;
            decimal capitalRate = 0m;

            if (divRateObj > divRatePerUnit)
            {
                if (capitalType == "N")
                {
                    divRateObj = divRatePerUnit;
                }
                else
                {
                    capitalRate = divRateObj - divRatePerUnit;
                }
            }

            decimal availableAfterFee = Math.Max(0m, divRatePerUnit - feeRate);
            if (availableAfterFee < divRateObj)
            {
                if (capitalType == "Y")
                {
                    capitalRate += (divRateObj - availableAfterFee);
                }
                else
                {
                    capitalRate = 0m;
                    divRateObj = availableAfterFee;
                }
            }

            int[] orders = new int[]
            {
                (int)(divSet.ITEM01_SEQ ?? 0),
                (int)(divSet.ITEM02_SEQ ?? 0),
                (int)(divSet.ITEM03_SEQ ?? 0),
                (int)(divSet.ITEM04_SEQ ?? 0),
                (int)(divSet.ITEM05_SEQ ?? 0),
                (int)(divSet.ITEM06_SEQ ?? 0),
                (int)(divSet.ITEM07_SEQ ?? 0),
                (int)(divSet.ITEM08_SEQ ?? 0),
                (int)(divSet.ITEM09_SEQ ?? 0),
                (int)(divSet.ITEM10_SEQ ?? 0)
            };

            decimal[] amounts = new decimal[10];
            decimal[] sourceAmounts = new decimal[] { pre1, pre2, pre3, pre4, pre5, div1, div2, div3, div4, div5 };

            for (int seq = 1; seq <= 10; seq++)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (orders[i] == seq && sourceAmounts[i] > 0)
                    {
                        amounts[seq - 1] = sourceAmounts[i];
                        break;
                    }
                }
            }

            decimal[] rateCal = new decimal[10];
            decimal divRateTot = capitalRate;

            for (int i = 0; i < 10; i++)
            {
                if (amounts[i] > 0 && unit.Value > 0)
                {
                    decimal tempRate = Math.Truncate((amounts[i] / unit.Value - 0.000003m) * 1000000) / 1000000;
                    rateCal[i] = tempRate > 0 ? tempRate : 0m;
                }
                else
                {
                    rateCal[i] = 0m;
                }

                if (divRateTot + rateCal[i] <= divRateObj)
                {
                    divRateTot += rateCal[i];
                }
                else
                {
                    rateCal[i] = divRateObj - divRateTot;
                    divRateTot = divRateObj;
                    for (int j = i + 1; j < 10; j++)
                    {
                        rateCal[j] = 0m;
                    }
                    break;
                }
            }

            if (divRateTot < divRateObj)
            {
                if (capitalType == "Y")
                {
                    capitalRate += (divRateObj - divRateTot);
                }
                else
                {
                    capitalRate = 0m;
                    divRateObj = divRateTot;
                }
            }

            var now = DateTime.UtcNow;
            await connection.ExecuteAsync(
                DividendSqlQueries.UpdateFundDivCalculation,
                new
                {
                    Nav = nav.Value,
                    Unit = unit.Value,
                    DivRateObj = divRateObj,
                    CapitalRate = capitalRate,
                    FeeRate = feeRate,
                    DivObjRate = divObjRate,
                    DivPre = divPre,
                    FundNo = fundNo,
                    Date = dividendDate.ToDateTime(TimeOnly.MinValue),
                    Type = dividendType,
                    Now = now
                },
                transaction);

            var rateUpdateParams = new DynamicParameters();
            rateUpdateParams.Add("Rate1", rateCal[0]);
            rateUpdateParams.Add("Rate2", rateCal[1]);
            rateUpdateParams.Add("Rate3", rateCal[2]);
            rateUpdateParams.Add("Rate4", rateCal[3]);
            rateUpdateParams.Add("Rate5", rateCal[4]);
            rateUpdateParams.Add("Rate6", rateCal[5]);
            rateUpdateParams.Add("Rate7", rateCal[6]);
            rateUpdateParams.Add("Rate8", rateCal[7]);
            rateUpdateParams.Add("Rate9", rateCal[8]);
            rateUpdateParams.Add("Rate10", rateCal[9]);
            rateUpdateParams.Add("FundNo", fundNo);
            rateUpdateParams.Add("Date", dividendDate.ToDateTime(TimeOnly.MinValue));
            rateUpdateParams.Add("Type", dividendType);

            var rateUpdateSql = BuildRateUpdateSql(orders);
            await connection.ExecuteAsync(rateUpdateSql, rateUpdateParams, transaction);

            transaction.Commit();

            return new DividendConfirmResult(true, "計算完成並已更新 FUND_DIV", nav, unit, divTot, divRate, divRateObj, capitalRate);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError(ex, "配息計算失敗: FundNo={FundNo}, Date={Date}, Type={Type}", fundNo, dividendDate, dividendType);
            return new DividendConfirmResult(false, $"計算失敗: {ex.Message}", null, null, null, null, null, null);
        }
    }

    public async Task<BatchConfirmResult> BatchConfirmAsync(DateOnly? dividendDate = null)
    {
        await using var connection = _connectionFactory.GetConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var pendingItems = await connection.QueryAsync<dynamic>(
            DividendSqlQueries.GetPendingFundDivs,
            new { Date = dividendDate?.ToDateTime(TimeOnly.MinValue) });

        var totalCount = 0;
        var successCount = 0;
        var failureCount = 0;
        var errors = new List<string>();

        foreach (var item in pendingItems)
        {
            totalCount++;
            string fundNo = item.FundNo;
            DateTime date = item.DividendDate;
            string type = item.DividendType;
            var dateOnly = DateOnly.FromDateTime(date);

            try
            {
                var result = await ConfirmAsync(fundNo, dateOnly, type);
                if (result.Success)
                {
                    successCount++;
                }
                else
                {
                    failureCount++;
                    errors.Add($"基金 {fundNo} ({dateOnly:yyyy-MM-dd}): {result.Message}");
                }
            }
            catch (Exception ex)
            {
                failureCount++;
                errors.Add($"基金 {fundNo} ({dateOnly:yyyy-MM-dd}): {ex.Message}");
                _logger.LogError(ex, "批量確認失敗: FundNo={FundNo}", fundNo);
            }
        }

        return new BatchConfirmResult(totalCount, successCount, failureCount, errors);
    }

    public async Task<DividendImportResult> ImportCompositionAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return new DividendImportResult(false, 0, 0, 1, new() { "檔案為空" });
        }

        var updated = 0;
        var failed = 0;
        var errors = new List<string>();

        await using var connection = _connectionFactory.GetConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using var reader = new StreamReader(file.OpenReadStream(), Encoding.GetEncoding("Big5"));
        
        // 修正：不跳過行，因為第一行就是 Header
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true, // 第一行是 Header
            Encoding = Encoding.GetEncoding("Big5"),
            ShouldSkipRecord = args => 
            {
                var row = args.Row;
                if (row.Parser.Count == 0) return true;
                var fundNo = row.GetField("fund_no");
                return string.IsNullOrWhiteSpace(fundNo) || fundNo.Equals("fund_no", StringComparison.OrdinalIgnoreCase);
            }
        };
        
        using var csv = new CsvReader(reader, config);
        
        var map = new DividendCompositionCsvMap();
        csv.Context.RegisterClassMap(map);

        List<DividendCompositionCsvRow> records;
        try
        {
            records = csv.GetRecords<DividendCompositionCsvRow>().ToList();
        }
        catch (CsvHelperException ex)
        {
            return new DividendImportResult(false, 0, 0, 1, new() { $"CSV 解析失敗: {ex.Message}" });
        }

        foreach (var record in records)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(record.FundNo) || string.IsNullOrWhiteSpace(record.DividendDateStr))
                {
                    continue; 
                }

                if (!DateTime.TryParse(record.DividendDateStr, out var dividendDate))
                {
                    failed++;
                    errors.Add($"基金 {record.FundNo}: 日期格式錯誤 '{record.DividendDateStr}'");
                    continue;
                }

                decimal interestRate = ParseRate(record.InterestRateStr);
                decimal capitalRate = ParseRate(record.CapitalRateStr);

                var now = DateTime.UtcNow;
                var rowsAffected = await connection.ExecuteAsync(
                    DividendSqlQueries.UpdateFundDivComposition,
                    new
                    {
                        record.FundNo,
                        Date = dividendDate,
                        Type = record.DividendType,
                        InterestRate = interestRate,
                        CapitalRate = capitalRate,
                        Now = now
                    });

                if (rowsAffected > 0)
                {
                    updated++;
                }
                else
                {
                    failed++;
                    errors.Add($"基金 {record.FundNo} ({dividendDate:yyyy/MM/dd}) 無法更新：找不到對應記錄");
                }
            }
            catch (Exception ex)
            {
                failed++;
                errors.Add($"基金 {record.FundNo}: {ex.Message}");
                _logger.LogError(ex, "匯入配息組成失敗: FundNo={FundNo}", record.FundNo);
            }
        }

        var success = failed == 0;
        return new DividendImportResult(success, 0, updated, failed, errors);
    }

    private decimal ParseRate(string? rateStr)
    {
        if (string.IsNullOrWhiteSpace(rateStr)) return 0m;
        rateStr = rateStr.Trim();
        
        if (rateStr.Contains("%"))
        {
            rateStr = rateStr.Replace("%", "");
            if (decimal.TryParse(rateStr, out var val))
            {
                return val / 100m;
            }
        }
        else
        {
            if (decimal.TryParse(rateStr, out var val))
            {
                return val; // 直接是小數 (e.g. 0.815)
            }
        }
        
        return 0m;
    }

    public async Task<bool> UploadToEcAsync(string fundNo, DateOnly dividendDate, string dividendType)
    {
        await using var connection = _connectionFactory.GetConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var now = DateTime.UtcNow;
        var rowsAffected = await connection.ExecuteAsync(
            DividendSqlQueries.UploadToWps,
            new
            {
                FundNo = fundNo,
                Date = dividendDate.ToDateTime(TimeOnly.MinValue),
                Type = dividendType,
                Now = now
            });

        return rowsAffected > 0;
    }

    private sealed class DividendCsvRow
    {
        public string FundNo { get; set; } = default!;
        public DateTime DividendDate { get; set; }
        public string DividendType { get; set; } = default!;
        public decimal PreDiv1 { get; set; }
        public decimal PreDiv2 { get; set; }
        public decimal PreDiv3 { get; set; }
        public decimal PreDiv4 { get; set; }
        public decimal PreDiv5 { get; set; }
        public decimal Div1 { get; set; }
        public decimal Div2 { get; set; }
        public decimal Div3 { get; set; }
        public decimal Div4 { get; set; }
        public decimal Div5 { get; set; }
        public decimal? PreDiv1B { get; set; }
        public decimal? Div1B { get; set; }
        public decimal Fee { get; set; }
        public decimal DivTot { get; set; }
        public decimal Nav { get; set; }
    }

    private static string BuildRateUpdateSql(int[] orders)
    {
        // (省略，內容未變更)
        var sql = @"
UPDATE MDS.FUND_DIV
SET PRE_DIV1_RATE = CASE WHEN @Ord1 = 1 THEN @Rate1 WHEN @Ord1 = 2 THEN @Rate2 WHEN @Ord1 = 3 THEN @Rate3 WHEN @Ord1 = 4 THEN @Rate4 WHEN @Ord1 = 5 THEN @Rate5 WHEN @Ord1 = 6 THEN @Rate6 WHEN @Ord1 = 7 THEN @Rate7 WHEN @Ord1 = 8 THEN @Rate8 WHEN @Ord1 = 9 THEN @Rate9 WHEN @Ord1 = 10 THEN @Rate10 ELSE 0 END,
    PRE_DIV1_RATE_M = CASE WHEN @Ord1 = 1 THEN @Rate1 WHEN @Ord1 = 2 THEN @Rate2 WHEN @Ord1 = 3 THEN @Rate3 WHEN @Ord1 = 4 THEN @Rate4 WHEN @Ord1 = 5 THEN @Rate5 WHEN @Ord1 = 6 THEN @Rate6 WHEN @Ord1 = 7 THEN @Rate7 WHEN @Ord1 = 8 THEN @Rate8 WHEN @Ord1 = 9 THEN @Rate9 WHEN @Ord1 = 10 THEN @Rate10 ELSE 0 END,
    PRE_DIV2_RATE = CASE WHEN @Ord2 = 1 THEN @Rate1 WHEN @Ord2 = 2 THEN @Rate2 WHEN @Ord2 = 3 THEN @Rate3 WHEN @Ord2 = 4 THEN @Rate4 WHEN @Ord2 = 5 THEN @Rate5 WHEN @Ord2 = 6 THEN @Rate6 WHEN @Ord2 = 7 THEN @Rate7 WHEN @Ord2 = 8 THEN @Rate8 WHEN @Ord2 = 9 THEN @Rate9 WHEN @Ord2 = 10 THEN @Rate10 ELSE 0 END,
    PRE_DIV2_RATE_M = CASE WHEN @Ord2 = 1 THEN @Rate1 WHEN @Ord2 = 2 THEN @Rate2 WHEN @Ord2 = 3 THEN @Rate3 WHEN @Ord2 = 4 THEN @Rate4 WHEN @Ord2 = 5 THEN @Rate5 WHEN @Ord2 = 6 THEN @Rate6 WHEN @Ord2 = 7 THEN @Rate7 WHEN @Ord2 = 8 THEN @Rate8 WHEN @Ord2 = 9 THEN @Rate9 WHEN @Ord2 = 10 THEN @Rate10 ELSE 0 END,
    PRE_DIV3_RATE = CASE WHEN @Ord3 = 1 THEN @Rate1 WHEN @Ord3 = 2 THEN @Rate2 WHEN @Ord3 = 3 THEN @Rate3 WHEN @Ord3 = 4 THEN @Rate4 WHEN @Ord3 = 5 THEN @Rate5 WHEN @Ord3 = 6 THEN @Rate6 WHEN @Ord3 = 7 THEN @Rate7 WHEN @Ord3 = 8 THEN @Rate8 WHEN @Ord3 = 9 THEN @Rate9 WHEN @Ord3 = 10 THEN @Rate10 ELSE 0 END,
    PRE_DIV3_RATE_M = CASE WHEN @Ord3 = 1 THEN @Rate1 WHEN @Ord3 = 2 THEN @Rate2 WHEN @Ord3 = 3 THEN @Rate3 WHEN @Ord3 = 4 THEN @Rate4 WHEN @Ord3 = 5 THEN @Rate5 WHEN @Ord3 = 6 THEN @Rate6 WHEN @Ord3 = 7 THEN @Rate7 WHEN @Ord3 = 8 THEN @Rate8 WHEN @Ord3 = 9 THEN @Rate9 WHEN @Ord3 = 10 THEN @Rate10 ELSE 0 END,
    PRE_DIV4_RATE = CASE WHEN @Ord4 = 1 THEN @Rate1 WHEN @Ord4 = 2 THEN @Rate2 WHEN @Ord4 = 3 THEN @Rate3 WHEN @Ord4 = 4 THEN @Rate4 WHEN @Ord4 = 5 THEN @Rate5 WHEN @Ord4 = 6 THEN @Rate6 WHEN @Ord4 = 7 THEN @Rate7 WHEN @Ord4 = 8 THEN @Rate8 WHEN @Ord4 = 9 THEN @Rate9 WHEN @Ord4 = 10 THEN @Rate10 ELSE 0 END,
    PRE_DIV4_RATE_M = CASE WHEN @Ord4 = 1 THEN @Rate1 WHEN @Ord4 = 2 THEN @Rate2 WHEN @Ord4 = 3 THEN @Rate3 WHEN @Ord4 = 4 THEN @Rate4 WHEN @Ord4 = 5 THEN @Rate5 WHEN @Ord4 = 6 THEN @Rate6 WHEN @Ord4 = 7 THEN @Rate7 WHEN @Ord4 = 8 THEN @Rate8 WHEN @Ord4 = 9 THEN @Rate9 WHEN @Ord4 = 10 THEN @Rate10 ELSE 0 END,
    PRE_DIV5_RATE = CASE WHEN @Ord5 = 1 THEN @Rate1 WHEN @Ord5 = 2 THEN @Rate2 WHEN @Ord5 = 3 THEN @Rate3 WHEN @Ord5 = 4 THEN @Rate4 WHEN @Ord5 = 5 THEN @Rate5 WHEN @Ord5 = 6 THEN @Rate6 WHEN @Ord5 = 7 THEN @Rate7 WHEN @Ord5 = 8 THEN @Rate8 WHEN @Ord5 = 9 THEN @Rate9 WHEN @Ord5 = 10 THEN @Rate10 ELSE 0 END,
    PRE_DIV5_RATE_M = CASE WHEN @Ord5 = 1 THEN @Rate1 WHEN @Ord5 = 2 THEN @Rate2 WHEN @Ord5 = 3 THEN @Rate3 WHEN @Ord5 = 4 THEN @Rate4 WHEN @Ord5 = 5 THEN @Rate5 WHEN @Ord5 = 6 THEN @Rate6 WHEN @Ord5 = 7 THEN @Rate7 WHEN @Ord5 = 8 THEN @Rate8 WHEN @Ord5 = 9 THEN @Ord5 = 10 THEN @Rate10 ELSE 0 END,
    DIV1_RATE = CASE WHEN @Ord6 = 1 THEN @Rate1 WHEN @Ord6 = 2 THEN @Rate2 WHEN @Ord6 = 3 THEN @Rate3 WHEN @Ord6 = 4 THEN @Rate4 WHEN @Ord6 = 5 THEN @Rate5 WHEN @Ord6 = 6 THEN @Rate6 WHEN @Ord6 = 7 THEN @Rate7 WHEN @Ord6 = 8 THEN @Rate8 WHEN @Ord6 = 9 THEN @Ord6 = 10 THEN @Rate10 ELSE 0 END,
    DIV1_RATE_M = CASE WHEN @Ord6 = 1 THEN @Rate1 WHEN @Ord6 = 2 THEN @Rate2 WHEN @Ord6 = 3 THEN @Rate3 WHEN @Ord6 = 4 THEN @Rate4 WHEN @Ord6 = 5 THEN @Rate5 WHEN @Ord6 = 6 THEN @Rate6 WHEN @Ord6 = 7 THEN @Rate7 WHEN @Ord6 = 8 THEN @Rate8 WHEN @Ord6 = 9 THEN @Ord6 = 10 THEN @Rate10 ELSE 0 END,
    DIV2_RATE = CASE WHEN @Ord7 = 1 THEN @Rate1 WHEN @Ord7 = 2 THEN @Rate2 WHEN @Ord7 = 3 THEN @Rate3 WHEN @Ord7 = 4 THEN @Rate4 WHEN @Ord7 = 5 THEN @Rate5 WHEN @Ord7 = 6 THEN @Rate6 WHEN @Ord7 = 7 THEN @Rate7 WHEN @Ord7 = 8 THEN @Rate8 WHEN @Ord7 = 9 THEN @Ord7 = 10 THEN @Rate10 ELSE 0 END,
    DIV2_RATE_M = CASE WHEN @Ord7 = 1 THEN @Rate1 WHEN @Ord7 = 2 THEN @Rate2 WHEN @Ord7 = 3 THEN @Rate3 WHEN @Ord7 = 4 THEN @Rate4 WHEN @Ord7 = 5 THEN @Rate5 WHEN @Ord7 = 6 THEN @Rate6 WHEN @Ord7 = 7 THEN @Rate7 WHEN @Ord7 = 8 THEN @Rate8 WHEN @Ord7 = 9 THEN @Ord7 = 10 THEN @Rate10 ELSE 0 END,
    DIV3_RATE = CASE WHEN @Ord8 = 1 THEN @Rate1 WHEN @Ord8 = 2 THEN @Rate2 WHEN @Ord8 = 3 THEN @Rate3 WHEN @Ord8 = 4 THEN @Rate4 WHEN @Ord8 = 5 THEN @Rate5 WHEN @Ord8 = 6 THEN @Rate6 WHEN @Ord8 = 7 THEN @Rate7 WHEN @Ord8 = 8 THEN @Rate8 WHEN @Ord8 = 9 THEN @Ord8 = 10 THEN @Rate10 ELSE 0 END,
    DIV3_RATE_M = CASE WHEN @Ord8 = 1 THEN @Rate1 WHEN @Ord8 = 2 THEN @Rate2 WHEN @Ord8 = 3 THEN @Rate3 WHEN @Ord8 = 4 THEN @Rate4 WHEN @Ord8 = 5 THEN @Rate5 WHEN @Ord8 = 6 THEN @Rate6 WHEN @Ord8 = 7 THEN @Rate7 WHEN @Ord8 = 8 THEN @Rate8 WHEN @Ord8 = 9 THEN @Ord8 = 10 THEN @Rate10 ELSE 0 END,
    DIV4_RATE = CASE WHEN @Ord9 = 1 THEN @Rate1 WHEN @Ord9 = 2 THEN @Rate2 WHEN @Ord9 = 3 THEN @Rate3 WHEN @Ord9 = 4 THEN @Rate4 WHEN @Ord9 = 5 THEN @Rate5 WHEN @Ord9 = 6 THEN @Rate6 WHEN @Ord9 = 7 THEN @Rate7 WHEN @Ord9 = 8 THEN @Rate9 = 9 THEN @Ord9 = 10 THEN @Rate10 ELSE 0 END,
    DIV4_RATE_M = CASE WHEN @Ord9 = 1 THEN @Rate1 WHEN @Ord9 = 2 THEN @Rate2 WHEN @Ord9 = 3 THEN @Rate3 WHEN @Ord9 = 4 THEN @Rate4 WHEN @Ord9 = 5 THEN @Rate5 WHEN @Ord9 = 6 THEN @Rate6 WHEN @Ord9 = 7 THEN @Rate7 WHEN @Ord9 = 8 THEN @Rate9 = 9 THEN @Ord9 = 10 THEN @Rate10 ELSE 0 END,
    DIV5_RATE = CASE WHEN @Ord10 = 1 THEN @Rate1 WHEN @Ord10 = 2 THEN @Rate2 WHEN @Ord10 = 3 THEN @Rate3 WHEN @Ord10 = 4 THEN @Rate4 WHEN @Ord10 = 5 THEN @Rate5 WHEN @Ord10 = 6 THEN @Rate6 WHEN @Ord10 = 7 THEN @Rate7 WHEN @Ord10 = 8 THEN @Rate8 WHEN @Ord10 = 9 THEN @Ord10 = 10 THEN @Rate10 ELSE 0 END,
    DIV5_RATE_M = CASE WHEN @Ord10 = 1 THEN @Rate1 WHEN @Ord10 = 2 THEN @Rate2 WHEN @Ord10 = 3 THEN @Rate3 WHEN @Ord10 = 4 THEN @Rate4 WHEN @Ord10 = 5 THEN @Rate5 WHEN @Ord10 = 6 THEN @Rate6 WHEN @Ord10 = 7 THEN @Rate7 WHEN @Ord10 = 8 THEN @Rate8 WHEN @Ord10 = 9 THEN @Ord10 = 10 THEN @Rate10 ELSE 0 END
WHERE FUND_NO = @FundNo AND DIVIDEND_DATE = @Date AND DIVIDEND_TYPE = @Type";

        sql = sql.Replace("@Ord1", orders[0].ToString())
                 .Replace("@Ord2", orders[1].ToString())
                 .Replace("@Ord3", orders[2].ToString())
                 .Replace("@Ord4", orders[3].ToString())
                 .Replace("@Ord5", orders[4].ToString())
                 .Replace("@Ord6", orders[5].ToString())
                 .Replace("@Ord7", orders[6].ToString())
                 .Replace("@Ord8", orders[7].ToString())
                 .Replace("@Ord9", orders[8].ToString())
                 .Replace("@Ord10", orders[9].ToString());

        return sql;
    }

    private sealed class DividendCsvMap : ClassMap<DividendCsvRow>
    {
        public DividendCsvMap()
        {
            Map(m => m.FundNo).Name("fund_no");
            Map(m => m.DividendDate).Name("dividend_date");
            Map(m => m.DividendType).Name("dividend_type");
            Map(m => m.PreDiv1).Name("pre_div1");
            Map(m => m.PreDiv2).Name("pre_div2");
            Map(m => m.PreDiv3).Name("pre_div3");
            Map(m => m.PreDiv4).Name("pre_div4");
            Map(m => m.PreDiv5).Name("pre_div5");
            Map(m => m.Div1).Name("div1");
            Map(m => m.Div2).Name("div2");
            Map(m => m.Div3).Name("div3");
            Map(m => m.Div4).Name("div4");
            Map(m => m.Div5).Name("div5");
            Map(m => m.PreDiv1B).Name("pre_div1_b").Optional();
            Map(m => m.Div1B).Name("div1_b").Optional();
            Map(m => m.Fee).Name("fee");
            Map(m => m.DivTot).Name("div_tot");
            Map(m => m.Nav).Name("nav").Optional();
        }
    }

    private sealed class DividendCompositionCsvMap : ClassMap<DividendCompositionCsvRow>
    {
        public DividendCompositionCsvMap()
        {
            Map(m => m.FundMasterNo).Name("fund_master_no").Optional();
            Map(m => m.FundNo).Name("fund_no");
            Map(m => m.DividendDateStr).Name("dividend_date");
            Map(m => m.DividendType).Name("dividend_type");
            Map(m => m.InterestRateStr).Name("interest_rate");
            Map(m => m.CapitalRateStr).Name("capital_rate");
        }
    }
}
