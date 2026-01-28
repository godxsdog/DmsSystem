# FAS6562 月報資料轉入程式說明

## 📋 檔案清單

1. **monthly_data.txt** - PowerBuilder 主程式碼
2. **CREATE_TABLES.sql** - 資料表建立語法
3. **Letter of Declaration with ISIN 月報_10.2025_IT.xlsx** - FILE 06 來源檔案
4. **SITCA AUM 1025月報_IT.xlsx** - FILE 07 來源檔案

## 🔧 已修復的錯誤

### 1. **變數宣告問題**
- ✅ 將 FILE06 & FILE07 使用的變數提升到檔案開頭統一宣告
- ✅ 移除循環內的重複變數宣告
- ✅ 新增變數：`ls_fund_name`, `ls_fund_abbr`, `ls_fbc`, `li_no` 等

### 2. **界面控件引用**
- ✅ 註解掉 `sle_file06.text` 和 `sle_file07.text` 的引用
- 💡 如需顯示檔案路徑，請在 PowerBuilder 界面中定義這兩個文本框控件

### 3. **資料庫表格**
- ✅ 提供完整的 CREATE TABLE 語法（`CREATE_TABLES.sql`）
- ✅ 定義所有必要的欄位、索引和註解
- 💡 請先執行 CREATE TABLE 語法建立表格後再執行程式

## 📊 資料表結構

### fas.new_agent_6562_temp1 (Letter of Declaration)

| 欄位名稱 | 類型 | 說明 |
|---------|------|------|
| fund_no | VARCHAR2(10) | 基金代號 (主鍵) |
| tx_date | DATE | 交易日期 (主鍵) |
| isin_code | VARCHAR2(20) | ISIN代碼 |
| fund_name | VARCHAR2(200) | 基金名稱 |
| fund_size | NUMBER(20,2) | 基金規模(USD) |
| derivative_enhance | NUMBER(20,2) | 衍生工具部位-強化績效 |
| limit_40_pct | NUMBER(10,4) | 40%限制比率 |
| derivative_hedge | NUMBER(20,2) | 衍生工具部位-避險目的 |
| denominator | NUMBER(20,2) | 分母(相關證券市值) |
| limit_100_pct | NUMBER(10,4) | 100%限制比率 |
| invest_taiwan | NUMBER(20,2) | 台灣投資金額 |
| limit_50_pct | NUMBER(10,4) | 50%限制比率 |
| invest_china | NUMBER(20,2) | 中國證券投資金額 |
| limit_20_pct | NUMBER(10,4) | 20%限制比率 |

### fas.new_agent_6562_temp2 (SITCA AUM)

| 欄位名稱 | 類型 | 說明 |
|---------|------|------|
| fund_no | VARCHAR2(10) | 基金代號 (主鍵) |
| tx_date | DATE | 交易日期 (主鍵) |
| fund_abbr | VARCHAR2(50) | 基金簡稱 |
| seq_no | NUMBER(10) | 序號 |
| fund_name | VARCHAR2(200) | 基金名稱 |
| isin_code | VARCHAR2(20) | ISIN代碼 |
| fbc | VARCHAR2(10) | 基金幣別 |
| fund_size_product_ccy | NUMBER(20,2) | 基金規模(產品幣別) |
| fx_rate | NUMBER(10,5) | 匯率 |
| nav_reporting_ccy | NUMBER(20,4) | NAV(報告幣別) |
| invest_amt_taiwan_usd | NUMBER(20,2) | 台灣投資人投資金額(USD) |
| fund_size_by_share_class | NUMBER(20,2) | 分級別基金規模(USD) |
| fund_size_global_aa | NUMBER(20,2) | 全球基金規模AA(USD) |
| invest_pct | NUMBER(10,4) | 台灣投資人占比 |

## 🚀 使用步驟

### 1. 建立資料表
```sql
-- 在 Oracle 中執行
@CREATE_TABLES.sql
```

### 2. 確認界面控件（選用）
如需顯示選擇的檔案路徑，請在 PowerBuilder 界面中新增：
- `sle_file06` (SingleLineEdit) - 顯示 Letter of Declaration 檔案路徑
- `sle_file07` (SingleLineEdit) - 顯示 SITCA AUM 檔案路徑

### 3. 執行程式
1. 勾選 `cbx_6` 來處理 Letter of Declaration 檔案
2. 勾選 `cbx_7` 來處理 SITCA AUM 檔案
3. 選擇對應的 Excel 檔案
4. 點擊執行按鈕

## 📝 程式流程

### FILE 06 處理流程
1. 開啟 Letter of Declaration Excel 檔案
2. 從第3行開始讀取（第1行是日期，第2行是表頭）
3. 讀取 Column A-L 的資料
4. 根據 ISIN 查詢基金編號
5. INSERT 或 UPDATE 到 `fas.new_agent_6562_temp1`

### FILE 07 處理流程
1. 開啟 SITCA AUM Excel 檔案
2. 從第3行開始讀取（第1行是標題，第2行是表頭）
3. 讀取 Column A-L 的資料（注意 ISIN 在 Column D）
4. 根據 ISIN 查詢基金編號
5. INSERT 或 UPDATE 到 `fas.new_agent_6562_temp2`

## ⚠️ 注意事項

1. **Excel 格式**：確保 Excel 檔案格式與程式中定義的欄位位置一致
2. **日期格式**：交易日期 (ld_tx_date) 從界面的 `sle_month.text` 取得
3. **基金資料**：程式會根據 ISIN 從 `fas.fund` 查詢基金編號
4. **錯誤處理**：如果 ISIN 查詢不到對應基金，該筆資料會被跳過
5. **空白行處理**：連續超過 20 行空白會停止讀取

## 🔍 查詢資料範例

```sql
-- 查詢 Letter of Declaration 資料
SELECT *
  FROM fas.new_agent_6562_temp1
 WHERE tx_date = TO_DATE('2025-10-31', 'YYYY-MM-DD')
 ORDER BY fund_no;

-- 查詢 SITCA AUM 資料
SELECT *
  FROM fas.new_agent_6562_temp2
 WHERE tx_date = TO_DATE('2025-10-31', 'YYYY-MM-DD')
 ORDER BY seq_no;

-- 統計資料筆數
SELECT 'temp1' AS table_name, COUNT(*) AS record_count
  FROM fas.new_agent_6562_temp1
 WHERE tx_date = TO_DATE('2025-10-31', 'YYYY-MM-DD')
UNION ALL
SELECT 'temp2', COUNT(*)
  FROM fas.new_agent_6562_temp2
 WHERE tx_date = TO_DATE('2025-10-31', 'YYYY-MM-DD');
```

## 📞 技術支援

如有問題，請確認：
1. ✅ 資料表已正確建立
2. ✅ Excel 檔案格式正確
3. ✅ `fas.fund` 表中有對應的 ISIN 資料
4. ✅ 使用者有足夠的資料庫權限

---

**最後更新日期**：2025-01-28
