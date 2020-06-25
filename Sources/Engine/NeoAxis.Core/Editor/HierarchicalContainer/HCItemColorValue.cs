// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public interface IHCColorValue : IHCTextBox
	{
		HCColorPreviewButton PreviewButton { get; }
	}

	public class HCItemColorValue : HCItemTextBox
	{
		public HCItemColorValue( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override UserControl CreateControlInsidePropertyItemControl()
		{
			return new HCGridColorValue();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var control = (IHCColorValue)CreatedControlInsidePropertyItemControl;
			control.PreviewButton.Click += PreviewButton_Click;
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCColorValue)CreatedControlInsidePropertyItemControl;
			UpdatePreviewColor();
		}

		protected override void GetExpandablePropertiesFilter( Metadata.Property p, ref bool skip )
		{
			base.GetExpandablePropertiesFilter( p, ref skip );

			//ColorValueNoAlphaAttribute
			if( p.Name == "Alpha" && Property.GetCustomAttributes( typeof( ColorValueNoAlphaAttribute ), true ).Length != 0 )
				skip = true;
		}

		void UpdatePreviewColor()
		{
			var values = GetValues();
			if( values == null )
				return;

			var valueResult = ColorValue.Zero;

			for( int nValue = 0; nValue < values.Length; nValue++ )
			{
				var value = values[ nValue ];
				var unrefValue = ReferenceUtility.GetUnreferencedValue( value );

				ColorValue color;
				if( unrefValue is ColorValuePowered )
					color = ( (ColorValuePowered)unrefValue ).Color;
				else
					color = (ColorValue)unrefValue;

				if( nValue == 0 )
					valueResult = color;
				else
				{
					if( valueResult != color )
						valueResult = ColorValue.One;
				}
			}

			var control = (IHCColorValue)CreatedControlInsidePropertyItemControl;
			if( control != null )
				control.PreviewButton.PreviewColor = valueResult;
		}

		private void PreviewButton_Click( object sender, EventArgs e )
		{
			Owner.ToggleDropDown( new ColorValuePoweredSelectControl( this ), this );
		}
	}
}
