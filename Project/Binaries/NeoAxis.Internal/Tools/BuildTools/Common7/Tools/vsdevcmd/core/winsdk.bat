
if "%VSCMD_DEBUG%" GEQ "1" echo [DEBUG:%~nx0] initializing...

set __winsdk_script_err_count=0
if "%VSCMD_TEST%" NEQ "" goto :test
if "%VSCMD_ARG_CLEAN_ENV%" NEQ "" goto :clean_env

:start

@REM if the user specified -winsdk=none, then skip to the end
if /I "%VSCMD_ARG_WINSDK%"=="none" (
    if "%VSCMD_DEBUG%" GEQ "1" echo [DEBUG:%~nx0] /winsdk=none specified, skipping init.
) else (
    call :GetWindowsSdkDir
    call :GetUniversalCRTSdkDir
)

goto :export_env

@REM -----------------------------------------------------------------------
:GetWindowsSdkDir
set WindowsSdkDir=
set WindowsLibPath=
set WindowsSDKVersion=
set WindowsSDKLibVersion=winv6.3\
set WindowsSdkBinPath=


@REM If the user specifically requested a Windows SDK Version, then attempt to use it.
if "%VSCMD_ARG_WINSDK%"=="8.1" (
  call :GetWin81SdkDir
  if errorlevel 1 call :GetWin81SdkDirError
  if errorlevel 1 exit /B 1
  exit /B 0
)
if NOT "%VSCMD_ARG_WINSDK%"=="" (
  call :GetWin10SdkDir
  if errorlevel 1 call :GetWin10SdkDirError
  if errorlevel 1 call exit /B 1
  exit /B 0
)

@REM If a specific SDK was not requested, first check for the latest Windows 10 SDK
@REM and if not found, fall back to the 8.1 SDK.
if "%WindowsSdkDir%"=="" call :GetWin10SdkDir
if "%WindowsSdkDir%"=="" call :GetWin81SdkDir

@REM If a Windows SDK is still not found, then we record an error.
@REM There are valid cases where no Windows SDK will be installed on the system.
@REM Once this script moves into a component-specific selection and the existence of a valid
@REM Windows SDK can be assumed, uncomment the following.
@REM if "%WindowsSdkDir%"=="" set /A __winsdk_script_err_count=__winsdk_script_err_count+1

if "%__winsdk_script_err_count%" NEQ "0" exit /B 1
exit /B 0

@REM ---------------------------------------------------------------------------
:GetWin10SdkDir

if "%VSCMD_DEBUG%" GEQ "3" goto :GetWin10SdkDirVerbose

call :GetWin10SdkDirHelper HKLM\SOFTWARE\Wow6432Node > nul 2>&1
if errorlevel 1 call :GetWin10SdkDirHelper HKCU\SOFTWARE\Wow6432Node > nul 2>&1
if errorlevel 1 call :GetWin10SdkDirHelper HKLM\SOFTWARE > nul 2>&1
if errorlevel 1 call :GetWin10SdkDirHelper HKCU\SOFTWARE > nul 2>&1
if errorlevel 1 exit /B 1
exit /B 0

:GetWin10SdkDirVerbose

call :GetWin10SdkDirHelper HKLM\SOFTWARE\Wow6432Node
if errorlevel 1 call :GetWin10SdkDirHelper HKCU\SOFTWARE\Wow6432Node
if errorlevel 1 call :GetWin10SdkDirHelper HKLM\SOFTWARE
if errorlevel 1 call :GetWin10SdkDirHelper HKCU\SOFTWARE
if errorlevel 1 exit /B 1

exit /B 0

:GetWin10SdkDirHelper

@REM Get Windows 10 SDK installed folder
for /F "tokens=1,2*" %%i in ('reg query "%1\Microsoft\Microsoft SDKs\Windows\v10.0" /v "InstallationFolder"') DO (
    if "%%i"=="InstallationFolder" (
        SET WindowsSdkDir=%%~k
    )
)

@REM get windows 10 sdk version number
setlocal enableDelayedExpansion

@REM Due to the SDK installer changes beginning with the 10.0.15063.0 (RS2 SDK), there is a chance that the
@REM Windows SDK installed may not have the full set of bits required for all application scenarios.
@REM We check for the existence of a file we know to be included in the "App" and "Desktop" portions
@REM of the Windows SDK, depending on the Developer Command Prompt's -app_platform configuration.
@REM If "windows.h" (UWP) or "winsdkver.h" (Desktop) are not found, the directory will be skipped as
@REM a candidate default value for [WindowsSdkDir].
set __check_file=winsdkver.h
if /I "%VSCMD_ARG_APP_PLAT%"=="UWP" set __check_file=Windows.h

