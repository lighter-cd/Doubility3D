@echo off
@rem Set local scope for the variables with windows NT shell
if "%OS%"=="Windows_NT" setlocal

if exist %1 goto findTargetPath


:findTargetPath

if not exist %1 (
	echo ERROR: path [%1] not be found.
	goto fail
)
if not exist %1\Assets (
	echo ERROR: path [%1\Assets] not be found.
	goto fail
)
if exist %1\Assets\Doubility3D (
	echo ERROR: path [%1\Assets\Doubility3D] is already exist.
	goto fail
)

set orign_path = %~dp0
@echo on
cd /d %1\Assets
mklink /D Doubility3D %~dp0DoubilityUnity

if not exist ArtWork (
	md ArtWork
)
if not exist StreamingAssets (
	md StreamingAssets
)

cd /d %~dp0
@echo off

:end
if "%ERRORLEVEL%"=="0" goto mainEnd

:fail
exit /b 1

:mainEnd
if "%OS%"=="Windows_NT" endlocal
exit /b 0