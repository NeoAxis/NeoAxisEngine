


@REM *************************************************************************
@REM The MSBuild command prompt is used for build environments that handle 
@REM discovery of the Windows SDK and other tools. In these cases, the
@REM normal developer command prompt may set environment variables that 
@REM override the discovery mechanism (e.g. WindowsSdkDir).
@REM *************************************************************************

@set VSCMD_BANNER_TEXT_ALT=Visual Studio 2017 MSBuild Command Prompt
@call "%~dp0\vsdevcmd.bat" -no_ext -winsdk=none %*
@set VSCMD_BANNER_TEXT_ALT=

:end
