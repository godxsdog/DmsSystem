@echo off
:: 1. 切換目錄
cd /d C:\Users\kentkhuang\source\repos\godxsdog\DmsSystem

:: 2. 加入所有變更
git add .

:: 3. 取得今天日期並 Commit
:: 這會抓取系統日期，格式通常為 YYYY-MM-DD
set "datestr=%date:~0,4%%date:~5,2%%date:~8,2%"
git commit -m "Auto Upload: %datestr%"

:: 4. 推送到遠端
git push -u origin

echo.
echo ================================
echo    上傳完成！請按任意鍵結束。
echo ================================
pause