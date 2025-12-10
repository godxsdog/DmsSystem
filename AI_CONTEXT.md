# DMS 系統 - AI 快速進入文件

> 📋 **最高指導原則**：此文件包含專案的所有核心資訊，每次更新專案時都應同步更新此文件。
程式有修正都要git push一次 且comment都要寫
路徑是/Users/kaichanghuang/Documents/Phoenix Code/DmsSystem
---

## 🎯 專案概況

### 專案名稱
**DMS 系統**（股東會資料管理系統）

### 技術棧
- **後端**：.NET 8.0, ASP.NET Core, Entity Framework Core, SQL Server
- **前端**：React + TypeScript + Vite（網頁應用程式）
- **架構**：洋蔥式架構（Onion Architecture）
- **測試**：xUnit, Moq, FluentValidation

### 專案目標
建立現代化的股東會資料管理系統，支援：
- 股東會資料上傳（Excel/CSV）
- 公司資訊上傳
- 股票餘額上傳
- 資料查詢與顯示（React 網頁前端）
- 報表產生
- **基金配息管理**：配息資料匯入、查詢、計算與確認（含批量處理）、配息組成維護與上傳 EC

### 前端技術
**系統現在以 React 網頁應用程式為主**，不再支援 Windows Forms 桌面應用程式。

---

## 🏗️ 專案結構

```
DmsSystem/
├── DmsSystem.Domain/              # 領域實體層（最內層）
│   └── Entities/                  # 資料庫對應的實體類別
│
├── DmsSystem.Application/         # 應用程式層
│   ├── Interfaces/                # 業務介面定義
│   ├── Services/                  # 業務邏輯實作（Service）
│   └── DTOs/                      # 資料傳輸物件
│
├── DmsSystem.Infrastructure/      # 基礎設施層
│   ├── Persistence/               # 資料存取
│   │   ├── Contexts/              # DbContext
│   │   ├── Repositories/         # Repository 實作
│   │   └── SqlQueries/           # SQL 語句資源類別
│   ├── FileParsing/               # 檔案解析實作（Excel、CSV）
│   └── FileGeneration/           # 檔案產生器實作
│   ⚠️ **注意**：Infrastructure 層不應包含 Services 資料夾
│   ⚠️ Service 實作應在 Application 層，不在 Infrastructure 層
│   ⚠️ SQL 語句應放在 SqlQueries 資源類別中，保持程式碼整潔
│
├── DmsSystem.Api/                 # API 表現層（最外層）
│   ├── Controllers/               # API 控制器
│   ├── Middleware/                # 錯誤處理 Middleware
│   └── Validators/                # 輸入驗證器
│
├── DmsSystem.Tests/               # 測試專案
│   ├── Services/                  # Service 單元測試
│   ├── Validators/                # 驗證器測試
│   ├── Integration/               # 整合測試
│   └── FileParsing/               # 檔案解析器測試
│
├── react-client/                  # React 前端（網頁應用程式）
│   ├── src/
│   │   ├── api/                  # API 客戶端
│   │   ├── components/           # React 元件
│   │   └── pages/                # 頁面元件
│
├── docs/                          # 技術文件
│   ├── 00-快速開始.md            # 通用：快速啟動（包含完整環境設定、Git 版本控制、錯誤診斷）
│   ├── 01-架構指南.md            # 通用：架構說明
│   ├── 02-架構分析與優勢.md      # 通用：架構分析
│   ├── 03-資料庫配置.md          # 通用：資料庫配置（包含資料庫操作、備份、還原）
│   ├── 04-測試指南.md            # 通用：測試說明（包含執行測試、配息功能測試）
│   ├── 05-專案完成總結.md        # 通用：專案總結
│   ├── 06-使用者手冊.md          # 通用：使用說明
│   ├── 07-架構修正說明.md        # 通用：架構修正說明
│   ├── 08-執行狀態報告.md        # 通用：狀態檢查
│   ├── 09-系統測試報告.md        # 通用：測試報告
│   ├── 10-最新修正記錄.md        # 通用：最新修正記錄（配息功能、SQL 重構、錯誤處理等）
│   │
│   ├── MAC-DEVELOPMENT-ONLY/      # Mac 本地開發測試（Docker SQL Server）
│   │   ├── 00-快速測試指南.md    # ⭐ 從這裡開始測試
│   │   ├── 01-Mac開發環境完整手冊.md
│   │   ├── 02-測試資料載入.md
│   │   └── 03-Docker-SQL-Server設定.md
│   │
│   └── FEATURES/                   # 功能模組文件
│       ├── DIVIDEND/              # 基金配息管理
│       │   ├── TEST_CASES.md      # 配息功能測試案例
│       │   ├── 配息系統操作與邏輯說明.md # 系統操作流程與詳細計算邏輯
│       │   └── ...
│       └── ...
│
└── scripts/                        # 工具腳本
    ├── init-db.sql                # 資料庫初始化
    ├── seed-test-data.sql         # 測試資料（Mac 本地測試用）
    ├── start-sqlserver.sh         # 啟動 SQL Server（Mac Docker）
    └── generate-error-report.sh   # 錯誤報告產生器
```

