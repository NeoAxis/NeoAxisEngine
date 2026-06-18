// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class MeshPreview : CanvasBasedPreview
	{
		protected override void OnCreate()
		{
			base.OnCreate();

			var scene = CreateScene( false );

			Mesh mesh = ObjectOfPreview as Mesh;
			if( mesh != null )
			{
				MeshInSpace objInSpace = scene.CreateComponent<MeshInSpace>();
				objInSpace.Mesh = mesh;
			}

			scene.Enabled = true;

			SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace() );

			if( mesh != null && mesh.EditorCameraTransform != null )
			{
				var tr = mesh.EditorCameraTransform;
				CameraInitialDistance = ( tr.Position - CameraLookTo ).Length() * 1.3;
				CameraDirection = SphericalDirection.FromVector( CameraLookTo - tr.Position );
			}
		}
	}
}