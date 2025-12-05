# DMS 系統

股東會資料管理系統（DMS System）是一個基於 .NET 8.0 和 React 的現代化應用程式。

## 📚 文件導覽

### 快速開始

- **[📖 文件目錄](./docs/README.md)** - 所有技術文件的完整索引
- **[🚀 快速開始](./docs/00-快速開始.md)** - 5 分鐘快速啟動指南

### 環境設定

根據您的環境選擇對應的手冊：

- **Mac 開發環境** → [Mac 開發環境手冊](./docs/08-1-Mac開發環境手冊.md)
- **Windows 正式環境** → [Windows 正式環境手冊](./docs/08-2-Windows正式環境手冊.md)

### 核心文件

所有技術文件位於 [`docs/`](./docs/) 資料夾，包含：

1. **[架構指南](./docs/01-架構指南.md)** - 完整的系統架構說明
2. **[資料庫配置](./docs/02-資料庫配置.md)** - 資料庫設定指南
3. **[測試資料載入](./docs/03-測試資料載入.md)** - 如何載入測試資料
4. **[測試指南](./docs/04-測試指南.md)** - 測試相關說明
5. **[專案完成總結](./docs/05-專案完成總結.md)** - 開發完成情況
6. **[SQL Server 設定](./docs/06-SQL-Server設定.md)** - SQL Server 設定指南
7. **[架構分析與優勢](./docs/07-架構分析與優勢.md)** - 架構設計分析
8. **[使用者手冊](./docs/08-使用者手冊.md)** - 完整的使用說明（包含資料流程）
9. **[執行狀態報告](./docs/09-執行狀態報告.md)** - 系統執行狀態檢查

### 如何使用文件

#### 首次使用系統

1. 閱讀 [快速開始](./docs/00-快速開始.md) 快速啟動系統
2. 根據您的環境選擇對應手冊：
   - Mac：閱讀 [Mac 開發環境手冊](./docs/08-1-Mac開發環境手冊.md)
   - Windows：閱讀 [Windows 正式環境手冊](./docs/08-2-Windows正式環境手冊.md)

#### 了解系統架構

1. 閱讀 [架構指南](./docs/01-架構指南.md) 了解整體設計
2. 閱讀 [架構分析與優勢](./docs/07-架構分析與優勢.md) 了解設計優勢

#### 設定資料庫

1. 閱讀 [資料庫配置](./docs/02-資料庫配置.md) 了解如何設定連接
2. 閱讀 [SQL Server 設定](./docs/06-SQL-Server設定.md) 了解 SQL Server 設定

#### 載入測試資料

閱讀 [測試資料載入](./docs/03-測試資料載入.md) 了解如何載入測試資料

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
│   ├── 00-快速開始.md
│   ├── 01-架構指南.md
│   ├── 02-資料庫配置.md
│   ├── 03-測試資料載入.md
│   ├── 04-測試指南.md
│   ├── 05-專案完成總結.md
│   ├── 06-SQL-Server設定.md
│   ├── 07-架構分析與優勢.md
│   ├── 08-使用者手冊.md
│   ├── 08-1-Mac開發環境手冊.md
│   ├── 08-2-Windows正式環境手冊.md
│   ├── 09-執行狀態報告.md
│   └── README.md            # 文件目錄索引
│
├── scripts/                 # 🔧 工具腳本
│   ├── init-db.sql         # 資料庫初始化
│   ├── seed-test-data.sql  # 測試資料
│   ├── start-sqlserver.sh  # 啟動 SQL Server
│   ├── stop-sqlserver.sh   # 停止 SQL Server
│   ├── load-test-data.sh   # 載入測試資料
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

## 🚀 快速啟動（3 步驟）

### 1. 啟動資料庫

```bash
docker-compose up -d
```

### 2. 啟動後端 API

```bash
cd DmsSystem.Api
dotnet run
```

### 3. 啟動前端

```bash
cd react-client
npm install  # 首次執行
npm run dev
```

**詳細步驟請參考：** [快速開始指南](./docs/00-快速開始.md)

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
- **架構**：洋蔥式架構（Onion Architecture）

## 📖 更多資訊

- 完整文件：請查看 [`docs/`](./docs/) 資料夾
- API 文件：啟動 API 後訪問 http://localhost:5137/swagger

---

**開始使用：** 請先閱讀 [快速開始指南](./docs/00-快速開始.md) 或根據您的環境選擇對應的手冊。