---

## 🔄 開發環境與流程

### Mac 本地開發測試環境

**使用情境**：
- 個人開發和測試
- 使用 Docker SQL Server 本地資料庫
- 完全獨立的開發環境

**資料庫**：Docker SQL Server（`localhost:1433`）

**環境特點**：
- 使用 Docker 容器運行 SQL Server
- 本地測試資料
- 獨立開發環境

**快速測試指南**：`docs/MAC-DEVELOPMENT-ONLY/00-快速測試指南.md`

### Windows 正式環境

**使用情境**：
- 正式環境執行和測試
- 連接正式區 SQL Server（已建立）
- 生產部署

**資料庫**：正式區 SQL Server（已建立）

**環境特點**：
- 直接連接公司 SQL Server
- 正式環境資料
- 生產部署環境

---

## 📝 文件命名規則

### 通用文件（docs/ 根目錄）

**編號規則**：`00-` 到 `09-`，按邏輯順序排列

- `00-快速開始.md` - 快速啟動指南
- `01-架構指南.md` - 架構說明
- `02-架構分析與優勢.md` - 架構分析
- `03-資料庫配置.md` - 資料庫配置
- `04-測試指南.md` - 測試說明
- `05-專案完成總結.md` - 專案總結
- `06-使用者手冊.md` - 使用說明
- `07-架構修正說明.md` - 架構修正歷程
- `08-執行狀態報告.md` - 狀態檢查
- `09-系統測試報告.md` - 測試報告
- `10-最新修正記錄.md` - 專案變更日誌

### Mac 環境文件（MAC-DEVELOPMENT-ONLY/）

**編號規則**：`00-` 到 `03-`

- `00-快速測試指南.md` - ⭐ 從這裡開始測試
- `01-Mac開發環境完整手冊.md` - 完整開發指南
- `02-測試資料載入.md` - 測試資料載入
- `03-Docker-SQL-Server設定.md` - Docker 設定

### Windows 環境文件

**位置**：所有 Windows 環境相關內容已整合到 `docs/` 根目錄的主要文件中

- `00-快速開始.md` - 包含完整環境設定、Git 版本控制、錯誤診斷等
- `03-資料庫配置.md` - 包含資料庫操作、備份、還原等
- `04-測試指南.md` - 包含執行測試、配息功能測試等

### 命名原則

1. **編號前綴**：使用兩位數字（`00-`, `01-`, `02-`...）確保排序
2. **檔名格式**：`編號-中文名稱.md`
3. **邏輯順序**：按照使用順序排列（快速開始 → 架構 → 配置 → 測試 → 使用）
4. **環境區分**：Mac 和 Windows 文件分別放在對應資料夾

---

## 🔀 Git 工作流程

### 分支策略

- **main**：主要分支（穩定版本）
- **feature/architecture-refactoring-and-enhancements**：目前開發分支

### Commit 訊息格式

**⚠️ 重要**：所有 commit message 必須使用**中文**，提升可讀性。

```
<type>: <簡短描述>

<詳細說明（可選）>
```

