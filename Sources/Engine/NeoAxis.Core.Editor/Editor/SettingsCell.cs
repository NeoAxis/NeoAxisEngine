#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class SettingsCell : EUserControl, ISettingsCell
	{
		float cellsSortingPriority;
		FormSizeType sizeType = FormSizeType.Percent;

		//

		public SettingsCell()
		{
			InitializeComponent();
		}

		internal virtual void PerformInit()
		{
		}

		[Browsable( false )]
		public SettingsProvider Provider2
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

		[Browsable( false )]
		public ISettingsProvider Provider
		{
			get { return Provider2; }
		}

		public virtual float CellsSortingPriority
		{
			get { return cellsSortingPriority; }
			set { cellsSortingPriority = value; }
		}

		public FormSizeType SizeType
		{
			get { return sizeType; }
			set { sizeType = value; }
		}

		public T[] GetObjects<T>() where T : class
		{
			return Provider2.SelectedObjects.OfType<T>().ToArray();
		}

		public T GetFirstObject<T>() where T : class
		{
			foreach( var obj in Provider2.SelectedObjects )
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