// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NeoAxis.Editor;
using System.Linq;
using SharpBgfx;
using ComponentFactory.Krypton.Toolkit;
using NeoAxis.Widget;

namespace NeoAxis.Editor
{
	public partial class Component_Product_DocumentWindow : DocumentWindowWithViewport
	{
		public Component_Product_DocumentWindow()
		{
			InitializeComponent();
		}

		public Component_Product Product
		{
			get { return (Component_Product)ObjectOfWindow; }
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
			return EditorLocalization.Translate( "ProductDocumentWindow", text );
		}

		protected override void GetTextInfoLeftTopCorner( List<string> lines )
		{
			base.GetTextInfoLeftTopCorner( lines );

			if( Product != null )
			{
				lines.Add( Product.ProductName );
				lines.Add( Product.Platform.ToString() );
				lines.Add( "" );

				var assets = Product.SelectedAssets.Value;

				if( string.IsNullOrEmpty( assets ) )
					lines.Add( "Assets: All" );
				else
				{
					lines.Add( "Assets:" );

					try
					{
						foreach( var asset in assets.Split( new char[] { ';' } ) )
						{
							var asset2 = asset.Trim();
							if( !string.IsNullOrEmpty( asset2 ) )
								lines.Add( "- " + asset2 );
						}
					}
					catch { }
				}
			}
		}

	}
}