**Type 類型（中文）**：
- `功能`: 新功能
- `修正`: 錯誤修正
- `文件`: 文件更新
- `重構`: 重構程式碼
- `測試`: 測試相關
- `建置`: 建置或工具
- `整理`: 文件或程式碼整理

**範例**：
```
功能：新增配息資料查詢 API

- 新增 GET /api/Dividends 端點
- 支援基金代號、配息頻率、日期範圍篩選
- 前端新增查詢 UI 和結果表格顯示
```

### 每次更新必須

1. **更新 AI_CONTEXT.md**：如有架構變更、環境變更、文件結構變更
2. **更新相關文件**：如有功能變更，更新對應的文件
3. **提交到 Git**：使用適當的 commit 訊息
4. **Push 到遠端**：確保遠端倉庫同步

### Git 同步檢查清單

- [ ] 所有變更已提交（`git status` 顯示 clean）
- [ ] Commit 訊息清楚描述變更內容
- [ ] 已推送到遠端倉庫（`git push`）
- [ ] AI_CONTEXT.md 已更新（如有需要）

---

## ⚙️ 環境設定

### Mac 環境（本地測試）

**資料庫連接字串**：
```
Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True
```

**位置**：`DmsSystem.Api/appsettings.Development.json`

**啟動方式**：
```bash
# 1. 啟動 Docker SQL Server
docker-compose up -d

# 2. 啟動 API
cd DmsSystem.Api && dotnet run

# 3. 啟動前端
cd react-client && npm run dev
```

**文件位置**：`docs/MAC-DEVELOPMENT-ONLY/`

### Windows 環境（正式環境）

**資料庫連接字串**：
```
Server=正式伺服器位址;Database=DMS;User Id=正式使用者;Password=正式密碼;TrustServerCertificate=True;MultipleActiveResultSets=True
```

**位置**：`DmsSystem.Api/appsettings.Production.json`

**啟動方式**：
```powershell
# 1. 確認正式區 SQL Server 已運行
# 2. 啟動 API
cd DmsSystem.Api
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run

# 3. 啟動前端
cd react-client
npm run dev
```

**文件位置**：所有 Windows 環境相關內容已整合到 `docs/` 根目錄的主要文件中

---

## 🧪 測試

### 測試專案位置
`DmsSystem.Tests/`

### 測試類型
- **單元測試**：`Services/`, `Validators/`
- **整合測試**：`Integration/`
- **檔案解析測試**：`FileParsing/`

### 執行測試
```bash
cd DmsSystem
dotnet test DmsSystem.Tests/DmsSystem.Tests.csproj
```

### 測試腳本
`DmsSystem.Tests/RunTests.sh` - 執行所有測試並產生報告

---

## 📚 文件結構原則

### 文件分類

1. **通用文件**：所有環境都適用（架構、測試、使用說明等）
   - 位置：`docs/` 根目錄
   - 編號：`00-` 到 `09-`

2. **Mac 環境文件**：個人開發測試使用
   - 位置：`docs/MAC-DEVELOPMENT-ONLY/`
   - 編號：`00-` 到 `03-`
   - 包含：Docker 設定、測試資料載入

3. **Windows 環境文件**：正式環境使用
   - 位置：`docs/WINDOWS-DEVELOPMENT/`
   - 編號：`01-` 到 `04-`
   - 包含：正式環境設定、Git 工作流程、資料庫遷移

### 文件更新原則

1. **每次功能變更**：更新對應的文件
2. **每次環境變更**：更新環境相關文件
3. **每次架構變更**：更新架構文件和 AI_CONTEXT.md
4. **每次 Git 提交**：確保文件同步更新

---

## 🔧 架構原則

### 依賴關係

**永遠指向內層**：
- `Api` → `Application` → `Domain`
- `Infrastructure` → `Application` → `Domain`

### 各層職責

1. **Domain**：只包含實體定義，不依賴任何其他專案
2. **Application**：業務邏輯和介面定義，只依賴 Domain
3. **Infrastructure**：技術實作（資料存取、檔案解析），依賴 Application
4. **Api**：HTTP 介面，依賴 Application 和 Infrastructure

### Service 位置

