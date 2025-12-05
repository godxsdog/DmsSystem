# Windows 開發環境手冊

> 📋 **適用對象**：所有使用 Windows 和 Visual Studio 2022 進行開發的同事

## 📁 文件列表

- [Windows 開發環境完整手冊](./01-Windows開發環境完整手冊.md) - 完整的 Windows 開發指南
- [版本控制指南](./02-Git版本控制指南.md) - 團隊協作的 Git 工作流程
- [環境切換指南](./03-環境切換指南.md) - 如何在 Mac 和 Windows 之間切換

## 🎯 快速開始

### 首次設定

1. **取得程式碼：**
```powershell
git clone <repository-url>
cd DmsSystem
```

2. **還原套件：**
```powershell
dotnet restore
```

3. **設定資料庫連接：**
   - 編輯 `DmsSystem.Api/appsettings.Production.json`
   - 或使用環境變數

4. **啟動專案：**
   - 開啟 Visual Studio 2022
   - 開啟 `DMS.sln`
   - 按 F5 執行

## 🔄 Git 工作流程

### 基本流程

1. **取得最新程式碼：**
```powershell
git pull origin main
```

2. **建立功能分支：**
```powershell
git checkout -b feature/your-feature-name
```

3. **開發並提交：**
```powershell
git add .
git commit -m "feat: 描述你的變更"
```

4. **推送到遠端：**
```powershell
git push origin feature/your-feature-name
```

5. **建立 Pull Request**

詳細說明請參考：[Git 版本控制指南](./02-Git版本控制指南.md)

## ⚠️ 重要提醒

- **不要**使用 Mac 專用的測試資料腳本
- **不要**修改 Mac 專用的 Docker 設定
- 使用正式環境的資料庫連接字串
- 遵循團隊的 Git 工作流程

## 🔗 相關文件

- 共用文件：請參考 [`../`](../) 根目錄的文件
- Mac 開發環境：請參考 [`../MAC-DEVELOPMENT-ONLY/`](../MAC-DEVELOPMENT-ONLY/)（僅供參考，不要使用）

