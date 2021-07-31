// Copyright (C) 2021 NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Threading;

namespace NeoAxis
{
	/// <summary>
	/// Class for working with the settings file of <see cref="Component_Image"/>.
	/// </summary>
	public static class Component_Image_Settings
	{
		public static string GetParameter( string virtualTextureFileName, string parameterName, out string error )
		{
			string fileName = virtualTextureFileName + ".settings";

			if( !VirtualFile.Exists( fileName ) )
			{
				error = "";
				return "";
			}

			var block = TextBlockUtility.LoadFromVirtualFile( fileName, out error );
			if( block == null )
				return "";

			error = "";
			return block.GetAttribute( parameterName );
		}

		public static bool SetParameter( string virtualTextureFileName, string parameterName, string parameterValue, out string error )
		{
			string fileName = virtualTextureFileName + ".settings";

			if( !VirtualFile.Exists( fileName ) && string.IsNullOrEmpty( parameterValue ) )
			{
				//no sense to update
				error = "";
				return true;
			}

			TextBlock block = null;
			if( VirtualFile.Exists( fileName ) )
			{
				block = TextBlockUtility.LoadFromVirtualFile( fileName, out error );
				if( block == null )
					return false;
			}
			else
				block = new TextBlock();

			if( !string.IsNullOrEmpty( parameterValue ) )
				block.SetAttribute( parameterName, parameterValue );
			else
				block.DeleteAttribute( parameterName );

			if( block.Children.Count != 0 || block.Attributes.Count != 0 )
			{
				if( !TextBlockUtility.SaveToVirtualFile( block, fileName, out error ) )
					return false;
			}
			else
			{
				if( VirtualFile.Exists( fileName ) )
				{
					try
					{
						File.Delete( VirtualPathUtility.GetRealPathByVirtual( fileName ) );
					}
					catch( Exception e )
					{
						error = e.Message;
						return false;
					}
				}
			}

			error = "";
			return true;
		}
	}
}
