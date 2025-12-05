# Mac 開發環境使用手冊

## 🖥️ 環境說明

- **作業系統**：macOS
- **開發工具**：Visual Studio Code 或終端機
- **資料庫**：Docker SQL Server 容器
- **用途**：開發、測試、除錯

## 🚀 快速啟動（4 步驟）

### 步驟 1：啟動資料庫

```bash
cd /Users/kaichanghuang/Documents/Phoenix\ Code/DmsSystem
docker-compose up -d
```

**等待 30-60 秒**讓 SQL Server 完全啟動

**驗證：**
```bash
docker ps | grep sqlserver
```

### 步驟 2：載入測試資料

```bash
# 複製測試資料腳本
docker cp scripts/seed-test-data.sql dms-sqlserver:/tmp/seed-test-data.sql

# 執行腳本
docker exec -i dms-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost \
  -U sa \
  -P 'DmsSystem@2024' \
  -d DMS \
  -i /tmp/seed-test-data.sql
```

**驗證：**
```bash
docker exec -it dms-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost \
  -U sa \
  -P 'DmsSystem@2024' \
  -d DMS \
  -Q "SELECT COUNT(*) FROM RIS.SHMT_SOURCE1"
```
應該看到 `3`（3 筆測試資料）

### 步驟 3：啟動後端 API

**開啟新終端視窗：**
```bash
cd /Users/kaichanghuang/Documents/Phoenix\ Code/DmsSystem/DmsSystem.Api
dotnet run
```

**應該看到：**
```
[INF] 應用程式啟動完成
[INF] Now listening on: http://localhost:5137
```

**驗證：** 打開瀏覽器訪問 http://localhost:5137/swagger

### 步驟 4：啟動前端

**開啟新終端視窗：**
```bash
cd /Users/kaichanghuang/Documents/Phoenix\ Code/DmsSystem/react-client
npm install  # 首次執行需要
npm run dev
```

**應該看到：**
```
➜  Local:   http://localhost:5173/
```

**驗證：** 打開瀏覽器訪問 http://localhost:5173

## ⚙️ 環境配置

### 資料庫連接字串

**位置：** `DmsSystem.Api/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

**說明：**
- `Server=localhost,1433`：Docker 容器的 SQL Server
- `Database=DMS`：資料庫名稱
- `User Id=sa`：SQL Server 管理員帳號
- `Password=DmsSystem@2024`：SQL Server 密碼

### 前端 API 設定

**位置：** `react-client/.env`（如果不存在，複製 `.env.example`）

```
VITE_API_BASE_URL=http://localhost:5137
```

## 🔍 錯誤診斷

### 產生錯誤報告

如果遇到錯誤，執行以下指令產生完整的錯誤報告：

```bash
cd /Users/kaichanghuang/Documents/Phoenix\ Code/DmsSystem

# 產生錯誤報告
cat > ERROR_REPORT.txt << 'EOF'
=== DMS 系統錯誤報告 ===
生成時間: $(date)

=== 系統資訊 ===
作業系統: $(uname -a)
.NET 版本: $(dotnet --version)
Node 版本: $(node --version)
Docker 版本: $(docker --version)

=== 資料庫狀態 ===
$(docker ps | grep sqlserver || echo "SQL Server 容器未運行")

=== API 建置狀態 ===
$(cd DmsSystem.Api && dotnet build 2>&1 | tail -10)

=== 資料庫連接測試 ===
$(docker exec -it dms-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'DmsSystem@2024' -d DMS -Q "SELECT DB_NAME()" 2>&1 || echo "無法連接資料庫")

=== 最近 API 日誌 ===
$(tail -20 /tmp/dms-api.log 2>/dev/null || echo "無日誌檔案")

=== 環境變數 ===
ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-未設定}
ConnectionStrings__DefaultConnection: ${ConnectionStrings__DefaultConnection:-未設定}

=== Git 狀態 ===
$(git status --short)

=== 分支資訊 ===
$(git branch --show-current)
$(git log -1 --oneline)
EOF

cat ERROR_REPORT.txt
```

**將 `ERROR_REPORT.txt` 的內容複製，提供給開發人員進行除錯。**

### 常見錯誤與解決

#### 錯誤 1：資料庫連接失敗

**錯誤訊息：**
```
Cannot open database "DMS" requested by the login
```

**解決步驟：**
1. 確認容器運行：`docker ps | grep sqlserver`
2. 等待 SQL Server 啟動：`sleep 60`
3. 測試連接：
```bash
docker exec -it dms-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost -U sa -P 'DmsSystem@2024' -Q "SELECT 1"
```

#### 錯誤 2：API 啟動失敗

**錯誤訊息：**
```
資料庫連接字串未設定
```

**解決步驟：**
1. 檢查 `appsettings.Development.json` 是否存在
2. 確認連接字串格式正確
3. 或使用環境變數：
```bash
export ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

#### 錯誤 3：前端無法連接 API

**錯誤訊息：**
```
Failed to fetch
```

**解決步驟：**
1. 確認 API 正在運行（檢查終端視窗）
2. 測試 API：`curl http://localhost:5137/swagger`
3. 檢查 CORS 設定（`Program.cs`）

## 🛠️ 開發工具

### Visual Studio Code

**推薦擴充功能：**
- C# Dev Kit
- C# Extensions
- .NET Extension Pack

### 終端機工具

**常用指令：**
```bash
# 查看容器日誌
docker logs dms-sqlserver

# 進入容器
docker exec -it dms-sqlserver bash

# 查看 API 日誌
tail -f /tmp/dms-api.log

# 重新建置專案
dotnet clean && dotnet build
```

## 📝 切換到正式環境

當需要切換到 Windows 正式環境時：

1. **提交所有變更：**
```bash
git add -A
git commit -m "feat: 描述變更內容"
git push
```

2. **在 Windows 電腦上：**
```bash
git pull
```

3. **參考：** [Windows 正式環境手冊](./08-2-Windows正式環境手冊.md)

## 🔄 日常開發流程

1. **啟動資料庫**（如果未運行）
2. **啟動 API**（開發時保持運行）
3. **啟動前端**（開發時保持運行）
4. **修改程式碼**
5. **測試功能**
6. **提交變更**

## 📚 相關文件

- [架構指南](./01-架構指南.md)
- [資料庫配置](./02-資料庫配置.md)
- [Windows 正式環境手冊](./08-2-Windows正式環境手冊.md)

