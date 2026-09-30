$input v_color0, v_texCoord0

// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#include "Common.sh"
#include "FragmentFunctions.sh"

uniform vec4 u_canvasClipRectangle;
uniform vec4/*bool, bool*/ u_bc5UNorm_L;//u_bc5UNorm

SAMPLER2D(s_baseTexture, 0);

//!!!!bindless example
//layout(set = 1, binding = 0) uniform sampler2D g_bindlessSamplerSet[];
//uni__form vec4 s_baseTexture;

void main()
{
	//clip rectangle
	vec2 pos = getFragCoord().xy * u_viewTexel.xy;
	if(pos.x < u_canvasClipRectangle.x || pos.y < u_canvasClipRectangle.y || pos.x > u_canvasClipRectangle.z || pos.y > u_canvasClipRectangle.w)
		discard;
	
//!!!!bindless example
//	vec4 rgba = texture2D(g_bindlessSamplerSet[int(s_baseTexture.x)], v_texCoord0);

	vec4 rgba = texture2D(s_baseTexture, v_texCoord0);

#ifndef LIMITED_DEVICE
	BRANCH
	if(u_bc5UNorm_L.x > 0.0)
	{
		vec3 normal = expand(rgba.xyz);
		normal.z = sqrt(max(1.0 - dot(normal.xy, normal.xy), 0.0));
		normal = normalize(normal);
		rgba.z = normal.z * 0.5 + 0.5;
	}
#endif
	
	if(u_bc5UNorm_L.y > 0.0)
		rgba = vec4(rgba.x, rgba.x, rgba.x, 1.0);

	gl_FragColor = rgba * v_color0;
}
