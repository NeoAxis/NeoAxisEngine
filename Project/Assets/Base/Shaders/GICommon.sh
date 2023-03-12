// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

vec4 convertRGBA8ToVec4( uint val )
{
    return vec4(float((val & 0x000000FF)), 
		float((val & 0x0000FF00) >> 8U), 
		float((val & 0x00FF0000) >> 16U), 
		float((val & 0xFF000000) >> 24U));
}

uint convertVec4ToRGBA8( vec4 val )
{
	//!!!!slowly. no sense to clamp
    return (uint(val.w) & 0x000000FF) << 24U | 
		(uint(val.z) & 0x000000FF) << 16U | 
		(uint(val.y) & 0x000000FF) << 8U  | 
		(uint(val.x) & 0x000000FF);
}

#define IndirectDebugMode_None 0
#define IndirectDebugMode_Normal 1
#define IndirectDebugMode_BaseColor 2
#define IndirectDebugMode_Metallic 3
#define IndirectDebugMode_Roughness 4
//#define IndirectDebugMode_Reflectance 5
//#define IndirectDebugMode_Emissive 6
//#define IndirectDebugMode_SubsurfaceColor 7
#define IndirectDebugMode_Radiance 8
