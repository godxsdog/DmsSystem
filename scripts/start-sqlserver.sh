#!/bin/bash

# SQL Server Docker 启动脚本

echo "正在启动 SQL Server Docker 容器..."

# 检查 Docker 是否运行
if ! docker info > /dev/null 2>&1; then
    echo "错误: Docker 未运行。请先启动 Docker Desktop。"
    exit 1
fi

# 启动容器
docker-compose up -d

# 等待 SQL Server 就绪
echo "等待 SQL Server 启动..."
sleep 10

# 检查容器状态
if docker ps | grep -q dms-sqlserver; then
    echo "✓ SQL Server 容器已启动"
    
    # 等待健康检查
    echo "等待 SQL Server 就绪（这可能需要 30-60 秒）..."
    max_attempts=30
    attempt=0
    
    while [ $attempt -lt $max_attempts ]; do
        if docker exec dms-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "DmsSystem@2024" -Q "SELECT 1" > /dev/null 2>&1; then
            echo "✓ SQL Server 已就绪！"
            echo ""
            echo "连接信息："
            echo "  Server: localhost,1433"
            echo "  Database: DMS"
            echo "  User: sa"
            echo "  Password: DmsSystem@2024"
            echo ""
            echo "可以使用以下命令连接到数据库："
            echo "  docker exec -it dms-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'DmsSystem@2024' -d DMS"
            exit 0
        fi
        attempt=$((attempt + 1))
        echo "  尝试 $attempt/$max_attempts..."
        sleep 2
    done
    
    echo "警告: SQL Server 可能仍在启动中，请稍后检查。"
else
    echo "错误: SQL Server 容器启动失败"
    docker-compose logs
    exit 1
fi


