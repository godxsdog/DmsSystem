# DmsSystem 架構指南 (新手導覽)

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
    * **內容**: 定義系統需要完成的「業務功能」(Interfaces)，例如 `IContractRepository.cs`, `IShareholderMeetingDetailService.cs`。未來可能還會包含 DTOs (資料傳輸物件) 和 Validation Rules (驗證規則)。
    * **職責**: 定義系統的核心「動詞」和業務流程的「合約」。
    * **規則**: 只參考 `Domain` 專案。

3.  **`DmsSystem.Infrastructure` (基礎設施層 - 次外層)**
    * **內容**: 包含所有與「外部世界」溝通的具體實作程式碼。例如 `ContractRepository.cs`, `ShareholderMeetingDetailService.cs` (包含 NPOI/CsvHelper 的邏輯), `DmsDbContext.cs` (EF Core)。
    * **職責**: 實作 `Application` 層定義的「合約」，負責處理「如何」連接資料庫、「如何」讀寫檔案等技術細節。
    * **規則**: 參考 `Application` 專案 (因為它需要實作介面)。

4.  **`DmsSystem.Api` (表現層 / API - 最外層)**
    * **內容**: ASP.NET Core Web API 專案，包含 `Controllers` (例如 `ContractsController.cs`, `ShareholderMeetingsController.cs`) 和 `Program.cs` (負責設定與啟動)。
    * **職責**: 作為系統的統一入口，接收來自客戶端 (例如 WinForms) 的 HTTP 請求，驗證輸入，將工作委派給 `Application` 層的服務，最後回傳結果。
    * **規則**: 參考 `Application` (呼叫服務介面) 和 `Infrastructure` (`Program.cs` 需要看到實作類別來進行 DI 註冊)。

5.  **`DmsSystem.WinFormsClient` (客戶端層 - 獨立)**
    * **內容**: Windows Forms 應用程式專案，包含 UI 介面 (`Form1.cs`) 和呼叫 API 的邏輯 (未來建議封裝到 `ApiClient.cs`)。
    * **職責**: 提供使用者操作介面，並透過 HTTP 請求與 `DmsSystem.Api` 互動。
    * **規則**: 參考 `Domain` (以便理解 API 回傳的資料結構)，**但不直接參考** `Application` 或 `Infrastructure`。

### 依賴關係黃金法則

**所有參考方向永遠指向內層** (`Api` -> `Application` -> `Domain`)。這確保了核心業務邏輯 (`Domain` 和 `Application`) 的純粹性，它們完全不知道也不關心外部是用什麼資料庫、什麼 UI 技術。

---

## 3. 核心概念：依賴注入 (Dependency Injection - DI)

這是將所有層**「黏合」**在一起的關鍵機制，也是理解現代 .NET 開發的核心。

### 為什麼需要 DI？(辦公大樓的比喻)

想像一下，如果沒有 DI：
* `ShareholderMeetingsController` (接待員) 需要親自去建立 `ShareholderMeetingDetailService` (股東會明細部主任)。
* 為了建立主任，接待員還得知道主任需要 `IShmtSource1Repository` (檔案管理員合約)，甚至還得知道如何找到並建立 `ShmtSource1Repository` (具體的管理員)。

這樣會導致接待員身兼數職，程式碼變得混亂且難以更換零件 (例如換一個檔案管理員)。

**DI 的作法是**：
* 接待員 (`Controller`) 在他的工作手冊 (建構函式) 上寫明：「我需要一位符合 `IShareholderMeetingDetailService` 合約的專家」。
* 主任 (`Service`) 在他的工作手冊上寫明：「我需要一位符合 `IShmtSource1Repository` 合約的專家」。
* 管理員 (`Repository`) 在他的工作手冊上寫明：「我需要 `DmsDbContext` 這個工具」。

然後，由一個**「總組立工廠」(也就是 .NET 的 DI 容器)**，在應用程式啟動時，讀取 `Program.cs` 這本「生產說明書」，預先知道所有零件的生產方式和組裝規則。

### DI 在 DmsSystem 中的運作

1.  **服務註冊 (在 `Program.cs`)**:
    * `builder.Services.AddDbContext<DmsDbContext>(...)`: 告訴工廠如何生產 `DbContext` (需要資料庫連線字串)。
    * `builder.Services.AddScoped<IShmtSource1Repository, ShmtSource1Repository>();`: 告訴工廠，當有人需要 `IShmtSource1Repository` (合約) 時，就生產一個 `ShmtSource1Repository` (工匠) 給他。
    * `builder.Services.AddScoped<IShareholderMeetingDetailService, ShareholderMeetingDetailService>();`: 同理，建立介面與實作的對應關係。

2.  **依賴解析 (在 Controller/Service 的建構函式)**:
    * 當 HTTP 請求進來，需要建立 `ShareholderMeetingsController` 時：
        * 工廠看到它需要 `IShareholderMeetingDetailService`。
        * 工廠根據註冊規則，決定去生產 `ShareholderMeetingDetailService`。
        * 工廠看到 `ShareholderMeetingDetailService` 需要 `IShmtSource1Repository`。
        * 工廠根據註冊規則，決定去生產 `ShmtSource1Repository`。
        * 工廠看到 `ShmtSource1Repository` 需要 `DmsDbContext`。
        * 工廠根據註冊規則，生產出 `DmsDbContext`。
        * 工廠將 `DmsDbContext` **注入** `ShmtSource1Repository`。
        * 工廠將 `ShmtSource1Repository` **注入** `ShareholderMeetingDetailService`。
        * 工廠將 `ShareholderMeetingDetailService` **注入** `ShareholderMeetingsController`。
        * 完成！一個所有依賴都已準備好的 Controller 被建立出來。

