// Copyright 2006–2026 Ivan Efimov. All rights reserved.
#pragma once

#ifdef _WIN32
	#define EXPORT extern "C" __declspec(dllexport)
#else
	#define EXPORT extern "C" __attribute__ ((visibility("default")))
#endif

//typedef unsigned char uint8;
