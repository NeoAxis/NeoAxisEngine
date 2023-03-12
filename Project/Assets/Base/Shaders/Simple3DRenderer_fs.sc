$input v_worldPosition_depth, v_colorVisible, v_colorInvisible

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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

void main()
{
	if( cutVolumes( v_worldPosition_depth.xyz ) )
		discard;	
	
	bool visible = true;
//!!!!need get depth from geometry in GLSL. right now depth check is skipped
#ifndef GLSL
	if(u_depthTextureAvailable > 0.0)
	{
		vec2 texCoord = getFragCoord().xy * u_viewportSizeInv;
		float rawDepth = texture2D(s_depthTexture, texCoord).r;
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
