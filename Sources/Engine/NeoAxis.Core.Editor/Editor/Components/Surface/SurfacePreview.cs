#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class SurfacePreview : CanvasBasedPreview
	{
		double lastUpdateTime;

		//

		public Surface Surface
		{
			get { return ObjectOfPreview as Surface; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			//create scene
			if( Surface != null )
			{
				var scene = CreateScene( false );
				CreateObjects( scene );
				scene.Enabled = true;
			}

			SetCameraByBounds( Scene.CalculateTotalBoundsOfObjectsInSpace(), 0.5 );

			if( Surface != null && Surface.EditorCameraTransform != null )
			{
				var tr = Surface.EditorCameraTransform;
				CameraInitialDistance = ( tr.Position - CameraLookTo ).Length() * 1.3;
				CameraDirection = SphericalDirection.FromVector( CameraLookTo - tr.Position );
			}

			//var bounds = Scene.CalculateTotalBoundsOfObjectsInSpace();
			//var bounds2 = new Bounds( bounds.GetCenter() );
			//bounds2.Expand( bounds.GetSize() / 4 );
			//SetCameraByBounds( bounds2 );
		}

		protected override void OnTick( float delta )
		{
			base.OnTick( delta );

			//update scene
			if( Time.Current > lastUpdateTime + 1.0 ) // 0.1 )
			{
				if( Surface != null && Scene != null )
					CreateObjects( Scene );
			}
		}

		//protected override void OnViewportUpdateBeforeOutput()
		//{
		//	base.OnViewportUpdateBeforeOutput();

		////update scene
		//if( Time.Current > lastUpdateTime + 0.1 )
		//{
		//	if( Surface != null && Scene != null )
		//		CreateObjects( Scene );
		//}
		//}

		void CreateObjects( Scene scene )
		{
			SurfaceEditorUtility.CreatePreviewObjects( scene, Surface );
			lastUpdateTime = Time.Current;
		}
	}
}

#endif