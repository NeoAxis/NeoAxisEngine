// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a type member for <see cref="HierarchicalContainer"/>.
	/// </summary>
	public abstract class HCItemMember : HierarchicalContainer.Item
	{
		object[] controlledObjects;

		public HCItemMember( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects )
			: base( owner, parent )
		{
			this.controlledObjects = controlledObjects;
		}

		public object[] ControlledObjects
		{
			get { return controlledObjects; }
			set { controlledObjects = value; }
		}

		public abstract Metadata.Member Member
		{
			get;
		}

		public T GetOneControlledObject<T>() where T : class
		{
			if( ControlledObjects.Length == 1 )
			{
				var c = ControlledObjects[ 0 ] as T;
				if( c != null )
					return c;
			}
			return null;
		}
	}
}
