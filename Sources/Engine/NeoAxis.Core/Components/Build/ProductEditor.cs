// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
#if !DEPLOY
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public partial class ProductEditor : CanvasBasedEditor
	{
		public ProductEditor()
		{
		}

		public Product Product
		{
			get { return (Product)ObjectOfEditor; }
		}

		protected override void OnCreate()
		{
			base.OnCreate();

			CreateScene( true );

			if( Product != null )
				SelectObjects( new object[] { Product } );
		}

		static string Translate( string text )
		{
			return EditorLocalization.Translate( "ProductEditor", text );
		}

		protected override void OnGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.OnGetTextInfoLeftTopCorner( lines );

			if( Product != null )
			{
				if( !string.IsNullOrEmpty( Product.Name ) )
					lines.Add( Product.Name );
				else
					lines.Add( "{NO NAME}" );
				lines.Add( "" );
				lines.Add( "Platform: " + Product.Platform.ToString() );

				var productStore = Product as Product_Store;
				if( productStore != null )
				{
					lines.Add( "Product Type: " + productStore.ProductType.Value.ToString() );
					lines.Add( "Categories: " + productStore.ProjectItemCategories.Value.ToString() );

					lines.Add( "License: " + productStore.License.Value.ToString() );
					lines.Add( "Identifier: " + productStore.GetIdentifier() );
					lines.Add( "" );

					lines.Add( "Short description:" );
					lines.AddRange( productStore.ShortDescription.Value.Trim().Replace( "\r", "" ).Split( new char[] { '\n' } ) );
					lines.Add( "" );

					lines.Add( "Full description:" );
					lines.AddRange( productStore.FullDescription.Value.Trim().Replace( "\r", "" ).Split( new char[] { '\n' } ) );
				}


				//can be slowly

				//lines.Add( "" );

				//var paths = Product.GetPaths();
				//lines.Add( "Paths:" );
				//foreach( var path in paths )
				//	lines.Add( "- " + path );
			}
		}
	}
}
#endif
