// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.

#define GLOBAL_FOG_SUPPORT 1
#define GLOBAL_LIGHT_MASK_SUPPORT 1

#if BGFX_SHADER_LANGUAGE_GLSL
#define TARGET_MOBILE 1
#endif

#ifdef GLSL
	#define DISPLACEMENT_STEPS 16
#else
	#define DISPLACEMENT_STEPS 32
#endif
