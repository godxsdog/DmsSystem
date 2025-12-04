# DmsSystem 架構指南

## 1. 專案目標

本專案 (`DmsSystem`) 旨在建立一個現代化的後端服務系統，用於處理股東會相關的資料作業，例如從外部檔案匯入資料到資料庫。前端介面目前規劃為 Windows Forms 應用程式 (`DmsSystem.WinFormsClient`)。

本文件將詳細說明此專案採用的架構設計、核心概念以及各個部分如何協同工作。

---

## 2. 專案架構：洋蔥式架構 (Onion Architecture)

為了確保程式碼的**清晰度、可測試性**與**長期可維護性**，我們採用了嚴格的洋蔥式分層架構。

### 各層職責

您可以將整個系統想像成好幾層同心圓，越往內層越核心、越穩定：

1.  **`DmsSystem.Domain` (核心領域層 - 最內層)**
    * **內容**: 只包含最純粹的業務物件定義 (Entities)，例如 `Contract.cs`, `ShmtSource1.cs` 等。這些通常由 EF Core 自動產生，代表了資料庫的結構。
    * **職責**: 定義系統的核心「名詞」。
    * **規則**: **絕對不參考**任何其他專案。

2.  **`DmsSystem.Application` (應用程式層 - 次內層)**
    * **內容**: 定義系統需要完成的「業務功能」(Interfaces)，例如 `IContractRepository.cs`, `IShareholderMeetingDetailService.cs`。**【建議】** 也應包含業務邏輯的**實作** (`...Service.cs`)。未來可能還會包含 DTOs 和 Validation Rules。
    * **職責**: 定義系統的核心「動詞」、業務流程的「合約」以及**編排業務邏輯**。
    * **規則**: 只參考 `Domain` 專案。

3.  **`DmsSystem.Infrastructure` (基礎設施層 - 次外層)**
    * **內容**: 包含所有與「外部世界」溝通的具體**實作**程式碼。例如 `ContractRepository.cs`, `DmsDbContext.cs`。包含 NPOI/CsvHelper 的檔案解析邏輯**目前**也在此層。
    * **職責**: **實作** `Application` 層定義的「資料存取合約」(Repositories) 或其他基礎設施介面（如檔案讀寫），負責處理「如何」連接資料庫、「如何」讀寫檔案等技術細節。
    * **規則**: 參考 `Application` 專案。

4.  **`DmsSystem.Api` (表現層 / API - 最外層)**
    * **內容**: ASP.NET Core Web API 專案，包含 `Controllers` 和 `Program.cs`。
    * **職責**: 作為系統的統一入口，接收 HTTP 請求，委派給 `Application` 層的服務，回傳結果。
    * **規則**: 參考 `Application` 和 `Infrastructure`。

5.  **`DmsSystem.WinFormsClient` (客戶端層 - 獨立)**
    * **內容**: Windows Forms 應用程式，包含 UI 介面 (`Form1.cs`) 和呼叫 API 的邏輯 (`ApiClient.cs`)。
    * **職責**: 提供使用者操作介面，透過 HTTP 與 `DmsSystem.Api` 互動。
    * **規則**: 參考 `Domain`，**不直接參考** `Application` 或 `Infrastructure`。

### 依賴關係黃金法則

**所有參考方向永遠指向內層** (`Api` -> `Application` -> `Domain`)。`Infrastructure` 也參考 `Application` 來實作其介面。

---

## 3. 核心概念：依賴注入 (Dependency Injection - DI)

這是將所有層**「黏合」**在一起的關鍵機制。

### 為什麼需要 DI？

DI 的核心思想是：**一個物件不應該自己建立它所需要的依賴物件，而應該由外部提供 (注入) 給它。** 這實現了**鬆散耦合**，使得：
* **易於替換**: 可以輕易更換某個功能的實作（例如換資料庫），而不影響呼叫它的程式碼。
* **易於測試**: 可以注入「假的」依賴項來進行單元測試。

### DI 在 DmsSystem 中的運作

1.  **服務註冊 (在 `Api/Program.cs`)**:
    * `builder.Services.AddDbContext<DmsDbContext>(...)`: 告訴 DI 容器如何建立 `DbContext`。
    * `builder.Services.AddScoped<IShmtSource1Repository, ShmtSource1Repository>();`: 建立**介面**與**實作**的對應關係。
    * `builder.Services.AddScoped<IShareholderMeetingDetailService, ShareholderMeetingDetailService>();`: 同上。

