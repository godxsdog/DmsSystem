#!/bin/bash
# DMS 系統錯誤報告產生器 (Mac)

echo "=== DMS 系統錯誤報告 ===" > ERROR_REPORT.txt
echo "生成時間: $(date)" >> ERROR_REPORT.txt
echo "" >> ERROR_REPORT.txt

echo "=== 系統資訊 ===" >> ERROR_REPORT.txt
echo "作業系統: $(uname -a)" >> ERROR_REPORT.txt
echo ".NET 版本: $(dotnet --version 2>&1)" >> ERROR_REPORT.txt
echo "Node 版本: $(node --version 2>&1)" >> ERROR_REPORT.txt
echo "Docker 版本: $(docker --version 2>&1)" >> ERROR_REPORT.txt
echo "" >> ERROR_REPORT.txt

echo "=== 資料庫狀態 ===" >> ERROR_REPORT.txt
docker ps | grep sqlserver >> ERROR_REPORT.txt || echo "SQL Server 容器未運行" >> ERROR_REPORT.txt
echo "" >> ERROR_REPORT.txt

echo "=== API 建置狀態 ===" >> ERROR_REPORT.txt
cd DmsSystem.Api && dotnet build 2>&1 | tail -10 >> ../ERROR_REPORT.txt
cd .. >> ERROR_REPORT.txt
echo "" >> ERROR_REPORT.txt

echo "=== 資料庫連接測試 ===" >> ERROR_REPORT.txt
docker exec -it dms-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'DmsSystem@2024' -d DMS -Q "SELECT DB_NAME()" 2>&1 >> ERROR_REPORT.txt || echo "無法連接資料庫" >> ERROR_REPORT.txt
echo "" >> ERROR_REPORT.txt

echo "=== 環境變數 ===" >> ERROR_REPORT.txt
echo "ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-未設定}" >> ERROR_REPORT.txt
echo "ConnectionStrings__DefaultConnection: ${ConnectionStrings__DefaultConnection:-未設定}" >> ERROR_REPORT.txt
echo "" >> ERROR_REPORT.txt

echo "=== Git 狀態 ===" >> ERROR_REPORT.txt
git status --short >> ERROR_REPORT.txt
echo "" >> ERROR_REPORT.txt

echo "=== 分支資訊 ===" >> ERROR_REPORT.txt
git branch --show-current >> ERROR_REPORT.txt
git log -1 --oneline >> ERROR_REPORT.txt

echo "✅ 錯誤報告已產生：ERROR_REPORT.txt"
cat ERROR_REPORT.txt
