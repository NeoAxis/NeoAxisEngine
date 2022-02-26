$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

/* Lanczos-2 image upscaler
*
* This takes a reduced resolution source, and scales it up while preserving detail.
* It is very close to an ideal Lanczos-2 filter, but 5 texture fetches instead of 16.
*
* Algorithm based on Jorge Jimenez's SIGGRAPH 2016 paper
* [Jimenez2016] Filmic SMAA: Sharp Morphological and Temporal
Antialiasing
* https://research.activision.com/publications/archives/filmic-smaasharp-morphological-and-temporal-antialiasing
*
* [Djovnov2012] Bicubic Filtering in Fewer Taps
* https://vec3.ca/bicubic-filtering-in-fewer-taps
*
* I've rolled by own tap coefficients, as seem to be much more accurate to Lanczos-2
* than the references above.
*/

SAMPLER2D(s_sourceTexture, 0);
uniform vec4 viewportSize;
uniform vec4 inputSize;

void main()
{
	// Source position in fractions of a texel
	vec2 src_pos = inputSize.xy * v_texCoord0;
	// Source bottom left texel centre
	vec2 src_centre = floor(src_pos - 0.5) + 0.5;
	// f is position. f.x runs left to right, y bottom to top, z right to left, w top to bottom
	vec4 f;
	f.xy = src_pos - src_centre;
	f.zw = 1.0 - f.xy;
	// Calculate weights in x and y in parallel.
	// These polynomials are piecewise approximation of Lanczos kernel
	// Calculator here: https://gist.github.com/going-digital/752271db735a07da7617079482394543
	vec4 l2_w0_o3 = ((1.5672 * f - 2.6445) * f + 0.0837) * f + 0.9976;
	vec4 l2_w1_o3 = ((-0.7389 * f + 1.3652) * f - 0.6295) * f - 0.0004;

	vec4 w1_2 = l2_w0_o3;
	vec2 w12 = w1_2.xy + w1_2.zw;
	vec4 wedge = l2_w1_o3.xyzw * w12.yxyx;
	// Calculate texture read positions. tc12 uses bilinear interpolation to do 4 reads in 1.
	vec2 tc12 = inputSize.zw * (src_centre + w1_2.zw / w12);
	vec2 tc0 = inputSize.zw * (src_centre - 1.0);
	vec2 tc3 = inputSize.zw * (src_centre + 2.0);
	// Sharpening adjustment
	vec3 col = vec3(
		texture2D(s_sourceTexture, vec2(tc12.x, tc0.y)).rgb * wedge.y +
		texture2D(s_sourceTexture, vec2(tc0.x, tc12.y)).rgb * wedge.x +
		texture2D(s_sourceTexture, tc12.xy).rgb * (w12.x * w12.y) +
		texture2D(s_sourceTexture, vec2(tc3.x, tc12.y)).rgb * wedge.z +
		texture2D(s_sourceTexture, vec2(tc12.x, tc3.y)).rgb * wedge.w
	);

	gl_FragColor = vec4(col,1.0);
}
