#include <string.h>
#include <sys/mman.h>
#include <unistd.h>
#include <linux/input-event-codes.h>
#include <wayland-client.h>
#include <wayland-cursor.h>
#include <xkbcommon/xkbcommon.h>
#include <libdecor-0/libdecor.h>

#define EXPORT extern "C" __attribute__( ( visibility( "default" ) ) )

enum MessageTypes
{
	WindowClose,
	WindowResize,
	MouseMove,
	MouseDown,
	MouseUp,
	KeyDown,
	KeyUp,
};

typedef void (*CallbackMessageEvent)(int messageType, int parameterA, int parameterB, int parameterC);

static CallbackMessageEvent messageEvent;
static wl_display* display;
static wl_registry* registry;
static wl_compositor* compositor;
static wl_shm* shm;
static wl_seat* seat;
static wl_pointer* pointer;
static wl_keyboard* keyboard;
static wl_surface* surface;
static wl_surface* cursorSurface;
static wl_cursor_theme* cursorTheme;
static xkb_context* xkbContext;
static xkb_keymap* xkbKeymap;
static xkb_state* xkbState;
static libdecor* decorContext;
static libdecor_frame* frame;
static int windowWidth;
static int windowHeight;
static bool configured;
static bool pointerInside;

static void SendMessageEvent(MessageTypes messageType, int parameterA, int parameterB)
{
	if (messageEvent)
		messageEvent(messageType, parameterA, parameterB, 0);
}

static void PointerEnter(void* data, wl_pointer* wlPointer, uint32_t serial, wl_surface* enteredSurface, wl_fixed_t x, wl_fixed_t y)
{
	pointerInside = enteredSurface == surface;
	if (!pointerInside)
		return;

	if (cursorTheme && cursorSurface)
	{
		wl_cursor* cursor = wl_cursor_theme_get_cursor(cursorTheme, "left_ptr");
		if (cursor)
		{
			wl_cursor_image* image = cursor->images[0];
			wl_surface_attach(cursorSurface, wl_cursor_image_get_buffer(image), 0, 0);
			wl_surface_damage(cursorSurface, 0, 0, image->width, image->height);
			wl_surface_commit(cursorSurface);
			wl_pointer_set_cursor(wlPointer, serial, cursorSurface, image->hotspot_x, image->hotspot_y);
		}
	}

	SendMessageEvent(MouseMove, wl_fixed_to_int(x), wl_fixed_to_int(y));
}

static void PointerLeave(void* data, wl_pointer* wlPointer, uint32_t serial, wl_surface* leftSurface)
{
	if (leftSurface == surface)
		pointerInside = false;
}

static void PointerMotion(void* data, wl_pointer* wlPointer, uint32_t time, wl_fixed_t x, wl_fixed_t y)
{
	if (pointerInside)
		SendMessageEvent(MouseMove, wl_fixed_to_int(x), wl_fixed_to_int(y));
}

static void PointerButton(void* data, wl_pointer* wlPointer, uint32_t serial, uint32_t time, uint32_t button, uint32_t state)
{
	if (!pointerInside)
		return;

	int x11Button = 0;
	switch (button)
	{
	case BTN_LEFT: x11Button = 1; break;
	case BTN_MIDDLE: x11Button = 2; break;
	case BTN_RIGHT: x11Button = 3; break;
	case BTN_SIDE: x11Button = 8; break;
	case BTN_EXTRA: x11Button = 9; break;
	}
	if (x11Button == 0)
		return;

	SendMessageEvent(state == WL_POINTER_BUTTON_STATE_PRESSED ? MouseDown : MouseUp, x11Button, 0);
}

static void PointerAxis(void* data, wl_pointer* wlPointer, uint32_t time, uint32_t axis, wl_fixed_t value)
{
	if (pointerInside && axis == WL_POINTER_AXIS_VERTICAL_SCROLL && value != 0)
		SendMessageEvent(MouseDown, value < 0 ? 4 : 5, 0);
}

static void PointerFrame(void* data, wl_pointer* wlPointer)
{
}

static void PointerAxisSource(void* data, wl_pointer* wlPointer, uint32_t axisSource)
{
}

static void PointerAxisStop(void* data, wl_pointer* wlPointer, uint32_t time, uint32_t axis)
{
}

static void PointerAxisDiscrete(void* data, wl_pointer* wlPointer, uint32_t axis, int32_t discrete)
{
}

static const wl_pointer_listener pointerListener = { PointerEnter, PointerLeave, PointerMotion, PointerButton, PointerAxis, PointerFrame, PointerAxisSource, PointerAxisStop, PointerAxisDiscrete };

