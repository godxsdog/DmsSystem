#!/bin/bash
# 載入配息測試資料腳本

echo "正在載入配息測試資料到資料庫..."

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
docker cp scripts/seed-dividend-test-data.sql dms-sqlserver:/tmp/seed-dividend-test-data.sql

# 載入測試資料
echo "📊 載入配息測試資料..."
docker exec dms-sqlserver bash -c "$SQLCMD_PATH -S localhost -U sa -P 'DmsSystem@2024' -d DMS -C -i /tmp/seed-dividend-test-data.sql" 2>&1

if [ $? -eq 0 ]; then
    echo "✅ 配息測試資料載入成功！"
    echo ""
    echo "測試資料說明："
    echo "- 基金代號: A001"
    echo "- 配息頻率: M (月配)"
    echo "- 配息基準日: 2024-12-01"
    echo "- NAV: 10.5"
    echo "- 單位數: 1,000,000"
    echo "- 可分配收益: 60,000 (扣除費用後)"
else
    echo "❌ 配息測試資料載入失敗"
    exit 1
fi
