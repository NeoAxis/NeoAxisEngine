// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	public interface IHCColorValuePowered : IHCColorValue
	{
		ComponentFactory.Krypton.Toolkit.KryptonTrackBar TrackBarPower { get; }
	}

	public class HCItemColorValuePowered : HCItemColorValue
	{
		ApplicableRangeColorValuePowerAttribute powerRange;

		bool trackBarFocused;
		bool trackBarModifying;
		HCItemProperty trackBarItemInHierarchyToAddUndo;
		bool trackBarDisableValueChangedEvent;

		//

		public HCItemColorValuePowered( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			return new HCGridColorValuePowered();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var control = (IHCColorValuePowered)CreatedControlInsidePropertyItemControl;
			//!!!!
			//control.TrackBarPower.MouseWheel += TrackBarPower_MouseWheel;

			//get range
			var array = Property.GetCustomAttributes( typeof( ApplicableRangeColorValuePowerAttribute ), true );
			if( array.Length != 0 )
				powerRange = (ApplicableRangeColorValuePowerAttribute)array[ 0 ];
			if( powerRange == null )
				powerRange = new ApplicableRangeColorValuePowerAttribute( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 2 );

			//configure track bar
			//if( powerRange != null )
			{
				var trackBar = control.TrackBarPower;

				trackBarDisableValueChangedEvent = true;
				powerRange.GetTrackBarMinMax( false, out int min, out int max );
				trackBar.Minimum = min;
				trackBar.Maximum = max;
				trackBar.LargeChange = ( trackBar.Maximum - trackBar.Minimum ) / 10;
				trackBar.SmallChange = ( trackBar.Maximum - trackBar.Minimum ) / 100;
				trackBarDisableValueChangedEvent = false;

				trackBar.GotFocus += TrackBar_GotFocus;
				trackBar.LostFocus += TrackBar_LostFocus;
				trackBar.MouseUp += TrackBar_MouseUp;
				trackBar.ValueChanged += TrackBar_ValueChanged;
			}

			////disable track bar
			////!!!!может еще каким-то условием показывать/скрывать
			//if( powerRange == null )
			//{
			//	control.TrackBar1.Enabled = false;
			//	control.TrackBar1.Visible = false;
			//	control.TextBox1.Dock = DockStyle.Fill;
			//}
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCColorValuePowered)CreatedControlInsidePropertyItemControl;
			var trackBar = control.TrackBarPower;

			if( powerRange != null )
			{
				var netType = Property.Type.GetNetType();
				bool isReferenceType = ReferenceUtility.IsReferenceType( netType );

				var values = GetValues();
				if( values != null )
				{
					trackBar.Enabled = CanEditValue();
					//bool referenceSpecified = false;
					//if( isReferenceType && value != null )
					//	referenceSpecified = ( (IReference)value ).ReferenceSpecified;
					//if( trackBar.Enabled != !referenceSpecified )
					//	trackBar.Enabled = !referenceSpecified;

					//!!!!new
					if( !trackBarModifying )//if( !trackBar.Focused )
						UpdateTrackBar();
				}
			}
		}

		void UpdateTrackBar()
		{
			var control = (IHCColorValuePowered)CreatedControlInsidePropertyItemControl;
			var trackBar = control.TrackBarPower;

			var values = GetValues();
			if( values == null )
				return;

			int resultValue = 0;

			for( int nValue = 0; nValue < values.Length; nValue++ )
			{
				var value = values[ nValue ];
				var unrefValue = ReferenceUtility.GetUnreferencedValue( value );
				var power = ( (ColorValuePowered)unrefValue ).Power;
				var value2 = powerRange.GetTrackBarValue( false, power );

				if( nValue == 0 )
					resultValue = value2;
				else
				{
					if( resultValue != value2 )
						resultValue = control.TrackBarPower.Minimum;
				}
			}

			if( trackBar.Value != resultValue )
			{
				trackBarDisableValueChangedEvent = true;
				trackBar.Value = resultValue;
				trackBarDisableValueChangedEvent = false;
			}
		}

		protected override void TextBoxTextChanged()
		{
			base.TextBoxTextChanged();

			if( powerRange != null )
				UpdateTrackBar();
		}

		private void TrackBar_GotFocus( object sender, EventArgs e )
		{
			trackBarFocused = true;
		}

		void TrackBarApplyChanges()
		{
			//end modifying. add undo action
			if( trackBarModifying )
			{
				trackBarItemInHierarchyToAddUndo.AddUndoActionWithSavedValuesToRestore();
				trackBarModifying = false;
			}
		}

		private void TrackBar_LostFocus( object sender, EventArgs e )
		{
			trackBarFocused = false;

			var control = (IHCColorValuePowered)CreatedControlInsidePropertyItemControl;
			if( !control.TextBox.ReadOnly )
				TrackBarApplyChanges();
		}

		private void TrackBar_MouseUp( object sender, MouseEventArgs e )
		{
			var control = (IHCColorValuePowered)CreatedControlInsidePropertyItemControl;
			if( !control.TextBox.ReadOnly )
				TrackBarApplyChanges();
		}

		private void TrackBar_ValueChanged( object sender, EventArgs e )
		{
			if( trackBarDisableValueChangedEvent )
				return;

			//!!!!new
			if( !trackBarFocused )
				return;

			//begin modifying. save old values for undo
			if( !trackBarModifying )
			{
				trackBarItemInHierarchyToAddUndo = GetItemInHierarchyToRestoreValues();
				trackBarItemInHierarchyToAddUndo.SaveValuesToRestore();
				trackBarModifying = true;
			}

			var control = (IHCColorValuePowered)CreatedControlInsidePropertyItemControl;
			var trackBar = control.TrackBarPower;

			trackBarDisableValueChangedEvent = true;

			var netType = Property.Type.GetNetType();
			var unrefType = ReferenceUtility.GetUnreferencedType( netType );


			var values = GetValues();
			if( values == null )
				return;

			//!!!!multiselection
			var value = values[ 0 ];

			var unrefValue = ReferenceUtility.GetUnreferencedValue( value );

			var color = (ColorValuePowered)unrefValue;

			try
			{
				var power = powerRange.GetValueFromTrackBar( false, trackBar.Value );
				color.Power = (float)power;

				SetValue( color, false );
			}
			catch
			{
			}

			trackBarDisableValueChangedEvent = false;
		}
	}
}
