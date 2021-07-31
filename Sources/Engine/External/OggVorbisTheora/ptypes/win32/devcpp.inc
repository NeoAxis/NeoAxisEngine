#
# Pre-build and post-build steps for Dev-C++
# used from PTypes_Lib and WShare projects
#

all-before:
	@cmd /C IF NOT EXIST Release mkdir Release

all-after:
	@cmd /C IF NOT EXIST ..\bin mkdir ..\bin
	@cmd /C IF NOT EXIST ..\lib mkdir ..\lib
	@cmd /C IF EXIST Release\libptypes.a copy Release\libptypes.a ..\lib > NUL
	@cmd /C IF EXIST Release\wshare.exe copy Release\wshare.exe ..\bin > NUL
	@cmd /C IF EXIST ..\bin\wshare.exe strip ..\bin\wshare.exe
