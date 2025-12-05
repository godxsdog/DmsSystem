using System.Globalization;
using System.Data;
using System.Data.Common;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using DmsSystem.Application.DTOs;
using DmsSystem.Application.Interfaces;
using DmsSystem.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace DmsSystem.Application.Services;

/// <summary>
/// 5A1 配息匯入與計算服務（使用 Dapper 直連資料庫）
/// </summary>
public class DividendService : IDividendService
{
    private readonly DmsDbContext _context;
    private readonly ILogger<DividendService> _logger;

    public DividendService(DmsDbContext context, ILogger<DividendService> logger)
    {
        _context = context;
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

        await using var connection = _context.Database.GetDbConnection();
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

    public async Task<DividendConfirmResult> ConfirmAsync(string fundNo, DateOnly dividendDate, string dividendType)
    {
        await using var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        // 取得 NAV 與單位數
        var nav = await connection.ExecuteScalarAsync<decimal?>(
            "SELECT NAV FROM NAV.NAV WHERE FUND_NO = @FundNo AND NAV_DATE = @Date",
            new { FundNo = fundNo, Date = dividendDate.ToDateTime(TimeOnly.MinValue) });

        var unit = await connection.ExecuteScalarAsync<decimal?>(
            "SELECT BAL_SHARE FROM FAS.FUND_LEDGE WHERE FUND_NO = @FundNo AND AC_DATE = @Date",
            new { FundNo = fundNo, Date = dividendDate.ToDateTime(TimeOnly.MinValue) });

        var fund = await connection.QuerySingleOrDefaultAsync<dynamic>(
            @"SELECT PRE_DIV1, PRE_DIV2, PRE_DIV3, PRE_DIV4, PRE_DIV5,
                     DIV1, DIV2, DIV3, DIV4, DIV5, FEE
              FROM MDS.FUND_DIV
              WHERE FUND_NO = @FundNo AND DIVIDEND_DATE = @Date AND DIVIDEND_TYPE = @Type",
            new { FundNo = fundNo, Date = dividendDate.ToDateTime(TimeOnly.MinValue), Type = dividendType });

        if (fund == null)
        {
            return new DividendConfirmResult(false, "找不到對應的 FUND_DIV 記錄", null, null, null, null, null, null);
        }

        decimal pre1 = fund.PRE_DIV1 ?? 0m;
        decimal pre2 = fund.PRE_DIV2 ?? 0m;
        decimal pre3 = fund.PRE_DIV3 ?? 0m;
        decimal pre4 = fund.PRE_DIV4 ?? 0m;
        decimal pre5 = fund.PRE_DIV5 ?? 0m;
        decimal div1 = fund.DIV1 ?? 0m;
        decimal div2 = fund.DIV2 ?? 0m;
        decimal div3 = fund.DIV3 ?? 0m;
        decimal div4 = fund.DIV4 ?? 0m;
        decimal div5 = fund.DIV5 ?? 0m;
        decimal fee = fund.FEE ?? 0m;

        decimal divTot = pre1 + pre2 + pre3 + pre4 + pre5 + div1 + div2 + div3 + div4 + div5;
        if (divTot > fee)
        {
            divTot -= fee;
        }
        else
        {
            divTot = 0m;
        }

        decimal divRate = 0m;
        if (unit.HasValue && unit.Value > 0)
        {
            divRate = Math.Round(divTot / unit.Value, 6);
        }

        decimal divRateM = dividendType switch
        {
            "M" => divRate,
            "Q" => Math.Round(divRate / 3m, 6),
            "S" => Math.Round(divRate / 6m, 6),
            "Y" => Math.Round(divRate / 12m, 6),
            _ => divRate
        };

        const string updateSql = @"
UPDATE MDS.FUND_DIV
SET NAV = @Nav,
    UNIT = ISNULL(@Unit, UNIT),
    DIV_TOT = @DivTot,
    DIV_RATE = @DivRate,
    DIV_RATE_M = @DivRateM,
    DIV_RATE_O = @DivRateM,
    CAPITAL_RATE = 0,
    STEP2_STATUS = 'O',
    STEP3_STATUS = 'O',
    STEP2_COF_EMP = 'SYSTEM',
    STEP2_COF_TIME = @Now,
    STEP3_COF_EMP = 'SYSTEM',
    STEP3_COF_TIME = @Now
WHERE FUND_NO = @FundNo AND DIVIDEND_DATE = @Date AND DIVIDEND_TYPE = @Type;";

        var now = DateTime.UtcNow;
        await connection.ExecuteAsync(updateSql, new
        {
            Nav = nav ?? 0m,
            Unit = unit ?? 0m,
            DivTot = divTot,
            DivRate = divRate,
            DivRateM = divRateM,
            FundNo = fundNo,
            Date = dividendDate.ToDateTime(TimeOnly.MinValue),
            Type = dividendType,
            Now = now
        });

        return new DividendConfirmResult(true, "計算完成並已更新 FUND_DIV", nav, unit, divTot, divRate, divRateM, 0m);
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

