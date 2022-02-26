vec3 a_position : POSITION;
uvec4 a_indices : BLENDINDICES;
vec4 a_weight : BLENDWEIGHT;
vec2 a_texcoord0 : TEXCOORD0;
vec4 i_data0 : TEXCOORD7;
vec4 i_data1 : TEXCOORD6;
vec4 i_data2 : TEXCOORD5;
vec4 i_data3 : TEXCOORD4;
vec4 i_data4 : TEXCOORD3;

vec2 v_depth : TEXCOORD0;
vec3 v_worldPosition : TEXCOORD1;
vec4 v_lodValue_visibilityDistance_receiveDecals : TEXCOORD2;
vec2 v_texCoord0 : TEXCOORD3;
vec4 v_billboardDataIndexes : TEXCOORD4 = vec4(0.0, 0.0, 0.0, 0.0);
vec3 v_billboardDataFactors : TEXCOORD5 = vec3(0.0, 0.0, 0.0);
vec4 v_billboardDataAngles : TEXCOORD6 = vec4(0.0, 0.0, 0.0, 0.0);
vec4 v_billboardRotation : TEXCOORD7 = vec4(0.0, 0.0, 0.0, 0.0);
