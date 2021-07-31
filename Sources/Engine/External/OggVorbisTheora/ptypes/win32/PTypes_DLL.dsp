# Microsoft Developer Studio Project File - Name="PTypes_DLL" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Dynamic-Link Library" 0x0102

CFG=PTypes_DLL - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "PTypes_DLL.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "PTypes_DLL.mak" CFG="PTypes_DLL - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "PTypes_DLL - Win32 Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "PTypes_DLL - Win32 Debug" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "PTypes_DLL - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "DLL_Release"
# PROP Intermediate_Dir "DLL_Release"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "PTYPES_DLL_EXPORTS" /YX /FD /c
# ADD CPP /nologo /MD /W3 /GX /O2 /I "../include" /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "PTYPES_DLL_EXPORTS" /YX /FD /c
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x419 /d "NDEBUG"
# ADD RSC /l 0x419 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /machine:I386
# ADD LINK32 kernel32.lib user32.lib ws2_32.lib /nologo /dll /map /machine:I386 /out:"DLL_Release/ptypes20.dll"
# Begin Special Build Tool
SOURCE="$(InputPath)"
PostBuild_Cmds=mkdir            ..\so\           	copy             DLL_Release\ptypes20.lib             ..\so\            	copy             DLL_Release\ptypes20.dll             ..\so\ 
# End Special Build Tool

!ELSEIF  "$(CFG)" == "PTypes_DLL - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "DLL_Debug"
# PROP Intermediate_Dir "DLL_Debug"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MTd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "PTYPES_DLL_EXPORTS" /YX /FD /GZ /c
# ADD CPP /nologo /MDd /W4 /Gm /GX /Zi /Od /I "../include" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "PTYPES_DLL_EXPORTS" /FR /YX /FD /GZ /c
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x419 /d "_DEBUG"
# ADD RSC /l 0x417 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /debug /machine:I386 /pdbtype:sept
# ADD LINK32 kernel32.lib user32.lib ws2_32.lib /nologo /dll /incremental:no /debug /machine:I386 /out:"DLL_Debug/ptypes20.dll" /pdbtype:sept

!ENDIF 

# Begin Target

# Name "PTypes_DLL - Win32 Release"
# Name "PTypes_DLL - Win32 Debug"
# Begin Group "Types"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\src\pcomponent.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pcset.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pcsetdbg.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pexcept.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pfatal.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pmem.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pobjlist.cxx
# End Source File
# Begin Source File

SOURCE=..\src\ppodlist.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pstrcase.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pstrconv.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pstring.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pstrlist.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pstrmanip.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pstrtoi.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pstrutils.cxx
# End Source File
# Begin Source File

SOURCE=..\src\ptextmap.cxx
# End Source File
# Begin Source File

SOURCE=..\src\ptime.cxx
# End Source File
# Begin Source File

SOURCE=..\src\punit.cxx
# End Source File
# Begin Source File

SOURCE=..\src\punknown.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pvariant.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pversion.cxx
# End Source File
# End Group
# Begin Group "Public Headers"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\include\pasync.h
# End Source File
# Begin Source File

SOURCE=..\include\pinet.h
# End Source File
# Begin Source File

SOURCE=..\include\pport.h
# End Source File
# Begin Source File

SOURCE=..\include\pstreams.h
# End Source File
# Begin Source File

SOURCE=..\include\ptime.h
# End Source File
# Begin Source File

SOURCE=..\include\ptypes.h
# End Source File
# End Group
# Begin Group "Streams"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\src\pfdxstm.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pinfile.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pinfilter.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pinmem.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pinstm.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pintee.cxx
# End Source File
# Begin Source File

SOURCE=..\src\piobase.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pmd5.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pnpipe.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pnpserver.cxx
# End Source File
# Begin Source File

SOURCE=..\src\poutfile.cxx
# End Source File
# Begin Source File

SOURCE=..\src\poutfilter.cxx
# End Source File
# Begin Source File

SOURCE=..\src\poutmem.cxx
# End Source File
# Begin Source File

SOURCE=..\src\poutstm.cxx
# End Source File
# Begin Source File

SOURCE=..\src\ppipe.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pputf.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pstdio.cxx
# End Source File
# End Group
# Begin Group "Async"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\src\pasync.cxx
# End Source File
# Begin Source File

SOURCE=..\src\patomic.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pmsgq.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pmtxtable.cxx
# End Source File
# Begin Source File

SOURCE=..\src\prwlock.cxx
# End Source File
# Begin Source File

SOURCE=..\src\psemaphore.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pthread.cxx
# End Source File
# Begin Source File

SOURCE=..\src\ptimedsem.cxx
# End Source File
# Begin Source File

SOURCE=..\src\ptrigger.cxx
# End Source File
# End Group
# Begin Group "Inet"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\src\pipbase.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pipmsg.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pipmsgsv.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pipstm.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pipstmsv.cxx
# End Source File
# Begin Source File

SOURCE=..\src\pipsvbase.cxx
# End Source File
# End Group
# Begin Group "Resources"

# PROP Default_Filter "*.rc,*.h"
# Begin Source File

SOURCE=.\dll_version.rc
# End Source File
# Begin Source File

SOURCE=.\resource.h
# End Source File
# End Group
# Begin Source File

SOURCE=..\LICENSE
# End Source File
# End Target
# End Project
