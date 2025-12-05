# EF Scaffold 一鍵重生指令（配息模組共享）

> 依照 AI_CONTEXT 原則，把重要指令放在文件。適用於配息（DIVIDEND）與整個 DMS 資料模型的重生。

## ⚠️ 重要：執行環境說明

**`Scaffold-DbContext` 命令只能在 Visual Studio 的套件管理器主控台（Package Manager Console）中使用！**

- ✅ **正確**：在 Visual Studio 中開啟專案 → 工具 → NuGet 套件管理器 → 套件管理器主控台
- ❌ **錯誤**：在 PowerShell、CMD 或終端機中直接執行 `Scaffold-DbContext`（會出現「無法辨識」錯誤）

如果要在命令列執行，請使用 `dotnet ef` 工具（見下方說明）。

---

## 方式一：Visual Studio 套件管理器主控台（推薦）

**步驟**：
1. 在 Visual Studio 2022 中開啟 `DMS.sln`
2. 工具 → NuGet 套件管理器 → 套件管理器主控台
3. 確認「預設專案」選擇 `DmsSystem.Infrastructure`
4. 複製以下指令並貼上執行：

```
Scaffold-DbContext "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer `
  -Project "DmsSystem.Infrastructure" `
  -StartupProject "DmsSystem.Api" `
  -OutputDir "..\DmsSystem.Domain\Entities" `
  -ContextDir "Persistence\Contexts" `
  -Namespace "DmsSystem.Domain.Entities" `
  -ContextNamespace "DmsSystem.Infrastructure.Persistence.Contexts" `
  -Force
```

---

## 方式二：dotnet-ef CLI（命令列）

**前置步驟：安裝 dotnet-ef 工具**

如果尚未安裝，請先執行：

```powershell
dotnet tool install --global dotnet-ef
```

**執行步驟**：
1. 開啟 PowerShell 或命令提示字元
2. 切換到方案根目錄（`DmsSystem/`）
3. 執行以下指令：

```powershell
dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer `
  --project DmsSystem.Infrastructure `
  --startup-project DmsSystem.Api `
  --output-dir ../DmsSystem.Domain/Entities `
  --context-dir Persistence/Contexts `
  --namespace DmsSystem.Domain.Entities `
  --context-namespace DmsSystem.Infrastructure.Persistence.Contexts `
  --force
```

---

## 故障排除

### ❌ 錯誤：`Scaffold-DbContext : 無法辨識 'Scaffold-DbContext' 詞彙...`

**原因**：在 PowerShell/CMD 中執行，而非 Visual Studio 套件管理器主控台。

**解決方案**：
1. **方案 A**：使用 Visual Studio 套件管理器主控台（見上方「方式一」）
2. **方案 B**：使用 `dotnet ef` CLI（見上方「方式二」）

### ❌ 錯誤：`找不到命令 'dotnet-ef'` 或 `找不到命令 'dotnet ef'`

**原因**：尚未安裝 dotnet-ef 工具。

**解決方案**：
```powershell
dotnet tool install --global dotnet-ef
```

安裝完成後，重新執行 `dotnet ef` 指令。

### ❌ 錯誤：連接字串無法讀取

**原因**：`appsettings.Production.json` 中的 `DefaultConnection` 設定不正確。

**解決方案**：
1. 確認 `DmsSystem.Api/appsettings.Production.json` 中有正確的連接字串
2. 或使用環境變數設定連接字串

---

## 注意事項

- `DefaultConnection` 請指向正式區 SQL Server。
- `--force` / `-Force` 會**覆寫**現有 DbContext 與實體，執行前請確認：
  - 已備份手動修改的實體類別
  - 或使用版本控制（Git）追蹤變更
- 若僅需配息相關表，可在指令後加上：
  - `--table MDS.FUND_DIV`（僅產生 FUND_DIV 表）
  - `--table MDS.FUND_DIV --table MDS.FUND_DIV_SET`（多個表）

---

## 相關文件

- [資料庫配置指南](../../../03-資料庫配置.md) - 完整的資料庫設定說明

