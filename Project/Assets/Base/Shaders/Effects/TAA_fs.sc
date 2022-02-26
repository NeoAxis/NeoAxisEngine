$input v_texCoord0

// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
/***************************************************************************
 # Copyright (c) 2015-21, NVIDIA CORPORATION. All rights reserved.
 #
 # Redistribution and use in source and binary forms, with or without
 # modification, are permitted provided that the following conditions
 # are met:
 #  * Redistributions of source code must retain the above copyright
 #    notice, this list of conditions and the following disclaimer.
 #  * Redistributions in binary form must reproduce the above copyright
 #    notice, this list of conditions and the following disclaimer in the
 #    documentation and/or other materials provided with the distribution.
 #  * Neither the name of NVIDIA CORPORATION nor the names of its
 #    contributors may be used to endorse or promote products derived
 #    from this software without specific prior written permission.
 #
 # THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS "AS IS" AND ANY
 # EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 # IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 # PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 # CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 # EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 # PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 # PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 # OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 # (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 # OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 **************************************************************************/
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_motionVectors, 1);
SAMPLER2D(s_previousColor, 2);

uniform vec4 viewportSize;
uniform vec4/*float*/ intensity;
uniform vec4/*vec2*/ taaParameters;

// Converts color from RGB to YCgCo space. Param RGBColor linear HDR RGB color.
vec3 RGBToYCgCo(vec3 rgb)
{
	float Y = dot(rgb, vec3(0.25f, 0.50f, 0.25f));
	float Cg = dot(rgb, vec3(-0.25f, 0.50f, -0.25f));
	float Co = dot(rgb, vec3(0.50f, 0.00f, -0.50f));
	return vec3(Y, Cg, Co);
}

// Converts color from YCgCo to RGB space. Param YCgCoColor linear HDR YCgCo color.
vec3 YCgCoToRGB(vec3 YCgCo)
{
	float tmp = YCgCo.x - YCgCo.y;
	float r = tmp + YCgCo.z;
	float g = YCgCo.x + YCgCo.y;
	float b = tmp - YCgCo.z;
	return vec3(r, g, b);
}

// Catmull-Rom filtering code from http://vec3.ca/bicubic-filtering-in-fewer-taps/
vec3 bicubicSampleCatmullRom(sampler2D tex, vec2 samplePos)
{
	vec2 invTextureSize = viewportSize.zw;
	vec2 tc = floor(samplePos - 0.5f) + vec2_splat(0.5f);
	vec2 f = samplePos - tc;
	vec2 f2 = f * f;
	vec2 f3 = f2 * f;

	vec2 w0 = f2 - 0.5f * (f3 + f);
	vec2 w1 = 1.5f * f3 - 2.5f * f2 + vec2_splat(1.0f);
	vec2 w3 = 0.5f * (f3 - f2);
	vec2 w2 = vec2_splat(1.0f) - w0 - w1 - w3;

	vec2 w12 = w1 + w2;

	vec2 tc0 = (tc - vec2_splat(1.0f)) * invTextureSize;
	vec2 tc12 = (tc + w2 / w12) * invTextureSize;
	vec2 tc3 = (tc + vec2_splat(2.0f)) * invTextureSize;

	vec3 result =
		texture2D(tex, vec2(tc0.x,  tc0.y)).rgb  * (w0.x  * w0.y) +
		texture2D(tex, vec2(tc0.x,  tc12.y)).rgb * (w0.x  * w12.y) +
		texture2D(tex, vec2(tc0.x,  tc3.y)).rgb  * (w0.x  * w3.y) +
		texture2D(tex, vec2(tc12.x, tc0.y)).rgb  * (w12.x * w0.y) +
		texture2D(tex, vec2(tc12.x, tc12.y)).rgb * (w12.x * w12.y) +
		texture2D(tex, vec2(tc12.x, tc3.y)).rgb  * (w12.x * w3.y) +
		texture2D(tex, vec2(tc3.x,  tc0.y)).rgb  * (w3.x  * w0.y) +
		texture2D(tex, vec2(tc3.x,  tc12.y)).rgb * (w3.x  * w12.y) +
		texture2D(tex, vec2(tc3.x,  tc3.y)).rgb  * (w3.x  * w3.y);

	return result;
}

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

