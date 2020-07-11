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
	public abstract class ContentBrowserFilteringMode
	{
		public abstract string Name
		{
			get;
		}

		public abstract bool AddGroupGeneral { get; }
		public abstract bool AddGroupsBaseTypesAddonsProject { get; }
		public abstract bool AddGroupsFavorites { get; }
		public abstract bool AddSolution { get; }
		public abstract bool AddGroupAllTypes { get; }
		public abstract bool AddFiles { get; }

		public virtual string[] FileSearchPatterns { get { return null; } }
		public virtual bool HideDirectoriesWithoutItems { get { return false; } }

		public virtual bool ExpandAllFileItemsAtStartup { get { return false; } }

		public virtual bool AddItem( ContentBrowser.Item item ) { return true; }
	}
}
