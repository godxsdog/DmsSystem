# DMS 系統 - 股東會資料管理系統

## 📋 專案概述

**DMS 系統**是一個現代化的股東會資料管理系統，採用 .NET 8.0 和 React 技術棧開發。

### 技術棧

- **後端**：.NET 8.0, ASP.NET Core, Entity Framework Core, SQL Server
- **前端**：React + TypeScript + Vite
- **架構**：洋蔥式架構（Onion Architecture）
- **測試**：xUnit, Moq, FluentValidation

### 主要功能

- 股東會資料上傳（Excel/CSV）
- 公司資訊上傳
- 股票餘額上傳
- 資料查詢與顯示（React 網頁前端）
- 報表產生
- **基金配息管理**：配息資料匯入、查詢、計算與確認

---

## 📁 專案結構

```
DmsSystem/
├── DmsSystem.Domain/              # 領域實體層
├── DmsSystem.Application/         # 應用程式層
├── DmsSystem.Infrastructure/      # 基礎設施層
├── DmsSystem.Api/                 # API 表現層
├── DmsSystem.Tests/               # 測試專案
├── react-client/                  # React 前端
├── docs/                          # 技術文件
└── scripts/                       # 工具腳本
```

---

## 🚀 快速開始

### Windows 環境（正式區 SQL Server）

1. **取得程式碼**
```powershell
git clone <repository-url>
cd DmsSystem
```

2. **還原套件**
```powershell
dotnet restore
```

3. **設定資料庫連接**
   - 編輯 `DmsSystem.Api/appsettings.Production.json`
   - 設定正式區 SQL Server 連接字串

4. **啟動系統**

**方式一：使用 Visual Studio 2022（推薦）**
   - 開啟 `DMS.sln`
   - 設定啟動專案為 `DmsSystem.Api`
   - 設定環境為 `Production`
   - 按 F5 啟動

**方式二：使用命令列**
```powershell
cd DmsSystem.Api
$env:ASPNETCORE_ENVIRONMENT="Production"
dotnet run
```

5. **啟動前端（新終端視窗）**
```powershell
cd react-client
npm install  # 僅首次需要
npm run dev
```

**詳細說明**：請參考 [快速開始文件](./docs/00-快速開始.md)

### Mac 環境（Docker SQL Server）

1. **啟動資料庫**
```bash
docker-compose up -d
```

2. **啟動 API**
```bash
cd DmsSystem.Api
dotnet run
```

3. **啟動前端**
```bash
cd react-client
npm install  # 僅首次需要
npm run dev
```

**詳細說明**：請參考 [`docs/MAC-DEVELOPMENT-ONLY/`](./docs/MAC-DEVELOPMENT-ONLY/)

### 使用啟動腳本（Mac/Linux）

**啟動後端 API（使用雲端 SQL）**
```bash
./start.sh
```
API 將在 http://localhost:5137 啟動（Swagger 已停用）

**啟動前端（新終端視窗）**
```bash
./start-frontend.sh
```
前端將在 http://localhost:5173 啟動

### 訪問系統

- **前端應用**：http://localhost:5173
- **API 文檔（Swagger）**：http://localhost:5137/swagger（注意：目前 Swagger 已停用）

---

## 📚 文件目錄

### 通用文件

- **[快速開始](./docs/00-快速開始.md)** - Windows 環境快速啟動
- **[架構指南](./docs/01-架構指南.md)** - 系統架構說明
- **[架構分析與優勢](./docs/02-架構分析與優勢.md)** - 架構設計分析
- **[資料庫配置](./docs/03-資料庫配置.md)** - 資料庫設定
- **[測試指南](./docs/04-測試指南.md)** - 測試說明

### Windows 環境文件

所有 Windows 環境相關內容已整合到主要文件中：

- [快速開始](./docs/00-快速開始.md) - 包含完整環境設定、Git 版本控制、錯誤診斷等
- [資料庫配置](./docs/03-資料庫配置.md) - 包含資料庫操作、備份、還原等
- [測試指南](./docs/04-測試指南.md) - 包含執行測試、配息功能測試等

