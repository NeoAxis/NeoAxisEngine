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
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public interface IHCLabel
	{
		//Label Label1 { get; }
		EngineTextBox Label2 { get; }
	}

	public class HCItemLabel : HCItemProperty
	{
		public HCItemLabel( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			//if( Owner.GridMode )
			return new HCGridLabel();
			//else
			//	return new HCFormLabel();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			//!!!!так?
			//subscribe for context menu
			var control = (IHCLabel)CreatedControlInsidePropertyItemControl;
			control.Label2.MouseUp += Control_MouseUp_ResetDefaultValue;
		}

		public static string GetValueText( object value )
		{
			if( value == null )
				return "(Null)";

			//get file name of resource for components without name.
			var component = value as Component;
			if( component != null )
			{
				if( string.IsNullOrEmpty( component.Name ) )
				{
					if( component.HierarchyController != null && component.HierarchyController.CreatedByResource != null )
					{
						var res = component.HierarchyController.CreatedByResource.Owner;
						if( res.LoadFromFile )
						{
							var fileName = Path.GetFileName( res.Name );
							if( !string.IsNullOrEmpty( fileName ) )
								return fileName;
						}
					}
				}
			}

			return value.ToString();
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCLabel)CreatedControlInsidePropertyItemControl;
			var values = GetValues();
			if( values == null )
				return;

			var valueResult = "";

			for( int nValue = 0; nValue < values.Length; nValue++ )
			{
				var value = values[ nValue ];
				var unrefValue = ReferenceUtility.GetUnreferencedValue( value );
				var stringValue = GetValueText( unrefValue );

				if( nValue == 0 )
					valueResult = stringValue;
				else
				{
					if( valueResult != stringValue )
						valueResult = "";
				}
			}

			if( control.Label2 != null )
				control.Label2.Text = valueResult;

			if( Property != null )
			{
				if( control.Label2.Enabled != !Property.ReadOnly )
					control.Label2.Enabled = !Property.ReadOnly;
			}

			////var netType = Property.Type.GetNetType();
			////var unrefType = ReferenceUtils.GetUnreferencedType( netType );

			////var type = MetadataManager.GetTypeOfNetType( unrefType );

			////if( control.Label1 != null )
			////	control.Label1.Text = Property.Name;
		}
	}
}
