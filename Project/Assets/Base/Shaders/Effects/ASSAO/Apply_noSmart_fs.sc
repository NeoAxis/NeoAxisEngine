$input v_texCoord0

// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#include "../../Common.sh"

SAMPLER2DARRAY(s_depthHalfTextureArray, 0);

void main()
{
    float a = bgfxTexture2DArray(s_depthHalfTextureArray, vec3(v_texCoord0, 0.0)).x;
    float b = bgfxTexture2DArray(s_depthHalfTextureArray, vec3(v_texCoord0, 1.0)).x;
    float c = bgfxTexture2DArray(s_depthHalfTextureArray, vec3(v_texCoord0, 2.0)).x;
    float d = bgfxTexture2DArray(s_depthHalfTextureArray, vec3(v_texCoord0, 3.0)).x;

    float avg = (a + b + c + d) * 0.25;

    gl_FragColor = vec4(avg, avg, avg, 1.0);
}