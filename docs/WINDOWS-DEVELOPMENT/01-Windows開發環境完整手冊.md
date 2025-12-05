# Windows 正式環境使用手冊

## 🖥️ 環境說明

- **作業系統**：Windows
- **開發工具**：Visual Studio 2022
- **資料庫**：正式 SQL Server（已建立）
- **用途**：正式環境執行、生產部署

## 🚀 首次設定

### 步驟 1：取得程式碼

```powershell
git clone <repository-url>
cd DmsSystem
```

### 步驟 2：還原 NuGet 套件

**在 Visual Studio 2022：**
1. 開啟 `DMS.sln`
2. 右鍵點擊方案 → 「還原 NuGet 套件」

**或使用命令列：**
```powershell
dotnet restore
```

### 步驟 3：設定資料庫連接

**詳細說明**：請參考 [資料庫配置指南](../03-資料庫配置.md)

**快速設定**：
- 編輯 `DmsSystem.Api/appsettings.Production.json`
- 設定正式區 SQL Server 連接字串

### 步驟 4：驗證資料庫連接

使用 SQL Server Management Studio (SSMS) 或命令列測試連接。

**詳細說明**：請參考 [資料庫配置指南](../03-資料庫配置.md) 的「驗證連接」章節

## 🚀 啟動系統

### 方法一：使用 Visual Studio 2022（推薦）

1. **開啟方案：**
   - 開啟 Visual Studio 2022
   - 檔案 → 開啟 → 專案/方案
   - 選擇 `DMS.sln`

2. **設定啟動專案：**
   - 在方案總管中，右鍵點擊 `DmsSystem.Api`
   - 選擇「設為起始專案」

3. **設定環境：**
   - 在工具列選擇「Production」或「Release」
   - 或修改 `launchSettings.json` 中的環境變數

4. **啟動：**
   - 按 `F5` 或點擊「開始」按鈕
   - API 將在設定的 URL 啟動（通常是 http://localhost:5137）

### 方法二：使用命令列

```powershell
cd DmsSystem.Api
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run
```

## ⚙️ 環境配置

### 資料庫連接字串設定

**詳細說明**：請參考 [資料庫配置指南](../03-資料庫配置.md)

### 前端配置（如果需要）

**位置**：`react-client/.env.production`

```
VITE_API_BASE_URL=http://正式API位址:5137
```

## 🔍 錯誤診斷與報告

### 產生錯誤報告（Windows）

建立批次檔 `generate-error-report.bat`：

```batch
@echo off
echo === DMS 系統錯誤報告 === > ERROR_REPORT.txt
echo 生成時間: %date% %time% >> ERROR_REPORT.txt
echo. >> ERROR_REPORT.txt

echo === 系統資訊 === >> ERROR_REPORT.txt
echo 作業系統: %OS% >> ERROR_REPORT.txt
ver >> ERROR_REPORT.txt
echo .NET 版本: >> ERROR_REPORT.txt
dotnet --version >> ERROR_REPORT.txt 2>&1
echo Node 版本: >> ERROR_REPORT.txt
node --version >> ERROR_REPORT.txt 2>&1
echo. >> ERROR_REPORT.txt

echo === API 建置狀態 === >> ERROR_REPORT.txt
cd DmsSystem.Api
dotnet build >> ..\ERROR_REPORT.txt 2>&1
cd .. >> ERROR_REPORT.txt
echo. >> ERROR_REPORT.txt

echo === 環境變數 === >> ERROR_REPORT.txt
echo ASPNETCORE_ENVIRONMENT: %ASPNETCORE_ENVIRONMENT% >> ERROR_REPORT.txt
echo ConnectionStrings__DefaultConnection: %ConnectionStrings__DefaultConnection% >> ERROR_REPORT.txt
echo. >> ERROR_REPORT.txt

echo === Git 狀態 === >> ERROR_REPORT.txt
git status --short >> ERROR_REPORT.txt
echo. >> ERROR_REPORT.txt

echo === 分支資訊 === >> ERROR_REPORT.txt
git branch --show-current >> ERROR_REPORT.txt
git log -1 --oneline >> ERROR_REPORT.txt

echo 錯誤報告已產生：ERROR_REPORT.txt
notepad ERROR_REPORT.txt
```

