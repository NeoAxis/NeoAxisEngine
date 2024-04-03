$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "../../FragmentFunctions.sh"

SAMPLER2D( s_reflectionTexture, 0 );
uniform vec4 u_ssrFillGapsParameters;

void main()
{
	vec4 value = texture2DLod( s_reflectionTexture, v_texCoord0, 0 );

	BRANCH
	if( value.w == 0.0 )
	{
		vec2 scale = u_ssrFillGapsParameters.xy;

		const int sampleCount = 32; //44;//64;
		
		value.rgb = vec3_splat( 0 );
		int foundCount = 0;
		
		UNROLL //LOOP
		for( int n = 0; n < sampleCount; n++ )
		{
			vec2 texCoord = v_texCoord0 + vogelDiskSample( n, sampleCount, 0.0 ) * scale;
			
			vec4 value2 = texture2DLod( s_reflectionTexture, texCoord, 0 );
			
			if( value2.w != 0.0 )
			{
				value.rgb += value2.rgb;
				foundCount++;
			}
			
			/*
			if( value2.w != 0.0 )
			{
				value.rgb = value2.rgb;
				break;
			}*/
		}
		
		value.rgb /= float( foundCount );
		//value.rgb /= float( sampleCount );		
	}
	
	gl_FragColor = value;
}
