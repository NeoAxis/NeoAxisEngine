$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
// SSRT. Copyright (c) 2019 CDRIN. MIT license.
#include "../../Common.sh"
#include "../../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_depthTexture, 1);
SAMPLER2D(s_normalTexture, 2);
//SAMPLER2D(s_noiseTexture, 3);
//SAMPLER2D(s_previousAccumulationBuffer, 4);

//uniform vec4 colorTextureSize;
//uniform vec4/*float*/ zNear;
//uniform vec4/*float*/ zFar;
//uniform vec4/*float*/ fov;
//uniform vec4/*float*/ aspectRatio;
//uniform vec4 viewportSize;
//uniform vec4 noiseTextureSize;

uniform vec4/*float*/ resolution;
uniform vec4/*float*/ reduction;
uniform vec4/*float*/ skyLighting;
uniform vec4/*float*/ radius;
uniform vec4/*float*/ expStart;
uniform vec4/*float*/ expFactor;
//uniform vec4/*float*/ jitterSamples;

//uniform vec4/*float*/ lnDlOffset;
//uniform vec4/*float*/ nDlOffset;

uniform vec4/*float*/ thickness;
uniform vec4/*float*/ falloff;

uniform mat4 itViewMatrix;
//uniform vec4 randomSeeds;
//uniform vec4/*float*/ accumulateFrames;
//uniform mat4 viewProj;
//uniform mat4 invViewProj;
uniform mat4 invProj;
uniform vec4/*vec3*/ cameraPosition;
uniform vec4/*float*/ halfProjScale;


vec3 PositionSSToVS(vec2 uv) 
{
	float rawDepth = texture2D(s_depthTexture, uv).x;	
	float linearDepth = toClipSpaceDepth(rawDepth);
	uv.y = 1.0 - uv.y;
	vec3 posVS;
	posVS.xy = uv * 2 - 1;  // Scale from screen [0, 1] to clip [-1, 1]
	posVS.xy = mul((mat2)invProj, posVS.xy) * linearDepth; // Apply inverse scale/offset, remove w division
	posVS.z = linearDepth;	
	return posVS;	
}

vec3 GetNormalVS(vec2 uv)
{
    vec3 normalWS = texture2D(s_normalTexture, uv).xyz * 2.0 - 1.0;
	vec3 normalVS = mul((mat3)itViewMatrix, normalWS);	
	normalVS = normalize(normalVS);
	normalVS.z = -normalVS.z;
	return normalVS;
}

// From Activision GTAO paper: https://www.activision.com/cdn/research/s2016_pbs_activision_occlusion.pptx
float SpatialOffsets(vec2 uv)
{
	int2 position = (int2)(uv * u_viewportSize.xy);
	return 0.25 * (float)((position.y - position.x) & 3);
}

// Interleaved gradient function from Jimenez 2014 http://goo.gl/eomGso
float GradientNoise(vec2 position)
{
	return frac(52.9829189 * frac(dot(position, vec2( 0.06711056, 0.00583715))));
}

// From http://byteblacksmith.com/improvements-to-the-canonical-one-liner-glsl-rand-for-opengl-es-2-0/
float rand(vec2 co)
{
	float a = 12.9898;
	float b = 78.233;
	float c = 43758.5453;
	float dt = dot(co.xy, vec2(a, b));
	float sn = fmod(dt, 3.14);
	return frac(sin(sn) * c);
}

// Converts color to luminance (grayscale)
float Luminance(vec3 rgb)
{
	vec4 colorSpaceLuminance = vec4(0.22, 0.707, 0.071, 0.0);
    return dot(rgb, colorSpaceLuminance.rgb);
}


