// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common/bgfx_shader.sh"
#include "Common/shaderlib.sh"
#include "UniformsGeneral.sh"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*
#if BGFX_SHADER_TYPE_VERTEX
#define VERTEX 1
#endif

#if BGFX_SHADER_TYPE_FRAGMENT
#define FRAGMENT 1
#endif

#if BGFX_SHADER_TYPE_COMPUTE
#define COMPUTE 1
#endif
*/

#if BGFX_SHADER_LANGUAGE_GLSL
	#define MOBILE 1
#endif

#ifdef MOBILE
	#define HIGHP highp
	#define MEDIUMP mediump
#else
	#define HIGHP
	#define MEDIUMP
#endif

//to mark public function of ShaderScript
#define public

#define GLOBAL_MATERIAL_SHADING_QUALITY 0
#define GLOBAL_MATERIAL_SHADING_BASIC 1
#define GLOBAL_MATERIAL_SHADING_SIMPLE 2

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifndef PI
#define PI 3.14159265359
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define DebugMode_None 0
#define DebugMode_Wireframe 1
#define DebugMode_Geometry 2
#define DebugMode_Surface 3
#define DebugMode_BaseColor 4
#define DebugMode_Metallic 5
#define DebugMode_Roughness 6
#define DebugMode_Reflectance 7
#define DebugMode_Emissive 8
#define DebugMode_Normal 9
#define DebugMode_SubsurfaceColor 10
#define DebugMode_TextureCoordinate0 11
#define DebugMode_TextureCoordinate1 12
#define DebugMode_TextureCoordinate2 13
//#define DebugMode_NormalMap 7
//#define DebugMode_AmbientOcclusionMap 8
//#define DebugMode_SpecialF0 9

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec2 expand(vec2 v)
{
	return v * 2.0 - 1.0;
}

vec3 expand(vec3 v)
{
	return v * 2.0 - 1.0;
}

mat3 toMat3(mat4 m)
{
#ifdef GLSL
	return mat3(m);
#else
	return (mat3)m;
#endif
}

bool any2(vec4 v)
{
#ifdef GLSL
	return v.x != 0.0 || v.y != 0.0 || v.z != 0.0 || v.w != 0.0;
#else
	return any(v);
#endif
}

bool any2(vec3 v)
{
#ifdef GLSL
	return v.x != 0.0 || v.y != 0.0 || v.z != 0.0;
#else
	return any(v);
#endif
}

bool any2(vec2 v)
{
#ifdef GLSL
	return v.x != 0.0 || v.y != 0.0;
#else
	return any(v);
#endif
}

vec4 pow2(vec4 x, float y)
{
#ifdef GLSL
	return vec4(pow(x.x, y), pow(x.y, y), pow(x.z, y), pow(x.w, y));
#else
	return pow(x, y);
#endif
}

float degreeToRadian( float v )
{
	return v * ( PI / 180.0f );
}

float getValue(mat3 m, int x, int y)
{
#ifdef GLSL
	return m[y][x];
#else
	return m[x][y];
#endif
}

void setValue(inout mat3 m, int x, int y, float v)
{
#ifdef GLSL
	m[y][x] = v;
#else
	m[x][y] = v;
#endif
}

float getValue(mat4 m, int x, int y)
{
#ifdef GLSL
	return m[y][x];
#else
	return m[x][y];
#endif
}

void setValue(inout mat4 m, int x, int y, float v)
{
#ifdef GLSL
	m[y][x] = v;
#else
	m[x][y] = v;
#endif
}

vec3 getTranslate(mat4 m)
{
#ifdef GLSL
	return vec3(m[3][0], m[3][1], m[3][2]);
#else	
	return vec3(m[0][3], m[1][3], m[2][3]);
#endif	
}

