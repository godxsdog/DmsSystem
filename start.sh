#!/bin/bash
# DMS 系統啟動腳本（使用雲端 SQL，無需 Docker）

echo "=== DMS 系統啟動（雲端 SQL） ==="
echo "使用連線：vtwesiwudb22 / DMS（appsettings 已設定）"
echo ""
echo "🚀 啟動 API 伺服器..."
echo "API 將在 http://localhost:5137 啟動（Swagger 已停用）"
echo ""

cd DmsSystem.Api
dotnet run
