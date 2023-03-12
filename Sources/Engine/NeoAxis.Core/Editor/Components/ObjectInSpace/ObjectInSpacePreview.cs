#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class ObjectInSpacePreview : CanvasBasedPreview
	{
		public ObjectInSpacePreview()
		{
		}

		[Browsable( false )]
		protected virtual bool EnableViewportControl
		{
			get
			{
				var objectInSpace = ObjectOfPreview as ObjectInSpace;
				if( objectInSpace != null && objectInSpace.ParentScene == null )//show only if no scene
					return true;
				return false;
			}
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( EnableViewportControl )
			{
				var objectInSpace = ObjectOfPreview as ObjectInSpace;
				if( objectInSpace != null && objectInSpace.ParentScene == null )//show only if no scene
				{
					var scene = CreateScene( false );

					var type = objectInSpace.GetProvidedType();
					if( type != null )
					{
						var obj = (ObjectInSpace)scene.CreateComponent( type );
						obj.Transform = Transform.Identity;
					}

					scene.Enabled = true;
					SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace() );

					if( objectInSpace != null && objectInSpace.EditorCameraTransform != null )
					{
						var tr = objectInSpace.EditorCameraTransform;
						CameraInitialDistance = ( tr.Position - CameraLookTo ).Length() * 1.3;
						CameraDirection = SphericalDirection.FromVector( CameraLookTo - tr.Position );
					}
				}
			}
			else
			{
				ViewportControl.AllowCreateRenderWindow = false;
				ViewportControl.Visible = false;
			}
		}
	}
}

#endif