void setTranslate(inout mat4 m, vec3 translate)
{
#ifdef GLSL
	m[3][0] = translate[0];
	m[3][1] = translate[1];
	m[3][2] = translate[2];
#else	
	m[0][3] = translate[0];
	m[1][3] = translate[1];
	m[2][3] = translate[2];
#endif
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec3 flipCubemapCoords(vec3 uvw)
{
	return vec3(-uvw.y, uvw.z, uvw.x);
}

vec3 flipCubemapCoords2(vec3 uvw)
{
	return vec3(uvw.y, uvw.z, uvw.x);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef GLOBAL_FOG
float getFogFactor(vec3 worldPosition, float backgroundFactor)
{
	BRANCH
	if(u_fogDistanceMode != 0.0 || u_fogHeightMode != 0.0)
	{
		int distanceMode = int(u_fogDistanceMode);
		int heightMode = int(u_fogHeightMode);
		
		float fog = 0.0;
		
		//Exp, Exp2
		bool modeExp = distanceMode == 1;
		bool modeExp2 = distanceMode == 2;
		if(modeExp || modeExp2)
		{
			float cameraDistance = length(worldPosition - u_viewportOwnerCameraPosition);

			float distance = cameraDistance - u_fogStartDistance;
			if(distance < 0.0)
				distance = 0.0;
			float m = distance * u_fogDensity;
			if( modeExp2 )
				m *= m;
			fog = 1.0 - saturate(1.0 / exp( m * log( 2.718281828 )));
			fog *= backgroundFactor;
		}

		//Height
		if(heightMode != 0)
			fog = saturate(fog + saturate((u_fogHeight - worldPosition.z) / u_fogHeightScale));

		fog *= u_fogColor.w;
		
		return 1.0 - fog;
	}
	else
		return 1.0;
}
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec2 getUnwrappedUV(vec2 texCoord0, vec2 texCoord1, vec2 texCoord2/*, vec2 texCoord3*/, float unwrappedUV)
{
	//if(unwrappedUV == 1.0)return texCoord0;
	if(unwrappedUV == 2.0)return texCoord1;
	if(unwrappedUV == 3.0)return texCoord2;
	//if(unwrappedUV == 4.0)return texCoord3;
	return texCoord0;//vec2_splat(0);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

float getVisibilityDistanceFactor(float visibilityDistance, float cameraDistance)
{
	float startFading = visibilityDistance * 0.95f;
	float v = (cameraDistance - startFading) / (visibilityDistance - startFading);			
	return saturate(1.0f - v);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#ifdef GLOBAL_REMOVE_TEXTURE_TILING

vec4 hash4( vec2 p )
{
	return fract(sin(vec4( 
		1.0+dot(p,vec2(37.0,17.0)),
		2.0+dot(p,vec2(11.0,47.0)),
		3.0+dot(p,vec2(41.0,29.0)),
		4.0+dot(p,vec2(23.0,31.0))))*103.0);
}

vec4 textureNoTile( sampler2D samp, vec2 uv )
{
    vec2 p = floor( uv );
    vec2 f = fract( uv );
	
    // derivatives (for correct mipmapping)
    vec2 ddx2 = dFdx( uv );
    vec2 ddy2 = dFdy( uv );
    
    // voronoi contribution
    vec4 va = vec4_splat( 0.0 );
    float wt = 0.0;
	UNROLL
    for( int j=-1; j<=1; j++ )
	{
		UNROLL
		for( int i=-1; i<=1; i++ )
		{
			vec2 g = vec2( float(i), float(j) );
			vec4 o = hash4( p + g );
			vec2 r = g - f + o.xy;
			float d = dot( r, r );
			float w = exp( -5.0 * d );
			vec4 c = texture2DGrad( samp, uv + o.zw, ddx2, ddy2 );
			va += w * c;
			wt += w;
		}
	}
	
    // normalization
    return va / wt;
}

#endif

vec4 texture2DRemoveTiling(sampler2D tex, vec2 uv, float factor)
{
	vec4 original = texture2DBias(tex, uv, u_mipBias);
#ifdef GLOBAL_REMOVE_TEXTURE_TILING
	//if enable will compile error on multi material
	//however it works good without the tag
	//BRANCH
	if(factor > 0.0)
	{
		vec4 v = textureNoTile(tex, uv);
		return lerp(original, v, factor);
	}
#endif
	return original;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

mat3 makeRotationMatrix(float angle, vec3 axis)
{
	float c, s;
	sincos(angle, s, c);

	float t = 1.0 - c;
	float x = axis.x;
	float y = axis.y;
	float z = axis.z;

	return mtxFromRows(
		t * x * x + c, t * x * y - s * z, t * x * z + s * y,
		t * x * y + s * z, t * y * y + c, t * y * z - s * x,
		t * x * z - s * y, t * y * z + s * x, t * z * z + c
	);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

vec3 mulQuat(vec4 q, vec3 v)
{
	vec3 qvec = q.xyz;
	vec3 uv = cross(qvec, v);
	vec3 uuv = cross(qvec, uv);	
	float qw2 = q.w * 2.0;
	return v + uv * qw2 + uuv * 2.0;
}

vec4 quatInverse(vec4 q)
{
	return vec4(-q.x, -q.y, -q.z, q.w);
}

/*
vec4 quatNormalize(vec4 q)
{
	float len = sqrt( q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w );
	if( len != 0.0 )
		return q * (1.0 / len);
	return q;
}
*/

/*
float vectorsAngle( vec3 vector1, vec3 vector2 )
{
	float _cos = dot( vector1, vector2 ) / ( length(vector1) * length(vector2) );
	_cos = clamp(_cos, -1.0, 1.0);
	return acos(_cos);
}
*/

vec3 sphericalDirectionGetVector( vec2 sphericalDirection )
{
	vec3 result;
	result.x = cos( sphericalDirection.y ) * cos( sphericalDirection.x );
	result.y = cos( sphericalDirection.y ) * sin( sphericalDirection.x );
	result.z = sin( sphericalDirection.y );
	return result;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//!!!!GLSL
#ifndef GLSL
mat4 matInverse( mat4 m )
{
    float n11 = m[0][0], n12 = m[1][0], n13 = m[2][0], n14 = m[3][0];
    float n21 = m[0][1], n22 = m[1][1], n23 = m[2][1], n24 = m[3][1];
    float n31 = m[0][2], n32 = m[1][2], n33 = m[2][2], n34 = m[3][2];
    float n41 = m[0][3], n42 = m[1][3], n43 = m[2][3], n44 = m[3][3];

    float t11 = n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44;
    float t12 = n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44;
    float t13 = n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44;
    float t14 = n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34;

    float det = n11 * t11 + n21 * t12 + n31 * t13 + n41 * t14;
    float idet = 1.0f / det;

    mat4 ret;

    ret[0][0] = t11 * idet;
    ret[0][1] = (n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44) * idet;
    ret[0][2] = (n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44) * idet;
    ret[0][3] = (n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43) * idet;

    ret[1][0] = t12 * idet;
    ret[1][1] = (n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44) * idet;
    ret[1][2] = (n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44) * idet;
    ret[1][3] = (n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43) * idet;

    ret[2][0] = t13 * idet;
    ret[2][1] = (n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44) * idet;
    ret[2][2] = (n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44) * idet;
    ret[2][3] = (n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43) * idet;

    ret[3][0] = t14 * idet;
    ret[3][1] = (n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34) * idet;
    ret[3][2] = (n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34) * idet;
    ret[3][3] = (n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33) * idet;

    return ret;
}
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//!!!!
#ifdef GLSL
vec4 texture2DMaskOpacity(sampler2D tex, vec2 uv, float isLayer, int primitiveID)
#else
vec4 texture2DMaskOpacity(sampler2D tex, vec2 uv, float isLayer, uint primitiveID)
#endif
{
	//!!!!
/*
	BRANCH
	if(isLayer == 2.0)
	{
		int size = (int)textureSize(tex, 0).x;
		
		//!!!!GLSL
		
		int x = (int)primitiveID % size;
		int y = (int)primitiveID / size;

		return texelFetch(tex, ivec2(x, y), 0);
		
//#ifdef GLSL
//	float materialIndex = renderOperationData[0].x;
//	for(int n=0;n<MATERIAL_STANDARD_FRAGMENT_SIZE;n++)
//	{
//		int x = int(mod(materialIndex, 64.0) * 8.0 + float(n));
//		int y = int(floor(materialIndex / 64.0));
//		materialStandardFragment[n] = texelFetch(materials, ivec2(x, y), 0);
//	}
//#else
//	int materialIndex = int(renderOperationData[0].x);
//	for(int n=0;n<MATERIAL_STANDARD_FRAGMENT_SIZE;n++)
//		materialStandardFragment[n] = texelFetch(materials, ivec2((int)(materialIndex % 64) * 8 + n, (int)(materialIndex / 64)), 0);	
//#endif	

		
	}
	else
*/
		return texture2D(tex, uv);
}
