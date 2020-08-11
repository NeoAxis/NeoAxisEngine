// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using NeoAxis.Editor;

namespace NeoAxis.Addon.ExampleEditorEngineGUI
{
	public class ExampleEditorEngineGUIEditor : CanvasBasedEditor
	{
		UIControl control;

		//

		public ExampleEditorEngineGUIEditor()
		{
		}

		public Component_ExampleEditorEngineGUI ExampleEditorEngineGUI
		{
			get { return ObjectOfEditor as Component_ExampleEditorEngineGUI; }
		}

		protected override void OnViewportCreated()
		{
			base.OnViewportCreated();

			control = ResourceManager.LoadSeparateInstance<UIControl>( @"Samples\Addons\Example Editor Engine GUI.ui", false, true );
			if( control != null )
			{
				Viewport.UIContainer.AddComponent( control );

				var button = control.GetComponent<UIButton>( "Button" );
				if( button != null )
					button.Click += Button_Click;
			}
		}

		protected override void OnViewportDestroyed()
		{
			if( control != null )
			{
				control.Dispose();
				control = null;
			}

			base.OnViewportDestroyed();
		}

		protected override void OnViewportUpdateBeforeOutput()
		{
			base.OnViewportUpdateBeforeOutput();

			//draw background
			Viewport.CanvasRenderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new ColorValue( 0.2, 0.2, 0.2 ) );

			//draw GUI
			Viewport.UIContainer.PerformRenderUI( Viewport.CanvasRenderer );
		}

		protected override void OnTick( float delta )
		{
			base.OnTick( delta );

			if( control != null )
			{
				var edit = control.GetComponent<UIEdit>( "Edit" );
				if( edit != null )
				{
					edit.Text = ExampleEditorEngineGUI.ValueToDisplay;
				}
			}
		}

		private void Button_Click( UIButton sender )
		{
			EditorMessageBox.ShowInfo( "Editor message box." );
		}
	}
}
