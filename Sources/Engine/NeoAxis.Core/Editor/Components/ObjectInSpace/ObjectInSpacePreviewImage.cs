#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class ObjectInSpacePreviewImage : PreviewImageGenerator
	{
		public ObjectInSpacePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var objectInSpace = ObjectOfPreview as ObjectInSpace;
			if( objectInSpace != null )
			{
				var scene = CreateScene( false );

				var type = objectInSpace.GetProvidedType();
				if( type != null )
				{
					var obj = (ObjectInSpace)scene.CreateComponent( type );
					obj.Transform = Transform.Identity;
				}

				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), 2.6 );

				if( objectInSpace != null && objectInSpace.EditorCameraTransform != null )
				{
					var tr = objectInSpace.EditorCameraTransform;
					CameraDistance = ( tr.Position - CameraLookTo ).Length();// * distanceScale;
					CameraDirection = SphericalDirection.FromVector( CameraLookTo - tr.Position );
				}
			}
		}
	}
}

#endif