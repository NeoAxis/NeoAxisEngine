// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System.Runtime.InteropServices;

namespace NeoAxis.Player.Web;

internal static class Emscripten
{
	[DllImport( "emscripten", EntryPoint = "emscripten_request_animation_frame_loop" )]
	[DefaultDllImportSearchPaths( DllImportSearchPath.SafeDirectories )]
	internal static extern unsafe void RequestAnimationFrameLoop( void* f, nint userDataPtr );
}