if not "%WindowsSdkDir%"=="" for /f %%i IN ('dir "%WindowsSdkDir%include\" /b /ad-h /on') DO (
    @REM Skip if Windows.h|winsdkver (based upon -app_platform configuration) is not found in %%i\um.  
    if EXIST "%WindowsSdkDir%include\%%i\um\%__check_file%" (
        set result=%%i
        if "!result:~0,3!"=="10." (
            set SDK=!result!
            if "!result!"=="%VSCMD_ARG_WINSDK%" set findSDK=1
        )
    )
)

if "%findSDK%"=="1" set SDK=%VSCMD_ARG_WINSDK%
endlocal & set WindowsSDKVersion=%SDK%\

if not "%VSCMD_ARG_WINSDK%"=="" (
  @REM if the user specified a version of the SDK and it wasn't found, then use the
  @REM user-specified version to set environment variables.

  if not "%VSCMD_ARG_WINSDK%\"=="%WindowsSDKVersion%" (
    if "%VSCMD_DEBUG%" GEQ "1" echo [DEBUG:%~nx0] specified /winsdk=%VSCMD_ARG_WINSDK% was not found or was incomplete
    set WindowsSDKVersion=%VSCMD_ARG_WINSDK%\
    set WindowsSDKNotFound=1
  )
) else (
  @REM if no full Windows 10 SDKs were found, unset WindowsSDKDir and exit with error.

  if "%WindowsSDKVersion%"=="\" (
    set WindowsSDKNotFound=1
    set WindowsSDKDir=
    set WindowsSDKBinPath=
    set WindowsSDKVerBinPath=
    goto :GetWin10SdkDirExit
  )
)

if not "%WindowsSDKVersion%"=="\" set WindowsSDKLibVersion=%WindowsSDKVersion%

@REM To support Win10 SDK versioned bin directory changes, the command prompts will first check for a
@REM versioned binary path
set "WindowsSdkBinPath=%WindowsSDKDir%bin\"
if EXIST "%WindowsSDKDir%bin\%WindowsSDKVersion%" (
    set "WindowsSdkVerBinPath=%WindowsSDKDir%bin\%WindowsSDKVersion%"
)

if "%WindowsSdkDir%"=="" goto :GetWin10SdkDirExit

@REM strip the trailing backslash from WindowsSdkVersion.
set _WinSdkVer_tmp=%WindowsSdkVersion:~0,-1%

if EXIST "%WindowsSdkDir%UnionMetadata\%_WinSdkVer_tmp%" (
  set "WindowsLibPath=%WindowsSdkDir%UnionMetadata\%_WinSdkVer_tmp%;%WindowsSdkDir%References\%_WinSdkVer_tmp%"
) else (
  set "WindowsLibPath=%WindowsSdkDir%UnionMetadata;%WindowsSdkDir%References"
)

set _WinSdkVer_tmp=

:GetWin10SdkDirExit

if "%WindowsSDKNotFound%"=="1" (
  set WindowsSDKNotFound=
  exit /B 1
)
exit /B 0

:GetWin10SdkDirError

@echo [ERROR:%~nx0] Windows SDK %VSCMD_ARG_WINSDK% : '%WindowsSdkDir%include\%VSCMD_ARG_WINSDK%\um' not found or was incomplete
set /A __winsdk_script_err_count=__winsdk_script_err_count+1

exit /B 1

@REM ---------------------------------------------------------------------
:GetWin81SdkDir

@REM Set paths to the Windows 8.1 SDK

if "%VSCMD_DEBUG%" GEQ "3" goto :GetWin81SdkDirVerbose

call :GetWin81SdkDirHelper HKLM\SOFTWARE\Wow6432Node > nul 2>&1
if errorlevel 1 call :GetWin81SdkDirHelper HKCU\SOFTWARE\Wow6432Node > nul 2>&1
if errorlevel 1 call :GetWin81SdkDirHelper HKLM\SOFTWARE > nul 2>&1
if errorlevel 1 call :GetWin81SdkDirHelper HKCU\SOFTWARE > nul 2>&1
if errorlevel 1 exit /B 1
exit /B 0

