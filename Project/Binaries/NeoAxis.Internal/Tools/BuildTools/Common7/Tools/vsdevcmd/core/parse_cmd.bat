

if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:core\%~nx0] initializaing with arguments '%*'

@REM This script will be called a second time for the -clean_env
@REM scenario.  We skip parsing the command line and simply goto
@REM env variable clean-up.
if "%VSCMD_ARG_CLEAN_ENV%" NEQ "" goto :clean_env

set __local_parse_error=0

:parse_loop
@for /F "tokens=1,* delims= " %%a in ("%__VSCMD_ARGS_LIST%") do (
    for /F "tokens=1,2 delims==" %%1 in ("%%a") do (
        if "%VSCMD_DEBUG%" GEQ "2" (
            @echo [DEBUG:parse_cmd] inner argument {%%1, %%2}
        )
        call :parse_arg_inner %%1 %%2
    )
    set "__VSCMD_ARGS_LIST=%%b"
    goto :parse_loop
)

if "%VSCMD_DEBUG%" GEQ "2" (
    @echo [DEBUG:parse_cmd] -no_ext : %__VSCMD_ARG_NO_EXT%
    @echo [DEBUG:parse_cmd] -winsdk : %__VSCMD_ARG_WINSDK%
    @echo [DEBUG:parse_cmd] -app_platform : %__VSCMD_ARG_APP_PLAT%
    @echo [DEBUG:parse_cmd] -test   : %__VSCMD_ARG_TEST%
    @echo [DEBUG:parse_cmd] -help   : %__VSCMD_ARG_HELP%
    @echo [DEBUG:parse_cmd] -arch   : %__VSCMD_ARG_TGT_ARCH%
    @echo [DEBUG:parse_cmd] -host_arch : %__VSCMD_ARG_HOST_ARCH%
    @echo [DEBUG:parse_cmd] -vcvars_ver : %__VSCMD_ARG_VCVARS_VER%
    @echo [DEBUG:parse_cmd] -startdir : %__VSCMD_ARG_STARTDIR%
)

@REM Always set help variable
set "VSCMD_ARG_help=%__VSCMD_ARG_HELP%"
if "%__VSCMD_ARG_HELP%" NEQ "" goto :print_help

goto :export_variables

@REM ------------------------------------------------------------------------
:parse_arg_inner
set __local_arg_found=

@REM First part (or only part) of an argument pair.
@REM Note: these are ordered alphabetically

@REM -- /? --
if /I "%1"=="-?" (
    set "__VSCMD_ARG_HELP=1"
    set "__local_arg_found=1"
)
if /I "%1"=="/?" (
    set "__VSCMD_ARG_HELP=1"
    set "__local_arg_found=1"
)

@REM -- /app_platform={platform} --
if /I "%1"=="-app_platform" (
    set "__VSCMD_ARG_APP_PLAT=%2"
    set "__local_arg_found=1"
)
if /I "%1"=="/app_platform" (
    set "__VSCMD_ARG_APP_PLAT=%2"
    set "__local_arg_found=1"
)

@REM -- /clean_env --
if /I "%1"=="-clean_env" (
    set __VSCMD_ARG_CLEAN_ENV=1
    set "__local_arg_found=1"
)
if /I "%1"=="/clean_env" (
    set __VSCMD_ARG_CLEAN_ENV=1
    set "__local_arg_found=1"
)

@REM -- /help --
if /I "%1"=="-help" (
    set "__VSCMD_ARG_HELP=1"
    set "__local_arg_found=1"
)
if /I "%1"=="/help" (
    set "__VSCMD_ARG_HELP=1"
    set "__local_arg_found=1"
)

@REM -- /no_ext --
if /I "%1"=="-no_ext" (
    set "__VSCMD_ARG_NO_EXT=1"
    set "__local_arg_found=1"
)
if /I "%1"=="/no_ext" (
    set "__VSCMD_ARG_NO_EXT=1"
    set "__local_arg_found=1"
)

@REM -- /no_logo --
if /I "%1"=="-no_logo" (
    set __VSCMD_ARG_NO_LOGO=1
    set "__local_arg_found=1"
)
if /I "%1"=="/no_logo" (
    set __VSCMD_ARG_NO_LOGO=1
    set "__local_arg_found=1"
)

@REM -- /test --
if /I "%1"=="-test" (
    set "__VSCMD_ARG_TEST=1"
    set "__local_arg_found=1"
)
if /I "%1"=="/test" (
    set "__VSCMD_ARG_TEST=1"
    set "__local_arg_found=1"
)

