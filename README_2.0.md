# DmsSystem 架構指南 2.0

## 📋 目錄

1. [專案概述](#專案概述)
2. [架構設計](#架構設計)
3. [專案結構](#專案結構)
4. [資料庫配置](#資料庫配置)
5. [環境設定](#環境設定)
6. [如何執行](#如何執行)
7. [架構改進建議](#架構改進建議)
8. [前端開發（React）](#前端開發react)

---

## 專案概述

**DmsSystem** 是一個股東會資料管理系統，採用 .NET 8.0 和洋蔥式架構（Onion Architecture）設計。系統主要功能包括：

- 股東會資料匯入與管理
- 公司資訊上傳與處理
- 股票餘額管理
- 股東會報表產生

**前端規劃**：原本規劃為 Windows Forms 應用程式，現已改為 **React 網頁應用程式**。

---

## 架構設計

### 洋蔥式架構（Onion Architecture）

本專案採用嚴格的洋蔥式分層架構，確保程式碼的**清晰度、可測試性**與**長期可維護性**。

```
┌─────────────────────────────────────┐
│   DmsSystem.Api (表現層)            │  ← HTTP 請求入口
├─────────────────────────────────────┤
│   DmsSystem.Infrastructure          │  ← 資料存取、檔案處理實作
├─────────────────────────────────────┤
│   DmsSystem.Application             │  ← 業務邏輯、服務介面
├─────────────────────────────────────┤
│   DmsSystem.Domain                  │  ← 領域實體（Entities）
└─────────────────────────────────────┘
```

### 各層職責

#### 1. **DmsSystem.Domain** (核心領域層 - 最內層)
- **內容**: 只包含最純粹的業務物件定義（Entities），例如 `Contract.cs`, `ShmtSource1.cs` 等
- **職責**: 定義系統的核心「名詞」
- **規則**: **絕對不參考**任何其他專案

#### 2. **DmsSystem.Application** (應用程式層 - 次內層)
- **內容**: 
  - 定義業務功能的**介面**（Interfaces），例如 `IContractRepository.cs`, `IShareholderMeetingDetailService.cs`
  - **【建議】** 業務邏輯的**實作**（Services）應位於此層
  - DTOs 和 Validation Rules
- **職責**: 定義系統的核心「動詞」、業務流程的「合約」以及**編排業務邏輯**
- **規則**: 只參考 `Domain` 專案

#### 3. **DmsSystem.Infrastructure** (基礎設施層 - 次外層)
- **內容**: 包含所有與「外部世界」溝通的具體**實作**程式碼
  - Repository 實作（如 `ContractRepository.cs`）
  - DbContext（`DmsDbContext.cs`）
  - 檔案解析實作（NPOI/CsvHelper）
  - Excel 產生器實作
- **職責**: **實作** `Application` 層定義的「資料存取合約」或其他基礎設施介面
- **規則**: 參考 `Application` 專案

#### 4. **DmsSystem.Api** (表現層 / API - 最外層)
- **內容**: ASP.NET Core Web API 專案，包含 `Controllers` 和 `Program.cs`
- **職責**: 作為系統的統一入口，接收 HTTP 請求，委派給 `Application` 層的服務，回傳結果
- **規則**: 參考 `Application` 和 `Infrastructure`

### 依賴關係黃金法則

**所有參考方向永遠指向內層**：
- `Api` → `Application` → `Domain`
- `Infrastructure` → `Application` → `Domain`

---

## 專案結構

```
DmsSystem/
├── DmsSystem.Domain/              # 領域實體層
│   └── Entities/                  # 資料庫對應的實體類別
│
├── DmsSystem.Application/         # 應用程式層
│   ├── Interfaces/                # 業務介面定義
│   ├── Services/                 # 【建議】業務邏輯實作應在此
│   └── DTOs/                     # 資料傳輸物件
│
├── DmsSystem.Infrastructure/      # 基礎設施層
│   ├── Persistence/               # 資料存取
│   │   ├── Contexts/            # DbContext
│   │   └── Repositories/        # Repository 實作
│   ├── Services/                 # 【目前】Service 實作在此（應移至 Application）
│   └── FileGeneration/           # 檔案產生器實作
│
├── DmsSystem.Api/                 # API 表現層
│   ├── Controllers/              # API 控制器
│   ├── Program.cs                # 應用程式入口與 DI 設定
│   └── appsettings.json          # 應用程式設定
│
└── DmsSystem.WinFormsClient/      # 【已棄用】Windows Forms 客戶端
```

---

## 資料庫配置

### 支援的資料庫環境

系統支援兩種資料庫環境：

1. **Mac 測試環境**：本地開發測試用（SQL Server Docker 容器）
2. **正式環境**：生產環境的 SQL Server

### 資料庫連接字串設定

連接字串透過 `appsettings.json` 和環境變數進行配置。

#### 方式一：使用 appsettings.{Environment}.json（推薦）

**開發環境** (`appsettings.Development.json`)：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

**正式環境** (`appsettings.Production.json`)：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-production-server;Database=DMS;User Id=your-user;Password=your-password;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

#### 方式二：使用環境變數（更安全）

在 Mac 上設定環境變數：
```bash
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

在 Windows 上設定環境變數：
```powershell
$env:ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

#### 方式三：使用 User Secrets（開發環境推薦）

```bash
cd DmsSystem.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

### 資料庫初始化

#### Mac 環境（使用 Docker）

1. **啟動 SQL Server 容器**：
```bash
cd DmsSystem
docker-compose up -d
```

或使用腳本：
```bash
./scripts/start-sqlserver.sh
```

2. **驗證連接**：
```bash
docker exec -it dms-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'DmsSystem@2024' -d DMS -Q "SELECT DB_NAME()"
```

3. **執行資料庫遷移**（如果使用 EF Core Migrations）：
```bash
cd DmsSystem.Api
dotnet ef database update
```

#### 正式環境

1. 確保 SQL Server 已安裝並運行
2. 建立資料庫（如果尚未建立）
3. 更新 `appsettings.Production.json` 中的連接字串
4. 執行資料庫遷移或初始化腳本

---

## 環境設定

### 設定 ASP.NET Core 環境

#### Mac / Linux
```bash
export ASPNETCORE_ENVIRONMENT=Development  # 或 Production
```

#### Windows PowerShell
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"  # 或 Production
```

#### Windows CMD
```cmd
set ASPNETCORE_ENVIRONMENT=Development  # 或 Production
```

### 環境變數優先順序

ASP.NET Core 會依照以下順序載入設定（後面的會覆蓋前面的）：

1. `appsettings.json`
2. `appsettings.{Environment}.json`（例如 `appsettings.Development.json`）
3. 環境變數
4. User Secrets（僅限開發環境）

---

## 如何執行

### 前置需求

- **.NET 8 SDK**：從 [.NET 官網](https://dotnet.microsoft.com/download/dotnet/8.0) 下載
- **Docker Desktop**（Mac 測試環境需要）
- **Visual Studio Code** 或 **Visual Studio 2022**

### 使用 Visual Studio Code

1. **還原套件**：
```bash
cd DmsSystem
dotnet restore
```

2. **設定資料庫連接**：
   - 修改 `DmsSystem.Api/appsettings.Development.json` 中的連接字串
   - 或使用環境變數/User Secrets

3. **啟動 SQL Server**（Mac 環境）：
```bash
docker-compose up -d
```

4. **執行專案**：
   - 按 `F5` 開始偵錯
   - 或使用命令面板 (`Cmd+Shift+P` / `Ctrl+Shift+P`) 選擇 "Debug: Start Debugging"
   - 選擇 ".NET Core Launch (API)" 配置
   - API 會自動啟動並開啟 Swagger UI (http://localhost:5137/swagger)

5. **信任 HTTPS 開發憑證**（首次執行）：
```bash
dotnet dev-certs https --trust
```

### 使用 Visual Studio 2022

1. 開啟 `DMS.sln`
2. 設定 `DmsSystem.Api` 為啟動專案
3. 設定資料庫連接字串
4. 按 `F5` 執行

### 使用命令列

```bash
cd DmsSystem.Api
dotnet run
```

---

## 架構改進建議

### 目前架構問題

1. **Service 實作位置不當**
   - **現況**：Service 實作（如 `CompanyInfoUploadService.cs`）位於 `Infrastructure` 層
   - **問題**：違反分層原則，導致「Infrastructure (Service) 呼叫 Infrastructure (Repository)」
   - **建議**：將 Service 實作移至 `Application` 層

2. **檔案解析邏輯耦合**
   - **現況**：Service 直接使用 NPOI 和 CsvHelper
   - **問題**：業務邏輯與技術實作耦合
   - **建議**：建立檔案解析介面，在 Infrastructure 層實作

### 建議的改進步驟

#### 步驟 1：建立檔案解析介面

在 `DmsSystem.Application/Interfaces/` 建立：
- `IFileParser.cs`：定義檔案解析介面

#### 步驟 2：實作檔案解析器

在 `DmsSystem.Infrastructure/FileParsing/` 建立：
- `ExcelFileParser.cs`：使用 NPOI 實作
- `CsvFileParser.cs`：使用 CsvHelper 實作

#### 步驟 3：移動 Service 實作

將 `DmsSystem.Infrastructure/Services/` 中的 Service 實作移至：
- `DmsSystem.Application/Services/`

#### 步驟 4：更新依賴注入

在 `Program.cs` 中更新服務註冊：
```csharp
// 檔案解析器
builder.Services.AddScoped<IExcelFileParser, ExcelFileParser>();
builder.Services.AddScoped<ICsvFileParser, CsvFileParser>();

// 業務服務（現在在 Application 層）
builder.Services.AddScoped<ICompanyInfoUploadService, CompanyInfoUploadService>();
```

### 改進後的架構優勢

- ✅ 符合分層原則
- ✅ 業務邏輯與技術實作解耦
- ✅ 更容易進行單元測試
- ✅ 更容易替換技術實作（例如從 NPOI 換到 EPPlus）

---

## 前端開發（React）

### API 端點

API 基於 RESTful 設計，主要端點包括：

- `GET /api/ShareholderMeetings`：取得股東會列表
- `POST /api/ShareholderMeetings/upload`：上傳股東會資料
- `POST /api/CompanyInfo/upload`：上傳公司資訊
- `POST /api/StockBalance/upload`：上傳股票餘額
- `GET /api/Reports/shareholder`：產生股東會報表

### Swagger UI

開發時可透過 Swagger UI 測試 API：
- 開發環境：http://localhost:5137/swagger

### CORS 設定

如需從 React 前端呼叫 API，需要在 `Program.cs` 中設定 CORS：

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // React 開發伺服器
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 在 app.UseAuthorization() 之前加入
app.UseCors("AllowReactApp");
```

### React 前端建議結構

```
react-client/
├── src/
│   ├── api/              # API 呼叫封裝
│   ├── components/      # React 元件
│   ├── pages/           # 頁面元件
│   ├── hooks/           # 自訂 Hooks
│   └── utils/           # 工具函數
├── package.json
└── ...
```

---

## 重要觀念與最佳實踐

### 1. 依賴注入 (Dependency Injection)

所有服務都透過 DI 容器管理，確保：
- 鬆散耦合
- 易於測試
- 易於替換實作

### 2. 非同步操作

所有 I/O 操作（資料庫、檔案）都使用 `async/await`：
```csharp
public async Task<Result> ProcessAsync()
{
    await _repository.AddAsync(entity);
    await _repository.SaveChangesAsync();
}
```

### 3. DTOs 使用

避免直接暴露 Domain Entities，使用 DTOs 進行資料傳輸。

### 4. 錯誤處理

建議建立全域錯誤處理 Middleware：
```csharp
app.UseExceptionHandler("/error");
```

### 5. 日誌記錄

建議引入 Serilog 進行結構化日誌記錄。

---

## 常見問題

### Q: 如何切換資料庫環境？

A: 修改 `appsettings.{Environment}.json` 中的連接字串，或使用環境變數。

### Q: Mac 上如何連接 SQL Server？

A: 使用 Docker 容器運行 SQL Server，連接字串使用 `localhost,1433`。

### Q: 如何新增新的 API 端點？

A: 
1. 在 `Application/Interfaces` 定義介面
2. 在 `Application/Services` 實作業務邏輯（或 `Infrastructure/Services`，待改進）
3. 在 `Infrastructure/Persistence/Repositories` 實作資料存取
4. 在 `Api/Controllers` 建立控制器
5. 在 `Program.cs` 註冊服務

### Q: 如何測試 API？

A: 使用 Swagger UI（開發環境自動啟用）或 Postman。

---

## 下一步

- [ ] 將 Service 實作移至 Application 層
- [ ] 建立檔案解析介面並解耦
- [ ] 建立 React 前端專案
- [ ] 實作全域錯誤處理
- [ ] 引入 Serilog 日誌
- [ ] 撰寫單元測試
- [ ] 實作輸入驗證（FluentValidation）

---

## 聯絡資訊

如有問題或建議，請聯絡專案維護者。

