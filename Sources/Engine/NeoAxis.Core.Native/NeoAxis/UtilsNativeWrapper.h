// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#pragma once

#ifdef _WIN32
	#define EXPORT extern "C" __declspec(dllexport)
#else
	#define EXPORT extern "C" __attribute__ ((visibility("default")))
#endif

//typedef unsigned char uint8;