### Mac 環境文件

位置：`docs/MAC-DEVELOPMENT-ONLY/`

- [快速測試指南](./docs/MAC-DEVELOPMENT-ONLY/00-快速測試指南.md)
- [Mac 開發環境完整手冊](./docs/MAC-DEVELOPMENT-ONLY/01-Mac開發環境完整手冊.md)
- [測試資料載入](./docs/MAC-DEVELOPMENT-ONLY/02-測試資料載入.md)
- [Docker SQL Server 設定](./docs/MAC-DEVELOPMENT-ONLY/03-Docker-SQL-Server設定.md)

### 功能模組文件

位置：`docs/FEATURES/`

- [股東會資料管理](./docs/FEATURES/SHAREHOLDER-MEETING/)
- [基金配息管理](./docs/FEATURES/DIVIDEND/)
- [全委投資系統](./docs/FEATURES/INVESTMENT/)

**完整文件說明**：請參考 [`docs/README.md`](./docs/README.md)

---

## 🏗️ 系統架構

### 洋蔥式架構

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

- **Domain**：領域實體定義（Entities）
- **Application**：業務邏輯和介面定義（Services, Interfaces）
- **Infrastructure**：技術實作（Repository, FileParser, FileGenerator）
- **Api**：HTTP 介面（Controllers, Middleware, Validators）

---

## 🔧 環境配置

### Windows 環境

- **資料庫**：正式區 SQL Server
- **連接字串位置**：`DmsSystem.Api/appsettings.Production.json`
- **開發工具**：Visual Studio 2022

### Mac 環境

- **資料庫**：Docker SQL Server（本地測試）
- **連接字串位置**：`DmsSystem.Api/appsettings.Development.json`
- **開發工具**：Visual Studio Code 或終端機

---

## 🧪 測試

### 執行單元測試

```bash
dotnet test DmsSystem.Tests/DmsSystem.Tests.csproj
```

### 測試腳本

```bash
cd DmsSystem.Tests
./RunTests.sh
```

### 配息功能測試

**測試資料**：
- 基金代號：D109, D110, D111, D123, D124, D125 等
- 配息基準日：2020-07-06
- 配息頻率：M (月配)

**測試步驟**：
1. 匯入 CSV 檔案：訪問 http://localhost:5173，點擊「配息管理」標籤，選擇 CSV 檔案並匯入
2. 查詢配息資料：使用查詢功能篩選已載入的配息資料
3. 執行配息計算：輸入基金代號、配息基準日、配息頻率，執行計算與確認

**詳細測試案例**：請參考 [配息功能測試案例](./docs/FEATURES/DIVIDEND/TEST_CASES.md)

---

## 📖 相關資源

- **API 文件**：啟動 API 後訪問 http://localhost:5137/swagger
- **AI 快速進入文件**：[AI_CONTEXT.md](./AI_CONTEXT.md)
- **完整文件**：[docs/README.md](./docs/README.md)

---

## ⚠️ 重要提醒

- **Windows 環境**：連接正式區 SQL Server，用於正式環境執行和生產部署
- **Mac 環境**：使用 Docker SQL Server，用於個人開發和測試
- **前端技術**：系統現在以 React 網頁應用程式為主，不再支援 Windows Forms
- **Swagger**：目前 Swagger 已停用，不會自動開啟瀏覽器
- **API 連接埠**：預設為 http://localhost:5137
- **前端連接埠**：預設為 http://localhost:5173

## 📝 最新修正

所有最新修正內容已記錄在 [最新修正記錄](./docs/10-最新修正記錄.md)，包含：
- 配息功能修正（CsvHelper 配置、CSV 格式處理、空值處理等）
- SQL 語句重構（移至獨立資源類別）
- 錯誤處理改進（前端錯誤顯示、CORS 設定等）
- 配息資料查詢功能

---

**開始使用**：請根據您的環境選擇對應的文件開始。
