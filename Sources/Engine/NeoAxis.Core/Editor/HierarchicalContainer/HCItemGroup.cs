// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public interface IHCGroup
	{
		Label Label1 { get; }
	}

	/// <summary>
	/// Represents a group item for <see cref="HierarchicalContainer"/>.
	/// </summary>
	public class HCItemGroup : HierarchicalContainer.Item
	{
		string name;

		public HCItemGroup( HierarchicalContainer owner, HierarchicalContainer.Item parent, string name )
			: base( owner, parent )
		{
			this.name = name;
		}

		public override EUserControl CreateControlImpl()
		{
			//if( Owner.GridMode )
			return new HCGridGroup();
			//else
			//	return new HCFormGroup();
		}

		public string Name
		{
			get { return name; }
		}

		public override void UpdateControl()
		{
			var control = (IHCGroup)CreatedControl;

			if( control.Label1 != null )
			{
				var text = name;
				Owner.PerformOverrideGroupDisplayName( this, ref text );
				control.Label1.Text = text;
			}
		}
	}
}
