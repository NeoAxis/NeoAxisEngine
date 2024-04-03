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
using Internal.ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public interface IHCTextBoxNumeric : IHCTextBox
	{
		KryptonTrackBar TrackBar { get; }
	}

	/// <summary>
	/// Represents a numeric text box item for property for <see cref="HierarchicalContainer"/>.
	/// </summary>
	public class HCItemTextBoxNumeric : HCItemTextBox
	{
		RangeAttribute range;

		bool trackBarFocused;
		bool trackBarModifying;
		HCItemProperty trackBarItemInHierarchyToAddUndo;
		bool trackBarDisableValueChangedEvent;

		//

		public HCItemTextBoxNumeric( HierarchicalContainer owner, HierarchicalContainer.Item parent, object[] controlledObjects, Metadata.Property property, object[] indexers )
			: base( owner, parent, controlledObjects, property, indexers )
		{
		}

		public override EUserControl CreateControlInsidePropertyItemControl()
		{
			return new HCGridTextBoxNumeric();
		}

		public override void ControlInsidePropertyItemControlWasCreated()
		{
			base.ControlInsidePropertyItemControlWasCreated();

			var control = (IHCTextBoxNumeric)CreatedControlInsidePropertyItemControl;

			//get range
			var array = Property.GetCustomAttributes( typeof( RangeAttribute ), true );
			if( array.Length != 0 )
				range = (RangeAttribute)array[ 0 ];

			//configure track bar
			if( range != null )
			{
				var trackBar = control.TrackBar;

				trackBarDisableValueChangedEvent = true;
				range.GetTrackBarMinMax( IsInteger(), out int min, out int max );
				trackBar.Minimum = min;
				trackBar.Maximum = max;
				trackBar.LargeChange = Math.Max( ( trackBar.Maximum - trackBar.Minimum ) / 10, 1 );
				trackBar.SmallChange = Math.Max( ( trackBar.Maximum - trackBar.Minimum ) / 100, 1 );
				trackBarDisableValueChangedEvent = false;

				trackBar.GotFocus += TrackBar_GotFocus;
				trackBar.LostFocus += TrackBar_LostFocus;
				trackBar.MouseUp += TrackBar_MouseUp;
				trackBar.ValueChanged += TrackBar_ValueChanged;
			}

			//disable track bar
			//!!!!может еще каким-то условием показывать/скрывать
			if( range == null )
			{
				control.TrackBar.Enabled = false;
				control.TrackBar.Visible = false;
				control.TextBox.Width = ( (Control)control ).Width;
				control.TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			}
		}

		public override void UpdateControl()
		{
			base.UpdateControl();

			var control = (IHCTextBoxNumeric)CreatedControlInsidePropertyItemControl;
			var trackBar = control.TrackBar;

			if( range != null )
			{
				var netType = Property.Type.GetNetType();
				bool isReferenceType = ReferenceUtility.IsReferenceType( netType );

				var values = GetValues();
				if( values != null )
				{
					trackBar.Enabled = CanEditValue();

					//!!!!new
					if( !trackBarModifying )//if( !trackBar.Focused )
						UpdateTrackBar();
				}
			}
		}

		void UpdateTrackBar()
		{
			var control = (IHCTextBoxNumeric)CreatedControlInsidePropertyItemControl;
			var trackBar = control.TrackBar;

			var values = GetValues();
			if( values == null )
				return;

			int resultValue = 0;

			for( int nValue = 0; nValue < values.Length; nValue++ )
			{
				var value = values[ nValue ];
				var unrefValue = ReferenceUtility.GetUnreferencedValue( value );
				//conversion by means string
				double doubleValue = double.Parse( unrefValue.ToString() );
				var value2 = range.GetTrackBarValue( IsInteger(), doubleValue );

				if( nValue == 0 )
					resultValue = value2;
				else
				{
					if( resultValue != value2 )
						resultValue = control.TrackBar.Minimum;
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

			if( range != null )
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

			var control = (IHCTextBoxNumeric)CreatedControlInsidePropertyItemControl;
			if( !control.TextBox.ReadOnly )
				TrackBarApplyChanges();
		}

		private void TrackBar_MouseUp( object sender, MouseEventArgs e )
		{
			var control = (IHCTextBoxNumeric)CreatedControlInsidePropertyItemControl;
			if( !control.TextBox.ReadOnly )
				TrackBarApplyChanges();
		}

		bool IsInteger()
		{
			var netType = Property.Type.GetNetType();
			var unrefType = ReferenceUtility.GetUnreferencedType( netType );

			return
				unrefType == typeof( sbyte ) ||
				unrefType == typeof( byte ) ||
				unrefType == typeof( char ) ||
				unrefType == typeof( short ) ||
				unrefType == typeof( ushort ) ||
				unrefType == typeof( int ) ||
				unrefType == typeof( uint ) ||
				unrefType == typeof( long ) ||
				unrefType == typeof( ulong );
		}

		private void TrackBar_ValueChanged( object sender, EventArgs e )
		{
			if( trackBarDisableValueChangedEvent )
				return;

			if( !trackBarFocused )
				return;

			//begin modifying. save old values for undo
			if( !trackBarModifying )
			{
				trackBarItemInHierarchyToAddUndo = GetItemInHierarchyToRestoreValues();
				trackBarItemInHierarchyToAddUndo.SaveValuesToRestore();
				trackBarModifying = true;
			}

			var control = (IHCTextBoxNumeric)CreatedControlInsidePropertyItemControl;
			var trackBar = control.TrackBar;

			trackBarDisableValueChangedEvent = true;

			var netType = Property.Type.GetNetType();
			var unrefType = ReferenceUtility.GetUnreferencedType( netType );

			try
			{
				double doubleValue = range.GetValueFromTrackBar( IsInteger(), trackBar.Value );

				//conversion by means string
				string str;
				if( IsInteger() )
					str = Convert.ToInt64( doubleValue ).ToString();
				else
					str = doubleValue.ToString();
				var value = SimpleTypes.ParseValue( unrefType, str );

				SetValue( value, false );
			}
			catch
			{
			}

			trackBarDisableValueChangedEvent = false;
		}
	}
}
#endif