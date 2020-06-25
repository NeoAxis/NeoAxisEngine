// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
#import "UtilsNativeWrapper_Mac.h"

EXPORT void UtilsNativeWrapper_GetOSVersion( int* major, int* minor, int* bugFix )
{
	SInt32 nmajor = 0;
	SInt32 nminor = 0;
	SInt32 nbugFix = 0;
	Gestalt(gestaltSystemVersionMajor, &nmajor);
	Gestalt(gestaltSystemVersionMinor, &nminor);
	Gestalt(gestaltSystemVersionBugFix, &nbugFix);
	*major = nmajor;
	*minor = nminor;
	*bugFix = nbugFix;
}

EXPORT bool UtilsNativeWrapper_IsSystem64Bit()
{
	int is64bitCapable;
	size_t len = sizeof(is64bitCapable);
	if(sysctlbyname("hw.optional.x86_64",&is64bitCapable,&len,NULL,0))
		is64bitCapable = NO;

	if(is64bitCapable == YES)
		return true;
	return false;
}
