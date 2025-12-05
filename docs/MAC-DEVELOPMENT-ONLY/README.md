# Mac 本地開發測試環境專用手冊

> ⚠️ **注意**：此文件夾內容僅供 Mac 開發環境使用，包含測試資料和個人開發設定。

## 📁 文件列表

- **[00-快速測試指南](./00-快速測試指南.md)** - ⭐ **從這裡開始**：完整的測試步驟，包含網頁操作和資料驗證
- **[01-Mac開發環境完整手冊](./01-Mac開發環境完整手冊.md)** - Mac 環境完整指南
- **[02-測試資料載入](./02-測試資料載入.md)** - 如何載入測試資料（僅 Mac 使用）
- **[03-Docker SQL Server 設定](./03-Docker-SQL-Server設定.md)** - Docker 容器設定

---

## 🎯 使用說明

此文件夾的所有內容**僅供 Mac 開發環境使用**，包含：

1. **測試資料**：僅用於個人開發測試
2. **Docker 設定**：Mac 環境的 SQL Server Docker 容器
3. **開發工具**：Mac 特定的開發工具設定

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
2. **基金配息管理**（DIVIDEND）
3. **全委投資系統**（INVESTMENT）

**詳細說明**：請參考 [`../FEATURES/`](../FEATURES/) 資料夾

---

## 💾 資料庫架構

### Docker SQL Server 設定

**資料庫連接字串**：
```
Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True
```

**位置**：`DmsSystem.Api/appsettings.Development.json`

### 資料庫架設

**啟動方式**：
```bash
docker-compose up -d
```

**資料庫初始化**：
- 使用 `scripts/init-db.sql` 初始化資料庫結構
- 使用 `scripts/seed-test-data.sql` 載入測試資料

**注意**：
- 此環境僅用於本地測試
- 測試資料僅供個人開發使用
- 不包含正式資料

---

## ⚠️ 重要提醒

- **不要**將測試資料腳本提交到正式環境
- **不要**將 Docker 設定用於 Windows 環境
- 此文件夾內容**不會**出現在 Windows 同事的文件中
- 測試完成後，程式碼需提交到 Git，在 Windows 環境進行正式測試

---

## 🔗 相關文件

### 通用文件
- [快速開始](../00-快速開始.md) - 5 分鐘快速啟動
- [架構指南](../01-架構指南.md) - 系統架構說明
- [架構分析與優勢](../02-架構分析與優勢.md) - 架構設計分析
- [資料庫配置](../03-資料庫配置.md) - 資料庫設定

### 功能模組
- [功能模組文件](../FEATURES/) - 各功能模組的開發和使用文件

### Windows 環境
- [Windows 正式環境](../WINDOWS-DEVELOPMENT/) - Windows 正式環境文件（僅供參考，不要使用）

---

**開始使用**：請先閱讀 [快速測試指南](./00-快速測試指南.md)
