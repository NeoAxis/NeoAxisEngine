#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NeoAxis.Editor
{
	public partial class NewResourceType_CSharpClassLibrary_Cell : NewObjectCell
	{
		public NewResourceType_CSharpClassLibrary_Cell()
		{
			InitializeComponent();
		}

		public override bool ReadyToCreate( out string reason )
		{
			if( !base.ReadyToCreate( out reason ) )
				return false;

			return true;
		}

		public override bool ObjectCreation( ObjectCreationContext context )
		{
			if( !base.ObjectCreation( context ) )
				return false;

			//!!!!

			//try
			//{
			//	File.WriteAllText( context.fileCreationRealFileName, "" );
			//}
			//catch( Exception e )
			//{
			//	Log.Warning( e.Message );
			//	return false;
			//}

			return true;
		}
	}
}

#endif