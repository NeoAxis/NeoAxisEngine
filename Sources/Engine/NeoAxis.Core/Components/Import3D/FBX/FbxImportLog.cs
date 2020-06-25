// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fbx;

namespace NeoAxis.Import.FBX
{
	//Temporary
	//ToDo : 
	static class FbxImportLog
	{
		public static void LogError( FbxNode node, string message )
		{
			//!!!!пока так

			Log.Info( "Import3D: Error: " + node.GetName() + " : " + message );
		}

		public static void LogWarning( FbxNode node, string message )
		{
			Log.Info( "Import3D: Warning: " + node.GetName() + " : " + message );
		}

		public static void LogMessage( FbxNode node, string message )
		{
			//Log.Info( "Import3D: Message: " + node.GetName() + " : " + message );
		}

		public static void LogError( string message )
		{
			Log.Info( "Import3D: Error: " + message );
		}

		public static void LogWarning( string message )
		{
			Log.Info( "Import3D: Warning: " + message );
		}

		public static void LogMessage( string message )
		{
			//Log.Info( "Import3D: Message: " + message );
		}
	}
}
