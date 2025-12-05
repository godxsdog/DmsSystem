# DMS 系統

股東會資料管理系統（DMS System）是一個基於 .NET 8.0 和 React 的現代化應用程式。

## 📚 文件導覽

### 快速開始

- **[📖 文件目錄](./docs/README.md)** - 所有技術文件的完整索引
- **[🚀 快速開始](./docs/00-快速開始.md)** - 5 分鐘快速啟動指南

### 環境設定

根據您的環境選擇對應的手冊：

- **Mac 本地開發測試環境（Docker SQL Server）** → [Mac 開發環境完整手冊](./docs/MAC-DEVELOPMENT-ONLY/01-Mac開發環境完整手冊.md)
- **Windows 正式環境（正式區 SQL Server）** → [Windows 正式環境手冊](./docs/WINDOWS-DEVELOPMENT/01-Windows開發環境完整手冊.md)

### 核心文件（通用）

所有技術文件位於 [`docs/`](./docs/) 資料夾，包含：

1. **[架構指南](./docs/01-架構指南.md)** - 完整的系統架構說明
2. **[資料庫配置](./docs/02-資料庫配置.md)** - 資料庫設定指南
3. **[測試指南](./docs/04-測試指南.md)** - 測試相關說明
4. **[專案完成總結](./docs/05-專案完成總結.md)** - 開發完成情況
5. **[架構分析與優勢](./docs/07-架構分析與優勢.md)** - 架構設計分析
6. **[使用者手冊](./docs/08-使用者手冊.md)** - 完整的使用說明（包含資料流程）
7. **[執行狀態報告](./docs/09-執行狀態報告.md)** - 系統執行狀態檢查
8. **[系統測試報告](./docs/10-系統測試報告.md)** - 系統測試報告

### 環境專用文件

- **Mac 本地開發測試環境**：位於 [`docs/MAC-DEVELOPMENT-ONLY/`](./docs/MAC-DEVELOPMENT-ONLY/)
  - 使用 Docker SQL Server 進行本地測試（因為無法連線到正式區 SQL Server）
  - 個人開發和測試使用
  
- **Windows 正式環境**：位於 [`docs/WINDOWS-DEVELOPMENT/`](./docs/WINDOWS-DEVELOPMENT/)
  - 連接正式區 SQL Server（已建立）
  - 正式環境執行和測試

### 開發流程

1. **Mac 本地開發**：在 Mac 環境使用 Docker SQL Server 進行開發和測試
2. **提交程式碼**：測試完成後，將程式碼提交到 Git
3. **Windows 正式環境**：在 Windows 環境拉取程式碼，連接正式區 SQL Server 進行正式測試

### 如何使用文件

#### Mac 開發者（本地測試）

1. 閱讀 [快速開始](./docs/00-快速開始.md) 快速啟動系統
2. 閱讀 [Mac 開發環境完整手冊](./docs/MAC-DEVELOPMENT-ONLY/01-Mac開發環境完整手冊.md) 設定本地環境
3. 使用 Docker SQL Server 進行測試
4. 測試完成後，參考 [環境切換指南](./docs/WINDOWS-DEVELOPMENT/03-環境切換指南.md) 切換到 Windows

#### Windows 開發者（正式環境）

1. 閱讀 [快速開始](./docs/00-快速開始.md) 快速啟動系統
2. 閱讀 [Windows 正式環境手冊](./docs/WINDOWS-DEVELOPMENT/01-Windows開發環境完整手冊.md) 設定正式環境
3. 從 Git 拉取 Mac 環境測試完成的程式碼
4. 連接正式區 SQL Server 進行正式環境測試

#### 了解系統架構

1. 閱讀 [架構指南](./docs/01-架構指南.md) 了解整體設計
2. 閱讀 [架構分析與優勢](./docs/07-架構分析與優勢.md) 了解設計優勢

#### 設定資料庫

1. 閱讀 [資料庫配置](./docs/02-資料庫配置.md) 了解如何設定連接
2. **Mac 環境**：參考 [Docker SQL Server 設定](./docs/MAC-DEVELOPMENT-ONLY/03-Docker-SQL-Server設定.md)（本地測試用）
3. **Windows 環境**：參考 [Windows 正式環境手冊](./docs/WINDOWS-DEVELOPMENT/01-Windows開發環境完整手冊.md)（正式區 SQL Server）

#### 資料庫遷移（Mac → Windows）

閱讀 [資料庫遷移指南](./docs/WINDOWS-DEVELOPMENT/04-資料庫遷移指南.md) 了解如何從 Mac Docker 資料庫遷移到正式區 SQL Server