**Service 實作位於 Application 層**：
- `DmsSystem.Application/Services/`
- ✅ `CompanyInfoUploadService`
- ✅ `ShareholderMeetingDetailService`
- ✅ `StockBalanceUploadService`
- ✅ `ReportService`
- ✅ `DividendService`

**⚠️ 重要**：Service 實作**不應**在 Infrastructure 層
- ❌ `DmsSystem.Infrastructure/Services/` - 已移除，不應存在

**Repository 實作位於 Infrastructure 層**：
- `DmsSystem.Infrastructure/Persistence/Repositories/`

**Infrastructure 層職責**：
- ✅ Repository 實作
- ✅ FileParser 實作
- ✅ FileGenerator 實作
- ✅ DbContext
- ✅ Factory 實作（如 `DbConnectionFactory`）
- ✅ SqlQueries 資源類別（存放 SQL 語句，保持程式碼整潔）
- ❌ **不應包含**：業務邏輯 Service

**資料庫連接抽象**：
- ✅ `IDbConnectionFactory` 介面定義於 Application 層
- ✅ `DbConnectionFactory` 實作位於 Infrastructure 層
- ✅ Application 層透過介面取得資料庫連接，避免直接依賴 Infrastructure 層的具體實作

---

## 🚨 重要提醒

### 前端技術
- **系統現在以 React 網頁應用程式為主**
- **不再支援 Windows Forms 桌面應用程式**
- WinForms 相關檔案已移除

### 資料庫環境
- **Mac**：Docker SQL Server（本地測試，無法連線正式區）
- **Windows**：正式區 SQL Server（已建立）

### 開發流程
1. Mac 本地測試完成
2. 提交程式碼到 Git
3. Windows 環境拉取程式碼
4. 在正式區 SQL Server 上測試

---

## 📖 快速參考

### 開始開發
1. 閱讀 `docs/00-快速開始.md`（包含 Windows 和 Mac 環境說明）
2. Mac 環境：閱讀 `docs/MAC-DEVELOPMENT-ONLY/00-快速測試指南.md`
3. Windows 環境：閱讀 `docs/00-快速開始.md`（已包含完整環境設定）

### 了解架構
1. 閱讀 `docs/01-架構指南.md`
2. 閱讀 `docs/02-架構分析與優勢.md`

### 執行測試
1. 閱讀 `docs/04-測試指南.md`
2. 執行 `DmsSystem.Tests/RunTests.sh`

### 資料庫遷移
閱讀 `docs/03-資料庫配置.md`（已包含資料庫操作、備份、還原等）

---

## 📦 Solution 文件管理原則

### DMS.sln 必須包含所有專案

**原則**：`DMS.sln` 必須包含專案中的所有 .NET 專案和重要檔案，確保在 Visual Studio 2022 中可以正確讀取和開啟。

**必須包含的專案**：
1. ✅ `DmsSystem.Domain` - 領域實體層
2. ✅ `DmsSystem.Application` - 應用程式層
3. ✅ `DmsSystem.Infrastructure` - 基礎設施層
4. ✅ `DmsSystem.Api` - API 表現層
5. ✅ `DmsSystem.Tests` - 測試專案

**Solution Items 資料夾**：
- **Solution Items**：包含 README.md、AI_CONTEXT.md、CHANGELOG.md、.gitignore、docker-compose.yml 等重要檔案
- **Frontend**：包含 React 前端專案的重要檔案（package.json、README.md、tsconfig.json、vite.config.ts）
- **Scripts**：包含資料庫腳本和工具腳本

**React 前端專案**：
- React 專案（`react-client/`）無法直接作為 .NET 專案加入 solution
- 但可以透過 Solution Items 資料夾在 VS2022 中查看和編輯前端檔案
- 前端專案需使用 VS Code 或終端機進行開發和執行

**檢查方式**：
```bash
# 檢查 solution 包含的專案
dotnet sln list

# 應該看到所有 5 個 .NET 專案
```

**更新原則**：
- 每次新增 .NET 專案時，必須使用 `dotnet sln add` 加入 solution
- 或手動編輯 `DMS.sln` 加入專案定義
- 確保所有專案都有正確的 GUID 和建置設定

---

## 🔄 更新記錄

