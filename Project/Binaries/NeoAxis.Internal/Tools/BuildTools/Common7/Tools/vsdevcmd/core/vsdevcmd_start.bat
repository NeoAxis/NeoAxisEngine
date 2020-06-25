

if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:core\%~n0] initializing with arguments '%*'

@REM At this point, [VS150COMNTOOLS] may not have been set, so we call
@REM the command line parsing script in the same directory as this one.
call "%~dp0parse_cmd.bat" %*
if "%ERRORLEVEL%" NEQ "0" set /A __vscmd_vsdevcmd_errcount=__vscmd_vsdevcmd_errcount+1

@REM Save the current directory, if -startdir=none was specified by the user.
if /I "%VSCMD_ARG_STARTDIR%" == "none" set "__VSCMD_CURRENT_DIR=%CD%"

if "%VSCMD_DEBUG%" GEQ "2" (
    @echo [DEBUG:%~n0] Parsing results...
    @echo [DEBUG:%~n0] -clean_env : %VSCMD_ARG_CLEAN_ENV%
    @echo [DEBUG:%~n0] -test      : %VSCMD_TEST%
    @echo [DEBUG:%~n0] -help      : %VSCMD_ARG_HELP%
)

@REM if -? was specified, then help was already printed and we can exit.
if "%VSCMD_ARG_HELP%"=="1" goto :end

@REM Test scripts must not modify the environment, so we protect "test mode"
@REM execution via setlocal and avoid setting additional environment variables
@REM other than those set by the command line argument parsing.
if "%VSCMD_TEST%" NEQ "" (

    if "%VSCMD_DEBUG%" NEQ "" (
        @echo [DEBUG:%~n0] Writing pre-initialization test environment to %temp%\test_dd_vsdevcmd15_env.log
    )
    @REM if running tests, dump the environment to [TEMP]\%test_dd_vsdevcmd15_env.log
    set > %temp%\test_dd_vsdevcmd15_env.log

    @REM matching 'endlocal' is in vsdevcmd_end.bat.
    setlocal

    if "%VSCMD_ARG_CLEAN_ENV%" NEQ "" (
        @echo [ERROR] -clean_env and -test cannot be specified at the same time. Ignoring -clean_env.
        if "%__vscmd_vsdevcmd_errcount%" NEQ "" set /A __vscmd_vsdevcmd_errcount=__vscmd_vsdevcmd_errcount+1
    )
    goto :end
)

@REM clean env should restore the pre-initialization environment.
@REM Variable initialization may be skipped here.
if "%VSCMD_ARG_CLEAN_ENV%" NEQ "" goto :end

@REM PATH should be set, but INCLUDE, LIB, and LIBPATH may not be
@REM set prior to execution of this script.  We'll "key" the capture of the
@REM environment variable values to restore on whether PATH has been saved.
if "%__VSCMD_PREINIT_PATH%"=="" (
    set "__VSCMD_PREINIT_PATH=%PATH%"
    if "%__VSCMD_PREINIT_INCLUDE%"=="" set "__VSCMD_PREINIT_INCLUDE=%INCLUDE%"
    if "%__VSCMD_PREINIT_LIB%"==""set "__VSCMD_PREINIT_LIB=%LIB%"
    if "%__VSCMD_PREINIT_LIBPATH%"=="" set "__VSCMD_PREINIT_LIBPATH=%LIBPATH%"
)

@REM if the classic installer is there, the environment will already have
@REM VS150COMNTOOLS set.  We save that value, so we can restore this
@REM variable on -clean_env.
if "%VS150COMNTOOLS%" NEQ "" (
    if "%__VSCMD_PREINIT_VS150COMNTOOLS%" == "" (
        set "__VSCMD_PREINIT_VS150COMNTOOLS=%VS150COMNTOOLS%"
    )
)

@REM using pushd/popd to avoid having "..\" in the env var.
@REM Set VS150COMNTOOLS to scripts root
@REM Go from Tools/vsdevcmd/core to Tools
pushd "%~dp0..\..\"
set "VS150COMNTOOLS=%CD%\"
popd

if "%VS150COMNTOOLS%" NEQ "" set "PATH=%VS150COMNTOOLS%;%PATH%"

:set_vsinstalldir

if "%VSINSTALLDIR%" NEQ "" goto :set_devenvdir
@REM Set VSINSTALLDIR to Visual Studio Installation Directory
@REM vsdevcmd_start.bat location: Microsoft Visual Studio 15.0\Common7\Tools\vsdevcmd\core
@REM VSINSTALLDIR location: Microsoft Visual Studio 15.0
pushd "%~dp0..\..\..\..\"
set "VSINSTALLDIR=%CD%\"
popd

:set_devenvdir

if "%DevEnvDir%" == "" (
    if EXIST "%VSINSTALLDIR%Common7\IDE\" (
        set "DevEnvDir=%VSINSTALLDIR%Common7\IDE\"
    )
)
if "%DevEnvDir%" NEQ "" set "PATH=%DevEnvDir%;%PATH%"

goto :end

@REM -----------------------------------------------------------------------
:end
if "%VSCMD_DEBUG%" GEQ "2" (
    @echo [DEBUG:%~n0] end of script: VS150COMNTOOLS="%VS150COMNTOOLS%"
    @echo [DEBUG:%~n0] end of script: VSINSTALLDIR="%VSINSTALLDIR%"
    @echo [DEBUG:%~n0] end of script: DevEnvDir="%DevEnvDir%"
)

exit /B 0