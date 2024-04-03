// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

uniform vec4 u_multiLightParams;
#define d_lightCount int(u_multiLightParams.x)

uniform vec4 u_lightGridParams;
#define d_lightGridEnabled u_lightGridParams.x
#define d_lightGridSize u_lightGridParams.y
#define d_lightGridStart u_lightGridParams.z
#define d_lightGridCellSize u_lightGridParams.w

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define d_lightPosition ( texelFetch( s_lightsTexture, ivec2( 0, nLight ), 0 ).xyz )
#define d_lightBoundingRadius ( texelFetch( s_lightsTexture, ivec2( 0, nLight ), 0 ).w )

#define d_lightDirection ( texelFetch( s_lightsTexture, ivec2( 1, nLight ), 0 ).xyz )
#define d_lightStartDistance ( texelFetch( s_lightsTexture, ivec2( 1, nLight ), 0 ).w )

#define d_lightPower ( texelFetch( s_lightsTexture, ivec2( 2, nLight ), 0 ).xyz )
#define d_lightType ( texelFetch( s_lightsTexture, ivec2( 2, nLight ), 0 ).w )

#define d_lightAttenuation ( texelFetch( s_lightsTexture, ivec2( 3, nLight ), 0 ).xyz )
#define d_lightMaskIndex ( texelFetch( s_lightsTexture, ivec2( 3, nLight ), 0 ).w )

#define d_lightSpot ( texelFetch( s_lightsTexture, ivec2( 4, nLight ), 0 ).xyz )
#define d_shadowMapIndex ( texelFetch( s_lightsTexture, ivec2( 4, nLight ), 0 ).w )

#define d_lightMaskMatrix mtxFromRows( texelFetch( s_lightsTexture, ivec2( 5, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 6, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 7, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 8, nLight ), 0 ) )

#define d_lightShadowIntensity ( texelFetch( s_lightsTexture, ivec2( 9, nLight ), 0 ).x )
#define d_lightShadowTextureSize ( texelFetch( s_lightsTexture, ivec2( 9, nLight ), 0 ).y )
#define d_lightShadowMapFarClipDistance ( texelFetch( s_lightsTexture, ivec2( 9, nLight ), 0 ).z )
#define d_lightShadowCascadesVisualize ( texelFetch( s_lightsTexture, ivec2( 9, nLight ), 0 ).w )

#define d_lightShadowBias ( texelFetch( s_lightsTexture, ivec2( 10, nLight ), 0 ).x )
#define d_lightShadowNormalBias ( texelFetch( s_lightsTexture, ivec2( 10, nLight ), 0 ).y )
#define d_lightShadowSoftness ( texelFetch( s_lightsTexture, ivec2( 10, nLight ), 0 ).z )
#define d_lightShadowContactLength ( texelFetch( s_lightsTexture, ivec2( 10, nLight ), 0 ).w )

#define d_lightShadowUnitDistanceTexelSizes ( texelFetch( s_lightsTexture, ivec2( 11, nLight ), 0 ) )

#define d_lightShadowTextureViewProjMatrix0 mtxFromRows( texelFetch( s_lightsTexture, ivec2( 12, nLight ), 0 ),  texelFetch( s_lightsTexture, ivec2( 13, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 14, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 15, nLight ), 0 ) )
#define d_lightShadowTextureViewProjMatrix1 mtxFromRows( texelFetch( s_lightsTexture, ivec2( 16, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 17, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 18, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 19, nLight ), 0 ) )
#define d_lightShadowTextureViewProjMatrix2 mtxFromRows( texelFetch( s_lightsTexture, ivec2( 20, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 21, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 22, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 23, nLight ), 0 ) )
#define d_lightShadowTextureViewProjMatrix3 mtxFromRows( texelFetch( s_lightsTexture, ivec2( 24, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 25, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 26, nLight ), 0 ), texelFetch( s_lightsTexture, ivec2( 27, nLight ), 0 ) )

#define d_lightShadowCascades ( texelFetch( s_lightsTexture, ivec2( 28, nLight ), 0 ) )

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//#define REFLECTION_PROBE_DATA_FRAGMENT_SIZE 2
//#define u_reflectionProbePosition u_reflectionProbeDataFragment[0]
//#define u_reflectionProbeRadius u_reflectionProbeDataFragment[1].x

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#define u_materialBaseColor ( texelFetch( s_materials, ivec2( 0, frameMaterialIndex ), 0 ).xyz )
#define u_materialOpacity ( texelFetch( s_materials, ivec2( 0, frameMaterialIndex ), 0 ).w )

#define u_materialAnisotropyDirection ( texelFetch( s_materials, ivec2( 1, frameMaterialIndex ), 0 ).xyz )
#define u_materialOpacityMaskThreshold ( texelFetch( s_materials, ivec2( 1, frameMaterialIndex ), 0 ).w )

#define u_materialSubsurfaceColor ( texelFetch( s_materials, ivec2( 2, frameMaterialIndex ), 0 ).xyz )
#define u_materialMetallic ( texelFetch( s_materials, ivec2( 2, frameMaterialIndex ), 0 ).w )

#define u_materialSheenColor ( texelFetch( s_materials, ivec2( 3, frameMaterialIndex ), 0 ).xyz )
#define u_materialRoughness ( texelFetch( s_materials, ivec2( 3, frameMaterialIndex ), 0 ).w )

