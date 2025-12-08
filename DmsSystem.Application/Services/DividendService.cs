using System.Globalization;
using System.Data;
using System.Data.Common;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace DmsSystem.Application.Services;

/// <summary>
/// 5A1 配息匯入與計算服務實作
/// </summary>
/// <remarks>
/// 使用 Dapper 直接操作資料庫，提供配息資料匯入與計算功能
/// </remarks>
public class DividendService : IDividendService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DividendService> _logger;

    public DividendService(IDbConnectionFactory connectionFactory, ILogger<DividendService> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    /// <summary>
    /// 匯入可分配收益 CSV 檔案並更新 MDS.FUND_DIV 資料表
    /// </summary>
    /// <param name="file">CSV 檔案（Big5 編碼）</param>
    /// <returns>匯入結果，包含新增、更新、失敗筆數及錯誤訊息</returns>
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
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Encoding = Encoding.GetEncoding("Big5")
        });
        csv.Context.RegisterClassMap<DividendCsvMap>();

        var records = csv.GetRecords<DividendCsvRow>().ToList();

        foreach (var record in records)
        {
            try
            {
                var dividendDate = DateOnly.FromDateTime(record.DividendDate);
                var dividendYear = dividendDate.Year;
                var now = DateTime.UtcNow;

                const string upsertSql = @"
MERGE MDS.FUND_DIV AS target
USING (SELECT @FundNo AS FUND_NO, @DividendYear AS DIVIDEND_YEAR, @DividendDate AS DIVIDEND_DATE, @DividendType AS DIVIDEND_TYPE) AS source
    ON (target.FUND_NO = source.FUND_NO AND target.DIVIDEND_YEAR = source.DIVIDEND_YEAR AND target.DIVIDEND_DATE = source.DIVIDEND_DATE AND target.DIVIDEND_TYPE = source.DIVIDEND_TYPE)
WHEN MATCHED THEN
    UPDATE SET
        NAV = @Nav,
        UNIT = ISNULL(UNIT, 0),
        PRE_DIV1 = @PreDiv1,
        PRE_DIV2 = @PreDiv2,
        PRE_DIV3 = @PreDiv3,
        PRE_DIV4 = @PreDiv4,
        PRE_DIV5 = @PreDiv5,
        DIV1 = @Div1,
        DIV2 = @Div2,
        DIV3 = @Div3,
        DIV4 = @Div4,
        DIV5 = @Div5,
        PRE_DIV1_B = @PreDiv1B,
        DIV1_B = @Div1B,
        FEE = @Fee,
        DIV_TOT = @DivTot,
        STEP2_STATUS = ISNULL(STEP2_STATUS, 'C'),
        STEP2_CRE_EMP = ISNULL(STEP2_CRE_EMP, 'SYSTEM'),
        STEP2_CRE_TIME = ISNULL(STEP2_CRE_TIME, @Now)
WHEN NOT MATCHED THEN
    INSERT (FUND_NO, DIVIDEND_YEAR, DIVIDEND_DATE, DIVIDEND_TYPE, NAV, UNIT,
            PRE_DIV1, PRE_DIV2, PRE_DIV3, PRE_DIV4, PRE_DIV5,
            DIV1, DIV2, DIV3, DIV4, DIV5,
            PRE_DIV1_B, DIV1_B, FEE, DIV_TOT,
            STEP2_STATUS, STEP2_CRE_EMP, STEP2_CRE_TIME, STATUS, STATUS_C)
    VALUES (@FundNo, @DividendYear, @DividendDate, @DividendType, @Nav, 0,
            @PreDiv1, @PreDiv2, @PreDiv3, @PreDiv4, @PreDiv5,
            @Div1, @Div2, @Div3, @Div4, @Div5,
            @PreDiv1B, @Div1B, @Fee, @DivTot,
            'C', 'SYSTEM', @Now, 'Y', 'N')
OUTPUT $action;";

                var action = await connection.ExecuteScalarAsync<string>(
                    upsertSql,
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
                        record.PreDiv1B,
                        record.Div1B,
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

    /// <summary>
    /// 執行配息計算與確認，更新配息率與狀態
    /// </summary>
    /// <param name="fundNo">基金代號</param>
    /// <param name="dividendDate">配息基準日</param>
    /// <param name="dividendType">配息頻率：M（月）、Q（季）、S（半年）、Y（年）</param>
    /// <returns>計算結果，包含 NAV、單位數、配息總額、配息率等資訊</returns>
    public async Task<DividendConfirmResult> ConfirmAsync(string fundNo, DateOnly dividendDate, string dividendType)
    {
        await using var connection = _connectionFactory.GetConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        var transaction = connection.BeginTransaction();
        try
        {
            // 1. 取得 NAV 與單位數
            var nav = await connection.ExecuteScalarAsync<decimal?>(
                "SELECT NAV FROM NAV.NAV WHERE FUND_NO = @FundNo AND NAV_DATE = @Date",
                new { FundNo = fundNo, Date = dividendDate.ToDateTime(TimeOnly.MinValue) },
                transaction);

            var unit = await connection.ExecuteScalarAsync<decimal?>(
                "SELECT BAL_SHARE FROM FAS.FUND_LEDGE WHERE FUND_NO = @FundNo AND AC_DATE = @Date",
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

            // 2. 取得 FUND_DIV 資料
            var fundDiv = await connection.QuerySingleOrDefaultAsync<dynamic>(
                @"SELECT PRE_DIV1, PRE_DIV2, PRE_DIV3, PRE_DIV4, PRE_DIV5,
                         DIV1, DIV2, DIV3, DIV4, DIV5, FEE, DIV_TOT
                  FROM MDS.FUND_DIV
                  WHERE FUND_NO = @FundNo AND DIVIDEND_DATE = @Date AND DIVIDEND_TYPE = @Type",
                new { FundNo = fundNo, Date = dividendDate.ToDateTime(TimeOnly.MinValue), Type = dividendType },
                transaction);

            if (fundDiv == null)
            {
                transaction.Rollback();
                return new DividendConfirmResult(false, "找不到對應的 FUND_DIV 記錄", null, null, null, null, null, null);
            }

            // 3. 取得配息參數設定（分攤順序）
            var divSet = await connection.QuerySingleOrDefaultAsync<dynamic>(
                @"SELECT ITEM01_SEQ, ITEM02_SEQ, ITEM03_SEQ, ITEM04_SEQ, ITEM05_SEQ,
                         ITEM06_SEQ, ITEM07_SEQ, ITEM08_SEQ, ITEM09_SEQ, ITEM10_SEQ,
                         CAPITAL_TYPE
                  FROM MDS.FUND_DIV_SET
                  WHERE FUND_NO = @FundNo AND DIV_TYPE = @Type",
                new { FundNo = fundNo, Type = dividendType },
                transaction);

            if (divSet == null)
            {
                transaction.Rollback();
                return new DividendConfirmResult(false, "找不到配息參數設定", null, null, null, null, null, null);
            }

            // 4. 取得目標配息率（依配息基準日向前回溯最近一筆）
            var divObj = await connection.QuerySingleOrDefaultAsync<dynamic>(
                @"SELECT DIV_OBJ, DIV_OBJ_AMT
                  FROM MDS.FUND_DIV_OBJ
                  WHERE FUND_NO = @FundNo AND DIV_TYPE = @Type AND TX_DATE <= @Date
                  ORDER BY TX_DATE DESC",
                new { FundNo = fundNo, Type = dividendType, Date = dividendDate.ToDateTime(TimeOnly.MinValue) },
                transaction);

            // 5. 取得上期配息率
            var previousDiv = await connection.QuerySingleOrDefaultAsync<dynamic>(
                @"SELECT DIV_RATE_M, NAV
                  FROM MDS.FUND_DIV
                  WHERE FUND_NO = @FundNo AND DIVIDEND_TYPE = @Type AND DIVIDEND_DATE < @Date
                  ORDER BY DIVIDEND_DATE DESC",
                new { FundNo = fundNo, Type = dividendType, Date = dividendDate.ToDateTime(TimeOnly.MinValue) },
                transaction);

            // 6. 計算基本數值
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

            // 更新 DIV_TOT 和 DIV_RATE（先計算基本值）
            decimal divTot = pre1 + pre2 + pre3 + pre4 + pre5 + div1 + div2 + div3 + div4 + div5 - fee;
            if (divTot < 0) divTot = 0m;

            decimal divRate = unit.Value > 0 ? Math.Round(divTot / unit.Value, 6) : 0m;

            // 7. 根據配息頻率計算年化配息率
            decimal navValue = nav!.Value;
            decimal rate = dividendType switch
            {
                "M" => divRate * 12 / navValue,
                "Q" => divRate * 4 / navValue,
                "S" => divRate * 2 / navValue,
                "Y" => divRate / navValue,
                _ => throw new ArgumentException($"不支援的配息頻率: {dividendType}")
            };

            // 8. 計算上期配息率
            decimal divPre = 0m;
            if (previousDiv != null && previousDiv.NAV != null)
            {
                decimal prevNav = (decimal)previousDiv.NAV;
                if (prevNav > 0)
                {
                    decimal prevDivRateM = (decimal?)(previousDiv.DIV_RATE_M) ?? 0m;
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

            // 9. 計算目標配息率與每單位配息金額
            decimal divObjRate = divObj?.DIV_OBJ ?? rate;
            decimal divObjAmt = divObj?.DIV_OBJ_AMT ?? 0m;
            decimal divRateObj = 0m; // 每單位配息金額（當期）

            if (divObjAmt > 0)
            {
                divRateObj = divObjAmt;
                // 根據配息頻率反推年化配息率
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
                // 根據目標配息率計算每單位配息金額
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

            // 10. 計算本金補足
            string capitalType = divSet.CAPITAL_TYPE ?? "N";
            decimal divRatePerUnit = divRate; // 每單位可分配收益
            decimal feeRate = unit.Value > 0 ? Math.Round(fee / unit.Value, 6) : 0m;
            decimal capitalRate = 0m;

            if (divRateObj <= divRatePerUnit)
            {
                capitalRate = 0m;
            }
            else
            {
                if (capitalType == "N")
                {
                    // 不可分配本金，調整每單位配息金額
                    capitalRate = 0m;
                    divRateObj = divRatePerUnit;
                }
                else
                {
                    // 可分配本金，差額放在本金比率
                    capitalRate = divRateObj - divRatePerUnit;
                }
            }

            // 11. 計算收益分攤（根據分攤順序）
            int[] orders = new int[]
            {
                divSet.ITEM01_SEQ ?? 0,
                divSet.ITEM02_SEQ ?? 0,
                divSet.ITEM03_SEQ ?? 0,
                divSet.ITEM04_SEQ ?? 0,
                divSet.ITEM05_SEQ ?? 0,
                divSet.ITEM06_SEQ ?? 0,
                divSet.ITEM07_SEQ ?? 0,
                divSet.ITEM08_SEQ ?? 0,
                divSet.ITEM09_SEQ ?? 0,
                divSet.ITEM10_SEQ ?? 0
            };

            // 根據分攤順序取得各項金額（依順序 1-10）
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

            // 計算各項分攤比率
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
                    // 後續項目設為 0
                    for (int j = i + 1; j < 10; j++)
                    {
                        rateCal[j] = 0m;
                    }
                    break;
                }
            }

            // 如果總分攤金額不足目標，補足本金或調整目標
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

            // 12. 更新 FUND_DIV
            var now = DateTime.UtcNow;
            await connection.ExecuteAsync(@"
UPDATE MDS.FUND_DIV
SET NAV = @Nav,
    UNIT = @Unit,
    PRE_DIV1 = ISNULL(PRE_DIV1, 0),
    PRE_DIV2 = ISNULL(PRE_DIV2, 0),
    PRE_DIV3 = ISNULL(PRE_DIV3, 0),
    PRE_DIV4 = ISNULL(PRE_DIV4, 0),
    PRE_DIV5 = ISNULL(PRE_DIV5, 0),
    DIV1 = ISNULL(DIV1, 0),
    DIV2 = ISNULL(DIV2, 0),
    DIV3 = ISNULL(DIV3, 0),
    DIV4 = ISNULL(DIV4, 0),
    DIV5 = ISNULL(DIV5, 0),
    FEE = ISNULL(FEE, 0),
    DIV_TOT = CASE WHEN (ISNULL(PRE_DIV1,0)+ISNULL(PRE_DIV2,0)+ISNULL(PRE_DIV3,0)+ISNULL(PRE_DIV4,0)+ISNULL(PRE_DIV5,0)+ISNULL(DIV1,0)+ISNULL(DIV2,0)+ISNULL(DIV3,0)+ISNULL(DIV4,0)+ISNULL(DIV5,0)-ISNULL(FEE,0)) > 0 
                   THEN (ISNULL(PRE_DIV1,0)+ISNULL(PRE_DIV2,0)+ISNULL(PRE_DIV3,0)+ISNULL(PRE_DIV4,0)+ISNULL(PRE_DIV5,0)+ISNULL(DIV1,0)+ISNULL(DIV2,0)+ISNULL(DIV3,0)+ISNULL(DIV4,0)+ISNULL(DIV5,0)-ISNULL(FEE,0))
                   ELSE 0 END,
    DIV_RATE = CASE WHEN (ISNULL(PRE_DIV1,0)+ISNULL(PRE_DIV2,0)+ISNULL(PRE_DIV3,0)+ISNULL(PRE_DIV4,0)+ISNULL(PRE_DIV5,0)+ISNULL(DIV1,0)+ISNULL(DIV2,0)+ISNULL(DIV3,0)+ISNULL(DIV4,0)+ISNULL(DIV5,0)-ISNULL(FEE,0)) > 0 
                   THEN (ISNULL(PRE_DIV1,0)+ISNULL(PRE_DIV2,0)+ISNULL(PRE_DIV3,0)+ISNULL(PRE_DIV4,0)+ISNULL(PRE_DIV5,0)+ISNULL(DIV1,0)+ISNULL(DIV2,0)+ISNULL(DIV3,0)+ISNULL(DIV4,0)+ISNULL(DIV5,0)-ISNULL(FEE,0))/ISNULL(UNIT,1)
                   ELSE 0 END,
    DIV_RATE_M = @DivRateObj,
    DIV_RATE_O = @DivRateObj,
    CAPITAL_RATE = @CapitalRate,
    CAPITAL_RATE_M = @CapitalRate,
    FEE_RATE = @FeeRate,
    FEE_RATE_M = @FeeRate,
    DIV_OBJ = @DivObjRate,
    DIV_PRE = @DivPre,
    STEP2_STATUS = 'O',
    STEP3_STATUS = 'O',
    STEP2_COF_EMP = 'SYSTEM',
    STEP2_COF_TIME = @Now,
    STEP3_COF_EMP = 'SYSTEM',
    STEP3_COF_TIME = @Now
WHERE FUND_NO = @FundNo AND DIVIDEND_DATE = @Date AND DIVIDEND_TYPE = @Type",
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

            // 13. 更新分攤比率（根據分攤順序）
            // 根據分攤順序動態設定各項比率
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
        public decimal PreDiv1B { get; set; }
        public decimal Div1B { get; set; }
        public decimal Fee { get; set; }
        public decimal DivTot { get; set; }
        public decimal Nav { get; set; }
    }

    /// <summary>
    /// 建立分攤比率更新 SQL（根據分攤順序動態設定）
    /// </summary>
    private string BuildRateUpdateSql(int[] orders)
    {
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
    PRE_DIV5_RATE_M = CASE WHEN @Ord5 = 1 THEN @Rate1 WHEN @Ord5 = 2 THEN @Rate2 WHEN @Ord5 = 3 THEN @Rate3 WHEN @Ord5 = 4 THEN @Rate4 WHEN @Ord5 = 5 THEN @Rate5 WHEN @Ord5 = 6 THEN @Rate6 WHEN @Ord5 = 7 THEN @Rate7 WHEN @Ord5 = 8 THEN @Rate8 WHEN @Ord5 = 9 THEN @Rate9 WHEN @Ord5 = 10 THEN @Rate10 ELSE 0 END,
    DIV1_RATE = CASE WHEN @Ord6 = 1 THEN @Rate1 WHEN @Ord6 = 2 THEN @Rate2 WHEN @Ord6 = 3 THEN @Rate3 WHEN @Ord6 = 4 THEN @Rate4 WHEN @Ord6 = 5 THEN @Rate5 WHEN @Ord6 = 6 THEN @Rate6 WHEN @Ord6 = 7 THEN @Rate7 WHEN @Ord6 = 8 THEN @Rate8 WHEN @Ord6 = 9 THEN @Rate9 WHEN @Ord6 = 10 THEN @Rate10 ELSE 0 END,
    DIV1_RATE_M = CASE WHEN @Ord6 = 1 THEN @Rate1 WHEN @Ord6 = 2 THEN @Rate2 WHEN @Ord6 = 3 THEN @Rate3 WHEN @Ord6 = 4 THEN @Rate4 WHEN @Ord6 = 5 THEN @Rate5 WHEN @Ord6 = 6 THEN @Rate6 WHEN @Ord6 = 7 THEN @Rate7 WHEN @Ord6 = 8 THEN @Rate8 WHEN @Ord6 = 9 THEN @Rate9 WHEN @Ord6 = 10 THEN @Rate10 ELSE 0 END,
    DIV2_RATE = CASE WHEN @Ord7 = 1 THEN @Rate1 WHEN @Ord7 = 2 THEN @Rate2 WHEN @Ord7 = 3 THEN @Rate3 WHEN @Ord7 = 4 THEN @Rate4 WHEN @Ord7 = 5 THEN @Rate5 WHEN @Ord7 = 6 THEN @Rate6 WHEN @Ord7 = 7 THEN @Rate7 WHEN @Ord7 = 8 THEN @Rate8 WHEN @Ord7 = 9 THEN @Rate9 WHEN @Ord7 = 10 THEN @Rate10 ELSE 0 END,
    DIV2_RATE_M = CASE WHEN @Ord7 = 1 THEN @Rate1 WHEN @Ord7 = 2 THEN @Rate2 WHEN @Ord7 = 3 THEN @Rate3 WHEN @Ord7 = 4 THEN @Rate4 WHEN @Ord7 = 5 THEN @Rate5 WHEN @Ord7 = 6 THEN @Rate6 WHEN @Ord7 = 7 THEN @Rate7 WHEN @Ord7 = 8 THEN @Rate8 WHEN @Ord7 = 9 THEN @Rate9 WHEN @Ord7 = 10 THEN @Rate10 ELSE 0 END,
    DIV3_RATE = CASE WHEN @Ord8 = 1 THEN @Rate1 WHEN @Ord8 = 2 THEN @Rate2 WHEN @Ord8 = 3 THEN @Rate3 WHEN @Ord8 = 4 THEN @Rate4 WHEN @Ord8 = 5 THEN @Rate5 WHEN @Ord8 = 6 THEN @Rate6 WHEN @Ord8 = 7 THEN @Rate7 WHEN @Ord8 = 8 THEN @Rate8 WHEN @Ord8 = 9 THEN @Rate9 WHEN @Ord8 = 10 THEN @Rate10 ELSE 0 END,
    DIV3_RATE_M = CASE WHEN @Ord8 = 1 THEN @Rate1 WHEN @Ord8 = 2 THEN @Rate2 WHEN @Ord8 = 3 THEN @Rate3 WHEN @Ord8 = 4 THEN @Rate4 WHEN @Ord8 = 5 THEN @Rate5 WHEN @Ord8 = 6 THEN @Rate6 WHEN @Ord8 = 7 THEN @Rate7 WHEN @Ord8 = 8 THEN @Rate8 WHEN @Ord8 = 9 THEN @Rate9 WHEN @Ord8 = 10 THEN @Rate10 ELSE 0 END,
    DIV4_RATE = CASE WHEN @Ord9 = 1 THEN @Rate1 WHEN @Ord9 = 2 THEN @Rate2 WHEN @Ord9 = 3 THEN @Rate3 WHEN @Ord9 = 4 THEN @Rate4 WHEN @Ord9 = 5 THEN @Rate5 WHEN @Ord9 = 6 THEN @Rate6 WHEN @Ord9 = 7 THEN @Rate7 WHEN @Ord9 = 8 THEN @Rate8 WHEN @Ord9 = 9 THEN @Rate9 WHEN @Ord9 = 10 THEN @Rate10 ELSE 0 END,
    DIV4_RATE_M = CASE WHEN @Ord9 = 1 THEN @Rate1 WHEN @Ord9 = 2 THEN @Rate2 WHEN @Ord9 = 3 THEN @Rate3 WHEN @Ord9 = 4 THEN @Rate4 WHEN @Ord9 = 5 THEN @Rate5 WHEN @Ord9 = 6 THEN @Rate6 WHEN @Ord9 = 7 THEN @Rate7 WHEN @Ord9 = 8 THEN @Rate8 WHEN @Ord9 = 9 THEN @Rate9 WHEN @Ord9 = 10 THEN @Rate10 ELSE 0 END,
    DIV5_RATE = CASE WHEN @Ord10 = 1 THEN @Rate1 WHEN @Ord10 = 2 THEN @Rate2 WHEN @Ord10 = 3 THEN @Rate3 WHEN @Ord10 = 4 THEN @Rate4 WHEN @Ord10 = 5 THEN @Rate5 WHEN @Ord10 = 6 THEN @Rate6 WHEN @Ord10 = 7 THEN @Rate7 WHEN @Ord10 = 8 THEN @Rate8 WHEN @Ord10 = 9 THEN @Rate9 WHEN @Ord10 = 10 THEN @Rate10 ELSE 0 END,
    DIV5_RATE_M = CASE WHEN @Ord10 = 1 THEN @Rate1 WHEN @Ord10 = 2 THEN @Rate2 WHEN @Ord10 = 3 THEN @Rate3 WHEN @Ord10 = 4 THEN @Rate4 WHEN @Ord10 = 5 THEN @Rate5 WHEN @Ord10 = 6 THEN @Rate6 WHEN @Ord10 = 7 THEN @Rate7 WHEN @Ord10 = 8 THEN @Rate8 WHEN @Ord10 = 9 THEN @Rate9 WHEN @Ord10 = 10 THEN @Rate10 ELSE 0 END
WHERE FUND_NO = @FundNo AND DIVIDEND_DATE = @Date AND DIVIDEND_TYPE = @Type";

        // 替換參數名稱
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
            Map(m => m.PreDiv1B).Name("pre_div1_b");
            Map(m => m.Div1B).Name("div1_b");
            Map(m => m.Fee).Name("fee");
            Map(m => m.DivTot).Name("div_tot");
            Map(m => m.Nav).Name("nav").Optional();
        }
    }
}

