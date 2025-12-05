#!/bin/bash

# DmsSystem 測試執行腳本
# 此腳本會執行所有測試並產生測試報告

echo "=========================================="
echo "DmsSystem 測試執行腳本"
echo "=========================================="
echo ""

# 檢查是否在正確的目錄
if [ ! -f "DmsSystem.Tests/DmsSystem.Tests.csproj" ]; then
    echo "錯誤: 請在 DmsSystem 根目錄執行此腳本"
    exit 1
fi

# 還原套件
echo "1. 還原 NuGet 套件..."
dotnet restore DmsSystem.Tests/DmsSystem.Tests.csproj

if [ $? -ne 0 ]; then
    echo "錯誤: 套件還原失敗"
    exit 1
fi

echo "✓ 套件還原完成"
echo ""

# 建置測試專案
echo "2. 建置測試專案..."
dotnet build DmsSystem.Tests/DmsSystem.Tests.csproj --no-restore

if [ $? -ne 0 ]; then
    echo "錯誤: 建置失敗"
    exit 1
fi

echo "✓ 建置完成"
echo ""

# 執行測試
echo "3. 執行測試..."
echo ""

# 執行所有測試並產生詳細報告
dotnet test DmsSystem.Tests/DmsSystem.Tests.csproj \
    --no-build \
    --verbosity normal \
    --logger "console;verbosity=detailed" \
    --collect:"XPlat Code Coverage"

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "✓ 所有測試通過！"
    echo "=========================================="
else
    echo ""
    echo "=========================================="
    echo "✗ 部分測試失敗"
    echo "=========================================="
    exit 1
fi

