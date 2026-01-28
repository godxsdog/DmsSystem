@echo off
:: 1. Change Directory
cd /d C:\Users\kentkhuang\source\repos\godxsdog\DmsSystem

:: 2. Format Date and Time (YYYYMMDD_HHMMSS)
set "datestr=%date:~0,4%%date:~5,2%%date:~8,2%"
set "t=%time: =0%"
set "timestr=%t:~0,2%%t:~3,2%%t:~6,2%"
set "full_datetime=%datestr%_%timestr%"

:: 3. Git Commands
git add .
git commit -m "%full_datetime%"
git push -u origin

echo.
echo ================================
echo    Upload Complete: %full_datetime%
echo ================================
pause