#
#
#  C++ Portable Types Library (PTypes)
#  Version 2.0.2  Released 17-May-2004
#
#  Copyright (C) 2001-2004 Hovik Melikyan
#
#  http://www.melikyan.com/ptypes/
#
#
#
# Makefile for Borland C++ 5.5 (aka C++ Builder)
# Please see notes in ../doc/compiling.html
#

CXX		= bcc32

INCDIRS		= -I\bcc55\include -I..\include

LIBDIRS		= -L\bcc55\lib -L..\lib

CXXFLAGS	= $(CXXDEFS) $(INCDIRS) -w -O2 -P -q -DWIN32 -tWM -5

BINDEST     = ..\bin

OBJS        = wshare.obj request.obj clients.obj sysutils.obj urlutils.obj log.obj \
	    mimetable.obj config.obj utils.obj modules.obj \
	    mod_file.obj mod_wstat.obj mod_about.obj


.cxx.obj:
	$(CXX) -c $(CXXFLAGS) $<


all: wshare.exe

wshare.exe: $(OBJS)
	$(CXX) -tWC $(CXXFLAGS) $(LIBDIRS) $(OBJS) ptypes.lib
    copy wshare.exe $(BINDEST)

wshare.obj: wshare.cxx clients.h request.h sysutils.h urlutils.h log.h config.h

request.obj: request.cxx request.h modules.h clients.h sysutils.h urlutils.h log.h config.h

clients.obj: clients.cxx clients.h log.h config.h

sysutils.obj: sysutils.cxx sysutils.h utils.h

urlutils.obj: urlutils.cxx urlutils.h

utils.obj: utils.cxx utils.h sysutils.h urlutils.h config.h

log.obj: log.cxx log.h config.h

config.obj: config.cxx config.h sysutils.h

mimetable.obj: mimetable.cxx config.h

modules.obj: modules.cxx modules.h request.h

mod_file.obj: mod_file.cxx config.h sysutils.h utils.h request.h clients.h

mod_wstat.obj: mod_wstat.cxx config.h utils.h request.h clients.h

mod_about.obj: mod_about.cxx config.h utils.h request.h


clean: clean-src
   	-del $(BINDEST)\wshare.exe

clean-src:
	-del *.obj
	-del wshare.exe wshare.tds
