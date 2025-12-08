# 配息功能 Review 報告

## 📅 Review 日期
2024-12-XX

## 📋 現況分析

### 已實作功能

#### ✅ Step 3: dms5A1 可分配餘額維護作業（部分實作）

**已實作**：
- ✅ CSV 檔案匯入功能（`DividendService.ImportAsync`）
- ✅ 基本配息計算（`DividendService.ConfirmAsync`）
- ✅ 資料庫更新（MDS.FUND_DIV）

**檔案位置**：
- `DmsSystem.Application/Services/DividendService.cs`
- `DmsSystem.Application/Interfaces/IDividendService.cs`
- `DmsSystem.Api/Controllers/DividendsController.cs`

---

## 🔍 發現的問題

### 1. 計算邏輯與業務需求不一致

#### 問題 1.1：配息率計算方式錯誤

**現有程式碼**（`DividendService.ConfirmAsync` 第 222-229 行）：
```csharp
decimal divRateM = dividendType switch
{
    "M" => divRate,
    "Q" => Math.Round(divRate / 3m, 6),
    "S" => Math.Round(divRate / 6m, 6),
    "Y" => Math.Round(divRate / 12m, 6),
    _ => divRate
};
```

**問題**：
- `divRate` 已經是「每單位可分配收益」（年化），不應再除以頻率係數
- 根據業務邏輯，`divRateM` 應該是「每單位配息金額」（當期），而非年化配息率除以頻率

**正確邏輯**（根據 `配息功能開發指南.md`）：
- `divRate` = 每單位可分配收益（年化）
- `divRateM` = 每單位配息金額（當期）= `divRate / freqFactor * nav`
- 其中 `freqFactor` = M:12, Q:4, S:2, Y:1

#### 問題 1.2：缺少目標配息率查詢

**現有程式碼**：直接使用計算出的 `divRate`，未查詢 `MDS.FUND_DIV_OBJ` 表

**應有邏輯**：
1. 查詢 `MDS.FUND_DIV_OBJ` 取得目標配息率或目標配息金額
2. 若設定 `div_obj_amt`，優先採用
3. 若設定 `div_obj`，換算為當期配息金額
4. 若未設定，使用實際可分配收益

#### 問題 1.3：缺少本金補足判斷

**現有程式碼**：`CAPITAL_RATE = 0`（固定為 0）

**應有邏輯**：
1. 查詢 `MDS.FUND_DIV_SET` 取得 `capital_type`
2. 若 `capital_type = 'Y'` 且 `divRate < divRateObj`，計算本金補足金額
3. 若 `capital_type = 'N'`，下修配息金額至可分配金額

#### 問題 1.4：缺少收益分攤邏輯

**現有程式碼**：未實作收益分攤

**應有邏輯**：
1. 查詢 `MDS.FUND_DIV_SET` 取得 `item01_seq` ~ `item10_seq`
2. 依順序將配息金額分攤至各收益項目（PRE_DIV1~5, DIV1~5）
3. 更新 `PRE_DIV1_RATE_M` ~ `PRE_DIV5_RATE_M`、`DIV1_RATE_M` ~ `DIV5_RATE_M`

---

### 2. 缺少必要功能

#### ❌ Step 1: mds1511 配息參數設定

**狀態**：未實作

**需要實作**：
- Entity: `FundDivSet`
- Repository: `IFundDivSetRepository`
- Service: `IFundDivSetService`
- Controller: `FundDivSetController`
- API: GET/POST 配息參數設定

#### ❌ Step 2: mds1512 目標配息率設定

**狀態**：未實作

**需要實作**：
- Entity: `FundDivObj`
- Repository: `IFundDivObjRepository`
- Service: `IFundDivObjService`
- Controller: `FundDivObjController`
- API: GET/POST 目標配息率設定

#### ❌ Step 4: dms5A3 境內配息組成資訊上傳EC

**狀態**：未實作

#### ❌ Step 5: mds153 境內配息資訊概況總表轉出

**狀態**：未實作

---

### 3. 資料表 Entity 缺失

**現況**：使用 Dapper 直接操作資料庫，未建立 Entity

**建議**：
- 使用 EF Core Scaffold 產生 Entity
- 或手動建立 Entity 類別

**需要的 Entity**：
- `FundDiv`（MDS.FUND_DIV）
- `FundDivSet`（MDS.FUND_DIV_SET）
- `FundDivObj`（MDS.FUND_DIV_OBJ）
- `FundDividend`（MDS.FUND_DIVIDEND）

---

### 4. 程式碼品質問題

#### 問題 4.1：硬編碼 SQL 語句

**現況**：SQL 語句直接寫在 Service 中

**建議**：
- 將 SQL 語句移至 Repository 層
- 或使用 EF Core 的 LINQ 查詢

#### 問題 4.2：缺少錯誤處理

**現況**：部分錯誤僅記錄日誌，未回傳詳細錯誤訊息

**建議**：
- 加強錯誤處理
- 提供更詳細的錯誤訊息

#### 問題 4.3：缺少資料驗證

**現況**：CSV 匯入時缺少資料驗證

**建議**：
- 添加資料驗證邏輯
- 驗證基金代號、日期格式、金額範圍等

---

## ✅ 改進建議

### 優先順序 1：修正現有計算邏輯

1. **修正配息率計算**
   - 正確計算 `divRateM`（每單位配息金額）
   - 加入目標配息率查詢邏輯
   - 加入本金補足判斷

2. **加入收益分攤邏輯**
   - 查詢配息參數設定
   - 依順序分攤配息金額

### 優先順序 2：補齊缺失功能

1. **實作 Step 1（配息參數設定）**
   - 建立 Entity、Repository、Service、Controller
   - 實作 CRUD API

2. **實作 Step 2（目標配息率設定）**
   - 建立 Entity、Repository、Service、Controller
   - 實作 CRUD API

### 優先順序 3：改善程式碼品質

1. **建立 Entity 類別**
   - 使用 EF Core Scaffold 或手動建立

2. **重構 Repository 層**
   - 將 SQL 語句移至 Repository
   - 使用介面抽象化

3. **加強錯誤處理與驗證**
   - 添加資料驗證
   - 改善錯誤訊息

---

## 📝 開發建議

### Phase 1: 修正現有功能（1-2 週）

1. 修正 `DividendService.ConfirmAsync` 的計算邏輯
2. 加入目標配息率查詢
3. 加入本金補足判斷
4. 加入收益分攤邏輯

### Phase 2: 補齊 Step 1 & Step 2（2-3 週）

1. 建立 Entity 類別
2. 實作配息參數設定功能
3. 實作目標配息率設定功能

### Phase 3: 完善功能與測試（1-2 週）

1. 加強錯誤處理
2. 添加單元測試
3. 添加整合測試

---

## 📚 參考文件

- [配息功能開發指南](./配息功能開發指南.md)
- [BRD 文件](../../../../配息相關文件/BRD_基金配息流程_v1.md)
- [業務邏輯說明](../../../../配息相關文件/配息業務邏輯說明 ver1.md)
- [資料表清單](../../../../配息相關文件/DMS5A_相關資料表清單.md)

---

## ⚠️ 注意事項

1. **資料庫 Schema**：確認 Production 環境的資料表結構與文件一致
2. **業務邏輯**：開發前需與業務人員確認計算邏輯
3. **測試資料**：準備足夠的測試資料進行驗證
4. **精度控制**：配息計算涉及金額，需注意小數點精度

---

**Review 完成日期**：2024-12-XX  
**Review 人員**：AI Assistant  
**下一步行動**：根據優先順序開始修正與開發
