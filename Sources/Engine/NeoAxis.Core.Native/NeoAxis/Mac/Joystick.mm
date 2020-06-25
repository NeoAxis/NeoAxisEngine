// Copyright (C) NeoAxis Group Ltd. This is part of NeoAxis 3D Engine SDK.
#import "MacAppNativeWrapper.h"
#import "Joystick.h"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

bool mutexInitialized = false;
pthread_mutex_t mutex;

void InitMutex()
{
	pthread_mutexattr_t attr;
	pthread_mutexattr_init(&attr);
	pthread_mutexattr_settype(&attr, PTHREAD_MUTEX_RECURSIVE);
	pthread_mutex_init(&mutex, &attr);
	mutexInitialized = true;
}

void DestroyMutex()
{
	if(mutexInitialized)
	{
		pthread_mutex_destroy(&mutex);
		mutexInitialized = false;
	}
}

void LockMutex()
{
	pthread_mutex_lock(&mutex);
}

void UnlockMutex()
{
	pthread_mutex_unlock(&mutex);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////

void QueueCallbackFunction(void* target, IOReturn result, void* refcon, void* sender);

void DeviceTopLevelElementHandler(const void* value, void* parameter)
{
	DeviceInstance* instance = (DeviceInstance*)parameter;

	if(CFGetTypeID(value) == CFDictionaryGetTypeID())
	{
		CFTypeRef refCF = 0;

		refCF = CFDictionaryGetValue((CFDictionaryRef)value, CFSTR(kIOHIDElementUsagePageKey));
		if(!CFNumberGetValue((CFNumberRef)refCF, kCFNumberLongType, &instance->deviceUsagePage))
		{
			LogInfo("InputSystem: Joystick Initialization: "\
				"CFNumberGetValue error retrieving DeviceInstance.usagePage.");
			instance->errorDuringInitialization = true;
			return;
		}

		refCF = CFDictionaryGetValue((CFDictionaryRef)value, CFSTR(kIOHIDElementUsageKey));
		if(!CFNumberGetValue((CFNumberRef)refCF, kCFNumberLongType, &instance->deviceUsage))
		{
			LogInfo("InputSystem: Joystick Initialization: "\
				"CFNumberGetValue error retrieving DeviceInstance.usage.");
			instance->errorDuringInitialization = true;
			return;
		}
	}
}

std::wstring GetWString(CFStringRef str)
{
	int length = CFStringGetLength(str);
	std::wstring result;
	result.resize(length);
	for(int n = 0; n < length; n++)
		result[n] = CFStringGetCharacterAtIndex(str, n);
	return result;
}

bool InitDeviceGeneralInfo(DeviceInstance* instance, io_object_t hidDevice, CFMutableDictionaryRef hidProperties)
{
	// Mac OS X currently is not mirroring all USB properties to HID page so need to look at USB device page also
	// get dictionary for usb properties: step up two levels and get CF dictionary for USB properties

	io_registry_entry_t parent1;
	if(IORegistryEntryGetParentEntry(hidDevice, kIOServicePlane, &parent1) != KERN_SUCCESS)
		return false;

	io_registry_entry_t parent2;
	if(IORegistryEntryGetParentEntry(parent1, kIOServicePlane, &parent2) != KERN_SUCCESS)
	{
		IOObjectRelease(parent1);
		return false;
	}

	CFMutableDictionaryRef usbProperties = NULL;
	if(IORegistryEntryCreateCFProperties(parent2, &usbProperties, 
		kCFAllocatorDefault, kNilOptions) != KERN_SUCCESS)
	{
		IOObjectRelease(parent2);
		IOObjectRelease(parent1);
		return false;
	}

	if(!usbProperties)
	{
		LogInfo("InputSystem: Joystick Initialization: "\
			"IORegistryEntryCreateCFProperties failed to create usbProperties.");
		IOObjectRelease(parent2);
		IOObjectRelease(parent1);
		return false;
	}

	CFTypeRef refCF = 0;

	bool result = true;

	//get device name
	{
		CFTypeRef refCF = CFDictionaryGetValue(hidProperties, CFSTR(kIOHIDProductKey));
		if(!refCF)
			refCF = CFDictionaryGetValue(usbProperties, CFSTR("USB Product Name"));
		if(!refCF)
		{
			LogInfo("InputSystem: Joystick Initialization: Unable to get device name.");
			result = false;
			goto end;
		}
		instance->deviceName = GetWString((CFStringRef)refCF);
	}

	//get usage page and usage

	refCF = CFDictionaryGetValue(hidProperties, CFSTR(kIOHIDPrimaryUsagePageKey));
	if(refCF)
	{
		if(!CFNumberGetValue((CFNumberRef)refCF, kCFNumberLongType, &instance->deviceUsagePage))
		{
			LogInfo("InputSystem: Joystick Initialization: "\
				"CFNumberGetValue error getting DeviceInstance.usagePage.");
			result = false;
			goto end;
		}
		refCF = CFDictionaryGetValue(hidProperties, CFSTR(kIOHIDPrimaryUsageKey));
		if(refCF)
		{
			if(!CFNumberGetValue((CFNumberRef)refCF, kCFNumberLongType, &instance->deviceUsage))
			{
				LogInfo("InputSystem: Joystick Initialization: "\
					"CFNumberGetValue error getting DeviceInstance.usage.");
				result = false;
				goto end;
			}
		}
	}
	
	//get top level element HID usage page or usage
	if(refCF == NULL)
	{
		CFTypeRef topElement = CFDictionaryGetValue(hidProperties, CFSTR(kIOHIDElementKey));
		CFRange range = {0, CFArrayGetCount((CFArrayRef)topElement)};
		CFArrayApplyFunction((CFArrayRef)topElement, range, DeviceTopLevelElementHandler, instance);
		if(instance->errorDuringInitialization)
		{
			result = false;
			goto end;
		}
	}
	
end:;
	CFRelease(usbProperties);
	IOObjectRelease(parent2);
	IOObjectRelease(parent1);

	return result;
}

//void DeviceRemovalCallback(void* target, IOReturn result, void* refcon, void* sender)
//{
//	LockMutex();
//
//	DeviceInstance* instance = (DeviceInstance*)refcon;
//
//	UnlockMutex();
//}

void GetJoystickComponentArrayCount(const void* value, void* parameter);

JoystickAxes GetAxisNameByUsage(long usage)
{
	switch(usage)
	{
	case kHIDUsage_GD_X: return JoystickAxes_X;
	case kHIDUsage_GD_Y: return JoystickAxes_Y;
	case kHIDUsage_GD_Z: return JoystickAxes_Z;
	case kHIDUsage_GD_Rx: return JoystickAxes_Rx;
	case kHIDUsage_GD_Ry: return JoystickAxes_Ry;
	case kHIDUsage_GD_Rz: return JoystickAxes_Rz;
	}
	LogFatal("MacAppNativeWrapper: GetAxisNameByUsage: Unknown axis type.");
	return (JoystickAxes)0;
}

void GetDeviceComponentArrayHandler(const void* value, void* parameter);

void AddDeviceComponent(CFTypeRef refElement, DeviceInstance* instance)
{
	CFTypeRef refElementType = CFDictionaryGetValue((CFDictionaryRef)refElement, CFSTR(kIOHIDElementTypeKey));
	CFTypeRef refUsagePage = CFDictionaryGetValue((CFDictionaryRef)refElement, CFSTR(kIOHIDElementUsagePageKey));
	CFTypeRef refUsage = CFDictionaryGetValue((CFDictionaryRef)refElement, CFSTR(kIOHIDElementUsageKey));	
	
	long elementType;
	if(refElementType && CFNumberGetValue((CFNumberRef)refElementType, kCFNumberLongType, &elementType))
	{
		if(elementType == kIOHIDElementTypeInput_Misc || elementType == kIOHIDElementTypeInput_Button || 
			elementType == kIOHIDElementTypeInput_Axis)
		{
			long usagePage, usage;
			if(refUsagePage && CFNumberGetValue((CFNumberRef)refUsagePage, kCFNumberLongType, &usagePage) &&
				refUsage && CFNumberGetValue((CFNumberRef)refUsage, kCFNumberLongType, &usage))
			{
				switch(usagePage)
				{
				case kHIDPage_GenericDesktop:
					{
						switch(usage)
						{
						case kHIDUsage_GD_X:
						case kHIDUsage_GD_Y:
						case kHIDUsage_GD_Z:
						case kHIDUsage_GD_Rx:
						case kHIDUsage_GD_Ry:
						case kHIDUsage_GD_Rz:
						//case kHIDUsage_GD_Dial:
						//case kHIDUsage_GD_Wheel:
							{
								DeviceInstance::AxisInfo info;

								CFTypeRef refType;
								long number;

								//cookie
								refType = CFDictionaryGetValue((CFDictionaryRef)refElement, 
									CFSTR(kIOHIDElementCookieKey));
								if(refType && CFNumberGetValue((CFNumberRef)refType, kCFNumberLongType, &number))
									info.cookie = (IOHIDElementCookie)number;
								else
									info.cookie = NULL;

								info.axisName = GetAxisNameByUsage(usage);

								//min value
								refType = CFDictionaryGetValue((CFDictionaryRef)refElement, CFSTR(kIOHIDElementMinKey));
								if(refType && CFNumberGetValue((CFNumberRef)refType, kCFNumberLongType, &number))
									info.minValue = number;
								else
									info.minValue = 0;

								//max value
								refType = CFDictionaryGetValue((CFDictionaryRef)refElement, CFSTR(kIOHIDElementMaxKey));
								if(refType && CFNumberGetValue((CFNumberRef)refType, kCFNumberLongType, &number))
									info.maxValue = number;
								else
									info.maxValue = 0;

								instance->axes.push_back(info);
							}
							break;

						case kHIDUsage_GD_Slider:
							{
								DeviceInstance::SliderInfo info;

								CFTypeRef refType;
								long number;

								//cookie
								refType = CFDictionaryGetValue((CFDictionaryRef)refElement, 
									CFSTR(kIOHIDElementCookieKey));
								if(refType && CFNumberGetValue((CFNumberRef)refType, kCFNumberLongType, &number))
									info.cookie = (IOHIDElementCookie)number;
								else
									info.cookie = NULL;

								//min value
								refType = CFDictionaryGetValue((CFDictionaryRef)refElement, CFSTR(kIOHIDElementMinKey));
								if(refType && CFNumberGetValue((CFNumberRef)refType, kCFNumberLongType, &number))
									info.minValue = number;
								else
									info.minValue = 0;

								//max value
								refType = CFDictionaryGetValue((CFDictionaryRef)refElement, CFSTR(kIOHIDElementMaxKey));
								if(refType && CFNumberGetValue((CFNumberRef)refType, kCFNumberLongType, &number))
									info.maxValue = number;
								else
									info.maxValue = 0;

								instance->sliders.push_back(info);
							}
							break;

						case kHIDUsage_GD_Hatswitch:
							{
								DeviceInstance::POVInfo info;

								CFTypeRef refType = CFDictionaryGetValue((CFDictionaryRef)refElement, 
									CFSTR(kIOHIDElementCookieKey));
								long number;
								if(refType && CFNumberGetValue((CFNumberRef)refType, kCFNumberLongType, &number))
									info.cookie = (IOHIDElementCookie)number;
								else
									info.cookie = NULL;

								instance->povs.push_back(info);
							}
							break;
						}
					}
					break;

				case kHIDPage_Button:
					{
						DeviceInstance::ButtonInfo info;

						CFTypeRef refType = CFDictionaryGetValue((CFDictionaryRef)refElement, 
							CFSTR(kIOHIDElementCookieKey));
						//!!!!!64-bit support?
						long number;
						if(refType && CFNumberGetValue((CFNumberRef)refType, kCFNumberLongType, &number))
							info.cookie = (IOHIDElementCookie)number;
						else
							info.cookie = NULL;

						instance->buttons.push_back(info);
					}
					break;

				}
			}
		}
		else if(kIOHIDElementTypeCollection == elementType)
		{
			//get elements
			CFTypeRef refElementTop = CFDictionaryGetValue((CFMutableDictionaryRef) refElement, 
				CFSTR(kIOHIDElementKey));
			if(refElementTop)
			{
				CFTypeID type = CFGetTypeID(refElementTop);
				if(type == CFArrayGetTypeID())
				{
					CFRange range = {0, CFArrayGetCount((CFArrayRef)refElementTop)};
					CFArrayApplyFunction((CFArrayRef)refElementTop, range, 
						GetDeviceComponentArrayHandler, instance);
				}
			}
		}
	}
}

void GetDeviceComponentArrayHandler(const void* value, void* parameter)
{
	if(CFGetTypeID(value) == CFDictionaryGetTypeID())
		AddDeviceComponent((CFTypeRef)value, (DeviceInstance*)parameter);
}

void InitDeviceComponents(DeviceInstance* instance, CFMutableDictionaryRef hidProperties)
{
	CFTypeRef refElementTop = CFDictionaryGetValue(hidProperties, CFSTR(kIOHIDElementKey));
	if(refElementTop)
	{
		CFTypeID type = CFGetTypeID(refElementTop);
		if(type == CFArrayGetTypeID())
		{
			CFRange range = {0, CFArrayGetCount ((CFArrayRef)refElementTop)};
			CFArrayApplyFunction((CFArrayRef)refElementTop, range, GetDeviceComponentArrayHandler, instance);
		}
	}
}

bool InitDeviceQueue(DeviceInstance* instance)
{
	IOHIDDeviceInterface** deviceInterface = instance->deviceInterface;

	IOReturn ret;

	IOHIDQueueInterface** queueInterface = (*deviceInterface)->allocQueue(deviceInterface);
	instance->queueInterface = queueInterface;

	ret = (*queueInterface)->create(queueInterface, 0, 64);
	if( ret != kIOReturnSuccess )
	{
		LogInfo("InputSystem: Joystick Initialization: IOHIDQueueInterface: Failed to create queue.");
		return false;
	}

	//add components
	for(int n = 0; n < instance->buttons.size(); n++)
		(*queueInterface)->addElement(queueInterface, instance->buttons[n].cookie, 0);
	for(int n = 0; n < instance->povs.size(); n++)
		(*queueInterface)->addElement(queueInterface, instance->povs[n].cookie, 0);
	for(int n = 0; n < instance->axes.size(); n++)
		(*queueInterface)->addElement(queueInterface, instance->axes[n].cookie, 0);
	for(int n = 0; n < instance->sliders.size(); n++)
		(*queueInterface)->addElement(queueInterface, instance->sliders[n].cookie, 0);

	CFRunLoopSourceRef eventSource;
	ret = (*queueInterface)->createAsyncEventSource(queueInterface, &eventSource);
	if( ret != kIOReturnSuccess )
	{
		LogInfo("InputSystem: Joystick Initialization: IOHIDQueueInterface: "\
			"createAsyncEventSource failed.");
		return false;
	}

	ret = (*queueInterface)->setEventCallout(queueInterface, QueueCallbackFunction, 
		NULL, instance);
	if( ret != kIOReturnSuccess )
	{
		LogInfo("InputSystem: Joystick Initialization: IOHIDQueueInterface: setEventCallout failed.");
		return false;
	}

	CFRunLoopAddSource(CFRunLoopGetCurrent(), eventSource, kCFRunLoopDefaultMode);

	ret = (*queueInterface)->start(queueInterface);
	if( ret != kIOReturnSuccess )
	{
		LogInfo("InputSystem: Joystick Initialization: IOHIDQueueInterface: start failed.");
		return false;
	}

	return true;
}

EXPORT bool MacAppNativeWrapper_InitDeviceManager(int& deviceCount, DeviceInstance** nativeObjects)
{
	InitMutex();

	deviceCount = 0;

	io_object_t hidObject = 0;
	io_iterator_t hidIterator = 0;
	IOReturn result = kIOReturnSuccess;
	mach_port_t masterPort = 0;
	CFMutableDictionaryRef hidDictionaryRef = NULL;
	
	result = IOMasterPort(bootstrap_port, &masterPort);
	if(kIOReturnSuccess != result)
	{
		LogInfo("InputSystem: Initialization: IOMasterPort failed.");
		return false;
	}
	
	hidDictionaryRef = IOServiceMatching(kIOHIDDeviceKey);
	if(!hidDictionaryRef)
	{
		LogInfo("InputSystem: Initialization: IOServiceMatching failed.");
		return false;
	}

	result = IOServiceGetMatchingServices(masterPort, hidDictionaryRef, &hidIterator);	
	if(kIOReturnSuccess != result)
	{
		LogInfo("InputSystem: Initialization: IOServiceGetMatchingServices failed.");
		return false;
	}
	
	if(hidIterator)
	{
		while( hidObject = IOIteratorNext(hidIterator) )
		{
			//get dictionary for HID properties
			CFMutableDictionaryRef hidProperties = 0;
			
			kern_return_t kernResult = IORegistryEntryCreateCFProperties(hidObject, &hidProperties, 
				kCFAllocatorDefault, kNilOptions);
			if(kernResult != KERN_SUCCESS || hidProperties == NULL)
				continue;

			SInt32 score = 0;
			IOCFPlugInInterface** pluginInterface = NULL;
			result = IOCreatePlugInInterfaceForService(hidObject, kIOHIDDeviceUserClientTypeID,
				kIOCFPlugInInterfaceID, &pluginInterface, &score);
			if(result != kIOReturnSuccess)
				continue;

			IOHIDDeviceInterface** deviceInterface;

			HRESULT pluginResult = (*pluginInterface)->QueryInterface(pluginInterface,
				CFUUIDGetUUIDBytes(kIOHIDDeviceInterfaceID), (void**)&deviceInterface);
			if(pluginResult != S_OK)
			{
				LogInfo("InputSystem: Joystick Initialization: "\
					"Query HID class device interface failed.");
				CFRelease(hidProperties);
				IOObjectRelease(hidObject);
				continue;
			}
			(*pluginInterface)->Release(pluginInterface);

			if(deviceInterface == NULL)
				continue;

			result = (*deviceInterface)->open(deviceInterface, 0);
			if(result != kIOReturnSuccess)
			{
				LogInfo("InputSystem: Joystick Initialization: Unable to open device interface.");
				CFRelease(hidProperties);
				IOObjectRelease(hidObject);
				continue;
			}

			DeviceInstance* instance = new DeviceInstance();
			instance->errorDuringInitialization = false;
			instance->deviceInterface = deviceInterface;
			instance->deviceUsage = 0;
			instance->deviceUsagePage = 0;

			if(!InitDeviceGeneralInfo(instance, hidObject, hidProperties))
			{
				//close device and skip
				(*deviceInterface)->close(deviceInterface);
				(*deviceInterface)->Release(deviceInterface);
				CFRelease(hidProperties);			
				IOObjectRelease(hidObject);
				delete instance;

				continue;
			}

			if( instance->deviceUsagePage != kHIDPage_GenericDesktop ||
				(instance->deviceUsage != kHIDUsage_GD_Joystick &&
				instance->deviceUsage != kHIDUsage_GD_GamePad &&
				instance->deviceUsage != kHIDUsage_GD_MultiAxisController) )
			{
				//close device and skip
				(*deviceInterface)->close(deviceInterface);
				(*deviceInterface)->Release(deviceInterface);
				CFRelease(hidProperties);			
				IOObjectRelease(hidObject);
				delete instance;

				continue;
			}

			InitDeviceComponents(instance, hidProperties);

			if(!InitDeviceQueue(instance))
			{
				//close device and skip
				(*deviceInterface)->close(deviceInterface);
				(*deviceInterface)->Release(deviceInterface);
				CFRelease(hidProperties);			
				IOObjectRelease(hidObject);
				delete instance;
				
				continue;
			}

			//(*deviceInterface)->setRemovalCallback(deviceInterface, DeviceRemovalCallback, &instance, 
			//	&instance);

			CFRelease(hidProperties);			
			IOObjectRelease(hidObject);

			nativeObjects[deviceCount] = instance;
			deviceCount++;
		}
	}
	
	return true;
}

EXPORT void MacAppNativeWrapper_ShutdownDeviceManager()
{
	DestroyMutex();
}

EXPORT uint16* MacAppNativeWrapper_GetDeviceName(DeviceInstance* instance)
{
	return CreateOutString(instance->deviceName);
}

EXPORT int MacAppNativeWrapper_GetDeviceButtonCount(DeviceInstance* instance)
{
	return instance->buttons.size();
}

EXPORT int MacAppNativeWrapper_GetDevicePOVCount(DeviceInstance* instance)
{
	return instance->povs.size();
}

EXPORT int MacAppNativeWrapper_GetDeviceAxisCount(DeviceInstance* instance)
{
	return instance->axes.size();
}

EXPORT void MacAppNativeWrapper_GetDeviceAxisInfo(DeviceInstance* instance, int axisIndex, 
	JoystickAxes* name, float* rangeMin, float* rangeMax )
{
	*name = instance->axes[axisIndex].axisName;
	*rangeMin = -1;
	*rangeMax = 1;
}

EXPORT int MacAppNativeWrapper_GetDeviceSliderCount(DeviceInstance* instance)
{
	return instance->sliders.size();
}


EXPORT void MacAppNativeWrapper_ShutdownDevice(DeviceInstance* instance)
{
	if(instance->queueInterface)
	{
		IOHIDQueueInterface** queueInterface = instance->queueInterface;
		(*queueInterface)->Release(queueInterface);
		instance->queueInterface = NULL;
	}

	if(instance->deviceInterface)
	{
		IOHIDDeviceInterface** deviceInterface = instance->deviceInterface;
		(*deviceInterface)->close(deviceInterface);
		(*deviceInterface)->Release(deviceInterface);
		instance->deviceInterface = NULL;
	}

	delete instance;
}

void AddQueueMessage(DeviceInstance* instance, ComponentTypes componentType, int componentIndex, 
	float value1, float value2)
{
	//overflow check
	if(instance->queueMessages.size() > 100000)
		return;

	DeviceInstance::QueueMessage message;
	message.componentType = componentType;
	message.componentIndex = componentIndex;
	message.value1 = value1;
	message.value2 = value2;
	instance->queueMessages.push(message);
}

void QueueCallbackFunction(void* target, IOReturn result, void* refcon, void* sender)
{
	LockMutex();

	DeviceInstance* instance = (DeviceInstance*)refcon;
	IOHIDQueueInterface** queueInterface = instance->queueInterface;

	while(result == kIOReturnSuccess)
	{
		IOHIDEventStruct event;
		AbsoluteTime zeroTime = {0,0};
		result = (*queueInterface)->getNextEvent( queueInterface, &event, zeroTime, 0);
		if( result != kIOReturnSuccess )
			continue;

		IOHIDElementCookie cookie = event.elementCookie;

		//buttons
		for(int n = 0; n < instance->buttons.size(); n++)
		{
			if(instance->buttons[n].cookie == cookie)
			{
				bool value = event.value != 0;
				AddQueueMessage(instance, ComponentTypes_Button, n, value ? 1.0f : 0.0f, 0);
				goto next;
			}
		}

		//povs
		for(int n = 0; n < instance->povs.size(); n++)
		{
			if(instance->povs[n].cookie == cookie)
			{
				float value = (float)event.value + .1f;
				AddQueueMessage(instance, ComponentTypes_POV, n, value, 0);
				goto next;
			}
		}

		//axes
		for(int n = 0; n < instance->axes.size(); n++)
		{
			DeviceInstance::AxisInfo axis = instance->axes[n];

			if(axis.cookie == cookie)
			{
				float v = ((float)event.value - (float)axis.minValue) / (float)axis.maxValue;
				float value = v * 2.0f - 1.0f;

				//!!!!!!?
				//invert value for specific axes
				if( axis.axisName == JoystickAxes_Y || axis.axisName == JoystickAxes_Ry || axis.axisName == JoystickAxes_Rz )
					value = -value;

				AddQueueMessage(instance, ComponentTypes_Axis, n, value, 0);
				goto next;
			}
		}

		//sliders
		for(int n = 0; n < instance->sliders.size(); n++)
		{
			DeviceInstance::SliderInfo slider = instance->sliders[n];

			if(slider.cookie == cookie)
			{
				float v = ((float)event.value - (float)slider.minValue) / (float)slider.maxValue;
				float value = v * 2.0f - 1.0f;
				value = -value;

				AddQueueMessage(instance, ComponentTypes_Slider, n, value, 0);
				goto next;
			}
		}

		//// Only intersted in 32 values right now
		//if(event.longValueSize != 0 && event.longValue != NULL)
		//{
		//	free(event.longValue);
		//	continue;
		//}

		next:;
	}

	UnlockMutex();
}

EXPORT bool MacAppNativeWrapper_UpdateDeviceState(DeviceInstance* instance, 
	ComponentTypes* componentType, int* componentIndex, float* value1, float* value2)
{
	bool result;

	LockMutex();

	if(instance->queueMessages.size() != 0)
	{
		DeviceInstance::QueueMessage message = instance->queueMessages.front();
		instance->queueMessages.pop();

		result = true;
		*componentType = message.componentType;
		*componentIndex = message.componentIndex;
		*value1 = message.value1;
		*value2 = message.value2;
	}
	else
	{
		result = false;
		*componentType = (ComponentTypes)0;
		*componentIndex = 0;
		*value1 = 0;
		*value2 = 0;
	}

	UnlockMutex();

	return result;
}
