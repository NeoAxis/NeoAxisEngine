$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"
#include "../../FragmentFunctions.sh"

SAMPLER2D(s_maskTexture, 0);

uniform vec4/*float*/ intensity;
uniform vec4 effectColor;
uniform vec4/*vec2*/ effectScale;
uniform vec4 effectAnyData;

float random2( vec2 p )
{
	vec2 k1 = vec2(
		23.14069263277926, // e^pi (Gelfond's constant)
		2.665144142690225 // 2^sqrt(2) (Gelfonda€“Schneider constant)
	);
	return frac( cos( dot( p, k1 ) ) * 12345.6789 );
}

void main()
{
	vec4 mask = texture2D(s_maskTexture, v_texCoord0);

	float minScale = 1.0;

	float rand = random2(v_texCoord0);
	
	int radiusSteps = 10;//8;//10;
	for(int nRadius = 1; nRadius <= radiusSteps; nRadius++)
	{
		float scl = float(nRadius) / float(radiusSteps);

		int sideCount = 8;//16;
		for(int nSide = 0; nSide < sideCount; nSide++)
		{
			float sideFactor = float(nSide) / float(sideCount - 1);
			
			float angle = PI * 2.0 * sideFactor + rand * 10.0;// + scl * 10.0;
			vec2 offset = vec2(cos(angle), sin(angle));
			
			vec2 p = v_texCoord0 + offset * scl * effectScale.xy;
			
			vec4 mask2 = texture2D(s_maskTexture, p);
			
			if(any(mask2.rgb))
			{
				minScale = scl;
				break;
			}
		}
		
		if(minScale != 1.0)
			break;		
	}

	bool insideModel = any(mask.rgb);
	if(insideModel)
		minScale = 0.0;//discard;

	float v = minScale * 2.0 - 1.0;
	float v2 = 1.0 - v * v;
	v2 = saturate(v2);
	
	gl_FragColor = vec4_splat(v2);
}



//SAMPLER2D(s_depthTexture, 1);
//SAMPLER2D(s_normalTexture, 2);
	
/*
void getPixelData(vec2 uv, out float depth, out vec3 normal)
{
	float rawDepth = texture2D(s_depthTexture, uv).x;
	depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
	normal = normalize(texture2D(s_normalTexture, uv).rgb * 2.0 - 1.0);
}
*/	

	/*
	float depthCenter;
	vec3 normalCenter;
	getPixelData(v_texCoord0, depthCenter, normalCenter);
	
	int totalCount = 0;
	int count = 0;

	for(int y=-2;y<=2;y++)
	{
		for(int x=-2;x<=2;x++)
		{
			totalCount++;
			
			if(x != 0 && y != 0 && abs(x) + abs(y) < 4)
			{	
				vec2 uv = v_texCoord0 + vec2(x,y) * u_viewportSizeInv * (thickness.x / 2.0);
				
				float depth;
				vec3 normal;
				getPixelData(uv, depth, normal);
				
				float depthDiff = abs(depth - depthCenter);
				float angle = acos(dot(normalCenter, normal));
				
				if(depthDiff > depthThreshold.x || angle > normalsThreshold.x * PI)
					count++;				
			}
		}
	}

	float coef = saturate(float(count) / float(totalCount) * 4.0);
	
	float coefMaxDistance = 0.0;
	float maxDistanceFar = maxDistance.x;
	float maxDistanceNear = maxDistanceFar * 0.9;
	if(depthCenter < maxDistanceFar)
	{
		if(depthCenter >= maxDistanceNear)
			coefMaxDistance = saturate(1.0 - (depthCenter - maxDistanceNear)/(maxDistanceFar - maxDistanceNear));
		else
			coefMaxDistance = 1.0;			
	}
	*/
