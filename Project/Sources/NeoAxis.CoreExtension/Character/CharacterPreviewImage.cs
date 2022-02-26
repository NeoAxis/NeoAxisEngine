// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class CharacterPreviewImage : ObjectInSpacePreviewImage
	{
		public CharacterPreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			//base.OnUpdate();

			var objectInSpace = ObjectOfPreview as ObjectInSpace;
			if( objectInSpace != null )
			{
				var scene = CreateScene( false );

				var type = objectInSpace.GetProvidedType();
				if( type != null )
				{
					var obj = scene.CreateComponent( type ) as Character;
					if( obj != null )
						obj.SetTransform( Transform.Identity, true );

					//var obj = (ObjectInSpace)scene.CreateComponent( type );
					//obj.Transform = Transform.Identity;
				}

				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), 1.6 );// 2.6 );
			}
		}
	}
}
#endif