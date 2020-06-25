$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ viewportSize;
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ sharpStrength;
uniform vec4/*float*/ sharpClamp;
uniform vec4/*float*/ offsetBias;

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

	vec3 coef_luma = vec3(0.2126, 0.7152, 0.0722);      // BT.709 & sRBG luma coefficient (Monitors and HD Television)
	//vec3 coef_luma = vec3(0.299, 0.587, 0.114);       // BT.601 luma coefficient (SD Television)
	//vec3 coef_luma = vec3(1.0/3.0, 1.0/3.0, 1.0/3.0); // Equal weight coefficient

	vec3 sharp_strength_luma = coef_luma * sharpStrength.x;

	// -- Gaussian filter --
	//   [ .25, .50, .25]     [ 1 , 2 , 1 ]
	//   [ .50,   1, .50]  =  [ 2 , 4 , 2 ]
 	//   [ .25, .50, .25]     [ 1 , 2 , 1 ]

	float px = viewportSize.z;
	float py = viewportSize.w;

	vec3 blur_ori = texture2D(s_sourceTexture, v_texCoord0 + vec2(px,-py) * 0.5 * offsetBias.x).rgb; // South East
	blur_ori += texture2D(s_sourceTexture, v_texCoord0 + vec2(-px,-py) * 0.5 * offsetBias.x).rgb;  // South West    
	blur_ori += texture2D(s_sourceTexture, v_texCoord0 + vec2(px,py) * 0.5 * offsetBias.x).rgb; // North East
	blur_ori += texture2D(s_sourceTexture, v_texCoord0 + vec2(-px,py) * 0.5 * offsetBias.x).rgb; // North West
	blur_ori *= 0.25;  // ( /= 4) Divide by the number of texture fetches


	// -- Calculate the sharpening --  
	vec3 sharp = sourceColor.rgb - blur_ori;  //Subtracting the blurred image from the original image

	// -- Adjust strength of the sharpening --
	float sharp_luma = dot(sharp, sharp_strength_luma); //Calculate the luma and adjust the strength

	// -- Clamping the maximum amount of sharpening to prevent halo artifacts --
	sharp_luma = clamp(sharp_luma, -sharpClamp.x, sharpClamp.x);  //TODO Try a curve function instead of a clamp

	// -- Combining the values to get the final sharpened pixel	--
	vec4 done = vec4(sourceColor.rgb + sharp_luma, sourceColor.w);    // Add the sharpening to the original.
	//vec4 done = inputcolor + sharp_luma;    // Add the sharpening to the input color.


	//#if show_sharpen == 1
	//  //vec3 chroma = ori - luma;
	//  //done = abs(sharp * 4).rrr;
	//  done = saturate(0.5 + (sharp_luma * 4)).rrrr;
	//#endif

	
	gl_FragColor = lerp(sourceColor, done, intensity.x);
	//vec4 done2 = saturate(done);
	//return lerp(sourceColor, done2, intensity);
}
