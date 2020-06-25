// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;

//!!!!не статично?

namespace NeoAxis.Editor
{
	/// <summary>
	/// Config for <see cref="TransformTool"/>.
	/// </summary>
	public static class TransformToolConfig
	{
		//[EngineConfig( "TransformTool", "movementSnapping" )]
		//public static bool movementSnapping;
		//[EngineConfig( "TransformTool", "rotationSnapping" )]
		//public static bool rotationSnapping;
		//[EngineConfig( "TransformTool", "scalingSnapping" )]
		//public static bool scalingSnapping;

		//[EngineConfig( "TransformTool", "movementSnappingValue" )]
		//public static float movementSnappingValue = .1f;
		//[EngineConfig( "TransformTool", "rotationSnappingValue" )]
		//public static Degree rotationSnappingValue = 5;
		//[EngineConfig( "TransformTool", "scalingSnappingValue" )]
		//public static float scalingSnappingValue = .1f;

		//[EngineConfig( "TransformTool", "moveObjectsUsingLocalCoordinates" )]
		//public static bool moveObjectsUsingLocalCoordinates;
		[EngineConfig( "TransformTool", "moveObjectsDuringRotation" )]
		public static bool moveObjectsDuringRotation = true;
		[EngineConfig( "TransformTool", "moveObjectsDuringScaling" )]
		public static bool moveObjectsDuringScaling;

		//[EngineConfig( "TransformTool", "rotationSensitivity" )]
		//public static float rotationSensitivity = 1;
		//[EngineConfig( "TransformTool", "size" )]
		//public static int size = 140;
		//[EngineConfig( "TransformTool", "lineThickness" )]
		//public static float lineThickness = 2;
		//[EngineConfig( "TransformTool", "shadowIntensity" )]
		//public static float shadowIntensity = .3f;//!!!! .07f;
	}
}
