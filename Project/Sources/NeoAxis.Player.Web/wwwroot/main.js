// Copyright 2006–2026 Ivan Efimov. All rights reserved.

//constants
// Ctrl+Shift+S = open Spector UI
// Ctrl+Shift+C = capture current canvas
// 3-finger double tap = open Spector UI
// 4-finger double tap = capture current canvas
const allowSpector = true;


import { dotnet } from './_framework/dotnet.js'
let canvas = null;

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Spector.js integration

let spectorInstance = null;
let spectorLoadingPromise = null;

const shouldAutoEnableSpector = () =>
{
	if (!allowSpector)
		return false;

	const value = new URLSearchParams(window.location.search).get("spector");
	return value !== null && value !== "0" && value.toLowerCase() !== "false";
}

const loadScriptAsync = (src) =>
{
	return new Promise((resolve, reject) =>
	{
		const existing = document.querySelector(`script[data-src="${src}"]`);
		if (existing)
		{
			if (existing.dataset.loaded === "true" || globalThis.SPECTOR)
			{
				resolve();
				return;
			}

			existing.addEventListener("load", () => resolve(), { once: true });
			existing.addEventListener("error", () => reject(new Error(`Unable to load script '${src}'.`)), { once: true });
			return;
		}

		const script = document.createElement("script");
		script.src = src;
		script.async = true;
		script.dataset.src = src;
		script.addEventListener("load", () =>
		{
			script.dataset.loaded = "true";
			resolve();
		}, { once: true });
		script.addEventListener("error", () => reject(new Error(`Unable to load script '${src}'.`)), { once: true });
		document.head.appendChild(script);
	});
}

const initializeSpectorAsync = async () =>
{
	if (!allowSpector)
		return null;

	if (spectorInstance !== null)
		return spectorInstance;
	if (spectorLoadingPromise !== null)
		return await spectorLoadingPromise;

	spectorLoadingPromise = (async () =>
	{
		try
		{
			if (!globalThis.SPECTOR)
				await loadScriptAsync("https://cdn.jsdelivr.net/npm/spectorjs@0.9.30/dist/spector.bundle.js");

			spectorInstance = new globalThis.SPECTOR.Spector();
			spectorInstance.spyCanvases();
			spectorInstance.displayUI();

			console.info("Spector.js enabled.");
			return spectorInstance;
		}
		catch (err)
		{
			console.warn(`Unable to initialize Spector.js: ${err?.message ?? err}`);
			return null;
		}
		finally
		{
			spectorLoadingPromise = null;
		}
	})();

	return await spectorLoadingPromise;
}

const captureWithSpectorAsync = async () =>
{
	if (!allowSpector)
		return;

	const instance = await initializeSpectorAsync();
	if (instance !== null && canvas !== null)
		instance.captureCanvas(canvas);
}

globalThis.neoAxisSpector =
{
	enable: async () => allowSpector ? await initializeSpectorAsync() : null,
	capture: async () => { if (allowSpector) await captureWithSpectorAsync(); }
};

if (allowSpector)
{
	window.addEventListener("keydown", async (e) =>
	{
		if (!e.ctrlKey || !e.shiftKey || e.altKey || e.metaKey)
			return;

		if (e.code === "KeyS")
		{
			e.preventDefault();
			const instance = await initializeSpectorAsync();
			if (instance !== null)
				instance.displayUI();
		}
		else if (e.code === "KeyC")
		{
			e.preventDefault();
			await captureWithSpectorAsync();
		}
	}, { capture: true });

	if (shouldAutoEnableSpector())
		await initializeSpectorAsync();
}

let lastSpectorTouchGestureTime = 0;
let lastSpectorTouchGestureFingers = 0;
const SPECTOR_TOUCH_GESTURE_INTERVAL = 400;

const processSpectorTouchGestureAsync = async (touchCount) =>
{
	if (!allowSpector)
		return false;

	if (touchCount !== 3 && touchCount !== 4)
		return false;

	const now = performance.now();
	const isDoubleTap =
		lastSpectorTouchGestureFingers === touchCount &&
		now - lastSpectorTouchGestureTime <= SPECTOR_TOUCH_GESTURE_INTERVAL;

	lastSpectorTouchGestureTime = now;
	lastSpectorTouchGestureFingers = touchCount;

	if (!isDoubleTap)
		return false;

	lastSpectorTouchGestureTime = 0;
	lastSpectorTouchGestureFingers = 0;

	if (touchCount === 3)
	{
		const instance = await initializeSpectorAsync();
		if (instance !== null)
			instance.displayUI();
	}
	else
		await captureWithSpectorAsync();

	return true;
}

