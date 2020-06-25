// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using ComponentFactory.Krypton.Workspace;
using ComponentFactory.Krypton.Docking;
using NeoAxis;
using System.ComponentModel;

namespace NeoAxis.Editor
{
	public partial class NewObjectCell : EUserControl
	{
		//!!!!надо ли
		float cellsSortingPriority;

		//

		public NewObjectCell()
		{
			InitializeComponent();
		}

		//!!!!!
		//[Browsable( false )]
		//public SettingsProvider Provider
		//{
		//	get
		//	{
		//		var p = Parent as TableLayoutPanel;
		//		if( p != null )
		//			return p.Tag as SettingsProvider;
		//		else
		//			return null;
		//	}
		//}

		public virtual float CellsSortingPriority
		{
			get { return cellsSortingPriority; }
			set { cellsSortingPriority = value; }
		}

		public virtual bool ReadyToCreate( out string reason )
		{
			reason = "";
			return true;
		}

		public class ObjectCreationContext
		{
			public object newObject;
			public string fileCreationRealFileName;
			public bool disableFileCreation;
		}
		public virtual bool ObjectCreation( ObjectCreationContext context )
		//public virtual bool ObjectCreation( object newObject, string fileCreationRealFileName, ref bool disableFileWriting )
		{
			return true;
		}
	}
}
