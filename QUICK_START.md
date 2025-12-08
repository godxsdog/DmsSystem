# DMS 系統快速啟動指南

## 快速啟動

### 1. 啟動後端 API

```bash
./start.sh
```

這個腳本會：
- 自動啟動 SQL Server 容器（如果尚未運行）
- 載入配息測試資料
- 啟動 API 伺服器（http://localhost:5137）

### 2. 啟動前端（新終端視窗）

```bash
./start-frontend.sh
```

前端將在 http://localhost:5173 啟動

### 3. 開啟瀏覽器

訪問 http://localhost:5173，點擊「配息管理」標籤

## 測試資料

### 配息測試資料

已自動載入以下測試資料：

- **基金代號**: A001
- **配息頻率**: M (月配)
- **配息基準日**: 2024-12-01
- **NAV**: 10.5
- **單位數**: 1,000,000
- **可分配收益**: 60,000 (扣除費用後)

### Step 1/2 API（配息參數與目標配息率）
- 配息參數設定（Step 1）：`POST /api/dividend-settings`，Body：`FundNo`, `DivType`, `Item01Seq`~`Item10Seq`, `CapitalType`, `EmailList`
- 查詢配息參數：`GET /api/dividend-settings/{fundNo}/{divType}`
- 目標配息率設定（Step 2）：`POST /api/dividend-settings/targets`，Body：`FundNo`, `DivType`, `TxDate`, `DivObj`, `DivObjAmt`
- 查詢目標配息率（回溯最近一筆）：`GET /api/dividend-settings/targets/{fundNo}/{divType}/{effectiveDate}`

### 手動載入測試資料

如果需要重新載入測試資料：

```bash
./scripts/load-dividend-test-data.sh
```

## 測試步驟

1. **匯入 CSV 檔案**
   - 點擊「配息管理」標籤
   - 選擇 CSV 檔案（Big5 編碼）
   - 點擊「匯入檔案」

2. **執行配息計算**
   - 輸入基金代號：`A001`
   - 選擇配息基準日：`2024-12-01`
   - 選擇配息頻率：`M` (月配)
   - 點擊「執行計算與確認」

3. **查看結果**
   - 系統會顯示計算結果，包含：
     - 基準日淨值 (NAV)
     - 基準日單位數
     - 總可分配金額
     - 每單位可分配收益（年化）
     - 每單位配息金額（當期）
     - 每單位本金配息比率

## 注意事項

- Swagger 已停用，不會自動開啟瀏覽器
- API 伺服器運行在 http://localhost:5137
- 前端運行在 http://localhost:5173
- 確保 SQL Server 容器正在運行

## 故障排除

### SQL Server 未啟動

```bash
docker-compose up -d
```

### 測試資料未載入

```bash
./scripts/load-dividend-test-data.sh
```

### 端口被占用

修改 `DmsSystem.Api/Properties/launchSettings.json` 中的端口號
