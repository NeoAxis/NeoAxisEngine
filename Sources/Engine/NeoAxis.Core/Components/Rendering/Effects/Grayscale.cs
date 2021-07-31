// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using System.ComponentModel;
using System.Reflection;

namespace NeoAxis
{
	/// <summary>
	/// Screen effect that turns the image into the grayscale.
	/// </summary>
	[DefaultOrderOfEffect( 9 )]
	[Editor.WhenCreatingShowWarningIfItAlreadyExists]
	public class Component_RenderingEffect_Grayscale : Component_RenderingEffect_Simple
	{
		const string shaderDefault = @"Base\Shaders\Effects\Grayscale_fs.sc";

		public Component_RenderingEffect_Grayscale()
		{
			Shader = shaderDefault;
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
