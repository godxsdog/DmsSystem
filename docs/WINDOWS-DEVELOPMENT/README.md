# Windows 正式環境文件

> 📋 **適用對象**：所有使用 Windows 和正式區 SQL Server 進行開發的團隊成員

## 📁 文件列表

### 環境設定
- **[01-Windows開發環境完整手冊](./01-Windows開發環境完整手冊.md)** - Windows 環境完整設定指南
  - 環境準備
  - 資料庫連接設定
  - 應用程式啟動
  - 錯誤診斷

### 版本控制與協作
- **[02-Git版本控制指南](./02-Git版本控制指南.md)** - 團隊協作的 Git 工作流程
  - 分支策略
  - Commit 訊息規範
  - Pull Request 流程

### 程式碼更新
- **[03-環境切換指南](./03-環境切換指南.md)** - 取得最新程式碼和更新指南
  - 從 Git 拉取最新程式碼
  - 分支切換
  - 提交變更

### 資料庫操作
- **[04-資料庫遷移指南](./04-資料庫遷移指南.md)** - 資料庫備份、還原和操作指南
  - 資料庫備份
  - 資料庫還原
  - 資料驗證

---

## 🎯 快速開始

### 首次設定

1. **取得程式碼**：
```powershell
git clone <repository-url>
cd DmsSystem
```

2. **還原套件**：
```powershell
dotnet restore
```

3. **設定資料庫連接**：
   - 編輯 `DmsSystem.Api/appsettings.Production.json`
   - 或使用環境變數

4. **啟動專案**：
   - 開啟 Visual Studio 2022
   - 開啟 `DMS.sln`
   - 按 F5 執行

---

## 🏗️ 架構說明

### 系統架構

DMS 系統採用**洋蔥式架構（Onion Architecture）**，分為四層：

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

### 功能模組

系統包含三大功能模組：

1. **股東會資料管理**（SHAREHOLDER-MEETING）
   - 股東會資料上傳與管理
   - 公司資訊管理
   - 股票餘額管理

2. **基金配息管理**（DIVIDEND）
   - 配息參數設定
   - 可分配餘額維護
   - 配息計算與分攤

3. **全委投資系統**（INVESTMENT）
   - （待開發）

**詳細說明**：請參考 [`../FEATURES/`](../FEATURES/) 資料夾

---

## 🔧 環境配置

### 資料庫連接

**位置**：`DmsSystem.Api/appsettings.Production.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=正式伺服器位址;Database=DMS;User Id=正式使用者;Password=正式密碼;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

### 前端 API 設定

**位置**：`react-client/.env.production`

```
VITE_API_BASE_URL=http://正式API位址:5137
```

---

## ⚠️ 重要提醒

### 環境說明

- **Windows 環境**：連接正式區 SQL Server 進行正式測試
- **資料庫**：正式區 SQL Server（已建立）
- **用途**：正式環境執行和生產部署

### 開發流程

1. **取得最新程式碼**：從 Git 倉庫拉取最新程式碼
2. **設定環境**：設定資料庫連接字串
3. **啟動系統**：使用 Visual Studio 2022 或命令列啟動
4. **測試驗證**：測試系統功能是否正常

### 前端技術

**系統現在以 React 網頁應用程式為主**，不再支援 Windows Forms 桌面應用程式。

---

## 🔗 相關文件

### 通用文件
- [快速開始](../00-快速開始.md) - 5 分鐘快速啟動
- [架構指南](../01-架構指南.md) - 系統架構說明
- [架構分析與優勢](../02-架構分析與優勢.md) - 架構設計分析
- [資料庫配置](../03-資料庫配置.md) - 資料庫設定
- [測試指南](../04-測試指南.md) - 測試相關說明

### 功能模組
- [功能模組文件](../FEATURES/) - 各功能模組的開發和使用文件

---

**開始使用**：請先閱讀 [Windows 開發環境完整手冊](./01-Windows開發環境完整手冊.md)