static void KeyboardKeymap(void* data, wl_keyboard* wlKeyboard, uint32_t format, int32_t fd, uint32_t size)
{
	if (format != WL_KEYBOARD_KEYMAP_FORMAT_XKB_V1)
	{
		close(fd);
		return;
	}

	char* map = (char*)mmap(nullptr, size, PROT_READ, MAP_PRIVATE, fd, 0);
	if (map == MAP_FAILED)
	{
		close(fd);
		return;
	}

	if (xkbState)
		xkb_state_unref(xkbState);
	if (xkbKeymap)
		xkb_keymap_unref(xkbKeymap);

	xkbKeymap = xkb_keymap_new_from_string(xkbContext, map, XKB_KEYMAP_FORMAT_TEXT_V1, XKB_KEYMAP_COMPILE_NO_FLAGS);
	munmap(map, size);
	close(fd);

	xkbState = xkbKeymap ? xkb_state_new(xkbKeymap) : nullptr;
}

static void KeyboardEnter(void* data, wl_keyboard* wlKeyboard, uint32_t serial, wl_surface* enteredSurface, wl_array* keys)
{
}

static void KeyboardLeave(void* data, wl_keyboard* wlKeyboard, uint32_t serial, wl_surface* leftSurface)
{
}

static void KeyboardKey(void* data, wl_keyboard* wlKeyboard, uint32_t serial, uint32_t time, uint32_t key, uint32_t state)
{
	if (!xkbState)
		return;

	xkb_keycode_t keycode = key + 8;

	const xkb_keysym_t* syms;
	int count = xkb_keymap_key_get_syms_by_level(xkbKeymap, keycode, 0, 0, &syms);
	int keySym = count > 0 ? (int)syms[0] : 0;

	if (state == WL_KEYBOARD_KEY_STATE_PRESSED)
		SendMessageEvent(KeyDown, keySym, (int)xkb_state_key_get_utf32(xkbState, keycode));
	else
		SendMessageEvent(KeyUp, keySym, 0);
}

static void KeyboardModifiers(void* data, wl_keyboard* wlKeyboard, uint32_t serial, uint32_t depressed, uint32_t latched, uint32_t locked, uint32_t group)
{
	if (xkbState)
		xkb_state_update_mask(xkbState, depressed, latched, locked, 0, 0, group);
}

static void KeyboardRepeatInfo(void* data, wl_keyboard* wlKeyboard, int32_t rate, int32_t delay)
{
}

static const wl_keyboard_listener keyboardListener = { KeyboardKeymap, KeyboardEnter, KeyboardLeave, KeyboardKey, KeyboardModifiers, KeyboardRepeatInfo };

static void SeatCapabilities(void* data, wl_seat* wlSeat, uint32_t capabilities)
{
	if ((capabilities & WL_SEAT_CAPABILITY_POINTER) && !pointer)
	{
		pointer = wl_seat_get_pointer(wlSeat);
		wl_pointer_add_listener(pointer, &pointerListener, nullptr);
	}
	if ((capabilities & WL_SEAT_CAPABILITY_KEYBOARD) && !keyboard)
	{
		keyboard = wl_seat_get_keyboard(wlSeat);
		wl_keyboard_add_listener(keyboard, &keyboardListener, nullptr);
	}
}

static void SeatName(void* data, wl_seat* wlSeat, const char* name)
{
}

static const wl_seat_listener seatListener = { SeatCapabilities, SeatName };

static void RegistryGlobal(void* data, wl_registry* registry, uint32_t name, const char* interface, uint32_t version)
{
	if (strcmp(interface, wl_compositor_interface.name) == 0)
		compositor = (wl_compositor*)wl_registry_bind(registry, name, &wl_compositor_interface, version < 4 ? version : 4);
	else if (strcmp(interface, wl_shm_interface.name) == 0)
		shm = (wl_shm*)wl_registry_bind(registry, name, &wl_shm_interface, 1);
	else if (strcmp(interface, wl_seat_interface.name) == 0 && !seat)
	{
		seat = (wl_seat*)wl_registry_bind(registry, name, &wl_seat_interface, version < 5 ? version : 5);
		wl_seat_add_listener(seat, &seatListener, nullptr);
	}
}

static void RegistryGlobalRemove(void* data, wl_registry* registry, uint32_t name)
{
}

static const wl_registry_listener registryListener = { RegistryGlobal, RegistryGlobalRemove };

static void DecorError(libdecor* context, libdecor_error error, const char* message)
{
}

static libdecor_interface decorInterface = { DecorError };

