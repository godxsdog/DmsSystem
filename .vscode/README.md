# VS Code 設定說明

本資料夾包含 Visual Studio Code 的專案設定檔，讓您可以在 VS Code 中開發和執行 DmsSystem 專案。

## 檔案說明

- **launch.json**: 偵錯配置，定義如何啟動和偵錯應用程式
- **tasks.json**: 建置任務，定義常用的 dotnet 命令
- **settings.json**: VS Code 工作區設定，包含 C# 和 OmniSharp 的配置
- **extensions.json**: 推薦的擴充功能清單

## 快速開始

1. **安裝必要的擴充功能**:
   - 開啟 VS Code 時會自動提示安裝推薦的擴充功能
   - 或手動安裝：C# Dev Kit (`ms-dotnettools.csdevkit`)

2. **還原 NuGet 套件**:
   ```bash
   dotnet restore
   ```

3. **設定資料庫連線**:
   - 編輯 `DmsSystem.Api/appsettings.Development.json`
   - 更新 `ConnectionStrings:DefaultConnection`

4. **啟動專案**:
   - 按 `F5` 開始偵錯
   - 或使用命令面板 (`Cmd+Shift+P`) -> "Debug: Start Debugging"
   - 選擇 ".NET Core Launch (API)" 配置

5. **開啟 Swagger UI**:
   - API 啟動後會自動開啟瀏覽器
   - 或手動訪問：http://localhost:5137/swagger

## 可用任務

使用命令面板 (`Cmd+Shift+P`) -> "Tasks: Run Task" 來執行：

- **build**: 建置專案
- **watch**: 監看模式執行（自動重新載入）
- **clean**: 清理建置檔案
- **restore**: 還原 NuGet 套件

## 注意事項

- **WinFormsClient 專案**: 這是 Windows Forms 應用程式，只能在 Windows 上執行。在 macOS/Linux 上無法建置此專案，但 API 專案可以正常運行。
- **資料庫連線**: 確保資料庫伺服器可訪問，且連線字串正確。
- **HTTPS 憑證**: 首次執行時可能需要信任開發憑證：
  ```bash
  dotnet dev-certs https --trust
  ```





