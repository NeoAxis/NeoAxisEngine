// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#define GLOBAL_FOG_SUPPORT 1
#define GLOBAL_LIGHT_MASK_SUPPORT 1

#if BGFX_SHADER_LANGUAGE_GLSL
	#define TARGET_MOBILE 1
#endif

#ifdef GLSL
	#define DISPLACEMENT_MAX_STEPS 16
#else
	#define DISPLACEMENT_MAX_STEPS 32
#endif

#ifdef HLSL
	#define GLOBAL_CUT_VOLUME_SUPPORT 1
	//also need change 'GLOBAL_CUT_VOLUME_MAX_COUNT' in SetCutVolumeSettingsUniforms method of Pipeline_Basic_Render.cs.
	#define GLOBAL_CUT_VOLUME_MAX_COUNT 10
#endif

#ifdef HLSL
	#define GLOBAL_REMOVE_TEXTURE_TILING 1
#endif
