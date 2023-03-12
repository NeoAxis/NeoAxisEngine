$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "../../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_blurTexture, 1);
SAMPLER2D(s_depthTexture, 2);
uniform vec4/*float*/ intensity;
uniform vec4 depthOfFieldProperties;

void main()
{
	float focalDistance = depthOfFieldProperties.x;
	float focalSize = depthOfFieldProperties.y;
	float backgroundTransitionLength = depthOfFieldProperties.z;
	float foregroundTransitionLength = depthOfFieldProperties.w;

	//get scene and blurred scene colors
	vec4 sceneColor = texture2D(s_sourceTexture, v_texCoord0);
	vec4 blurColor = texture2D(s_blurTexture, v_texCoord0);

	////calculate focal distance for auto focus mode
	//if(focalDistance < 0)
	//	focalDistance = tex2D(autoFocusCurrentTexture, float2(.5f, .5f)).r;

	float halfFocalSize = focalSize / 2.0;
	float startBackgroundDistance = focalDistance + halfFocalSize;
	float startForegroundDistance = focalDistance - halfFocalSize;

	//get depth
	float rawDepth = texture2D(s_depthTexture, v_texCoord0).r;
	float depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);

	//calculate blur factor
	float blurFactor;
	if(depth > focalDistance)
	{
		blurFactor = (depth - startBackgroundDistance) / backgroundTransitionLength;
		////don't blur the sky
		//if( normalizedDepth > .98f)
		//	blurFactor = 0;
	}
	else
	{
		if(foregroundTransitionLength >= 0.0)
			blurFactor = (startForegroundDistance - depth) / foregroundTransitionLength;
		else
			blurFactor = 0.0;
	}
	blurFactor = saturate(blurFactor);

	//calculate result color
	vec4 color = lerp(sceneColor, blurColor, blurFactor);

	gl_FragColor = lerp(sceneColor, color, intensity.x);
}
