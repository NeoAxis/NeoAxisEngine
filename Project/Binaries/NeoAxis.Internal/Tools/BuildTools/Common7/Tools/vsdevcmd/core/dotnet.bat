
if "%VSCMD_DEBUG%" GEQ "2" @echo [vsdevcmd\core\%~nx0] initializing...

set __dotnet_error_count=0
if "%VSCMD_TEST%" NEQ "" goto :test
if "%VSCMD_ARG_CLEAN_ENV%" NEQ "" goto :clean_env

:start

if "%VSCMD_ARG_TGT_ARCH%"=="x86" (
    set __DOTNET_ADD_32BIT=1
)

if "%VSCMD_ARG_TGT_ARCH%"=="x64" (
    set __DOTNET_ADD_64BIT=1
)

@REM Don't add any extra Framework dirs for ARM.

if "%VSCMD_ARG_HOST_ARCH%"=="x86" (
    set __DOTNET_ADD_32BIT=1
    set __DOTNET_PREFERRED_BITNESS=32
) else if "%VSCMD_ARG_HOST_ARCH%"=="x64" (
    set __DOTNET_ADD_64BIT=1
    set __DOTNET_PREFERRED_BITNESS=64
) else (
    @echo ERROR: Invalid host architecture '%VSCMD_ARG_HOST_ARCH%'.
    set /A __dotnet_error_count=__dotnet_error_count+1
)

if "%VSCMD_DEBUG%" GEQ "2" (
if "%__DOTNET_ADD_32BIT%"=="1" @echo [DEBUG:core\%~nx0] Adding 32-bit .NET Framework Path
if "%__DOTNET_ADD_64BIT%"=="1" @echo [DEBUG:core\%~nx0] Adding 64-bit .NET Framework Path
@echo [DEBUG:core\%~nx0] Framework Preference: %__DOTNET_PREFERRED_BITNESS%bit
)

if "%__DOTNET_ADD_32BIT%"=="1" (
	@call :GetFrameworkDir32
	@call :GetFrameworkVer32
	@call :CheckFramework32
)
if "%__DOTNET_ADD_64BIT%"=="1" (
	@call :GetFrameworkDir64
	@call :GetFrameworkVer64
	@call :CheckFramework64
)

set Framework40Version=v4.0
call :CheckFramework40Version

goto :export_env

@REM -----------------------------------------------------------------------
:GetFrameworkDir32
set FrameworkDir32=
call :GetFrameworkDir32Helper32 HKLM > nul 2>&1
if errorlevel 1 call :GetFrameworkDir32Helper32 HKCU > nul 2>&1
if errorlevel 1 call :GetFrameworkDir32Helper64  HKLM > nul 2>&1
if errorlevel 1 call :GetFrameworkDir32Helper64  HKCU > nul 2>&1
if errorlevel 1 call :GetFrameworkDir32Default
exit /B 0

:GetFrameworkDir32Helper32
for /F "tokens=1,2*" %%i in ('reg query "%1\SOFTWARE\Microsoft\VisualStudio\SxS\VC7" /v "FrameworkDir32"') DO (
	if "%%i"=="FrameworkDir32" (
		SET FrameworkDIR32=%%k
	)
)
if "%FrameworkDir32%"=="" exit /B 1
exit /B 0

:GetFrameworkDir32Helper64
for /F "tokens=1,2*" %%i in ('reg query "%1\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\SxS\VC7" /v "FrameworkDir32"') DO (
	if "%%i"=="FrameworkDir32" (
		SET FrameworkDIR32=%%k
	)
)
if "%FrameworkDIR32%"=="" exit /B 1
exit /B 0

@REM TODO: Remove default setting. dotnet.bat should fail if we cannot find the correct FrameworkDir32
:GetFrameworkDir32Default
SET FrameworkDir32=C:\Windows\Microsoft.NET\Framework\
if "%VSCMD_DEBUG%" GEQ "2" @echo [DEBUG:core\%~nx0] Cannot find registry key for FrameworkDir32 - using default: %FrameworkDir32%
exit /B 0

@REM -----------------------------------------------------------------------
:GetFrameworkDir64
set FrameworkDir64=
call :GetFrameworkDir64Helper32 HKLM > nul 2>&1
if errorlevel 1 call :GetFrameworkDir64Helper32 HKCU > nul 2>&1
if errorlevel 1 call :GetFrameworkDir64Helper64  HKLM > nul 2>&1
if errorlevel 1 call :GetFrameworkDir64Helper64  HKCU > nul 2>&1
if errorlevel 1 call :GetFrameworkDir64Default
exit /B 0

