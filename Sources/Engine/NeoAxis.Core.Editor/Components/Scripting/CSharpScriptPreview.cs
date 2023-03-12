// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class CSharpScriptPreview : PreviewControlWithViewport
	{
		ScriptPrinter scriptPrinter;
		ImageComponent previewTexture;

		public CSharpScriptPreview()
		{
			InitializeComponent();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			//var scene = CreateScene( false );
			//scene.Enabled = true;

			var script = ObjectOfPreview as CSharpScript;
			script.CodeChanged += Script_CodeChanged;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			//!!!!:  Is this the right place to unsubscribe from an event? it seems it is very rarely called

			var script = ObjectOfPreview as CSharpScript;
			if( script != null )
				script.CodeChanged -= Script_CodeChanged;
		}

		private void Script_CodeChanged( CSharpScript obj )
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

				var script = ObjectOfPreview as CSharpScript;
				previewTexture = scriptPrinter.PrintToTexture( script.Code, new Vector2I( viewport.SizeInPixels.X, viewport.SizeInPixels.Y ) );
			}

			var renderer = viewport.CanvasRenderer;
			renderer.AddQuad( new Rectangle( 0, 0, 1, 1 ), new Rectangle( 0, 0, 1, 1 ), previewTexture );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			var script = ObjectOfPreview as CSharpScript;
			if( script != null )
			{
				var info = CSharpScriptSettingsCell.GetInfo( script );
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
