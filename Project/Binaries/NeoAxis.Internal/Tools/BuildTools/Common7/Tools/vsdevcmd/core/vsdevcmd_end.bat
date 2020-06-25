

if "%VSCMD_DEBUG%" GEQ "1" @echo [DEBUG:core\%~n0] initializing with arguments '%*'

@REM skip to end if -help (-?) were specified
if "%VSCMD_ARG_HELP%" NEQ "" goto :end

@REM If both -test and -clean_env are specified, -clean_env should be ignored.
if "%VSCMD_TEST%" NEQ "" (
    endlocal
    goto :end
)
if "%VSCMD_ARG_CLEAN_ENV%" NEQ "" goto :clean_env

@REM Check for "none", "auto" or a valid directory depending on user specified value in -startdir.
@REM [1] if "none", restore directory to the current directory when vsdevcmd.bat was invoked.
@REM [2] if "auto", process auto-detection of [USERPROFILE]\Source
@REM [3] if no value was specified, check the VSCMD_START_DIR environment variable to determine
@REM     whether a startdir has been specified.
if "%__VSCMD_CURRENT_DIR%" NEQ "" cd /d "%__VSCMD_CURRENT_DIR%"
if /I "%VSCMD_ARG_STARTDIR%" == "none" (
    goto :end
) else if /I "%VSCMD_ARG_STARTDIR%" == "auto" (
    goto :auto_start_dir
) else if "%VSCMD_ARG_STARTDIR%" NEQ "" (
    @echo [ERROR:%~nx0] Invalid -startdir=mode specified
    set /A __vscmd_vsdevcmd_errcount=__vscmd_vsdevcmd_errcount+1
) else if "%VSCMD_START_DIR%" NEQ "" (
    cd /d "%VSCMD_START_DIR%"
)

goto :end

@REM ------------------------------------------------------------------------
:auto_start_dir

@REM Set the current directory that users will be set after the script completes
@REM in the following order:
@REM 1. [VSCMD_START_DIR] will be used if specified in the user environment
@REM 2. [USERPROFILE]\source if it exists 
@REM 3. do nothing
if "%VSCMD_START_DIR%" NEQ "" (
    cd /d "%VSCMD_START_DIR%"
) else if EXIST "%USERPROFILE%\Source" (
    cd /d "%USERPROFILE%\Source"
)

goto :end    



@REM ------------------------------------------------------------------------
:clean_env

set DevEnvDir=
set VSINSTALLDIR=
set VSCMD_VER=
set VisualStudioVersion=

@REM restore old PATH, INCLUDE, LIB, and LIBPATH if they were
@REM saved prior to execution of the VSDevCmd scripts.
if not "%__VSCMD_PREINIT_PATH%"=="" set "PATH=%__VSCMD_PREINIT_PATH%"
if not "%__VSCMD_PREINIT_INCLUDE%"=="" (
    set "INCLUDE=%__VSCMD_PREINIT_INCLUDE%"
) else (
    set INCLUDE=
)
if not "%__VSCMD_PREINIT_LIB%"=="" (
    set "LIB=%__VSCMD_PREINIT_LIB%"
) else (
    set LIB=
)
if not "%__VSCMD_PREINIT_LIBPATH%"=="" (
    set "LIBPATH=%__VSCMD_PREINIT_LIBPATH%"
) else (
    set LIBPATH=
)

@REM clean up the variables that would be set due to command line args
call "%~dp0parse_cmd.bat"

@REM Restore the "classic installer" VS150COMNTOOLS value, if it was set prior
@REM to command prompt init.
if "%__VSCMD_PREINIT_VS150COMNTOOLS%" NEQ "" (
    set "VS150COMNTOOLS=%__VSCMD_PREINIT_VS150COMNTOOLS%"
) else (
    set VS150COMNTOOLS=
)

set __VSCMD_PREINIT_PATH=
set __VSCMD_PREINIT_INCLUDE=
set __VSCMD_PREINIT_LIB=
set __VSCMD_PREINIT_LIBPATH=
set __VSCMD_PREINIT_VS150COMNTOOLS=

@REM Dump then environment after clean-up of ENV scripts.  This is used
@REM for testing to ensure that the pre-init environment can be successfully
@REM restored via -clean_env.
@REM
@REM Note: This mechanism depends on the __VSCMD_PREINIT_* variables having
@REM successfully captured the pre-initialization values AND individual
@REM component scripts having correctly implemented -clean_env functionality
@REM for the variables they set. If end-users further customize the
@REM environment, those settings many not be cleaned-up correctly.
if "%VSCMD_DEBUG%" NEQ "" (
    @echo [DEBUG:%~n0] Writing post-clean environment to %temp%\dd_vsdevcmd15_postclean_env.log
    set > "%temp%\dd_vsdevcmd15_postclean_env.log"
)

goto :end

@REM ------------------------------------------------------------------------
:end
set VSCMD_TEST=
set VSCMD_ARG_HELP=
set VSCMD_ERR=
set __VSCMD_CURRENT_DIR=