static void FrameConfigure(libdecor_frame* frame, libdecor_configuration* configuration, void* userData)
{
	int width, height;
	if (!libdecor_configuration_get_content_size(configuration, frame, &width, &height))
	{
		width = windowWidth;
		height = windowHeight;
	}

	libdecor_state* state = libdecor_state_new(width, height);
	libdecor_frame_commit(frame, state, configuration);
	libdecor_state_free(state);

	configured = true;

	if (windowWidth != width || windowHeight != height)
	{
		windowWidth = width;
		windowHeight = height;
		SendMessageEvent(WindowResize, width, height);
	}
}

static void FrameClose(libdecor_frame* frame, void* userData)
{
	SendMessageEvent(WindowClose, 0, 0);
}

static void FrameCommit(libdecor_frame* frame, void* userData)
{
	wl_surface_commit(surface);
}

static void FrameDismissPopup(libdecor_frame* frame, const char* seatName, void* userData)
{
}

static libdecor_frame_interface frameInterface = { FrameConfigure, FrameClose, FrameCommit, FrameDismissPopup };

EXPORT int LinuxWaylandWrapper_IsAvailable()
{
	wl_display* testDisplay = wl_display_connect(nullptr);
	if (!testDisplay)
		return 0;
	wl_display_disconnect(testDisplay);
	return 1;
}

EXPORT void* LinuxWaylandWrapper_CreateWindow(CallbackMessageEvent callback, const char* title, int width, int height)
{
	messageEvent = callback;
	windowWidth = width;
	windowHeight = height;
	configured = false;
	pointerInside = false;

	display = wl_display_connect(nullptr);
	if (!display)
		return nullptr;

	xkbContext = xkb_context_new(XKB_CONTEXT_NO_FLAGS);

	registry = wl_display_get_registry(display);
	wl_registry_add_listener(registry, &registryListener, nullptr);
	wl_display_roundtrip(display);
	if (!compositor)
		return nullptr;

	if (shm)
		cursorTheme = wl_cursor_theme_load(nullptr, 24, shm);
	cursorSurface = wl_compositor_create_surface(compositor);

	surface = wl_compositor_create_surface(compositor);

	decorContext = libdecor_new(display, &decorInterface);
	frame = libdecor_decorate(decorContext, surface, &frameInterface, nullptr);
	libdecor_frame_set_app_id(frame, "NeoAxis");
	libdecor_frame_set_title(frame, title);
	libdecor_frame_map(frame);

	while (!configured)
	{
		if (libdecor_dispatch(decorContext, -1) < 0)
			return nullptr;
	}

	return surface;
}

EXPORT void* LinuxWaylandWrapper_GetDisplay()
{
	return display;
}

EXPORT void LinuxWaylandWrapper_ProcessEvents()
{
	if (decorContext)
		libdecor_dispatch(decorContext, 0);
}

EXPORT void LinuxWaylandWrapper_GetWindowSize(int* width, int* height)
{
	*width = windowWidth;
	*height = windowHeight;
}

EXPORT void LinuxWaylandWrapper_SetWindowTitle(const char* title)
{
	if (frame)
		libdecor_frame_set_title(frame, title);
}

EXPORT void LinuxWaylandWrapper_DestroyWindow()
{
	if (frame)
	{
		libdecor_frame_unref(frame);
		frame = nullptr;
	}
	if (decorContext)
	{
		libdecor_unref(decorContext);
		decorContext = nullptr;
	}
	if (keyboard)
	{
		wl_keyboard_destroy(keyboard);
		keyboard = nullptr;
	}
	if (pointer)
	{
		wl_pointer_destroy(pointer);
		pointer = nullptr;
	}
	if (seat)
	{
		wl_seat_destroy(seat);
		seat = nullptr;
	}
	if (xkbState)
	{
		xkb_state_unref(xkbState);
		xkbState = nullptr;
	}
	if (xkbKeymap)
	{
		xkb_keymap_unref(xkbKeymap);
		xkbKeymap = nullptr;
	}
	if (xkbContext)
	{
		xkb_context_unref(xkbContext);
		xkbContext = nullptr;
	}
	if (cursorTheme)
	{
		wl_cursor_theme_destroy(cursorTheme);
		cursorTheme = nullptr;
	}
	if (cursorSurface)
	{
		wl_surface_destroy(cursorSurface);
		cursorSurface = nullptr;
	}
	if (surface)
	{
		wl_surface_destroy(surface);
		surface = nullptr;
	}
	if (shm)
	{
		wl_shm_destroy(shm);
		shm = nullptr;
	}
	if (compositor)
	{
		wl_compositor_destroy(compositor);
		compositor = nullptr;
	}
	if (registry)
	{
		wl_registry_destroy(registry);
		registry = nullptr;
	}
	if (display)
	{
		wl_display_disconnect(display);
		display = nullptr;
	}
	messageEvent = nullptr;
}