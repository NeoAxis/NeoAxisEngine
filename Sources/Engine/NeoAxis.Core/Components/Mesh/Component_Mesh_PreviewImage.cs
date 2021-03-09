// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_Mesh_PreviewImage : PreviewImageGenerator
	{
		public Component_Mesh_PreviewImage()
		{
		}

		protected override void Update()
		{
			var scene = CreateScene( false );

			var mesh = ObjectOfPreview as Component_Mesh;
			if( mesh != null )
			{
				var objInSpace = scene.CreateComponent<Component_MeshInSpace>();
				objInSpace.Mesh = mesh;
			}

			scene.Enabled = true;

			SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), 2.6 );// 1.7 );

			if( mesh.EditorCameraTransform != null )
			{
				var tr = mesh.EditorCameraTransform;
				CameraDistance = ( tr.Position - CameraLookTo ).Length();
				CameraDirection = SphericalDirection.FromVector( CameraLookTo - tr.Position );
			}
		}
	}
}
