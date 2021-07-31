$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica. Licensed under the NeoAxis License.
#include "../Common.sh"
#include "../FragmentFunctions.sh"

SAMPLER2D(s_sourceTexture, 0);
SAMPLER2D(s_depthTexture, 1);
SAMPLER2D(s_normalTexture, 2);
uniform vec4/*float*/ intensity;
uniform vec4/*float*/ depthThreshold;
uniform vec4/*float*/ normalsThreshold;
uniform vec4 edgeColor;
uniform vec4/*float*/ thickness;
uniform vec4/*float*/ maxDistance;

void getPixelData(vec2 uv, out float depth, out vec3 normal)
{
	float rawDepth = texture2D(s_depthTexture, uv).x;
	depth = getDepthValue(rawDepth, u_viewportOwnerNearClipDistance, u_viewportOwnerFarClipDistance);
	normal = normalize(texture2D(s_normalTexture, uv).rgb * 2.0 - 1.0);
}

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);

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
	
	vec4 color;
	color.rgb = lerp(sourceColor.rgb, edgeColor.rgb, edgeColor.a * coef * coefMaxDistance);
	color.a = sourceColor.a;
	
	gl_FragColor = lerp(sourceColor, color, intensity.x);
}

/*
	//float minLength = 100.0;

	//float len = length(vec2(x,y));
	//if(len < minLength)
	//	minLength = len;

	if(minLength < 100.0)
	{
		float coef = saturate(2.0 - minLength);
		
		color.rgb = lerp(sourceColor.rgb, edgeColor.rgb, edgeColor.a * coef);
	}
*/
	
	
/*
	float minDepth = 1000000.0;
	float maxDepth = 0.0;
	
	for(int y=-2;y<=2;y++)
	{
		for(int x=-2;x<=2;x++)
		{
			vec2 uv = v_texCoord0 + vec2(x,y) * u_viewportSizeInv * (thickness.x / 5.0);
			
			float depth;
			vec3 normal;
			getPixelData(uv, depth, normal);
			
			if(depth < minDepth)
				minDepth = depth;
			if(depth > maxDepth)
				maxDepth = depth;
		}
	}
	
	vec4 color = sourceColor;
	
	if(maxDepth - minDepth > depthThreshold.x)
	{
		color.rgb = lerp(sourceColor.rgb, edgeColor.rgb, edgeColor.a);
	}
*/
