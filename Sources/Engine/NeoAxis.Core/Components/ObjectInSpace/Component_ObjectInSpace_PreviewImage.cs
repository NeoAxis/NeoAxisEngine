// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_ObjectInSpace_PreviewImage : PreviewImageGenerator
	{
		public Component_ObjectInSpace_PreviewImage()
		{
		}

		protected override void Update()
		{
			var objectInSpace = ObjectOfPreview as Component_ObjectInSpace;
			if( objectInSpace != null )
			{
				var scene = CreateScene( false );

				var type = objectInSpace.GetProvidedType();
				if( type != null )
				{
					var obj = (Component_ObjectInSpace)scene.CreateComponent( type );
					obj.Transform = Transform.Identity;
				}

				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), 2.6 );
			}
		}
	}
}