:GetFrameworkDir64Helper32
for /F "tokens=1,2*" %%i in ('reg query "%1\SOFTWARE\Microsoft\VisualStudio\SxS\VC7" /v "FrameworkDir64"') DO (
	if "%%i"=="FrameworkDir64" (
		SET "FrameworkDIR64=%%k"
	)
)
if "%FrameworkDIR64%"=="" exit /B 1
exit /B 0

:GetFrameworkDir64Helper64
for /F "tokens=1,2*" %%i in ('reg query "%1\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\SxS\VC7" /v "FrameworkDir64"') DO (
	if "%%i"=="FrameworkDir64" (
		SET "FrameworkDIR64=%%k"
	)
)
if "%FrameworkDIR64%"=="" exit /B 1
exit /B 0

@REM TODO: Remove default setting. dotnet.bat should fail if we cannot find the correct FrameworkDir64
:GetFrameworkDir64Default
SET FrameworkDir64=C:\Windows\Microsoft.NET\Framework64\
if "%VSCMD_DEBUG%" GEQ "2" @echo [DEBUG:core\%~nx0] Cannot find registry key for FrameworkDir64 - using default: %FrameworkDir64%
exit /B 0

@REM -----------------------------------------------------------------------
:GetFrameworkVer32
set FrameworkVer32=
call :GetFrameworkVer32Helper32 HKLM > nul 2>&1
if errorlevel 1 call :GetFrameworkVer32Helper32 HKCU > nul 2>&1
if errorlevel 1 call :GetFrameworkVer32Helper64  HKLM > nul 2>&1
if errorlevel 1 call :GetFrameworkVer32Helper64  HKCU > nul 2>&1
if errorlevel 1 call :GetFrameworkVer32Default
exit /B 0

:GetFrameworkVer32Helper32
for /F "tokens=1,2*" %%i in ('reg query "%1\SOFTWARE\Microsoft\VisualStudio\SxS\VC7" /v "FrameworkVer32"') DO (
	if "%%i"=="FrameworkVer32" (
		SET FrameworkVersion32=%%k
	)
)
if "%FrameworkVersion32%"=="" exit /B 1
exit /B 0

:GetFrameworkVer32Helper64
for /F "tokens=1,2*" %%i in ('reg query "%1\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\SxS\VC7" /v "FrameworkVer32"') DO (
	if "%%i"=="FrameworkVer32" (
		SET FrameworkVersion32=%%k
	)
)
if "%FrameworkVersion32%"=="" exit /B 1
exit /B 0

@REM TODO: Remove default setting. dotnet.bat should fail if we cannot find the correct FrameworkVersion32
:GetFrameworkVer32Default
SET FrameworkVersion32=v4.0.30319
if "%VSCMD_DEBUG%" GEQ "2" @echo [DEBUG:core\%~nx0] Cannot find registry key for FrameworkVersion32 - using default: %FrameworkVersion32%
exit /B 0

@REM -----------------------------------------------------------------------
:GetFrameworkVer64
set FrameworkVer64=
call :GetFrameworkVer64Helper32 HKLM > nul 2>&1
if errorlevel 1 call :GetFrameworkVer64Helper32 HKCU > nul 2>&1
if errorlevel 1 call :GetFrameworkVer64Helper64  HKLM > nul 2>&1
if errorlevel 1 call :GetFrameworkVer64Helper64  HKCU > nul 2>&1
if errorlevel 1 call :GetFrameworkVer64Default
exit /B 0

:GetFrameworkVer64Helper32
for /F "tokens=1,2*" %%i in ('reg query "%1\SOFTWARE\Microsoft\VisualStudio\SxS\VC7" /v "FrameworkVer64"') DO (
	if "%%i"=="FrameworkVer64" (
		SET FrameworkVersion64=%%k
	)
)
@if "%FrameworkVersion64%"=="" exit /B 1
@exit /B 0

:GetFrameworkVer64Helper64
for /F "tokens=1,2*" %%i in ('reg query "%1\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\SxS\VC7" /v "FrameworkVer64"') DO (
	if "%%i"=="FrameworkVer64" (
		SET FrameworkVersion64=%%k
	)
)
if "%FrameworkVersion64%"=="" exit /B 1
exit /B 0