2.  **依賴解析 (在 Controller/Service 的建構函式)**:
    * 當需要建立 `ShareholderMeetingsController` 時，DI 容器看到它需要 `IShareholderMeetingDetailService`。
    * 容器查找註冊表，找到對應的 `ShareholderMeetingDetailService` 實作。
    * 容器看到 `ShareholderMeetingDetailService` 需要 `IShmtSource1Repository`。
    * 容器查找註冊表，找到對應的 `ShmtSource1Repository` 實作。
    * 容器看到 `ShmtSource1Repository` 需要 `DmsDbContext`。
    * 容器建立 `DmsDbContext`，注入 `ShmtSource1Repository`，再注入 `ShareholderMeetingDetailService`，最後注入 `ShareholderMeetingsController`。

---

## 4. DAO vs. Repository 釐清

* **DAO**: 更貼近**資料庫表格**的抽象，專注於基礎 CRUD。
* **Repository**: 更貼近**領域模型**的抽象，模擬「物件集合」，作為**應用層**與**資料映射層**的中介。
* **在本專案中**: `Infrastructure/Repositories` (如 `ShmtSource1Repository.cs`) 功能上類似 DAO，但其目的是作為 `Application` 層 `IRepository` **介面的具體實作**，遵循 Repository Pattern 的精神來**解耦**應用邏輯與資料存取細節。

---

## 5. Service 位置的討論與【建議調整】

* **目前狀況**: Service 的**實作** (`CompanyInfoUploadService.cs` 等) 位於 `Infrastructure` 層。這導致「Infrastructure (Service) 呼叫 Infrastructure (Repository)」的現象。
* **理想架構**: Service 的**實作**應位於 **`Application` 層**，負責編排業務邏輯，並依賴 `Application` 層定義的 `IRepository` 介面。`Infrastructure` 層則專注實作 `IRepository`。
* **【建議調整步驟】**:
    1.  將 `Infrastructure/Services` 資料夾內的 **Service 實作類別** (`.cs` 檔案) **移動**到 `Application` 專案下新建的 `Services` 資料夾。
    2.  修正這些檔案的 `namespace`。
    3.  確保專案參考關係正確。
    4.  確保 `NPOI`, `CsvHelper` 等 NuGet 套件**只安裝在 `Infrastructure` 層**。
        * *(進階)* 若 `Application` Service 需檔案解析功能，應透過**新介面**(在 `Application` 定義)和**實作**(在 `Infrastructure` 提供)來注入。*(若覺複雜可暫緩此步)*

---

## 6. 如何在本機執行？

### 使用 Visual Studio Code

1.  **環境需求**: 
    * .NET 8 SDK ([下載連結](https://dotnet.microsoft.com/download/dotnet/8.0))
    * Visual Studio Code
    * C# Dev Kit 擴充功能 (VS Code 會自動提示安裝)

2.  **安裝擴充功能**:
    * 開啟 VS Code 後，會自動提示安裝推薦的擴充功能
    * 或手動安裝：`ms-dotnettools.csdevkit`、`ms-dotnettools.csharp`

3.  **還原套件**:
    ```bash
    dotnet restore
    ```

4.  **設定連線**: 修改 `DmsSystem.Api/appsettings.Development.json` 中的 `ConnectionStrings:DefaultConnection`。

5.  **執行專案**:
    * 按 `F5` 開始偵錯，或
    * 使用命令面板 (`Cmd+Shift+P` / `Ctrl+Shift+P`) 選擇 "Debug: Start Debugging"
    * 選擇 ".NET Core Launch (API)" 配置
    * API 會自動啟動並開啟 Swagger UI (http://localhost:5137/swagger)

6.  **其他可用任務** (命令面板 -> "Tasks: Run Task"):
    * `build`: 建置專案
    * `watch`: 監看模式執行 (自動重新載入)
    * `clean`: 清理建置檔案
    * `restore`: 還原 NuGet 套件

7.  **(首次執行)** 信任本機 HTTPS 開發憑證:
    ```bash
    dotnet dev-certs https --trust
    ```

### 使用 Visual Studio 2022

1.  **環境**: .NET 8 SDK, VS 2022 (含 ASP.NET 和 .NET 桌面開發 worklaods)。
2.  **取得程式碼**: Git Clone。
3.  **還原套件**: 開啟方案檔 `.sln`，VS 自動還原或手動重建。
4.  **設定連線**: 修改 `Api/appsettings.Development.json` 中的 `ConnectionStrings:DefaultConnection`。
5.  **設定多重啟動**: 將 `Api` 和 `WinFormsClient` 設為啟動專案。
6.  **執行**: 按 F5。
7.  **(首次執行)** 信任本機 HTTPS 開發憑證 (`dotnet dev-certs https --trust`)。

---

## 7. 重要觀念與下一步 (架構師建議)

* **DTOs**: 避免直接暴露 Domain Entities。
* **輸入驗證**: 使用 FluentValidation。
* **全域錯誤處理**: 建立 Middleware。
* **結構化日誌**: 引入 Serilog。
* **自動化測試**: 撰寫單元/整合測試。
* **非同步 (`async/await`)**: 貫徹所有 I/O 操作。
