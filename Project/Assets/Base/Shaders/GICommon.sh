// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

//!!!!

uint giGetBit( uint x, uint y, uint z, bool firstBitSet )
{
	uint z2 = firstBitSet ? z : ( z - 2 );	
	return 1u << ( ( z2 * 4 + y ) * 4u + x );
}

/*
uint giGetBit( uint x, uint y, uint z )
{
	return 1u << ( ( z * 4 + y ) * 4u + x );
}
*/


vec4 convertRGBA8ToVec4( uint val )
{
    return vec4(
		float((val & 0x000000FF)), 
		float((val & 0x0000FF00) >> 8U), 
		float((val & 0x00FF0000) >> 16U), 
		float((val & 0xFF000000) >> 24U));
}

uint convertVec4ToRGBA8( vec4 val )
{
    return (
		uint(val.w) & 0x000000FF) << 24U | 
		(uint(val.z) & 0x000000FF) << 16U | 
		(uint(val.y) & 0x000000FF) << 8U  | 
		(uint(val.x) & 0x000000FF);
}

uvec4 convertRGBA8ToUVec4( uint val )
{
	return uvec4( 
		uint((val & 0x000000FF)), 
		uint((val & 0x0000FF00) >> 8U), 
		uint((val & 0x00FF0000) >> 16U), 
		uint((val & 0xFF000000) >> 24U));
}

uint convertUVec4ToRGBA8( uvec4 val )
{
    return 
		(val.w & 0x000000FF) << 24U | 
		(val.z & 0x000000FF) << 16U | 
		(val.y & 0x000000FF) << 8U  | 
		(val.x & 0x000000FF);
}

/*
vec4 giEncodeRGBE8(vec3 _rgb)
{
	vec4 rgbe8;
	float maxComponent = max(max(_rgb.x, _rgb.y), _rgb.z);
	float exponent = ceil(log2(maxComponent) );
	rgbe8.xyz = _rgb / exp2(exponent);
	
	//!!!!
	float w = ( exponent + 128.0 );
	if( w > 253.0 )
		w = 253.0;
	rgbe8.w = w / 255.0;
	
	//rgbe8.w = (exponent + 128.0) / 255.0;
	
	return rgbe8;
}

vec3 giDecodeRGBE8(vec4 _rgbe8)
{
//!!!!?
	float exponent = _rgbe8.w * 255.0 - 128.0;
	//float exponent = _rgbe8.w * 255.0 - 128.0;
	vec3 rgb = _rgbe8.xyz * exp2(exponent);
	return rgb;
}

void giUnpackGridValue( uint gridValue, inout bool isColor, inout vec3 color, inout ivec3 sdfGridIndex )
{
	vec4 rgba8 = convertRGBA8ToVec4( gridValue );
	
	isColor = rgba8.w < 254.0;
	color = giDecodeRGBE8( rgba8 / 255.0 );
	sdfGridIndex = ivec3( rgba8.xyz );
}

uint giPackGridValue_Color( vec3 color )
{
	return convertVec4ToRGBA8( giEncodeRGBE8( color ) * 255.0 );
}

uint giPackGridValue_SdfGridIndex( ivec3 index )
{
	return convertVec4ToRGBA8( vec4( vec3( index ), 255 ) );
}
*/


/*
#define IndirectDebugMode_None 0
#define IndirectDebugMode_Normal 1
#define IndirectDebugMode_BaseColor 2
#define IndirectDebugMode_Metallic 3
#define IndirectDebugMode_Roughness 4
//#define IndirectDebugMode_Reflectance 5
//#define IndirectDebugMode_Emissive 6
//#define IndirectDebugMode_SubsurfaceColor 7
#define IndirectDebugMode_Radiance 8
*/

//!!!!
/*
bool giGetReflection( usampler3D giBasicGrid, vec4 _showRenderTargetGI0, vec4 _showRenderTargetGI1, vec3 rayStart, vec3 rayDirection, inout vec4 resultColor ) //, inout vec3 resultPosition
{
	//!!!!max length
	
	//!!!!use accelerated structure

	vec3 gridPosition = _showRenderTargetGI0.xyz;
	int gridSize = int( _showRenderTargetGI0.w );
	float cellSize = _showRenderTargetGI1.x;
	int cascade = int( _showRenderTargetGI1.y );
	
	//!!!!
	float step = cellSize / 5;
	const int steps = 2000;
	
	vec3 current = rayStart;
	ivec3 lastCheckedIndex = ivec3( -1000, -1000, -1000 );
	
	LOOP
	for( int n = 0; n < steps; n++ )
	{
		vec3 cellIndexF = ( current - gridPosition ) / cellSize;
		ivec3 cellIndex = ivec3( cellIndexF );

		//!!!! if( any( cellIndex != lastCheckedIndex ) )
		BRANCH
		if( cellIndex.x != lastCheckedIndex.x || cellIndex.y != lastCheckedIndex.y || cellIndex.z != lastCheckedIndex.z ) //if( cellIndex != lastCheckedIndex )
		{
			lastCheckedIndex = cellIndex;
	
			BRANCH
			if( cellIndex.x >= 0 && cellIndex.x < gridSize && cellIndex.y >= 0 && cellIndex.y < gridSize && cellIndex.z >= 0 && cellIndex.z < gridSize )
			{
				ivec3 cellIndex2 = cellIndex;
				//!!!!
				//texCoord2.x += debugModeLevelF;
				//texCoord2.x /= levelCount;
				
				uint gridValue = texelFetch( giBasicGrid, cellIndex2, 0 ).r;
				
				//!!!!
				if( gridValue != 0 )
				{
					resultColor = convertRGBA8ToVec4( gridValue ) / 255.0;
					return true;
					//return gridValue;
				}
			}

			
		////vec3 texCoord = cellIndexF / gridResolution;
		//
		//	BRANCH
		//	if( texCoord.x > 0.0 && texCoord.x < 1.0 && texCoord.y > 0.0 && texCoord.y < 1.0 && texCoord.z > 0.0 && texCoord.z < 1.0 )
		//	{
		//		vec3 texCoord2 = texCoord;
		//		//!!!!
		//		//texCoord2.x += debugModeLevelF;
		//		//texCoord2.x /= levelCount;
		//
		//		uint gridValue = texture3DLod( s_giBasicGrid, texCoord2, 0 ).x;
		//	}
		
		}
		
		current += rayDirection * step;
	}
	
	resultColor = vec4_splat( 0 );
	return false;
}
*/