#### 執行測試

閱讀 [測試指南](./docs/04-測試指南.md) 了解如何執行測試

#### 遇到問題

1. 根據環境選擇對應手冊的「錯誤診斷」章節
2. 執行錯誤報告產生器：
   - Mac：`./scripts/generate-error-report.sh`
   - Windows：執行 `generate-error-report.bat`（需建立）
3. 查看 [執行狀態報告](./docs/09-執行狀態報告.md)

## 🏗️ 專案結構

```
DmsSystem/
├── docs/                    # 📚 所有技術文件
│   ├── 00-快速開始.md      # 通用：快速啟動
│   ├── 01-架構指南.md      # 通用：架構說明
│   ├── 02-資料庫配置.md    # 通用：資料庫配置
│   ├── 04-測試指南.md      # 通用：測試說明
│   ├── 05-專案完成總結.md  # 通用：專案總結
│   ├── 07-架構分析與優勢.md # 通用：架構分析
│   ├── 08-使用者手冊.md    # 通用：使用說明
│   ├── 09-執行狀態報告.md  # 通用：狀態檢查
│   ├── 10-系統測試報告.md  # 通用：測試報告
│   │
│   ├── MAC-DEVELOPMENT-ONLY/  # Mac 本地開發測試（Docker SQL Server）
│   │   ├── 01-Mac開發環境完整手冊.md
│   │   ├── 02-測試資料載入.md
│   │   └── 03-Docker-SQL-Server設定.md
│   │
│   └── WINDOWS-DEVELOPMENT/   # Windows 正式環境（正式區 SQL Server）
│       ├── 01-Windows開發環境完整手冊.md
│       ├── 02-Git版本控制指南.md
│       ├── 03-環境切換指南.md
│       └── 04-資料庫遷移指南.md
│
├── scripts/                 # 🔧 工具腳本
│   ├── init-db.sql         # 資料庫初始化
│   ├── seed-test-data.sql  # 測試資料（Mac 本地測試用）
│   ├── start-sqlserver.sh  # 啟動 SQL Server（Mac Docker）
│   ├── stop-sqlserver.sh   # 停止 SQL Server（Mac Docker）
│   ├── load-test-data.sh   # 載入測試資料（Mac Docker）
│   └── generate-error-report.sh  # 錯誤報告產生器
│
├── DmsSystem.Domain/        # 領域實體層
├── DmsSystem.Application/  # 應用程式層
├── DmsSystem.Infrastructure/ # 基礎設施層
├── DmsSystem.Api/          # API 表現層
├── DmsSystem.Tests/        # 測試專案
├── react-client/           # React 前端
└── README.md              # 本文件
```

## 🚀 快速啟動

### Mac 環境（本地測試，Docker SQL Server）

```bash
# 1. 啟動 Docker SQL Server
docker-compose up -d

# 2. 啟動後端 API
cd DmsSystem.Api
dotnet run

# 3. 啟動前端
cd react-client
npm install  # 首次執行
npm run dev
```

### Windows 環境（正式環境，正式區 SQL Server）

1. 確認正式區 SQL Server 已運行
2. 設定連接字串（`appsettings.Production.json`）
3. 啟動 API：`dotnet run`
4. 啟動前端：`npm run dev`

**詳細步驟請參考：** 
- Mac： [Mac 開發環境完整手冊](./docs/MAC-DEVELOPMENT-ONLY/01-Mac開發環境完整手冊.md)
- Windows： [Windows 正式環境手冊](./docs/WINDOWS-DEVELOPMENT/01-Windows開發環境完整手冊.md)

## 📋 功能清單

- ✅ 股東會資料上傳（Excel/CSV）
- ✅ 公司資訊上傳
- ✅ 股票餘額上傳
- ✅ 資料查詢與顯示
- ✅ 報表產生

## 🛠️ 技術棧

- **後端**：.NET 8.0, ASP.NET Core, Entity Framework Core
- **前端**：React, TypeScript, Vite
- **資料庫**：SQL Server
  - **Mac 本地測試**：Docker SQL Server
  - **Windows 正式環境**：正式區 SQL Server
- **架構**：洋蔥式架構（Onion Architecture）

## 📖 更多資訊

- 完整文件：請查看 [`docs/`](./docs/) 資料夾
- API 文件：啟動 API 後訪問 http://localhost:5137/swagger

---

**開始使用：** 請先閱讀 [快速開始指南](./docs/00-快速開始.md) 或根據您的環境選擇對應的手冊。
