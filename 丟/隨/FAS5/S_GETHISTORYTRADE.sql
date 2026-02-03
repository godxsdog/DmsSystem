--  DDL for Procedure S_GETHISTORYTRADE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "ECS"."S_GETHISTORYTRADE" 
-- =============================================
-- Author:      JIANXIN
-- Create date: 2018/08/13
-- Description: 歷史交易
-- 2018.08.22 JIANXIN MOD BY 調整收益分配 BRANCH_NO = '7000'
-- 2018.08.23 JIANXIN MOD BY 新增帳號隱碼函數
-- 2018.09.05 tingting MOD BY 1.修正轉申購手續費欄位錯誤 
--                            2.(不)定額扣款列表 分開拆成定額扣款列表、不定額扣款列表及相關調整 
--                            3.申購列表、定額扣款列表增加傳出欄位：申購總價金、台幣申購總價金 
--                            4.不定額扣款列表增加傳出欄位：申購總價金、台幣申購總價金、加碼點(%)、加碼金額、減碼點(%)、減碼金額
-- 2018.09.07 JIANXIN MOD BY 因應系統資料無狀態欄位，進行調整
-- 2018.09.07 tingting MOD BY 定額扣款列表、不定額扣款列表增加傳出欄位：每月扣款日
-- 2018.09.07 tingting MOD BY 1.申購列表' 定額扣款列表、不定額扣款列表 增加傳出欄位：扣款總行代碼、扣款分行代碼、扣款銀行簡稱、扣款帳號、扣款帳號後5碼
--                            2.調整申購列表 交易狀態 = S：交易成功 的取資料方式(Mark掉Union上面那段，從下面那段出資料)
-- 2018.09.24 JIANXIN MOD BY 調整日期為配息日
-- 2018.10.05 JIANXIN MOD BY 歷史贖回交易顯示處理
-- 2018.10.08 JIANXIN MOD BY 處理基金級別串接問題
-- 2018.10.09 ChengYu 增加傳出參數 風險屬性(RISK_CATEGORY)
-- 2018.10.12 JIANXIN MOD BY 調整歷史交易申購重複問題
-- 2018.10.12 yunchu 調整轉入.轉出淨值為依據轉入基金及轉出基金
-- 2018.10.17 ChengYu 修正轉申購的手續費、贖回金額、轉申購金額、轉申購單位數
-- 2018.10.24 ChengYu 調整FAS.PURCHASE、FAS.RED_BANK 的JOIN語法
-- 2018.10.25 tingting 1.增加傳出欄位：排列序號
--                     2.調整排序：先依交易日期、交易狀態(處理中→交易成功→交易失敗)
--                                再依基金排序規則(境內外、基金公司、投信顧公會基金類型、母基金排序、基金級別、子基金排序)
-- 2018.10.30 ChengYu 補FAS.PURCHASE、FAS.RED_BANK 的JOIN條件
-- 2018.11.01 JIANXIN 排除Robo交易
-- 2018.11.09 PEN MOD 網路定額、網路不定額列表 調整 處理中 的判斷條件
-- 2018.11.30 ChengYu 修正轉申購 轉出淨值、轉入淨值 抓取方式
-- 2018.12.03 JIANXIN 調整 發放日、每單位分配金額、實際分配淨額、總分配金額 取得邏輯
-- 2018.12.04 ChengYu 修正轉申購金額、手續費欄位(瀚亞NTD_AMOUNT、NTD_SERVICE_CHARGE欄位無值，改抓AMOUNT、SERVICE_CHARGE)
-- 2018.12.04 ChengYu 調整轉申購金額、手續費、交易幣別、交易幣別名稱、轉入小數位數、淨值日
-- 2018.12.05 JIANXIN 1. 調整贖回金額、贖回淨額等欄位取得方式
--                    2. 贖回交易幣別改為計價幣別
--                    3. 不定額百分比顯示調整
-- 2018.12.06 JIANXIN 調整配息內容交易幣別與計價幣別回傳錯誤
-- 2018.12.12 JIANXIN MOD BY 調整贖回串接FE_BROKER_NO 
-- 2018.12.18 JIANXIN 調整配息的內容
-- 2018.12.18 JIANXIN 調整配息相關費用的內容
-- 2018.12.19 JIANXIN 依據 與瀚亞Ada討論，邏輯調整如下:
--                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO <> 計價幣 則 回傳 NTD_AMOUT
--                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO = 計價幣 則 回傳 AMOUT
--                    配息=再申購：金額 回傳 AMOUT
--                    配息=再申購：交易幣別 回傳 計價幣別
-- 2018.12.27 JIANXIN 若付款日期無資料，則回傳1900/01/01 
-- 2019.02.14 yunchu 調整欄位名稱買回預計入款日(PAY_DATE) 
-- 2019.02.15 yunchu 調整買回預計入款日字串轉日期 
-- 2019.02.21 JIANXIN 若付款日期無資料，則回傳0001/01/01 (暫時解決線上問題) 
-- 2019.03.11 JIANXIN 若付款日期無資料，則回傳1900/01/01 
-- 2019.04.17 tingting 因應需求編號"ES_20190315_01"，新增輸出欄位：
--                     單筆申購　：新增【是否為配息(IS_DIVIDEND)】、【配息頻率(DIVIDEND_TYPE)】、【配息頻率名稱(DIVIDEND_TYPE_NAME)】、【委託日期(RCV_DATE)】、【優惠方案代碼(FEE_CHOICE_TYPE)】、【優惠方案名稱(FEE_CHOICE_NAME)】、【促銷活動代碼(PROJECT_CODE)】、【促銷活動名稱(PROJECT_NAME)】欄位
--                     贖回　　　：新增【是否為配息(IS_DIVIDEND)】、【配息頻率(DIVIDEND_TYPE)】、【配息頻率名稱(DIVIDEND_TYPE_NAME)】、【委託日期(RCV_DATE)】欄位
--                     轉申購　　：新增【轉出基金是否為配息(IS_DIVIDEND_OUT)】、【轉出基金配息頻率(DIVIDEND_TYPE_OUT)】、【轉出基金配息頻率名稱(DIVIDEND_TYPE_NAME_OUT)】、【轉入基金是否為配息(IS_DIVIDEND_IN)】、【轉入基金配息頻率(DIVIDEND_TYPE_IN)】、【轉入基金配息頻率名稱(DIVIDEND_TYPE_NAME_IN)】、【委託日期(RCV_DATE)】、【郵匯費(WIRE_FEE)】、【轉出基金淨額(NET_AMOUNT_OUT)】、【匯率日期(EX_DATE)】、【匯率-交叉匯率(EX_RATE)】、【優惠方案代碼(FEE_CHOICE_TYPE)】、【優惠方案名稱(FEE_CHOICE_NAME)】、【促銷活動代碼(PROJECT_CODE)】、【促銷活動名稱(PROJECT_NAME)】欄位
--                     定期定額　：新增【是否為配息(IS_DIVIDEND)】、【配息頻率(DIVIDEND_TYPE)】、【配息頻率名稱(DIVIDEND_TYPE_NAME)】、【委託日期(RCV_DATE)】、【優惠方案代碼(FEE_CHOICE_TYPE)】、【優惠方案名稱(FEE_CHOICE_NAME)】、【促銷活動代碼(PROJECT_CODE)】、【促銷活動名稱(PROJECT_NAME)】
--                     定期不定額：新增【是否為配息(IS_DIVIDEND)】、【配息頻率(DIVIDEND_TYPE)】、【配息頻率名稱(DIVIDEND_TYPE_NAME)】、【委託日期(RCV_DATE)】、【優惠方案代碼(FEE_CHOICE_TYPE)】、【優惠方案名稱(FEE_CHOICE_NAME)】、【促銷活動代碼(PROJECT_CODE)】、【促銷活動名稱(PROJECT_NAME)】
--                     收益分配　：新增【是否為配息(IS_DIVIDEND)】、【配息頻率(DIVIDEND_TYPE)】、【配息頻率名稱(DIVIDEND_TYPE_NAME)】
-- 2019.04.29 tingting 修正申購列表單位數錯誤
-- 2019.04.29 tingting 修正定額扣款列表、不定額扣款列表單位數錯誤
-- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
-- 2019.05.09 tingting 贖回交易幣別改回付款幣別
-- 2019.05.09 JIANXIN 贖回交易幣別改從買回付款(含轉申購)檔來、修正贖回價金錯誤
-- 2019.05.09 tingting 調整促銷活動名稱顯示
-- 2019.05.14 tingting 修正促銷活動名稱顯示(語法問題造成網路費率無法顯示)
-- 2019.12.25 ChengYu 贖回使用FAS.REDEMPTION【買回】的欄位匯率日(EX_DATE)來取匯率(EX_RATE)、轉申購雖從FAS.PURCHASE抓取不影響，但調整轉出轉入基金匯率與FAS.PURCHASE保持規則一致性
-- 2020.01.21 JIANXIN 1. 匯率改由買進匯率取得
--                    2. 贖回金額顯示方式為交易幣別與計價幣別不同時，顯示不同的金額欄位
-- 2020.04.14 JIANXIN 調整轉申購發生效能問題
-- 2020.04.28 Chengyu 調整贖回與轉申購效能
-- 2020.08.31 JIANXIN 將處理中的轉申購與贖回另外處理
-- 2020.12.15 Chengyu 調整淨值資料限制條件為NAV_DATE
-- 2021.05.10 Chengyu 修正贖回、轉申購時，FAS.REDEMPTION串接條件
-- 2021.06.24 Chengyu 新增書面資料
-- 2021.07.19 Chengyu 調整書面與ROBO資料取法
-- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
-- 2021.07.22 JIANXIN 調整書面相關資訊並且整理程式碼
-- 2021.09.03 Chengyu 1.調整申購金額顯示
--                    2.修正轉申購資料顯示
-- 2021.09.22 Chengyu 修正轉申購時的申購資料串接語法
-- 2021.09.24 Chengyu 新增欄位除息日
-- 2021.10.14 Chengyu 調整分配串接FAS.PURCHASE邏輯(TX_DATE)
-- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
-- 2021.10.28 Chengyu 調整分配串接FAS.PURCHASE邏輯(TX_DATE)
-- 2022.03.02 Chengyu 贖回與轉申購新增欄位"報酬率"
-- 2022.04.14 JIANXIN 調整報酬率小數點2位
-- 2022.04.21 JIANXIN 調整轉申購配息不能用PUR_SER_NO (因為有一些是空的)
-- 2022.05.24 JIANXIN 調整報酬率計算方式，若遇到配息再申購或者是瑞萬系列的基金，則以 CERTIFICATE.AMOUNT 作為成本，否則以 CERTIFICATE.COST 作為成本
-- 2022.10.19 JIANXIN 調整報酬率計算方式
-- 2023.10.25 JIANXIN 調整匯率改由函數取得
-- 2023.10.27 JIANXIN 若交易幣=MMA、FCY、NULL，則回傳RDEMEPTION的幣別
-- 2024.05.24 RICHARD 調整PURCHASE.FEE_CHOICE = '5' 由固定hard code改為,campaign.campagin_shnm
-- 2024.08.01 RICHARD 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
-- 2025.10.20 Patrick 加上Key值 (SER_NO) 
-- =============================================
(
     WACCOUNT_NO        INTEGER             -- 受益人戶號
    ,WTX_DATE_FROM      DATE                -- 交易日期(起)
    ,WTX_DATE_TO        DATE                -- 交易日期(迄)
    ,WTX_TYPE           VARCHAR2            -- 交易類別　ALL：全部；1：單筆申購；2：定期定額；3：定期不定額；4：買回；5：轉申購；6：收益分配
    ,WTX_STATUS         VARCHAR2            -- 交易狀態　C：處理中；S：交易成功；F：交易失敗
    ,WDIV_TYPE          VARCHAR2            -- 收益分配方式　1：匯款；2：再投資；3：郵寄支票
    ,ERRDT              OUT SYS_REFCURSOR   -- 錯誤訊息列表
    ,ALLOTDT            OUT SYS_REFCURSOR   -- 申購列表
    ,REDEMDT            OUT SYS_REFCURSOR   -- 贖回列表
    ,SWITCHDT           OUT SYS_REFCURSOR   -- 轉申購列表
    ,RSPDT              OUT SYS_REFCURSOR   -- 定額扣款列表
    ,DRSPDT             OUT SYS_REFCURSOR   -- 不定額扣款列表
    ,DIVDENDDT          OUT SYS_REFCURSOR   -- 收益分配列表
) IS

    XID                 VARCHAR2(10);       -- 受益人ID
    XWARNS_TYPE         VARCHAR2(1);        -- 警示方式
    XERROR_MSG          VARCHAR2(100);      -- 錯誤訊息

