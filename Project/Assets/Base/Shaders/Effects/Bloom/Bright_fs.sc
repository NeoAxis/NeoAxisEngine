$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ brightThreshold;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec4 color = sourceColor;

	////downScale4x4
	//half4 sample = half4(0,0,0,0);
	//for(int n = 0; n < 16; n++)
	//	sample += tex2D(sourceTexture, uv + sampleOffsets[n]);
	//sample /= 16;

	////BrightPass filter
	//half adaptedLuminance = tex2D(luminanceTexture, half2(.5f, .5f)).r;

	//half middleGray = .5f;
	//// Determine what the pixel's value will be after tone mapping occurs
	//sample.rgb *= middleGray / (adaptedLuminance + .001f);
	
	// Subtract out dark pixels
	color.rgb -= brightThreshold.x;
	
	// Clamp to 0
	color = max(color, 0.0f);
	
	// Map the resulting value into the 0 to 1 range. Higher values for
	// brightOffset will isolate lights from illuminated scene 
	// objects.
	const float brightOffset = 1.0f; // Offset for BrightPass filter
	color.rgb /= (brightOffset + color.rgb);

	gl_FragColor = color;
}
