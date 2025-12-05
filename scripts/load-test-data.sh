#!/bin/bash

# 載入測試資料腳本

echo "正在載入測試資料到資料庫..."

docker exec -i dms-sqlserver /opt/mssql-tools/bin/sqlcmd \
  -S localhost \
  -U sa \
  -P 'DmsSystem@2024' \
  -d DMS \
  -i /var/opt/mssql/backup/seed-test-data.sql

if [ $? -eq 0 ]; then
    echo "✅ 測試資料載入成功！"
else
    echo "❌ 測試資料載入失敗"
    exit 1
fi