@REM TODO: Remove default setting. dotnet.bat should fail if we cannot find the correct FrameworkVersion64
:GetFrameworkVer64Default
SET FrameworkVersion64=v4.0.30319
if "%VSCMD_DEBUG%" GEQ "2" @echo [DEBUG:core\%~nx0] Cannot find registry key for FrameworkVersion64 - using default: %FrameworkVersion64%
exit /B 0

:CheckFramework32

set __check_result=
if "%FrameworkDir32%"=="" (
    @echo ERROR: Cannot determine the location of the .NET Framework 32bit installation.
    set __check_result=1
)
if "%FrameworkVersion32%"=="" (
    @echo ERROR: Cannot determine the version of the .NET Framework 32bit installation.
    set __check_result=1
)
if "%__check_result%"=="1" (
    set __check_result=
    exit /B 1
)

exit /B 0

:CheckFramework64

set __check_result=
if "%FrameworkDir64%"=="" (
    @echo ERROR: Cannot determine the location of the .NET Framework 64bit installation.
    set __check_result=1
)
if "%FrameworkVersion64%"=="" (
    @echo ERROR: Cannot determine the version of the .NET Framework 64bit installation.
    set __check_result=1
)
if "%__check_result%"=="1" (
    set __check_result=
    exit /B 1
)
exit /B 0

:CheckFramework40Version
if "%Framework40Version%"=="" (
    @echo ERROR: Cannot determine the .NET Framework 4.0 version.
    exit /B 1
)

exit /B 0

:test
set __dotnet_error_count=0

setlocal
echo [TEST:%~nx0] Checking for ilasm.exe...
where ilasm.exe > NUL 2>&1
if "%ERRORLEVEL%" NEQ "0" (
   echo [ERROR:%~nx0] Test 'where ilasm.exe' failed.
   set /A __dotnet_error_count=__dotnet_error_count+1
)
endlocal & set __dotnet_error_count=%__dotnet_error_count%

goto :end

:clean_env

    set __DOTNET_ADD_32BIT=
    set __DOTNET_ADD_64BIT=
    set __DOTNET_PREFERRED_BITNESS=
    set FrameworkDir=
    set FrameworkVersion=
    set FrameworkDir32=
    set FrameworkVersion32=
    set FrameworkDir64=
    set FrameworkVersion64=
    set Framework40Version=

    REM;; PATH and LIBPATH are cleaned-up by vsdevcmd.bat, so clean_up is not
    REM;; needed here.

    goto :end

:export_env

@REM Choose FrameworkDir32 or FrameworkDir64 depending on the preferred bitness.
setlocal EnableDelayedExpansion
set __DOTNET_FrameworkDir=!FrameworkDir%__DOTNET_PREFERRED_BITNESS%!
set __DOTNET_FrameworkVersion=!FrameworkVersion%__DOTNET_PREFERRED_BITNESS%!

if "__DOTNET_FrameworkDir"=="" (
    @echo ERROR: FrameworkDir%__DOTNET_PREFERRED_BITNESS% cannot be found.
    set /A __dotnet_error_count=__dotnet_error_count+1
)

if "__DOTNET_FrameworkVersion"=="" (
    @echo ERROR: FrameworkVersion%__DOTNET_PREFERRED_BITNESS% cannot be found.
    set /A __dotnet_error_count=__dotnet_error_count+1
)

endlocal & set "FrameworkDir=%__DOTNET_FrameworkDir%" & set "FrameworkVersion=%__DOTNET_FrameworkVersion%"

@REM Normalize FrameworkDir to always contain a trailing backslash. When found in the registry,
@REM FrameworkDir64 does not have a trailing '\', while the 32-bit registry value does.
if NOT "%FrameworkDir:~-1%"=="\" (
    if "%VSCMD_DEBUG%" GEQ "2" @echo [DEBUG:core\%~nx0] appending '\' to FrameworkDir.
    set "FrameworkDir=%FrameworkDir%\"
)

if exist "%FrameworkDir%%Framework40Version%" set "PATH=%FrameworkDir%%Framework40Version%;%PATH%"
if exist "%FrameworkDir%%FrameworkVersion%" set "PATH=%FrameworkDir%%FrameworkVersion%;%PATH%"
if exist "%FrameworkDir%%Framework40Version%" set "LIBPATH=%FrameworkDir%%Framework40Version%;%LIBPATH%"
if exist "%FrameworkDir%%FrameworkVersion%" set "LIBPATH=%FrameworkDir%%FrameworkVersion%;%LIBPATH%"

:end

if "%__dotnet_error_count%" NEQ "0" (
    set __dotnet_error_count=
    exit /B 1
)

set __dotnet_error_count=
exit /B 0
