// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using SharpBgfx;

namespace NeoAxis
{
	/// <summary>
	/// Specifies the engine settings.
	/// </summary>
	public static class EngineSettings
	{
		/// <summary>
		/// Represents initialization parameters for <see cref="EngineSettings"/>.
		/// </summary>
		public static class Init
		{
			public static RendererBackend RendererBackend = RendererBackend.Default;
			public static bool RendererReportDebugToLog;
			public static bool SimulationVSync = true;
			public static bool SimulationTripleBuffering;
			public static bool UseShaderCache = true;
			public static bool AnisotropicFiltering = true;

			public static string SoundSystemDLL = "";
			public static int SoundMaxReal2DChannels = 32;
			public static int SoundMaxReal3DChannels = 50;

			public static bool ScriptingCompileProjectSolutionAtStartup = true;

			public static double AutoUnloadTexturesNotUsedForLongTimeInSecondsInEditor = 300;
			public static double AutoUnloadTexturesNotUsedForLongTimeInSecondsInSimulation = 300;

			//!!!!!gamma?
		}
	}
}
