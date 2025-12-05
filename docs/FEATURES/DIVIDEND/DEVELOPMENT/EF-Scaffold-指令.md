# EF Scaffold 一鍵重生指令（配息模組共享）

> 依照 AI_CONTEXT 原則，把重要指令放在文件。適用於配息（DIVIDEND）與整個 DMS 資料模型的重生。

## Visual Studio 套件管理器主控台
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

## dotnet-ef（跨平台 CLI）
在方案根目錄執行：
```
dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer \
  --project DmsSystem.Infrastructure \
  --startup-project DmsSystem.Api \
  --output-dir ../DmsSystem.Domain/Entities \
  --context-dir Persistence/Contexts \
  --namespace DmsSystem.Domain.Entities \
  --context-namespace DmsSystem.Infrastructure.Persistence.Contexts \
  --force
```

### 注意
- `DefaultConnection` 請指向正式區 SQL Server。
- `--force` / `-Force` 會覆寫現有 DbContext 與實體，執行前請確認沒有手動客製的實體程式碼。
- 若僅需配息相關表，可於指令後加上 `--table` / `-Table` 參數指定表名。

