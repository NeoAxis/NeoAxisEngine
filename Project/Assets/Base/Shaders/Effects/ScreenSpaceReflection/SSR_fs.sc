$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "../../UniformsFragment.sh"
#include "../../FragmentFunctions.sh"

SAMPLER2D(s_depthTexture, 0);
SAMPLER2D(s_gBuffer0Texture, 1);
SAMPLER2D(s_normalTexture, 2);
SAMPLER2D(s_gBuffer2Texture, 3);
//using one sampler for s_gBuffer2Texture, s_gBuffer3Texture, s_gBuffer4Texture because samplers limit
SAMPLER2D_TEXTUREONLY(s_gBuffer3Texture, 16); //SAMPLER2D(s_gBuffer3Texture, 4);
SAMPLER2D_TEXTUREONLY(s_gBuffer4Texture, 17); //SAMPLER2D(s_gBuffer4Texture, 5);
SAMPLER2D(s_gBuffer5Texture, 6);

SAMPLERCUBE(s_environmentTexture, 4);

SAMPLER2D(s_brdfLUT, 7);

SAMPLER2D(s_lightsTexture, 8);
#ifdef GLOBAL_LIGHT_GRID
	SAMPLER3D(s_lightGrid, 9);
#endif

#ifdef SHADOW_MAP
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		SAMPLER2DARRAY(s_shadowMapShadowDirectional, 10);
	#else
		SAMPLER2DARRAYSHADOW(s_shadowMapShadowDirectional, 10);
	#endif
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		SAMPLER2DARRAY(s_shadowMapShadowSpot, 11);
	#else
		SAMPLER2DARRAYSHADOW(s_shadowMapShadowSpot, 11);
	#endif
	#ifdef SHADOW_TEXTURE_FORMAT_BYTE4
		SAMPLERCUBEARRAY(s_shadowMapShadowPoint, 12);
	#else
		SAMPLERCUBEARRAYSHADOW(s_shadowMapShadowPoint, 12);
	#endif
#endif

#ifdef GLOBAL_LIGHT_MASK
	SAMPLER2DARRAY(s_lightMaskDirectional, 13);
	SAMPLER2DARRAY(s_lightMaskSpot, 14);
	SAMPLERCUBEARRAY(s_lightMaskPoint, 15);
#endif

#ifdef SHADOW_MAP
	#include "../../ShadowReceiverFunctions.sh"
#endif

uniform vec4 u_deferredEnvironmentData[4];
#define u_deferredEnvironmentDataRotation u_deferredEnvironmentData[0]
#define u_deferredEnvironmentDataMultiplierAndAffect u_deferredEnvironmentData[1]
#define u_deferredEnvironmentDataIBLRotation u_deferredEnvironmentData[2]
#define u_deferredEnvironmentDataIBLMultiplierAndAffect u_deferredEnvironmentData[3]
uniform vec4 u_deferredEnvironmentIrradiance[9];

uniform vec4 u_ssrParameters[ 3 ];
#define colorTextureSize u_ssrParameters[ 0 ].xy
#define colorTextureSizeInv u_ssrParameters[ 0 ].zw
#define stepScaleStart u_ssrParameters[ 1 ].x
#define penetrationThreshold u_ssrParameters[ 1 ].y
#define fov u_ssrParameters[ 1 ].z
#define aspectRatio u_ssrParameters[ 1 ].w
#define stepScaleThreshold u_ssrParameters[ 2 ].x
#define maxDistance u_ssrParameters[ 2 ].y

#define SHADING_MODEL_SUBSURFACE
#define SHADING_MODEL_FOLIAGE

#include "../../PBRFilament/common_math.sh"
#include "../../PBRFilament/brdf.sh"
#include "../../PBRFilament/PBRFilament.sh"

float getDepth( vec2 coord )
{
	float zdepth = texture2DLod( s_depthTexture, coord, 0 ).x;
	return getDepthValue( zdepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance );
}

vec3 getViewPosition( vec2 coord )
{
	vec3 pos;
	pos =  vec3((coord.x * 2.0 - 1.0) / (90.0 / fov.x), (coord.y * 2.0 - 1.0) / aspectRatio.x / (90.0 / fov.x), 1.0);
	return (pos * getDepth(coord));
}

