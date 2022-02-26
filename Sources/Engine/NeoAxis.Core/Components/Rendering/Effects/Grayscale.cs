// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// Screen effect that turns the image into the grayscale.
	/// </summary>
	[DefaultOrderOfEffect( 9 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class RenderingEffect_Grayscale : RenderingEffect_Simple
	{
		const string shaderDefault = @"Base\Shaders\Effects\Grayscale_fs.sc";

		public RenderingEffect_Grayscale()
		{
			ShaderFile = shaderDefault;
		}

		public override bool LimitedDevicesSupport
		{
			get { return true; }
		}

		public override ScreenLabelInfo GetScreenLabelInfo()
		{
			return new ScreenLabelInfo( "ScreenEffect", true );
		}
	}
}