:GetWin81SdkDirVerbose

@REM Set paths to the Windows 8.1 SDK

call :GetWin81SdkDirHelper HKLM\SOFTWARE\Wow6432Node
if errorlevel 1 call :GetWin81SdkDirHelper HKCU\SOFTWARE\Wow6432Node
if errorlevel 1 call :GetWin81SdkDirHelper HKLM\SOFTWARE
if errorlevel 1 call :GetWin81SdkDirHelper HKCU\SOFTWARE
if errorlevel 1 exit /B 1
exit /B 0

:GetWin81SdkDirHelper

@REM Get Windows 8.1 SDK installed folder, if Windows 10 SDK is not installed or user specified to use 8.1 SDK

set WindowsSDKLibVersion=winv6.3\
set WindowsSdkDir=
set WindowsLibPath=
set WindowsSdkBinPath=

if "%WindowsSdkDir%"=="" for /F "tokens=1,2*" %%i in ('reg query "%1\Microsoft\Microsoft SDKs\Windows\v8.1" /v "InstallationFolder"') DO (
    if "%%i"=="InstallationFolder" (
        SET WindowsSdkDir=%%k
    )
)
if "%WindowsLibPath%"==""  set WindowsLibPath=%WindowsSdkDir%References\CommonConfiguration\Neutral
if "%WindowsSdkDir%"=="" exit /B 1
set "WindowsSdkBinPath=%WindowsSdkDir%bin\"
exit /B 0

:GetWin81SdkDirError

@echo [ERROR:%~nx0] Windows SDK 8.1 : '%WindowsSdkDir%include' not found
cd %WindowsSdkDir%include
set /A __winsdk_script_err_count=__winsdk_script_err_count+1
exit /B 1

@REM -----------------------------------------------------------------------
:GetUniversalCRTSdkDir
set UniversalCRTSdkDir=

if "%VSCMD_DEBUG%" GEQ "3" goto :GetUniversalCRTSdkDirVerbose

call :GetUniversalCRTSdkDirHelper HKLM\SOFTWARE\Wow6432Node > nul 2>&1
if errorlevel 1 call :GetUniversalCRTSdkDirHelper HKCU\SOFTWARE\Wow6432Node > nul 2>&1
if errorlevel 1 call :GetUniversalCRTSdkDirHelper HKLM\SOFTWARE > nul 2>&1
if errorlevel 1 call :GetUniversalCRTSdkDirHelper HKCU\SOFTWARE > nul 2>&1
if errorlevel 1 exit /B 1
exit /B 0

:GetUniversalCRTSdkDirVerbose

call :GetUniversalCRTSdkDirHelper HKLM\SOFTWARE\Wow6432Node
if errorlevel 1 call :GetUniversalCRTSdkDirHelper HKCU\SOFTWARE\Wow6432Node
if errorlevel 1 call :GetUniversalCRTSdkDirHelper HKLM\SOFTWARE
if errorlevel 1 call :GetUniversalCRTSdkDirHelper HKCU\SOFTWARE
if errorlevel 1 exit /B 1
exit /B 0

:GetUniversalCRTSdkDirHelper
for /F "tokens=1,2*" %%i in ('reg query "%1\Microsoft\Windows Kits\Installed Roots" /v "KitsRoot10"') DO (
    if "%%i"=="KitsRoot10" (
        SET UniversalCRTSdkDir=%%k
    )
)
if "%UniversalCRTSdkDir%"=="" (
  exit /B 1
)

setlocal enableDelayedExpansion
for /f %%i IN ('dir "%UniversalCRTSdkDir%include\" /b /ad-h /on') DO (
    set result=%%i
    if "!result:~0,3!"=="10." set CRT=!result!
    if "!result!"=="%VSCMD_ARG_WINSDK%" set match=1
)
if not "%match%"=="" set CRT=%VSCMD_ARG_WINSDK%
endlocal & set UCRTVersion=%CRT%
exit /B 0

@REM -----------------------------------------------------------------------
:test

setlocal

REM;; --- Check for signtool.exe in PATH ---
if "%WindowsSDKDir%" == "" goto :test_lib
@echo [TEST:%~nx0] Checking for 'signtool.exe'...
where signtool.exe > NUL 2>&1
if "%ERRORLEVEL%" NEQ "0" (
    @echo [ERROR:%~nx0] Test 'where signtool.exe' failed.
    set /A __winsdk_script_err_count=__winsdk_script_err_count+1
)

