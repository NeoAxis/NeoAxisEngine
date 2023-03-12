// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
//#if CLOUD
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace NeoAxis.Networking
{
	public static class RepositorySettingsFile
	{
		public class Settings
		{
			public ESet<string> IgnoreFolders = new ESet<string>();
		}

		///////////////////////////////////////////////

		public static bool Load( string realFilePath, out Settings settings, out string error )
		{
			settings = null;
			error = "";

			if( !File.Exists( realFilePath ) )
			{
				error = "The file is not exists.";
				return false;
			}

			var block = TextBlockUtility.LoadFromRealFile( realFilePath, out error );
			if( !string.IsNullOrEmpty( error ) )
				return false;

			var result = new Settings();

			try
			{
				foreach( var itemBlock in block.Children )
				{
					if( itemBlock.Name == "IgnoreFolder" )
					{
						var fileName = itemBlock.GetAttribute( "FileName" );
						if( !string.IsNullOrEmpty( fileName ) )
							result.IgnoreFolders.AddWithCheckAlreadyContained( fileName );
					}
				}
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			settings = result;
			return true;
		}

		public static bool Save( string realFilePath, Settings settings, out string error )
		{
			error = "";

			var rootBlock = new TextBlock();

			foreach( var ignoreFolder in settings.IgnoreFolders )
			{
				var block = rootBlock.AddChild( "IgnoreFolder" );
				block.SetAttribute( "FileName", ignoreFolder );
			}

			try
			{
				File.WriteAllText( realFilePath, rootBlock.DumpToString() );
			}
			catch( Exception e )
			{
				error = e.Message;
				return false;
			}

			return true;
		}
	}
}
//#endif