### 2024-12-10（最新）
- **配息功能重大更新 (5A1/5A3)**：
  - **批量確認功能 (5A1)**：新增 `POST /api/Dividends/confirm-all` API 與前端「批量計算所有未確認項目」按鈕，可一次處理所有待確認配息。
  - **配息組成維護 (5A3)**：移植 PB 邏輯，新增 CSV 匯入與上傳 EC 功能。
    - 匯入：`POST /api/Dividends/composition/import`，更新 `I_RATE`, `C_RATE`。
    - 上傳：`POST /api/Dividends/{fundNo}/{date}/{type}/upload-ec`，模擬同步至 WPS 並更新狀態。
  - **Entity/Schema 修正**：補全 `FundDiv.cs` 欄位 (`DivAdj`) 並修正 SQL 欄位對應 (`INTEREST_RATE` -> `I_RATE`, `CAPITAL_RATE` -> `C_RATE`)。
  - **前端優化**：新增 API 連線設定 UI，解決開發環境 Port 不一致問題。
  - **文件新增**：新增 `docs/FEATURES/DIVIDEND/配息系統操作與邏輯說明.md`。

### 2024-12-08
- **配息功能修正**：
  - 修正 CsvHelper 配置，避免 ArgumentNullException
  - 處理 CSV 檔案前 3 行說明文字
  - 修正空值欄位處理（PreDiv1B、Div1B）
  - 修正 SQL 欄位錯誤（移除不存在的 STATUS、STATUS_C 等欄位）
  - 修正資料型別錯誤（DividendYear 從 int? 改為 decimal?）
- **配息功能新增**：
  - 新增配息資料查詢功能（GET /api/Dividends）
  - 前端新增配息資料查詢 UI，支援多種篩選條件
- **程式碼重構**：
  - 建立 `DividendSqlQueries` 類別存放所有配息相關 SQL 語句
  - 將 SQL 字串從業務邏輯中分離，提升程式碼整潔度
- **錯誤處理改進**：
  - 改進前端錯誤處理，正確顯示詳細錯誤訊息
  - 改進 CORS 設定，開發環境允許所有 localhost port
- **文件整合**：
  - 將 WINDOWS-DEVELOPMENT 資料夾內容整合到主要文件
  - 建立 10-最新修正記錄.md 記錄所有修正內容
  - 刪除重複的 WINDOWS-DEVELOPMENT 資料夾和 QUICK_START.md
- **Commit Message 規範**：所有 commit message 改為中文

### 2024-12-XX
- **架構修正**：建立 `IDbConnectionFactory` 介面解決 Application 層直接依賴 Infrastructure 層的問題
- **修復編譯錯誤**：添加 `Microsoft.AspNetCore.Http.Abstractions` 和 `Microsoft.EntityFrameworkCore` NuGet 套件到 Application 層
- **依賴注入**：在 `Program.cs` 中註冊 `IDbConnectionFactory`，確保架構符合洋蔥式架構原則
- `DividendService` 現在透過 `IDbConnectionFactory` 取得資料庫連接，而非直接使用 `DmsDbContext`

### 2024-12-05
- 移除 WinForms 客戶端，系統現在以 React 網頁應用程式為主
- 重新整理文件結構，明確區分 Mac 和 Windows 環境
- 建立資料庫遷移指南
- 新增測試案例
- 修復 DMS.sln 缺少 DmsSystem.Tests 專案的問題
- 更新 DMS.sln 加入 Solution Items 和 react-client 資料夾，確保 VS2022 可以讀取所有檔案
- **架構修正**：移除 Infrastructure/Services 中的舊 Service 實作，確保 Service 實作都在 Application 層

---

## 📝 每次更新此文件時

1. **更新「更新記錄」**：記錄變更日期和內容
2. **檢查「專案結構」**：如有新增或移除專案/資料夾
3. **檢查「環境設定」**：如有環境變更
4. **檢查「文件命名規則」**：如有文件結構變更
5. **檢查「Solution 文件管理原則」**：如有新增專案，確保加入 solution
6. **提交到 Git**：確保變更同步

---

**此文件應作為 AI 助手的首要參考文件，每次對話開始時都應先閱讀此文件以了解專案現況。**
