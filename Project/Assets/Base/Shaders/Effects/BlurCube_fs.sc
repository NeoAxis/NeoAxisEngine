$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"
#include "../UniformsFragment.sh"
#include "../FragmentFunctions.sh"

#ifndef SAMPLE_COUNT
#define SAMPLE_COUNT 15
#endif

SAMPLERCUBE( s_sourceTexture, 0 );

uniform vec4/*vec2*/ u_blurSampleOffsets[15];//SAMPLE_COUNT];
uniform vec4 u_blurSampleWeights[15];//SAMPLE_COUNT];
uniform vec4 u_blurCubemapParameters;

#ifdef BLEND_WITH_TEXTURE
	SAMPLER2D(s_blendWithTexture, 1);
	uniform vec4/*float*/ intensity;
#endif

void main()
{
	int face = int( u_blurCubemapParameters.x );
	float sourceMip = u_blurCubemapParameters.y;

	vec2 texCoord = v_texCoord0 * 2.0 - 1.0;

	vec3 coords = vec3_splat( 0 );
	switch( face )
	{
		case 0: coords = vec3( 1, -texCoord.y, -texCoord.x ); break;
		case 1: coords = vec3( -1, -texCoord.y, texCoord.x ); break;
		case 2: coords = vec3( texCoord.x, 1, texCoord.y ); break;
		case 3: coords = vec3( texCoord.x, -1, -texCoord.y ); break;
		case 4: coords = vec3( texCoord.x, -texCoord.y, 1 ); break;
		case 5: coords = vec3( -texCoord.x, -texCoord.y, -1 ); break;
	}

	vec2 spherical = sphericalDirectionFromVector( coords );
	
	vec4 color = vec4_splat( 0 );

	for( int n = 0; n < SAMPLE_COUNT; n++ )
	{
		vec2 spherical2 = spherical + u_blurSampleOffsets[ n ].xy;	
		vec3 vector2 = sphericalDirectionGetVector( spherical2 );
		color += u_blurSampleWeights[ n ] * textureCubeLod( s_sourceTexture, vector2, sourceMip );
	}
	
#ifdef BLEND_WITH_TEXTURE
	vec4 v1 = texture2D(s_blendWithTexture, v_texCoord0);
	gl_FragColor = lerp(v1, color, intensity.x);
#else	
	gl_FragColor = color;
#endif
}
