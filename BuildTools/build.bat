@echo off
setlocal enabledelayedexpansion

echo **************************************
echo *	获取配置文件
echo **************************************
set a=0
for /f "tokens=1-3 delims=," %%i in (config.txt) do (
set /a n+=1
set var_!n!=%%i)

set UNITY_PATH=!var_1!
set PROJECT_PATH=!var_2!
set BUILD_TARGET=!var_3!
set UNITY_CMD=%UNITY_PATH%Unity.exe
set RUN_PATH=%~dp0

echo UNITY_PATH=!UNITY_PATH!
echo PROJECT_PATH=!PROJECT_PATH!
echo BUILD_TARGET=!BUILD_TARGET!
echo UNITY_CMD=!UNITY_CMD!
echo RUN_PATH=!RUN_PATH!

echo **************************************
echo *	检查 Unity.EXE 是否已经在运行
echo **************************************

tasklist|find "Unity.exe" > NUL
if %ERRORLEVEL% EQU 0 (
color 04
ECHO Unity.EXE 正在运行，请先关闭Unity编辑器 和 由Unity编辑器启动的vs2013
GOTO :end
)else ECHO ok


echo **************************************
echo *	执行 unity 命令
echo **************************************

for /f "tokens=1* delims=," %%i in (command.txt) do (
@echo on
%UNITY_CMD% -quit -batchmode -buildTarget %BUILD_TARGET% -executeMethod %%i
@echo off
echo %%i 执行完毕
call :Waiting
）

pause
:end
goto :eof

:Waiting
echo wscript.sleep 1000 > delay.vbs
cscript //nologo delay.vbs & del delay.vbs
echo 等待时间完毕

:startWaitingProcess
tasklist|find "Unity.exe" > NUL
if %ERRORLEVEL% EQU 0 GOTO :startWaitingProcess
:endWaitingProcess
echo 等待进程完毕
goto :eof