BEGIN

    BEGIN

        --取得受益人ID
        SELECT ID 
          INTO XID
          FROM FAS.HOLDER
         WHERE ACCOUNT_NO = WACCOUNT_NO;

        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                RAISE_APPLICATION_ERROR(-20000, '無法取得受益人ID，請檢查！');

    END;

    -- ============================================================== 單筆申購 BNG ==============================================================  
    OPEN ALLOTDT FOR
    SELECT  ROWNUM AS ROW_NUM                                     -- 排列序號
           ,DataList.*
      FROM (
            -- EC 單筆申購 交易失敗 或 處理中 或 交易成功
            SELECT  PURCHASE.FUND_NO                              -- 基金代碼
                   ,FUND.NAME AS FUND_NAME                        -- 基金名稱
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO               -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                      -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                   -- 國際證券代碼
                   ,FUND.SHARE_CLASS                              -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME     -- 基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO               -- 基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME           -- 基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL      -- 基金計價幣別小數位數
                   -- 2018.10.09 ChengYu 增加傳出參數 風險屬性(RISK_CATEGORY)
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY           -- 風險屬性
                   ,PURCHASE.TX_DATE                              -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,CASE WHEN PURCHASE.BRANCH_NO = '8000' THEN 'L1'
                         ELSE '' 
                     END AS PRODUCT_TYPE                          -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,CASE WHEN PURCHASE.BRANCH_NO = '8000' THEN ECS.F_GET_PRODUCT_NAME('L1')
                         ELSE '' 
                     END AS PRODUCT_NAME                          -- 產品類別名稱
                   ,PURCHASE.CURRENCY_NO AS TRADE_CRNCY           -- 付款幣別
                   ,CURRENCY.NAME AS TRADE_CRNCY_NAME             -- 付款幣別名稱
                   ,CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL     -- 付款幣別小數位數
                   -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
                   ,CASE WHEN FUND.CURRENCY_NO = PURCHASE.CURRENCY_NO THEN 1 
                         ELSE NVL(FAS_PURCHASE.EX_RATE,0)
                     END AS EX_RATE                               -- 參考匯率
                   ,PURCHASE.AMOUNT                               -- 原幣申購價金
                   -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_AMOUNT IS NOT NULL THEN FAS_PURCHASE.NTD_AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.AMOUNT IS NOT NULL THEN FAS_PURCHASE.AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.AMOUNT  --未轉入FAS時顯示
                         ELSE 0
                     END AS NTD_AMOUNT                            -- 台幣申購價金
                   -- 2019.04.29 tingting 修正申購列表單位數錯誤
                   ,FAS_PURCHASE.SHARES AS SHARES                 -- 單位數
                   ,FUND.SHARE_DECIMAL                            -- 單位數小數位數
                   ,PURCHASE.AC_DATE                              -- 淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN PURCHASE.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV.NAV END NAV                     -- 淨值 
                   ,FUND.NAV_DECIMAL                              -- 淨值小數位數
                   ,PURCHASE.SERVICE_CHARGE                       -- 原幣手續費 
                   -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.NTD_SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.SERVICE_CHARGE --未轉入FAS時顯示
                         ELSE 0 
                     END AS NTD_SERVICE_CHARGE                    -- 台幣申購手續費
                   -- 2018.09.05 tingting MOD BY 1.修正轉申購手續費欄位錯誤 
                   --                            2.(不)定額扣款列表 分開拆成定額扣款列表、不定額扣款列表及相關調整 
                   --                            3.申購列表、定額扣款列表增加傳出欄位：申購總價金、台幣申購總價金 
                   --                            4.不定額扣款列表增加傳出欄位：申購總價金、台幣申購總價金、加碼點(%)、加碼金額、減碼點(%)、減碼金額
                   ,ROUND(PURCHASE.AMOUNT + PURCHASE.SERVICE_CHARGE, FUND_CURRENCY.AMT_DECIMAL) AS TOT_AMOUNT    -- 原幣申購總價金
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,ROUND(CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_AMOUNT IS NOT NULL THEN FAS_PURCHASE.NTD_AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.AMOUNT IS NOT NULL THEN FAS_PURCHASE.AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.AMOUNT  --未轉入FAS時顯示
                               ELSE 0 
                           END + 
                          CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.NTD_SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.SERVICE_CHARGE --未轉入FAS時顯示
                               ELSE 0  
                           END, 0) AS NTD_TOT_AMOUNT     -- 台幣申購總價金
                   ,CASE WHEN PURCHASE.STATUS = 'C' THEN 'C'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE = '00' THEN 'S'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE <> '00' THEN 'F'
                         ELSE '' 
                     END AS STATUS                               -- 交易狀態代碼
                   ,CASE WHEN PURCHASE.STATUS = 'C' THEN '處理中'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE = '00' THEN '交易成功'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE <> '00' THEN '交易失敗'
                         ELSE '' 
                     END AS STATUS_NAME                           -- 交易狀態
                   ,PURCHASE.BROKER_NO                            -- 銷售機構代碼
                   ,PURCHASE.BRANCH_NO                            -- 銷售機構分行代碼
                   ,PURCHASE.SER_NO                               -- 成交序號
                   ,PURCHASE.PAY_TYPE                             -- 付款方式
                   ,PAY_TYPE.NAME AS PAY_TYPE_NAME                -- 付款方式名稱
                   -- 2018.09.07 tingting MOD BY 1.申購列表' 定額扣款列表、不定額扣款列表 增加傳出欄位：扣款總行代碼、扣款分行代碼、扣款銀行簡稱、扣款帳號、扣款帳號後5碼
                   --                            2.調整申購列表 交易狀態 = S：交易成功 的取資料方式(Mark掉Union上面那段，從下面那段出資料)
                   ,PURCHASE.BANK_NO AS AC_BANK                   -- 扣款總行代碼
                   ,PURCHASE.AC_BRANCH                            -- 扣款分行代碼
                   ,FIS_BRANCH.SNAME AS FIS_SNAME                 -- 扣款銀行簡稱
                   ,PURCHASE.AC_CODE                              -- 扣款帳號
                   ,ECS.FN_STUFF_STR(PURCHASE.AC_CODE,5,'*') AS AC_CODE_5  -- 往來帳號後5碼
                   -- 2019.04.17 tingting 因應需求編號"ES_20190315_01"，新增輸出欄位
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE           -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME             -- 配息頻率名稱
                   ,PURCHASE_LOG.REVISION_DATE AS RCV_DATE        -- 委託日期(含時分秒)
                   ,PURCHASE.FEE_CHOICE AS FEE_CHOICE_TYPE        -- 優惠方案代碼
                   -- 2019.05.09 tingting 調整促銷活動名稱顯示
                   -- 2019.05.14 tingting 修正促銷活動名稱顯示(語法問題造成網路費率無法顯示)
                   -- 2024.08.01 RICHARD begin 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
--                   ,CASE WHEN PURCHASE.FEE_CHOICE = '1' THEN CAMPAIGN.CAMPAIGN_SHNM
--                         WHEN PURCHASE.FEE_CHOICE = '2' THEN '優惠券' || PURCHASE.COUPON_ID
--                         WHEN PURCHASE.FEE_CHOICE = '3' THEN '紅利折抵' || PURCHASE.USED_POINT || '點'
--                         WHEN PURCHASE.FEE_CHOICE = '5' THEN CAMPAIGN.CAMPAIGN_SHNM
                   ,CASE WHEN PURCHASE.FEE_CHOICE in ('1','2','3','5') THEN CAMPAIGN.CAMPAIGN_SHNM
                   -- 2024.08.01 RICHARD end 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
                         WHEN PURCHASE.FEE_CHOICE IS NULL THEN '網路費率'
                         END AS FEE_CHOICE_NAME                   -- 優惠方案名稱
                   ,PURCHASE.PROJECT_CODE                         -- 促銷活動代碼
                   ,CAMPAIGN.CAMPAIGN_SHNM AS PROJECT_NAME        -- 促銷活動名稱
                   ,FUND.FUND_CATEGORY
                   ,FUND.AMC_NO
                   ,FUND.SITCA_FUND_TYPE
                   ,FUND.ORDINAL
              FROM ECS.PURCHASE PURCHASE
              JOIN FAS.FUND
                ON PURCHASE.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON PURCHASE.CURRENCY_NO = CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              JOIN FAS.PAY_TYPE
                ON PAY_TYPE.PAY_TYPE = PURCHASE.PAY_TYPE
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               -- 2018.10.08 JIANXIN MOD BY 處理基金級別串接問題
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              LEFT JOIN FAS.PURCHASE FAS_PURCHASE
                ON FAS_PURCHASE.ID = PURCHASE.ID
               AND FAS_PURCHASE.FUND_NO = PURCHASE.FUND_NO
               AND FAS_PURCHASE.TX_DATE = PURCHASE.TX_DATE
               AND FAS_PURCHASE.BRANCH_NO = PURCHASE.BRANCH_NO
               AND FAS_PURCHASE.SER_NO = PURCHASE.SER_NO
               -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
              LEFT JOIN NAV.NAV
                ON NAV.FUND_NO = FUND.FUND_NO
               AND NAV.NAV_DATE = PURCHASE.AC_DATE
              LEFT JOIN FAS.FIS_BRANCH FIS_BRANCH
                ON FIS_BRANCH.BANK_NO = PURCHASE.BANK_NO
               AND FIS_BRANCH.BRANCH_NO = PURCHASE.AC_BRANCH
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN (
                    SELECT  PURCHASE_LOG.FUND_NO
                           ,PURCHASE_LOG.ID
                           ,PURCHASE_LOG.TX_DATE
                           ,PURCHASE_LOG.BROKER_NO
                           ,PURCHASE_LOG.BRANCH_NO
                           ,PURCHASE_LOG.SER_NO
                           ,MAX(PURCHASE_LOG.REVISION_DATE) AS REVISION_DATE 
                      FROM ECS.PURCHASE_LOG     -- EC申購異動紀錄
                     WHERE PURCHASE_LOG.ACTION_TYPE = 'A'   -- 交易動作(A:新增/M:修改/D:刪除)
                       AND PURCHASE_LOG.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
                     GROUP BY PURCHASE_LOG.FUND_NO
                           ,PURCHASE_LOG.ID
                           ,PURCHASE_LOG.TX_DATE
                           ,PURCHASE_LOG.BROKER_NO
                           ,PURCHASE_LOG.BRANCH_NO
                           ,PURCHASE_LOG.SER_NO
                   ) PURCHASE_LOG       -- EC申購異動紀錄
                ON PURCHASE_LOG.FUND_NO = PURCHASE.FUND_NO
               AND PURCHASE_LOG.ID = PURCHASE.ID
               AND PURCHASE_LOG.TX_DATE = PURCHASE.TX_DATE
               AND PURCHASE_LOG.BROKER_NO = PURCHASE.BROKER_NO
               AND PURCHASE_LOG.BRANCH_NO = PURCHASE.BRANCH_NO
               AND PURCHASE_LOG.SER_NO = PURCHASE.SER_NO
              LEFT JOIN ECS.CAMPAIGN    -- 促銷活動
                ON CAMPAIGN.CAMPAIGN_CODE = PURCHASE.PROJECT_CODE
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE PURCHASE.ID = XID
               -- 2018.11.01 JIANXIN 排除Robo交易
               AND TX_TYPE.TX_TYPE IN ('ALL' ,'1' ) 
               AND PURCHASE.BRANCH_NO IN ('8000') -- EC單筆：8000；Robo :8800
               -- 2018.10.12 JIANXIN MOD BY 調整歷史交易申購重複問題
               AND (  (TX_STATUS.TX_STATUS = 'C' AND PURCHASE.STATUS = 'C')  -- 處理中：STATUS='C'；交易失敗：STATUS='P' AND RESULT_CODE <> '00'
                   OR (TX_STATUS.TX_STATUS = 'F' AND PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE <> '00')
                   OR (TX_STATUS.TX_STATUS = 'S' AND PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE = '00') -- 交易成功
                   )   
               AND PURCHASE.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO             
            UNION
            -- 2021.06.24 Chengyu 新增書面資料
            SELECT  PURCHASE.FUND_NO                              -- 基金代碼
                   ,FUND.NAME AS FUND_NAME                        -- 基金名稱
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO               -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                      -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                   -- 國際證券代碼
                   ,FUND.SHARE_CLASS                              -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME     -- 基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO               -- 基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME           -- 基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL      -- 基金計價幣別小數位數
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY           -- 風險屬性
                   ,PURCHASE.TX_DATE                              -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,PURCHASE.PRODUCT_TYPE AS PRODUCT_TYPE                          -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,ECS.F_GET_PRODUCT_NAME(PURCHASE.PRODUCT_TYPE) AS PRODUCT_NAME                          -- 產品類別名稱
                   ,PURCHASE.CURRENCY_NO AS TRADE_CRNCY           -- 付款幣別
                   ,CURRENCY.NAME AS TRADE_CRNCY_NAME             -- 付款幣別名稱
                   ,CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL     -- 付款幣別小數位數
                   ,CASE WHEN FUND.CURRENCY_NO = PURCHASE.CURRENCY_NO THEN 1 
                         ELSE NVL(PURCHASE.EX_RATE,0)
                     END AS EX_RATE                               -- 參考匯率
                   ,PURCHASE.AMOUNT                               -- 原幣申購價金
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_AMOUNT IS NOT NULL THEN PURCHASE.NTD_AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.AMOUNT IS NOT NULL THEN PURCHASE.AMOUNT
                         ELSE 0 
                     END AS NTD_AMOUNT                            -- 台幣申購價金
                   ,PURCHASE.SHARES AS SHARES                     -- 單位數
                   ,FUND.SHARE_DECIMAL                            -- 單位數小數位數
                   ,PURCHASE.AC_DATE                              -- 淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN PURCHASE.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV.NAV END NAV                     -- 淨值 
                   ,FUND.NAV_DECIMAL                              -- 淨值小數位數
                   ,PURCHASE.SERVICE_CHARGE                       -- 原幣手續費 
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN PURCHASE.NTD_SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.SERVICE_CHARGE IS NOT NULL THEN PURCHASE.SERVICE_CHARGE
                         ELSE 0
                     END AS NTD_SERVICE_CHARGE                    -- 台幣申購手續費
                   ,ROUND(PURCHASE.AMOUNT + PURCHASE.SERVICE_CHARGE, FUND_CURRENCY.AMT_DECIMAL) AS TOT_AMOUNT    -- 原幣申購總價金
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,ROUND(CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_AMOUNT IS NOT NULL THEN PURCHASE.NTD_AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.AMOUNT IS NOT NULL THEN PURCHASE.AMOUNT
                               ELSE 0 
                           END + 
                          CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN PURCHASE.NTD_SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.SERVICE_CHARGE IS NOT NULL THEN PURCHASE.SERVICE_CHARGE
                               ELSE 0
                           END, 0) AS NTD_TOT_AMOUNT     -- 台幣申購總價金
                   -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
                   ,'S' AS STATUS                                 -- 交易狀態代碼
                   ,'交易成功' AS STATUS_NAME                     -- 交易狀態
                   ,PURCHASE.BROKER_NO                            -- 銷售機構代碼
                   ,PURCHASE.BRANCH_NO                            -- 銷售機構分行代碼
                   ,PURCHASE.SER_NO                               -- 成交序號
                   ,PURCHASE.PAY_TYPE                             -- 付款方式
                   ,PAY_TYPE.NAME AS PAY_TYPE_NAME                -- 付款方式名稱
                   ,CASE WHEN PURCHASE.PAY_TYPE IN ('4','7') THEN FAX_PURCHASE.AC_BANK
                         ELSE '' END AS AC_BANK                   -- 扣款總行代碼
                   ,CASE WHEN PURCHASE.PAY_TYPE IN ('4','7') THEN FAX_PURCHASE.AC_BRANCH
                         ELSE '' END AS AC_BRANCH                 -- 扣款分行代碼
                   ,CASE WHEN PURCHASE.PAY_TYPE IN ('4','7') THEN FIS_BRANCH.SNAME
                         ELSE '' END AS FIS_SNAME                 -- 扣款銀行簡稱
                   ,CASE WHEN PURCHASE.PAY_TYPE IN ('4','7') THEN FAX_PURCHASE.AC_CODE
                         ELSE '' END AS AC_CODE                   -- 扣款帳號
                   ,CASE WHEN PURCHASE.PAY_TYPE IN ('4','7') THEN ECS.FN_STUFF_STR(FAX_PURCHASE.AC_CODE,5,'*') 
                         ELSE '' END AS AC_CODE_5                 -- 往來帳號後5碼
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE           -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME             -- 配息頻率名稱
                   ,TO_DATE('1900/01/01','YYYY/MM/DD') AS RCV_DATE            -- 委託日期(含時分秒)
                   ,'' FEE_CHOICE_TYPE     -- 優惠方案代碼
                   ,'' FEE_CHOICE_NAME     -- 優惠方案名稱
                   ,'' PROJECT_CODE        -- 促銷活動代碼(臨櫃促銷活動與EC促銷活動取得方式不同)
                   ,'' PROJECT_NAME        -- 促銷活動名稱(臨櫃促銷活動與EC促銷活動取得方式不同)
                   ,FUND.FUND_CATEGORY
                   ,FUND.AMC_NO
                   ,FUND.SITCA_FUND_TYPE
                   ,FUND.ORDINAL
              FROM FAS.PURCHASE PURCHASE
              JOIN FAS.FUND
                ON PURCHASE.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON PURCHASE.CURRENCY_NO = CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              JOIN FAS.PAY_TYPE
                ON PAY_TYPE.PAY_TYPE = PURCHASE.PAY_TYPE  
              -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
              LEFT JOIN (SELECT * FROM NAV.NAV WHERE NAV_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO ) NAV  --限制特定區間以減少效能問題
                ON NAV.FUND_NO = FUND.FUND_NO
               AND NAV.NAV_DATE = PURCHASE.AC_DATE            
              LEFT JOIN FAS.FAX_PURCHASE
                ON FAX_PURCHASE.FUND_NO = PURCHASE.FUND_NO
               AND FAX_PURCHASE.TX_DATE = PURCHASE.TX_DATE 
               AND FAX_PURCHASE.BROKER_NO = PURCHASE.BROKER_NO
               AND FAX_PURCHASE.BRANCH_NO = PURCHASE.BRANCH_NO
               --2025.10.20 Patrick Chen. 加上Key值 
               AND FAX_PURCHASE.SER_NO = PURCHASE.SER_NO               
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              LEFT JOIN FAS.FIS_BRANCH FIS_BRANCH
                ON FIS_BRANCH.BANK_NO = FAX_PURCHASE.AC_BANK
               AND FIS_BRANCH.BRANCH_NO = FAX_PURCHASE.AC_BRANCH
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE PURCHASE.ID = XID
               -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'1' ) AND PURCHASE.BRANCH_NO NOT IN ('8000','8200','8300','8400','8800')) -- EC單筆：8000；Robo :8800
               -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
               -- 書面申購交易成功： FAS.PURCHASE.STATUS = 'S' or 'T'都為成功
               AND (TX_STATUS.TX_STATUS = 'S' AND PURCHASE.STATUS IN ('S','T')) 
               AND PURCHASE.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
           ) DataList
     -- 2018.10.25 tingting 1.增加傳出欄位：排列序號
     --                     2.調整排序：先依交易日期、交易狀態(處理中→交易成功→交易失敗)
     --                                再依基金排序規則(境內外、基金公司、投信顧公會基金類型、母基金排序、基金級別、子基金排序)
     ORDER BY DataList.TX_DATE,                         --交易日期
              CASE WHEN DataList.STATUS = 'C' THEN 1    --處理中
                   WHEN DataList.STATUS = 'S' THEN 2        --交易成功
                   WHEN DataList.STATUS = 'F' THEN 3       --交易失敗
                   ELSE 4 END,                          --交易狀態
              DataList.FUND_CATEGORY, DataList.AMC_NO, DataList.SITCA_FUND_TYPE, DataList.ORDINAL, DataList.SHARE_CLASS NULLS FIRST, DataList.ORDINAL;
    -- ============================================================== 單筆申購 END ==============================================================

    -- ============================================================== 單筆贖回 BNG ==============================================================
    OPEN REDEMDT FOR
    SELECT  ROWNUM AS ROW_NUM                                -- 排列序號
           ,DataList.*
           -- 2022.03.02 Chengyu 贖回與轉申購新增欄位"報酬率"
           ,RATE  --報酬率
      FROM (
            -- 贖回
            -- 2020.08.31 JIANXIN 將處理中的轉申購與贖回另外處理
            SELECT  REDEMPTION.FUND_NO                              -- 基金代碼
                   ,FUND.NAME AS FUND_NAME                          -- 基金名稱
                   ,FUND.FUND_CATEGORY                              -- 基金類別(境內/境外基金) 1:境內 2:境外
                   ,FUND.AMC_NO                                     -- 基金經理公司 1.refer to fas.AMC(已隱藏) 2.目前分類 01.瀚亞投信(境內) 02.瀚亞投資(新加坡) 03.MG(新加坡)    04.瑞萬通博(TDCC)
                   ,FUND.SITCA_FUND_TYPE                            -- 投信顧公會基金類型 1.refer to FAS.SITCA_FUND_TYPE 2.目前分類，請參閱 FAS.SITCA_FUND_TYPE
                   ,FUND.ORDINAL                                    -- 基金序數 - 用於查詢或報表的排序
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO                 -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                        -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                     -- 國際證券代碼
                   ,FUND.SHARE_CLASS                                -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME       -- 基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO                 -- 基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME             -- 基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL        -- 基金計價幣別小數位數
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY             -- 風險屬性
                   ,REDEMPTION.TX_DATE                              -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,CASE WHEN REDEMPTION.BRANCH_NO = '8000' THEN 'R1'
                         ELSE '' 
                     END AS PRODUCT_TYPE                            -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,CASE WHEN REDEMPTION.BRANCH_NO = '8000' THEN ECS.F_GET_PRODUCT_NAME('R1')
                         ELSE '' 
                     END AS PRODUCT_NAME                            -- 產品類別名稱
                   -- 2023.10.27 JIANXIN 若交易幣=MMA、FCY、NULL，則回傳RDEMEPTION的幣別
                   ,CASE WHEN RED_BANK.CURRENCY_NO IS NULL THEN REDEMPTION.CURRENCY_NO 
                         WHEN RED_BANK.CURRENCY_NO IN ('MMA','FCY') THEN REDEMPTION.CURRENCY_NO 
                         ELSE RED_BANK.CURRENCY_NO
                     END AS TRADE_CRNCY             -- 付款幣別
                   ,CURRENCY.NAME AS TRADE_CRNCY_NAME               -- 付款幣別名稱
                   ,CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL       -- 付款幣別小數位數
                   ,0 AS EX_RATE                                    -- 參考匯率(處理中，固定給 0 )
                   ,0 AS AMOUNT                                     -- 贖回價金(處理中，固定給 0 )
                   ,REDEMPTION.APPLICATION_SHARE AS SHARES          -- 贖回單位數
                   ,FUND.SHARE_DECIMAL                              -- 單位數小數位數
                   ,REDEMPTION.AC_DATE                              -- 淨值日期
                   ,0 AS NAV                                        -- 淨值 (處理中，固定給 0 )
                   ,FUND.NAV_DECIMAL                                -- 淨值小數位數
                   ,0 AS WIRE_FEE                                   -- 匯費(處理中匯費，固定給 0 )  
                   ,0  AS TOTAL_RETURN                              -- 贖回價金淨額 (處理中，固定給 0 )
                   ,'C' AS STATUS                                   -- 交易狀態代碼 (處理中，固定給 C)
                   ,'處理中' AS STATUS_NAME                         -- 交易狀態 (處理中，固定給 '處理中')
                   ,REDEMPTION.BROKER_NO                            -- 銷售機構代碼
                   ,REDEMPTION.BRANCH_NO                            -- 銷售機構分行代碼
                   ,REDEMPTION.SER_NO                               -- 成交序號
                   ,'3' AS PAY_TYPE                                 -- 付款方式
                   ,'匯款' AS PAY_TYPE_NAME                         -- 付款方式名稱
                   ,TO_DATE('1900/01/01','YYYY/MM/DD') AS PAY_DATE   -- 付款日期 (處理中 固定給 1900/01/01 )
                   ,REDEMPTION.AC_NAME                              -- 贖回匯款戶名
                   ,REDEMPTION.AC_FIS_SNAME                         -- 贖回匯款銀行名稱
                   ,REDEMPTION.AC_BANK                              -- 贖回匯款銀行代碼
                   ,REDEMPTION.AC_BRANCH                            -- 贖回匯款銀行分行碼
                   -- 2018.08.23 JIANXIN MOD BY 新增帳號隱碼函數
                   ,ECS.FN_STUFF_STR(REDEMPTION.AC_CODE,5,'*') AS AC_CODE_5 -- 往來帳號後5碼
                   -- 2019.04.17 tingting 因應需求編號"ES_20190315_01"，新增輸出欄位
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE             -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME             -- 配息頻率名稱
                   ,REDEMPTION_LOG.REVISION_DATE AS RCV_DATE        -- 委託日期(含時分秒)
                   -- 2022.03.02 Chengyu 贖回與轉申購新增欄位"報酬率"
                   ,0 RATE
              FROM ECS.REDEMPTION REDEMPTION
              JOIN FAS.FUND
                ON REDEMPTION.FUND_NO = FUND.FUND_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              -- 2021.05.10 Chengyu 修正贖回、轉申購時，FAS.REDEMPTION串接條件
              LEFT JOIN FAS.RED_BANK RED_BANK
                ON RED_BANK.TX_DATE = REDEMPTION.TX_DATE
               AND RED_BANK.FUND_NO = REDEMPTION.FUND_NO
               AND RED_BANK.FE_BROKER_NO = REDEMPTION.BROKER_NO
               AND RED_BANK.FE_BRANCH_NO = REDEMPTION.BRANCH_NO
               AND RED_BANK.FE_SER_NO = REDEMPTION.SER_NO
              -- 2023.10.27 JIANXIN 若交易幣=MMA、FCY、NULL，則回傳RDEMEPTION的幣別
              LEFT JOIN ECS.V_FUND_CRNCY CURRENCY
                ON CASE WHEN RED_BANK.CURRENCY_NO IS NULL THEN REDEMPTION.CURRENCY_NO 
                        WHEN RED_BANK.CURRENCY_NO IN ('MMA','FCY') THEN REDEMPTION.CURRENCY_NO 
                        ELSE RED_BANK.CURRENCY_NO
                    END = CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN (
                    SELECT  REDEMPTION_LOG.FUND_NO
                           ,REDEMPTION_LOG.ID
                           ,REDEMPTION_LOG.TX_DATE
                           ,REDEMPTION_LOG.BROKER_NO
                           ,REDEMPTION_LOG.BRANCH_NO
                           ,REDEMPTION_LOG.SER_NO
                           ,MAX(REDEMPTION_LOG.REVISION_DATE) AS REVISION_DATE 
                      FROM ECS.REDEMPTION_LOG       -- EC買回異動紀錄
                     WHERE REDEMPTION_LOG.ACTION_TYPE = 'A'     -- 交易動作(A:新增/M:修改/D:刪除)
                       AND REDEMPTION_LOG.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
                     GROUP BY REDEMPTION_LOG.FUND_NO
                           ,REDEMPTION_LOG.ID
                           ,REDEMPTION_LOG.TX_DATE
                           ,REDEMPTION_LOG.BROKER_NO
                           ,REDEMPTION_LOG.BRANCH_NO
                           ,REDEMPTION_LOG.SER_NO
                   ) REDEMPTION_LOG       -- EC買回異動紀錄
                ON REDEMPTION_LOG.FUND_NO = REDEMPTION.FUND_NO
               AND REDEMPTION_LOG.ID = REDEMPTION.ID
               AND REDEMPTION_LOG.TX_DATE = REDEMPTION.TX_DATE
               AND REDEMPTION_LOG.BROKER_NO = REDEMPTION.BROKER_NO
               AND REDEMPTION_LOG.BRANCH_NO = REDEMPTION.BRANCH_NO
               AND REDEMPTION_LOG.SER_NO = REDEMPTION.SER_NO
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE REDEMPTION.ID = XID
               -- 2018.11.01 JIANXIN 排除Robo交易
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'4' ) AND REDEMPTION.BRANCH_NO IN ('8000')) -- 單筆贖回：8000；ROBO贖回 :8800
               AND (TX_STATUS.TX_STATUS = 'C' AND REDEMPTION.STATUS = 'C') -- 交易成功：STATUS='P' ；處理中：STATUS='C'；
               AND REDEMPTION.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
            UNION
            SELECT  REDEMPTION.FUND_NO                              -- 基金代碼
                   ,FUND.NAME AS FUND_NAME                          -- 基金名稱
                   ,FUND.FUND_CATEGORY                              -- 基金類別(境內/境外基金) 1:境內 2:境外
                   ,FUND.AMC_NO                                     -- 基金經理公司 1.refer to fas.AMC(已隱藏) 2.目前分類 01.瀚亞投信(境內) 02.瀚亞投資(新加坡) 03.MG(新加坡)    04.瑞萬通博(TDCC)
                   ,FUND.SITCA_FUND_TYPE                            -- 投信顧公會基金類型 1.refer to FAS.SITCA_FUND_TYPE 2.目前分類，請參閱 FAS.SITCA_FUND_TYPE
                   ,FUND.ORDINAL                                    -- 基金序數 - 用於查詢或報表的排序
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO                 -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                        -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                     -- 國際證券代碼
                   ,FUND.SHARE_CLASS                                -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME       -- 基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO                 -- 基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME             -- 基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL        -- 基金計價幣別小數位數
                   -- 2018.10.09 ChengYu 增加傳出參數 風險屬性(RISK_CATEGORY)
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY             -- 風險屬性
                   ,REDEMPTION.TX_DATE                              -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,CASE WHEN REDEMPTION.BRANCH_NO = '8000' THEN 'R1'
                         ELSE '' 
                     END AS PRODUCT_TYPE                            -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,CASE WHEN REDEMPTION.BRANCH_NO = '8000' THEN ECS.F_GET_PRODUCT_NAME('R1')
                         ELSE '' 
                     END AS PRODUCT_NAME                            -- 產品類別名稱
                   -- 2018.12.05 JIANXIN 贖回交易幣別改為計價幣別
                   -- 2019.05.09 tingting 贖回交易幣別改回付款幣別
                   -- 2019.05.09 JIANXIN 贖回交易幣別改從買回付款(含轉申購)檔來、修正贖回價金錯誤
                   --,FUND.CURRENCY_NO AS TRADE_CRNCY                 -- 付款幣別
                   --,FUND_CURRENCY.NAME AS TRADE_CRNCY_NAME             -- 付款幣別名稱
                   --,FUND_CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL        -- 付款幣別小數位數
                   -- 2023.10.27 JIANXIN 若交易幣=MMA、FCY、NULL，則回傳RDEMEPTION的幣別
                   ,CASE WHEN RED_BANK.CURRENCY_NO IS NULL THEN REDEMPTION.CURRENCY_NO 
                         WHEN RED_BANK.CURRENCY_NO IN ('MMA','FCY') THEN REDEMPTION.CURRENCY_NO 
                         ELSE RED_BANK.CURRENCY_NO
                     END AS TRADE_CRNCY           -- 付款幣別
                   ,CURRENCY.NAME AS TRADE_CRNCY_NAME               -- 付款幣別名稱
                   ,CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL       -- 付款幣別小數位數
                   -- 2020.01.21 JIANXIN 匯率改由買進匯率取得
                   -- 2023.10.25 JIANXIN 調整匯率改由函數取得
                   ,CASE WHEN FUND.CURRENCY_NO = REDEMPTION.CURRENCY_NO THEN 1 
                         ELSE ECS.F_GET_EX_RATE(FAS_REDEMPTION.FUND_NO,FAS_REDEMPTION.EX_DATE,'B')
                     END AS EX_RATE                                 -- 參考匯率
                   -- 2018.10.05 JIANXIN MOD BY 歷史贖回交易顯示處理
                   -- 2018.12.05 JIANXIN 調整贖回金額、贖回淨額等欄位取得方式
                   -- 2019.05.09 JIANXIN 贖回交易幣別改從買回付款(含轉申購)檔來、修正贖回價金錯誤
                   -- 2020.01.21 JIANXIN 贖回金額顯示方式為交易幣別與計價幣別不同時，顯示不同的金額欄位                   
                   ,CASE WHEN FUND.CURRENCY_NO = REDEMPTION.CURRENCY_NO THEN RED_BANK.AMOUNT 
                         ELSE RED_BANK.NTD_AMOUNT
                     END AS AMOUNT                -- 贖回價金
                   ,REDEMPTION.APPLICATION_SHARE AS SHARES          -- 贖回單位數
                   ,FUND.SHARE_DECIMAL                              -- 單位數小數位數
                   ,REDEMPTION.AC_DATE                              -- 淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN REDEMPTION.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV.NAV END NAV                       -- 淨值 
                   ,FUND.NAV_DECIMAL                                -- 淨值小數位數
                   -- 2021.05.10 Chengyu 修正贖回、轉申購時，FAS.REDEMPTION串接條件
                   ,NVL(RED_BANK.WIRE_FEE,0) AS WIRE_FEE                                   -- 匯費  
                   -- 2018.12.05 JIANXIN 調整贖回金額、贖回淨額等欄位取得方式
                   ,NVL(RED_BANK.AMOUNT - RED_BANK.WIRE_FEE  ,0) AS TOTAL_RETURN                               -- 贖回價金淨額 
                   ,CASE WHEN REDEMPTION.STATUS = 'C' THEN 'C'
                         WHEN REDEMPTION.STATUS = 'P' AND REDEMPTION.RESULT_CODE = '00' THEN 'S'
                         WHEN REDEMPTION.STATUS = 'P' AND REDEMPTION.RESULT_CODE <> '00' THEN 'F'
                         ELSE '' 
                     END AS STATUS                                 -- 交易狀態代碼
                   ,CASE WHEN REDEMPTION.STATUS = 'C' THEN '處理中'
                         WHEN REDEMPTION.STATUS = 'P' AND REDEMPTION.RESULT_CODE = '00' THEN '交易成功'
                         WHEN REDEMPTION.STATUS = 'P' AND REDEMPTION.RESULT_CODE <> '00' THEN '交易失敗'
                         ELSE '' 
                     END AS STATUS_NAME                             -- 交易狀態
                   ,REDEMPTION.BROKER_NO                            -- 銷售機構代碼
                   ,REDEMPTION.BRANCH_NO                            -- 銷售機構分行代碼
                   ,REDEMPTION.SER_NO                               -- 成交序號
                   ,'3' AS PAY_TYPE                                 -- 付款方式
                   ,'匯款' AS PAY_TYPE_NAME                         -- 付款方式名稱
                   -- 2018.12.27 JIANXIN 若付款日期無資料，則回傳1900/01/01 
                   -- 2019.02.14 yunchu 調整欄位名稱買回預計入款日(PAY_DATE) 
                   -- 2019.02.15 yunchu 調整買回預計入款日字串轉日期 
                   -- 2019.02.21 JIANXIN 若付款日期無資料，則回傳0001/01/01 (暫時解決線上問題) 
                   -- 2019.03.11 JIANXIN 若付款日期無資料，則回傳1900/01/01 
                   ,TO_DATE(NVL(TO_CHAR(REDEMPTION.PAY_DATE,'YYYY/MM/DD'),'1900/01/01'),'YYYY/MM/DD') AS PAY_DATE   -- 付款日期
                   ,REDEMPTION.AC_NAME                              -- 贖回匯款戶名
                   ,REDEMPTION.AC_FIS_SNAME                         -- 贖回匯款銀行名稱
                   ,REDEMPTION.AC_BANK                              -- 贖回匯款銀行代碼
                   ,REDEMPTION.AC_BRANCH                            -- 贖回匯款銀行分行碼
                   -- 2018.08.23 JIANXIN MOD BY 新增帳號隱碼函數
                   ,ECS.FN_STUFF_STR(REDEMPTION.AC_CODE,5,'*') AS AC_CODE_5 -- 往來帳號後5碼
                   -- 2019.04.17 tingting 因應需求編號"ES_20190315_01"，新增輸出欄位
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE             -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME             -- 配息頻率名稱
                   ,REDEMPTION_LOG.REVISION_DATE AS RCV_DATE        -- 委託日期(含時分秒)
                   -- 2022.03.02 Chengyu 贖回與轉申購新增欄位"報酬率"
                   -- 2022.04.14 JIANXIN 調整報酬率小數點2位
                   -- 2022.05.24 JIANXIN 調整報酬率計算方式，若遇到配息再申購或者是瑞萬系列的基金，則以 CERTIFICATE.AMOUNT 作為成本，否則以 CERTIFICATE.COST 作為成本
                   -- 2022.10.19 JIANXIN 調整報酬率計算方式
                   ,(SELECT ROUND((FAS_REDEMPTION.AMOUNT / SUM((CASE WHEN CERTIFICATE.BRANCH_NO ='7000' OR CERTIFICATE.FUND_NO LIKE 'V%' THEN CERTIFICATE.AMOUNT ELSE CERTIFICATE.COST END * RED_CERTIFICATE.REDEMPTION_SHARE / RED_CERTIFICATE.CERTIFICATE_SHARE)) - 1)*100 ,2)
                       FROM FAS.CERTIFICATE,
                            FAS.RED_CERTIFICATE
                      WHERE FAS.CERTIFICATE.FUND_NO = FAS.RED_CERTIFICATE.FUND_NO
                        AND FAS.CERTIFICATE.CERTIFICATE_NO = FAS.RED_CERTIFICATE.CERTIFICATE_NO
                        AND FAS_REDEMPTION.FUND_NO = FAS.RED_CERTIFICATE.FUND_NO
                        AND FAS_REDEMPTION.TX_DATE = FAS.RED_CERTIFICATE.TX_DATE
                        AND FAS_REDEMPTION.BROKER_NO = FAS.RED_CERTIFICATE.BROKER_NO
                        AND FAS_REDEMPTION.BRANCH_NO = FAS.RED_CERTIFICATE.BRANCH_NO
                        AND FAS_REDEMPTION.SER_NO = FAS.RED_CERTIFICATE.SER_NO
                    ) RATE
              FROM ECS.REDEMPTION REDEMPTION
              JOIN FAS.FUND
                ON REDEMPTION.FUND_NO = FUND.FUND_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               -- 2018.10.08 JIANXIN MOD BY 處理基金級別串接問題
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
               -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
               -- 2020.04.28 Chengyu 調整贖回與轉申購效能
               -- 2020.12.15 Chengyu 調整淨值資料限制條件為NAV_DATE
               -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
              LEFT JOIN (SELECT * FROM NAV.NAV WHERE NAV_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) NAV
                ON NAV.FUND_NO = FUND.FUND_NO
               AND NAV.NAV_DATE = REDEMPTION.AC_DATE
              -- 2021.05.10 Chengyu 修正贖回、轉申購時，FAS.REDEMPTION串接條件
              LEFT JOIN FAS.RED_BANK RED_BANK
                ON RED_BANK.TX_DATE = REDEMPTION.TX_DATE
               AND RED_BANK.FUND_NO = REDEMPTION.FUND_NO
               -- 2018.12.12 JIANXIN MOD BY 調整贖回串接FE_BROKER_NO 
               AND RED_BANK.FE_BROKER_NO = REDEMPTION.BROKER_NO 
               AND RED_BANK.FE_BRANCH_NO = REDEMPTION.BRANCH_NO
               AND RED_BANK.FE_SER_NO = REDEMPTION.SER_NO
              LEFT JOIN FAS.REDEMPTION FAS_REDEMPTION
                ON FAS_REDEMPTION.ID = REDEMPTION.ID 
               AND FAS_REDEMPTION.TX_DATE = RED_BANK.TX_DATE 
               AND FAS_REDEMPTION.FUND_NO = RED_BANK.FUND_NO  
               AND FAS_REDEMPTION.BROKER_NO = RED_BANK.BROKER_NO
               AND FAS_REDEMPTION.BRANCH_NO = RED_BANK.BRANCH_NO
               AND FAS_REDEMPTION.SER_NO = RED_BANK.SER_NO    
                -- 2020.04.28 Chengyu 調整贖回與轉申購效能
              --LEFT JOIN (SELECT * FROM FAS.V_CURRENCY_RATE WHERE TX_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) V_CURRENCY_RATE
              --  ON V_CURRENCY_RATE.FUND_NO = FAS_REDEMPTION.FUND_NO
              -- AND V_CURRENCY_RATE.TX_DATE = FAS_REDEMPTION.EX_DATE
              -- 2018.10.05 JIANXIN MOD BY 歷史贖回交易顯示處理
              -- 2023.10.27 JIANXIN 若交易幣=MMA、FCY、NULL，則回傳RDEMEPTION的幣別
              LEFT JOIN ECS.V_FUND_CRNCY CURRENCY
                ON CASE WHEN RED_BANK.CURRENCY_NO IS NULL THEN REDEMPTION.CURRENCY_NO 
                        WHEN RED_BANK.CURRENCY_NO IN ('MMA','FCY') THEN REDEMPTION.CURRENCY_NO 
                        ELSE RED_BANK.CURRENCY_NO
                    END = CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN (
                    SELECT  REDEMPTION_LOG.FUND_NO
                           ,REDEMPTION_LOG.ID
                           ,REDEMPTION_LOG.TX_DATE
                           ,REDEMPTION_LOG.BROKER_NO
                           ,REDEMPTION_LOG.BRANCH_NO
                           ,REDEMPTION_LOG.SER_NO
                           ,MAX(REDEMPTION_LOG.REVISION_DATE) AS REVISION_DATE 
                      FROM ECS.REDEMPTION_LOG       -- EC買回異動紀錄
                     WHERE REDEMPTION_LOG.ACTION_TYPE = 'A'     -- 交易動作(A:新增/M:修改/D:刪除)
                       AND REDEMPTION_LOG.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
                     GROUP BY REDEMPTION_LOG.FUND_NO
                           ,REDEMPTION_LOG.ID
                           ,REDEMPTION_LOG.TX_DATE
                           ,REDEMPTION_LOG.BROKER_NO
                           ,REDEMPTION_LOG.BRANCH_NO
                           ,REDEMPTION_LOG.SER_NO
                   ) REDEMPTION_LOG       -- EC買回異動紀錄
                ON REDEMPTION_LOG.FUND_NO = REDEMPTION.FUND_NO
               AND REDEMPTION_LOG.ID = REDEMPTION.ID
               AND REDEMPTION_LOG.TX_DATE = REDEMPTION.TX_DATE
               AND REDEMPTION_LOG.BROKER_NO = REDEMPTION.BROKER_NO
               AND REDEMPTION_LOG.BRANCH_NO = REDEMPTION.BRANCH_NO
               AND REDEMPTION_LOG.SER_NO = REDEMPTION.SER_NO
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE REDEMPTION.ID = XID
               -- 2018.11.01 JIANXIN 排除Robo交易
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'4' ) AND REDEMPTION.BRANCH_NO IN ('8000')) -- 單筆贖回：8000；ROBO贖回 :8800
               AND (TX_STATUS.TX_STATUS = 'S' AND REDEMPTION.STATUS = 'P') -- 交易成功：STATUS='P' ；處理中：STATUS='C'；
               AND REDEMPTION.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
             UNION 
             -- 2021.06.24 Chengyu 新增書面資料
             SELECT  REDEMPTION.FUND_NO                              -- 基金代碼
                   ,FUND.NAME AS FUND_NAME                          -- 基金名稱
                   ,FUND.FUND_CATEGORY                              -- 基金類別(境內/境外基金) 1:境內 2:境外
                   ,FUND.AMC_NO                                     -- 基金經理公司 1.refer to fas.AMC(已隱藏) 2.目前分類 01.瀚亞投信(境內) 02.瀚亞投資(新加坡) 03.MG(新加坡)    04.瑞萬通博(TDCC)
                   ,FUND.SITCA_FUND_TYPE                            -- 投信顧公會基金類型 1.refer to FAS.SITCA_FUND_TYPE 2.目前分類，請參閱 FAS.SITCA_FUND_TYPE
                   ,FUND.ORDINAL                                    -- 基金序數 - 用於查詢或報表的排序
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO                 -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                        -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                     -- 國際證券代碼
                   ,FUND.SHARE_CLASS                                -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME       -- 基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO                 -- 基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME             -- 基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL        -- 基金計價幣別小數位數
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY             -- 風險屬性
                   ,REDEMPTION.TX_DATE                              -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,REDEMPTION.PRODUCT_TYPE  AS PRODUCT_TYPE                            -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,ECS.F_GET_PRODUCT_NAME(REDEMPTION.PRODUCT_TYPE) AS PRODUCT_NAME                            -- 產品類別名稱
                   -- 2023.10.27 JIANXIN 若交易幣=MMA、FCY、NULL，則回傳RDEMEPTION的幣別
                   ,CASE WHEN RED_BANK.CURRENCY_NO IS NULL THEN REDEMPTION.CURRENCY_NO 
                         WHEN RED_BANK.CURRENCY_NO IN ('MMA','FCY') THEN REDEMPTION.CURRENCY_NO 
                         ELSE RED_BANK.CURRENCY_NO
                    END AS TRADE_CRNCY           -- 付款幣別
                   ,CURRENCY.NAME AS TRADE_CRNCY_NAME               -- 付款幣別名稱
                   ,CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL       -- 付款幣別小數位數
                   -- 2023.10.25 JIANXIN 調整匯率改由函數取得
                   ,CASE WHEN FUND.CURRENCY_NO = REDEMPTION.CURRENCY_NO THEN 1 
                         ELSE ECS.F_GET_EX_RATE(REDEMPTION.FUND_NO,REDEMPTION.EX_DATE,'B') 
                     END AS EX_RATE                                 -- 參考匯率
                   ,CASE WHEN FUND.CURRENCY_NO = REDEMPTION.CURRENCY_NO THEN RED_BANK.AMOUNT 
                         ELSE RED_BANK.NTD_AMOUNT
                     END AS AMOUNT                -- 贖回價金
                   ,REDEMPTION.APPLICATION_SHARE AS SHARES          -- 贖回單位數
                   ,FUND.SHARE_DECIMAL                              -- 單位數小數位數
                   ,REDEMPTION.AC_DATE                              -- 淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN REDEMPTION.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV.NAV END NAV                       -- 淨值 
                   ,FUND.NAV_DECIMAL                                -- 淨值小數位數
                   ,NVL(RED_BANK.WIRE_FEE,0) AS WIRE_FEE                                   -- 匯費  
                   ,NVL(RED_BANK.AMOUNT - RED_BANK.WIRE_FEE  ,0) AS TOTAL_RETURN           -- 贖回價金淨額   
                   -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)                 
                   ,'S' AS STATUS                                   -- 交易狀態代碼
                   ,'交易成功' AS STATUS_NAME                       -- 交易狀態
                   ,REDEMPTION.BROKER_NO                            -- 銷售機構代碼
                   ,REDEMPTION.BRANCH_NO                            -- 銷售機構分行代碼
                   ,REDEMPTION.SER_NO                               -- 成交序號
                   ,'3' AS PAY_TYPE                                 -- 付款方式
                   ,'匯款' AS PAY_TYPE_NAME                         -- 付款方式名稱
                   ,TO_DATE(NVL(TO_CHAR(REDEMPTION.PAY_DATE,'YYYY/MM/DD'),'1900/01/01'),'YYYY/MM/DD') AS PAY_DATE   -- 付款日期
                   ,RED_BANK.AC_NAME                              -- 贖回匯款戶名
                   ,RED_BANK.AC_FIS_SNAME                         -- 贖回匯款銀行名稱                   
                   ,RED_BANK.AC_BANK                              -- 贖回匯款銀行代碼
                   ,RED_BANK.AC_BRANCH                            -- 贖回匯款銀行分行碼
                   ,ECS.FN_STUFF_STR(RED_BANK.AC_CODE,5,'*') AS AC_CODE_5 -- 往來帳號後5碼
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE           -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME             -- 配息頻率名稱
                   ,TO_DATE('1900/01/01','YYYY/MM/DD') AS RCV_DATE          -- 委託日期(含時分秒)
                   -- 2022.03.02 Chengyu 贖回與轉申購新增欄位"報酬率"
                   -- 2022.04.14 JIANXIN 調整報酬率小數點2位
                   -- 2022.05.24 JIANXIN 調整報酬率計算方式，若遇到配息再申購或者是瑞萬系列的基金，則以 CERTIFICATE.AMOUNT 作為成本，否則以 CERTIFICATE.COST 作為成本
                   -- 2022.10.19 JIANXIN 調整報酬率計算方式
                   ,(SELECT ROUND((REDEMPTION.AMOUNT / SUM((CASE WHEN CERTIFICATE.BRANCH_NO ='7000' OR CERTIFICATE.FUND_NO LIKE 'V%' THEN CERTIFICATE.AMOUNT ELSE CERTIFICATE.COST END * RED_CERTIFICATE.REDEMPTION_SHARE / RED_CERTIFICATE.CERTIFICATE_SHARE)) - 1)*100 ,2)
                       FROM FAS.CERTIFICATE,
                            FAS.RED_CERTIFICATE
                      WHERE FAS.CERTIFICATE.FUND_NO = FAS.RED_CERTIFICATE.FUND_NO
                        AND FAS.CERTIFICATE.CERTIFICATE_NO = FAS.RED_CERTIFICATE.CERTIFICATE_NO
                        AND REDEMPTION.FUND_NO = FAS.RED_CERTIFICATE.FUND_NO
                        AND REDEMPTION.TX_DATE = FAS.RED_CERTIFICATE.TX_DATE
                        AND REDEMPTION.BROKER_NO = FAS.RED_CERTIFICATE.BROKER_NO
                        AND REDEMPTION.BRANCH_NO = FAS.RED_CERTIFICATE.BRANCH_NO
                        AND REDEMPTION.SER_NO = FAS.RED_CERTIFICATE.SER_NO
                    ) RATE
              FROM FAS.REDEMPTION REDEMPTION
              JOIN FAS.FUND
                ON REDEMPTION.FUND_NO = FUND.FUND_NO              
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
              LEFT JOIN (SELECT * FROM NAV.NAV WHERE NAV_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) NAV
                ON NAV.FUND_NO = FUND.FUND_NO
               AND NAV.NAV_DATE = REDEMPTION.AC_DATE
              LEFT JOIN FAS.RED_BANK RED_BANK
                ON RED_BANK.TX_DATE = REDEMPTION.TX_DATE
               AND RED_BANK.FUND_NO = REDEMPTION.FUND_NO
               AND RED_BANK.BROKER_NO = REDEMPTION.BROKER_NO
               AND RED_BANK.BRANCH_NO = REDEMPTION.BRANCH_NO
               AND RED_BANK.SER_NO = REDEMPTION.SER_NO
              --LEFT JOIN (SELECT * FROM FAS.V_CURRENCY_RATE WHERE TX_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) V_CURRENCY_RATE
              --  ON V_CURRENCY_RATE.FUND_NO = REDEMPTION.FUND_NO
              -- AND V_CURRENCY_RATE.TX_DATE = REDEMPTION.EX_DATE
              -- 2023.10.27 JIANXIN 若交易幣=MMA、FCY、NULL，則回傳RDEMEPTION的幣別
              LEFT JOIN ECS.V_FUND_CRNCY CURRENCY
                ON CASE WHEN RED_BANK.CURRENCY_NO IS NULL THEN REDEMPTION.CURRENCY_NO 
                        WHEN RED_BANK.CURRENCY_NO IN ('MMA','FCY') THEN REDEMPTION.CURRENCY_NO 
                        ELSE RED_BANK.CURRENCY_NO
                    END = CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE REDEMPTION.ID = XID
               -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'4' ) AND REDEMPTION.BRANCH_NO NOT IN ('8000','8200','8300','8400','8800')) -- 單筆贖回：8000；ROBO贖回 :8800
               -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
               -- 書面交易成功： FAS.REDEMPTION.STATUS = 'S' or 'T'
               AND (TX_STATUS.TX_STATUS = 'S' AND REDEMPTION.STATUS IN  ('S','T')) 
               AND REDEMPTION.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
               --無轉申購日期為贖回資料
               AND REDEMPTION.ST_DATE IS NULL
           ) DataList
           -- 2018.10.25 tingting 1.增加傳出欄位：排列序號
           --                     2.調整排序：先依交易日期、交易狀態(處理中→交易成功→交易失敗)
           --                                再依基金排序規則(境內外、基金公司、投信顧公會基金類型、母基金排序、基金級別、子基金排序)
           ORDER BY TX_DATE,                          --交易日期
                      CASE WHEN STATUS = 'C' THEN 1     --處理中
                           WHEN STATUS = 'S' THEN 2     --交易成功
                           WHEN STATUS = 'F' THEN 3     --交易失敗
                           ELSE 4 END,                  --交易狀態
                      FUND_CATEGORY, AMC_NO, SITCA_FUND_TYPE, ORDINAL, SHARE_CLASS NULLS FIRST, ORDINAL; 
    -- ============================================================== 單筆贖回 END ==============================================================

    -- =============================================================== 轉申購 BNG ===============================================================
    OPEN SWITCHDT FOR
    SELECT  ROWNUM AS ROW_NUM                                                            -- 排列序號
           ,DataList.*
           -- 2022.03.02 Chengyu 贖回與轉申購新增欄位"報酬率"
           ,RATE  --034340161            
      FROM (
            -- 轉換
            -- 2020.08.31 JIANXIN 將處理中的轉申購與贖回另外處理
            SELECT  SWITCH.FUND_NO AS FUND_NO_OUT                                      -- 轉出基金代碼
                   ,FUND.NAME AS FUND_NAME_OUT                                         -- 轉出基金名稱
                   ,FUND.FUND_CATEGORY                                                 -- 轉出基金類別(境內/境外基金) 1:境內 2:境外
                   ,FUND.AMC_NO                                                        -- 轉出基金經理公司 1.refer to fas.AMC(已隱藏) 2.目前分類 01.瀚亞投信(境內) 02.瀚亞投資(新加坡) 03.MG(新加坡)    04.瑞萬通博(TDCC)
                   ,FUND.SITCA_FUND_TYPE                                               -- 轉出基金投信顧公會基金類型 1.refer to FAS.SITCA_FUND_TYPE 2.目前分類，請參閱 FAS.SITCA_FUND_TYPE
                   ,FUND.ORDINAL                                                       -- 轉出基金序數 - 用於查詢或報表的排序
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO_OUT                                -- 轉出基金警語
                   ,FUND.SNAME AS FUND_SNAME_OUT                                       -- 轉出基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE_OUT                                    -- 轉出基金國際證券代碼
                   ,FUND.SHARE_CLASS AS SHARE_CLASS_OUT                                -- 轉出基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME_OUT                      -- 轉出基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO_OUT                                -- 轉出基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME_OUT                            -- 轉出基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL_OUT                       -- 轉出基金計價幣別小數位數
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY_OUT                            -- 轉出基金風險屬性 
                   ,SWITCH.TX_DATE                                                     -- 交易日期
                   ,'' AS PRODUCT_TYPE                                                 -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,'轉申購' AS PRODUCT_NAME                                           -- 產品類別名稱
                   ,FUND_CURRENCY.CURRENCY_NO AS TRADE_CRNCY_OUT                       -- 轉出基金申購幣別
                   ,FUND_CURRENCY.NAME AS TRADE_CRNCY_NAME_OUT                         -- 轉出基金申購幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL_OUT                 -- 轉出基金申購幣別小數位數
                   ,0 AS EX_RATE_OUT                                                   -- 轉出基金參考匯率 (處理中，固定為 0)
                   ,0 AS AMOUNT_OUT                                                    -- 贖回價金 (處理中，固定為 0)
                   ,SWITCH.APPLICATION_SHARE AS SHARES_OUT                             -- 贖回單位數
                   ,FUND.SHARE_DECIMAL AS SHARE_DECIMAL_OUT                            -- 轉出基金單位數小數位數
                   ,SWITCH.AC_DATE AS AC_DATE_OUT                                      -- 轉出基金淨值日期
                   ,0 AS NAV_OUT                                                       -- 轉出基金淨值 (處理中，固定為 0)
                   ,FUND.NAV_DECIMAL AS NAV_DECIMAL_OUT                                -- 轉出基金淨值小數位數
                   ,'C' AS STATUS                                                      -- 交易狀態代碼 (處理中，固定為 C)
                   ,'處理中' AS STATUS_NAME                                            -- 交易狀態 (處理中，固定為 '處理中')
                   ,SWITCH.BROKER_NO                                                   -- 銷售機構代碼
                   ,SWITCH.BRANCH_NO                                                   -- 銷售機構分行代碼
                   ,SWITCH.SER_NO                                                      -- 成交序號
                   ,SWITCH.FUND_NO_IN AS FUND_NO_IN                                    -- 轉入基金代碼
                   ,FUND_IN.NAME AS FUND_NAME_IN                                       -- 轉入基金名稱
                   ,FUND_IN.DIVIDEND_DESC AS FUND_MEMO_IN                              -- 轉入基金警語
                   ,FUND_IN.SNAME AS FUND_SNAME_IN                                     -- 轉入基金簡稱
                   ,FUND_IN.ISIN_CODE AS ISIN_CODE_IN                                  -- 轉入基金國際證券代碼
                   ,FUND_IN.SHARE_CLASS AS SHARE_CLASS_IN                              -- 轉入基金級別
                   ,FUND_IN.NAME AS SHARE_CLASS_NAME_IN                                -- 轉入基金級別名稱
                   ,FUND_IN.CURRENCY_NO AS CURRENCY_NO_IN                              -- 轉入基金計價幣別
                   ,FUND_CURRENCY_IN.NAME AS CURRENCY_NAME_IN                          -- 轉入基金計價幣別名稱
                   ,FUND_CURRENCY_IN.AMT_DECIMAL AS AMT_DECIMAL_IN                     -- 轉入基金計價幣別小數位數
                   ,FUND_IN.RISK_CATEGORY AS RISK_CATEGORY_IN                          -- 轉入基金風險屬性
                   ,SWITCH.SWITCH_DATE AS SWITCH_DATE                                  -- 轉申購日期
                   ,FUND_CURRENCY_IN.CURRENCY_NO AS TRADE_CRNCY_IN                     -- 轉入基金申購幣別
                   ,FUND_CURRENCY_IN.NAME AS TRADE_CRNCY_NAME_IN                       -- 轉入基金申購幣別名稱
                   ,FUND_CURRENCY_IN.AMT_DECIMAL AS TRADE_AMT_DECIMAL_IN               -- 轉入基金申購幣別小數位數
                   ,0 AS EX_RATE_IN                                                    -- 轉入基金參考匯率(處理中，固定為 0)
                   ,0 AS AMOUNT_IN                                                     -- 轉申購金額(轉入基金淨額)(處理中，固定為 0)
                   ,0 AS SERVICE_CHARGE_IN                                             -- 轉申購手續費(處理中，固定為 0)
                   ,0 AS SHARES_IN                                                     -- 轉申購單位數(處理中，固定為 0)
                   ,FUND_IN.SHARE_DECIMAL AS SHARE_DECIMAL_IN                          -- 轉入基金單位數小數位數
                   ,FAS_PURCHASE.AC_DATE AS AC_DATE_IN                                 -- 轉入基金淨值日期
                   ,0 AS NAV_IN                                                        -- 轉入基金淨值(處理中，固定為 0) 
                   ,FUND_IN.NAV_DECIMAL AS NAV_DECIMAL_IN                              -- 轉入基金淨值小數位數
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND_OUT     -- 轉出基金是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE_OUT                                         -- 轉出基金配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ_OUT.DISPLAY_NAME AS DIVIDEND_TYPE_NAME_OUT             -- 轉出基金配息頻率名稱
                   ,CASE WHEN FUND_IN.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND_IN   -- 轉入基金是否為配息
                   ,FUND_IN.DIVIDEND_FREQ AS DIVIDEND_TYPE_IN                                       -- 轉入基金配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ_IN.DISPLAY_NAME AS DIVIDEND_TYPE_NAME_IN               -- 轉入基金配息頻率名稱
                   ,REDEMPTION_LOG.REVISION_DATE AS RCV_DATE                           -- 委託日期(含時分秒)
                   ,0 AS WIRE_FEE                                                      -- 郵匯費(處理中，固定為 0) 
                   -- 2021.05.10 Chengyu 修正贖回、轉申購時，FAS.REDEMPTION串接條件
                   ,ROUND(RED_BANK.AMOUNT,FUND_CURRENCY.AMT_DECIMAL) - ROUND(RED_BANK.WIRE_FEE,FUND_CURRENCY.AMT_DECIMAL) AS NET_AMOUNT_OUT   -- 轉出基金淨額(贖回價金 - 郵匯費)
                   ,FAS_SWITCH.EX_DATE                                                 -- 匯率日期
                   ,0 AS EX_RATE                                                       -- 匯率-交叉匯率(處理中，固定為 0) 
                   ,SWITCH.FEE_CHOICE AS FEE_CHOICE_TYPE                               -- 優惠方案代碼
                   ,CASE WHEN SWITCH.FEE_CHOICE = '1' THEN CAMPAIGN.CAMPAIGN_SHNM
                         WHEN SWITCH.FEE_CHOICE = '2' THEN '優惠券' || SWITCH.COUPON_ID
                         WHEN SWITCH.FEE_CHOICE = '3' THEN '紅利折抵' || SWITCH.USED_POINT || '點'
                         WHEN SWITCH.FEE_CHOICE = '5' THEN CAMPAIGN.CAMPAIGN_SHNM
                         WHEN SWITCH.FEE_CHOICE IS NULL THEN '網路費率'
                         END AS FEE_CHOICE_NAME                                        -- 優惠方案名稱
                   ,SWITCH.PROJECT_CODE                                                -- 促銷活動代碼
                   ,CAMPAIGN.CAMPAIGN_SHNM AS PROJECT_NAME                             -- 促銷活動名稱
                   -- 2022.03.02 Chengyu 贖回與轉申購新增欄位"報酬率"
                   ,0 RATE
              FROM ECS.REDEMPTION SWITCH    
              JOIN FAS.FUND
                ON SWITCH.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON SWITCH.CURRENCY_NO = CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              JOIN FAS.FUND FUND_IN
                ON SWITCH.FUND_NO_IN = FUND_IN.FUND_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY_IN
                ON FUND_IN.CURRENCY_NO = FUND_CURRENCY_IN.CURRENCY_NO      
              LEFT JOIN FAS.PURCHASE FAS_PURCHASE
                ON FAS_PURCHASE.FUND_NO_OUT = SWITCH.FUND_NO
               AND FAS_PURCHASE.TX_DATE_OUT = SWITCH.TX_DATE
               AND FAS_PURCHASE.BROKER_NO = SWITCH.BROKER_NO
               AND FAS_PURCHASE.BRANCH_NO = SWITCH.BRANCH_NO
               AND FAS_PURCHASE.SER_NO_OUT = SWITCH.SER_NO
               AND FAS_PURCHASE.FUND_NO = SWITCH.FUND_NO_IN     
              LEFT JOIN FAS.RED_BANK RED_BANK
                ON RED_BANK.FUND_NO = FAS_PURCHASE.FUND_NO_OUT
               AND RED_BANK.TX_DATE = FAS_PURCHASE.TX_DATE_OUT
               AND RED_BANK.FE_BROKER_NO = '999'
               AND RED_BANK.FE_BRANCH_NO = '8300'
               AND RED_BANK.FE_SER_NO = FAS_PURCHASE.SER_NO_OUT
               AND RED_BANK.FUND_NO_IN = FAS_PURCHASE.FUND_NO
               AND RED_BANK.AC_BRANCH = SWITCH.AC_BRANCH
               AND RED_BANK.AC_BANK = SWITCH.AC_BANK
               AND RED_BANK.AC_CODE = SWITCH.AC_CODE
              -- 2021.05.10 Chengyu 修正贖回、轉申購時，FAS.REDEMPTION串接條件
              LEFT JOIN FAS.REDEMPTION FAS_SWITCH
                ON FAS_SWITCH.FUND_NO = RED_BANK.FUND_NO
               AND FAS_SWITCH.TX_DATE = RED_BANK.TX_DATE
               AND FAS_SWITCH.BROKER_NO = RED_BANK.BROKER_NO
               AND FAS_SWITCH.BRANCH_NO = RED_BANK.BRANCH_NO
               AND FAS_SWITCH.SER_NO = RED_BANK.SER_NO
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ_OUT -- 欄位選單資料(轉出基金配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ_OUT.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ_OUT.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ_IN  -- 欄位選單資料(轉入基金配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ_IN.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ_IN.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN (
                    SELECT  REDEMPTION_LOG.FUND_NO
                           ,REDEMPTION_LOG.ID
                           ,REDEMPTION_LOG.TX_DATE
                           ,REDEMPTION_LOG.BROKER_NO
                           ,REDEMPTION_LOG.BRANCH_NO
                           ,REDEMPTION_LOG.SER_NO
                           ,MAX(REDEMPTION_LOG.REVISION_DATE) AS REVISION_DATE 
                      FROM ECS.REDEMPTION_LOG       -- EC買回異動紀錄
                     WHERE REDEMPTION_LOG.ACTION_TYPE = 'A'     -- 交易動作(A:新增/M:修改/D:刪除)
                       AND REDEMPTION_LOG.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
                     GROUP BY REDEMPTION_LOG.FUND_NO
                           ,REDEMPTION_LOG.ID
                           ,REDEMPTION_LOG.TX_DATE
                           ,REDEMPTION_LOG.BROKER_NO
                           ,REDEMPTION_LOG.BRANCH_NO
                           ,REDEMPTION_LOG.SER_NO
                   ) REDEMPTION_LOG       -- EC買回異動紀錄
                ON REDEMPTION_LOG.FUND_NO = SWITCH.FUND_NO
               AND REDEMPTION_LOG.ID = SWITCH.ID
               AND REDEMPTION_LOG.TX_DATE = SWITCH.TX_DATE
               AND REDEMPTION_LOG.BROKER_NO = SWITCH.BROKER_NO
               AND REDEMPTION_LOG.BRANCH_NO = SWITCH.BRANCH_NO
               AND REDEMPTION_LOG.SER_NO = SWITCH.SER_NO
              LEFT JOIN ECS.CAMPAIGN    -- 促銷活動
                ON CAMPAIGN.CAMPAIGN_CODE = SWITCH.PROJECT_CODE
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE SWITCH.ID = XID
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'5' ) AND SWITCH.BRANCH_NO IN ('8300')) -- 轉申購：8300；
               AND (TX_STATUS.TX_STATUS = 'C' AND SWITCH.STATUS = 'C') -- 交易成功：STATUS='P' ；處理中：STATUS='C'；
               AND SWITCH.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
            UNION
            SELECT  SWITCH.FUND_NO AS FUND_NO_OUT                                      -- 轉出基金代碼
                   ,FUND.NAME AS FUND_NAME_OUT                                         -- 轉出基金名稱
                   ,FUND.FUND_CATEGORY                                                 -- 轉出基金類別(境內/境外基金) 1:境內 2:境外
                   ,FUND.AMC_NO                                                        -- 轉出基金經理公司 1.refer to fas.AMC(已隱藏) 2.目前分類 01.瀚亞投信(境內) 02.瀚亞投資(新加坡) 03.MG(新加坡)    04.瑞萬通博(TDCC)
                   ,FUND.SITCA_FUND_TYPE                                               -- 轉出基金投信顧公會基金類型 1.refer to FAS.SITCA_FUND_TYPE 2.目前分類，請參閱 FAS.SITCA_FUND_TYPE
                   ,FUND.ORDINAL                                                       -- 轉出基金序數 - 用於查詢或報表的排序
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO_OUT                                -- 轉出基金警語
                   ,FUND.SNAME AS FUND_SNAME_OUT                                       -- 轉出基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE_OUT                                    -- 轉出基金國際證券代碼
                   ,FUND.SHARE_CLASS AS SHARE_CLASS_OUT                                -- 轉出基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME_OUT                      -- 轉出基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO_OUT                                -- 轉出基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME_OUT                            -- 轉出基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL_OUT                       -- 轉出基金計價幣別小數位數
                   -- 2018.10.09 ChengYu 增加傳出參數 風險屬性(RISK_CATEGORY)
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY_OUT                            -- 轉出基金風險屬性 
                   ,SWITCH.TX_DATE                                                     -- 交易日期
                   ,'' AS PRODUCT_TYPE                                                 -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,'轉申購' AS PRODUCT_NAME                                           -- 產品類別名稱
                   -- 2018.12.04 ChengYu 調整轉申購金額、手續費、交易幣別、交易幣別名稱、轉入小數位數、淨值日
                   ,FUND_CURRENCY.CURRENCY_NO AS TRADE_CRNCY_OUT                       -- 轉出基金申購幣別
                   ,FUND_CURRENCY.NAME AS TRADE_CRNCY_NAME_OUT                         -- 轉出基金申購幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL_OUT                 -- 轉出基金申購幣別小數位數
                   -- 2020.01.21 JIANXIN 匯率改由買進匯率取得(同步修正，但畫面上沒有此欄位)
                   -- 2023.10.25 JIANXIN 調整匯率改由函數取得
                   ,CASE WHEN FUND.CURRENCY_NO = SWITCH.CURRENCY_NO THEN 1 
                         ELSE ECS.F_GET_EX_RATE(FAS_SWITCH.FUND_NO,FAS_SWITCH.EX_DATE,'B') 
                     END AS EX_RATE_OUT                                                -- 轉出基金參考匯率
                   -- 2018.10.17 ChengYu 修正轉申購的手續費、贖回金額、轉申購金額、轉申購單位數
                   -- 2018.12.04 ChengYu 修正轉申購金額、手續費欄位(瀚亞NTD_AMOUNT、NTD_SERVICE_CHARGE欄位無值，改抓AMOUNT、SERVICE_CHARGE)
                   -- 2018.12.04 ChengYu 調整轉申購金額、手續費、交易幣別、交易幣別名稱、轉入小數位數、淨值日
                   ,ROUND(RED_BANK.AMOUNT,FUND_CURRENCY.AMT_DECIMAL) AS AMOUNT_OUT     -- 贖回價金
                   ,SWITCH.APPLICATION_SHARE AS SHARES_OUT                             -- 贖回單位數
                   ,FUND.SHARE_DECIMAL AS SHARE_DECIMAL_OUT                            -- 轉出基金單位數小數位數
                   ,SWITCH.AC_DATE AS AC_DATE_OUT                                      -- 轉出基金淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN SWITCH.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV_OUT.NAV END  AS NAV_OUT                              -- 轉出基金淨值 
                   ,FUND.NAV_DECIMAL AS NAV_DECIMAL_OUT                                -- 轉出基金淨值小數位數
                   ,CASE WHEN SWITCH.STATUS = 'C' THEN 'C'
                         WHEN SWITCH.STATUS = 'P' AND SWITCH.RESULT_CODE = '00' THEN 'S'
                         WHEN SWITCH.STATUS = 'P' AND SWITCH.RESULT_CODE <> '00' THEN 'F'
                         ELSE '' 
                     END AS STATUS                                                     -- 交易狀態代碼
                   ,CASE WHEN SWITCH.STATUS = 'C' THEN '處理中'
                         WHEN SWITCH.STATUS = 'P' AND SWITCH.RESULT_CODE = '00' THEN '交易成功'
                         WHEN SWITCH.STATUS = 'P' AND SWITCH.RESULT_CODE <> '00' THEN '交易失敗'
                         ELSE '' 
                     END AS STATUS_NAME                                                -- 交易狀態
                   ,SWITCH.BROKER_NO                                                   -- 銷售機構代碼
                   ,SWITCH.BRANCH_NO                                                   -- 銷售機構分行代碼
                   ,SWITCH.SER_NO                                                      -- 成交序號
                   ,SWITCH.FUND_NO_IN AS FUND_NO_IN                                    -- 轉入基金代碼
                   ,FUND_IN.NAME AS FUND_NAME_IN                                       -- 轉入基金名稱
                   ,FUND_IN.DIVIDEND_DESC AS FUND_MEMO_IN                              -- 轉入基金警語
                   ,FUND_IN.SNAME AS FUND_SNAME_IN                                     -- 轉入基金簡稱
                   ,FUND_IN.ISIN_CODE AS ISIN_CODE_IN                                  -- 轉入基金國際證券代碼
                   ,FUND_IN.SHARE_CLASS AS SHARE_CLASS_IN                              -- 轉入基金級別
                   ,FUND_IN.NAME AS SHARE_CLASS_NAME_IN                                -- 轉入基金級別名稱
                   -- 2018.12.04 ChengYu 調整轉申購金額、手續費、交易幣別、交易幣別名稱、轉入小數位數、淨值日
                   ,FUND_IN.CURRENCY_NO AS CURRENCY_NO_IN                              -- 轉入基金計價幣別
                   ,FUND_CURRENCY_IN.NAME AS CURRENCY_NAME_IN                          -- 轉入基金計價幣別名稱
                   ,FUND_CURRENCY_IN.AMT_DECIMAL AS AMT_DECIMAL_IN                     -- 轉入基金計價幣別小數位數
                   -- 2018.10.09 ChengYu 增加傳出參數 風險屬性(RISK_CATEGORY)
                   ,FUND_IN.RISK_CATEGORY AS RISK_CATEGORY_IN                          -- 轉入基金風險屬性
                   ,SWITCH.SWITCH_DATE AS SWITCH_DATE                                  -- 轉申購日期
                   -- 2018.12.04 ChengYu 調整轉申購金額、手續費、交易幣別、交易幣別名稱、轉入小數位數、淨值日
                   ,FUND_CURRENCY_IN.CURRENCY_NO AS TRADE_CRNCY_IN                     -- 轉入基金申購幣別
                   ,FUND_CURRENCY_IN.NAME AS TRADE_CRNCY_NAME_IN                       -- 轉入基金申購幣別名稱
                   ,FUND_CURRENCY_IN.AMT_DECIMAL AS TRADE_AMT_DECIMAL_IN               -- 轉入基金申購幣別小數位數
                   -- 2020.01.21 JIANXIN 匯率改由賣出匯率取得(同步修正，但畫面上沒有此欄位)
                   -- 2023.10.25 JIANXIN 調整匯率改由函數取得
                   ,CASE WHEN SWITCH.CURRENCY_NO = SWITCH.BANK_CURRENCY_NO THEN 1 
                         ELSE ECS.F_GET_EX_RATE(SWITCH.FUND_NO_IN,FAS_SWITCH.EX_DATE,'S')
                     END AS EX_RATE_IN                                                 -- 轉入基金參考匯率
                   -- 2018.10.17 ChengYu 修正轉申購的手續費、贖回金額、轉申購金額、轉申購單位數 BEGIN
                   -- 2018.12.04 ChengYu 修正轉申購金額、手續費欄位(瀚亞NTD_AMOUNT、NTD_SERVICE_CHARGE欄位無值，改抓AMOUNT、SERVICE_CHARGE)
                   -- 2018.12.04 ChengYu 調整轉申購金額、手續費、交易幣別、交易幣別名稱、轉入小數位數、淨值日
                   ,ROUND(FAS_PURCHASE.AMOUNT,FUND_CURRENCY_IN.AMT_DECIMAL)  AS AMOUNT_IN  -- 轉申購金額(轉入基金淨額)
                   -- 2018.12.04 ChengYu 修正轉申購金額、手續費欄位(瀚亞NTD_AMOUNT、NTD_SERVICE_CHARGE欄位無值，改抓AMOUNT、SERVICE_CHARGE)
                   -- 2018.12.04 ChengYu 調整轉申購金額、手續費、交易幣別、交易幣別名稱、轉入小數位數、淨值日
                   ,ROUND(FAS_PURCHASE.SERVICE_CHARGE,FUND_CURRENCY_IN.AMT_DECIMAL) AS SERVICE_CHARGE_IN  -- 轉申購手續費
                   ,FAS_PURCHASE.SHARES AS SHARES_IN                                   -- 轉申購單位數
                   -- 2018.10.17 ChengYu 修正轉申購的手續費、贖回金額、轉申購金額、轉申購單位數 END
                   -- 2018.12.04 ChengYu 調整轉申購金額、手續費、交易幣別、交易幣別名稱、轉入小數位數、淨值日
                   ,FUND_IN.SHARE_DECIMAL AS SHARE_DECIMAL_IN                          -- 轉入基金單位數小數位數
                   ,FAS_PURCHASE.AC_DATE AS AC_DATE_IN                                 -- 轉入基金淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN SWITCH.AC_DATE < FUND_IN.INCEPTION_DATE THEN 10 
                         ELSE NAV_IN.NAV END AS NAV_IN                                 -- 轉入基金淨值 
                   ,FUND_IN.NAV_DECIMAL AS NAV_DECIMAL_IN                              -- 轉入基金淨值小數位數
                   -- 2019.04.17 tingting 因應需求編號"ES_20190315_01"，新增輸出欄位
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND_OUT     -- 轉出基金是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE_OUT                                         -- 轉出基金配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ_OUT.DISPLAY_NAME AS DIVIDEND_TYPE_NAME_OUT             -- 轉出基金配息頻率名稱
                   ,CASE WHEN FUND_IN.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND_IN   -- 轉入基金是否為配息
                   ,FUND_IN.DIVIDEND_FREQ AS DIVIDEND_TYPE_IN                                       -- 轉入基金配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ_IN.DISPLAY_NAME AS DIVIDEND_TYPE_NAME_IN               -- 轉入基金配息頻率名稱
                   ,REDEMPTION_LOG.REVISION_DATE AS RCV_DATE                           -- 委託日期(含時分秒)
                   -- 2021.05.10 Chengyu 修正贖回、轉申購時，FAS.REDEMPTION串接條件
                   ,ROUND(RED_BANK.WIRE_FEE,FUND_CURRENCY.AMT_DECIMAL) AS WIRE_FEE   -- 郵匯費
                   ,ROUND(RED_BANK.AMOUNT,FUND_CURRENCY.AMT_DECIMAL) - ROUND(RED_BANK.WIRE_FEE,FUND_CURRENCY.AMT_DECIMAL) AS NET_AMOUNT_OUT   -- 轉出基金淨額(贖回價金 - 郵匯費)
                   ,FAS_SWITCH.EX_DATE                                                 -- 匯率日期
                   ,FAS_PURCHASE.EX_RATE                                               -- 匯率-交叉匯率
                   ,SWITCH.FEE_CHOICE AS FEE_CHOICE_TYPE                               -- 優惠方案代碼
                   -- 2019.05.09 tingting 調整促銷活動名稱顯示
                   -- 2019.05.14 tingting 修正促銷活動名稱顯示(語法問題造成網路費率無法顯示)
                   ,CASE WHEN SWITCH.FEE_CHOICE = '1' THEN CAMPAIGN.CAMPAIGN_SHNM
                         WHEN SWITCH.FEE_CHOICE = '2' THEN '優惠券' || SWITCH.COUPON_ID
                         WHEN SWITCH.FEE_CHOICE = '3' THEN '紅利折抵' || SWITCH.USED_POINT || '點'
                         WHEN SWITCH.FEE_CHOICE = '5' THEN CAMPAIGN.CAMPAIGN_SHNM
                         WHEN SWITCH.FEE_CHOICE IS NULL THEN '網路費率'
                         END AS FEE_CHOICE_NAME                                        -- 優惠方案名稱
                   ,SWITCH.PROJECT_CODE                                                -- 促銷活動代碼
                   ,CAMPAIGN.CAMPAIGN_SHNM AS PROJECT_NAME                             -- 促銷活動名稱
                   -- 2022.03.02 Chengyu 贖回與轉申購新增欄位"報酬率"
                   -- 2022.04.14 JIANXIN 調整報酬率小數點2位
                   -- 2022.05.24 JIANXIN 調整報酬率計算方式，若遇到配息再申購或者是瑞萬系列的基金，則以 CERTIFICATE.AMOUNT 作為成本，否則以 CERTIFICATE.COST 作為成本
                   -- 2022.10.19 JIANXIN 調整報酬率計算方式
                   ,(SELECT ROUND((FAS_SWITCH.AMOUNT / SUM((CASE WHEN CERTIFICATE.BRANCH_NO ='7000' OR CERTIFICATE.FUND_NO LIKE 'V%' THEN CERTIFICATE.AMOUNT ELSE CERTIFICATE.COST END * RED_CERTIFICATE.REDEMPTION_SHARE / RED_CERTIFICATE.CERTIFICATE_SHARE)) - 1)*100 ,2)
                       FROM FAS.CERTIFICATE,
                            FAS.RED_CERTIFICATE
                      WHERE FAS.CERTIFICATE.FUND_NO = FAS.RED_CERTIFICATE.FUND_NO
                        AND FAS.CERTIFICATE.CERTIFICATE_NO = FAS.RED_CERTIFICATE.CERTIFICATE_NO
                        AND FAS_SWITCH.FUND_NO = FAS.RED_CERTIFICATE.FUND_NO
                        AND FAS_SWITCH.TX_DATE = FAS.RED_CERTIFICATE.TX_DATE
                        AND FAS_SWITCH.BROKER_NO = FAS.RED_CERTIFICATE.BROKER_NO
                        AND FAS_SWITCH.BRANCH_NO = FAS.RED_CERTIFICATE.BRANCH_NO
                        AND FAS_SWITCH.SER_NO = FAS.RED_CERTIFICATE.SER_NO
                    ) RATE
              FROM ECS.REDEMPTION SWITCH    
              JOIN FAS.FUND
                ON SWITCH.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON SWITCH.CURRENCY_NO = CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               -- 2018.10.08 JIANXIN MOD BY 處理基金級別串接問題
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              JOIN FAS.FUND FUND_IN
                ON SWITCH.FUND_NO_IN = FUND_IN.FUND_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY_IN
                ON FUND_IN.CURRENCY_NO = FUND_CURRENCY_IN.CURRENCY_NO
              -- 2018.11.30 ChengYu 修正轉申購 轉出淨值、轉入淨值 抓取方式
              -- 2020.04.14 JIANXIN 調整轉申購發生效能問題
              -- 2020.12.15 Chengyu 調整淨值資料限制條件為NAV_DATE
              -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
              LEFT JOIN (SELECT * FROM NAV.NAV WHERE NAV_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO ) NAV_OUT
                ON NAV_OUT.FUND_NO = SWITCH.FUND_NO
               AND NAV_OUT.NAV_DATE = SWITCH.AC_DATE              
              -- 2018.10.24 ChengYu 調整FAS.PURCHASE、FAS.RED_BANK 的JOIN語法 BEGIN
              LEFT JOIN FAS.PURCHASE FAS_PURCHASE
                ON FAS_PURCHASE.FUND_NO_OUT = SWITCH.FUND_NO
               AND FAS_PURCHASE.TX_DATE_OUT = SWITCH.TX_DATE
               AND FAS_PURCHASE.BROKER_NO = SWITCH.BROKER_NO
               AND FAS_PURCHASE.BRANCH_NO = SWITCH.BRANCH_NO
               -- 2018.10.30 ChengYu 補FAS.PURCHASE、FAS.RED_BANK 的JOIN條件
               AND FAS_PURCHASE.SER_NO_OUT = SWITCH.SER_NO
               AND FAS_PURCHASE.FUND_NO = SWITCH.FUND_NO_IN     
              -- 2018.10.12 yunchu 調整轉入.轉出淨值為依據轉入基金及轉出基金
              -- 2018.11.30 ChengYu 修正轉申購 轉出淨值、轉入淨值 抓取方式
              -- 2020.04.14 JIANXIN 調整轉申購發生效能問題
              -- 2020.12.15 Chengyu 調整淨值資料限制條件為NAV_DATE
              -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
              LEFT JOIN (SELECT * FROM NAV.NAV WHERE NAV_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) NAV_IN
                ON NAV_IN.FUND_NO = FAS_PURCHASE.FUND_NO
               AND NAV_IN.NAV_DATE = FAS_PURCHASE.AC_DATE
              LEFT JOIN FAS.RED_BANK RED_BANK
                ON RED_BANK.FUND_NO = FAS_PURCHASE.FUND_NO_OUT
               AND RED_BANK.TX_DATE = FAS_PURCHASE.TX_DATE_OUT
               -- 2018.10.30 ChengYu 補FAS.PURCHASE、FAS.RED_BANK 的JOIN條件
               AND RED_BANK.FE_BROKER_NO = '999'
               AND RED_BANK.FE_BRANCH_NO = '8300'
               AND RED_BANK.FE_SER_NO = FAS_PURCHASE.SER_NO_OUT
               AND RED_BANK.FUND_NO_IN = FAS_PURCHASE.FUND_NO
               AND RED_BANK.AC_BRANCH = SWITCH.AC_BRANCH
               AND RED_BANK.AC_BANK = SWITCH.AC_BANK
               AND RED_BANK.AC_CODE = SWITCH.AC_CODE
              -- 2021.05.10 Chengyu 修正贖回、轉申購時，FAS.REDEMPTION串接條件
              LEFT JOIN FAS.REDEMPTION FAS_SWITCH
                ON FAS_SWITCH.FUND_NO = RED_BANK.FUND_NO
               AND FAS_SWITCH.TX_DATE = RED_BANK.TX_DATE
               AND FAS_SWITCH.BROKER_NO = RED_BANK.BROKER_NO
               AND FAS_SWITCH.BRANCH_NO = RED_BANK.BRANCH_NO
               AND FAS_SWITCH.SER_NO = RED_BANK.SER_NO
              -- 2019.12.25 ChengYu 贖回使用FAS.REDEMPTION【買回】的欄位匯率日(EX_DATE)來取匯率(EX_RATE)、轉申購雖從FAS.PURCHASE抓取不影響，但調整轉出轉入基金匯率與FAS.PURCHASE保持規則一致性
              -- 2020.04.28 Chengyu 調整贖回與轉申購效能
              --LEFT JOIN (SELECT * FROM FAS.V_CURRENCY_RATE WHERE TX_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) V_CURRENCY_RATE
              --  ON V_CURRENCY_RATE.FUND_NO = FAS_SWITCH.FUND_NO
              -- AND V_CURRENCY_RATE.TX_DATE = FAS_SWITCH.EX_DATE
              -- 2019.12.25 ChengYu 贖回使用FAS.REDEMPTION【買回】的欄位匯率日(EX_DATE)來取匯率(EX_RATE)、轉申購雖從FAS.PURCHASE抓取不影響，但調整轉出轉入基金匯率與FAS.PURCHASE保持規則一致性
              -- 2020.04.28 Chengyu 調整贖回與轉申購效能
              --LEFT JOIN (SELECT * FROM FAS.V_CURRENCY_RATE WHERE TX_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) V_CURRENCY_RATE_IN
              --  ON V_CURRENCY_RATE_IN.FUND_NO = SWITCH.FUND_NO_IN
              -- AND V_CURRENCY_RATE_IN.TX_DATE = FAS_SWITCH.EX_DATE
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ_OUT -- 欄位選單資料(轉出基金配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ_OUT.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ_OUT.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ_IN  -- 欄位選單資料(轉入基金配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ_IN.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ_IN.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN (
                    SELECT  REDEMPTION_LOG.FUND_NO
                           ,REDEMPTION_LOG.ID
                           ,REDEMPTION_LOG.TX_DATE
                           ,REDEMPTION_LOG.BROKER_NO
                           ,REDEMPTION_LOG.BRANCH_NO
                           ,REDEMPTION_LOG.SER_NO
                           ,MAX(REDEMPTION_LOG.REVISION_DATE) AS REVISION_DATE 
                      FROM ECS.REDEMPTION_LOG       -- EC買回異動紀錄
                     WHERE REDEMPTION_LOG.ACTION_TYPE = 'A'     -- 交易動作(A:新增/M:修改/D:刪除)
                       AND REDEMPTION_LOG.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
                     GROUP BY REDEMPTION_LOG.FUND_NO
                           ,REDEMPTION_LOG.ID
                           ,REDEMPTION_LOG.TX_DATE
                           ,REDEMPTION_LOG.BROKER_NO
                           ,REDEMPTION_LOG.BRANCH_NO
                           ,REDEMPTION_LOG.SER_NO
                   ) REDEMPTION_LOG       -- EC買回異動紀錄
                ON REDEMPTION_LOG.FUND_NO = SWITCH.FUND_NO
               AND REDEMPTION_LOG.ID = SWITCH.ID
               AND REDEMPTION_LOG.TX_DATE = SWITCH.TX_DATE
               AND REDEMPTION_LOG.BROKER_NO = SWITCH.BROKER_NO
               AND REDEMPTION_LOG.BRANCH_NO = SWITCH.BRANCH_NO
               AND REDEMPTION_LOG.SER_NO = SWITCH.SER_NO
              LEFT JOIN ECS.CAMPAIGN    -- 促銷活動
                ON CAMPAIGN.CAMPAIGN_CODE = SWITCH.PROJECT_CODE
              -- 2018.10.24 ChengYu 調整FAS.PURCHASE、FAS.RED_BANK 的JOIN語法 END
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE SWITCH.ID = XID
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'5' ) AND SWITCH.BRANCH_NO IN ('8300')) -- 轉申購：8300；
               AND (TX_STATUS.TX_STATUS = 'S' AND SWITCH.STATUS = 'P') -- 交易成功：STATUS='P' ；處理中：STATUS='C'；
               AND SWITCH.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
             UNION 
             -- 2021.06.24 Chengyu 新增書面資料
             SELECT  SWITCH.FUND_NO AS FUND_NO_OUT                                      -- 轉出基金代碼
                   ,FUND.NAME AS FUND_NAME_OUT                                         -- 轉出基金名稱
                   ,FUND.FUND_CATEGORY                                                 -- 轉出基金類別(境內/境外基金) 1:境內 2:境外
                   ,FUND.AMC_NO                                                        -- 轉出基金經理公司 1.refer to fas.AMC(已隱藏) 2.目前分類 01.瀚亞投信(境內) 02.瀚亞投資(新加坡) 03.MG(新加坡)    04.瑞萬通博(TDCC)
                   ,FUND.SITCA_FUND_TYPE                                               -- 轉出基金投信顧公會基金類型 1.refer to FAS.SITCA_FUND_TYPE 2.目前分類，請參閱 FAS.SITCA_FUND_TYPE
                   ,FUND.ORDINAL                                                       -- 轉出基金序數 - 用於查詢或報表的排序
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO_OUT                                -- 轉出基金警語
                   ,FUND.SNAME AS FUND_SNAME_OUT                                       -- 轉出基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE_OUT                                    -- 轉出基金國際證券代碼
                   ,FUND.SHARE_CLASS AS SHARE_CLASS_OUT                                -- 轉出基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME_OUT                      -- 轉出基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO_OUT                                -- 轉出基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME_OUT                            -- 轉出基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL_OUT                       -- 轉出基金計價幣別小數位數
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY_OUT                            -- 轉出基金風險屬性 
                   ,SWITCH.TX_DATE                                                     -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,SWITCH.PRODUCT_TYPE AS PRODUCT_TYPE                                                 -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,ECS.F_GET_PRODUCT_NAME(SWITCH.PRODUCT_TYPE) AS PRODUCT_NAME                                           -- 產品類別名稱
                   ,FUND_CURRENCY.CURRENCY_NO AS TRADE_CRNCY_OUT                       -- 轉出基金申購幣別
                   ,FUND_CURRENCY.NAME AS TRADE_CRNCY_NAME_OUT                         -- 轉出基金申購幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL_OUT                 -- 轉出基金申購幣別小數位數
                   -- 2023.10.25 JIANXIN 調整匯率改由函數取得
                   ,CASE WHEN FUND.CURRENCY_NO = SWITCH.CURRENCY_NO THEN 1 
                         ELSE ECS.F_GET_EX_RATE(SWITCH.FUND_NO,SWITCH.TX_DATE,'B') 
                     END AS EX_RATE_OUT                                                -- 轉出基金參考匯率
                   ,ROUND(RED_BANK.AMOUNT,FUND_CURRENCY.AMT_DECIMAL) AS AMOUNT_OUT     -- 贖回價金
                   ,SWITCH.APPLICATION_SHARE AS SHARES_OUT                             -- 贖回單位數
                   ,FUND.SHARE_DECIMAL AS SHARE_DECIMAL_OUT                            -- 轉出基金單位數小數位數
                   ,SWITCH.AC_DATE AS AC_DATE_OUT                                      -- 轉出基金淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN SWITCH.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV_OUT.NAV END AS NAV_OUT                               -- 轉出基金淨值 
                   ,FUND.NAV_DECIMAL AS NAV_DECIMAL_OUT                                -- 轉出基金淨值小數位數
                   -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
                   ,'S' AS STATUS                                                      -- 交易狀態代碼
                   ,'交易成功' AS STATUS_NAME                                          -- 交易狀態
                   ,SWITCH.BROKER_NO                                                   -- 銷售機構代碼
                   ,SWITCH.BRANCH_NO                                                   -- 銷售機構分行代碼
                   ,SWITCH.SER_NO                                                      -- 成交序號
                   ,RED_BANK.FUND_NO_IN AS FUND_NO_IN                                  -- 轉入基金代碼
                   ,FUND_IN.NAME AS FUND_NAME_IN                                       -- 轉入基金名稱
                   ,FUND_IN.DIVIDEND_DESC AS FUND_MEMO_IN                              -- 轉入基金警語
                   ,FUND_IN.SNAME AS FUND_SNAME_IN                                     -- 轉入基金簡稱
                   ,FUND_IN.ISIN_CODE AS ISIN_CODE_IN                                  -- 轉入基金國際證券代碼
                   ,FUND_IN.SHARE_CLASS AS SHARE_CLASS_IN                              -- 轉入基金級別
                   ,FUND_IN.NAME AS SHARE_CLASS_NAME_IN                                -- 轉入基金級別名稱
                   ,FUND_IN.CURRENCY_NO AS CURRENCY_NO_IN                              -- 轉入基金計價幣別
                   ,FUND_CURRENCY_IN.NAME AS CURRENCY_NAME_IN                          -- 轉入基金計價幣別名稱
                   ,FUND_CURRENCY_IN.AMT_DECIMAL AS AMT_DECIMAL_IN                     -- 轉入基金計價幣別小數位數
                   ,FUND_IN.RISK_CATEGORY AS RISK_CATEGORY_IN                          -- 轉入基金風險屬性
                   ,FAS_PURCHASE.TX_DATE AS SWITCH_DATE                                -- 轉申購日期
                   ,FUND_CURRENCY_IN.CURRENCY_NO AS TRADE_CRNCY_IN                     -- 轉入基金申購幣別
                   ,FUND_CURRENCY_IN.NAME AS TRADE_CRNCY_NAME_IN                       -- 轉入基金申購幣別名稱
                   ,FUND_CURRENCY_IN.AMT_DECIMAL AS TRADE_AMT_DECIMAL_IN               -- 轉入基金申購幣別小數位數
                   -- 2023.10.25 JIANXIN 調整匯率改由函數取得
                   ,CASE WHEN SWITCH.CURRENCY_NO = RED_BANK.CURRENCY_NO THEN 1 
                         ELSE ECS.F_GET_EX_RATE(RED_BANK.FUND_NO,SWITCH.TX_DATE,'S') 
                     END AS EX_RATE_IN                                                 -- 轉入基金參考匯率
                   ,ROUND(FAS_PURCHASE.AMOUNT,FUND_CURRENCY_IN.AMT_DECIMAL)  AS AMOUNT_IN  -- 轉申購金額(轉入基金淨額)
                   ,ROUND(FAS_PURCHASE.SERVICE_CHARGE,FUND_CURRENCY_IN.AMT_DECIMAL) AS SERVICE_CHARGE_IN  -- 轉申購手續費
                   ,FAS_PURCHASE.SHARES AS SHARES_IN                                   -- 轉申購單位數
                   ,FUND_IN.SHARE_DECIMAL AS SHARE_DECIMAL_IN                          -- 轉入基金單位數小數位數
                   ,FAS_PURCHASE.AC_DATE AS AC_DATE_IN                                 -- 轉入基金淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN SWITCH.AC_DATE < FUND_IN.INCEPTION_DATE THEN 10 
                         ELSE NAV_IN.NAV END AS NAV_IN                                 -- 轉入基金淨值 
                   ,FUND_IN.NAV_DECIMAL AS NAV_DECIMAL_IN                              -- 轉入基金淨值小數位數
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND_OUT     -- 轉出基金是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE_OUT                                         -- 轉出基金配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ_OUT.DISPLAY_NAME AS DIVIDEND_TYPE_NAME_OUT             -- 轉出基金配息頻率名稱
                   ,CASE WHEN FUND_IN.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND_IN   -- 轉入基金是否為配息
                   ,FUND_IN.DIVIDEND_FREQ AS DIVIDEND_TYPE_IN                                       -- 轉入基金配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ_IN.DISPLAY_NAME AS DIVIDEND_TYPE_NAME_IN               -- 轉入基金配息頻率名稱
                   ,TO_DATE('1900/01/01','YYYY/MM/DD') AS  RCV_DATE                                               -- 委託日期(含時分秒)
                   ,ROUND(RED_BANK.WIRE_FEE,FUND_CURRENCY.AMT_DECIMAL) AS WIRE_FEE   -- 郵匯費
                   ,ROUND(RED_BANK.AMOUNT,FUND_CURRENCY.AMT_DECIMAL) - ROUND(RED_BANK.WIRE_FEE,FUND_CURRENCY.AMT_DECIMAL) AS NET_AMOUNT_OUT   -- 轉出基金淨額(贖回價金 - 郵匯費)
                   ,SWITCH.EX_DATE                                                     -- 匯率日期
                   ,FAS_PURCHASE.EX_RATE                                               -- 匯率-交叉匯率
                   ,'' AS FEE_CHOICE_TYPE                                              -- 優惠方案代碼
                   ,'' AS FEE_CHOICE_NAME                                              -- 優惠方案名稱
                   ,'' AS PROJECT_CODE                                                 -- 促銷活動代碼(臨櫃促銷活動處理不同)
                   ,'' AS PROJECT_NAME                                                 -- 促銷活動名稱(臨櫃促銷活動處理不同)
                   -- 2022.03.02 Chengyu 贖回與轉申購新增欄位"報酬率"
                   -- 2022.04.14 JIANXIN 調整報酬率小數點2位
                   -- 2022.05.24 JIANXIN 調整報酬率計算方式，若遇到配息再申購或者是瑞萬系列的基金，則以 CERTIFICATE.AMOUNT 作為成本，否則以 CERTIFICATE.COST 作為成本
                   -- 2022.10.19 JIANXIN 調整報酬率計算方式
                   ,(SELECT ROUND((SWITCH.AMOUNT / SUM((CASE WHEN CERTIFICATE.BRANCH_NO ='7000' OR CERTIFICATE.FUND_NO LIKE 'V%' THEN CERTIFICATE.AMOUNT ELSE CERTIFICATE.COST END * RED_CERTIFICATE.REDEMPTION_SHARE / RED_CERTIFICATE.CERTIFICATE_SHARE)) - 1)*100 ,2)
                       FROM FAS.CERTIFICATE,
                            FAS.RED_CERTIFICATE
                      WHERE FAS.CERTIFICATE.FUND_NO = FAS.RED_CERTIFICATE.FUND_NO
                        AND FAS.CERTIFICATE.CERTIFICATE_NO = FAS.RED_CERTIFICATE.CERTIFICATE_NO
                        AND SWITCH.FUND_NO = FAS.RED_CERTIFICATE.FUND_NO
                        AND SWITCH.TX_DATE = FAS.RED_CERTIFICATE.TX_DATE
                        AND SWITCH.BROKER_NO = FAS.RED_CERTIFICATE.BROKER_NO
                        AND SWITCH.BRANCH_NO = FAS.RED_CERTIFICATE.BRANCH_NO
                        AND SWITCH.SER_NO = FAS.RED_CERTIFICATE.SER_NO
                    ) RATE
              FROM FAS.REDEMPTION SWITCH    
              JOIN FAS.FUND
                ON SWITCH.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON SWITCH.CURRENCY_NO = CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
              LEFT JOIN (SELECT * FROM NAV.NAV WHERE NAV_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO ) NAV_OUT
                ON NAV_OUT.FUND_NO = SWITCH.FUND_NO
               AND NAV_OUT.NAV_DATE = SWITCH.AC_DATE
              LEFT JOIN FAS.RED_BANK RED_BANK
                ON RED_BANK.FUND_NO = SWITCH.FUND_NO
               AND RED_BANK.TX_DATE = SWITCH.TX_DATE
               AND RED_BANK.BROKER_NO = SWITCH.BROKER_NO
               AND RED_BANK.BRANCH_NO = SWITCH.BRANCH_NO
               AND RED_BANK.SER_NO = SWITCH.SER_NO
              -- 2021.09.03 Chengyu 2.修正轉申購資料顯示
              LEFT JOIN FAS.PURCHASE FAS_PURCHASE
                ON FAS_PURCHASE.ID = SWITCH.ID
               AND FAS_PURCHASE.FUND_NO_OUT = SWITCH.FUND_NO
               AND FAS_PURCHASE.TX_DATE_OUT = SWITCH.TX_DATE
               AND FAS_PURCHASE.BROKER_NO = SWITCH.BROKER_NO
               AND FAS_PURCHASE.BRANCH_NO = SWITCH.BRANCH_NO
               AND FAS_PURCHASE.ORG_SER_NO = SWITCH.SER_NO
               AND FAS_PURCHASE.FUND_NO = RED_BANK.FUND_NO_IN
              JOIN FAS.FUND FUND_IN
                ON FUND_IN.FUND_NO = FAS_PURCHASE.FUND_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY_IN
                ON FUND_IN.CURRENCY_NO = FUND_CURRENCY_IN.CURRENCY_NO
              -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
              LEFT JOIN (SELECT * FROM NAV.NAV WHERE NAV_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) NAV_IN
                ON NAV_IN.FUND_NO = FAS_PURCHASE.FUND_NO
               AND NAV_IN.NAV_DATE = FAS_PURCHASE.AC_DATE
              --LEFT JOIN (SELECT * FROM FAS.V_CURRENCY_RATE WHERE TX_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) V_CURRENCY_RATE
              --  ON V_CURRENCY_RATE.FUND_NO = SWITCH.FUND_NO
              -- AND V_CURRENCY_RATE.TX_DATE = SWITCH.EX_DATE
              --LEFT JOIN (SELECT * FROM FAS.V_CURRENCY_RATE WHERE TX_DATE BETWEEN ADD_MONTHS(WTX_DATE_FROM, -1) AND WTX_DATE_TO) V_CURRENCY_RATE_IN
              --  ON V_CURRENCY_RATE_IN.FUND_NO = RED_BANK.FUND_NO_IN
              -- AND V_CURRENCY_RATE_IN.TX_DATE = SWITCH.EX_DATE
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ_OUT -- 欄位選單資料(轉出基金配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ_OUT.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ_OUT.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ_IN  -- 欄位選單資料(轉入基金配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ_IN.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ_IN.TEXT_VALUE = FUND.DIVIDEND_FREQ
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE SWITCH.ID = XID
               -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'5' ) AND SWITCH.BRANCH_NO NOT IN ('8000','8200','8300','8400','8800')) -- 轉申購：8300；
               -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
               -- 書面交易成功： FAS.REDEMPTION.STATUS = 'S' or 'T'
               AND (TX_STATUS.TX_STATUS = 'S' AND SWITCH.STATUS IN ('S','T')) 
               AND SWITCH.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
               --有轉申購日期為轉申購資料
               AND SWITCH.ST_DATE IS NOT NULL
           ) DataList
             -- 2018.10.25 tingting 1.增加傳出欄位：排列序號
             --                     2.調整排序：先依交易日期、交易狀態(處理中→交易成功→交易失敗)
             --                                再依基金排序規則(境內外、基金公司、投信顧公會基金類型、母基金排序、基金級別、子基金排序)
             ORDER BY TX_DATE,                          --交易日期
                      CASE WHEN STATUS = 'C' THEN 1     --處理中
                           WHEN STATUS = 'S' THEN 2     --交易成功
                           WHEN STATUS = 'F' THEN 3     --交易失敗
                           ELSE 4 END,                  --交易狀態
                      FUND_CATEGORY, AMC_NO, SITCA_FUND_TYPE, ORDINAL, SHARE_CLASS_OUT NULLS FIRST, ORDINAL;
    -- =============================================================== 轉申購 END ===============================================================

    -- ============================================================== 網路/書面定額 BNG ==============================================================
    -- 2018.09.05 tingting MOD BY 1.修正轉申購手續費欄位錯誤 
    --                            2.(不)定額扣款列表 分開拆成定額扣款列表、不定額扣款列表及相關調整 
    --                            3.申購列表、定額扣款列表增加傳出欄位：申購總價金、台幣申購總價金 
    --                            4.不定額扣款列表增加傳出欄位：申購總價金、台幣申購總價金、加碼點(%)、加碼金額、減碼點(%)、減碼金額
    OPEN RSPDT FOR
    SELECT  ROWNUM AS ROW_NUM                                     -- 排列序號
           ,DataList.*
      FROM (
            -- EC 網路申購
            SELECT  PURCHASE.FUND_NO                              -- 基金代碼
                   ,FUND.NAME AS FUND_NAME                        -- 基金名稱
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO               -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                      -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                   -- 國際證券代碼
                   ,FUND.SHARE_CLASS                              -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME     -- 基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO               -- 基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME           -- 基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL      -- 基金計價幣別小數位數
                   ,PURCHASE.TX_DATE                              -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,'P8' AS PRODUCT_TYPE                          -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,ECS.F_GET_PRODUCT_NAME('P8') AS PRODUCT_NAME                     -- 產品類別名稱
                   -- 2018.09.07 tingting MOD BY 定額扣款列表、不定額扣款列表增加傳出欄位：每月扣款日
                   ,PURCHASE.AC_DAY                               -- 每月扣款日
                   ,PURCHASE.CURRENCY_NO AS TRADE_CRNCY           -- 付款幣別
                   ,CURRENCY.NAME AS TRADE_CRNCY_NAME             -- 付款幣別名稱
                   ,CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL     -- 付款幣別小數位數
                   -- 2018.10.09 ChengYu 增加傳出參數 風險屬性(RISK_CATEGORY)
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY           -- 基金風險屬性
                   -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
                   ,CASE WHEN FUND.CURRENCY_NO = PURCHASE.CURRENCY_NO THEN 1 
                         ELSE NVL(FAS_PURCHASE.EX_RATE,0)
                     END AS EX_RATE                               -- 參考匯率
                   ,PURCHASE.AMOUNT                               -- 原幣申購價金
                   -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_AMOUNT IS NOT NULL THEN FAS_PURCHASE.NTD_AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.AMOUNT IS NOT NULL THEN FAS_PURCHASE.AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.AMOUNT
                         ELSE NVL(FAS_PURCHASE.NTD_AMOUNT,0) 
                     END AS NTD_AMOUNT                            -- 台幣申購價金
                   ,ROUND(PURCHASE.AMOUNT + PURCHASE.SERVICE_CHARGE, FUND_CURRENCY.AMT_DECIMAL) AS TOT_AMOUNT    -- 原幣申購總價金
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,ROUND(CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_AMOUNT IS NOT NULL THEN FAS_PURCHASE.NTD_AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.AMOUNT IS NOT NULL THEN FAS_PURCHASE.AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.AMOUNT
                               ELSE 0
                           END + 
                          CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.NTD_SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.SERVICE_CHARGE
                               ELSE 0 
                           END, 0) AS NTD_TOT_AMOUNT   -- 台幣申購總價金
                   -- 2019.04.29 tingting 修正定額扣款列表、不定額扣款列表單位數錯誤
                   ,FAS_PURCHASE.SHARES                           -- 單位數
                   ,FUND.SHARE_DECIMAL                            -- 單位數小數位數
                   ,PURCHASE.AC_DATE                              -- 淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN PURCHASE.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV.NAV END NAV                     -- 淨值 
                   ,FUND.NAV_DECIMAL                              -- 淨值小數位數
                   ,PURCHASE.SERVICE_CHARGE                       -- 原幣手續費 
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.NTD_SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.SERVICE_CHARGE
                         ELSE 0 
                     END AS NTD_SERVICE_CHARGE                    -- 台幣申購手續費
                   -- 2018.11.09 PEN MOD 網路定額、網路不定額列表 調整 處理中 的判斷條件
                   ,CASE WHEN PURCHASE.STATUS = 'C' OR PURCHASE.STATUS = 'O' THEN 'C'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE <> '00' THEN 'F'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE = '00' THEN 'S'
                         ELSE '' 
                     END AS STATUS                                -- 交易狀態代碼
                   ,CASE WHEN PURCHASE.STATUS = 'C' OR PURCHASE.STATUS = 'O' THEN '處理中'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE <> '00' THEN '交易失敗'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE = '00' THEN '交易成功'
                         ELSE '' 
                     END AS STATUS_NAME                           -- 交易狀態
                   ,PURCHASE.BROKER_NO                            -- 銷售機構代碼
                   ,PURCHASE.BRANCH_NO                            -- 銷售機構分行代碼
                   ,PURCHASE.SER_NO                               -- 成交序號
                   ,'4' AS PAY_TYPE                               -- 付款方式
                   ,'扣款' AS PAY_TYPE_NAME                       -- 付款方式名稱
                   -- 2018.09.07 tingting MOD BY 1.申購列表' 定額扣款列表、不定額扣款列表 增加傳出欄位：扣款總行代碼、扣款分行代碼、扣款銀行簡稱、扣款帳號、扣款帳號後5碼
                   --                            2.調整申購列表 交易狀態 = S：交易成功 的取資料方式(Mark掉Union上面那段，從下面那段出資料)
                   ,PURCHASE.AC_BANK                              -- 扣款總行代碼
                   ,PURCHASE.AC_BRANCH                            -- 扣款分行代碼
                   ,FIS_BRANCH.SNAME AS  FIS_SNAME                -- 扣款銀行簡稱
                   ,PURCHASE.AC_CODE                              -- 扣款帳號
                   ,ECS.FN_STUFF_STR(PURCHASE.AC_CODE,5,'*') AS AC_CODE_5  -- 往來帳號後5碼
                   -- 2019.04.17 tingting 因應需求編號"ES_20190315_01"，新增輸出欄位
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE           -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME             -- 配息頻率名稱
                   ,PURCHASE.REVISION_DATE AS RCV_DATE            -- 委託日期(含時分秒)
                   ,PURCHASE.FEE_CHOICE AS FEE_CHOICE_TYPE        -- 優惠方案代碼
                   -- 2019.05.09 tingting 調整促銷活動名稱顯示
                   -- 2019.05.14 tingting 修正促銷活動名稱顯示(語法問題造成網路費率無法顯示)
                   -- 2024.08.01 RICHARD begin 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
--                   -- 2024.05.24 RICHARD begin 調整PURCHASE.FEE_CHOICE = '5' 由固定hard code改為,campaign.campagin_shnm
--                   --,CASE WHEN PURCHASE.FEE_CHOICE = '1' THEN CAMPAIGN.CAMPAIGN_SHNM
--                   ,CASE WHEN PURCHASE.FEE_CHOICE in ('1','5') THEN CAMPAIGN.CAMPAIGN_SHNM
--                   -- 2024.05.24 richard end 調整PURCHASE.FEE_CHOICE = '5' 由固定hard code改為,campaign.campagin_shnm
--                         WHEN PURCHASE.FEE_CHOICE = '2' THEN '優惠券' || RSP.COUPON_ID
--                         WHEN PURCHASE.FEE_CHOICE = '3' THEN '紅利折抵'
--                         -- 2024.05.24 richard begin 調整PURCHASE.FEE_CHOICE = '5' 由固定hard code改為,campaign.campagin_shnm
--                         --WHEN PURCHASE.FEE_CHOICE = '5' THEN '生日優惠'
--                         -- 2024.05.24 richard end 調整PURCHASE.FEE_CHOICE = '5' 由固定hard code改為,campaign.campagin_shnm
--                         WHEN PURCHASE.FEE_CHOICE IS NULL THEN '網路費率'
--                         END AS FEE_CHOICE_NAME                   -- 優惠方案名稱
                   -- 2024.08.01 RICHARD begin 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
--                   ,CASE WHEN PURCHASE.FEE_CHOICE in ('1','5') THEN CAMPAIGN.CAMPAIGN_SHNM
--                         WHEN PURCHASE.FEE_CHOICE = '2' THEN '優惠券' || RSP.COUPON_ID
--                         WHEN PURCHASE.FEE_CHOICE = '3' THEN '紅利折抵'
                     ,CASE WHEN PURCHASE.FEE_CHOICE in ('1','2','3','5') THEN CAMPAIGN.CAMPAIGN_SHNM
                   -- 2024.08.01 RICHARD end 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
                         WHEN PURCHASE.FEE_CHOICE IS NULL THEN '網路費率'
                         END AS FEE_CHOICE_NAME                   -- 優惠方案名稱
                   -- 2024.08.01 RICHARD end 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
                   ,PURCHASE.PROJECT_CODE                         -- 促銷活動代碼
                   ,CAMPAIGN.CAMPAIGN_SHNM AS PROJECT_NAME        -- 促銷活動名稱
                   ,FUND.FUND_CATEGORY
                   ,FUND.AMC_NO
                   ,FUND.SITCA_FUND_TYPE
                   ,FUND.ORDINAL
              FROM FAS.EC_RSP_PURCHASE PURCHASE
              JOIN FAS.FUND
                ON PURCHASE.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON PURCHASE.CURRENCY_NO = CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               -- 2018.10.08 JIANXIN MOD BY 處理基金級別串接問題
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              LEFT JOIN FAS.PURCHASE FAS_PURCHASE
                ON FAS_PURCHASE.ID = PURCHASE.ID
               AND FAS_PURCHASE.FUND_NO = PURCHASE.FUND_NO
               AND FAS_PURCHASE.TX_DATE = PURCHASE.TX_DATE
               AND FAS_PURCHASE.BRANCH_NO = PURCHASE.BRANCH_NO
               AND FAS_PURCHASE.SER_NO = PURCHASE.SER_NO
               -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
              LEFT JOIN NAV.NAV
                ON NAV.FUND_NO = FUND.FUND_NO
               AND NAV.NAV_DATE = PURCHASE.AC_DATE
              LEFT JOIN ECS.RSP RSP
                ON RSP.ID = PURCHASE.ID
               AND RSP.ID_SEQ = PURCHASE.ID_SEQ
              LEFT JOIN FAS.FIS_BRANCH FIS_BRANCH
                ON FIS_BRANCH.BANK_NO = PURCHASE.AC_BANK
               AND FIS_BRANCH.BRANCH_NO = PURCHASE.AC_BRANCH
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN ECS.CAMPAIGN            -- 促銷活動
                ON CAMPAIGN.CAMPAIGN_CODE = PURCHASE.PROJECT_CODE
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE PURCHASE.ID = XID
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'2' ) AND PURCHASE.BRANCH_NO = '8200') -- 網路定額：8200
               -- 2018.11.09 PEN MOD 網路定額、網路不定額列表 調整 處理中 的判斷條件
               AND (  (TX_STATUS.TX_STATUS = 'S' AND PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE = '00') -- 交易成功：STATUS='P' AND RESULT_CODE = '00'；處理中：STATUS='C'；交易失敗：STATUS='P' AND RESULT_CODE <> '00'
                   OR (TX_STATUS.TX_STATUS = 'C' AND PURCHASE.STATUS IN ('C','O')) 
                   OR (TX_STATUS.TX_STATUS = 'F' AND PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE <> '00'))
               AND PURCHASE.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
             UNION
             -- 2021.06.24 Chengyu 新增書面資料
             SELECT  PURCHASE.FUND_NO                             -- 基金代碼
                   ,FUND.NAME AS FUND_NAME                        -- 基金名稱
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO               -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                      -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                   -- 國際證券代碼
                   ,FUND.SHARE_CLASS                              -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME     -- 基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO               -- 基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME           -- 基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL      -- 基金計價幣別小數位數
                   ,PURCHASE.TX_DATE                              -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,PERIODIC.PRODUCT_TYPE AS PRODUCT_TYPE         -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,ECS.F_GET_PRODUCT_NAME(PERIODIC.PRODUCT_TYPE) AS PRODUCT_NAME                    -- 產品類別名稱
                   ,PURCHASE.AC_DAY                               -- 每月扣款日
                   ,PURCHASE.CURRENCY_NO AS TRADE_CRNCY           -- 付款幣別
                   ,CURRENCY.NAME AS TRADE_CRNCY_NAME             -- 付款幣別名稱
                   ,CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL     -- 付款幣別小數位數
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY           -- 基金風險屬性
                   ,CASE WHEN FUND.CURRENCY_NO = PURCHASE.CURRENCY_NO THEN 1 
                         ELSE NVL(PURCHASE.EX_RATE,0)
                     END AS EX_RATE                               -- 參考匯率
                   ,PURCHASE.AMOUNT                               -- 原幣申購價金
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_AMOUNT IS NOT NULL THEN PURCHASE.NTD_AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.AMOUNT IS NOT NULL THEN PURCHASE.AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.AMOUNT
                         ELSE 0
                     END AS NTD_AMOUNT                            -- 台幣申購價金  
                   ,ROUND(PURCHASE.AMOUNT + PURCHASE.SERVICE_CHARGE, FUND_CURRENCY.AMT_DECIMAL) AS TOT_AMOUNT    -- 原幣申購總價金
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,ROUND(CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_AMOUNT IS NOT NULL THEN PURCHASE.NTD_AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.AMOUNT IS NOT NULL THEN PURCHASE.AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.AMOUNT
                               ELSE 0
                           END + 
                          CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN PURCHASE.NTD_SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.SERVICE_CHARGE IS NOT NULL THEN PURCHASE.SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.SERVICE_CHARGE
                               ELSE 0 
                           END, 0) AS NTD_TOT_AMOUNT   -- 台幣申購總價金
                   ,PURCHASE.SHARES                               -- 單位數
                   ,FUND.SHARE_DECIMAL                            -- 單位數小數位數
                   ,PURCHASE.AC_DATE                              -- 淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN PURCHASE.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV.NAV END NAV                     -- 淨值 
                   ,FUND.NAV_DECIMAL                              -- 淨值小數位數
                   ,PURCHASE.SERVICE_CHARGE                       -- 原幣手續費 
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN PURCHASE.NTD_SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.SERVICE_CHARGE IS NOT NULL THEN PURCHASE.SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.SERVICE_CHARGE
                         ELSE 0 
                     END AS NTD_SERVICE_CHARGE                    -- 台幣申購手續費
                   -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
                   ,'S' STATUS                                    -- 交易狀態代碼
                   ,'交易成功' AS STATUS_NAME                     -- 交易狀態
                   ,PERIODIC.BROKER_NO                            -- 銷售機構代碼
                   ,PERIODIC.BRANCH_NO                            -- 銷售機構分行代碼
                   ,CAST(PERIODIC.SER_NO AS VARCHAR2(10)) SER_NO  -- 成交序號
                   ,'4' AS PAY_TYPE                               -- 付款方式
                   ,'扣款' AS PAY_TYPE_NAME                       -- 付款方式名稱
                   ,PURCHASE.AC_BANK                              -- 扣款總行代碼
                   ,'' AS AC_BRANCH                               -- 扣款分行代碼(後台未記錄扣款當下分行)
                   ,FIS_BANK.SNAME AS  FIS_SNAME                  -- 扣款銀行簡稱
                   ,PURCHASE.AC_CODE                              -- 扣款帳號
                   ,ECS.FN_STUFF_STR(PURCHASE.AC_CODE,5,'*') AS AC_CODE_5  -- 往來帳號後5碼
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE           -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME             -- 配息頻率名稱
                   ,TO_DATE('1900/01/01','YYYY/MM/DD') AS RCV_DATE-- 委託日期(含時分秒)
                   ,'' AS FEE_CHOICE_TYPE                         -- 優惠方案代碼
                   ,'' AS FEE_CHOICE_NAME                         -- 優惠方案名稱
                   ,'' AS PROJECT_CODE                            -- 促銷活動代碼(書面定額促銷活動處理方式不同)
                   ,'' AS PROJECT_NAME                            -- 促銷活動名稱(書面定額促銷活動處理方式不同)
                   ,FUND.FUND_CATEGORY
                   ,FUND.AMC_NO
                   ,FUND.SITCA_FUND_TYPE
                   ,FUND.ORDINAL
              FROM FAS.PERIODIC_PURCHASE PURCHASE
              JOIN FAS.PERIODIC
                ON PERIODIC.ID = PURCHASE.ID
               AND PERIODIC.ID_SEQ = PURCHASE.ID_SEQ
              JOIN FAS.FUND
                ON PURCHASE.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON PURCHASE.CURRENCY_NO = CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              LEFT JOIN NAV.NAV
                ON NAV.FUND_NO = FUND.FUND_NO
               AND NAV.NAV_DATE = PURCHASE.AC_DATE
              LEFT JOIN FAS.FIS_BANK FIS_BANK
                ON FIS_BANK.BANK_NO = PURCHASE.AC_BANK
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE PURCHASE.ID = XID
               -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'2' ) AND PERIODIC.PRODUCT_TYPE = 'P1') 
               -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
               -- 書面定額交易成功： 只顯示成功資料，FAS.PERIODIC_PURCHASE.RESULT_CODE = '00'  AND PERIODIC_PURCHASE.SHARES > 0
               AND (TX_STATUS.TX_STATUS = 'S' AND PURCHASE.RESULT_CODE = '00'  AND PURCHASE.SHARES > 0)
               AND PURCHASE.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
           ) DataList
     -- 2018.10.25 tingting 1.增加傳出欄位：排列序號
     --                     2.調整排序：先依交易日期、交易狀態(處理中→交易成功→交易失敗)
     --                                再依基金排序規則(境內外、基金公司、投信顧公會基金類型、母基金排序、基金級別、子基金排序)
     ORDER BY TX_DATE,                          --交易日期
              CASE WHEN STATUS = 'C' THEN 1     --處理中
                   WHEN STATUS = 'S' THEN 2     --交易成功
                   WHEN STATUS = 'F' THEN 3     --交易失敗
                   ELSE 4 END,                           --交易狀態
              FUND_CATEGORY, AMC_NO, SITCA_FUND_TYPE, ORDINAL, SHARE_CLASS NULLS FIRST, ORDINAL;    
    -- ============================================================== 網路/書面定額 END ==============================================================

    -- 2018.09.05 tingting MOD BY 1.修正轉申購手續費欄位錯誤 
    --                            2.(不)定額扣款列表 分開拆成定額扣款列表、不定額扣款列表及相關調整 
    --                            3.申購列表、定額扣款列表增加傳出欄位：申購總價金、台幣申購總價金 
    --                            4.不定額扣款列表增加傳出欄位：申購總價金、台幣申購總價金、加碼點(%)、加碼金額、減碼點(%)、減碼金額
    -- ============================================================= 網路/書面不定額 BNG ==============================================================
    OPEN DRSPDT FOR
    SELECT  ROWNUM AS ROW_NUM                                     -- 排列序號
           ,DataList.*
      FROM (
            -- EC 網路申購
            SELECT  PURCHASE.FUND_NO                              -- 基金代碼
                   ,FUND.NAME AS FUND_NAME                        -- 基金名稱
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO               -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                      -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                   -- 國際證券代碼
                   ,FUND.SHARE_CLASS                              -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME     -- 基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO               -- 基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME           -- 基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL      -- 基金計價幣別小數位數
                   ,PURCHASE.TX_DATE                              -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,'P9' AS PRODUCT_TYPE                          -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,ECS.F_GET_PRODUCT_NAME('P9') AS PRODUCT_NAME                   -- 產品類別名稱
                   -- 2018.09.07 tingting MOD BY 定額扣款列表、不定額扣款列表增加傳出欄位：每月扣款日
                   ,PURCHASE.AC_DAY                               -- 每月扣款日
                   ,PURCHASE.CURRENCY_NO AS TRADE_CRNCY           -- 付款幣別
                   ,CURRENCY.NAME AS TRADE_CRNCY_NAME             -- 付款幣別名稱
                   ,CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL     -- 付款幣別小數位數
                   -- 2018.10.09 ChengYu 增加傳出參數 風險屬性(RISK_CATEGORY)
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY           -- 風險屬性
                   -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
                   ,CASE WHEN FUND.CURRENCY_NO = PURCHASE.CURRENCY_NO THEN 1 
                         ELSE NVL(FAS_PURCHASE.EX_RATE,0)
                     END AS EX_RATE                               -- 參考匯率
                   ,PURCHASE.AMOUNT                               -- 原幣申購價金
                   -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤  
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示                 
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_AMOUNT IS NOT NULL THEN FAS_PURCHASE.NTD_AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.AMOUNT IS NOT NULL THEN FAS_PURCHASE.AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.AMOUNT
                         ELSE 0 
                     END AS NTD_AMOUNT                            -- 台幣申購價金
                   ,ROUND(PURCHASE.AMOUNT + PURCHASE.SERVICE_CHARGE, FUND_CURRENCY.AMT_DECIMAL) AS TOT_AMOUNT    -- 原幣申購總價金
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,ROUND(CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_AMOUNT IS NOT NULL THEN FAS_PURCHASE.NTD_AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.AMOUNT IS NOT NULL THEN FAS_PURCHASE.AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.AMOUNT
                               ELSE 0 
                           END + 
                          CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.NTD_SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.SERVICE_CHARGE
                               ELSE 0 
                           END, 0) AS NTD_TOT_AMOUNT   -- 台幣申購總價金
                   -- 2019.04.29 tingting 修正定額扣款列表、不定額扣款列表單位數錯誤
                   ,FAS_PURCHASE.SHARES                           -- 單位數
                   ,FUND.SHARE_DECIMAL                            -- 單位數小數位數
                   ,PURCHASE.AC_DATE                              -- 淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN PURCHASE.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV.NAV END NAV                     -- 淨值 
                   ,FUND.NAV_DECIMAL                              -- 淨值小數位數
                   ,PURCHASE.SERVICE_CHARGE                       -- 原幣手續費 
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.NTD_SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND FAS_PURCHASE.SERVICE_CHARGE IS NOT NULL THEN FAS_PURCHASE.SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' THEN PURCHASE.SERVICE_CHARGE
                         ELSE 0 
                     END AS NTD_SERVICE_CHARGE                    -- 台幣申購手續費
                   -- 2018.11.09 PEN MOD 網路定額、網路不定額列表 調整 處理中 的判斷條件
                   ,CASE WHEN PURCHASE.STATUS = 'C' OR PURCHASE.STATUS = 'O' THEN 'C'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE <> '00' THEN 'F'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE = '00' THEN 'S'
                         ELSE '' 
                     END AS STATUS                                -- 交易狀態代碼
                   ,CASE WHEN PURCHASE.STATUS = 'C' OR PURCHASE.STATUS = 'O' THEN '處理中'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE <> '00' THEN '交易失敗'
                         WHEN PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE = '00' THEN '交易成功'
                         ELSE '' 
                     END AS STATUS_NAME                           -- 交易狀態
                   ,PURCHASE.BROKER_NO                            -- 銷售機構代碼
                   ,PURCHASE.BRANCH_NO                            -- 銷售機構分行代碼
                   ,PURCHASE.SER_NO                               -- 成交序號
                   ,'4' AS PAY_TYPE                               -- 付款方式
                   ,'扣款' AS PAY_TYPE_NAME                       -- 付款方式名稱
                   -- 2018.12.05 JIANXIN 不定額百分比顯示調整
                   ,RSP.RAISE_ROI * 100 AS RAISE_ROI              -- 加碼點(%)
                   ,RSP.RAISE_AMOUNT                              -- 加碼金額
                   -- 2018.12.05 JIANXIN 不定額百分比顯示調整
                   ,RSP.REDUCE_ROI * 100 AS REDUCE_ROI            -- 減碼點(%)
                   ,RSP.REDUCE_AMOUNT                             -- 減碼金額
                   -- 2018.09.07 tingting MOD BY 1.申購列表' 定額扣款列表、不定額扣款列表 增加傳出欄位：扣款總行代碼、扣款分行代碼、扣款銀行簡稱、扣款帳號、扣款帳號後5碼
                   --                            2.調整申購列表 交易狀態 = S：交易成功 的取資料方式(Mark掉Union上面那段，從下面那段出資料)
                   ,PURCHASE.AC_BANK                              -- 扣款總行代碼
                   ,PURCHASE.AC_BRANCH                            -- 扣款分行代碼
                   ,FIS_BRANCH.SNAME AS  FIS_SNAME                -- 扣款銀行簡稱
                   ,PURCHASE.AC_CODE                              -- 扣款帳號
                   ,ECS.FN_STUFF_STR(PURCHASE.AC_CODE,5,'*') AS AC_CODE_5  -- 往來帳號後5碼
                   -- 2019.04.17 tingting 因應需求編號"ES_20190315_01"，新增輸出欄位
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE           -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME             -- 配息頻率名稱
                   ,PURCHASE.REVISION_DATE AS RCV_DATE            -- 委託日期(含時分秒)
                   ,PURCHASE.FEE_CHOICE AS FEE_CHOICE_TYPE        -- 優惠方案代碼
                   -- 2019.05.09 tingting 調整促銷活動名稱顯示
                   -- 2019.05.14 tingting 修正促銷活動名稱顯示(語法問題造成網路費率無法顯示)
                   -- 2024.08.01 richard begin 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
--                   -- 2024.05.24 richard begin 調整PURCHASE.FEE_CHOICE = '5' 由固定hard code改為,campaign.campagin_shnm
--                   --,CASE WHEN PURCHASE.FEE_CHOICE = '1' THEN CAMPAIGN.CAMPAIGN_SHNM
--                   ,CASE WHEN PURCHASE.FEE_CHOICE in ('1','5') THEN CAMPAIGN.CAMPAIGN_SHNM
--                   -- 2024.05.24 richard end 調整PURCHASE.FEE_CHOICE = '5' 由固定hard code改為,campaign.campagin_shnm
--                         WHEN PURCHASE.FEE_CHOICE = '2' THEN '優惠券' || RSP.COUPON_ID
--                         WHEN PURCHASE.FEE_CHOICE = '3' THEN '紅利折抵'
--                         -- 2024.05.24 richard begin 調整PURCHASE.FEE_CHOICE = '5' 由固定hard code改為,campaign.campagin_shnm
--                         --WHEN PURCHASE.FEE_CHOICE = '5' THEN '生日優惠'
--                         -- 2024.05.24 richard end 調整PURCHASE.FEE_CHOICE = '5' 由固定hard code改為,campaign.campagin_shnm
--                         WHEN PURCHASE.FEE_CHOICE IS NULL THEN '網路費率'
--                         END AS FEE_CHOICE_NAME                   -- 優惠方案名稱

                   -- 2024.08.01 richard begin 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
--                   ,CASE WHEN PURCHASE.FEE_CHOICE in ('1','5') THEN CAMPAIGN.CAMPAIGN_SHNM
--                         WHEN PURCHASE.FEE_CHOICE = '2' THEN '優惠券' || RSP.COUPON_ID
--                         WHEN PURCHASE.FEE_CHOICE = '3' THEN '紅利折抵'
                   -- 2024.08.01 richard begin 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
                   ,CASE WHEN PURCHASE.FEE_CHOICE in ('1','2','3','5') THEN CAMPAIGN.CAMPAIGN_SHNM
                         WHEN PURCHASE.FEE_CHOICE IS NULL THEN '網路費率'
                         END AS FEE_CHOICE_NAME                   -- 優惠方案名稱                         
                   -- 2024.08.01 richard end 調整PURCHASE.FEE_CHOICE in ('2','3') 由固定hard code改為,campaign.campagin_shnm
                   ,PURCHASE.PROJECT_CODE                         -- 促銷活動代碼
                   ,CAMPAIGN.CAMPAIGN_SHNM AS PROJECT_NAME        -- 促銷活動名稱
                   ,FUND.FUND_CATEGORY
                   ,FUND.AMC_NO
                   ,FUND.SITCA_FUND_TYPE
                   ,FUND.ORDINAL
              FROM FAS.EC_RSP_PURCHASE PURCHASE
              JOIN FAS.FUND
                ON PURCHASE.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON PURCHASE.CURRENCY_NO = CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               -- 2018.10.08 JIANXIN MOD BY 處理基金級別串接問題
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              LEFT JOIN FAS.PURCHASE FAS_PURCHASE
                ON FAS_PURCHASE.ID = PURCHASE.ID
               AND FAS_PURCHASE.FUND_NO = PURCHASE.FUND_NO
               AND FAS_PURCHASE.TX_DATE = PURCHASE.TX_DATE
               AND FAS_PURCHASE.BRANCH_NO = PURCHASE.BRANCH_NO
               AND FAS_PURCHASE.SER_NO = PURCHASE.SER_NO
               -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
              LEFT JOIN NAV.NAV
                ON NAV.FUND_NO = FUND.FUND_NO
               AND NAV.NAV_DATE = PURCHASE.AC_DATE
              LEFT JOIN ECS.RSP RSP
                ON RSP.ID = PURCHASE.ID
               AND RSP.ID_SEQ = PURCHASE.ID_SEQ
              LEFT JOIN FAS.FIS_BRANCH FIS_BRANCH
                ON FIS_BRANCH.BANK_NO = PURCHASE.AC_BANK
               AND FIS_BRANCH.BRANCH_NO = PURCHASE.AC_BRANCH
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ
              LEFT JOIN ECS.CAMPAIGN            -- 促銷活動
                ON CAMPAIGN.CAMPAIGN_CODE = PURCHASE.PROJECT_CODE
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE PURCHASE.ID = XID
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'3' ) AND PURCHASE.BRANCH_NO = '8400') -- 網路不定額 :8400
               -- 2018.11.09 PEN MOD 網路定額、網路不定額列表 調整 處理中 的判斷條件
               AND (  (TX_STATUS.TX_STATUS = 'S' AND PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE = '00') -- 交易成功：STATUS='P' AND RESULT_CODE = '00'；處理中：STATUS='C'；交易失敗：STATUS='P' AND RESULT_CODE <> '00'
                   OR (TX_STATUS.TX_STATUS = 'C' AND PURCHASE.STATUS IN ('C','O')) 
                   OR (TX_STATUS.TX_STATUS = 'F' AND PURCHASE.STATUS = 'P' AND PURCHASE.RESULT_CODE <> '00'))
               AND PURCHASE.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
           UNION 
           -- 2021.06.24 Chengyu 新增書面資料
           SELECT  PURCHASE.FUND_NO                              -- 基金代碼
                   ,FUND.NAME AS FUND_NAME                        -- 基金名稱
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO               -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                      -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                   -- 國際證券代碼
                   ,FUND.SHARE_CLASS                              -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME     -- 基金級別名稱
                   ,FUND.CURRENCY_NO AS CURRENCY_NO               -- 基金計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME           -- 基金計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL      -- 基金計價幣別小數位數
                   ,PURCHASE.TX_DATE                              -- 交易日期
                   -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
                   ,PERIODIC.PRODUCT_TYPE AS PRODUCT_TYPE         -- 產品類別：L1：單筆申購；L7：ROBO申購；P8：網路定額；P9：網路不定額；R1：買回；R7：ROBO買回
                   ,ECS.F_GET_PRODUCT_NAME(PERIODIC.PRODUCT_TYPE) AS PRODUCT_NAME                   -- 產品類別名稱
                   ,PURCHASE.AC_DAY                               -- 每月扣款日
                   ,PURCHASE.CURRENCY_NO AS TRADE_CRNCY           -- 付款幣別
                   ,CURRENCY.NAME AS TRADE_CRNCY_NAME             -- 付款幣別名稱
                   ,CURRENCY.AMT_DECIMAL AS TRADE_AMT_DECIMAL     -- 付款幣別小數位數
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY           -- 風險屬性
                   ,CASE WHEN FUND.CURRENCY_NO = PURCHASE.CURRENCY_NO THEN 1 
                         ELSE NVL(PURCHASE.EX_RATE,0)
                     END AS EX_RATE                               -- 參考匯率
                   ,PURCHASE.AMOUNT                               -- 原幣申購價金    
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示         
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_AMOUNT IS NOT NULL THEN PURCHASE.NTD_AMOUNT
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.AMOUNT IS NOT NULL THEN PURCHASE.AMOUNT
                         ELSE 0 
                     END AS NTD_AMOUNT                            -- 台幣申購價金
                   ,ROUND(PURCHASE.AMOUNT + PURCHASE.SERVICE_CHARGE, FUND_CURRENCY.AMT_DECIMAL) AS TOT_AMOUNT    -- 原幣申購總價金
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,ROUND(CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_AMOUNT IS NOT NULL THEN PURCHASE.NTD_AMOUNT
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.AMOUNT IS NOT NULL THEN PURCHASE.AMOUNT
                               ELSE 0 
                           END + 
                          CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN PURCHASE.NTD_SERVICE_CHARGE
                               WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.SERVICE_CHARGE IS NOT NULL THEN PURCHASE.SERVICE_CHARGE
                               ELSE 0 
                           END, 0) AS NTD_TOT_AMOUNT   -- 台幣申購總價金
                   ,PURCHASE.SHARES                               -- 單位數
                   ,FUND.SHARE_DECIMAL                            -- 單位數小數位數
                   ,PURCHASE.AC_DATE                              -- 淨值日期
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN PURCHASE.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV.NAV END NAV                     -- 淨值 
                   ,FUND.NAV_DECIMAL                              -- 淨值小數位數
                   ,PURCHASE.SERVICE_CHARGE                       -- 原幣手續費 
                   -- 2021.09.03 Chengyu 1.調整申購金額顯示
                   ,CASE WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.NTD_SERVICE_CHARGE IS NOT NULL THEN PURCHASE.NTD_SERVICE_CHARGE
                         WHEN PURCHASE.CURRENCY_NO = 'NTD' AND PURCHASE.SERVICE_CHARGE IS NOT NULL THEN PURCHASE.SERVICE_CHARGE
                         ELSE 0 
                     END AS NTD_SERVICE_CHARGE                    -- 台幣申購手續費
                   -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
                   ,'S' STATUS                                    -- 交易狀態代碼
                   ,'交易成功' AS STATUS_NAME                     -- 交易狀態
                   ,PERIODIC.BROKER_NO                            -- 銷售機構代碼
                   ,PERIODIC.BRANCH_NO                            -- 銷售機構分行代碼
                   ,CAST(PERIODIC.SER_NO AS VARCHAR2(10)) SER_NO  -- 成交序號
                   ,'4' AS PAY_TYPE                               -- 付款方式
                   ,'扣款' AS PAY_TYPE_NAME                       -- 付款方式名稱
                   ,ROI_RATE  AS RAISE_ROI                        -- 加碼點(%)
                   ,PERIODIC.RAISE_AMOUNT AS RAISE_AMOUNT         -- 加碼金額
                   ,0 AS REDUCE_ROI                               -- 減碼點(%)
                   ,0 AS REDUCE_AMOUNT                            -- 減碼金額
                   ,PURCHASE.AC_BANK                              -- 扣款總行代碼
                   ,'' AS AC_BRANCH                               -- 扣款分行代碼(後台未記錄扣款當下分行)
                   ,FIS_BANK.SNAME AS  FIS_SNAME                  -- 扣款銀行簡稱
                   ,PURCHASE.AC_CODE                              -- 扣款帳號
                   ,ECS.FN_STUFF_STR(PURCHASE.AC_CODE,5,'*') AS AC_CODE_5  -- 往來帳號後5碼
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE           -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME             -- 配息頻率名稱
                   ,TO_DATE('1900/01/01','YYYY/MM/DD') AS RCV_DATE            -- 委託日期(含時分秒)
                   ,''AS FEE_CHOICE_TYPE                          -- 優惠方案代碼
                   ,''AS FEE_CHOICE_NAME                          -- 優惠方案名稱
                   ,'' AS PROJECT_CODE                            -- 促銷活動代碼(書面定額促銷活動處理方式不同)
                   ,'' AS PROJECT_NAME                            -- 促銷活動名稱(書面定額促銷活動處理方式不同)
                   ,FUND.FUND_CATEGORY
                   ,FUND.AMC_NO
                   ,FUND.SITCA_FUND_TYPE
                   ,FUND.ORDINAL
              FROM FAS.PERIODIC_PURCHASE PURCHASE
              JOIN FAS.PERIODIC
                ON PERIODIC.ID = PURCHASE.ID
               AND PERIODIC.ID_SEQ = PURCHASE.ID_SEQ
              JOIN FAS.FUND
                ON PURCHASE.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON PURCHASE.CURRENCY_NO = CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              LEFT JOIN NAV.NAV
                ON NAV.FUND_NO = FUND.FUND_NO
               AND NAV.NAV_DATE = PURCHASE.AC_DATE
              LEFT JOIN FAS.FIS_BANK FIS_BANK
                ON FIS_BANK.BANK_NO = PURCHASE.AC_BANK
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ              
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             CROSS JOIN (SELECT VALUE1 TX_STATUS
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
                         ) TX_STATUS
             WHERE PURCHASE.ID = XID
               -- 2021.07.19 Chengyu 調整書面與ROBO資料取法
               AND (TX_TYPE.TX_TYPE IN ('ALL' ,'3' )  AND PERIODIC.PRODUCT_TYPE = 'P3') 
               -- 2021.07.21 Chengyu 調整書面與ROBO資料取法(只取交易成功資料)
               -- 書面定額交易成功： 只顯示成功資料，FAS.PERIODIC_PURCHASE.RESULT_CODE = '00'  AND PERIODIC_PURCHASE.shares > 0
               AND (TX_STATUS.TX_STATUS = 'S' AND PURCHASE.RESULT_CODE = '00'  AND PURCHASE.SHARES > 0)
               AND PURCHASE.TX_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
           ) DataList
      -- 2018.10.25 tingting 1.增加傳出欄位：排列序號
      --                     2.調整排序：先依交易日期、交易狀態(處理中→交易成功→交易失敗)
      --                                再依基金排序規則(境內外、基金公司、投信顧公會基金類型、母基金排序、基金級別、子基金排序)
      ORDER BY TX_DATE,                          --交易日期
               CASE WHEN STATUS = 'C' THEN 1     --處理中
                    WHEN STATUS = 'S' THEN 2     --交易成功
                    WHEN STATUS = 'F' THEN 3     --交易失敗
                    ELSE 4 END,                           --交易狀態
               FUND_CATEGORY, AMC_NO, SITCA_FUND_TYPE, ORDINAL, SHARE_CLASS NULLS FIRST, ORDINAL;
    -- ============================================================= 網路/書面不定額 END =============================================================

    -- ============================================================== 收益分配 BNG ==============================================================
    OPEN DIVDENDDT FOR
    SELECT  ROWNUM AS ROW_NUM                                                                   -- 排列序號
           ,DataList.*
      FROM (
            SELECT  DIVIDEND_DETAIL.DIVIDEND_DATE AS DIVIDEND_DATE                              -- 收益分配日
                   -- 2018.12.03 JIANXIN MOD BY 調整 發放日 由 DIVIDEND 取得
                   ,DIVIDEND.PAYMENT_DATE AS PAYMENT_DATE                                       -- 收益分配發放日
                   ,DIVIDEND_DETAIL.FUND_NO AS FUND_NO                                          -- 基金別
                   ,FUND.NAME AS FUND_NAME                                                      -- 基金名稱
                   ,FUND.DIVIDEND_DESC AS FUND_MEMO                                             -- 基金警語
                   ,FUND.SNAME AS FUND_SNAME                                                    -- 基金簡稱
                   ,FUND.ISIN_CODE AS ISIN_CODE                                                 -- 國際證券代碼
                   ,FUND.SHARE_CLASS AS SHARE_CLASS                                             -- 基金級別
                   ,FUND_SHARE_CLASS.NAME AS SHARE_CLASS_NAME                                   -- 基金級別名稱
                   -- 2018.12.06 JIANXIN 調整配息內容交易幣別與計價幣別回傳錯誤
                   -- 2018.12.18 JIANXIN 調整配息的內容
                   -- DIVIDEND_DETAIL.ORG_CURRENCY_NO 為憑證檔中的交易幣別
                   -- DIVIDEND_DETAIL.CURRENCY_NO 為收益分配的付款幣別(包含FCY 或 MMA)
                   -- 2018.12.19 JIANXIN 依據 與瀚亞Ada討論，邏輯調整如下:
                   --                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO <> 計價幣 則 回傳 NTD_AMOUT
                   --                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO = 計價幣 則 回傳 AMOUT
                   --                    配息=再申購：金額 回傳 AMOUT
                   --                    配息=再申購：交易幣別 回傳 計價幣別
                   ,CASE WHEN DIVIDEND_DETAIL.PAY_TYPE = '2' THEN FUND.CURRENCY_NO 
                         ELSE DIVIDEND_DETAIL.ORG_CURRENCY_NO
                     END AS TRADE_CRNCY                                                         -- 申購幣別 
                   ,CASE WHEN DIVIDEND_DETAIL.PAY_TYPE = '2' THEN FUND_CURRENCY.NAME  
                         ELSE CURRENCY.NAME 
                     END AS TRADE_CRNCY_NAME                                                    -- 申購幣別名稱
                   ,CASE WHEN DIVIDEND_DETAIL.PAY_TYPE = '2' THEN FUND_CURRENCY.AMT_DECIMAL
                         ELSE CURRENCY.AMT_DECIMAL
                     END AS TRADE_AMT_DECIMAL                                                   -- 申購幣別小數位數
                   ,FUND.CURRENCY_NO AS CURRENCY_NO                                             -- 計價幣別
                   ,FUND_CURRENCY.NAME AS CURRENCY_NAME                                         -- 計價幣別名稱
                   ,FUND_CURRENCY.AMT_DECIMAL AS AMT_DECIMAL                                    -- 計價幣別小數位數
                   -- 2018.10.09 ChengYu 增加傳出參數 風險屬性(RISK_CATEGORY)
                   ,FUND.RISK_CATEGORY AS RISK_CATEGORY                                         -- 風險屬性
                   ,FUND.SHARE_DECIMAL AS SHARE_DECIMAL                                         -- 單位數小數位數
                   ,FUND.NAV_DECIMAL AS NAV_DECIMAL                                             -- 淨值小數位數
                   ,DIVIDEND_DETAIL.SHARES AS SHARES                                            -- 持有單位數
                   -- 2018.12.03 JIANXIN MOD BY 調整 每單位分配金額 由 DIVIDEND 取得
                   ,DIVIDEND.DIVIDEND_RATE AS ASSIGN_VALUE                                      -- 每單位分配金額
                   ,DIVIDEND_DETAIL.AMOUNT AS AMOUNT                                            -- 總分配金額(計價幣別)
                   ,DIVIDEND.EX_RATE AS EX_RATE                                                 -- 匯率
                   -- 2018.12.03 JIANXIN MOD BY 調整 總分配金額 取得邏輯
                   -- 2018.12.19 JIANXIN 依據 與瀚亞Ada討論，邏輯調整如下:
                   --                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO <> 計價幣 則 回傳 NTD_AMOUT
                   --                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO = 計價幣 則 回傳 AMOUT
                   --                    配息=再申購：金額 回傳 AMOUT
                   --                    配息=再申購：交易幣別 回傳 計價幣別
                   ,CASE WHEN DIVIDEND_DETAIL.PAY_TYPE = '2' THEN DIVIDEND_DETAIL.AMOUNT
                         WHEN DIVIDEND_DETAIL.PAY_TYPE IN ('3','4') AND DIVIDEND_DETAIL.ORG_CURRENCY_NO <> FUND.CURRENCY_NO THEN DIVIDEND_DETAIL.NTD_AMOUNT
                         WHEN DIVIDEND_DETAIL.PAY_TYPE IN ('3','4') AND DIVIDEND_DETAIL.ORG_CURRENCY_NO = FUND.CURRENCY_NO  THEN DIVIDEND_DETAIL.AMOUNT
                         ELSE DIVIDEND_DETAIL.AMOUNT 
                     END AS TX_AMOUNT                                                           -- 總分配金額(申購幣別)
                   -- 2018.12.18 JIANXIN 調整配息相關費用的內容
                   -- 2018.12.19 JIANXIN 依據 與瀚亞Ada討論，邏輯調整如下:
                   --                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO <> 計價幣 則 回傳 NTD_AMOUT
                   --                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO = 計價幣 則 回傳 AMOUT
                   --                    配息=再申購：金額 回傳 AMOUT
                   --                    配息=再申購：交易幣別 回傳 計價幣別
                   ,CASE WHEN DIVIDEND_DETAIL.PAY_TYPE = '2' THEN DIVIDEND_DETAIL.WIRE_FEE
                         WHEN DIVIDEND_DETAIL.PAY_TYPE IN ('3','4') AND DIVIDEND_DETAIL.ORG_CURRENCY_NO <> FUND.CURRENCY_NO THEN DIVIDEND_DETAIL.NTD_WIRE_FEE
                         WHEN DIVIDEND_DETAIL.PAY_TYPE IN ('3','4') AND DIVIDEND_DETAIL.ORG_CURRENCY_NO = FUND.CURRENCY_NO  THEN DIVIDEND_DETAIL.WIRE_FEE
                         ELSE DIVIDEND_DETAIL.WIRE_FEE 
                     END AS WIRE_FEE                                                            -- 相關費用(申購幣別)
                   -- 2018.12.03 JIANXIN MOD BY 調整 實際分配淨額 取得邏輯
                   -- 2018.12.19 JIANXIN 依據 與瀚亞Ada討論，邏輯調整如下:
                   --                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO <> 計價幣 則 回傳 NTD_AMOUT
                   --                    配息=匯款：幣別 回傳 ORG_CURRENCY_NO； ORG_CURRENCY_NO = 計價幣 則 回傳 AMOUT
                   --                    配息=再申購：金額 回傳 AMOUT
                   --                    配息=再申購：交易幣別 回傳 計價幣別
                   ,CASE WHEN DIVIDEND_DETAIL.PAY_TYPE = '2' THEN DIVIDEND_DETAIL.TOTAL_RETURN
                         WHEN DIVIDEND_DETAIL.PAY_TYPE IN ('3','4') AND DIVIDEND_DETAIL.ORG_CURRENCY_NO <> FUND.CURRENCY_NO THEN DIVIDEND_DETAIL.NTD_TOTAL_RETURN
                         WHEN DIVIDEND_DETAIL.PAY_TYPE IN ('3','4') AND DIVIDEND_DETAIL.ORG_CURRENCY_NO = FUND.CURRENCY_NO  THEN DIVIDEND_DETAIL.TOTAL_RETURN
                         ELSE DIVIDEND_DETAIL.TOTAL_RETURN 
                     END AS TOTAL_RETURN                                                        -- 實際分配淨額(申購幣別)
                   ,DIVIDEND_DETAIL.PAY_TYPE AS PAY_TYPE                                        -- 收益分配方式
                   ,CASE WHEN DIVIDEND_DETAIL.PAY_TYPE = '1' THEN '郵寄支票'                    
                         WHEN DIVIDEND_DETAIL.PAY_TYPE = '2' THEN '轉申購'                      
                         WHEN DIVIDEND_DETAIL.PAY_TYPE = '3' THEN '匯款'                        
                         WHEN DIVIDEND_DETAIL.PAY_TYPE = '4' THEN '匯款 (不受最低金額限制) '    
                         ELSE ''                                                                
                     END AS PAY_TYPE_NAME                                                       -- 收益分配方式名稱
                   ,DIVIDEND_DETAIL.CHEQUE AS CHEQUE                                            -- 郵寄支票號碼
                   ,DIVIDEND_DETAIL.PAY_ADDRESS AS PAY_ADDRESS                                  -- 郵寄地址
                   ,PURCHASE.TX_DATE AS SWITCH_DATE                                             -- 再申購日期
                   ,PURCHASE.AMOUNT AS AMOUNT_IN                                                -- 再申購金額
                   ,PURCHASE.SERVICE_CHARGE AS SERVICE_CHARGE_IN                                -- 再申購手續費
                   -- 2021.10.18 Chengyu 調整淨值資料取法(若交易淨值日期在基金成立日前，淨值傳出10)
                   ,CASE WHEN PURCHASE.AC_DATE < FUND.INCEPTION_DATE THEN 10 
                         ELSE NAV.NAV END AS NAV_IN                                             -- 再申購淨值
                   ,PURCHASE.SHARES AS SHARES_IN                                                -- 再申購單位數
                   ,DIVIDEND_DETAIL.AC_NAME AS AC_NAME                                          -- 匯款銀行戶名
                   ,DIVIDEND_DETAIL.SNAME AS SNAME                                              -- 匯款銀行名稱
                   ,DIVIDEND_DETAIL.BANK_NO AS BANK_NO                                          -- 匯款銀行代碼
                   ,DIVIDEND_DETAIL.BRANCH_NO AS BRANCH_NO                                      -- 匯款銀行分行代碼
                   -- 2018.08.23 JIANXIN MOD BY 新增帳號隱碼函數
                   ,ECS.FN_STUFF_STR(DIVIDEND_DETAIL.AC_CODE,5,'*') AS AC_CODE_5                -- 往來帳號後5碼
                   -- 2019.04.17 tingting 因應需求編號"ES_20190315_01"，新增輸出欄位
                   ,CASE WHEN FUND.DIVIDEND_FREQ = 'N' THEN 'N' ELSE 'Y' END AS IS_DIVIDEND     -- 是否為配息
                   ,FUND.DIVIDEND_FREQ AS DIVIDEND_TYPE                                         -- 配息頻率
                   ,ECS_OPTION_DIVIDEND_FREQ.DISPLAY_NAME AS DIVIDEND_TYPE_NAME                 -- 配息頻率名稱
                   -- 2021.09.24 Chengyu 新增欄位除息日
                   ,DIVIDEND.NEXT_DIVIDEND_DATE                                                 -- 除息日
              FROM FAS.DIVIDEND_DETAIL 
              JOIN FAS.DIVIDEND
                ON DIVIDEND_DETAIL.DIVIDEND_DATE = DIVIDEND.DIVIDEND_DATE
               AND DIVIDEND_DETAIL.FUND_NO = DIVIDEND.FUND_NO
               AND DIVIDEND_DETAIL.DIVIDEND_TYPE = DIVIDEND.DIVIDEND_TYPE
              JOIN FAS.FUND 
                ON DIVIDEND_DETAIL.FUND_NO = FUND.FUND_NO
              JOIN ECS.V_FUND_CRNCY FUND_CURRENCY
                ON FUND.CURRENCY_NO = FUND_CURRENCY.CURRENCY_NO
              JOIN ECS.V_FUND_CRNCY CURRENCY
                ON DIVIDEND_DETAIL.ORG_CURRENCY_NO = CURRENCY.CURRENCY_NO
              LEFT JOIN ECS.FUND_SHARE_CLASS 
                ON FUND_SHARE_CLASS.SHARE_CLASS = FUND.SHARE_CLASS
               -- 2018.10.08 JIANXIN MOD BY 處理基金級別串接問題
               AND FUND.AMC_NO = FUND_SHARE_CLASS.AMC_NO
              LEFT JOIN FAS.PURCHASE
                ON PURCHASE.ID = DIVIDEND_DETAIL.ID 
               AND PURCHASE.FUND_NO = DIVIDEND.FUND_NO
               -- 2021.10.14 Chengyu 調整分配串接FAS.PURCHASE邏輯(TX_DATE)
               -- 2021.10.28 Chengyu 調整分配串接FAS.PURCHASE邏輯(TX_DATE)
               AND DIVIDEND_DETAIL.DIVIDEND_YEAR IN (TO_CHAR(PURCHASE.TX_DATE,'yyyy'), TO_CHAR(PURCHASE.TX_DATE,'yyyy')-1)
               -- 2021.09.22 Chengyu 修正轉申購時的申購資料串接語法
               AND PURCHASE.SER_NO = DIVIDEND_DETAIL.PUR_SER_NO
               -- 2022.04.21 JIANXIN 調整轉申購配息不能用PUR_SER_NO (因為有一些是空的)
               AND PURCHASE.TX_DATE BETWEEN DIVIDEND_DETAIL.PAYMENT_DATE AND TRUNC(DIVIDEND_DETAIL.PAYMENT_DATE + 20 )
               AND PURCHASE.BROKER_NO = '999'
               -- 2018.08.22 JIANXIN MOD BY 調整收益分配 BRANCH_NO = '7000'
               AND PURCHASE.BRANCH_NO = '7000'
               -- 2019.05.03 tingting 修正單筆申購列表，匯率、台幣申購價金、台幣申購手續費錯誤、淨值錯誤
              LEFT JOIN NAV.NAV
                ON NAV.FUND_NO = PURCHASE.FUND_NO
               AND NAV.NAV_DATE = PURCHASE.AC_DATE
              LEFT JOIN ECS.ECS_OPTION ECS_OPTION_DIVIDEND_FREQ     -- 欄位選單資料(配息頻率)
                ON ECS_OPTION_DIVIDEND_FREQ.SOURCE_TYPE = '054'
               AND ECS_OPTION_DIVIDEND_FREQ.TEXT_VALUE = FUND.DIVIDEND_FREQ
             CROSS JOIN (SELECT VALUE1 TX_TYPE
                           FROM TABLE(ECS.F_FormatStringListToTable(WTX_TYPE))
                         ) TX_TYPE
             -- 2018.09.07 JIANXIN MOD BY 因應系統資料無狀態欄位，進行調整
             --CROSS JOIN (SELECT VALUE1 TX_STATUS
             --              FROM TABLE(ECS.F_FormatStringListToTable(WTX_STATUS))
             --            ) TX_STATUS
             WHERE DIVIDEND_DETAIL.ID = XID
               AND TX_TYPE.TX_TYPE IN ('ALL' ,'6' )  -- 收益分配
               -- 2018.09.07 JIANXIN MOD BY 因應系統資料無狀態欄位，進行調整
               --AND (  (TX_STATUS.TX_STATUS = 'S' AND DIVIDEND_DETAIL.STATUS = 'C') -- 交易成功：STATUS='C' ；處理中：STATUS='O'
               --    OR (TX_STATUS.TX_STATUS = 'C' AND DIVIDEND_DETAIL.STATUS = 'O') )
               -- 2018.09.24 JIANXIN MOD BY 調整日期為配息日
               AND DIVIDEND_DETAIL.DIVIDEND_DATE BETWEEN WTX_DATE_FROM AND WTX_DATE_TO
             -- 2018.10.25 tingting 1.增加傳出欄位：排列序號
             --                     2.調整排序：先依交易日期、交易狀態(處理中→交易成功→交易失敗)
             --                                再依基金排序規則(境內外、基金公司、投信顧公會基金類型、母基金排序、基金級別、子基金排序)
             ORDER BY DIVIDEND_DATE,                    --收益分配日
                      FUND.FUND_CATEGORY, FUND.AMC_NO, FUND.SITCA_FUND_TYPE, FUND.ORDINAL, FUND.SHARE_CLASS NULLS FIRST, FUND.ORDINAL,DIVIDEND.NEXT_DIVIDEND_DATE
           ) DataList; 
    -- ============================================================== 收益分配 END ==============================================================

    -- ============================================================== 錯誤訊息 BNG ==============================================================
    OPEN ERRDT FOR
    SELECT  XWARNS_TYPE AS WARNS_TYPE
           ,XERROR_MSG AS ERROR_MSG
      FROM DUAL;
    -- ============================================================== 錯誤訊息 END ==============================================================

END;

/