**好處**: 每個類別都只依賴「合約」(Interface)，彼此之間**鬆散耦合**，非常容易抽換零件或進行單元測試。

---

## 4. 實際流程：一個檔案上傳請求的旅程

以「股東會明細上傳」為例：

1.  **`WinFormsClient` (`Form1.cs`)**: 使用者點擊「確定」，`btnConfirm_Click` 方法觸發。
2.  **`ApiClient.cs` (WinForms 內部)**: (建議將 HTTP 邏輯封裝於此) 建立 `MultipartFormDataContent`，包含使用者選擇的檔案。
3.  **`HttpClient`**: 發送 `POST` 請求到 `https://localhost:7036/api/ShareholderMeetings/details/upload`。
4.  **`DmsSystem.Api` (`ShareholderMeetingsController.cs`)**:
    * ASP.NET Core 框架接收請求，路由到 `UploadDetails` 方法。
    * 框架透過 DI 容器建立 `ShareholderMeetingsController` 實例，並自動注入 `IShareholderMeetingDetailService`。
    * `UploadDetails` 方法被執行，它從請求中取得 `IFormFile file`。
    * Controller 將 `file` 的 `Stream` 和 `FileName` 傳遞給 `_detailService.ProcessUploadAsync(...)`。
5.  **`DmsSystem.Infrastructure` (`ShareholderMeetingDetailService.cs`)**:
    * `ProcessUploadAsync` 方法被執行。
    * 它檢查副檔名，選擇 `ParseCsvStream` 或 `ParseXlsxStream`。
    * 使用 `NPOI` 或 `CsvHelper` 解析 `Stream`，並建立 `List<ShmtSource1>` 物件。
    * 呼叫 `_repository.AddRangeAsync(entitiesToInsert)`。(`_repository` 是 DI 容器注入的 `ShmtSource1Repository`)。
6.  **`DmsSystem.Infrastructure` (`ShmtSource1Repository.cs`)**:
    * `AddRangeAsync` 方法被執行。
    * 它使用 `_context.ShmtSource1s.AddRangeAsync()` 將 Entities 加入 EF Core 的追蹤。
    * 呼叫 `_context.SaveChangesAsync()` 將變更寫入資料庫。(`_context` 是 DI 容器注入的 `DmsDbContext`)。
7.  **回傳**: 結果一路從 Repository -> Service -> Controller -> API Response -> WinForms Client -> MessageBox 顯示給使用者。

---

## 5. 如何新增功能？(標準作業流程 SOP)

未來要新增任何功能（例如新的查詢 API 或新的檔案上傳），請遵循以下模式：

1.  **定義需求**: 明確新功能需要操作哪些資料 (`Domain` Entities)。
2.  **建立 Repository (如果需要)**:
    * 在 `Application\Interfaces` 建立新的 `I...Repository` 介面，定義所需的操作。
    * 在 `Infrastructure\Persistence\Repositories` 建立對應的實作類別。
3.  **建立 Service (如果需要)**:
    * 在 `Application\Interfaces` 建立新的 `I...Service` 介面，定義業務邏輯。
    * 在 `Infrastructure\Services` (或 `Application\Services`，根據您的最終決定) 建立對應的實作類別，注入所需的 Repository。
4.  **建立 Controller / Action**:
    * 找到對應業務領域的 Controller (或建立新的)。
    * 新增 Action 方法，注入所需的 Service 介面，並呼叫其方法。
5.  **註冊 DI**: 在 `Api\Program.cs` 中，註冊所有新建的介面與實作的對應關係。

---

## 6. 如何在本機執行？

1.  **確保環境**: 安裝 .NET 8 SDK 和 Visual Studio 2022 (含 ASP.NET 和 .NET 桌面開發工作負載)。
2.  **取得程式碼**: (您已完成) 使用 Git Clone 下載專案。
3.  **還原套件**: 在 Visual Studio 中打開 `.sln` 方案檔，VS 會自動還原所有 NuGet 套件。如果沒有，請手動執行「重建方案」。
4.  **設定資料庫連線**: 修改 `DmsSystem.Api` 專案下的 `appsettings.Development.json`，將 `ConnectionStrings:DefaultConnection` 設定為您本機或測試資料庫的連線字串。
5.  **設定啟始專案**:
    * 右鍵點擊方案 -> 「設定啟始專案」。
    * 選擇「多個啟始專案」。
    * 將 `DmsSystem.Api` 和 `DmsSystem.WinFormsClient` 的動作都設為「啟動」。
6.  **執行**: 按下 F5 或點擊綠色執行按鈕。

---

## 7. 重要觀念與下一步 (架構師建議)

* **DTOs (Data Transfer Objects)**: 引入 DTOs 來避免直接暴露 Domain Entities 給 API 消費者。
* **輸入驗證 (Input Validation)**: 使用 FluentValidation 等工具來驗證 API 的輸入參數。
* **全域錯誤處理 (Global Exception Handling)**: 建立 Middleware 來統一處理未預期的錯誤。
* **結構化日誌 (Structured Logging)**: 引入 Serilog 等工具來記錄詳細的應用程式日誌。
* **單元/整合測試**: 為 Service 和 Repository 撰寫自動化測試，確保程式碼品質。
* **非同步 (`async/await`)**: 確保所有 I/O 操作都使用非同步模式。

希望這份文件能幫助您更好地理解和掌握這個專案的架構！