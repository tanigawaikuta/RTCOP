@echo off
chcp 65001

cd /d %~dp0

set ROOT_DIR=..\..\..
set TOOL_DIR=%ROOT_DIR%\Tool\Windows
set TEST_DIR=%ROOT_DIR%\Test\TestSpace\Windows\Tool

echo ファイルコピー:
xcopy /I /Y "%TEST_DIR%\Release\*.exe" "%TOOL_DIR%\"
xcopy /I /Y "%TEST_DIR%\Release\*.dll" "%TOOL_DIR%\"
xcopy /I /Y "%TEST_DIR%\Release\*.config" "%TOOL_DIR%\"
xcopy /I /Y "%TEST_DIR%\Release\*.xml" "%TOOL_DIR%\"
echo;

pause
