// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using NeoAxis;
using NeoAxis.Editor;

namespace CommandLineTools
{
	internal class Program
	{
		static void Main( string[] args )
		{
			if( Debugger.IsAttached )
			{
				Main2();
			}
			else
			{
				try
				{
					Main2();
				}
				catch( Exception e )
				{
					Log.FatalAsException( e.ToString() );
				}
			}
		}

		static void Main2()
		{
			if( EditorCommandLineTools.Process() )
				return;

			Console.WriteLine( "ERROR: Invalid parameters." );
		}
	}
}