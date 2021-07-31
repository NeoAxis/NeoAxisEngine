// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
#pragma once

#include <objc/Object.h>
#include <IOKit/IOKitLib.h>
#include <IOKit/IOCFPlugIn.h>
#include <Kernel/IOKit/hidsystem/IOHIDUsageTables.h>
#include <IOKit/hid/IOHIDLib.h>
#include <IOKit/hid/IOHIDKeys.h>

enum JoystickAxes
{
	JoystickAxes_X,
	JoystickAxes_Y,
	JoystickAxes_Z,
	JoystickAxes_Rx,
	JoystickAxes_Ry,
	JoystickAxes_Rz,

	//XBox360 Controller
	JoystickAxes_XBox360_LeftThumbstickX,
	JoystickAxes_XBox360_LeftThumbstickY,
	JoystickAxes_XBox360_RightThumbstickX,
	JoystickAxes_XBox360_RightThumbstickY,
	JoystickAxes_XBox360_LeftTrigger,
	JoystickAxes_XBox360_RightTrigger,

	JoystickAxes_Special1,
	JoystickAxes_Special2,
	JoystickAxes_Special3,
	JoystickAxes_Special4,
	JoystickAxes_Special5,
	JoystickAxes_Special6,
	JoystickAxes_Special7,
	JoystickAxes_Special8,
	JoystickAxes_Special9,
	JoystickAxes_Special10,
};

enum ComponentTypes
{
	ComponentTypes_Button,
	ComponentTypes_POV,
	ComponentTypes_Axis,
	ComponentTypes_Slider,
};

struct DeviceInstance
{
	bool errorDuringInitialization;

	//general info
	IOHIDDeviceInterface** deviceInterface;
	IOHIDQueueInterface** queueInterface;
	std::wstring deviceName;
	long deviceUsage;
	long deviceUsagePage;

	//buttons
	struct ButtonInfo
	{
		IOHIDElementCookie cookie;
	};
	std::vector<ButtonInfo> buttons;

	//povs
	struct POVInfo
	{
		IOHIDElementCookie cookie;
	};
	std::vector<POVInfo> povs;

	//axes
	struct AxisInfo
	{
		IOHIDElementCookie cookie;
		JoystickAxes axisName;
		int minValue;
		int maxValue;
	};
	std::vector<AxisInfo> axes;

	//sliders
	struct SliderInfo
	{
		IOHIDElementCookie cookie;
		int minValue;
		int maxValue;
	};
	std::vector<SliderInfo> sliders;

	//queue
	struct QueueMessage
	{
		ComponentTypes componentType;
		int componentIndex;
		float value1;
		float value2;
	};
	std::queue<QueueMessage> queueMessages;

};
