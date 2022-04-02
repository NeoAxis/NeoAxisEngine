// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Win32;
using NeoAxis.Editor;

namespace NeoAxis
{
	public static class SketchfabLogin
	{
		const string registryPath = @"SOFTWARE\NeoAxis";

		//

		public static void Init()
		{
			//Add login properties to the Store window options
			ContentBrowserOptions.Configure += ContentBrowserOptions_Configure;
		}

		static private void ContentBrowserOptions_Configure( ContentBrowserOptions sender )
		{
			if( sender is StoresWindow.StoresContentBrowserOptions )
			{
				LoadFromRegistry( out var username, out var password, out var prepareStoreProduct );

				{
					var attributes = new List<Attribute>();
					attributes.Add( new DisplayNameAttribute( "Sketchfab Username" ) );

					var property = new ContentBrowserOptions.PropertyImpl( sender, "SketchfabUsername", MetadataManager.GetTypeOfNetType( typeof( string ) ), attributes, "Sketchfab", "" );
					property.DefaultValueSpecified = true;
					property.DefaultValue = "";

					property.Value = username;

					property.ValueChanged += delegate ( ContentBrowserOptions.PropertyImpl sender )
					{
						Save( (ContentBrowserOptions)sender.Owner );
					};

					sender.AddProperty( property );
				}

				{
					var attributes = new List<Attribute>();
					attributes.Add( new DisplayNameAttribute( "Sketchfab Password" ) );

					var property = new ContentBrowserOptions.PropertyImpl( sender, "SketchfabPassword", MetadataManager.GetTypeOfNetType( typeof( string ) ), attributes, "Sketchfab", "" );
					property.DefaultValueSpecified = true;
					property.DefaultValue = "";
					property.Attributes.Add( new HCTextBoxPasswordAttribute() );

					property.Value = password;

					property.ValueChanged += delegate ( ContentBrowserOptions.PropertyImpl sender )
					{
						Save( (ContentBrowserOptions)sender.Owner );
					};

					sender.AddProperty( property );
				}

				{
					var attributes = new List<Attribute>();
					attributes.Add( new DisplayNameAttribute( "Sketchfab Product for Store" ) );

					var property = new ContentBrowserOptions.PropertyImpl( sender, "SketchfabPrepareStoreProduct", MetadataManager.GetTypeOfNetType( typeof( bool ) ), attributes, "Sketchfab", "" );
					property.DefaultValueSpecified = true;
					property.DefaultValue = false;
					property.Attributes.Add( new HCTextBoxPasswordAttribute() );

					property.Attributes.Add( new DescriptionAttribute( "Whether to prepare a product file for upload to NeoAxis Store." ) );

					property.Value = prepareStoreProduct;

					property.ValueChanged += delegate ( ContentBrowserOptions.PropertyImpl sender )
					{
						Save( (ContentBrowserOptions)sender.Owner );
					};

					sender.AddProperty( property );
				}
			}
		}

		public static bool LoadFromRegistry( out string username, out string hash, out bool prepareStoreProduct )
		{
			try
			{
				//opening the subkey  
				RegistryKey key = Registry.CurrentUser.OpenSubKey( registryPath );

				//if it does exist, retrieve the stored values
				if( key != null )
				{
					username = ( key.GetValue( "SketchfabUsername" ) ?? "" ).ToString();

					var p = key.GetValue( "SketchfabHash" );
					if( p != null )
						hash = EncryptDecrypt( p.ToString() );
					else
						hash = "";

					prepareStoreProduct = false;
					{
						var v = key.GetValue( "SketchfabPrepareStoreProduct", false );
						if( v != null )
						{
							if( v is bool )
								prepareStoreProduct = (bool)v;
							else
								bool.TryParse( v.ToString(), out prepareStoreProduct );
						}
					}

					key.Close();
					return true;
				}
			}
			catch { }

			username = "";
			hash = "";
			prepareStoreProduct = false;
			return false;
		}

		static string EncryptDecrypt( string input )
		{
			char[] key = { 'K', 'C', 'Q' }; //Any chars will work, in an array of any size
			char[] output = new char[ input.Length ];

			for( int i = 0; i < input.Length; i++ )
				output[ i ] = (char)( input[ i ] ^ key[ i % key.Length ] );

			return new string( output );
		}

		public static void SaveToRegistry( string email, string hash, bool prepareProductStore )
		{
			try
			{
				var key = Registry.CurrentUser.CreateSubKey( registryPath );

				key.SetValue( "SketchfabUsername", email );
				key.SetValue( "SketchfabHash", EncryptDecrypt( hash ) );
				key.SetValue( "SketchfabPrepareStoreProduct", prepareProductStore );

				key.Close();
			}
			catch( Exception e )
			{
				EditorMessageBox.ShowWarning( e.Message );
				return;
			}
		}

		static void Save( ContentBrowserOptions options )
		{
			var username = (string)( (ContentBrowserOptions.PropertyImpl)options.MetadataGetMemberBySignature( "property:SketchfabUsername" ) ).Value;
			var password = (string)( (ContentBrowserOptions.PropertyImpl)options.MetadataGetMemberBySignature( "property:SketchfabPassword" ) ).Value;
			var prepareProductStore = (bool)( (ContentBrowserOptions.PropertyImpl)options.MetadataGetMemberBySignature( "property:SketchfabPrepareStoreProduct" ) ).Value;

			SaveToRegistry( username, password, prepareProductStore );
		}

	}
}
