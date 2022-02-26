// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Text;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class CharacterPreview : CanvasBasedPreview
	{
		double lastUpdateTime;
		Character instanceInScene;

		//

		public Character Character
		{
			get { return ObjectOfPreview as Character; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( Character != null && Character.ParentScene == null )//show only when not in a scene
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace() );
			}
			else
			{
				ViewportControl.AllowCreateRenderWindow = false;
				ViewportControl.Visible = false;
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

			instanceInScene = (Character)Character.Clone();
			instanceInScene.SetTransform( Transform.Identity, true );
			Scene.AddComponent( instanceInScene );
		}
	}
}
#endif