**使用方式：**
1. 雙擊執行 `generate-error-report.bat`
2. 將產生的 `ERROR_REPORT.txt` 內容複製
3. 提供給開發人員進行除錯

### 常見錯誤與解決

#### 錯誤 1：資料庫連接失敗

**錯誤訊息：**
```
A network-related or instance-specific error occurred while establishing a connection to SQL Server
```

**解決步驟：**
1. 確認 SQL Server 服務正在運行
2. 確認防火牆允許連接
3. 測試連接（參考 [資料庫配置指南](../03-資料庫配置.md)）
4. 檢查連接字串格式是否正確
5. 確認使用者帳號有足夠權限

#### 錯誤 2：找不到組件

**錯誤訊息：**
```
Could not load file or assembly
```

**解決步驟：**
```powershell
dotnet clean
dotnet restore
dotnet build
```

#### 錯誤 3：連接字串未設定

**錯誤訊息：**
```
資料庫連接字串未設定
```

**解決步驟：**
1. 確認 `appsettings.Production.json` 存在
2. 確認連接字串格式正確
3. 或使用環境變數設定

**詳細說明**：請參考 [資料庫配置指南](../03-資料庫配置.md)

## 🏗️ 建置與部署

### 建置專案

**在 Visual Studio 2022：**
1. 建置 → 重建方案（或按 `Ctrl+Shift+B`）

**或使用命令列：**
```powershell
dotnet build --configuration Release
```

### 發佈專案

**發佈為可執行檔：**
```powershell
cd DmsSystem.Api
dotnet publish -c Release -o ./publish
```

**發佈檔案位置：** `DmsSystem.Api/publish/`

### 部署到 IIS（可選）

1. 安裝 .NET 8.0 Hosting Bundle
2. 在 IIS 中建立應用程式集區（使用 .NET CLR 版本：無 Managed 程式碼）
3. 建立網站，指向發佈資料夾
4. 設定應用程式集區

## 📋 檢查清單

### 首次設定
- [ ] 已安裝 Visual Studio 2022
- [ ] 已安裝 .NET 8 SDK
- [ ] 已取得最新程式碼
- [ ] 已還原 NuGet 套件
- [ ] 已設定資料庫連接字串（參考 [資料庫配置指南](../03-資料庫配置.md)）
- [ ] 已測試資料庫連接

### 每次啟動
- [ ] 確認 SQL Server 正在運行
- [ ] 確認連接字串正確
- [ ] 確認環境變數設定（如使用）
- [ ] 啟動 API 並檢查日誌
- [ ] 測試 Swagger 可訪問

## 🛠️ Visual Studio 2022 使用技巧

### 設定多個啟動專案

如果需要同時啟動 API 和多個專案：

1. 右鍵點擊方案 → 屬性
2. 選擇「多個啟動專案」
3. 設定各專案的動作

### 偵錯設定

1. 在 `launchSettings.json` 中設定環境變數
2. 使用「偵錯」→「視窗」→「輸出」查看日誌
3. 設定中斷點進行除錯

### 查看日誌

**應用程式日誌位置：**
- 檔案日誌：`logs/dms-YYYYMMDD.txt`（在專案目錄下）
- 輸出視窗：在 Visual Studio 的「輸出」視窗查看

## 📚 相關文件

- [快速開始](../00-快速開始.md) - 5 分鐘快速啟動
- [架構指南](../01-架構指南.md) - 系統架構說明
- [資料庫配置](../03-資料庫配置.md) - 資料庫設定（**重要**：資料庫配置請參考此文件）
- [Git 版本控制指南](./02-Git版本控制指南.md) - 團隊協作流程

## ⚠️ 重要提醒

1. **密碼安全**：正式環境的密碼請妥善保管，不要提交到 Git
2. **環境變數**：建議使用環境變數而非直接寫在設定檔中
3. **備份**：部署前請備份資料庫
4. **測試**：部署到正式環境前，請先在測試環境驗證

---

**如有問題，請產生錯誤報告並提供給開發人員。**
