// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_CSharpScript_PreviewControl : PreviewControlWithViewport
	{
		ScriptPrinter scriptPrinter;
		Component_Image previewTexture;

		public Component_CSharpScript_PreviewControl()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			//var scene = CreateScene( false );
			//scene.Enabled = true;

			var script = ObjectForPreview as Component_CSharpScript;
			script.CodeChanged += Script_CodeChanged;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			//!!!!:  Is this the right place to unsubscribe from an event? it seems it is very rarely called

			var script = ObjectForPreview as Component_CSharpScript;
			if( script != null )
				script.CodeChanged -= Script_CodeChanged;
		}

		private void Script_CodeChanged( Component_CSharpScript obj )
		{
			if( obj.DisableUpdate )
				return;

			if( previewTexture != null )
				previewTexture.Dispose();
			previewTexture = null;
		}

		protected override void Viewport_UpdateBeforeOutput( Viewport viewport )
		{
			base.Viewport_UpdateBeforeOutput( viewport );

			if( previewTexture == null || previewTexture.Result.ResultSize != viewport.SizeInPixels )
			{
				if( previewTexture != null )
					previewTexture.Dispose();

				if( scriptPrinter == null )
					scriptPrinter = new ScriptPrinter();

				var script = ObjectForPreview as Component_CSharpScript;
				previewTexture = scriptPrinter.PrintToTexture( script.Code, new Vector2I( viewport.SizeInPixels.X, viewport.SizeInPixels.Y ) );
			}

			var renderer = viewport.CanvasRenderer;
			renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new Rectangle( 0, 0, 1, 1 ), previewTexture );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			var script = ObjectForPreview as Component_CSharpScript;
			if( script != null )
			{
				var info = Component_CSharpScript_SettingsCell.GetInfo( script );
				if( info == null )
					lines.Add( "No data" );

				//if( info != null )
				//	lines.AddRange( info.Split( '\n' ) );
				//else
				//	lines.Add( "No data" );
			}
		}
	}
}
