vec3 a_position : POSITION;
uvec4 a_indices : BLENDINDICES;
vec4 a_weight : BLENDWEIGHT;
vec2 a_texcoord0 : TEXCOORD0;
vec4 i_data0 : TEXCOORD7;
vec4 i_data1 : TEXCOORD6;
vec4 i_data2 : TEXCOORD5;
vec4 i_data3 : TEXCOORD4;
vec4 i_data4 : TEXCOORD3;

vec3 v_worldPosition : TEXCOORD0;
vec4 v_lodValue_visibilityDistance_receiveDecals : TEXCOORD1;
vec2 v_texCoord0 : TEXCOORD2;
vec3 v_objectSpacePosition : TEXCOORD3 = vec3(0.0, 0.0, 0.0);
vec3 v_cameraPositionObjectSpace : TEXCOORD4 = vec3(0.0, 0.0, 0.0);
vec4 v_worldMatrix0 : TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
vec4 v_worldMatrix1 : TEXCOORD6 = vec4(0.0, 0.0, 0.0, 0.0);
vec4 v_worldMatrix2 : TEXCOORD7 = vec4(0.0, 0.0, 0.0, 0.0);

float glPositionZ : TEXCOORD8 = 0.0;