// Spector.js integration END
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


const splash = (() =>
{
	const el = document.createElement("div");
	el.style.cssText =
		"position:fixed;inset:0;z-index:9999;display:flex;align-items:center;justify-content:center;" +
		"background:#000000;pointer-events:none;opacity:0;transition:opacity .5s ease-out;";

	const img = document.createElement("img");
	img.src = new URL("./Assets/NeoAxisLogo_DarkBackground.png", import.meta.url).href;
	img.draggable = false;
	img.style.cssText = "max-width:30vw;max-height:30vh;object-fit:contain;user-select:none;";
	el.appendChild(img);
	document.body.appendChild(el);

	setTimeout(() =>
	{
		el.style.opacity = "1";
		el.style.transition = "opacity 1s ease-in";
	}, 0);

	let hidden = false;
	return {
		hide()
		{
			if (hidden) return;
			hidden = true;
			el.style.opacity = "0";
			el.style.transition = "opacity .5s ease-out";
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

canvas = globalThis.document.getElementById("canvas");
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

		//set system settings
		let mobileDevice = false;
		if (navigator.userAgentData)
			mobileDevice = navigator.userAgentData.mobile;
		if (!mobileDevice)
		{
			const ua = navigator.userAgent;
			const isMobileUA = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(ua);
			const isAndroidTabletDesktopMode = /Linux/i.test(navigator.platform) && navigator.maxTouchPoints > 1;
			const isIPadDesktopMode = navigator.platform === 'MacIntel' && navigator.maxTouchPoints > 1;
			mobileDevice = isMobileUA || isAndroidTabletDesktopMode || isIPadDesktopMode;
		}
		interop.SetSystemSettings(mobileDevice);

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

		let activeTouchId = null;

		const findActiveTouch = (e) =>
		{
			if (activeTouchId === null)
				return null;

			const touches = e.changedTouches;
			for (let i = 0; i < touches.length; i++)
			{
				if (touches[i].identifier === activeTouchId)
					return touches[i];
			}
			return null;
		}

		const touchStart = (e) =>
		{
			e.preventDefault();

			//Spector specific
			if (allowSpector)
			{
				const spectorTouchCount = e.touches.length;
				if (spectorTouchCount >= 3 && spectorTouchCount <= 4)
				{
					processSpectorTouchGestureAsync(spectorTouchCount);
					return;
				}
			}
			//Spector specific END

			if (activeTouchId !== null)
				return;

			const touch = e.changedTouches[0];
			if (!touch)
				return;

			activeTouchId = touch.identifier;
			canvas.focus();
			invalidateCanvasRect();

			const position = getCanvasPosition(touch.clientX, touch.clientY);
			interop.OnTouchStart(touch.identifier, position.x, position.y, getEventModifiers(e));
		}

		const touchMove = (e) =>
		{
			e.preventDefault();

			const touch = findActiveTouch(e);
			if (touch === null)
				return;

			const position = getCanvasPosition(touch.clientX, touch.clientY);
			interop.OnTouchMove(touch.identifier, position.x, position.y);
		}

		const touchEnd = (e) =>
		{
			e.preventDefault();

			const touch = findActiveTouch(e);
			if (touch === null)
				return;

			activeTouchId = null;

			const position = getCanvasPosition(touch.clientX, touch.clientY);
			interop.OnTouchEnd(touch.identifier, position.x, position.y, getEventModifiers(e));
		}

		canvas.addEventListener("contextmenu", (e) => e.preventDefault(), false);
		canvas.addEventListener("keydown", keyDown, false);
		canvas.addEventListener("keyup", keyUp, false);
		canvas.addEventListener("mousemove", mouseMove, false);
		canvas.addEventListener("mousedown", mouseDown, false);
		canvas.addEventListener("mouseup", mouseUp, false);
		canvas.addEventListener("dblclick", mouseDoubleClick, false);
		canvas.addEventListener("wheel", mouseWheel, { capture: false, passive: false }); //passive: true

		canvas.addEventListener("touchstart", touchStart, { capture: false, passive: false });
		canvas.addEventListener("touchmove", touchMove, { capture: false, passive: false });
		canvas.addEventListener("touchend", touchEnd, { capture: false, passive: false });
		canvas.addEventListener("touchcancel", touchEnd, { capture: false, passive: false });

		canvas.style.touchAction = "none";
		canvas.style.userSelect = "none";
		canvas.style.webkitUserSelect = "none";
		canvas.style.webkitTouchCallout = "none";
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
