$input v_color0, v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"

uniform vec4 u_canvasClipRectangle;
uniform vec4/*bool, bool*/ u_bc5UNorm_L;//u_bc5UNorm
uniform vec4 u_canvasColorMultiplier;

SAMPLER2D(s_baseTexture, 0);

void main()
{
	//clip rectangle
	vec2 pos = getFragCoord().xy * u_viewTexel.xy;
	if(pos.x < u_canvasClipRectangle.x || pos.y < u_canvasClipRectangle.y || pos.x > u_canvasClipRectangle.z || pos.y > u_canvasClipRectangle.w)
		discard;
	
	vec4 rgba = texture2D(s_baseTexture, v_texCoord0);
	
#ifndef MOBILE
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
	
	gl_FragColor = rgba * v_color0 * u_canvasColorMultiplier;
	
	//gl_FragColor = texture2D(s_baseTexture, v_texCoord0) * v_color0;
}
