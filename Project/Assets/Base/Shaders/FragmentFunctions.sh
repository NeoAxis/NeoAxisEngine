// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

void smoothLOD(vec4 fragCoord, float lodValue)
{
	BRANCH
	if(lodValue != 0)
	{
		int x = (int)fragCoord.x;
		int y = (int)fragCoord.y;
		int xx = x % 4;
		int yy = y % 4;
		int v = xx * 4 + yy;
		const float thresholds[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
		float threshold = thresholds[v];
		if(lodValue > 0)
			clip(threshold - lodValue);
		else
			clip(-threshold - lodValue);
	}
}

float dither(vec4 fragCoord, float value)
{
	BRANCH
	if(value > 0.02 && value < 0.98)
	{
		int x = (int)fragCoord.x;
		int y = (int)fragCoord.y;
		int xx = x % 4;
		int yy = y % 4;
		int v = xx * 4 + yy;
		const float array[16] = {0.5625, 0.3125, 0.99, 0.25, 0.1875, 0.8125, 0.4375, 0.9375, 0.75, 0.0625, 0.375, 0.5, 0.6875, 0.875, 0.125, 0.625};
		float vv = array[v];
		value -= vv * 1 - 0.5;
		//value -= vv * 0.5 - 0.25;
		//value -= vv * 0.25 - 0.125;
		value = saturate(value);
	}	
	return value;
}
