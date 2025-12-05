# DMS 系統文件目錄

本資料夾包含 DMS 系統的所有技術文件。

## 📚 通用文件（所有環境適用）

### 快速開始與基礎
- **[00-快速開始](./00-快速開始.md)** - 5 分鐘快速啟動指南
- **[01-架構指南](./01-架構指南.md)** - 完整的系統架構說明
- **[02-資料庫配置](./02-資料庫配置.md)** - 資料庫連接和環境設定指南
- **[04-測試指南](./04-測試指南.md)** - 測試相關說明（單元測試、整合測試）

### 專案資訊
- **[05-專案完成總結](./05-專案完成總結.md)** - 專案開發完成情況總結
- **[07-架構分析與優勢](./07-架構分析與優勢.md)** - 系統架構詳細分析
- **[08-使用者手冊](./08-使用者手冊.md)** - 完整的使用說明（包含資料流程）
- **[09-執行狀態報告](./09-執行狀態報告.md)** - 系統執行狀態檢查
- **[10-系統測試報告](./10-系統測試報告.md)** - 系統測試報告

---

## 🔀 環境專用文件

### Mac 本地開發測試環境

> ⚠️ **注意**：Mac 環境使用 Docker SQL Server 進行本地測試，因為無法連線到正式區 SQL Server。

**使用情境：**
- 個人開發和測試
- 使用 Docker SQL Server 本地資料庫
- 測試完成後，程式碼會搬到 Windows 正式環境

**位置：** [`MAC-DEVELOPMENT-ONLY/`](./MAC-DEVELOPMENT-ONLY/)

- **[Mac 開發環境完整手冊](./MAC-DEVELOPMENT-ONLY/01-Mac開發環境完整手冊.md)** - Mac 本地開發測試完整指南
- **[測試資料載入](./MAC-DEVELOPMENT-ONLY/02-測試資料載入.md)** - 如何載入測試資料到 Docker SQL Server
- **[Docker SQL Server 設定](./MAC-DEVELOPMENT-ONLY/03-Docker-SQL-Server設定.md)** - Docker 容器設定和操作

### Windows 正式環境

> 📋 **適用對象**：Windows 正式環境，連接正式區 SQL Server。

**使用情境：**
- 正式環境執行和測試
- 連接正式區 SQL Server（已建立）
- 接收從 Mac 環境測試完成的程式碼

**位置：** [`WINDOWS-DEVELOPMENT/`](./WINDOWS-DEVELOPMENT/)

- **[Windows 正式環境手冊](./WINDOWS-DEVELOPMENT/01-Windows開發環境完整手冊.md)** - Windows 正式環境完整指南
- **[Git 版本控制指南](./WINDOWS-DEVELOPMENT/02-Git版本控制指南.md)** - 團隊協作的 Git 工作流程
- **[環境切換指南](./WINDOWS-DEVELOPMENT/03-環境切換指南.md)** - 從 Mac 切換到 Windows 的步驟
- **[資料庫遷移指南](./WINDOWS-DEVELOPMENT/04-資料庫遷移指南.md)** - 從 Mac Docker 資料庫遷移到正式區 SQL Server

---

## 🔄 開發流程說明

### Mac 本地開發流程

1. **在 Mac 環境開發和測試**
   - 使用 Docker SQL Server 作為本地測試資料庫
   - 進行功能開發和測試
   - 確認功能正常運作

2. **提交程式碼**
   - 將測試完成的程式碼提交到 Git
   - 不包含資料庫資料（僅程式碼）

3. **切換到 Windows 正式環境**
   - 在 Windows 環境拉取最新程式碼
   - 連接正式區 SQL Server
   - 進行正式環境測試

### Windows 正式環境流程

1. **取得程式碼**
   - 從 Git 拉取 Mac 環境測試完成的程式碼

2. **設定正式環境**
   - 連接正式區 SQL Server
   - 設定生產環境配置

3. **正式環境測試**
   - 在正式區 SQL Server 上進行測試
   - 驗證功能正常運作

---

## 🚀 快速導覽

### Mac 開發者（本地測試）

1. **閱讀快速開始**：閱讀 [快速開始](./00-快速開始.md)
2. **設定本地環境**：[Mac 開發環境完整手冊](./MAC-DEVELOPMENT-ONLY/01-Mac開發環境完整手冊.md)
3. **載入測試資料**：[測試資料載入](./MAC-DEVELOPMENT-ONLY/02-測試資料載入.md)
4. **Docker 操作**：[Docker SQL Server 設定](./MAC-DEVELOPMENT-ONLY/03-Docker-SQL-Server設定.md)
5. **測試完成後**：參考 [環境切換指南](./WINDOWS-DEVELOPMENT/03-環境切換指南.md) 切換到 Windows

### Windows 開發者（正式環境）

1. **閱讀快速開始**：閱讀 [快速開始](./00-快速開始.md)
2. **設定正式環境**：[Windows 正式環境手冊](./WINDOWS-DEVELOPMENT/01-Windows開發環境完整手冊.md)
3. **取得程式碼**：從 Git 拉取最新程式碼
4. **連接正式區 SQL Server**：設定連接字串
5. **進行正式環境測試**

### 了解系統架構

1. [架構指南](./01-架構指南.md) - 整體設計說明
2. [架構分析與優勢](./07-架構分析與優勢.md) - 設計優勢分析

### 執行測試

1. [測試指南](./04-測試指南.md) - 測試執行說明
2. [系統測試報告](./10-系統測試報告.md) - 測試結果

---

## 📖 文件結構說明

```
docs/
├── 00-快速開始.md              # 通用：快速啟動
├── 01-架構指南.md              # 通用：架構說明
├── 02-資料庫配置.md            # 通用：資料庫配置
├── 04-測試指南.md              # 通用：測試說明
├── 05-專案完成總結.md          # 通用：專案總結
├── 07-架構分析與優勢.md        # 通用：架構分析
├── 08-使用者手冊.md            # 通用：使用說明
├── 09-執行狀態報告.md          # 通用：狀態檢查
├── 10-系統測試報告.md          # 通用：測試報告
│
├── MAC-DEVELOPMENT-ONLY/       # Mac 本地開發測試（Docker SQL Server）
│   ├── 01-Mac開發環境完整手冊.md
│   ├── 02-測試資料載入.md
│   └── 03-Docker-SQL-Server設定.md
│
└── WINDOWS-DEVELOPMENT/        # Windows 正式環境（正式區 SQL Server）
    ├── 01-Windows開發環境完整手冊.md
    ├── 02-Git版本控制指南.md
    ├── 03-環境切換指南.md
    └── 04-資料庫遷移指南.md
```

---

## 📖 其他資源

- **主 README**：`../README.md` - 專案概述和快速開始
- **API 文件**：啟動 API 後訪問 http://localhost:5137/swagger

---

## 💡 重要說明

### Mac 環境
- **資料庫**：Docker SQL Server（本地測試用）
- **用途**：個人開發和測試
- **限制**：無法連線到正式區 SQL Server，因此使用本地 Docker

### Windows 環境
- **資料庫**：正式區 SQL Server（已建立）
- **用途**：正式環境執行和測試
- **流程**：接收 Mac 環境測試完成的程式碼，在正式區 SQL Server 上進行測試

---

**開始使用：** 請根據您的環境選擇對應的手冊開始。
