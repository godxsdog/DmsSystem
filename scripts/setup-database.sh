#!/bin/bash
# 資料庫設定腳本 - 建立資料表和載入測試資料

echo "=== DMS 資料庫設定 ==="

# 檢查容器是否運行
if ! docker ps | grep -q dms-sqlserver; then
    echo "❌ SQL Server 容器未運行，請先執行: docker-compose up -d"
    exit 1
fi

# 尋找 sqlcmd 路徑
SQLCMD_PATH=$(docker exec dms-sqlserver bash -c "find /opt -name sqlcmd 2>/dev/null | head -1" | tr -d '\r')

if [ -z "$SQLCMD_PATH" ]; then
    echo "❌ 找不到 sqlcmd，嘗試使用預設路徑"
    SQLCMD_PATH="/opt/mssql-tools18/bin/sqlcmd"
fi

echo "使用 sqlcmd: $SQLCMD_PATH"

# 複製腳本到容器
echo "📋 複製腳本到容器..."
docker cp scripts/create-tables.sql dms-sqlserver:/tmp/create-tables.sql
docker cp scripts/seed-test-data.sql dms-sqlserver:/tmp/seed-test-data.sql

# 建立資料表
echo "🏗️  建立資料表..."
docker exec dms-sqlserver bash -c "$SQLCMD_PATH -S localhost -U sa -P 'DmsSystem@2024' -d DMS -C -i /tmp/create-tables.sql" 2>&1

if [ $? -eq 0 ]; then
    echo "✅ 資料表建立成功"
else
    echo "⚠️  資料表建立可能失敗，請檢查錯誤訊息"
fi

# 載入測試資料
echo "📊 載入測試資料..."
docker exec dms-sqlserver bash -c "$SQLCMD_PATH -S localhost -U sa -P 'DmsSystem@2024' -d DMS -C -i /tmp/seed-test-data.sql" 2>&1

if [ $? -eq 0 ]; then
    echo "✅ 測試資料載入成功"
else
    echo "⚠️  測試資料載入可能失敗，請檢查錯誤訊息"
fi

# 驗證資料
echo "🔍 驗證資料..."
docker exec dms-sqlserver bash -c "$SQLCMD_PATH -S localhost -U sa -P 'DmsSystem@2024' -d DMS -C -Q \"SELECT COUNT(*) as Count FROM RIS.SHMT_SOURCE1\"" 2>&1 | grep -E "Count|^[0-9]"

echo ""
echo "=== 設定完成 ==="
echo "現在可以啟動 API 進行測試"

