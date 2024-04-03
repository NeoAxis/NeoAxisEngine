#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;

namespace NeoAxis.Editor
{
	public class ContentBrowserFilteringMode_AllFiles : ContentBrowserFilteringMode
	{
		public ContentBrowserFilteringMode_AllFiles()
		{
		}

		public override string Name
		{
			get { return "All Files"; }
		}

		public override bool AddGroupsBaseTypesAddonsProject
		{
			get { return true; }
		}

		public override bool AddGroupFavorites
		{
			get { return true; }
		}

		public override bool AddGroupAllTypes
		{
			get { return true; }
		}

		public override bool AddFiles
		{
			get { return true; }
		}

		public override bool AddAllFiles
		{
			get { return true; }
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class EditorExtensions_AddFiles : EditorExtensions
	{
		public override void OnRegister()
		{
			ContentBrowser.FilteringModes.Add( new ContentBrowserFilteringMode_AllFiles() );
		}
	}
}
#endif