// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class SpritePreview : ObjectInSpacePreview
	{
		double lastUpdateTime;
		Sprite instanceInScene;

		//

		public SpritePreview()
		{
		}

		public Sprite Sprite
		{
			get { return ObjectOfPreview as Sprite; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			if( Sprite != null && Sprite.ParentScene == null )//show only when not in a scene
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), mode2D: true );
			}
		}

		protected override void OnTick( float delta )
		{
			base.OnTick( delta );

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

			instanceInScene = (Sprite)Sprite.Clone();
			Scene.AddComponent( instanceInScene );

			//var type = Sprite.GetProvidedType();
			//if( type != null )
			//{
			//	instanceInScene = (Sprite)Scene.CreateComponent( type );
			//}
		}
	}
}
