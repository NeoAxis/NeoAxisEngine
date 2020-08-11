// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class Component_Character2D_Preview : CanvasBasedPreview
	{
		double lastUpdateTime;
		Component_Character2D instanceInScene;

		//

		public Component_Character2D Character
		{
			get { return ObjectOfPreview as Component_Character2D; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( Character != null && Character.ParentScene == null )//show only when not in a scene
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), mode2D: true );
			}
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			//update
			if( instanceInScene != null && Time.Current > lastUpdateTime + 0.1 )
			{
				CreateObject();
				lastUpdateTime = Time.Current;
			}
		}

		void CreateObject()
		{
			instanceInScene?.Dispose();

			instanceInScene = (Component_Character2D)Character.Clone();
			Scene.AddComponent( instanceInScene );
		}
	}
}
