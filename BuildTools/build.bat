@echo off
setlocal enabledelayedexpansion

echo **************************************
echo *	��ȡ�����ļ�
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
echo *	��� Unity.EXE �Ƿ��Ѿ�������
echo **************************************

tasklist|find "Unity.exe" > NUL
if %ERRORLEVEL% EQU 0 (
color 04
ECHO Unity.EXE �������У����ȹر�Unity�༭�� �� ��Unity�༭��������vs2013
GOTO :end
)else ECHO ok


echo **************************************
echo *	ִ�� unity ����
echo **************************************

for /f "tokens=1* delims=," %%i in (command.txt) do (
@echo on
%UNITY_CMD% -quit -batchmode -buildTarget %BUILD_TARGET% -executeMethod %%i
@echo off
echo %%i ִ�����
call :Waiting
��

pause
:end
goto :eof

:Waiting
echo wscript.sleep 1000 > delay.vbs
cscript //nologo delay.vbs & del delay.vbs
echo �ȴ�ʱ�����

:startWaitingProcess
tasklist|find "Unity.exe" > NUL
if %ERRORLEVEL% EQU 0 GOTO :startWaitingProcess
:endWaitingProcess
echo �ȴ��������
goto :eof

