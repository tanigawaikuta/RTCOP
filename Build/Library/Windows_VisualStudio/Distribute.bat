@echo off
chcp 65001

cd /d %~dp0

set ROOT_DIR=..\..\..
set LIB_DIR=%ROOT_DIR%\Library\Windows
set TEST_DIR=%ROOT_DIR%\Test\TestSpace\Windows\Library

echo x86のファイルコピー:
xcopy /I /Y "%TEST_DIR%\x86\Release\*.lib" "%LIB_DIR%\x86\"
echo;

echo x64のファイルコピー:
xcopy /I /Y "%TEST_DIR%\x64\Release\*.lib" "%LIB_DIR%\x64\"
echo;

pause
