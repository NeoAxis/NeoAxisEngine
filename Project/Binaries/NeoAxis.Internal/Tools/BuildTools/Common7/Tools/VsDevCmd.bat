

@if NOT "%VSCMD_DEBUG%" GEQ "3" @echo off

@REM If in debug mode, we want to log the environment variable state
@REM prior to VSDevCmd.bat being executed. This is disabled by default
@REM and is enabled by setting [VSCMD_DEBUG] to some value.
if "%VSCMD_DEBUG%" NEQ "" (
        @echo [DEBUG:%~n0] Writing pre-initialization environment to %temp%\dd_vsdevcmd15_preinit_env.log
        set > %temp%\dd_vsdevcmd15_preinit_env.log
)

@REM Dump the pre-initialization environment if debug level is 2 or greater (detailed or full trace).
if "%VSCMD_DEBUG%" GEQ "2" (
    @echo [DEBUG:%~nx0] --------------------- VS Developer Command Prompt Environment [pre-init] ---------------------
    set
    @echo [DEBUG:%~nx0] --------------------- VS Developer Command Prompt Environment [pre-init] --------------------- 
)

@REM script-local error counter
set __vscmd_vsdevcmd_errcount=0

@REM Parse the command line and set variables needed.
@REM Need to use this variable instead of passing arguments to escape
@REM the /? option, which will otherwise display the help for 'call'.
set "__VSCMD_ARGS_LIST=%*"
call "%~dp0vsdevcmd\core\vsdevcmd_start.bat"
set __VSCMD_ARGS_LIST=

@REM if -? was specified, then help was already printed and we can exit.
if "%VSCMD_ARG_HELP%"=="1" goto :end

@REM Set VisualStudioVersion for compatibility with previous revisions of the
@REM VS Developer Command Prompt.
set "VisualStudioVersion=15.0"

@REM set the version number to ensure the banner/logo can print it.
@REM We set the version number to the general VS Version (e.g. 15.0)
@REM but will attempt to get a more specific build number from
@REM devenv.exe, if that file is found.
set "VSCMD_VER=15.0"
call :get_vscmd_ver
call :print_vscmd_header

if "%VSCMD_DEBUG%" GEQ "2" (
    @echo [DEBUG:%~nx0] -clean_env : %VSCMD_ARG_CLEAN_ENV%
    @echo [DEBUG:%~nx0] -test : %VSCMD_TEST%
    @echo [DEBUG:%~nx0] VS150COMNTOOLS : "%VS150COMNTOOLS%"
)

@REM Process scripts 'core' and then 'ext in alphabetical order'.
call :process_core
call :process_ext

goto :end

@REM ------------------------------------------------------------------------
:process_core

@REM *****************************************************************
@REM This section processes known scripts under vsdevcmd\core.
@REM These scripts must be explicitly included in this section to be
@REM called.
@REM
@REM This section should only contain support for components that
@REM are required by environment scripts (i.e. dependencies). All
@REM leaf node scripts should be placed in vsdevcmd\ext, instead.
@REM *****************************************************************

@REM *** .NET Framework ***
:core_dotnet
if EXIST "%VS150COMNTOOLS%VsDevCmd\core\dotnet.bat" call :call_script_helper core\dotnet.bat

@REM *** msbuild ***
:core_msbuild
if EXIST "%VS150COMNTOOLS%VsDevCmd\core\msbuild.bat" call :call_script_helper core\msbuild.bat

@REM *** Windows SDK ***
:core_winsdk
if EXIST "%VS150COMNTOOLS%VsDevCmd\core\winsdk.bat" call :call_script_helper core\winsdk.bat

exit /B 0

@REM ------------------------------------------------------------------------
:process_ext

if "%VSCMD_ARG_NO_EXT%"=="1" (
    if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:%~nx0] Skipping vsdevcmd\ext scripts since -no_ext was specified
    goto :ext_end
)

