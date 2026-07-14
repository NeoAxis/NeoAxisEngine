// Copyright 2006–2026 Ivan Efimov. All rights reserved.
import { dotnet } from './_framework/dotnet.js'

// dotnet.withEnvironmentVariable("MONO_LOG_LEVEL", "debug");
// dotnet.withEnvironmentVariable("MONO_LOG_MASK", "all");
//.withDiagnosticTracing(false)

//!!!!
dotnet.withDiagnosticTracing(true);

//!!!!
dotnet.withApplicationArgumentsFromQuery();

const { setModuleImports, getAssemblyExports, getConfig } = await dotnet.create();

const config = getConfig();
const exports = await getAssemblyExports(config.mainAssemblyName);
const interop = exports.NeoAxis.Player.Web.Interop;

const canvas = globalThis.document.getElementById("canvas");
dotnet.instance.Module["canvas"] = canvas;

setModuleImports("main.js", {
	initialize: () =>
	{
		const InputModifiers =
		{
			Shift: 1,
			Ctrl: 2,
			Alt: 4,
			Meta: 8
		};

		const checkCanvasResize = (dispatch) =>
		{

			// const dpr = window.devicePixelRatio || 1.0;
			// const rect = canvas.getBoundingClientRect();

			// const displayWidth = Math.round(rect.width * dpr);
			// const displayHeight = Math.round(rect.height * dpr);

			// if (canvas.width !== displayWidth || canvas.height !== displayHeight) {
			//     canvas.width = displayWidth;
			//     canvas.height = displayHeight;
			//     dispatch = true;
			// }

			// if (dispatch) {
			//     interop.OnCanvasResize(canvas.width, canvas.height, dpr);
			// }


			var devicePixelRatio = window.devicePixelRatio || 1.0;
			var displayWidth = canvas.clientWidth * devicePixelRatio;
			var displayHeight = canvas.clientHeight * devicePixelRatio;

			if (canvas.width != displayWidth || canvas.height != displayHeight)
			{
				canvas.width = displayWidth;
				canvas.height = displayHeight;
				dispatch = true;
			}

			//to update windowed mode in the engine
			var fullscreenEnabled = document.fullscreenElement != null;

			if (dispatch)
				interop.OnCanvasResize(displayWidth, displayHeight/*, devicePixelRatio*/, fullscreenEnabled);
		}

		function checkCanvasResizeFrame()
		{
			checkCanvasResize(false);
			requestAnimationFrame(checkCanvasResizeFrame);
		}

		function getEventModifiers(/** @type {KeyboardEvent|MouseEvent|TouchEvent} */e)
		{
			var flags = 0;
			if (e.shiftKey) flags |= InputModifiers.Shift;
			if (e.ctrlKey) flags |= InputModifiers.Ctrl;
			if (e.altKey) flags |= InputModifiers.Alt;
			if (e.metaKey) flags |= InputModifiers.Meta;
			return flags;
		}

		const keyDown = (e) =>
		{
			var keyLocked = false;
			if (e.key == "Insert")
				keyLocked = e.getModifierState('Insert');
			if (e.key == "NumLock")
				keyLocked = e.getModifierState('NumLock');
			if (e.key == "CapsLock")
				keyLocked = e.getModifierState('CapsLock');
			if (e.key == "ScrollLock")
				keyLocked = e.getModifierState('ScrollLock');

			interop.OnKeyDown(e.keyCode, e.key, getEventModifiers(e), keyLocked);
			e.stopPropagation();
			e.preventDefault();
			return false;
		}

		const keyUp = (e) =>
		{
			var keyLocked = false;
			if (e.key == "Insert")
				keyLocked = e.getModifierState('Insert');
			if (e.key == "NumLock")
				keyLocked = e.getModifierState('NumLock');
			if (e.key == "CapsLock")
				keyLocked = e.getModifierState('CapsLock');
			if (e.key == "ScrollLock")
				keyLocked = e.getModifierState('ScrollLock');

			interop.OnKeyUp(e.keyCode, getEventModifiers(e), keyLocked);
			e.stopPropagation();
			e.preventDefault();
			return false;
		}

		const mouseMove = (e) =>
		{
			var devicePixelRatio = window.devicePixelRatio || 1.0;
			var x = e.offsetX * devicePixelRatio;
			var y = e.offsetY * devicePixelRatio;
			interop.OnMouseMove(x, y);
		}

		const mouseDown = (e) =>
		{
			interop.OnMouseDown(e.button, getEventModifiers(e));
		}

		const mouseUp = (e) =>
		{
			interop.OnMouseUp(e.button, getEventModifiers(e));
		}

		const mouseDoubleClick = (e) =>
		{
			interop.OnMouseDoubleClick(e.button, getEventModifiers(e));
			e.stopPropagation();
			e.preventDefault();
			return false;
		}

		const mouseWheel = (e) =>
		{
			let pixelsY = e.deltaY;
			if (e.deltaMode === 1) // strings
				pixelsY = e.deltaY * 33.33;
			else if (e.deltaMode === 2) // pages
				pixelsY = e.deltaY * window.innerHeight;
			else // pixels
				pixelsY = e.deltaY * 4;

			interop.OnMouseWheel(e.deltaX, -pixelsY);

			e.stopPropagation();
			//!!!!? maybe bool handled?
			e.preventDefault();

			return false;
		}

		const shouldIgnore = (e) =>
		{
			e.preventDefault();
			return e.touches.length > 1 || e.type == "touchend" && e.touches.length > 0;
		}

		const touchStart = (e) =>
		{
			if (shouldIgnore(e))
				return;

			var bcr = e.target.getBoundingClientRect();
			var devicePixelRatio = window.devicePixelRatio || 1.0;
			var touches = e.changedTouches;

			//!!!!gpt:
			//Это неверно, потому что touches.length — число, а for...in тут не подходит.Нужно:
			//for (let i = 0; i < touches.length; i++)

			for (var i in touches.length)
			{
				var touch = e.changedTouches[i];
				var x = (touch.clientX - bcr.x) * devicePixelRatio;
				var y = (touch.clientY - bcr.y) * devicePixelRatio;
				interop.OnTouchStart(touch.identifier, x, y, getEventModifiers(e));
			}
		}

		const touchMove = (e) =>
		{
			if (shouldIgnore(e))
				return;

			var bcr = e.target.getBoundingClientRect();
			var devicePixelRatio = window.devicePixelRatio || 1.0;
			var touches = e.changedTouches;
			for (var i in touches.length)
			{
				var touch = e.changedTouches[i];
				var x = (touch.clientX - bcr.x) * devicePixelRatio;
				var y = (touch.clientY - bcr.y) * devicePixelRatio;
				interop.OnTouchMove(touch.identifier, x, y);
			}
		}

		const touchEnd = (e) =>
		{
			if (shouldIgnore(e))
				return;

			var bcr = e.target.getBoundingClientRect();
			var devicePixelRatio = window.devicePixelRatio || 1.0;
			var touches = e.changedTouches;
			for (var i in touches.length)
			{
				var touch = e.changedTouches[i];
				var x = (touch.clientX - bcr.x) * devicePixelRatio;
				var y = (touch.clientY - bcr.y) * devicePixelRatio;
				interop.OnTouchEnd(touch.identifier, x, y, getEventModifiers(e));
			}
		}

		canvas.addEventListener("contextmenu", (e) => e.preventDefault(), false);
		canvas.addEventListener("keydown", keyDown, false);
		canvas.addEventListener("keyup", keyUp, false);
		canvas.addEventListener("mousemove", mouseMove, false);
		canvas.addEventListener("mousedown", mouseDown, false);
		canvas.addEventListener("mouseup", mouseUp, false);
		canvas.addEventListener("dblclick", mouseDoubleClick, false);
		canvas.addEventListener("wheel", mouseWheel, { capture: false, passive: false }); //passive: true
		//!!!!passive
		canvas.addEventListener("touchstart", touchStart, { capture: false, passive: true });
		canvas.addEventListener("touchmove", touchMove, { capture: false, passive: true });
		canvas.addEventListener("touchend", touchEnd, { capture: false, passive: true });
		checkCanvasResize(true);
		checkCanvasResizeFrame();

		canvas.tabIndex = 1000;

		interop.SetRootUri(window.location.toString());
	},
	setClipboardText: (text) =>
	{
		if (globalThis.document.hasFocus())
		{
			try
			{
				if (navigator.clipboard?.writeText)
					navigator.clipboard.writeText(text);
			}
			catch { }
		}
	},
	getClipboardTextAsync: async () =>
	{
		if (globalThis.document.hasFocus())
		{
			try
			{
				if (navigator.clipboard?.readText)
					return await navigator.clipboard.readText();
			}
			catch { }
		}
		return "";
	},
	setFullscreenAsync: async (enable) =>
	{
		try
		{
			if (globalThis.document.hasFocus())
			{
				if (enable)
				{
					if (!document.fullscreenElement)
						await document.documentElement.requestFullscreen();
				}
				else
					await document.exitFullscreen();
			}
		}
		catch (err)
		{
			console.error(`Fullscreen error: ${err.message}`);
		}
	},
	// setFullscreen: (enable) =>
	// {
	// 	try
	// 	{
	// 		if (globalThis.document.hasFocus())
	// 		{
	// 			if (enable)
	// 			{
	// 				if (!document.fullscreenElement)
	// 				{
	// 					document.documentElement.requestFullscreen().catch((err) =>
	// 					{
	// 						console.error(`Unable to enter fullscreen: ${err.message}`);
	// 					});
	// 				}
	// 			}
	// 			else
	// 			{
	// 				document.exitFullscreen().catch((err) =>
	// 				{
	// 					console.error(`Unable to exit fullscreen: ${err.message}`);
	// 				});
	// 			}
	// 		}
	// 	}
	// 	catch { }
	// },
});

await dotnet.run();