vec3 getViewNormal( vec2 coord )
{
	//!!!!try use normal texture?
	
	float pW = colorTextureSizeInv.x; //1.0 / colorTextureSize.x;
	float pH = colorTextureSizeInv.y; //1.0 / colorTextureSize.y;

	vec3 p1  = getViewPosition(coord + vec2(pW, 0.0)).xyz;
	vec3 p2  = getViewPosition(coord + vec2(0.0, pH)).xyz;
	vec3 p3  = getViewPosition(coord + vec2(-pW, 0.0)).xyz;
	vec3 p4  = getViewPosition(coord + vec2(0.0, -pH)).xyz;

	vec3 vP  = getViewPosition(coord);

	vec3 dx  = vP - p1;
	vec3 dy  = p2 - vP;
	vec3 dx2 = p3 - vP;
	vec3 dy2 = vP - p4;

	if (length(dx2) < length(dx) && coord.x - pW >= 0.0 || coord.x + pW > 1.0)
		dx = dx2;
	if (length(dy2) < length(dy) && coord.y - pH >= 0.0 || coord.y + pH > 1.0)
		dy = dy2;

	return normalize(cross(dx, dy));
}

vec2 getScreenCoord( vec3 pos )
{
	vec3 norm = pos / pos.z;
	vec2 view = vec2((norm.x * (90.0 / fov.x) + 1.0) / 2.0, (norm.y * (90.0 / fov.x) * aspectRatio.x + 1.0) / 2.0);
	return view;
}

/*
vec2 snapToPixel( vec2 coord )
{
	return ( floor( coord * colorTextureSize ) + vec2_splat( 0.5 ) ) * colorTextureSizeInv;
		
	////coord.x = (floor(coord.x * colorTextureSize.x) + 0.5) / colorTextureSize.x;
	////coord.y = (floor(coord.y * colorTextureSize.y) + 0.5) / colorTextureSize.y;
	////return coord;
}
*/

vec3 raymarch( vec3 position, vec3 direction, inout bool notFound )
{
	//!!!!
	const int stepTooMuchLimit = 200;

	float maxDistance2 = min( u_viewportOwnerFarClipDistance, maxDistance );

	//!!!!	
	float stepSize = 0.01;
	
	//int u_stepsInt = min( int( u_steps ), 500 );
	//float stepSize = 1.0f / u_steps; //float stepSize = 1.0f / float( MAX_STEPS );
	
	
	vec3 direction2 = direction * stepSize;
	float stepScale = stepScaleStart;
	
	LOOP
	for( int step = 0; step < stepTooMuchLimit; step++ )
	{
		vec3 deltaPos = direction2 * stepScale * position.z;
		position += deltaPos;
		
		vec2 screenCoord = getScreenCoord( position );

		if( screenCoord.x < 0.0 || screenCoord.x > 1.0 || screenCoord.y < 0.0 || screenCoord.y > 1.0 || position.z > maxDistance2 || position.z < u_viewportOwnerNearClipDistance )
		{
			notFound = true;
			return vec3_splat( 0 );
		}

		//vec2 screenCoord2 = snapToPixel( screenCoord );
		float penetration = length( position ) - length( getViewPosition( screenCoord ) );

		if( penetration > 0.0 )
		{
			if ( stepScale > stepScaleThreshold )
			{
				position -= deltaPos;
				stepScale *= 0.5;
			}
			else if( penetration < penetrationThreshold )
			{
				notFound = false;
				return position;
			}
		}
	}

	notFound = true;
	return vec3_splat( 0 );
	
	
	/*
	int u_stepsInt = min( int( u_steps ), 500 );
	float stepSize = 1.0f / u_steps; //float stepSize = 1.0f / float( MAX_STEPS );
	
	vec3 direction2 = direction * stepSize;
	float stepScale = stepScaleStart;
	
	LOOP
	for( int steps = 0; steps < u_stepsInt; steps++ ) //for( int steps = 0; steps < MAX_STEPS; steps++ )
	{
		vec3 deltaPos = direction2 * stepScale * position.z;
		position += deltaPos;
		vec2 screenCoord = getScreenCoord( position );

		if( screenCoord.x < 0.0 || screenCoord.x > 1.0 || screenCoord.y < 0.0 || screenCoord.y > 1.0 || position.z > u_viewportOwnerFarClipDistance || position.z < u_viewportOwnerNearClipDistance )
		{
			notFound = true;
			return vec3_splat( 0 );
		}

		screenCoord = snapToPixel( screenCoord );
		float penetration = length( position ) - length( getViewPosition( screenCoord ) );

		if( penetration > 0.0 )
		{
			if ( stepScale > stepScaleThreshold )
			{
				position -= deltaPos;
				stepScale *= 0.5;
			}
			else if( penetration < penetrationThreshold )
			{
				notFound = false;
				return position;
			}
		}
	}

	notFound = true;
	return vec3_splat( 0 );
	*/
}

