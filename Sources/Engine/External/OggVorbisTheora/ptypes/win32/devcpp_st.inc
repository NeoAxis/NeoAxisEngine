#
# Pre-build and post-build steps for Dev-C++
# used from PTypes_Lib and WShare projects
#

all-before:
	@cmd /C IF NOT EXIST Release_ST mkdir Release_ST

all-after:
	@cmd /C IF NOT EXIST ..\lib mkdir ..\lib
	@cmd /C IF EXIST Release_ST\libptypesn.a copy Release_ST\libptypesn.a ..\lib > NUL