void main()
{
	vec4 sceneColor = texture2D(s_sourceTexture, v_texCoord0);

	//!!!!
	//uv += (1.0 / u_viewportSize) * (_ResolutionDownscale == 1 ? 0 : 0.5);

	vec3 jitterSamples = vec4_splat(1);
	vec4 lnDlOffset = vec4_splat(0);
	vec4 nDlOffset = vec4_splat(0);
	
	//!!!!
	float _TemporalOffsets = 0;
	float _TemporalDirections = 0;
	
	vec2 uv = v_texCoord0;
	
	
	vec3 posVS = PositionSSToVS(uv);
	vec3 normalVS = GetNormalVS(uv);
	vec3 viewDir = normalize(-posVS);

	float thickness2 = 1;

	float noiseOffset = SpatialOffsets(uv);
	float noiseDirection = GradientNoise(uv * u_viewportSize.xy);

	float initialRayStep = frac(noiseOffset + _TemporalOffsets) + (rand(uv) * 2.0 - 1.0) * 0.25 * jitterSamples.x;

	vec2 H;
	vec3 col = vec3_splat(0);

	UNROLL
	for (int i = 0; i < ROTATION_COUNT; i++)
	{
		float rotationAngle = (i + noiseDirection + _TemporalDirections) * (PI / (float)ROTATION_COUNT);
		vec3 sliceDir = vec3(cos(rotationAngle), sin(rotationAngle), 0);
		vec2 slideDir_TexelSize = sliceDir.xy * (1.0 / u_viewportSize.xy);
		vec2 h = vec2_splat(-1);
		
		float stepRadius = max((radius.x * halfProjScale) / posVS.z, (float)STEP_COUNT);
		stepRadius /= ((float)STEP_COUNT + 1);
		stepRadius *= expStart.x;

		UNROLL
		for (int j = 0; j < STEP_COUNT; j++)
		{
			vec2 uvOffset = slideDir_TexelSize * max(stepRadius * (j + initialRayStep), 1 + j);
			vec2 uvSlice = uv + uvOffset;
			
			if(uvSlice.x <= 0 || uvSlice.y <= 0 || uvSlice.x >= 1 || uvSlice.y >= 1)
				break;

			stepRadius *= expFactor.x;

			vec3 ds = PositionSSToVS(uvSlice) - posVS;

			float dds = dot(ds, ds);
			float dsdtLength = rsqrt(dds);

			float falloff2 = saturate(dds * (2 / pow(radius.x, 2)) * falloff.x);

			H.x = dot(ds, viewDir) * dsdtLength;
			
			if (H.x > h.x)
			{
				vec3 lmA = texture2DLod(s_sourceTexture, uvSlice, 0).rgb;
				//if(Luminance(lmA) > 0.0)
				{
					float dsl = length(ds);
					float distA = clamp(dsl, 0.1, 50);
					float attA = clamp(1.0 / (/*3.1416*distA**/distA), 0, 50);
					vec3 dsn = normalize(ds);
					float nDlA = saturate(dot(normalVS, dsn) + nDlOffset.x);

					if (attA * nDlA > 0.0)
					{
						vec3 sliceANormal = GetNormalVS(uvSlice);

						float LnDlA = saturate(dot(sliceANormal, -dsn) + lnDlOffset.x);
						
						float skyFactor = 1.0;
						float rawDepth = texture2D(s_depthTexture, uvSlice).x;	
						if(rawDepth >= 1.0)
							skyFactor = skyLighting.x;
						
						col.xyz += attA * lmA * nDlA * LnDlA * skyFactor;
					}
				}
			}
			
			h.x = (H.x > h.x && -(ds.z) < thickness.x) ? lerp(H.x, h.x, falloff2) : lerp(H.x, h.x, thickness2);
		}
		
		UNROLL
		for (j = 0; j < STEP_COUNT; j++) 
		{
			vec2 uvOffset = slideDir_TexelSize * max(stepRadius * (j + initialRayStep), 1 + j);
			vec2 uvSlice = uv - uvOffset;
			
			if(uvSlice.x <= 0 || uvSlice.y <= 0 || uvSlice.x >= 1 || uvSlice.y >= 1)
				break;

			stepRadius *= expFactor.x;

			vec3 dt = PositionSSToVS(uvSlice) - posVS;

			float ddt = dot(dt, dt);
			float dsdtLength = rsqrt(ddt);

			float falloff2 = saturate(ddt * (2 / pow(radius.x, 2)) * falloff.x);

			H = dot(dt, viewDir) * dsdtLength;

			if (H.y > h.y)
			{
				vec3 lmB = texture2DLod(s_sourceTexture, uvSlice, 0).rgb;
				//if(Luminance(lmB) > 0.0)
				{
					float dtl = length(dt);
					float distB = clamp(dtl, 0.1, 50);
					float attB = clamp(1.0 / (/*3.1416*distB**/distB), 0, 50);
					vec3 dtn = normalize(dt);
					float nDlB = saturate(dot(normalVS, dtn) + nDlOffset.x);

					if (attB * nDlB> 0.0)
					{
						vec3 sliceBNormal = GetNormalVS(uvSlice);

						float LnDlB = saturate(dot(sliceBNormal, -dtn) + lnDlOffset.x);
						
						float skyFactor = 1.0;
						float rawDepth = texture2D(s_depthTexture, uvSlice).x;	
						if(rawDepth >= 1.0)
							skyFactor = skyLighting.x;
						
						col.xyz += attB * lmB * nDlB * LnDlB * skyFactor;
					}
				}
			}
			
			h.y = (H.y > h.y && -dt.z < thickness.x) ? lerp(H.y, h.y, falloff2) : lerp(H.y, h.y, thickness2);
		}
	}

	{
		float rawDepth = texture2D(s_depthTexture, uv).x;
		if(rawDepth >= 1.0)
			col *= vec3_splat(skyLighting.x);
	}
	
	col /= ROTATION_COUNT * STEP_COUNT * 2;//max(sampleCount, 1);

	col.rgb -= vec3_splat(reduction.x);
	if(col.x < 0)col.x = 0;
	if(col.y < 0)col.y = 0;
	if(col.z < 0)col.z = 0;
	
	gl_FragColor = vec4(col, 1);
}