vec4 glossyReflection( vec3 position, vec3 viewNormal, vec3 view, vec4 fragCoord )
{
	const float fresnel_multiplier = 2.8;
	const float fresnel_pow = 2.0;

	vec3 reflected = normalize( reflect( view, viewNormal ) );
	bool collisionNotFound;
	vec3 collision = raymarch( position, reflected, collisionNotFound );

	vec3 reflection;
	float alpha;
	
	BRANCH
	if( collisionNotFound )
	{
		reflection = vec3_splat( 0.5 );
		//reflection = vec3( 1, 0, 0 );
		alpha = 0;
	}
	else
	{
		vec2 screenCoord = getScreenCoord( collision );
		
		float rawDepth = texture2DLod( s_depthTexture, screenCoord, 0 ).r;
		vec3 worldPosition = reconstructWorldPosition( u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, screenCoord, rawDepth );
		
		vec3 baseColor = decodeRGBE8( texture2DLod( s_gBuffer0Texture, screenCoord, 0 ) );
		vec4 normal_and_reflectance = texture2DLod( s_normalTexture, screenCoord, 0 );
		vec3 normal = normalize( normal_and_reflectance.rgb * 2 - 1 );

		vec4 gBuffer2Data = texture2DLod( s_gBuffer2Texture, screenCoord, 0 );
		vec4 gBuffer3Data = texture2DLod( makeSampler( s_gBuffer2Texture, s_gBuffer3Texture ), screenCoord, 0 ); //texture2DLod( s_gBuffer3Texture, screenCoord, 0 );
		vec4 gBuffer4Data = texture2DLod( makeSampler( s_gBuffer2Texture, s_gBuffer4Texture ), screenCoord, 0 ); //texture2DLod( s_gBuffer4Texture, screenCoord, 0 );
		vec4 gBuffer5Data = texture2DLod( s_gBuffer5Texture, screenCoord, 0 );

		vec4 tangent = gBuffer4Data * 2.0 - 1.0;
		tangent.xyz = normalize(tangent.xyz);

//#ifdef SHADING_MODEL_FORCE_SIMPLE
//		const int shadingModel = 4;
//#else
		int shadingModel = int(round(gBuffer5Data.x * 8.0));
//#endif
		bool receiveDecals = gBuffer5Data.y == 1.0;
		float thickness = gBuffer5Data.z;
		float subsurfacePower = gBuffer5Data.w * 15.0;
		
		bool shadingModelSubsurface = shadingModel == 1;
		bool shadingModelFoliage = shadingModel == 2;
		bool shadingModelSimple = shadingModel == 4;//3;
		
		vec3 subsurfaceColor = vec3_splat(0);
		vec3 emissive = vec3_splat(0);
		{
			vec3 v = decodeRGBE8(gBuffer3Data);
			if(shadingModelSubsurface || shadingModelFoliage)
				subsurfaceColor = v;
			else
				emissive = v;
		}
		
		vec3 bitangent = normalize(cross(tangent.xyz, normal) * tangent.w);

		vec3 toCamera = u_viewportOwnerCameraPosition - worldPosition;
		float cameraDistance = length( toCamera );
		toCamera = normalize( toCamera );
		
		//vec2 vCoordsEdgeFact = vec2(1.0, 1.0) - pow(saturate(abs(screenCoord.xy - vec2(0.5, 0.5)) * 2.0), edgeFactorPower.x);
		//float fScreenEdgeFactor = saturate(min(vCoordsEdgeFact.x, vCoordsEdgeFact.y));
		
		//float fresnel = saturate( fresnel_multiplier * pow( 1.0 + dot( view, viewNormal ), fresnel_pow ) );
		
		float ssr_attenuation = 1;
		//can apply three multipliers
		//float ssr_attenuation = saturate(reflected.z) * fresnel * fScreenEdgeFactor;
				
		
		MEDIUMP vec4 resultColor = vec4_splat( 0 );

		//iterate lights
		{

			#ifdef GLOBAL_LIGHT_GRID
				vec2 lightGridIndex = ( worldPosition.xy - vec2_splat( d_lightGridStart ) ) / d_lightGridCellSize;
				vec4 lightGrid0 = texelFetch( s_lightGrid, ivec3( ivec2( lightGridIndex ), 0 ), 0 );
				bool lightGridEnabled2 = d_lightGridEnabled > 0.0 && lightGridIndex.x >= 0 && lightGridIndex.x < d_lightGridSize && lightGridIndex.y >= 0 && lightGridIndex.y < d_lightGridSize && lightGrid0.x >= 0.0;
				int lightCount = lightGridEnabled2 ? ( int( lightGrid0.x ) + 1 ) : d_lightCount;
			#else
				int lightCount = d_lightCount;
			#endif
		
			LOOP
			for( int lightCounter = 1; lightCounter < lightCount; lightCounter++ )
			{
				int nLight;
				#ifdef GLOBAL_LIGHT_GRID
					if( lightGridEnabled2 )
						nLight = int( texelFetch( s_lightGrid, ivec3( ivec2( lightGridIndex ), lightCounter / 4 ), 0 )[ lightCounter % 4 ] );
					else
				#endif
					nLight = lightCounter;

				//cull by bounding sphere
				float lightDistance = length( d_lightPosition - worldPosition );
				if( lightDistance > d_lightBoundingRadius )
					continue;

				int lightType = int(d_lightType);
				
				//objectLightAttenuation and start distance
				float objectLightAttenuation = 1.0;
				BRANCH
				if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT || lightType == ENUM_LIGHT_TYPE_POINT )
				{
					objectLightAttenuation = getLightAttenuation2(d_lightAttenuation, lightDistance);
					if(objectLightAttenuation == 0.0 || lightDistance < d_lightStartDistance)
						continue;
				}

				//lightWorldDirection
				vec3 lightWorldDirection;
				if( lightType == ENUM_LIGHT_TYPE_DIRECTIONAL )
					lightWorldDirection = -d_lightDirection;
				else
					lightWorldDirection = normalize(d_lightPosition - worldPosition);

				BRANCH
				if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
				{
					// factor in spotlight angle
					float rho0 = saturate(dot(-d_lightDirection, lightWorldDirection));
					// factor = (rho - cos(outer/2)) / (cos(inner/2) - cos(outer/2)) ^ falloff 
					float spotFactor0 = saturate(pow(saturate(rho0 - d_lightSpot.y) / (d_lightSpot.x - d_lightSpot.y), d_lightSpot.z));
					if(spotFactor0 == 0.0)
						continue;
					objectLightAttenuation *= spotFactor0;
				}
				
				//lightMaskMultiplier
				vec3 lightMaskMultiplier = vec3(1,1,1);
			#ifdef GLOBAL_LIGHT_MASK
				float lightMaskIndex = d_lightMaskIndex;
				BRANCH
				if( lightMaskIndex >= 0.0 )
				{
					BRANCH
					if( lightType == ENUM_LIGHT_TYPE_POINT )
					{
						vec3 dir = normalize( worldPosition - d_lightPosition );
						dir = mul( d_lightMaskMatrix, vec4( dir, 0 ) ).xyz;
						lightMaskMultiplier = textureCubeArrayLod( s_lightMaskPoint, vec4( dir, lightMaskIndex ), 0.0 ).rgb;
					}
					else if( lightType == ENUM_LIGHT_TYPE_SPOTLIGHT )
					{
						vec4 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) );
						lightMaskMultiplier = texture2DArrayLod( s_lightMaskSpot, vec3( texCoord.xy / texCoord.w, lightMaskIndex ), 0 ).rgb;				
					}
					else
					{
						vec2 texCoord = mul( d_lightMaskMatrix, vec4( worldPosition, 1 ) ).xy;				
						lightMaskMultiplier = texture2DArrayLod( s_lightMaskDirectional, vec3( texCoord, lightMaskIndex ), 0 ).rgb;
					}
				}
			#endif

				//shadows
				float shadowMultiplier = 1.0;
			#ifdef SHADOW_MAP
				float shadowMapIndex = d_shadowMapIndex;
				BRANCH
				if( shadowMapIndex >= 0.0 )
				{
					vec3 plane = u_viewportOwnerCameraDirection;
					//easy to calculate projective depth
					float cascadeDepth = dot( plane, worldPosition );
					
					//vec3 left = cross( u_viewportOwnerCameraDirection, u_viewportOwnerCameraUp );
					//vec3 plane;//vec4 plane;
					//plane.xyz = normalize( cross( u_viewportOwnerCameraUp, left ) );
					////camera position is equal zero plane.w = -( dot( plane.xyz, vec3_splat( 0 ) ) );
					//float cascadeDepth = dot( plane, vec4( worldPosition, 1.0 ) );
					
					//for fading by distance
					float cameraDistance3 = length( worldPosition - u_viewportOwnerCameraPosition );
					//float cameraDistance3 = 0.0;
					
					shadowMultiplier = getShadowMultiplierMulti( worldPosition, cameraDistance3, cascadeDepth, lightWorldDirection, normal, screenCoord, fragCoord, lightType, shadowMapIndex, nLight );
				}		
			#endif
			
				//light color
				vec3 lightColor = ( d_lightPower.rgb * 10000.0 ) * u_cameraExposure * objectLightAttenuation * lightMaskMultiplier * shadowMultiplier;
			
				if( shadingModelSimple )
				{
					//Simple shading model
					resultColor.rgb += baseColor * lightColor;
				}
				else
				{
					//Lit, Subsurface, Foliage shading models

			#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE
					resultColor += baseColor * lightColor * saturate(dot(normal, lightWorldDirection) / PI);
			#else
					
					float metallic = gBuffer2Data.x;
					float roughness = gBuffer2Data.y;
					float ambientOcclusion = gBuffer2Data.z;
					float reflectance = normal_and_reflectance.a;

					MaterialInputs material;

					material.baseColor = vec4(baseColor, 0.0);
					material.roughness = roughness;
					material.metallic = metallic;
					material.reflectance = reflectance;
					material.ambientOcclusion = ambientOcclusion;

		#ifdef MATERIAL_HAS_ANISOTROPY
					material.anisotropyDirection = vec3_splat(0);
					material.anisotropy = 0;
		#endif

					//#if defined(SHADING_MODEL_SUBSURFACE)
					material.thickness = thickness;
					material.subsurfacePower = subsurfacePower;
					material.subsurfaceColor = subsurfaceColor;
					//#endif

		#ifdef MATERIAL_HAS_CLEAR_COAT
					material.clearCoat = 0;
					material.clearCoatRoughness = 0;
					material.clearCoatNormal = vec3_splat(0);
		#endif

					//#if defined(SHADING_MODEL_CLOTH)
					//	material.sheenColor = vec3_splat(0);
					//	material.subsurfaceColor = vec3_splat(0);
					//#endif
					
					ShadingParams shading;
					getPBRFilamentShadingParams( material, tangent.xyz, bitangent, normal, vec3_splat(0), lightWorldDirection, toCamera, false, shading );

					PixelParams pixel;
					getPBRFilamentPixelParams( material, shading, pixel );

					vec3 shadingResult;
					BRANCH
					if( shadingModelSubsurface || shadingModelFoliage )
						shadingResult = surfaceShadingSubSurface( pixel, shading );
					else
						shadingResult = surfaceShadingStandard( pixel, shading );
					resultColor.rgb += shadingResult * lightColor;
					
			#endif					
					
				}
			}	
		}
		
		//ambient light
		{
			vec3 ambientLightPower = texelFetch( s_lightsTexture, ivec2( 2, 0 ), 0 ).xyz * 10000.0;
			
			//light color
			MEDIUMP vec3 lightColor = ambientLightPower * u_cameraExposure;

			if( shadingModelSimple )
			{			
				//Simple shading model
				resultColor.rgb += baseColor * lightColor;
			}
			else
			{
				//Lit, Subsurface, Foliage shading models

		#if GLOBAL_MATERIAL_SHADING == GLOBAL_MATERIAL_SHADING_SIMPLE
				resultColor.rgb += baseColor * lightColor;
		#else
				
				float metallic = gBuffer2Data.x;
				float roughness = gBuffer2Data.y;
				float ambientOcclusion = gBuffer2Data.z;
				float reflectance = normal_and_reflectance.a;

				//toLight
				vec3 toLight = normal; // For ambient light dot(N, L) == 1, i.e. we can do L = N;

				MaterialInputs material;

				material.baseColor = vec4(baseColor, 0.0);
				material.roughness = roughness;
				material.metallic = metallic;
				material.reflectance = reflectance;
				material.ambientOcclusion = ambientOcclusion;

		#ifdef MATERIAL_HAS_ANISOTROPY
				material.anisotropyDirection = vec3_splat(0);
				material.anisotropy = 0;
		#endif

				//#if defined(SHADING_MODEL_SUBSURFACE)
				material.thickness = thickness;
				material.subsurfacePower = subsurfacePower;
				material.subsurfaceColor = subsurfaceColor;
				//#endif

		#ifdef MATERIAL_HAS_CLEAR_COAT
				material.clearCoat = 0;
				material.clearCoatRoughness = 0;
				material.clearCoatNormal = vec3_splat(0);
		#endif

				/*
				#if defined(SHADING_MODEL_CLOTH)
					material.sheenColor = vec3_splat(0);		
					material.subsurfaceColor = vec3_splat(0);
				#endif
				*/

				ShadingParams shading;
				getPBRFilamentShadingParams( material, tangent.xyz, bitangent, normal, vec3_splat(0), toLight, toCamera, false, shading );

				PixelParams pixel;
				getPBRFilamentPixelParams( material, shading, pixel );

				EnvironmentTextureData data1;
				data1.rotation = u_deferredEnvironmentDataRotation;
				data1.multiplierAndAffect = u_deferredEnvironmentDataMultiplierAndAffect;

				EnvironmentTextureData dataIBL;
				dataIBL.rotation = u_deferredEnvironmentDataIBLRotation;
				dataIBL.multiplierAndAffect = u_deferredEnvironmentDataIBLMultiplierAndAffect;

				vec3 diffuse = iblDiffuse( material, pixel, shading, u_deferredEnvironmentIrradiance, dataIBL, s_environmentTexture, data1, shadingModelSubsurface, shadingModelFoliage );
				vec3 specular = iblSpecular( material, pixel, shading, vec3_splat( 0 ), 0, s_environmentTexture, data1 );
				resultColor.rgb += ( diffuse + specular ) * lightColor;
				
		#endif
				
			}			
		}

		//emissive
		resultColor.rgb += emissive * u_emissiveMaterialsFactor;
				
		reflection = max( resultColor.rgb, vec3_splat( 0 ) );
		alpha = ssr_attenuation;
	}
	
