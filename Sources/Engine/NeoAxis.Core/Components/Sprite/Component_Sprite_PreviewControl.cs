// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_Sprite_PreviewControl : Component_ObjectInSpace_PreviewControl
	{
		double lastUpdateTime;
		Component_Sprite instanceInScene;

		//

		public Component_Sprite_PreviewControl()
		{
			InitializeComponent();
		}

		public Component_Sprite Sprite
		{
			get { return ObjectForPreview as Component_Sprite; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			if( Sprite != null && Sprite.ParentScene == null )//show only when not in a scene
			{
				var scene = CreateScene( false );
				CreateObject();
				scene.Enabled = true;

				SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace(), mode2D: true );
			}
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

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

			instanceInScene = (Component_Sprite)Sprite.Clone();
			Scene.AddComponent( instanceInScene );

			//var type = Sprite.GetProvidedType();
			//if( type != null )
			//{
			//	instanceInScene = (Component_Sprite)Scene.CreateComponent( type );
			//}
		}
	}
}
