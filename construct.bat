@echo off
@rem Set local scope for the variables with windows NT shell
if "%OS%"=="Windows_NT" setlocal

@rem have to provide target path
if "%1" == "" (
	echo USAGE: construct targetPath
	echo you must provide a target path
	goto fail
)

@rem confirm unity is exist in register table.
reg query "HKEY_CURRENT_USER\Software\Unity Technologies\Installer\Unity"
if "%ERRORLEVEL%"=="1" (
	echo ERROR: [HKEY_CURRENT_USER\Software\Unity Technologies\Installer\Unity] not be found.
	goto fail
)

@rem get unity.exe path from register table.
for /f "tokens=3*" %%i in ('reg query "HKEY_CURRENT_USER\Software\Unity Technologies\Installer\Unity"^|findstr /i Location') do set unity_path=%%j

@rem confirm unity.exe is exist in file system.
if not exist %unity_path% (
	echo ERROR: path [%unity_path%] not be found.
	goto fail
)

@rem confirm target is not exist
if exist %1 (
	echo ERROR: path [%1] already exist.
	goto fail
)

@rem call unity.exe to create a new empty project
%unity_path%\Editor\Unity.exe -quit -batchmode -nographics -createProject %1 -logFile construct.txt
if "%ERRORLEVEL%"=="1" (
	echo ERROR: run Unity.exe error.
	goto fail
)

@rem switch to project folder
set orign_path = %~dp0
cd /d %1\Assets

mklink /D Doubility3D %~dp0DoubilityUnity

if not exist ArtWork (
	md ArtWork
)
if not exist StreamingAssets (
	md StreamingAssets
)

@rem switch to work folder
cd /d %~dp0

:end
if "%ERRORLEVEL%"=="0" goto mainEnd

:fail
exit /b 1

:mainEnd
if "%OS%"=="Windows_NT" endlocal
exit /b 0