///////////////////////////////////////////////////////////////////////////

/*


vec3 getWorldPositionFromScreenCoord(vec2 coord)
{
	float rawDepth = texture2D(s_depthTexture, coord).x;
	return reconstructWorldPosition(invViewProj, coord, rawDepth);
}

vec2 getScreenCoordFromWorldPosition(vec3 pos)
{
	vec4 p = mul(viewProj, vec4(pos,1));
#if BGFX_SHADER_LANGUAGE_HLSL || BGFX_SHADER_LANGUAGE_PSSL || BGFX_SHADER_LANGUAGE_METAL
	p.y = -p.y;
#endif	
	vec2 p2 = (p.xy / p.w) * 0.5 + 0.5;			
	return p2;
}


	//!!!!slowly?
	//world space
	//vec3 position = getWorldPositionFromScreenCoord(v_texCoord0);
	//vec3 normal = normalize(texture2D(s_normalTexture, v_texCoord0).rgb * 2.0 - 1.0);


float getDepth(vec2 coord)
{
	float zdepth = texture2D(s_depthTexture, coord).x;
	return getDepthValue(zdepth, zNear.x, zFar.x);
}

//!!!!reconstructWorldPosition
//!!!!можно проще получить вроде
vec3 getViewPosition(vec2 coord)
{
	vec3 pos;
	pos =  vec3((coord.x * 2.0 - 1.0) / (90.0 / fov.x), (coord.y * 2.0 - 1.0) / aspectRatio.x / (90.0 / fov.x), 1.0);
	return (pos * getDepth(coord));
}

vec2 getScreenCoord(vec3 pos)
{
	vec3 norm = pos / pos.z;
	vec2 view = vec2((norm.x * (90.0 / fov.x) + 1.0) / 2.0, (norm.y * (90.0 / fov.x) * aspectRatio.x + 1.0) / 2.0);
	return view;
}

vec2 snapToPixel(vec2 coord)
{
	coord.x = (floor(coord.x * colorTextureSize.x) + 0.5) / colorTextureSize.x;
	coord.y = (floor(coord.y * colorTextureSize.y) + 0.5) / colorTextureSize.y;
	return coord;
}

*/


/*
//Interval: 0 - 1
float random2( vec2 p )
{
	vec2 k1 = vec2(
		23.14069263277926, // e^pi (Gelfond's constant)
		2.665144142690225 // 2^sqrt(2) (Gelfonda€“Schneider constant)
	);
	return frac( cos( dot( p, k1 ) ) * 12345.6789 );
}

float getVectorsAngle( vec3 vector1, vec3 vector2 )
{
	float _cos = dot( vector1, vector2 ) / ( length(vector1) * length(vector2) );
	_cos = clamp(_cos, -1.0f, 1.0f);
	return acos( _cos );
}
*/

/*
vec2 getSphericalDirectionFromNormal( vec3 dir )
{
	float horizontal = atan2( dir.y, dir.x );
	float dir2Length = sqrt( dir.x * dir.x + dir.y * dir.y );
	float vertical = atan2( dir.z, dir2Length );
	return vec2(horizontal, vertical);
}

vec3 getVectorFromSphericalDirection( vec2 sphericalDirection )
{
	float x = cos( sphericalDirection.y ) * cos( sphericalDirection.x );
	float y = cos( sphericalDirection.y ) * sin( sphericalDirection.x );
	float z = sin( sphericalDirection.y );
	return vec3(x, y, z);
}
*/


/*
//!!!!right?
vec3 getNormalInViewSpace(vec2 texCoord)
{
    vec3 normal = texture2D(s_normalTexture, texCoord).xyz * 2.0 - 1.0;
    
    vec3 tnormal = normalize(mul((mat3)itViewMatrix, normal));
    tnormal.z = -tnormal.z;
    tnormal = normalize(tnormal);

    return tnormal;
}
*/
	