REM;; --- Tests for LIB ---
:test_lib
if "%LIB%"=="" goto :test_include

REM;; --- Check for ucrt.lib in LIB ---
@echo [TEST:%~nx0] Checking for 'ucrt.lib' in LIB...
set "__TEST_LIB=%LIB%"
call :test_lib_helper ucrt.lib
if "%ERRORLEVEL%" NEQ "0" (
    @echo [ERROR:%~nx0] Test for 'ucrt.lib' in LIB failed.
    set /A __winsdk_script_err_count=__winsdk_script_err_count+1
)
set __TEST_LIB=

REM;; --- Check for kernel32.lib in LIB ---
if "%WindowsSDKDir%" == "" goto :test_include

@echo [TEST:%~nx0] Checking for 'kernel32.lib' in LIB...
set "__TEST_LIB=%LIB%"
call :test_lib_helper kernel32.lib
if "%ERRORLEVEL%" NEQ "0" (
    @echo [ERROR:%~nx0] Test for 'kernel32.lib' in LIB failed.
    set /A __winsdk_script_err_count=__winsdk_script_err_count+1
)
set __TEST_LIB=

REM;; -- tests for INCLUDE --
:test_include
if "%INCLUDE%"=="" goto :test_end

REM;; --- Check for corecrt.h in INCLUDE ---
@echo [TEST:%~nx0] Checking for 'corecrt.h' in INCLUDE...
set "__TEST_INCLUDE=%INCLUDE%"
call :test_inc_helper corecrt.h
if "%ERRORLEVEL%" NEQ "0" (
    @echo [ERROR:%~nx0] Test for 'corecrt.h' in INCLUDE failed.
    set /A __winsdk_script_err_count=__winsdk_script_err_count+1
)
set __TEST_INCLUDE=

REM;; --- Check for Windows.h in INCLUDE ---
if "%WindowsSDKDir%" == "" goto :test_end
@echo [TEST:%~nx0] Checking for 'windows.h' in INCLUDE...
set "__TEST_INCLUDE=%INCLUDE%"
call :test_inc_helper windows.h
if "%ERRORLEVEL%" NEQ "0" (
    @echo [ERROR:%~nx0] Test for 'Windows.h' in INCLUDE failed.
    set /A __winsdk_script_err_count=__winsdk_script_err_count+1
)
set __TEST_INCLUDE=

@REM ---- end of test execution ----
:test_end

endlocal & set __winsdk_script_err_count=%__winsdk_script_err_count%
goto :end

@REM ---- Test ucrt.lib ----
:test_lib_helper

for /F "tokens=1* delims=;" %%A in ("%__TEST_LIB%") do (

   if EXIST "%%A\%1" (
      exit /B 0
   )

   set "__TEST_LIB=%%B"
   goto :test_lib_helper
)

@REM if the test gets to this point, then ucrt.lib was not found in LIB
exit /B 1

@REM ---- Test corecrt.h ----
:test_inc_helper

for /F "tokens=1* delims=;" %%A in ("%__TEST_INCLUDE%") do (

   if EXIST "%%A\%1" (
      exit /B 0
   )

   set "__TEST_INCLUDE=%%B"
   goto :test_inc_helper
)

@REM if the test gets to this point, then the file was not found in INCLUDE
exit /B 1

@REM ---- Test ucrt.lib ----
:test_ucrtlib

for /F "tokens=1* delims=;" %%A in ("%__TEST_LIB%") do (

   if EXIST "%%A\ucrt.lib" (
      exit /B 0
   )

   set "__TEST_LIB=%%B"
   goto :test_ucrtlib
)

@REM if the test gets to this point, then ucrt.lib was not found in LIB
exit /B 1

@REM -----------------------------------------------------------------------
:clean_env

set UCRTVersion=
set UniversalCRTSdkDir=
set WindowsLibPath=
set WindowsSdkBinPath=
set WindowsSdkDir=
set WindowsSDKLibVersion=
set WindowsSDKVersion=
set WindowsSDKVerBinPath=

goto :end

@REM -----------------------------------------------------------------------
:export_env

@REM Set path based upon the build environment's architecture.
call :check_host_arch
if "%ERRORLEVEL%" NEQ "0" goto :end

