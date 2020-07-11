// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Text;

namespace NeoAxis.Editor
{
	public class ContentBrowserFilteringMode_CSharp : ContentBrowserFilteringMode
	{
		public ContentBrowserFilteringMode_CSharp()
		{
		}

		public override string Name
		{
			get { return "C#"; }
		}

		public override bool AddGroupGeneral
		{
			get { return false; }
		}

		public override bool AddGroupsBaseTypesAddonsProject
		{
			get { return false; }
		}

		public override bool AddGroupsFavorites
		{
			get { return false; }
		}

		public override bool AddSolution
		{
			get { return true; }
		}

		public override bool AddGroupAllTypes
		{
			get { return false; }
		}

		public override bool AddFiles
		{
			get { return true; }
		}

		public override string[] FileSearchPatterns
		{
			get { return new string[] { "*.cs" }; }
		}

		public override bool HideDirectoriesWithoutItems
		{
			get { return true; }
		}

		public override bool ExpandAllFileItemsAtStartup
		{
			get { return true; }
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	class EditorExtensions_CSharp : EditorExtensions
	{
		public override void Register()
		{
			ContentBrowser.FilteringModes.Add( new ContentBrowserFilteringMode_CSharp() );
		}
	}
}