@REM -- /winsdk --
if /I "%1"=="-winsdk" (
    set "__VSCMD_ARG_WINSDK=%2"
    set "__local_arg_found=1"
)
if /I "%1"=="/winsdk" (
    set "__VSCMD_ARG_WINSDK=%2"
    set "__local_arg_found=1"
)

@REM -- /arch --
if /I "%1"=="-arch" (
    set "__VSCMD_ARG_TGT_ARCH=%2"
    set "__local_arg_found=1"
)
if /I "%1"=="/arch" (
    set "__VSCMD_ARG_TGT_ARCH=%2"
    set "__local_arg_found=1"
)

@REM -- /host_arch --
if /I "%1"=="-host_arch" (
    set "__VSCMD_ARG_HOST_ARCH=%2"
    set "__local_arg_found=1"
)
if /I "%1"=="/host_arch" (
    set "__VSCMD_ARG_HOST_ARCH=%2"
    set "__local_arg_found=1"
)

@REM -- /startdir
if /I "%1"=="-startdir" (
    set "__VSCMD_ARG_STARTDIR=%2"
    set "__local_arg_found=1"
)
if /I "%1"=="/startdir" (
    set "__VSCMD_ARG_STARTDIR=%2"
    set "__local_arg_found=1"
)

@REM -- /vcvars-specific parameters --
if "%__local_arg_found%" NEQ "1" (
    if /I "%__VSCMD_ARG_NO_EXT%" NEQ "1" (
        set "__VSCMD_INTERNAL_INIT_STATE=parse"
        if EXIST "%~dp0..\ext\vcvars.bat" call "%~dp0..\ext\vcvars.bat" %1 %2
        set "__VSCMD_INTERNAL_INIT_STATE="
    )
)

@REM This is here so there will not be code breaking changes
if /I "%1"=="-vcvars_ver" (
    set "__local_arg_found=1"
)

if /I "%1"=="/vcvars_ver" (
    set "__local_arg_found=1"
)

if "%__local_arg_found%" NEQ "1" (
   if "%2"=="" (
       @echo [ERROR:%~nx0] Invalid command line argument: '%1'. Argument will be ignored.
   ) else (
       @echo [ERROR:%~nx0] Invalid command line argument: '%1=%2'.  Argument will be ignored.
   )
   set /A __local_parse_error=__local_parse_error+1
   set __local_arg_found=
   exit /B 1
)

set __local_arg_found=
exit /B 0

@REM ------------------------------------------------------------------------
:export_variables

@REM **** Export environment variables ****
set VSCMD_TEST=%__VSCMD_ARG_TEST%

@REM only set the following environmnet variables if we are NOT in test mode.
@REM
if "%VSCMD_TEST%" NEQ "" goto :end

set "VSCMD_ARG_winsdk=%__VSCMD_ARG_WINSDK%"

@REM set -app_platform
    if NOT "%__VSCMD_ARG_APP_PLAT%"=="" (
        set "VSCMD_ARG_app_plat=%__VSCMD_ARG_APP_PLAT%"
    ) else (
        set "VSCMD_ARG_app_plat=Desktop"
    )

    @REM Set host and target architecture for tools that depend on this being
    @REM available. Note that we have special handling of "amd64" to convert to
    @REM "x64" due legacy usage of the former.
    if "%__VSCMD_ARG_TGT_ARCH%" NEQ "" (
        if "%__VSCMD_ARG_TGT_ARCH%"=="amd64" (
            set "VSCMD_ARG_TGT_ARCH=x64"
        ) else (
            set "VSCMD_ARG_TGT_ARCH=%__VSCMD_ARG_TGT_ARCH%"
        )
    ) else (
        set "VSCMD_ARG_TGT_ARCH=x86"
    )

    if "%__VSCMD_ARG_HOST_ARCH%" NEQ "" (
        if "%__VSCMD_ARG_HOST_ARCH%"=="amd64" (
            set "VSCMD_ARG_HOST_ARCH=x64"
        ) else (
            set "VSCMD_ARG_HOST_ARCH=%__VSCMD_ARG_HOST_ARCH%"
        )
    ) else (
        @REM By default, the host architecture will match the target
        @REM architecture, which was exported above.
        set "VSCMD_ARG_HOST_ARCH=%VSCMD_ARG_TGT_ARCH%"
    )

    set "VSCMD_ARG_no_ext=%__VSCMD_ARG_NO_EXT%"
    set "VSCMD_ARG_no_logo=%__VSCMD_ARG_NO_LOGO%"
    set "VSCMD_ARG_CLEAN_ENV=%__VSCMD_ARG_CLEAN_ENV%"
    set "__VSCMD_INTERNAL_INIT_STATE=export"
    if EXIST "%~dp0..\ext\vcvars.bat" call "%~dp0..\ext\vcvars.bat"
    set "__VSCMD_INTERNAL_INIT_STATE="
    set "VSCMD_ARG_STARTDIR=%__VSCMD_ARG_STARTDIR%"