#ifdef GLSL
	const vec2 offset[8] = vec2[8] ( vec2(-1, -1), vec2(-1, 1), vec2(1, -1), vec2(1, 1), vec2(1, 0), vec2( 0, -1),vec2(0, 1), vec2(-1, 0) );
#else
	const vec2 offset[8] = { vec2(-1, -1), vec2(-1, 1), vec2(1, -1), vec2(1, 1), vec2(1, 0), vec2( 0, -1),vec2(0, 1), vec2(-1, 0) };
#endif

	float gAlpha = taaParameters.x;
	float gColorBoxSigma = taaParameters.y;
	
	//vec2 pos = texC * texDim;
	//ivec2 ipos = ivec2(pos);

	// Fetch the current pixel color and compute the color bounding box
	// Details here: http://www.gdcvault.com/play/1023521/From-the-Lab-Bench-Real
	// and here: http://cwyman.org/papers/siga16_gazeTrackedFoveatedRendering.pdf
	vec3 color = texture2D(s_sourceTexture, v_texCoord0).rgb;
	//vec3 color = gTexColor.Load(int3(ipos, 0)).rgb;
	color = RGBToYCgCo(color);
	vec3 colorAvg = color;
	vec3 colorVar = color * color;
	UNROLL
	for (int k = 0; k < 8; k++)
	{
		vec3 c = texture2D(s_sourceTexture, v_texCoord0 + offset[k] * viewportSize.zw).rgb;
		//vec3 c = gTexColor.Load(int3(ipos + offset[k], 0)).rgb;
		c = RGBToYCgCo(c);
		colorAvg += c;
		colorVar += c * c;
	}

	float oneOverNine = 1.0f / 9.0f;
	colorAvg *= oneOverNine;
	colorVar *= oneOverNine;

	vec3 sigma = sqrt(max(0.0f, colorVar - colorAvg * colorAvg));
	vec3 colorMin = colorAvg - gColorBoxSigma * sigma;
	vec3 colorMax = colorAvg + gColorBoxSigma * sigma;

	// Find the longest motion vector
	vec2 motion = texture2D(s_motionVectors, v_texCoord0).xy * vec2(-1.0f, 1.0f);
	//vec2 motion = gTexMotionVec.Load(int3(ipos, 0)).xy;
	UNROLL
	for (int a = 0; a < 8; a++)
	{
		vec2 m = texture2D(s_motionVectors, v_texCoord0 + offset[a] * viewportSize.zw).xy * vec2(-1.0f, 1.0f);
		//vec2 m = gTexMotionVec.Load(int3(ipos + offset[a], 0)).rg;
		motion = dot(m, m) > dot(motion, motion) ? m : motion;
	}

	// Use motion vector to fetch previous frame color (history)
	vec3 history = bicubicSampleCatmullRom(s_previousColor, (v_texCoord0 + motion) * viewportSize.xy);

	history = RGBToYCgCo(history);

	// Anti-flickering, based on Brian Karis talk @Siggraph 2014
	// https://de45xmedrsdbp.cloudfront.net/Resources/files/TemporalAA_small-59732822.pdf
	// Reduce blend factor when history is near clamping
	float distToClamp = min(abs(colorMin.x - history.x), abs(colorMax.x - history.x));
	float alpha = saturate((gAlpha * distToClamp) / (distToClamp + colorMax.x - colorMin.x));

	history = clamp(history, colorMin, colorMax);
	vec3 result = YCgCoToRGB(lerp(history, color, alpha));
	vec4 resultColor = vec4(result, 1.0f);
	
	gl_FragColor = lerp(sourceColor, resultColor, intensity.x);
}
