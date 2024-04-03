$input v_texCoord0

// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
// Copyright: LGPL-3.0 license. https://github.com/jagracar/webgl-shader-examples/blob/master/shaders/frag-rain.glsl
#include "../Common.sh"

SAMPLER2D(s_sourceTexture, 0);
uniform vec4/*float*/ intensity;


float random1(float dt)
{
    highp float c = 43758.5453;
    highp float sn = mod(dt, 3.14);
    return fract(sin(sn) * c);
}

// Returns a random drop position for the given seed value
vec2 random_drop_pos(float val, vec2 screen_dim, vec2 velocity)
{
    float max_x_move = velocity.x * abs(screen_dim.y / velocity.y);
    float x = -max_x_move * step(0.0, max_x_move) + (screen_dim.x + abs(max_x_move)) * random1(val);
    float y = (1.0 + 0.05 * random1(1.234 * val)) * screen_dim.y;

    return vec2(x, y);
}

// Calculates the drop trail color at the given pixel position
vec3 trail_color(vec2 pixel, vec2 pos, vec2 velocity_dir, float width, float size)
{
    vec2 pixel_dir = pixel - pos;
    float projected_dist = dot(pixel_dir, -velocity_dir);
    float tanjential_dist_sq = dot(pixel_dir, pixel_dir) - pow(projected_dist, 2.0);
    float width_sq = pow(width, 2.0);

    float line1 = step(0.0, projected_dist) * (1.0 - smoothstep(width_sq / 2.0, width_sq, tanjential_dist_sq));
    float dashed_line = line1 * step(0.5, cos(0.3 * projected_dist - PI / 3.0));
    float fading_dashed_line = dashed_line * (1.0 - smoothstep(size / 5.0, size, projected_dist));

    return vec3(fading_dashed_line,fading_dashed_line,fading_dashed_line);
}

vec4 mainproc(float time, vec2 gl_FragCoord)
{
	// Set the total number of rain drops that are visible at a given time
	float n_drops = 50.0 + abs(sin(PI - time/10)) * 100.0;
    //float n_drops = 50.0 + abs(sin(PI - time/10)) * 300.0;

    // Set the drop trail radius
    float trail_width = 0.0005 + abs(sin(PI - time/10))*0.001;

    // Set the drop trail size
    float trail_size = 0.1;

    // Set the drop wave size
    float wave_size = 20.0;

    // Set the drop fall time in seconds
    float fall_time = 0.2;

    // Set the drop total life time
    float life_time = fall_time + 0.5;
    
    vec2 resolution = vec2(1.0, 1.0);

    // Set the drop velocity in pixels per second
    vec2 velocity = vec2(0.05 * resolution.x, 1.0 * resolution.y) / fall_time;
    vec2 velocity_dir = normalize(velocity);

    // Iterate over the drops to calculate the pixel color
    vec3 pixel_color = vec3(0.0,0.0,0.0);

	LOOP
    for (float i = 0.0; i < n_drops; i++)
	{
        // Offset the running time for each drop
        float time1 = time + life_time * (i + i / n_drops);

        // Calculate the time since the drop appeared on the screen
        float ellapsed_time = mod(time1, life_time);

        // Calculate the drop initial position
        vec2 initial_pos = 1.0 - random_drop_pos(i + floor(time1 / life_time - i) * n_drops, resolution, velocity);

        // Add the drop to the pixel color
        if (ellapsed_time < fall_time)
		{
            // Calculate the drop current position
            vec2 current_pos = initial_pos + ellapsed_time * velocity;

            // Add the trail color to the pixel color
            pixel_color += trail_color(gl_FragCoord.xy, current_pos, velocity_dir, trail_width, trail_size);
        }
		else
		{
            // Calculate the drop final position
            //vec2 final_pos = initial_pos + fall_time * velocity;

            // Add the wave color to the pixel color
            //pixel_color += wave_color(gl_FragCoord.xy, final_pos, wave_size, ellapsed_time - fall_time);
        }
    }

    // Fragment shader output
    return vec4(pixel_color, 1.0);
}

void main()
{
	vec4 sourceColor = texture2D(s_sourceTexture, v_texCoord0);
	
	vec4 color = sourceColor + mainproc(u_engineTime, v_texCoord0) / 20.0;
	
	gl_FragColor = lerp(sourceColor, color, intensity.x);
}
