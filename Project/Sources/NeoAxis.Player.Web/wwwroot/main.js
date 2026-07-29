// Copyright 2006–2026 Ivan Efimov. All rights reserved.
import { dotnet } from './_framework/dotnet.js'

const splash = (() =>
{
	const el = document.createElement("div");
	el.style.cssText =
		"position:fixed;inset:0;z-index:9999;display:flex;align-items:center;justify-content:center;" +
		"background:#1b1b1b;pointer-events:none;opacity:1;transition:opacity .5s ease-out;";

	const img = document.createElement("img");
	img.src = new URL("./Assets/NeoAxisLogo_DarkBackground.png", import.meta.url).href;
	img.draggable = false;
	img.style.cssText = "max-width:30vw;max-height:30vh;object-fit:contain;user-select:none;";
	el.appendChild(img);
	document.body.appendChild(el);

	let hidden = false;
	return {
		hide()
		{
			if (hidden) return;
			hidden = true;
			el.style.opacity = "0";
			el.addEventListener("transitionend", () => el.remove(), { once: true });
			setTimeout(() => el.remove(), 1500);
		}
	};
})();

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

let mouseRelativeModeHandler = (enable) => { };

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



		let currentWidth = 0;
		let currentHeight = 0;
		let currentFullscreen = null;
		let cachedRect = null;

		const invalidateCanvasRect = () =>
		{
			cachedRect = null;
		}

		const getCanvasRect = () =>
		{
			if (cachedRect === null)
				cachedRect = canvas.getBoundingClientRect();
			return cachedRect;
		}

		const measureDevicePixelSize = () =>
		{
			const devicePixelRatio = window.devicePixelRatio || 1.0;
			const rect = getCanvasRect();

			const width = Math.round(rect.right * devicePixelRatio) - Math.round(rect.left * devicePixelRatio);
			const height = Math.round(rect.bottom * devicePixelRatio) - Math.round(rect.top * devicePixelRatio);

			return { width: Math.max(1, width), height: Math.max(1, height) };
		}

		const applyCanvasSize = (width, height) =>
		{
			width = Math.max(1, Math.round(width));
			height = Math.max(1, Math.round(height));

			const fullscreenEnabled = document.fullscreenElement != null;

			if (width === currentWidth && height === currentHeight && fullscreenEnabled === currentFullscreen)
				return;

			currentWidth = width;
			currentHeight = height;
			currentFullscreen = fullscreenEnabled;

			if (canvas.width !== width)
				canvas.width = width;
			if (canvas.height !== height)
				canvas.height = height;

			invalidateCanvasRect();

			interop.OnCanvasResize(width, height, fullscreenEnabled);
		}

		const checkCanvasResize = () =>
		{
			invalidateCanvasRect();
			const size = measureDevicePixelSize();
			applyCanvasSize(size.width, size.height);
		}

		if (typeof ResizeObserver !== "undefined")
		{
			const observer = new ResizeObserver((entries) =>
			{
				const box = entries[entries.length - 1].devicePixelContentBoxSize;
				if (box && box.length > 0)
					applyCanvasSize(box[0].inlineSize, box[0].blockSize);
				else
					checkCanvasResize();
			});

			try
			{
				observer.observe(canvas, { box: "device-pixel-content-box" });
			}
			catch
			{
				observer.observe(canvas, { box: "content-box" });
			}
		}
		else
		{
			const checkCanvasResizeFrame = () =>
			{
				checkCanvasResize();
				requestAnimationFrame(checkCanvasResizeFrame);
			}
			requestAnimationFrame(checkCanvasResizeFrame);
		}

		let devicePixelRatioMedia = null;

		function onDevicePixelRatioChanged()
		{
			watchDevicePixelRatio();
			checkCanvasResize();
		}

		function watchDevicePixelRatio()
		{
			if (devicePixelRatioMedia !== null)
				devicePixelRatioMedia.removeEventListener("change", onDevicePixelRatioChanged);
			devicePixelRatioMedia = window.matchMedia(`(resolution: ${window.devicePixelRatio}dppx)`);
			devicePixelRatioMedia.addEventListener("change", onDevicePixelRatioChanged, { once: true });
		}

		watchDevicePixelRatio();

		window.addEventListener("resize", checkCanvasResize);
		window.addEventListener("scroll", invalidateCanvasRect, true);
		document.addEventListener("fullscreenchange", checkCanvasResize);

		const getCanvasPosition = (clientX, clientY) =>
		{
			const rect = getCanvasRect();
			const scaleX = rect.width > 0 ? canvas.width / rect.width : 1.0;
			const scaleY = rect.height > 0 ? canvas.height / rect.height : 1.0;

			return {
				x: (clientX - rect.left) * scaleX,
				y: (clientY - rect.top) * scaleY
			};
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

		let relativeModeRequested = false;

		const POINTER_LOCK_COOLDOWN = 1500;
		let pointerLockExitTime = 0;
		let exitRequestedByPage = false;

		let unadjustedMovementSupported = true;

		const isPointerLocked = () => document.pointerLockElement === canvas;

		const requestPointerLock = () =>
		{
			if (isPointerLocked())
				return;

			if (Date.now() - pointerLockExitTime < POINTER_LOCK_COOLDOWN)
				return;

			let promise;
			try
			{
				promise = unadjustedMovementSupported
					? canvas.requestPointerLock({ unadjustedMovement: true })
					: canvas.requestPointerLock();
			}
			catch (err)
			{
				console.warn(`Pointer lock is not available: ${err.message}`);
				return;
			}

			if (!promise || typeof promise.catch !== "function")
				return;

			promise.catch((err) =>
			{
				if (err.name === "NotSupportedError" && unadjustedMovementSupported)
				{
					unadjustedMovementSupported = false;

					const retry = canvas.requestPointerLock();
					if (retry && typeof retry.catch === "function")
						retry.catch((retryError) => console.warn(`Pointer lock request failed: ${retryError.message}`));
					return;
				}

				console.warn(`Pointer lock request failed: ${err.message}`);
			});
		}

		const setMouseRelativeMode = (enable) =>
		{
			relativeModeRequested = enable;

			if (enable)
				requestPointerLock();
			else if (isPointerLocked())
			{
				exitRequestedByPage = true;
				document.exitPointerLock();
			}
		}

		mouseRelativeModeHandler = setMouseRelativeMode;

		//esc always leaves pointer lock and the page can not prevent it, so the engine has to be
		//told. relativeModeRequested is deliberately not cleared here: the engine still wants the
		//mode, so the next click inside the canvas grabs the mouse back.
		document.addEventListener("pointerlockchange", () =>
		{
			const locked = isPointerLocked();

			if (!locked)
			{
				if (!exitRequestedByPage)
					pointerLockExitTime = Date.now();
				exitRequestedByPage = false;
			}

			interop.OnMouseRelativeModeChanged(locked);
		});

		if (window.self !== window.top)
			console.warn("The player runs inside an iframe. Pointer lock needs allow=\"pointer-lock\" on the iframe tag.");
		const toCanvasPixels = (value) => value * (window.devicePixelRatio || 1.0);

		//CHANGED end

		const mouseMove = (e) =>
		{
			if (isPointerLocked())
				interop.OnMouseMoveRelative(toCanvasPixels(e.movementX), toCanvasPixels(e.movementY));
			else
			{
				const position = getCanvasPosition(e.clientX, e.clientY);
				interop.OnMouseMove(position.x, position.y);
			}
		}

		const mouseDown = (e) =>
		{
			if (relativeModeRequested && !isPointerLocked())
				requestPointerLock();

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
			return e.touches.length > 1 || e.type == "touchend" && e.touches.length > 0;
		}

		const touchStart = (e) =>
		{
			if (shouldIgnore(e))
				return;

			const touches = e.changedTouches;
			for (let i = 0; i < touches.length; i++)
			{
				const touch = touches[i];
				const position = getCanvasPosition(touch.clientX, touch.clientY);
				interop.OnTouchStart(touch.identifier, position.x, position.y, getEventModifiers(e));
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
		checkCanvasResize();

		canvas.tabIndex = 1000;

		interop.SetRootUri(window.location.toString());
	},
	hideLogo: () => splash.hide(),
	setMouseRelativeMode: (enable) => mouseRelativeModeHandler(enable),
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
