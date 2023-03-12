#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
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
	public interface IHCCheckBox
	{
		Internal.ComponentFactory.Krypton.Toolkit.KryptonCheckBox CheckBox1 { get; }
		bool CheckBox1SetText { get; }
	}

	/// <summary>
	/// Represents a check box item for <see cref="HierarchicalContainer"/>.
	/// </summary>
	public class HCItemCheckBox : HCItemProperty
	{
		bool allowChangedEvent = true;

		//

		public HCItemCheckBox( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			//if( Owner.GridMode )
			return new HCGridCheckBox();
			//else
			//	return new HCFormCheckBox();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var control = (IHCCheckBox)CreatedControlInsidePropertyItemControl;
			control.CheckBox1.CheckedChanged += CheckBox1_CheckedChanged;
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCCheckBox)CreatedControlInsidePropertyItemControl;
			var values = GetValues();
			if( values == null )
				return;

			bool existsFalse = false;
			bool existsTrue = false;
			foreach( var v in values )
			{
				bool boolV = (bool)ReferenceUtility.GetUnreferencedValue( v );
				if( !boolV )
					existsFalse = true;
				if( boolV )
					existsTrue = true;
			}

			string text = Property.Name;
			//{
			//	object[] attribs = Property.GetCustomAttributes( typeof( DescriptionAttribute ), true );
			//	if( attribs.Length != 0 )
			//	{
			//		var a = (DescriptionAttribute)attribs[ 0 ];
			//		text = a.Description;
			//	}
			//}

			if( control.CheckBox1SetText )
				control.CheckBox1.Text = text;
			else
				control.CheckBox1.Text = "";

			control.CheckBox1.Enabled = CanEditValue();

			allowChangedEvent = false;

			if( existsFalse && existsTrue )
			{
				control.CheckBox1.ThreeState = true;
				control.CheckBox1.CheckState = CheckState.Indeterminate;
			}
			else
			{
				control.CheckBox1.ThreeState = false;
				control.CheckBox1.CheckState = existsTrue ? CheckState.Checked : CheckState.Unchecked;
			}

			allowChangedEvent = true;
		}

		private void CheckBox1_CheckedChanged( object sender, EventArgs e )
		{
			if( !allowChangedEvent )
				return;

			var control = (IHCCheckBox)CreatedControlInsidePropertyItemControl;
			object v = control.CheckBox1.Checked;

			SetValue( v, true );
		}
	}
}

#endif