$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "Common.sh"
//!!!!add compute to Common.sh?
#include "Common/bgfx_compute.sh"

//SAMPLER2D(s_colorTexture, 0);
//SAMPLER2D(s_secondTexture, 1);

SAMPLER2D( s_oitScreen, 0 );
//USAMPLER2D( s_oitScreen, 0 );
//SAMPLER2D( s_oitLists, 1 );

//IMAGE2D_RW( s_oitScreen, rgba32f, 0 );
//IMAGE2D_RO( s_oitScreen, rgba32f, 0 );
//UIMAGE2D_RO( s_oitScreen, r32ui, 0 );
//IMAGE2D_RO( s_oitLists, rgba32f, 1 );

void main()
{
	vec4 fragCoord = getFragCoord();
	
	//!!!!
	float v = texelFetch( s_oitScreen, ivec2( fragCoord.xy ), 0 ).x;
	//uint v = texelFetch( s_oitScreen, ivec2( fragCoord.xy ), 0 ).x;
	//uint v = texelFetch( s_oitScreen, ivec3( ivec2( fragCoord.xy ), 0 ), 0 ).x;
	//float v = imageLoad( s_oitScreen, ivec2( fragCoord.xy ) ).x;
	//uint v = imageLoad( s_oitScreen, ivec2( fragCoord.xy ) ).x;
	
	if( v != 0 )
		gl_FragColor = vec4( 0, 1, 0, 1 );
	else
		gl_FragColor = vec4( 1, 0, 0, 0.5 );
	
	
	//!!!!
	gl_FragColor = texelFetch( s_oitScreen, ivec2( fragCoord.xy ), 0 );
	
	

	
	/*
	vec4 accum = texture2D( s_colorTexture, v_texCoord0 );
	float opacity = texture2D( s_secondTexture, v_texCoord0 ).x;
	gl_FragColor = vec4( accum.xyz / clamp( accum.w, 1e-4, 5e4 ), opacity );
	*/
	
	
	/*
	vec4 accum = texture2D(s_colorTexture, v_texCoord0);
	float opacity = accum.w;
	float weight = texture2D(s_secondTexture, v_texCoord0).x;
	gl_FragColor = vec4(accum.xyz / clamp(weight, 1e-4, 5e4), opacity);
	*/
	
	//!!!!
	//gl_FragColor = vec4(accum.xyz, opacity);
	
	//!!!!
	//gl_FragColor = vec4(accum.xyz, 0.5);
	
	
	/*
	vec4 accum = texture2D(s_colorTexture, v_texCoord0);
	float reveal = texture2D(s_secondTexture, v_texCoord0).r;
	gl_FragColor = vec4(accum.rgb / max(accum.a, 0.00001), reveal);	
	*/
}
