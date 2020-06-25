$input v_texCoord0

#include "../../Common.sh"

uniform vec4 depthUnpackConsts;
uniform vec4 viewportPixelSize;

SAMPLER2D(s_depthTexture, 0);

float screenSpaceToViewSpaceDepth(float screenDepth)
{
    float depthLinearizeMul = depthUnpackConsts.x;
    float depthLinearizeAdd = depthUnpackConsts.y;

    return depthLinearizeMul / (depthLinearizeAdd - screenDepth);
}

void main()
{
	float depth_0 = screenSpaceToViewSpaceDepth(texture2D(s_depthTexture, v_texCoord0).r);
	float depth_1 = screenSpaceToViewSpaceDepth(texture2D(s_depthTexture, v_texCoord0 + vec2(viewportPixelSize.x, 0.0)).r);
	float depth_2 = screenSpaceToViewSpaceDepth(texture2D(s_depthTexture, v_texCoord0 + vec2(0.0, viewportPixelSize.y)).r);
	float depth_3 = screenSpaceToViewSpaceDepth(texture2D(s_depthTexture, v_texCoord0 + vec2(viewportPixelSize.x, viewportPixelSize.y)).r);

	gl_FragData[0] = vec4(depth_0, 0.0, 0.0, 0.0);
	gl_FragData[1] = vec4(depth_1, 0.0, 0.0, 0.0);
	gl_FragData[2] = vec4(depth_2, 0.0, 0.0, 0.0);
	gl_FragData[3] = vec4(depth_3, 0.0, 0.0, 0.0);
}