#define u_materialEmissive ( texelFetch( s_materials, ivec2( 4, frameMaterialIndex ), 0 ).xyz )
#define u_materialReflectance ( texelFetch( s_materials, ivec2( 4, frameMaterialIndex ), 0 ).w )

#define u_materialClearCoat ( texelFetch( s_materials, ivec2( 5, frameMaterialIndex ), 0 ).x )
#define u_materialClearCoatRoughness ( texelFetch( s_materials, ivec2( 5, frameMaterialIndex ), 0 ).y )
#define u_materialAnisotropy ( texelFetch( s_materials, ivec2( 5, frameMaterialIndex ), 0 ).z )
#define u_materialThickness ( texelFetch( s_materials, ivec2( 5, frameMaterialIndex ), 0 ).w )

#define u_materialSubsurfacePower ( texelFetch( s_materials, ivec2( 6, frameMaterialIndex ), 0 ).x )
#define u_materialShadingModel int( texelFetch( s_materials, ivec2( 6, frameMaterialIndex ), 0 ).y )
//#define u_materialRayTracingReflection ( texelFetch( s_materials, ivec2( 6, frameMaterialIndex ), 0 ).y )
#define u_materialAnisotropyDirectionBasis ( texelFetch( s_materials, ivec2( 6, frameMaterialIndex ), 0 ).z )
#define u_materialMultiSubMaterialSeparatePassIndex int( texelFetch( s_materials, ivec2( 6, frameMaterialIndex ), 0 ).w )

#define u_materialDisplacementScale ( texelFetch( s_materials, ivec2( 7, frameMaterialIndex ), 0 ).x )
#define u_materialReceiveDecals ( texelFetch( s_materials, ivec2( 7, frameMaterialIndex ), 0 ).y )
//#define u_materialShadingModel int( abs( texelFetch( s_materials, ivec2( 7, frameMaterialIndex ), 0 ).y ) - 1.0 )
//#define u_materialReceiveDecals ( texelFetch( s_materials, ivec2( 7, frameMaterialIndex ), 0 ).y < 0.0 )
////#define u_materialReceiveDecals materialStandardFragment[7].y
#define u_materialUseVertexColor ( texelFetch( s_materials, ivec2( 7, frameMaterialIndex ), 0 ).z )
#define u_materialSoftParticlesDistance ( texelFetch( s_materials, ivec2( 7, frameMaterialIndex ), 0 ).w )



/*
#define MATERIAL_STANDARD_FRAGMENT_SIZE 8

#define u_materialBaseColor materialStandardFragment[0].xyz
#define u_materialOpacity materialStandardFragment[0].w
#define u_materialAnisotropyDirection materialStandardFragment[1].xyz
#define u_materialOpacityMaskThreshold materialStandardFragment[1].w
#define u_materialSubsurfaceColor materialStandardFragment[2].xyz
#define u_materialMetallic materialStandardFragment[2].w
#define u_materialSheenColor materialStandardFragment[3].xyz
#define u_materialRoughness materialStandardFragment[3].w
#define u_materialEmissive materialStandardFragment[4].xyz
#define u_materialReflectance materialStandardFragment[4].w
#define u_materialClearCoat materialStandardFragment[5].x
#define u_materialClearCoatRoughness materialStandardFragment[5].y
#define u_materialAnisotropy materialStandardFragment[5].z
#define u_materialThickness materialStandardFragment[5].w
#define u_materialSubsurfacePower materialStandardFragment[6].x
#define u_materialRayTracingReflection materialStandardFragment[6].y
#define u_materialAnisotropyDirectionBasis materialStandardFragment[6].z
#define u_materialMultiSubMaterialSeparatePassIndex int(materialStandardFragment[6].w)
#define u_materialDisplacementScale materialStandardFragment[7].x

#define u_materialShadingModel int( abs( materialStandardFragment[7].y ) - 1.0 )
#define u_materialReceiveDecals ( materialStandardFragment[7].y < 0.0 )
//#define u_materialReceiveDecals materialStandardFragment[7].y

#define u_materialUseVertexColor materialStandardFragment[7].z
#define u_materialSoftParticlesDistance materialStandardFragment[7].w
*/

/*
void getMaterialData( sampler2D materials, uint frameMaterialIndex, out vec4 materialStandardFragment[ MATERIAL_STANDARD_FRAGMENT_SIZE ] )
{
#ifdef GLSL
	//!!!!need?
	float materialIndex2 = float(frameMaterialIndex);
	UNROLL
	for(int n=0;n<MATERIAL_STANDARD_FRAGMENT_SIZE;n++)
	{
		int x = int(mod(materialIndex2, 64.0) * 8.0 + float(n));
		int y = int(floor(materialIndex2 / 64.0));
		materialStandardFragment[n] = texelFetch(materials, ivec2(x, y), 0);
	}
#else
	uvec2 startIndex = uvec2( ( frameMaterialIndex % 64 ) * 8, frameMaterialIndex / 64 );
	UNROLL
	for( uint n = 0; n < MATERIAL_STANDARD_FRAGMENT_SIZE; n++ )
	{
		materialStandardFragment[ n ] = texelFetch( materials, startIndex + uvec2( n, 0 ), 0 );
		//materialStandardFragment[ n ] = texelFetch( materials, uvec2( ( frameMaterialIndex % 64 ) * 8 + n, frameMaterialIndex / 64 ), 0 );
	}
#endif	
}
*/