if not "%WindowsSdkDir%"==""   set "PATH=%WindowsSdkBinPath%%VSCMD_ARG_HOST_ARCH%;%PATH%"
if not "%WindowsSdkVerBinPath%" == "" set "PATH=%WindowsSdkVerBinPath%%VSCMD_ARG_HOST_ARCH%;%PATH%"

@REM Set LIB based upon the target environment's architecture
call :check_target_arch
if "%ERRORLEVEL%" NEQ "0" goto :end
if "%WindowsSdkDir%" == "" goto :export_ucrt

set "LIB=%WindowsSdkDir%lib\%WindowsSDKLibVersion%um\%VSCMD_ARG_TGT_ARCH%;%LIB%"

@REM the folowing are architecture neutral
set __tmpwinsdk_include=
if "%INCLUDE%" NEQ "" set "__tmpwinsdk_include=;%INCLUDE%"
set "INCLUDE=%WindowsSdkDir%include\%WindowsSDKVersion%shared;%WindowsSdkDir%include\%WindowsSDKVersion%um;%WindowsSdkDir%include\%WindowsSDKVersion%winrt;%WindowsSdkDir%include\%WindowsSDKVersion%cppwinrt%__tmpwinsdk_include%"
set __tmpwinsdk_include=

set "LIBPATH=%WindowsLibPath%;%LIBPATH%"

@REM -----------------------------------------------------------------------
:export_ucrt
@REM
@REM Set UniversalCRT lib path, the default is the latest installed version.
@REM Note: The UniversalCRT must end up ahead of the Windows SDK in the search path.
@REM
if "%UCRTVersion%" NEQ "" set "INCLUDE=%UniversalCRTSdkDir%include\%UCRTVersion%\ucrt;%INCLUDE%"
@REM
@REM Set UniversalCRT lib path, the default is the latest installed version.
@REM Note: The UniversalCRT must end up ahead of the Windows SDK in the search path.
@REM
if "%UCRTVersion%" NEQ "" set "LIB=%UniversalCRTSdkDir%lib\%UCRTVersion%\ucrt\%VSCMD_ARG_TGT_ARCH%;%LIB%"

goto :end

@REM -----------------------------------------------------------------------
:check_host_arch

set __vscmd_local_host_arch_err=0
if "%VSCMD_ARG_HOST_ARCH%"=="" (
    set __vscmd_local_host_arch_err=1
)
if "%VSCMD_ARG_HOST_ARCH%" NEQ "x64" if "%VSCMD_ARG_HOST_ARCH%" NEQ "x86" (
    if "%VSCMD_ARG_HOST_ARCH%" NEQ "arm" if "%VSCMD_ARG_HOST_ARCH%" NEQ "arm64" (
        set __vscmd_local_host_arch_err=1
    )
)

if "%__vscmd_local_host_arch_err%"=="1" (
    set __vscmd_local_host_arch_err=
    set /A __winsdk_script_err_count=__winsdk_script_err_count+1
    @echo [ERROR:%~nx0] Host architecture is not valid : '%VSCMD_ARG_HOST_ARCH%'
    exit /B 1
)
set __vscmd_local_host_arch_err=
exit /B 0

@REM -----------------------------------------------------------------------
:check_target_arch

set __vscmd_local_tgt_arch_err=0
if "%VSCMD_ARG_TGT_ARCH%"=="" (
    set __vscmd_local_tgt_arch_err=1
)
if "%VSCMD_ARG_TGT_ARCH%" NEQ "x64" if "%VSCMD_ARG_TGT_ARCH%" NEQ "x86" (
    if "%VSCMD_ARG_TGT_ARCH%" NEQ "arm" if "%VSCMD_ARG_TGT_ARCH%" NEQ "arm64" (
        set __vscmd_local_tgt_arch_err=1
    )
)

if "%__vscmd_local_tgt_arch_err%"=="1" (
    set __vscmd_local_tgt_arch_err=
    set /A __winsdk_script_err_count=__winsdk_script_err_count+1
    @echo [ERROR:%~nx0] Target architecture is not valid : '%VSCMD_ARG_TGT_ARCH%'
    exit /B 1
)
set __vscmd_local_tgt_arch_err=
exit /B 0

@REM -----------------------------------------------------------------------
:end
if "%__winsdk_script_err_count%" NEQ "0" (
   set __winsdk_script_err_count=
   exit /B 1
)

set __winsdk_script_err_count=
exit /B 0

