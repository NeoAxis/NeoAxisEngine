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

.SUFFIXES:	.cxx .obj

AR		= tlib
CXX		= bcc32

INCDIR		= ..\include

INCDIRS		= -I\bcc55\include -I$(INCDIR)

LIBDIRS		= -L\bcc55\lib

CXXFLAGS	= $(CXXDEFS) $(INCDIRS) -w -O2 -P -q -DWIN32 -DNO_PTYPES19_COMPAT -tWM -5

LIBNAME		= ptypes.lib

LIBDEST		= ..\lib

LIBOBJS		= pversion.obj pmem.obj pfatal.obj pstring.obj pcset.obj pcsetdbg.obj \
		pstrmanip.obj pstrutils.obj pstrconv.obj pstrtoi.obj pstrcase.obj ptime.obj \
		punknown.obj pcomponent.obj pexcept.obj ppodlist.obj pobjlist.obj pstrlist.obj ptextmap.obj \
		patomic.obj pasync.obj psemaphore.obj pthread.obj pmsgq.obj ptimedsem.obj \
		prwlock.obj ptrigger.obj pmtxtable.obj pvariant.obj \
		piobase.obj pinstm.obj pinfile.obj ppipe.obj pinmem.obj poutmem.obj \
		pintee.obj poutstm.obj poutfile.obj pinfilter.obj poutfilter.obj pmd5.obj \
		pputf.obj pstdio.obj pfdxstm.obj pnpipe.obj pnpserver.obj \
		pipbase.obj pipsvbase.obj pipstm.obj pipstmsv.obj pipmsg.obj \
		pipmsgsv.obj punit.obj

TLIBOBJS	= +pversion.obj +pmem.obj +pfatal.obj +pstring.obj +pcset.obj +pcsetdbg.obj \
		+pstrmanip.obj +pstrutils.obj +pstrconv.obj +pstrtoi.obj +pstrcase.obj +ptime.obj \
		+punknown.obj +pcomponent.obj +pexcept.obj +ppodlist.obj +pobjlist.obj +pstrlist.obj +ptextmap.obj\
		+patomic.obj +pasync.obj +psemaphore.obj +pthread.obj +pmsgq.obj +ptimedsem.obj \
		+prwlock.obj +ptrigger.obj +pmtxtable.obj +pvariant.obj \
		+piobase.obj +pinstm.obj +pinfile.obj +ppipe.obj +pinmem.obj +poutmem.obj \
		+pintee.obj +poutstm.obj +poutfile.obj +pinfilter.obj +poutfilter.obj +pmd5.obj \
		+pputf.obj +pstdio.obj +pfdxstm.obj +pnpipe.obj +pnpserver.obj \
		+pipbase.obj +pipsvbase.obj +pipstm.obj +pipstmsv.obj +pipmsg.obj \
		+pipmsgsv.obj +punit.obj

HLEVEL1		= $(INCDIR)/pport.h

HLEVEL2		= $(HLEVEL1) $(INCDIR)/ptypes.h $(INCDIR)/pasync.h $(INCDIR)/ptime.h

HLEVEL3		= $(HLEVEL2) $(INCDIR)/pstreams.h

HLEVEL4		= $(HLEVEL3) $(INCDIR)/pinet.h

HALL		= $(HLEVEL4)


.cxx.obj:
	$(CXX) -c $(CXXFLAGS) $<


all: $(LIBNAME) ptypes_test.exe


#
# libptypes
#

$(LIBNAME): $(LIBOBJS)
	-del $(LIBNAME)
	$(AR) $(LIBNAME) @&&|
$(TLIBOBJS)
|
	copy $(LIBNAME) $(LIBDEST)

pversion.obj: pversion.cxx $(HLEVEL1)

pmem.obj: pmem.cxx $(HLEVEL1)

pfatal.obj: pfatal.cxx $(HLEVEL1)

pstring.obj: pstring.cxx $(HLEVEL2)

