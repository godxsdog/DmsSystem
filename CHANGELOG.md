# 變更日誌

## [未發布] - 2024-12-05

### 移除
- **移除 WinForms 客戶端**：已移除 `DmsSystem.WinFormsClient` 專案及相關檔案
  - 原因：系統現在以 React 網頁應用程式為主
  - 影響：不再支援 Windows Forms 桌面應用程式
  - 替代方案：使用 React 網頁前端（`react-client/`）

### 文件更新
- **文件結構整理**：
  - Mac 本地開發測試環境文件移至 `docs/MAC-DEVELOPMENT-ONLY/`
  - Windows 正式環境文件移至 `docs/WINDOWS-DEVELOPMENT/`
  - 通用文件保留在 `docs/` 根目錄
- **明確環境區分**：
  - Mac：使用 Docker SQL Server 進行本地測試（無法連線到正式區 SQL Server）
  - Windows：連接正式區 SQL Server 進行正式環境測試
- **新增資料庫遷移指南**：`docs/WINDOWS-DEVELOPMENT/04-資料庫遷移指南.md`

### 技術變更
- 前端技術棧：**React + TypeScript + Vite**（網頁應用程式）
- 後端 API：支援 CORS，可與 React 前端整合

---

## 開發流程說明

### Mac 本地開發流程
1. 在 Mac 環境使用 Docker SQL Server 進行開發和測試
2. 測試完成後，將程式碼提交到 Git
3. 在 Windows 環境拉取程式碼，連接正式區 SQL Server 進行正式測試

### Windows 正式環境流程
1. 從 Git 拉取 Mac 環境測試完成的程式碼
2. 連接正式區 SQL Server
3. 進行正式環境測試

---

**注意**：系統現在以 React 網頁應用程式為主，不再支援 Windows Forms 桌面應用程式。

