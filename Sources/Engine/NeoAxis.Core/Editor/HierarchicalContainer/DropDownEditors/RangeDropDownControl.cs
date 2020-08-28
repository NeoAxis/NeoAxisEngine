// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel.DataAnnotations;

namespace NeoAxis.Editor
{
	public partial class RangeDropDownControl : HCDropDownControl
	{
		HCItemProperty itemProperty;
		HCItemProperty propertyItemForUndoSupport;
		bool valueWasChanged;
		Range value;
		bool valueChanging;
		RangeAttribute rangeAttr;

		private static class RangeConverter
		{
			public static Range ObjectToRange( object value )
			{
				// pattern matching.
				switch( value )
				{
				case Range v:
					return v;
				case RangeF v:
					return v;
				case RangeI v:
					return new Range( v.Minimum, v.Maximum );
				case Vector2 v:
					return new Range( v.X, v.Y );
				case Vector2I v:
					return new Range( v.X, v.Y );
				case Vector2F v:
					return new Range( v.X, v.Y );
				default:
					throw new ArgumentException( "Type not supported.", nameof( value ) );
				}
			}

			public static object ConvertRange( Range range, Type type )
			{
				if( type == typeof( Range ) )
					return range;
				else if( type == typeof( RangeF ) )
					return range.ToRangeF();
				else if( type == typeof( RangeI ) )
					return range.ToRangeI();
				else if( type == typeof( Vector2 ) )
					return range.ToVector2();
				else if( type == typeof( Vector2F ) )
					return range.ToVector2().ToVector2F();
				else if( type == typeof( Vector2I ) )
					return range.ToVector2().ToVector2I();
				else
					throw new ArgumentException( "Type not supported.", nameof( type ) );
			}
		}

		public RangeDropDownControl()
		{
			InitializeComponent();

			EditorThemeUtility.ApplyDarkThemeToForm( this );
		}

		public RangeDropDownControl( HCItemProperty itemProperty )
		{
			InitializeComponent();

			AddOkCancelButtons( out _, out _ );

			this.itemProperty = itemProperty;

			rangeAttr = (RangeAttribute)itemProperty.Property.GetCustomAttributes( typeof( RangeAttribute) ).FirstOrDefault();
			if( rangeAttr == null )
				rangeAttr = new RangeAttribute( 0, 100 ); // default

			SetupUIApplicableRange();

			minTrackBar.ValueChanged += new EventHandler( anyTrackBar_ValueChanged );
			maxTrackBar.ValueChanged += new EventHandler( anyTrackBar_ValueChanged );

			object obj = ReferenceUtility.GetUnreferencedValue( itemProperty.GetValues()[ 0 ] );
			value = RangeConverter.ObjectToRange( obj );

			propertyItemForUndoSupport = itemProperty.GetItemInHierarchyToRestoreValues();
			propertyItemForUndoSupport.SaveValuesToRestore();

			UpdateTrackBarsAndTextBoxes();

			EditorThemeUtility.ApplyDarkThemeToForm( this );
		}

		protected override void OnParentChanged( EventArgs e )
		{
			base.OnParentChanged( e );

			//!!!! need buttons ?
			//if( ParentHolder != null)
			//	ParentHolder.ButtonsVisible = false;
		}

		public override void OnCommitChanges()
		{
			base.OnCommitChanges();

			if( valueWasChanged )
				propertyItemForUndoSupport.AddUndoActionWithSavedValuesToRestore();
		}

		public override void OnCancelChanges()
		{
			base.OnCancelChanges();

			if( valueWasChanged )
				propertyItemForUndoSupport.RestoreSavedOldValues();
		}

		private void UpdateItemProperty()
		{
			if( itemProperty != null && itemProperty.CanEditValue() )
			{
				var type = ReferenceUtility.GetUnreferencedType( itemProperty.Property.Type ).GetNetType();
				//var type = itemProperty.Property.Type.GetNetUnreferencedType();
				itemProperty.SetValue( RangeConverter.ConvertRange( value, type ), false );
			}
			valueWasChanged = true;
		}

		private bool IsIntegerType()
		{
			var type = ReferenceUtility.GetUnreferencedType( itemProperty.Property.Type ).GetNetType();
			//var type = itemProperty.Property.Type.GetNetUnreferencedType();
			return type == typeof( RangeI ) || type == typeof( Vector2I );
		}

		private void SetupUIApplicableRange()
		{
			if( rangeAttr != null )
			{
				rangeAttr.GetTrackBarMinMax( IsIntegerType(), out int min, out int max );

				minTrackBar.Minimum = min;
				minTrackBar.Maximum = max;
				minTrackBar.LargeChange = Math.Max( ( minTrackBar.Maximum - minTrackBar.Minimum ) / 10, 1 );
				minTrackBar.SmallChange = Math.Max( ( minTrackBar.Maximum - minTrackBar.Minimum ) / 100, 1 );

				maxTrackBar.Minimum = min;
				maxTrackBar.Maximum = max;
				maxTrackBar.LargeChange = Math.Max( ( minTrackBar.Maximum - minTrackBar.Minimum ) / 10, 1 );
				maxTrackBar.SmallChange = Math.Max( ( minTrackBar.Maximum - minTrackBar.Minimum ) / 100, 1 );
			}
		}

		private void UpdateTrackBarsAndTextBoxes()
		{
			valueChanging = true;

			// cast to float to fix double "precision lost" formatting.
			minTextBox.Text = ( (float)value.Minimum ).ToString();
			maxTextBox.Text = ( (float)value.Maximum ).ToString();

			minTrackBar.Value = rangeAttr.GetTrackBarValue( IsIntegerType(), value.Minimum );
			maxTrackBar.Value = rangeAttr.GetTrackBarValue( IsIntegerType(), value.Maximum );

			valueChanging = false;
		}

		private void anyTextBox_TextChanged( object sender, EventArgs e )
		{
			if( valueChanging )
				return;

			if( !ValidateTextBox( (EngineTextBox)sender ) )
				return;

			try
			{
				valueChanging = true;

				value.Minimum = double.Parse( minTextBox.Text );
				value.Maximum = double.Parse( maxTextBox.Text );

				minTrackBar.Value = rangeAttr.GetTrackBarValue( IsIntegerType(), value.Minimum );
				maxTrackBar.Value = rangeAttr.GetTrackBarValue( IsIntegerType(), value.Maximum );

				UpdateItemProperty();
			}
			finally
			{
				valueChanging = false;
			}
		}

		private void anyTrackBar_ValueChanged( object sender, EventArgs e )
		{
			if( valueChanging )
				return;
			try
			{
				valueChanging = true;

				value.Minimum = rangeAttr.GetValueFromTrackBar( IsIntegerType(), minTrackBar.Value );
				value.Maximum = rangeAttr.GetValueFromTrackBar( IsIntegerType(), maxTrackBar.Value );

				minTextBox.Text = value.Minimum.ToString();
				maxTextBox.Text = value.Maximum.ToString();

				ValidateTextBox( minTextBox );
				ValidateTextBox( maxTextBox );

				UpdateItemProperty();
			}
			finally
			{
				valueChanging = false;
			}
		}

		private bool ValidateTextBox( EngineTextBox textBox )
		{
			try
			{
				// validate for double

				double.Parse( textBox.Text );
				textBox.SetError( "" );
				return true;
			}
			catch( FormatException exc )
			{
				textBox.SetError( exc.Message );
				return false;
			}
		}

		private void anyTextBox_Validated( object sender, EventArgs e )
		{
			ValidateTextBox( (EngineTextBox)sender );
		}
	}
}