pcset.obj: pcset.cxx $(HLEVEL2)

pcsetdbg.obj: pcsetdbg.cxx $(HLEVEL2)

pstrmanip.obj: pstrmanip.cxx $(HLEVEL2)

pstrconv.obj: pstrconv.cxx $(HLEVEL2)

pstrtoi.obj: pstrtoi.cxx $(HLEVEL2)

pstrutils.obj: pstrutils.cxx $(HLEVEL2)

pstrcase.obj: pstrcase.cxx $(HLEVEL2)

ptime.obj: ptime.cxx $(HLEVEL2)

punknown.obj: punknown.cxx $(HLEVEL2)

pcomponent.obj: pcomponent.cxx $(HLEVEL2)

pexcept.obj: pexcept.cxx $(HLEVEL2)

ppodlist.obj: ppodlist.cxx $(HLEVEL2)

pobjlist.obj: pobjlist.cxx $(HLEVEL2)

pstrlist.obj: pstrlist.cxx $(HLEVEL2)

ptextmap.obj: ptextmap.cxx $(HLEVEL2)

patomic.obj: patomic.cxx $(HLEVEL2)

pasync.obj: pasync.cxx $(HLEVEL2)

psemaphore.obj: psemaphore.cxx $(HLEVEL2)

pthread.obj: pthread.cxx $(HLEVEL2)

pmsgq.obj: pmsgq.cxx $(HLEVEL2)

ptimedsem.obj: ptimedsem.cxx $(HLEVEL2)

prwlock.obj: prwlock.cxx $(HLEVEL2)

ptrigger.obj: ptrigger.cxx $(HLEVEL2)

pmtxtable.obj: pmtxtable.cxx $(HLEVEL2)

pvariant.obj: pvariant.cxx $(HLEVEL2)

piobase.obj: piobase.cxx $(HLEVEL3)

pinstm.obj: pinstm.cxx $(HLEVEL3)

pinfile.obj: pinfile.cxx $(HLEVEL3)

ppipe.obj: ppipe.cxx $(HLEVEL3)

pintee.obj: pintee.cxx $(HLEVEL3)

pinmem.obj: pinmem.cxx $(HLEVEL3)

poutmem.obj: poutmem.cxx $(HLEVEL3)

poutstm.obj: poutstm.cxx $(HLEVEL3)

poutfile.obj: poutfile.cxx $(HLEVEL3)

pinfilter.obj: pinfilter.cxx $(HLEVEL3)

poutfilter.obj: poutfilter.cxx $(HLEVEL3)

pmd5.obj: pmd5.cxx $(HLEVEL3)

pputf.obj: pputf.cxx $(HLEVEL3)

pstdio.obj: pstdio.cxx $(HLEVEL3)

pfdxstm.obj: pfdxstm.cxx $(HLEVEL3)

pnpipe.obj: pnpipe.cxx $(HLEVEL3)

pnpserver.obj: pnpserver.cxx $(HLEVEL3)

pipbase.obj: pipbase.cxx $(HLEVEL4)

pipsvbase.obj: pipsvbase.cxx $(HLEVEL4)

pipstm.obj: pipstm.cxx $(HLEVEL4)

pipstmsv.obj: pipstmsv.cxx $(HLEVEL4)

pipmsg.obj: pipmsg.cxx $(HLEVEL4)

pipmsgsv.obj: pipmsgsv.cxx $(HLEVEL4)

punit.obj: punit.cxx $(HLEVEL3)


#
# libptypes test program
#

ptypes_test.obj: ptypes_test.cxx $(HALL)

ptypes_test.exe: ptypes_test.obj $(LIBNAME)
	$(CXX) -tWC $(CXXFLAGS) $(LIBDIRS) ptypes_test.obj ptypes.lib

clean: clean-src
	-del $(LIBDEST)\$(LIBNAME)

clean-src:
	-del *.obj
	-del $(LIBNAME)
	-del ptypes_test.exe ptypes_test.tds
	-del stmtest.txt