/*

//!!!!
#define MAX_STEPS 40
//#define MAX_RAYS 4

vec3 raymarch(vec3 position, vec3 direction)
{
	
	//!!!!
	//vec4 initialStepScale = vec4(1,0,0,0);
	//vec4 worldThickness = vec4(1,0,0,0);
	
	float stepLength = radius.x / (float)MAX_STEPS;
	vec3 stepVector = direction * stepLength;
	
	//float stepSize = 1.0f / (float)MAX_STEPS;
	
	//direction = direction * stepSize;
	//float stepScale = initialStepScale.x;
	
	vec3 currentPosition = position;// + stepVector;

	for (int steps = 0; steps < MAX_STEPS; steps++)
	{
		//vec3 delta = direction * stepScale * position.z;
		
		currentPosition += stepVector;
		
		//!!!!
		vec2 screenCoord = getScreenCoordFromWorldPosition(currentPosition);
		//vec2 screenCoord = getScreenCoord(currentPosition);
		
		//!!!!zFar проверять?
		if( screenCoord.x < 0.0 || screenCoord.x > 1.0 || screenCoord.y < 0.0 || screenCoord.y > 1.0)
			return vec3_splat(0);
		//if( screenCoord.x < 0.0 || screenCoord.x > 1.0 || screenCoord.y < 0.0 || screenCoord.y > 1.0 || currentPosition.z >  zFar.x || currentPosition.z <  zNear.x)
		//	return vec3_splat(0);

		//!!!!
		//screenCoord = snapToPixel(screenCoord);

		vec3 position2 = getWorldPositionFromScreenCoord(screenCoord);
		float penetration = length(currentPosition - cameraPosition.xyz) - length(position2 - cameraPosition.xyz);
		
		////!!!!в world координатах иначе
		////!!!!может считать расстояние до луча?
		//float penetration = length(currentPosition) - length(getViewPosition(screenCoord));
		
		if (penetration > 0.0)
		{
//			if (stepScale > 1.0)
//			{
//				position -= deltaPos;
//				stepScale *= 0.5;
//			}
//			else
	
			if (penetration < thickness.x)
				return currentPosition;
		}
	}

	return vec3_splat(0);
}

*/
	
	
	
	
/*	
	
	//!!!!
	vec3 color = vec3_splat(0);
	int count = 0;

	//!!!!нормаль проверять

	vec2 noiseUV = v_texCoord0 * viewportSize.xy * noiseTextureSize.zw;
	vec4 noise = texture2D(s_noiseTexture, noiseUV);

	vec2 rand1 = random2(noise.xy + randomSeeds.xy);
	vec2 rand2 = random2(noise.yz + randomSeeds.zw);


	
	//!!!!
	//UNROLL
	//for (int nRay = 0; nRay < 2; nRay++)
	//for (int nRay = 0; nRay < MAX_RAYS; nRay++)
	{
		//!!!!
		//if(nRay == 1)
		//	direction = -direction;
		
		//get random ray direction
		vec3 direction = vec3(rand1.x, rand1.y, rand2.x) * 2.0f - vec3_splat(1.0f);
		if(!any(direction))
			direction.z = 0.01f;	
		direction = normalize(direction);
		////invert normal to make right angle
		//if(getVectorsAngle(direction, normal) > PI * 0.5f)
		//	direction = -direction;

		vec3 collision = raymarch(position, direction);
		if(collision.z != 0)
		{
			vec3 collisionDir = normalize(collision - position);

			float angle = getVectorsAngle(normal, collisionDir);
			float angleCoef = angle / (PI * 0.5f);
			
			//!!!!
			angleCoef = 0.0f;
			
			if(angleCoef <= 1.0f)
			{

				//!!!!расстояние учитывать
		
				vec2 screenCoord = getScreenCoordFromWorldPosition(collision);
				//vec2 screenCoord = getScreenCoord(collision);
				
				vec4 sceneColor2 = texture2D(s_sourceTexture, screenCoord);
				
				color += sceneColor2.rgb * (1.0f - abs(angleCoef));
				count++;
			}
		}
	}
	
	if(count > 1)
		color /= (float)count;
	
	
	if(accumulateFrames.x != 0.0f)
	{
		vec3 previousLighting = texture2D(s_previousAccumulationBuffer, v_texCoord0).rgb;
		
		float accumulateScale = 1.0f / accumulateFrames.x;
		color.rgb *= accumulateScale;

		vec3 c = previousLighting;
		c.rgb -= vec3_splat(accumulateScale);
		if(c.r < 0.0f)
			c.r = 0.0f;
		if(c.g < 0.0f)
			c.g = 0.0f;
		if(c.b < 0.0f)
			c.b = 0.0f;
		
		if(c.r < color.r)
			c.r = color.r;
		if(c.g < color.g)
			c.g = color.g;
		if(c.b < color.b)
			c.b = color.b;
		
		gl_FragColor = vec4(c.rgb, 1);
		
	}
	else
		gl_FragColor = vec4(color.rgb, 1);

*/	
	
	//!!!!
	/*
	{
		vec3 p = vec3(5.82093991462485f, 1.81641286037754f, 1.03291637807473f);
		
		vec2 screen = getScreenCoordFromWorldPosition(p);
		
		//gl_FragColor.r = length(v_texCoord0 - screen);
		if(length(v_texCoord0 - screen) < 0.1f)
			gl_FragColor.r = 1.0f;		
	}*/
