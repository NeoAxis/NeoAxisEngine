// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class Component_ObjectInSpace_Preview : CanvasBasedPreview
	{
		public Component_ObjectInSpace_Preview()
		{
		}

		[Browsable( false )]
		protected virtual bool EnableViewportControl
		{
			get
			{
				var objectInSpace = ObjectOfPreview as Component_ObjectInSpace;
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
				var objectInSpace = ObjectOfPreview as Component_ObjectInSpace;
				if( objectInSpace != null && objectInSpace.ParentScene == null )//show only if no scene
				{
					var scene = CreateScene( false );

					var type = objectInSpace.GetProvidedType();
					if( type != null )
					{
						var obj = (Component_ObjectInSpace)scene.CreateComponent( type );
						obj.Transform = Transform.Identity;
					}

					scene.Enabled = true;
					SetCameraByBounds( scene.CalculateTotalBoundsOfObjectsInSpace() );
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