@REM *****************************************************************
@REM This section executes all .bat files found in vsdevcmd\ext.
@REM Any "leaf node" script should be placed in this directory.
@REM A few notes:
@REM * For determinism sake, the scripts are called in alphabetical
@REM   order.
@REM * This section does NOT recursively look in sub-directories
@REM   under vsdevcmd\ext. Sub-directories may be used for
@REM   "implementation detail" scripts called by .bat files in the
@REM   vsdevcmd\ext folder.
@REM *****************************************************************

@REM Iterate through ext scripts
if NOT EXIST "%VS150COMNTOOLS%vsdevcmd\ext\" (
    @echo [ERROR:%~nx0] Cannot find 'ext' folder "%VS150COMNTOOLS%vsdevcmd\ext\"
    set /A __vscmd_vsdevcmd_errcount=__vscmd_vsdevcmd_errcount+1
    goto :ext_end
)

for /F %%a in ( 'dir "%VS150COMNTOOLS%vsdevcmd\ext\*.bat" /b /a-d-h /on' ) do (
    call :call_script_helper ext\%%a
)

:ext_end
set __vscmd_dir_cmd_opt=
exit /B 0

@REM ------------------------------------------------------------------------
:call_script_helper
if NOT EXIST "%VS150COMNTOOLS%vsdevcmd\%1" (
    @echo [ERROR:%~nx0] Script "vsdevcmd\%1" could not be found.
    set /A __vscmd_vsdevcmd_errcount=__vscmd_vsdevcmd_errcount+1
    exit /B 1
)

if "%VSCMD_TEST%" NEQ "" set __VSCMD_INTERNAL_INIT_STATE=test
if "%VSCMD_ARG_CLEAN_ENV%" NEQ "" set __VSCMD_INTERNAL_INIT_STATE=clean

if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:%~nx0] calling "%1"
call "%VS150COMNTOOLS%vsdevcmd\%1"

set __VSCMD_INTERNAL_INIT_STATE=

if "%ERRORLEVEL%" NEQ "0" (
    if "%VSCMD_DEBUG%" NEQ "" @echo [ERROR:%1] init:FAILED code:%ERRORLEVEL%

    set /A __vscmd_vsdevcmd_errcount=__vscmd_vsdevcmd_errcount+1
    exit /B 1
) else (
    if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:%1] init:COMPLETE
)
exit /B 0

:get_vscmd_ver

@REM VsDevCmd.bat location: Microsoft Visual Studio 15.0\Common7\Tools
@REM [devenv|wdexpress].isolation.ini location: Microsoft Visual Studio 15.0\Common7\IDE

@REM First look for wdexpress.isolation.ini, then devenv.isolation.ini, then fallback to 
@REM printing. We look in this order to ensure we can parse only one *.isolation.ini
@REM file. WDExpress has both devenv.isolation.ini and wdexpress.isolation.ini dropped 
@REM into Common7\IDE, but only the later (wdexpress.isolation.ini) has the SemanticVersion
@REM specified.

set __vscmd_isolation_file=
if EXIST "%VSINSTALLDIR%Common7\IDE\wdexpress.isolation.ini" (
    set "__vscmd_isolation_file=%VSINSTALLDIR%Common7\IDE\wdexpress.isolation.ini"
) else if EXIST "%VSINSTALLDIR%Common7\IDE\devenv.isolation.ini" (
    set "__vscmd_isolation_file=%VSINSTALLDIR%Common7\IDE\devenv.isolation.ini"
) else (
    @REM In the "else" case, we'll use the default version number, which comes from the
    @REM Branding file.
    goto :get_vscmd_ver_end
)

set __VSCMD_VER=

@REM Looking for a line of the form "SemanticVersion=<semver>+<bld>", so we split the
@REM contents of the line on '=' and '+'.
for /F "tokens=1,2,* delims==+" %%A in ('type "%__vscmd_isolation_file%"') do (
    if "%VSCMD_DEBUG%" GEQ "3" @echo [DEBUG:%~nx0] Isolation File: "%%A" , "%%B"
    if /I "%%A" == "SemanticVersion" (
        if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:%~nx0] Found version "%%B"
        set "__VSCMD_VER=%%B"
    )
)

