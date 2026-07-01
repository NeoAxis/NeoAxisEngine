// Copyright 2006–2026 Ivan Efimov. All rights reserved.

//!!!!use push constants
uniform vec4 u_drawParameters;
#define drawID int( u_drawParameters.x )

#define d_renderOperationData0 texelFetch( s_drawBufferTexture, ivec2( 0, drawID ), 0 )
#define d_renderOperationData1 texelFetch( s_drawBufferTexture, ivec2( 1, drawID ), 0 )
#define d_renderOperationData2 texelFetch( s_drawBufferTexture, ivec2( 2, drawID ), 0 )
#define d_renderOperationData3 texelFetch( s_drawBufferTexture, ivec2( 3, drawID ), 0 )
#define d_renderOperationData4 texelFetch( s_drawBufferTexture, ivec2( 4, drawID ), 0 )
#define d_renderOperationData5 texelFetch( s_drawBufferTexture, ivec2( 5, drawID ), 0 )
#define d_renderOperationData6 texelFetch( s_drawBufferTexture, ivec2( 6, drawID ), 0 )
#define d_renderOperationData7 texelFetch( s_drawBufferTexture, ivec2( 7, drawID ), 0 )

#define d_objectInstanceParameters0 texelFetch( s_drawBufferTexture, ivec2( 8, drawID ), 0 )
#define d_objectInstanceParameters1 texelFetch( s_drawBufferTexture, ivec2( 9, drawID ), 0 )

#define d_materialCustomParameters0 texelFetch( s_drawBufferTexture, ivec2( 10, drawID ), 0 )
#define d_materialCustomParameters1 texelFetch( s_drawBufferTexture, ivec2( 11, drawID ), 0 )

#define d_multiMaterialCombinedInfo texelFetch( s_drawBufferTexture, ivec2( 12, drawID ), 0 )
float d_multiMaterialCombinedMaterials_get( uint localGroupMaterialIndex )
{
	vec4 v = texelFetch( s_drawBufferTexture, ivec2( 13 + int( localGroupMaterialIndex ) / 4, drawID ), 0 );
	return v[ localGroupMaterialIndex % 4u ];
}

#define d_viewportCutVolumeSettings texelFetch( s_drawBufferTexture, ivec2( 21, drawID ), 0 )
mat4 d_viewportCutVolumeData_get( int index )
{
	int itemIndex = 22 + index * 4;
	return mtxFromCols(
		texelFetch( s_drawBufferTexture, ivec2( itemIndex + 0, drawID ), 0 ),
		texelFetch( s_drawBufferTexture, ivec2( itemIndex + 1, drawID ), 0 ),
		texelFetch( s_drawBufferTexture, ivec2( itemIndex + 2, drawID ), 0 ),
		texelFetch( s_drawBufferTexture, ivec2( itemIndex + 3, drawID ), 0 ) );
}

mat4 d_decalMatrix_get()
{
	return mtxFromCols(
		texelFetch( s_drawBufferTexture, ivec2( 38, drawID ), 0 ),
		texelFetch( s_drawBufferTexture, ivec2( 39, drawID ), 0 ),
		texelFetch( s_drawBufferTexture, ivec2( 40, drawID ), 0 ),
		texelFetch( s_drawBufferTexture, ivec2( 41, drawID ), 0 ) );
}
#define d_decalNormal texelFetch( s_drawBufferTexture, ivec2( 42, drawID ), 0 )
#define d_decalTangent texelFetch( s_drawBufferTexture, ivec2( 43, drawID ), 0 )

#define d_forwardEnvironmentDataRotation1 texelFetch( s_drawBufferTexture, ivec2( 44, drawID ), 0 )
#define d_forwardEnvironmentDataMultiplierAndAffect1 texelFetch( s_drawBufferTexture, ivec2( 45, drawID ), 0 )
#define d_forwardEnvironmentDataRotation2 texelFetch( s_drawBufferTexture, ivec2( 46, drawID ), 0 )
#define d_forwardEnvironmentDataMultiplierAndAffect2 texelFetch( s_drawBufferTexture, ivec2( 47, drawID ), 0 )
#define d_forwardEnvironmentDataBlendingFactor texelFetch( s_drawBufferTexture, ivec2( 48, drawID ), 0 ).x
vec4 d_forwardEnvironmentIrradiance1_get( int index )
{
	return texelFetch( s_drawBufferTexture, ivec2( 49 + index, drawID ), 0 );
}
vec4 d_forwardEnvironmentIrradiance2_get( int index )
{
	return texelFetch( s_drawBufferTexture, ivec2( 58 + index, drawID ), 0 );
}


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool cutVolumes( vec3 worldPosition )
{
#if GLOBAL_CUT_VOLUME_MAX_AMOUNT > 0

	BRANCH
	if(d_viewportCutVolumeSettings.x > 0.0)
	{
		int count = int(d_viewportCutVolumeSettings.x);
		//BRANCH
		LOOP
		for(int n = 0; n < count; n++)
		{
			mat4 m = d_viewportCutVolumeData_get( n ); //mat4 m = u_viewportCutVolumeData[n];
			float shape = m[3][3];
			m[3][3] = 1.0;

			vec3 p = abs(mul(m, vec4(worldPosition, 1.0)).xyz);
			bool invert = shape < 0.0;
			float shapeAbs = abs(shape);
			
			bool clip = false;
			
			if(shapeAbs == 1.0)
			{
				//Box
				clip = p.x < 0.5 && p.y < 0.5 && p.z < 0.5;
			}
			else if(shapeAbs == 2.0)
			{
				//Sphere
				clip = length(p) < 0.5;
			}
			else// if(shapeAbs == 3.0)
			{
				//Cylinder
				clip = p.x < 0.5 && length(p.yz) < 0.5;
			}
/*			else
			{
				//Plane
				vec4 plane = m[0];
				clip = dot(plane, vec4(worldPosition, 1.0));
			}*/
			
			if(invert)
				clip = !clip;
			
			if(clip)
				return true;
		}
	}

#endif

	return false;
}
