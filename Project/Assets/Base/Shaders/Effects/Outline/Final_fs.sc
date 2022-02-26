$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "../../FragmentFunctions.sh"

SAMPLER2D(s_outlineTexture, 0);

uniform vec4/*float*/ intensity;
uniform vec4 effectColor;
uniform vec4/*vec2*/ effectScale;
uniform vec4 effectAnyData;

uniform vec4/*vec2*/ sourceSizeInv;

void main()
{
	float vc = texture2D(s_outlineTexture, v_texCoord0).x;
	float v00 = texture2D(s_outlineTexture, v_texCoord0 + vec2(-1.0,0.0) * sourceSizeInv.xy).x;
	float v01 = texture2D(s_outlineTexture, v_texCoord0 + vec2(1.0,0.0) * sourceSizeInv.xy).x;
	float v10 = texture2D(s_outlineTexture, v_texCoord0 + vec2(0.0,-1.0) * sourceSizeInv.xy).x;
	float v11 = texture2D(s_outlineTexture, v_texCoord0 + vec2(0.0,1.0) * sourceSizeInv.xy).x;

	float w00 = texture2D(s_outlineTexture, v_texCoord0 + vec2(-1.0,-1.0) * sourceSizeInv.xy).x;
	float w01 = texture2D(s_outlineTexture, v_texCoord0 + vec2(1.0,-1.0) * sourceSizeInv.xy).x;
	float w10 = texture2D(s_outlineTexture, v_texCoord0 + vec2(1.0,1.0) * sourceSizeInv.xy).x;
	float w11 = texture2D(s_outlineTexture, v_texCoord0 + vec2(-1.0,1.0) * sourceSizeInv.xy).x;

	float q00 = texture2D(s_outlineTexture, v_texCoord0 + vec2(-1.0,0.0) * sourceSizeInv.xy * 2.0).x;
	float q01 = texture2D(s_outlineTexture, v_texCoord0 + vec2(1.0,0.0) * sourceSizeInv.xy * 2.0).x;
	float q10 = texture2D(s_outlineTexture, v_texCoord0 + vec2(0.0,-1.0) * sourceSizeInv.xy * 2.0).x;
	float q11 = texture2D(s_outlineTexture, v_texCoord0 + vec2(0.0,1.0) * sourceSizeInv.xy * 2.0).x;
	
	//float v = (vc + v00 + v01 + v10 + v11 + w00 + w01 + w10 + w11 + q00 + q01 + q10 + q11) / 13.0;
	//float v = (vc + v00 + v01 + v10 + v11 + w00 + w01 + w10 + w11) / 9.0;
	float v = (vc + v00 + v01 + v10 + v11) * 0.2;
	//float v = (v00 + v01 + v10 + v11) * 0.25;
	//float v = vc;
	
	vec4 color = effectColor;
	color.a *= v * intensity.x;
	
	gl_FragColor = color;
}