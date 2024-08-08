// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

//Vehicle specific. calculate emissive for lamps. the mask format is 2x1 (left, right).
vec3 vehicleLamps( vec3 mask, vec3 emission, vec4 instanceParameter1, vec4 instanceParameter2, vec2 texCoord0 )
{
#ifdef GLSL
	//!!!!
	return vec3(0,0,0);
#else	

	//extract values from object instance parameters. the parameters are described in Vehicle.cs, OnGetRenderSceneDataAddToFrameData method
	//use instanceParameter2 to add more parameters
	vec2 param1x = unpackHalf2x16(asuint(instanceParameter1.x));
	vec2 param1y = unpackHalf2x16(asuint(instanceParameter1.y));
	vec2 param1z = unpackHalf2x16(asuint(instanceParameter1.z));
	vec2 param1w = unpackHalf2x16(asuint(instanceParameter1.w));	

	int maskIndex = int( round( dot( mask.rgb, vec3( 4, 2, 1 ) ) ) );
	bool isLeftPart = texCoord0.x < 0.5;
	
	float headlightLow = param1x.x;
	float headlightHigh = param1x.y;
	float rearFactor = param1y.x;
	float leftTurnSignal = param1y.y;
	float rightTurnSignal = param1z.x;
	//!!!!impl all

	
	float factor = 0.0;

	switch( maskIndex )
	{
	// (1 0 0) red
	case 4: factor = headlightLow; break;

	// (0 1 0) green
	case 2: factor = headlightHigh; break;
	
	// (0 0 1) blue
	case 1: factor = rearFactor; break;		

	// (1 1 0) yellow
	case 6: factor = isLeftPart ? leftTurnSignal : rightTurnSignal; break;	
	}
	
	
	//debug mask
	//return mask.rgb;
	
	return emission.rgb * factor;
	//return emission.rgb * emission.w * factor;
	
#endif
}
