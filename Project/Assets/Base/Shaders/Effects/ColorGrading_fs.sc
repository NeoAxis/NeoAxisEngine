$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_lookupTable, 1);
uniform vec4/*float*/ intensity;
uniform vec4/*float3*/ multiply;
uniform vec4/*float3*/ add;
uniform vec4/*float*/ useLookupTable;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec4 color = sourceColor;

	if(useLookupTable.x > 0.0)
	{
		color = saturate(color);

		vec2 offset = vec2( 0.5f / 256.0f, 0.5f / 16.0f );
		float scale = 15.0f / 16.0f;
		float intB = floor( color.b * 14.9999f ) / 16.0f;
		float fracB = color.b * 15.0f - intB * 16.0f;
		float u = intB + color.r * scale / 16.0f;
		float v = color.g * scale;
		vec3 rg0 = texture2DLod( s_lookupTable, offset + vec2( u, v ), 0.0 ).rgb;
		vec3 rg1 = texture2DLod( s_lookupTable, offset + vec2( u + 1.0f / 16.0f, v ), 0.0 ).rgb;
		vec3 color2 = lerp( rg0, rg1, fracB );
		color = vec4( color2, color.w );
	}

	vec4 value = vec4(color.rgb * multiply.xyz + add.xyz, color.a);

	gl_FragColor = lerp(sourceColor, value, intensity.x);
}
