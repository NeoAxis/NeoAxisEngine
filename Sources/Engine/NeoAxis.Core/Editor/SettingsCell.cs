#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using Internal.ComponentFactory.Krypton.Toolkit;
using Internal.ComponentFactory.Krypton.Navigator;
using Internal.ComponentFactory.Krypton.Workspace;
using Internal.ComponentFactory.Krypton.Docking;
using System.Linq;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class SettingsCell : EUserControl
	{
		float cellsSortingPriority;
		SizeType sizeType = SizeType.Percent;

		//

		public SettingsCell()
		{
			InitializeComponent();
		}

		internal virtual void PerformInit()
		{
		}

		[Browsable( false )]
		public SettingsProvider Provider
		{
			get
			{
				var p = Parent as TableLayoutPanel;
				if( p != null )
					return p.Tag as SettingsProvider;
				else
					return null;
			}
		}

		public virtual float CellsSortingPriority
		{
			get { return cellsSortingPriority; }
			set { cellsSortingPriority = value; }
		}

		public SizeType SizeType
		{
			get { return sizeType; }
			set { sizeType = value; }
		}

		public T[] GetObjects<T>() where T : class
		{
			return Provider.SelectedObjects.OfType<T>().ToArray();
		}

		public T GetFirstObject<T>() where T : class
		{
			foreach( var obj in Provider.SelectedObjects )
			{
				var obj2 = obj as T;
				if( obj2 != null )
					return obj2;
			}
			return null;
		}
	}
}

#endif