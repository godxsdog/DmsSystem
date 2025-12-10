namespace DmsSystem.Infrastructure.Persistence.SqlQueries;

/// <summary>
/// 配息相關 SQL 查詢語句
/// </summary>
public static class DividendSqlQueries
{
    /// <summary>
    /// 匯入配息資料的 MERGE 語句
    /// </summary>
    public const string UpsertFundDiv = @"
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
            STEP2_STATUS, STEP2_CRE_EMP, STEP2_CRE_TIME)
    VALUES (@FundNo, @DividendYear, @DividendDate, @DividendType, @Nav, 0,
            @PreDiv1, @PreDiv2, @PreDiv3, @PreDiv4, @PreDiv5,
            @Div1, @Div2, @Div3, @Div4, @Div5,
            @PreDiv1B, @Div1B, @Fee, @DivTot,
            'C', 'SYSTEM', @Now)
    OUTPUT $action;";

    /// <summary>
    /// 查詢 NAV
    /// </summary>
    public const string GetNav = @"
SELECT NAV 
FROM NAV.NAV 
WHERE FUND_NO = @FundNo AND NAV_DATE = @Date";

    /// <summary>
    /// 查詢單位數
    /// </summary>
    public const string GetUnit = @"
SELECT BAL_SHARE 
FROM FAS.FUND_LEDGE 
WHERE FUND_NO = @FundNo AND AC_DATE = @Date";

    /// <summary>
    /// 查詢 FUND_DIV 資料
    /// </summary>
    public const string GetFundDiv = @"
SELECT PRE_DIV1, PRE_DIV2, PRE_DIV3, PRE_DIV4, PRE_DIV5,
       DIV1, DIV2, DIV3, DIV4, DIV5, FEE, DIV_TOT
FROM MDS.FUND_DIV
WHERE FUND_NO = @FundNo AND DIVIDEND_DATE = @Date AND DIVIDEND_TYPE = @Type";

    /// <summary>
    /// 查詢配息參數設定
    /// </summary>
    public const string GetFundDivSet = @"
SELECT ITEM01_SEQ, ITEM02_SEQ, ITEM03_SEQ, ITEM04_SEQ, ITEM05_SEQ,
       ITEM06_SEQ, ITEM07_SEQ, ITEM08_SEQ, ITEM09_SEQ, ITEM10_SEQ,
       CAPITAL_TYPE
FROM MDS.FUND_DIV_SET
WHERE FUND_NO = @FundNo AND DIV_TYPE = @Type";

    /// <summary>
    /// 查詢目標配息率
    /// </summary>
    public const string GetFundDivObj = @"
SELECT DIV_OBJ, DIV_OBJ_AMT
FROM MDS.FUND_DIV_OBJ
WHERE FUND_NO = @FundNo AND DIV_TYPE = @Type AND TX_DATE <= @Date
ORDER BY TX_DATE DESC";

    /// <summary>
    /// 查詢上期配息率
    /// </summary>
    public const string GetPreviousFundDiv = @"
SELECT DIV_RATE_M, NAV
FROM MDS.FUND_DIV
WHERE FUND_NO = @FundNo AND DIVIDEND_TYPE = @Type AND DIVIDEND_DATE < @Date
ORDER BY DIVIDEND_DATE DESC";

    /// <summary>
    /// 更新 FUND_DIV 配息計算結果
    /// </summary>
    public const string UpdateFundDivCalculation = @"
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
WHERE FUND_NO = @FundNo AND DIVIDEND_DATE = @Date AND DIVIDEND_TYPE = @Type";

    /// <summary>
    /// 查詢待確認的配息紀錄
    /// </summary>
    public const string GetPendingFundDivs = @"
SELECT FUND_NO AS FundNo, DIVIDEND_DATE AS DividendDate, DIVIDEND_TYPE AS DividendType
FROM MDS.FUND_DIV
WHERE STEP2_STATUS = 'C'
AND (@Date IS NULL OR DIVIDEND_DATE = @Date)";
}
