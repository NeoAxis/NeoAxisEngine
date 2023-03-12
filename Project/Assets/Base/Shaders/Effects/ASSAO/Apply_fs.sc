$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "ASSAO_Helpers.sh"

uniform vec4 invSharpness;
uniform vec4 halfViewportPixelSize;

SAMPLER2DARRAY(s_depthHalfTextureArray,  0);
SAMPLER2DARRAY(s_depthHalfTextureArrayL, 1);

void main()
{
	float ao;
    	ivec2 pixPos = (ivec2)getFragCoord().xy;
    	ivec2 pixPosHalf = pixPos / ivec2(2, 2);

	// calculate index in the four deinterleaved source array texture
    	int mx = (pixPos.x % 2);
    	int my = (pixPos.y % 2);
    	int ic = mx + my * 2;             // center index
    	int ih = (1 - mx) + my * 2;       // neighbouring, horizontal
    	int iv = mx + (1 - my) * 2;       // neighbouring, vertical
    	int id = (1 - mx) + (1 - my) * 2; // diagonal

	vec2 uv0 = (vec2)pixPosHalf * halfViewportPixelSize.xy;

	vec2 centerVal = texture2DArray(s_depthHalfTextureArray, vec3(uv0, (float)ic));
    
	ao = centerVal.x;

	vec4 edgesLRTB = UnpackEdges(centerVal.y, invSharpness.x);
	//vec4 ec = 1.0 - vec4( edgesLRTB.x, edgesLRTB.y * 0.5 + edgesLRTB.w * 0.5, edgesLRTB.z, 0.0 ); // DBG : for view edges

	// convert index shifts to sampling offsets
	float fmx = (float)mx;
	float fmy = (float)my;
    
	// in case of an edge, push sampling offsets away from the edge (towards pixel center)
	float fmxe  = (edgesLRTB.y - edgesLRTB.x);
	float fmye  = (edgesLRTB.w - edgesLRTB.z);

	// calculate final sampling offsets and sample using bilinear filter

	vec2  uvH = (getFragCoord().xy + vec2(fmx + fmxe - 0.5, 0.5 - fmy)) * 0.5 * halfViewportPixelSize;
	float aoH = texture2DArray(s_depthHalfTextureArrayL, vec3(uvH, (float)ih)).x;

	vec2  uvV = (getFragCoord().xy + vec2(0.5 - fmx, fmy - 0.5 + fmye)) * 0.5 * halfViewportPixelSize;
	float aoV = texture2DArray(s_depthHalfTextureArrayL, vec3(uvV, (float)iv)).x;

	vec2  uvD = (getFragCoord().xy + vec2(fmx - 0.5 + fmxe, fmy - 0.5 + fmye)) * 0.5 * halfViewportPixelSize;
	float aoD = texture2DArray(s_depthHalfTextureArrayL, vec3(uvD, (float)id)).x;

	// reduce weight for samples near edge - if the edge is on both sides, weight goes to 0
	vec4 blendWeights;
	blendWeights.x = 1.0;
	blendWeights.y = (edgesLRTB.x + edgesLRTB.y) * 0.5;
	blendWeights.z = (edgesLRTB.z + edgesLRTB.w) * 0.5;
	blendWeights.w = (blendWeights.y + blendWeights.z) * 0.5;

	// calculate weighted average
	float blendWeightsSum = dot(blendWeights, vec4(1.0, 1.0, 1.0, 1.0));
	ao = dot(vec4(ao, aoH, aoV, aoD), blendWeights) / blendWeightsSum;

	gl_FragColor = vec4(ao, ao, ao, 1.0);
}