if "%__VSCMD_VER%" == "" (
    if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:%~nx0] SemanticVersion not found
) else (
    if "%VSCMD_DEBUG%" GEQ "2" @echo [DEBUG:%~nx0] "%__vscmd_isolation_file%" found. Setting VSCMD_VER="%VSCMD_VER%".
    set "VSCMD_VER=%__VSCMD_VER%"
)

:get_vscmd_ver_end

set __vscmd_isolation_file=
set __VSCMD_VER=
exit /B 0

@REM ------------------------------------------------------------------------
:print_vscmd_header

@REM Allow other Visual Studio command prompts to override the banner text
if "%VSCMD_BANNER_TEXT_ALT%"=="" (
    set "__VSCMD_BANNER_TEXT=Visual Studio 2017 Developer Command Prompt v%VSCMD_VER%"
) else (
    set "__VSCMD_BANNER_TEXT=%VSCMD_BANNER_TEXT_ALT%"
)

if "%VSCMD_ARG_no_logo%"=="" (
    @echo **********************************************************************
    @echo ** %__VSCMD_BANNER_TEXT%
    @echo ** Copyright ^(c^) 2017 Microsoft Corporation
    @echo **********************************************************************
)

set __VSCMD_BANNER_TEXT=
exit /B 0

@REM ------------------------------------------------------------------------
:end

@REM Script clean-up of environment variables used to track
@REM command line options and other state that does not need to
@REM persist past the end of the script.
call "%~dp0vsdevcmd\core\vsdevcmd_end.bat"

if "%__vscmd_vsdevcmd_errcount%" NEQ "0" (
    @echo [ERROR:%~nx0] *** VsDevCmd.bat encountered errors. Environment may be incomplete and/or incorrect. ***
    @echo [ERROR:%~nx0] In an uninitialized command prompt, please 'set VSCMD_DEBUG=[value]' and then re-run 
    @echo [ERROR:%~nx0] vsdevcmd.bat [args] for additional details.
    @echo [ERROR:%~nx0] Where [value] is:
    @echo [ERROR:%~nx0]    1 : basic debug logging
    @echo [ERROR:%~nx0]    2 : detailed debug logging
    @echo [ERROR:%~nx0]    3 : trace level logging. Redirection of output to a file when using this level is recommended.
    @echo [ERROR:%~nx0] Example: set VSCMD_DEBUG=3
    @echo [ERROR:%~nx0]          vsdevcmd.bat ^> vsdevcmd.trace.txt 2^>^&1
    set __vscmd_vsdevcmd_errcount=
    call :final_log
    exit /B 1
) else (
    if "%VSCMD_TEST%" NEQ "" @echo [TEST:%~nx0] *** VsDevCmd.bat tests are complete. ***
)

set __vscmd_vsdevcmd_errcount=

@REM ------------------------------------------------------------------------
:final_log

@REM Dump then environment after execution of vsdevcmd.bat.  This is used
@REM for debugging issues with the developer command prompt.  This logging
@REM is disabled by default and will only be enabled by setting of [VSCMD_DEBUG]
@REM in the environment
if "%VSCMD_DEBUG%" NEQ "" (
    @echo [DEBUG:%~n0] Writing post-execution environment to %temp%\dd_vsdevcmd15_env.log
    set > "%temp%\dd_vsdevcmd15_env.log"
)

@REM Dump the post-initialization environment if debug level is 2 or greater (detailed or full trace).
if "%VSCMD_DEBUG%" GEQ "2" (
    @echo [DEBUG:%~nx0] --------------------- VS Developer Command Prompt Environment [post-init] --------------------- 
    set
    @echo [DEBUG:%~nx0] --------------------- VS Developer Command Prompt Environment [post-init] --------------------- 
)

exit /B 0
