# Microsoft Developer Studio Project File - Name="PTypes_Lib_ST" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Static Library" 0x0104

CFG=PTypes_Lib_ST - WIN32 DEBUG
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "PTypes_Lib_ST.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "PTypes_Lib_ST.mak" CFG="PTypes_Lib_ST - WIN32 DEBUG"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "PTypes_Lib_ST - Win32 Release" (based on "Win32 (x86) Static Library")
!MESSAGE "PTypes_Lib_ST - Win32 Debug" (based on "Win32 (x86) Static Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
RSC=rc.exe

!IF  "$(CFG)" == "PTypes_Lib_ST - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release_ST"
# PROP BASE Intermediate_Dir "Release_ST"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Release_ST"
# PROP Intermediate_Dir "Release_ST"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /YX /FD /c
# ADD CPP /nologo /W3 /GX /O2 /I "../include" /D "NDEBUG" /D "_LIB" /D "WIN32" /D "_MBCS" /D "PTYPES_ST" /YX /FD /c
# SUBTRACT CPP /Fr
# ADD BASE RSC /l 0x409 /d "NDEBUG"
# ADD RSC /l 0x409 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"Release_ST\ptypesn.lib"
# Begin Special Build Tool
SOURCE="$(InputPath)"
PostBuild_Cmds=mkdir             ..\lib\            	copy              Release_ST\ptypesn.lib              ..\lib\ 
# End Special Build Tool

!ELSEIF  "$(CFG)" == "PTypes_Lib_ST - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug_ST"
# PROP BASE Intermediate_Dir "Debug_ST"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Debug_ST"
# PROP Intermediate_Dir "Debug_ST"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX /FD /GZ /c
# ADD CPP /nologo /W4 /Gm /GX /Zi /Od /I "../include" /D "_DEBUG" /D "_LIB" /D "WIN32" /D "_MBCS" /D "PTYPES_ST" /FR /YX /FD /GZ /c
# ADD BASE RSC /l 0x409 /d "_DEBUG"
# ADD RSC /l 0x409 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"Debug_ST\ptypesn.lib"

!ENDIF 

# Begin Target

# Name "PTypes_Lib_ST - Win32 Release"
# Name "PTypes_Lib_ST - Win32 Debug"
# Begin Group "Types"

# PROP Default_Filter "h;cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=..\src\patomic.cxx
# End Source File
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

# PROP Default_Filter "h;hpp;hxx;hm;inl"
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

# PROP Default_Filter "h;cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
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
# Begin Group "Inet"

# PROP Default_Filter "h;cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
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
# Begin Source File

SOURCE=..\LICENSE
# End Source File
# End Target
# End Project
