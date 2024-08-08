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
			return EditorLocalization2.Translate( "ProductEditor", text );
		}

		protected override void OnGetTextInfoLeftTopCorner( List<string> lines )
		{
			base.OnGetTextInfoLeftTopCorner( lines );

			if( Product != null )
			{
				var name = Product.GetName();
				if( !string.IsNullOrEmpty( name ) )
					lines.Add( name );
				else
					lines.Add( "{NO NAME}" );
				lines.Add( "" );
				lines.Add( "Platform: " + Product.Platform.ToString() );

				var productStore = Product as Product_Store;
				if( productStore != null )
				{
					lines.Add( "Product Type: " + productStore.ProductType.Value.ToString() );

					var categories = productStore.ProjectItemCategories.Value.ToString();
					if( productStore.ProjectItemCategories.Value == Product_Store.ProjectItemCategoriesEnum.Uncategorized )
						categories = categories.ToUpper() + " - " + Translate( "None of the categories are selected." ).ToUpper();
					lines.Add( "Categories: " + categories );
					//lines.Add( "Categories: " + productStore.ProjectItemCategories.Value.ToString() );

					var license = productStore.License.Value.ToString();
					if( productStore.License.Value == StoreProductLicense.None )
						license = license.ToUpper() + " - " + Translate( "The license is not specified." ).ToUpper();
					lines.Add( "License: " + license );
					//lines.Add( "License: " + productStore.License.Value.ToString() );

					lines.Add( "Identifier: " + productStore.GetIdentifier() );
					lines.Add( "" );

					lines.Add( "Description:" );
					lines.AddRange( productStore.Description.Value.Trim().Replace( "\r", "" ).Split( new char[] { '\n' } ) );
					lines.Add( "" );

					//lines.Add( "Short description:" );
					//lines.AddRange( productStore.ShortDescription.Value.Trim().Replace( "\r", "" ).Split( new char[] { '\n' } ) );
					//lines.Add( "" );

					//lines.Add( "Full description:" );
					//lines.AddRange( productStore.FullDescription.Value.Trim().Replace( "\r", "" ).Split( new char[] { '\n' } ) );
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