#ifdef GLOBAL_FOG
	float fogFactor = getFogFactor( position, collision, collisionNotFound );
	reflection *= fogFactor;
	reflection += u_fogColor.rgb * ( 1.0 - fogFactor );
#endif

	return vec4( reflection, alpha );
}

void main()
{
	vec4 fragCoord = getFragCoord();
	
	vec3 position = getViewPosition( v_texCoord0 );
	vec3 viewNormal = getViewNormal( v_texCoord0 );
	vec3 view = normalize( position );

	vec4 value = glossyReflection( position, viewNormal, view, fragCoord );
	gl_FragColor = value;
}





/*

	//float skyAmount = collisionNotFound ? 1.0 : 0.0;
	//vec3 result = mix(reflection, skyColor, skyAmount);

//uniform vec4 ambientLightTextureRotation;
//uniform vec4 ambientLightTextureMultiplierAndAffect;

	//vec3 specIBL = specularIBL(v_texCoord0);
	
	//vec3 reflection = glossyReflection(position, normal, view, specIBL);
	//gl_FragColor = vec4(reflection, 0.0);	

vec3 glossyReflection(vec3 position, vec3 normal, vec3 view, vec3 specIBL)
{
	vec3 skyColor = saturate(specIBL);
	
	const float fresnel_multiplier = 2.8;
	const float fresnel_pow = 2.0;

	vec3 reflected = normalize(reflect(view, normal));
	bool collisionNotFound;
	vec3 collision = raymarch(position, reflected, collisionNotFound);
	vec2 screenCoord = getScreenCoord(collision);

	vec2 vCoordsEdgeFact = vec2(1.0, 1.0) - pow(saturate(abs(screenCoord.xy - vec2(0.5, 0.5)) * 2.0), edgeFactorPower.x);
	float fScreenEdgeFactor = saturate(min(vCoordsEdgeFact.x, vCoordsEdgeFact.y));

	float fresnel = saturate(fresnel_multiplier * pow(1.0 + dot(view, normal), fresnel_pow));

	float ssr_attenuation = saturate(reflected.z) * fresnel * fScreenEdgeFactor;
	
	vec3 reflection = mix(skyColor, saturate(texture2D(s_sceneTexture, screenCoord).rgb), ssr_attenuation);
	
	float skyAmount = collisionNotFound ? 1.0 : 0.0;
	vec3 result = mix(reflection, skyColor, skyAmount);
	
#ifdef GLOBAL_FOG
	float fogFactor = getFogFactor(position, collision, collisionNotFound);
	result *= fogFactor;
	result += u_fogColor.rgb * (1.0 - fogFactor);
#endif

	return result;
}

vec3 specularIBL(vec2 texCoords)
{
	vec3 baseColor = decodeRGBE8(texture2D(s_gBuffer0Texture, texCoords));
	vec4 gBufferData = texture2D(s_gBuffer2Texture, texCoords);
	vec4 normal_and_reflectance = texture2D(s_normalTexture, texCoords);
	vec3 normal = normalize(normal_and_reflectance.rgb * 2 - 1);

	float rawDepth = texture2D(s_depthTexture, texCoords).r;
	vec3 worldPosition = reconstructWorldPosition(u_viewportOwnerViewInverse, u_viewportOwnerProjectionInverse, texCoords, rawDepth);
	
	vec3 toCamera = normalize(u_viewportOwnerCameraPosition - worldPosition);
	vec3 toLight = normal;

	MaterialInputs material;
	material.baseColor = vec4(baseColor, 0.0);
	material.roughness = gBufferData.y;
	material.metallic = gBufferData.x;
	material.reflectance = normal_and_reflectance.a;
	material.ambientOcclusion = gBufferData.z;

#ifdef MATERIAL_HAS_ANISOTROPY
	material.anisotropy = 0.0f;
	material.anisotropyDirection = vec3_splat(0);
#endif

#ifdef MATERIAL_HAS_CLEAR_COAT
	material.clearCoat = 0;
	material.clearCoatRoughness = 0;
	material.clearCoatNormal = vec3_splat(0);
#endif

	ShadingParams shading;
	getPBRFilamentShadingParams(material, vec3_splat(0), vec3_splat(0), normal, normal, toLight, toCamera, false, shading);//gl_FrontFacing);

	PixelParams pixel;
	getPBRFilamentPixelParams(material, shading, pixel);

	EnvironmentTextureData data;
	data.rotation = ambientLightTextureRotation;
	data.multiplierAndAffect = ambientLightTextureMultiplierAndAffect;

	return getSpecularIrradiance(pixel, shading, s_environmentTexture, data);
	//return iblDiffuse(material, pixel) + iblSpecular(material, pixel, vec3_splat(0), 0);
}
*/
