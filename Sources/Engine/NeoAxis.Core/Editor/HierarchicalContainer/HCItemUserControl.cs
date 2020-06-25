// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Represents a user control item for <see cref="HierarchicalContainer"/>.
	/// </summary>
	public class HCItemUserControl : HCItemProperty
	{
		public HCItemUserControl( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override UserControl CreateControlInsidePropertyItemControl()
		{
			var userControl = new UserControl();
			userControl.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			return userControl;
		}

		//object currentValue;

		//public override void UpdateControl()
		//{
		//	base.UpdateControl();

		//	var userControl = (HCItemUserControl_Control)CreatedControlInsidePropertyItemControl;
		//	userControl.ReadOnly = !CanEditValue();

		//	var values = GetValues();
		//	if( values == null )
		//		return;

		//	//!!!!multiselection
		//	var value = values[ 0 ];

		//	object unrefValue = ReferenceUtility.GetUnreferencedValue( value );
		//	if( !object.Equals( currentValue, unrefValue ) )
		//	{
		//		OnValueChanged( currentValue, unrefValue );
		//		currentValue = unrefValue;
		//	}
		//}

		//public virtual void OnValueChanged( object oldValue, object newValue )
		//{
		//	var userControl = (HCItemUserControl_Control)CreatedControlInsidePropertyItemControl;
		//	userControl.OnPropertyValueChanged( oldValue, newValue );
		//}
	}
}