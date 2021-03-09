// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class Component_StoreProduct_DocumentWindow : DocumentWindowWithViewport
	{
		public Component_StoreProduct_DocumentWindow()
		{
			InitializeComponent();
		}

		public Component_StoreProduct Product
		{
			get { return (Component_StoreProduct)ObjectOfWindow; }
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			CreateScene( true );

			if( ObjectOfWindow != null )
				SelectObjects( new object[] { ObjectOfWindow } );
		}

		static string Translate( string text )
		{
			return EditorLocalization.Translate( "StoreProductDocumentWindow", text );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			base.GetTextInfoLeftTopCorner( lines );

			if( Product != null )
			{
				if( !string.IsNullOrEmpty( Product.Title ) )
					lines.Add( Product.Title );
				else
					lines.Add( "{NO TITLE}" );
				lines.Add( "" );

				lines.Add( "License: " + Product.License.Value.ToString() );
				lines.Add( "Identifier: " + Product.GetIdentifier() );
				lines.Add( "" );

				lines.Add( "Short description:" );
				lines.AddRange( Product.ShortDescription.Value.Trim().Replace( "\r", "" ).Split( new char[] { '\n' } ) );
				lines.Add( "" );

				lines.Add( "Full description:" );
				lines.AddRange( Product.FullDescription.Value.Trim().Replace( "\r", "" ).Split( new char[] { '\n' } ) );
			}
		}
	}
}
