$input v_worldPosition_depth, v_colorVisible, v_colorInvisible

// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "Common.sh"
#include "FragmentFunctions.sh"

//!!!!need get depth from geometry in GLSL. right now depth check is skipped
#ifndef GLSL
	SAMPLER2D(s_depthTexture, 0);
#endif

uniform vec4 u_simple3DRendererVertex[3];
#define u_color u_simple3DRendererVertex[0]
#define u_colorInvisibleBehindObjects u_simple3DRendererVertex[1]
#define u_useColorFromUniform u_simple3DRendererVertex[2].x
#define u_depthTextureAvailable u_simple3DRendererVertex[2].y

#if GLOBAL_CUT_VOLUME_MAX_AMOUNT > 0
uniform vec4 u_viewportCutVolumeSettings;
uniform mat4 u_viewportCutVolumeData[GLOBAL_CUT_VOLUME_MAX_AMOUNT];
#endif

bool cutVolumesSimple3DRenderer( vec3 worldPosition )
{
#if GLOBAL_CUT_VOLUME_MAX_AMOUNT > 0

	BRANCH
	if(u_viewportCutVolumeSettings.x > 0.0)
	{
		int count = int(u_viewportCutVolumeSettings.x);
		//BRANCH
		LOOP
		for(int n = 0; n < count; n++)
		{
			mat4 m = u_viewportCutVolumeData[n];
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

void main()
{
	if( cutVolumesSimple3DRenderer( v_worldPosition_depth.xyz ) )
		discard;	
	
	bool visible = true;
//!!!!need get depth from geometry in GLSL. right now depth check is skipped
#ifndef GLSL
	if(u_depthTextureAvailable > 0.0)
	{
		vec2 texCoord = getFragCoord().xy * u_viewportSizeInv;
		float rawDepth = texture2D(s_depthTexture, texCoord).r;
		//!!!!
		//float depth = getDepthValue2(texCoord, rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance, u_viewportOwnerProjectionInverse);
		float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);

		//!!!!
		float offset = 0.05;

		if(depth < v_worldPosition_depth.w - offset)
			visible = false;
		
		
		//float originalZ = gl_FragCoord.z / gl_FragCoord.w;
		//if(depth < originalZ - offset)
		//	visible = false;	
	}
#endif
	
	if(visible)
		gl_FragColor = v_colorVisible;
	else
	{
		gl_FragColor = v_colorInvisible;
		
		//need for support occlusion query
		if(!any2(v_colorInvisible))
			discard;
	}
}
