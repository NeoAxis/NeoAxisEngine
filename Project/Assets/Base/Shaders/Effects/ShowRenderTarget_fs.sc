$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"
#include "../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_showTexture, 1);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ nearClipDistance;
uniform vec4/*float*/ farClipDistance;
uniform vec4/*int*/ mode;
uniform vec4/*float*/ depthMultiplier;
uniform vec4/*float*/ motionMultiplier;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	vec4 showColor = texture2D(s_showTexture, v_texCoord0);

	vec4 color;

	if(mode.x == 0.0)
	{
		//normal
		color = showColor;
	}
	else if(mode.x == 1.0)
	{
		//depth
		float depth = getDepthValue(showColor.r, nearClipDistance.x, farClipDistance.x);		
		float v = saturate(depth / farClipDistance.x * depthMultiplier.x);
		//float v = saturate(depth * depthMultiplier.x);
		color = vec4(v, v, v, 1.0);
	}
	else
	{
		//motion vector
		color = showColor * motionMultiplier.x;
	}

	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
