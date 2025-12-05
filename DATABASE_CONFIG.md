# 資料庫配置指南

## 快速開始

### Mac 測試環境

1. **啟動 SQL Server Docker 容器**：
```bash
cd /Users/kaichanghuang/Documents/Phoenix\ Code/DmsSystem
docker-compose up -d
```

2. **驗證連接**：
```bash
docker exec -it dms-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'DmsSystem@2024' -d DMS -Q "SELECT DB_NAME()"
```

3. **設定連接字串**（選擇其中一種方式）：

#### 方式 A：修改 appsettings.Development.json（最簡單）
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

#### 方式 B：使用環境變數（推薦，更安全）
```bash
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

#### 方式 C：使用 User Secrets（開發環境推薦）
```bash
cd DmsSystem.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

### 正式環境（SQL Server）

1. **修改 appsettings.Production.json**：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_IP;Database=DMS;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

2. **或使用環境變數**：
```bash
export ConnectionStrings__DefaultConnection="Server=YOUR_SERVER_IP;Database=DMS;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=True"
export ASPNETCORE_ENVIRONMENT=Production
```

## 連接字串格式說明

```
Server=<伺服器位址>,<埠號>;Database=<資料庫名稱>;User Id=<使用者名稱>;Password=<密碼>;TrustServerCertificate=True;MultipleActiveResultSets=True
```

### 參數說明

- **Server**: SQL Server 位址和埠號
  - Mac Docker: `localhost,1433`
  - 正式環境: `<IP或網域名稱>,<埠號>`
- **Database**: 資料庫名稱（預設為 `DMS`）
- **User Id**: SQL Server 登入帳號
- **Password**: SQL Server 登入密碼
- **TrustServerCertificate**: 信任伺服器憑證（開發環境使用）
- **MultipleActiveResultSets**: 允許多個活動結果集

## 環境變數優先順序

ASP.NET Core 會依照以下順序載入設定（後面的會覆蓋前面的）：

1. `appsettings.json`
2. `appsettings.{Environment}.json`（例如 `appsettings.Development.json`）
3. 環境變數
4. User Secrets（僅限開發環境）

## 切換環境

### 設定 ASP.NET Core 環境

#### Mac / Linux
```bash
export ASPNETCORE_ENVIRONMENT=Development  # 或 Production
```

#### Windows PowerShell
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"  # 或 Production
```

#### Windows CMD
```cmd
set ASPNETCORE_ENVIRONMENT=Development  # 或 Production
```

## 常見問題

### Q: 如何確認連接字串是否正確？

A: 執行應用程式時，如果連接字串錯誤，會在啟動時拋出例外。也可以使用 SQL Server Management Studio 或 `sqlcmd` 測試連接。

### Q: Mac 上如何連接遠端 SQL Server？

A: 將連接字串中的 `Server` 參數改為遠端伺服器的 IP 或網域名稱。

### Q: 如何保護生產環境的密碼？

A: 
1. 使用環境變數（推薦）
2. 使用 Azure Key Vault 或其他密碼管理服務
3. 使用 User Secrets（僅限開發環境）

### Q: 連接失敗怎麼辦？

A: 檢查：
1. SQL Server 是否正在運行
2. 防火牆設定是否允許連接
3. 連接字串中的參數是否正確
4. 使用者帳號是否有足夠權限

