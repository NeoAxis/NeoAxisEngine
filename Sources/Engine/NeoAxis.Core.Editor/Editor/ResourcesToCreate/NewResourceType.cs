#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace NeoAxis.Editor
{
	public class NewResourceType
	{
	}

	/// <summary>
	/// Create a new text file.
	/// </summary>
	[NewObjectCell( typeof( NewResourceType_TextFile_Cell ) )]
	[ResourceFileExtension( "txt" )]
	public class NewResourceType_TextFile : NewResourceType
	{
	}

	//!!!!nb
	//[EditorNewObjectCell( typeof( NewResourceType_CSharpClassLibrary_Cell ) )]
	/// <summary>
	/// Create a new C# class library. The feature is not implemented.
	/// </summary>
	[ResourceFileExtension( "csproj" )]//!!!!
	public class NewResourceType_CSharpClassLibrary : NewResourceType
	{
	}

	//!!!!nb
	//[EditorNewObjectCell( typeof( NewResourceType_CSharpClassLibrary_Cell ) )]
	//[ResourceFileExtension( "csproj" )]//!!!!
	/// <summary>
	/// Create a new executable module (C# app). The feature is not implemented.
	/// </summary>
	public class NewResourceType_ExecutableApp : NewResourceType
	{
	}

	//[EditorNewObjectCell( typeof( NewResourceType_FlowGraphLibrary_Cell ) )]
	//[ResourceFileExtension( "component" )]
	//public class NewResourceType_FlowGraphLibrary : NewResourceType
	//{
	//}
}

#endif