// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis;

namespace Project
{
	public static class ProjectUtility
	{
		public static void UpdateSceneAntialiasingByAppSettings( Component_Scene scene )
		{
			var pipeline = scene.RenderingPipeline.Value;
			if( pipeline != null )
			{
				var antialising = pipeline.GetComponent<Component_RenderingEffect_Antialiasing>( true );
				if( antialising != null )
				{
					if( Enum.TryParse<Component_RenderingEffect_Antialiasing.TechniqueEnum>( SimulationApp.Antialiasing, false, out var value ) )
					{
						// Save original value to AnyData.
						if( antialising.AnyData == null )
							antialising.AnyData = antialising.Technique.Value;

						antialising.Technique = value;
					}
					else
					{
						// Restore original value from AnyData.
						if( antialising.AnyData != null && antialising.AnyData is Component_RenderingEffect_Antialiasing.TechniqueEnum )
							antialising.Technique = (Component_RenderingEffect_Antialiasing.TechniqueEnum)antialising.AnyData;
					}
				}
			}
		}
	}
}
