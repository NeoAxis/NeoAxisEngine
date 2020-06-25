

if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:core\%~nx0] initializing...

set __VSCMD_msbuild_failcount=0
if "%VSCMD_TEST%" NEQ "" goto :test
if "%VSCMD_ARG_CLEAN_ENV%" NEQ "" goto :clean_env

@REM Add path to MSBuild Binaries
if exist "%VSINSTALLDIR%\MSBuild\15.0\bin" (
    set "PATH=%VSINSTALLDIR%\MSBuild\15.0\bin;%PATH%"
) else (
    set /A __VSCMD_msbuild_failcount=__VSCMD_msbuild_failcount+1
)

goto :end

:clean_env

@REM This script depends on vsdevcmd.bat to restore PATH.
goto :end

:test

setlocal
@echo [TEST:%~nx0] checking for msbuild.exe...
where msbuild.exe 2>&1 >  NUL
if "%ERRORLEVEL%" NEQ "0" (
    @echo [ERROR:%~nx0] Test 'where msbuild.exe' failed.
    set /A __VSCMD_msbuild_failcount=__VSCMD_msbuild_failcount+1
) else (
    if "%VSCMD_DEBUG%" GEQ "1" echo [DEBUG:%~nx0] msbuild.exe found successfully
)

endlocal & set __VSCMD_msbuild_failcount=%__VSCMD_msbuild_failcount%
goto :end

:end

if "%__VSCMD_msbuild_failcount%" NEQ "0" (
    set __VSCMD_msbuild_failcount=
    exit /B 1
)
set __VSCMD_msbuild_failcount=
exit /B 0
