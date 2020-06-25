// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace NeoAxis.Editor
{
	public interface IHCCollection
	{
		HCKryptonTextBox Label2 { get; }
		ComponentFactory.Krypton.Toolkit.KryptonButton ButtonEdit { get; }
	}

	public class HCItemCollection : HCItemProperty
	{
		public HCItemCollection( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override UserControl CreateControlInsidePropertyItemControl()
		{
			return new HCGridCollection();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			//!!!!так?
			//subscribe for context menu
			var control = (IHCCollection)CreatedControlInsidePropertyItemControl;
			control.Label2.MouseUp += Control_MouseUp_ResetDefaultValue;

			//control.ButtonEdit.Click += ButtonEdit_Click;
		}

		public virtual string GetValueText( object value )
		{
			if( value.GetType().IsArray )
			{
				int length = (int)value.GetType().GetProperty( "Length" ).GetValue( value, null );
				return $"Length: {length}";
			}
			else
			{
				int count = (int)value.GetType().GetProperty( "Count" ).GetValue( value, null );
				return $"Count: {count}";
			}
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCCollection)CreatedControlInsidePropertyItemControl;
			var values = GetValues();
			if( values == null )
				return;

			var valueResult = "";

			for( int nValue = 0; nValue < values.Length; nValue++ )
			{
				var value = values[ nValue ];
				var unrefValue = ReferenceUtility.GetUnreferencedValue( value );
				string value2;
				if( unrefValue != null )
					value2 = GetValueText( unrefValue );
				else
					value2 = "(Null)";

				if( nValue == 0 )
					valueResult = value2;
				else
				{
					if( valueResult != value2 )
						valueResult = "";
				}
			}

			if( control.Label2 != null )
				control.Label2.Text = valueResult;
		}

		//private void ButtonEdit_Click( object sender, EventArgs e )
		//{
		//	if( Owner.DocumentWindow != null )
		//	{
		//		var values = GetValues();

		//		//!!!!multiselection
		//		var value = values[ 0 ];

		//		var unrefValue = ReferenceUtils.GetUnreferencedValue( value );

		//		EditorForm.Instance.ShowObjectSettingsWindow( Owner.DocumentWindow.Document, unrefValue, true );
		//	}
		//}
	}
}