goto :end

@REM ------------------------------------------------------------------------
:print_help

@echo .
@echo ** Visual Studio "15" Developer Command Prompt Help **
@echo ** Version : %VSCMD_VER%
@echo .
@echo Syntax: vsdevcmd.bat [options]
@echo [options] :
@echo     -arch=architecture : Architecture for compiled binaries/libraries
@echo            ** x86 [default]
@echo            ** amd64
@echo            ** arm
@echo            ** arm64
@echo     -host_arch=architecture : Architecture of compiler binaries
@echo            ** x86 [default]
@echo            ** amd64
@echo     -winsdk=version : Version of Windows SDK to select.
@echo            ** 10.0.xxyyzz.0 : Windows 10 SDK (e.g 10.0.10240.0)
@echo                               [default : Latest Windows 10 SDK]
@echo            ** 8.1 : Windows 8.1 SDK
@echo            ** none : Do not setup Windows SDK variables.
@echo                      For use with build systems that prefer to
@echo                      determine Windows SDK version independently.
@echo     -app_platform=platform : Application Platform Target Type.
@echo            ** Desktop : Classic Win32 Apps          [default]
@echo            ** UWP : Universal Windows Platform Apps
@echo     -no_ext : Only scripts from [VS150COMNTOOLS]\VsDevCmd\Core
@echo               directory are run during initialization.
@echo     -no_logo : Suppress printing of the developer command prompt banner.
set "__VSCMD_INTERNAL_INIT_STATE=help"
if EXIST "%~dp0..\ext\vcvars.bat" call "%~dp0..\ext\vcvars.bat"
set "__VSCMD_INTERNAL_INIT_STATE="
@echo     -startdir=mode : configures the current directory after (successful) initialization of the environment.
@echo            ** none : the command prompt will exist in the same current directory as when invoked
@echo            ** auto : the command prompt will search for [USERPROFILE]\Source and will change directory
@echo                      if it exists.
@echo            ** If -startdir=mode is not provided, the developer command prompt scripts will 
@echo               additionally check for the [VSCMD_START_DIR] environment variable. If not specified, 
@echo               the default behavior will be 'none' mode.
@echo     -test : Run smoke tests to verify environment integrity in an already-initialized command prompt.
@echo             Executing with -test will NOT modify the environment, so it must be used in a separate call
@echo             to vsdevcmd.bat (all other parameters should be the same as when the environment was 
@echo             initialied)
@echo     -help : prints this help message.
@echo.

goto :end

@REM ------------------------------------------------------------------------
:clean_env

if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:%~n0] cleaning environment

set VSCMD_ARG_app_plat=
set VSCMD_ARG_CLEAN_ENV=
set VSCMD_ARG_help=
set VSCMD_ARG_host_arch=
set VSCMD_ARG_no_ext=
set VSCMD_ARG_no_logo=
set VSCMD_TEST=
set VSCMD_ARG_tgt_arch=
set VSCMD_ARG_winsdk=
set VSCMD_ARG_STARTDIR=

goto :end

@REM ------------------------------------------------------------------------
:end

@REM Remove the local temporary variables used by this script
set __VSCMD_ARG_APP_PLAT=
set __VSCMD_ARG_CLEAN_ENV=
set __VSCMD_ARG_HELP=
set __VSCMD_ARG_HOST_ARCH=
set __VSCMD_ARG_NO_EXT=
set __VSCMD_ARG_NO_LOGO=
set __VSCMD_ARG_TEST=
set __VSCMD_ARG_TGT_ARCH=
set __VSCMD_ARG_WINSDK=
set __VSCMD_ARG_STARTDIR=

if "%__local_parse_error%" NEQ "0" (
    set __local_parse_error=
    exit /B 1
)

set __local_parse_error=
exit /B 0

