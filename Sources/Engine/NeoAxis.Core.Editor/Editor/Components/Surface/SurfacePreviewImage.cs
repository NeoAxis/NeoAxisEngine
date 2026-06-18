// Copyright 2006–2026 Ivan Efimov. All rights reserved.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class SurfacePreviewImage : PreviewImageGenerator
	{
		public SurfacePreviewImage()
		{
		}

		protected override void OnUpdate()
		{
			var surface = ObjectOfPreview as Surface;

			if( surface != null )
			{
				var scene = CreateScene( false );
				SurfaceEditorUtility.CreatePreviewObjects( scene, surface );
				scene.Enabled = true;
			}

			var distanceScale = 1.6;

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), distanceScale );

			if( surface != null && surface.EditorCameraTransform != null )
			{
				var tr = surface.EditorCameraTransform;
				CameraDistance = ( tr.Position - CameraLookTo ).Length();// * distanceScale;
				CameraDirection = SphericalDirection.FromVector( CameraLookTo - tr.Position );
			}
		}
	}
}