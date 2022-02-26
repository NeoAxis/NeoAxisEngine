// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis
{
	/// <summary>
	/// The class is intended to register 2D character resource.
	/// </summary>
	public class Character2DAssemblyRegistration : AssemblyRegistration
	{
		public override void OnRegister()
		{
			//file extension
			var type = ResourceManager.RegisterType( "Character 2D", new string[] { "character2D" }, typeof( Resource ) );
#if !DEPLOY
			Editor.PreviewImagesManager.RegisterResourceType( type );
#endif
		}
	}
}
