
//https://www.gamedev.net/articles/programming/graphics/a-closer-look-at-parallax-occlusion-mapping-r3262/

vec2 getParallaxOcclusionMappingOffset(vec2 texCoord, vec3 eye, vec3 normal, float materialDisplacementScale, int maxSteps)
{
	float heightScale;// = 0.05;
	{
		vec2 texCoord0 = vec2_splat(0);//dummy
		float displacement = 0.0;//dummy
		float displacementScale = materialDisplacementScale;//u_materialDisplacementScale;
		vec4 customParameter1 = u_materialCustomParameters[0];
		vec4 customParameter2 = u_materialCustomParameters[1];
		vec4 instanceParameter1 = u_objectInstanceParameters[0];
		vec4 instanceParameter2 = u_objectInstanceParameters[1];
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DBias(makeSampler(s_linearSamplerFragment, _sampler), _uv, u_mipBias)
		DISPLACEMENT_CODE_BODY
		#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
		#undef CODE_BODY_TEXTURE2D
		heightScale = displacementScale;
	}	
	
	// Compute initial parallax displacement direction:
	vec2 parallaxDirection = normalize( eye.xy );

	// The length of this vector determines the furthest amount of displacement:
	float fLength = length( eye );
	float parallaxLength = sqrt( fLength * fLength - eye.z * eye.z ) / eye.z;

	// Compute the actual reverse parallax displacement vector:
	vec2 parallaxOffsetTS = parallaxDirection * parallaxLength;

	// Need to scale the amount of displacement to account for different height ranges
	// in height maps. This is controlled by an artist-editable parameter:
	parallaxOffsetTS *= heightScale;

	// Adaptive in-shader level-of-detail system implementation. Compute the 
	// current mip level explicitly in the pixel shader and use this information 
	// to transition between different levels of detail from the full effect to 
	// simple bump mapping. See the above paper for more discussion of the approach
	// and its benefits.

	// Compute the current gradients:
	//vec2 textureDims = vec2(1,1);
	//vec2 textureDims = vec2(4096.0,4096.0);
	
	//vec2 texCoordsPerSize = texCoord * textureDims;

	vec2 dx = dFdx( texCoord );
	vec2 dy = dFdy( -texCoord );
	//vec2 dx = ddx( texCoord );
	//vec2 dy = ddy( texCoord );
	
	// Compute all 4 derivatives in x and y in a single instruction to optimize:
	/*
	vec4 dx4 = ddx( vec4( texCoordsPerSize, texCoord ) );
	vec2 dxSize = dx4.xy;
	vec2 dx = dx4.zw;
	vec4 dy4 = ddy( vec4( texCoordsPerSize, texCoord ) );
	vec2 dySize = dy4.xy;
	vec2 dy = dy4.zw;
	*/
	
	/*
	vec2 dxSize, dySize;
	vec2 dx, dy;
	vec4( dxSize, dx ) = ddx( vec4( texCoordsPerSize, texCoord ) );
	vec4( dySize, dy ) = ddy( vec4( texCoordsPerSize, texCoord ) );

	//float mipLevelInt;    // mip level integer portion
	//float mipLevelFrac;   // mip level fractional amount for blending in between levels

	// Find min of change in u and v across quad: compute du and dv magnitude across quad
	vec2 texCoords = dxSize * dxSize + dySize * dySize;

	// Standard mipmapping uses max here
	float minTexCoordDelta = max( texCoords.x, texCoords.y );

	// Compute the current mip level  (* 0.5 is effectively computing a square root before )
	float mipLevel = max( 0.5 * log2( minTexCoordDelta ), 0 );

	//// Start the current sample located at the input texture coordinate, which would correspond
	//// to computing a bump mapping result:
	//vec2 texSample = texCoord;
	*/

	//float lodThreshold = 4000;
	//if ( mipLevel <= (float)lodThreshold )
	//{
		
	// Utilize dynamic flow control to change the number of samples per ray 
	// depending on the viewing angle for the surface. Oblique angles require 
	// smaller step sizes to achieve more accurate precision for computing displacement.
	// We express the sampling rate as a linear function of the angle between 
	// the geometric normal and the view direction ray:

#ifdef GLSL
	int numSteps = min(DISPLACEMENT_MAX_STEPS, maxSteps);
#else
	const int numSteps = min(DISPLACEMENT_MAX_STEPS, maxSteps);
#endif
	//const int maxSamples = 32;
	//const int minSamples = 10;
	//int numSteps = int(lerp( maxSamples, minSamples, dot( eye, normal ) ));

	// Intersect the view ray with the height field profile along the direction of
	// the parallax offset ray (computed in the vertex shader. Note that the code is
	// designed specifically to take advantage of the dynamic flow control constructs
	// in HLSL and is very sensitive to specific syntax. When converting to other examples,
	// if still want to use dynamic flow control in the resulting assembly shader,
	// care must be applied.
	// 
	// In the below steps we approximate the height field profile as piecewise linear
	// curve. We find the pair of endpoints between which the intersection between the 
	// height field profile and the view ray is found and then compute line segment
	// intersection for the view ray and the line segment formed by the two endpoints.
	// This intersection is the displacement offset from the original texture coordinate.
	// See the above paper for more details about the process and derivation.
	//

	float stepSize = 1.0 / float(numSteps);
	float prevHeight = 1.0;

	vec2 texOffsetPerStep = stepSize * parallaxOffsetTS;
	vec2 texCurrentOffset = texCoord;
	float currentBound = 1.0;

	vec2 pt1 = vec2_splat(0);
	vec2 pt2 = vec2_splat(0);

	LOOP
	for(int nIteration = 0; nIteration < DISPLACEMENT_MAX_STEPS; nIteration++)//for(int nIteration = 0; nIteration < numSteps; nIteration++)
	{
		if(nIteration >= maxSteps)
			break;
		
		texCurrentOffset -= texOffsetPerStep;

		vec2 texCoord0 = texCurrentOffset;
		float displacement = 0.0;
		float displacementScale = 0.0;//dummy
		vec4 customParameter1 = u_materialCustomParameters[0];
		vec4 customParameter2 = u_materialCustomParameters[1];
		vec4 instanceParameter1 = u_objectInstanceParameters[0];
		vec4 instanceParameter2 = u_objectInstanceParameters[1];
		#define CODE_BODY_TEXTURE2D_REMOVE_TILING(_sampler, _uv) texture2DGrad(makeSampler(s_linearSamplerFragment, _sampler), _uv, dx, dy)
		#define CODE_BODY_TEXTURE2D(_sampler, _uv) texture2DGrad(makeSampler(s_linearSamplerFragment, _sampler), _uv, dx, dy)
		DISPLACEMENT_CODE_BODY
		#undef CODE_BODY_TEXTURE2D_REMOVE_TILING
		#undef CODE_BODY_TEXTURE2D
		float currHeight = displacement;
		
		currentBound -= stepSize;

		if ( currHeight > currentBound ) 
		{
			pt1 = vec2( currentBound, currHeight );
			pt2 = vec2( currentBound + stepSize, prevHeight );
			break;
		}
		else
			prevHeight = currHeight;
	}   

	float delta2 = pt2.x - pt2.y;
	float delta1 = pt1.x - pt1.y;
	float denominator = delta2 - delta1;

	float parallaxAmount;
	if ( denominator == 0.0f )
		parallaxAmount = 0.0f;
	else
		parallaxAmount = (pt1.x * delta2 - pt2.x * delta1 ) / denominator;

	vec2 parallaxOffset = parallaxOffsetTS * ( 1.0 - parallaxAmount );
	
	return parallaxOffset;
		
	//}
	//return vec2(0,0);
}
