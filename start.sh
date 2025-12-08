#!/bin/bash
# DMS 系統啟動腳本

echo "=== DMS 系統啟動 ==="

# 檢查 SQL Server 是否運行
if ! docker ps | grep -q dms-sqlserver; then
    echo "📦 啟動 SQL Server 容器..."
    docker-compose up -d
    
    echo "⏳ 等待 SQL Server 啟動（30秒）..."
    sleep 30
fi

# 載入配息測試資料（如果尚未載入）
echo "📊 檢查並載入配息測試資料..."
./scripts/load-dividend-test-data.sh 2>&1 | tail -5

echo ""
echo "🚀 啟動 API 伺服器..."
echo "API 將在 http://localhost:5137 啟動"
echo ""

cd DmsSystem.Api
dotnet run
