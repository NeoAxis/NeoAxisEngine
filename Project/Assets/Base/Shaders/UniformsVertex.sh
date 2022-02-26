// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#define LIGHTDATA_VERTEX_SIZE 1
//#define LIGHTDATA_VERTEX_SIZE 5
//vec4 u_lightDataVertex[LIGHTDATA_VERTEX_SIZE]
#define u_lightPosition u_lightDataVertex[0]
//#define u_lightShadowTextureViewProjMatrix0 mtxFromRows(u_lightDataVertex[1], u_lightDataVertex[2], u_lightDataVertex[3], u_lightDataVertex[4])
