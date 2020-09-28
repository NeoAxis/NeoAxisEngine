$input v_pos, v_colorVisible, v_colorInvisible

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
#include "FragmentFunctions.sh"

SAMPLER2D(s_depthTexture, 0);

uniform vec4 u_simple3DRendererVertex[3];
#define u_color u_simple3DRendererVertex[0]
#define u_colorInvisibleBehindObjects u_simple3DRendererVertex[1]
#define u_useColorFromUniform u_simple3DRendererVertex[2].x
#define u_depthTextureAvailable u_simple3DRendererVertex[2].y

void main()
{
	bool visible = true;
	if(u_depthTextureAvailable > 0.0)
	{	
		vec2 ndc = v_pos.xy/v_pos.w;
#ifdef HLSL
		// flip ndc y coord to match directx v tex coord
		ndc.y = -ndc.y;
#endif
		float rawDepth = texture2D(s_depthTexture, 0.5*ndc + 0.5).r;
		float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
		if(depth < v_pos.z) visible = false;
	}	
	
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
