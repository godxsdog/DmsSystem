# SQL Server 本地设置指南

本指南将帮助您在本地设置 SQL Server 并连接到 DmsSystem 项目。

## 前置要求

1. **Docker Desktop** - 必须安装并运行
   - macOS: 从 [Docker Desktop for Mac](https://www.docker.com/products/docker-desktop) 下载安装
   - 确保 Docker Desktop 正在运行

2. **.NET 8 SDK** - 项目使用 .NET 8.0

## 快速开始

### 1. 启动 SQL Server

使用提供的脚本启动 SQL Server：

```bash
cd /Users/kaichanghuang/Documents/Phoenix\ Code/DmsSystem
chmod +x scripts/start-sqlserver.sh
./scripts/start-sqlserver.sh
```

或者使用 Docker Compose：

```bash
docker-compose up -d
```

### 2. 验证连接

等待 SQL Server 启动完成后（约 30-60 秒），可以使用以下命令验证：

```bash
docker exec -it dms-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'DmsSystem@2024' -d DMS -Q "SELECT DB_NAME()"
```

### 3. 运行数据库迁移

使用 Entity Framework Core 创建数据库表结构：

```bash
cd DmsSystem.Api
dotnet ef database update
```

**注意**: 如果还没有创建迁移，需要先创建：

```bash
dotnet ef migrations add InitialCreate --project ../DmsSystem.Infrastructure --startup-project .
```

### 4. 运行应用程序

```bash
cd DmsSystem.Api
dotnet run
```

## 连接信息

- **Server**: `localhost,1433`
- **Database**: `DMS`
- **User**: `sa`
- **Password**: `DmsSystem@2024`

## 连接字符串

连接字符串已配置在以下文件中：
- `DmsSystem.Api/appsettings.json`
- `DmsSystem.Api/appsettings.Development.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=DMS;User Id=sa;Password=DmsSystem@2024;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

## 停止 SQL Server

```bash
./scripts/stop-sqlserver.sh
```

或使用 Docker Compose：

```bash
docker-compose down
```

## 查看日志

```bash
docker-compose logs -f sqlserver
```

## 重置数据库

如果需要重置数据库（删除所有数据）：

```bash
docker-compose down -v
docker-compose up -d
```

然后重新运行迁移：

```bash
cd DmsSystem.Api
dotnet ef database update
```

## 使用 SQL Server Management Studio (SSMS) 连接

1. 下载并安装 [SQL Server Management Studio](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
2. 使用以下连接信息：
   - Server name: `localhost,1433`
   - Authentication: SQL Server Authentication
   - Login: `sa`
   - Password: `DmsSystem@2024`

## 使用 Azure Data Studio 连接

1. 下载并安装 [Azure Data Studio](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio)
2. 创建新连接：
   - Server: `localhost`
   - Port: `1433`
   - Authentication type: SQL Login
   - User name: `sa`
   - Password: `DmsSystem@2024`
   - Database: `DMS`

## 故障排除

### Docker 未运行
确保 Docker Desktop 正在运行。在 macOS 上，检查菜单栏中的 Docker 图标。

### 端口 1433 已被占用
如果端口 1433 已被占用，可以修改 `docker-compose.yml` 中的端口映射：

```yaml
ports:
  - "1434:1433"  # 改为使用 1434
```

然后更新连接字符串中的端口号。

### SQL Server 启动失败
查看容器日志：

```bash
docker-compose logs sqlserver
```

### 无法连接到数据库
1. 确保容器正在运行：`docker ps`
2. 检查容器健康状态：`docker ps --format "table {{.Names}}\t{{.Status}}"`
3. 等待足够的时间让 SQL Server 完全启动（可能需要 30-60 秒）

## 数据库 Schema

项目使用两个 Schema：
- **DMS**: 主要业务数据
- **RIS**: 风险管理系统数据

这些 Schema 会在数据库初始化时自动创建。


