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
using System.Reflection;

namespace NeoAxis.Editor
{
	//TODO: make class global ?
	public static class EnumExtension
	{
		public static string GetValueDescription( object value )
		{
			FieldInfo fi = value.GetType().GetField( value.ToString() );
			if( fi == null )
				return string.Empty;
			var attr = fi.GetCustomAttribute<DescriptionAttribute>();
			return attr?.Description ?? string.Empty;
		}

		//public static string GetValueDisplayName( object value )
		//{
		//	//TODO: we can use DisplayAttribute: https://msdn.microsoft.com/ru-ru/library/system.componentmodel.dataannotations.displayattribute(v=vs.110).aspx
		//	// or own custom attribute.

		//	//FieldInfo fi = Value.GetType().GetField( Value.ToString() );
		//	//var attr = fi.GetCustomAttribute<DisplayAttribute>();
		//	//if( attr != null )
		//	//	return attr.DisplayName;
		//	//else
		//	//	return Value.ToString();
		//	return value.ToString();
		//}
	}

	public class HCItemEnumDropDown : HCItemProperty
	{
		object currentValue;

		public HCItemEnumDropDown( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			return new HCGridDropDownButton();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var control = (IHCDropDownButton)CreatedControlInsidePropertyItemControl;
			control.Button.MouseUp += Control_MouseUp_ResetDefaultValue;
			control.Button.DropDown += Button_DropDown;
		}

		private void Button_DropDown( object sender, ComponentFactory.Krypton.Toolkit.ContextPositionMenuArgs e )
		{
			var control = (HCGridDropDownButton)CreatedControlInsidePropertyItemControl;
			Owner.ToggleDropDown( new EnumDropDownControl( this ), control.Button );
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			bool readOnly = !CanEditValue();
			var control = ( HCGridDropDownButton /*IHCDropDownButton*/)CreatedControlInsidePropertyItemControl;
			control.Button.Enabled = !readOnly;

			//update width. Anchor for this control works bad in .NET Core
			control.Button.Width = control.Width - control.Button.Location.X;

			var values = GetValues();
			if( values == null )
				return;

			object resultValue = null;

			for( int nValue = 0; nValue < values.Length; nValue++ )
			{
				var value = values[ nValue ];
				object unrefValue = ReferenceUtility.GetUnreferencedValue( value );

				if( nValue == 0 )
					resultValue = unrefValue;
				else
				{
					if( !Equals( resultValue, unrefValue ) )
						resultValue = "";
				}
			}

			if( !Equals( currentValue, resultValue ) )
			{
				OnValueChanged( currentValue, resultValue );
				currentValue = resultValue;
			}
		}

		protected virtual void OnValueChanged( object oldValue, object newValue )
		{
			var control = (IHCDropDownButton)CreatedControlInsidePropertyItemControl;

			var displayName = TypeUtility.DisplayNameAddSpaces( newValue.ToString() );
			var description = EnumExtension.GetValueDescription( newValue );
			Owner.PerformOverridePropertyEnumItem( this, ref displayName, ref description );

			control.Button.Text = displayName;

			var propControl = (IHCProperty)CreatedControl;
			propControl.SetToolTip( control.Button, description );

			//control.Button.Text = TypeUtility.DisplayNameAddSpaces( newValue.ToString() );

			//var propControl = (IHCProperty)CreatedControl;
			//propControl.SetToolTip( control.Button, EnumExtension.GetValueDescription( newValue ) );
		}
	}
}
