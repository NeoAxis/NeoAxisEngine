// Copyright (C) 2022 NeoAxis, Inc. Delaware, USA; NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NeoAxis.Editor
{
	//share changes with ColorValuePoweredSelectControl.cs

	public partial class ColorValuePoweredSelectForm : EngineForm
	{
		//HCItemProperty itemProperty;
		bool noAlpha;
		bool powered;
		ApplicableRangeColorValuePowerAttribute powerRange;
		bool readOnly;

		bool valueWasChanged;
		bool noUpdate;

		//HCItemProperty propertyItemForUndoSupport;

		//

		public ColorValuePoweredSelectForm()
		{
			InitializeComponent();

			Text = EditorLocalization.Translate( "ColorValuePoweredSelectForm", Text );
			buttonOK.Text = EditorLocalization.Translate( "General", buttonOK.Text );
			buttonCancel.Text = EditorLocalization.Translate( "General", buttonCancel.Text );

			EditorThemeUtility.ApplyDarkThemeToForm( this );
		}

		//public ColorValuePoweredSelectForm( HCItemProperty itemProperty )
		//{
		//	InitializeComponent();

		//	this.itemProperty = itemProperty;

		//	powered = ReferenceUtility.GetUnreferencedType( itemProperty.Property.Type.GetNetType() ) == typeof( ColorValuePowered );

		//	var values = itemProperty.GetValues();
		//	if( values == null )
		//		return;

		//	//!!!!multiselection
		//	var value = values[ 0 ];

		//	ColorValuePowered initValue;

		//	bool readOnly;
		//	{
		//		var netType = itemProperty.Property.Type.GetNetType();
		//		bool isReferenceType = ReferenceUtility.IsReferenceType( netType );

		//		bool referenceSpecified = false;
		//		IReference iReference = null;
		//		if( isReferenceType && value != null )
		//		{
		//			iReference = (IReference)value;
		//			referenceSpecified = !string.IsNullOrEmpty( iReference.GetByReference );
		//		}

		//		readOnly = referenceSpecified;
		//	}

		//	if( powered )
		//		initValue = (ColorValuePowered)ReferenceUtility.GetUnreferencedValue( value );
		//	else
		//	{
		//		var c = (ColorValue)ReferenceUtility.GetUnreferencedValue( value );
		//		initValue = new ColorValuePowered( c.Red, c.Green, c.Blue, c.Alpha, 1 );
		//	}

		//	//for( int n = 0; n < propertyOwners.Length; n++ )
		//	//{
		//	//	ColorValue v = (ColorValue)property.GetValue( propertyOwners[ n ], null );

		//	//	if( n != 0 )
		//	//	{
		//	//		if( value != v )
		//	//		{
		//	//			value = new ColorValue( 1, 1, 1 );
		//	//			break;
		//	//		}
		//	//	}
		//	//	else
		//	//		value = v;
		//	//}

		//	bool noAlpha = itemProperty.Property.GetCustomAttributes( typeof( ColorValueNoAlphaAttribute ), true ).Length != 0;

		//	ApplicableRangeColorValuePowerAttribute powerRange = null;
		//	if( powered )
		//	{
		//		var array = itemProperty.Property.GetCustomAttributes( typeof( ApplicableRangeColorValuePowerAttribute ), true );
		//		if( array.Length != 0 )
		//			powerRange = (ApplicableRangeColorValuePowerAttribute)array[ 0 ];
		//	}

		//	Init( initValue, noAlpha, powered, powerRange, readOnly );

		//	propertyItemForUndoSupport = itemProperty.GetItemInHierarchyToRestoreValues();
		//	propertyItemForUndoSupport.SaveValuesToRestore();
		//}

		public void Init( ColorValuePowered value, bool noAlpha, bool powered, ApplicableRangeColorValuePowerAttribute powerRange, bool readOnly )
		{
			this.noAlpha = noAlpha;
			this.powered = powered;
			if( powered && powerRange == null )
				powerRange = new ApplicableRangeColorValuePowerAttribute( 0, 100, RangeAttribute.ConvenientDistributionEnum.Exponential, 2 );
			this.powerRange = powerRange;
			this.readOnly = readOnly;

			noUpdate = true;

			UpdateRGBAControls( value.Color, null );

			if( powered )
			{
				numericUpDownPower.Minimum = (decimal)powerRange.Minimum;
				numericUpDownPower.Maximum = (decimal)powerRange.Maximum;

				powerRange.GetTrackBarMinMax( false, out int min, out int max );
				trackBarPower.Minimum = min;
				trackBarPower.Maximum = max;
				trackBarPower.LargeChange = ( trackBarPower.Maximum - trackBarPower.Minimum ) / 10;
				trackBarPower.SmallChange = ( trackBarPower.Maximum - trackBarPower.Minimum ) / 100;

				UpdatePowerControls( value.Power, null );
			}
			else
			{
				numericUpDownPower.Enabled = false;
				trackBarPower.Enabled = false;
				label5.Enabled = false;

				numericUpDownPower.Minimum = 0;
				numericUpDownPower.Maximum = 1;
				numericUpDownPower.Value = 1;
				trackBarPower.Minimum = 0;
				trackBarPower.Maximum = 1;
				trackBarPower.Value = 1;
			}

			//no alpha channel
			if( noAlpha )
			{
				numericUpDownAlpha.Enabled = false;
				trackBarAlpha.Enabled = false;
				label4.Enabled = false;
			}

			PrepareColorControls();

			if( readOnly )
			{
				colorWheel.Enabled = false;
				colorGradientControl.Enabled = false;

				numericUpDownRed.Enabled = false;
				numericUpDownGreen.Enabled = false;
				numericUpDownBlue.Enabled = false;
				numericUpDownAlpha.Enabled = false;
				numericUpDownPower.Enabled = false;
				//!!!!вверх, вниз всё равно работают при ReadOnly
				//numericUpDownRed.ReadOnly = true;
				//numericUpDownGreen.ReadOnly = true;
				//numericUpDownBlue.ReadOnly = true;
				//numericUpDownAlpha.ReadOnly = true;
				//numericUpDownPower.ReadOnly = true;

				trackBarAlpha.Enabled = false;
				trackBarBlue.Enabled = false;
				trackBarGreen.Enabled = false;
				trackBarRed.Enabled = false;
				trackBarPower.Enabled = false;
			}

			numericUpDownRed.TextChanged += NumericUpDown_TextChanged;
			numericUpDownGreen.TextChanged += NumericUpDown_TextChanged;
			numericUpDownBlue.TextChanged += NumericUpDown_TextChanged;
			numericUpDownAlpha.TextChanged += NumericUpDown_TextChanged;
			numericUpDownPower.TextChanged += NumericUpDown_TextChanged;

			noUpdate = false;
		}

		void UpdateRGBAControls( ColorValue value, object ignoreControl )
		{
			HSVColor hsvColor = HSVColor.FromRGB( value );

			if( ignoreControl != colorWheel )
				colorWheel.HsvColor = hsvColor;
			if( ignoreControl != colorGradientControl )
			{
				colorGradientControl.Value = (int)( hsvColor.Value * 255 );
				colorGradientControl.TopColor = new HSVColor( hsvColor.Hue, hsvColor.Saturation, 1 ).ToColor();
				colorGradientControl.BottomColor = Color.Black;
			}

			if( ignoreControl != numericUpDownRed )
				numericUpDownRed.Value = (decimal)MathEx.Saturate( value.Red ) * 255;
			if( ignoreControl != numericUpDownGreen )
				numericUpDownGreen.Value = (decimal)MathEx.Saturate( value.Green ) * 255;
			if( ignoreControl != numericUpDownBlue )
				numericUpDownBlue.Value = (decimal)MathEx.Saturate( value.Blue ) * 255;
			if( ignoreControl != numericUpDownAlpha )
				numericUpDownAlpha.Value = (decimal)( MathEx.Saturate( noAlpha ? 1 : value.Alpha ) * 255 );

			if( ignoreControl != trackBarRed )
				trackBarRed.Value = (int)( MathEx.Saturate( value.Red ) * 1000 );
			if( ignoreControl != trackBarGreen )
				trackBarGreen.Value = (int)( MathEx.Saturate( value.Green ) * 1000 );
			if( ignoreControl != trackBarBlue )
				trackBarBlue.Value = (int)( MathEx.Saturate( value.Blue ) * 1000 );
			if( ignoreControl != trackBarAlpha )
				trackBarAlpha.Value = (int)( MathEx.Saturate( noAlpha ? 1 : value.Alpha ) * 1000 );
		}

		void UpdatePowerControls( double value, object ignoreControl )
		{
			if( ignoreControl != numericUpDownPower )
				numericUpDownPower.Value = (decimal)MathEx.Clamp( value, powerRange.Minimum, powerRange.Maximum );
			if( ignoreControl != trackBarPower )
				trackBarPower.Value = powerRange.GetTrackBarValue( false, value );
		}

		[Browsable( false )]
		public bool ValueWasChanged
		{
			get { return valueWasChanged; }
		}

		[Browsable( false )]
		public ColorValuePowered CurrentValue
		{
			get
			{
				return new ColorValuePowered(
					(float)( numericUpDownRed.TextValue / 255 ),
					(float)( numericUpDownGreen.TextValue / 255 ),
					(float)( numericUpDownBlue.TextValue / 255 ),
					(float)( numericUpDownAlpha.TextValue / 255 ),
					(float)numericUpDownPower.TextValue );
			}
		}

		private void Control_ValueChanged( object sender, EventArgs e )
		{
			NumericUpDown_TextChanged( sender, e );
		}

		private void NumericUpDown_TextChanged( object sender, EventArgs e )
		{
			if( noUpdate )
				return;

			noUpdate = true;

			valueWasChanged = true;

			if( sender == colorWheel || sender == colorGradientControl ||
				sender == numericUpDownRed || sender == numericUpDownGreen || sender == numericUpDownBlue || sender == numericUpDownAlpha ||
				sender == trackBarRed || sender == trackBarGreen || sender == trackBarBlue || sender == trackBarAlpha )
			{
				ColorValue color = CurrentValue.Color;

				if( sender == colorWheel || sender == colorGradientControl )
				{
					HSVColor hsvColor = colorWheel.HsvColor;
					hsvColor.Value = (double)colorGradientControl.Value / 255.0;

					var c = hsvColor.ToColorValue();
					color.Red = c.Red;
					color.Green = c.Green;
					color.Blue = c.Blue;
				}

				if( sender == trackBarRed )
					color.Red = (float)( (double)trackBarRed.Value / 1000 );
				if( sender == trackBarGreen )
					color.Green = (float)( (double)trackBarGreen.Value / 1000 );
				if( sender == trackBarBlue )
					color.Blue = (float)( (double)trackBarBlue.Value / 1000 );
				if( sender == trackBarAlpha )
					color.Alpha = (float)( (double)trackBarAlpha.Value / 1000 );

				UpdateRGBAControls( color, sender );
			}

			if( powered && ( sender == numericUpDownPower || sender == trackBarPower ) )
			{
				double power = CurrentValue.Power;

				if( sender == trackBarPower )
					power = powerRange.GetValueFromTrackBar( false, trackBarPower.Value );

				UpdatePowerControls( power, sender );
			}

			//if( itemProperty != null && itemProperty.CanEditValue() )
			//{
			//	if( powered )
			//		itemProperty.SetValue( CurrentValue, false );
			//	else
			//		itemProperty.SetValue( CurrentValue.ToColorValue(), false );
			//}

			UpdateButtonCheckImages();

			noUpdate = false;
		}

		static Tuple<string, string>[] allColors =
		{
			Tuple.Create( "AliceBlue", "F0F8FF" ),
			Tuple.Create( "AntiqueWhite", "FAEBD7" ),
			Tuple.Create( "Aqua", "00FFFF" ),
			Tuple.Create( "Aquamarine", "7FFFD4" ),
			Tuple.Create( "Azure", "F0FFFF" ),
			Tuple.Create( "Beige", "F5F5DC" ),
			Tuple.Create( "Bisque", "FFE4C4" ),
			Tuple.Create( "Black", "000000" ),
			Tuple.Create( "BlanchedAlmond", "FFEBCD" ),
			Tuple.Create( "Blue", "0000FF" ),
			Tuple.Create( "BlueViolet", "8A2BE2" ),
			Tuple.Create( "Brown", "A52A2A" ),
			Tuple.Create( "BurlyWood", "DEB887" ),
			Tuple.Create( "CadetBlue", "5F9EA0" ),
			Tuple.Create( "Chartreuse", "7FFF00" ),
			Tuple.Create( "Chocolate", "D2691E" ),
			Tuple.Create( "Coral", "FF7F50" ),
			Tuple.Create( "CornflowerBlue", "6495ED" ),
			Tuple.Create( "Cornsilk", "FFF8DC" ),
			Tuple.Create( "Crimson", "DC143C" ),
			Tuple.Create( "Cyan", "00FFFF" ),
			Tuple.Create( "DarkBlue", "00008B" ),
			Tuple.Create( "DarkCyan", "008B8B" ),
			Tuple.Create( "DarkGoldenRod", "B8860B" ),
			Tuple.Create( "DarkGray", "A9A9A9" ),
			Tuple.Create( "DarkGrey", "A9A9A9" ),
			Tuple.Create( "DarkGreen", "006400" ),
			Tuple.Create( "DarkKhaki", "BDB76B" ),
			Tuple.Create( "DarkMagenta", "8B008B" ),
			Tuple.Create( "DarkOliveGreen", "556B2F" ),
			Tuple.Create( "DarkOrange", "FF8C00" ),
			Tuple.Create( "DarkOrchid", "9932CC" ),
			Tuple.Create( "DarkRed", "8B0000" ),
			Tuple.Create( "DarkSalmon", "E9967A" ),
			Tuple.Create( "DarkSeaGreen", "8FBC8F" ),
			Tuple.Create( "DarkSlateBlue", "483D8B" ),
			Tuple.Create( "DarkSlateGray", "2F4F4F" ),
			Tuple.Create( "DarkSlateGrey", "2F4F4F" ),
			Tuple.Create( "DarkTurquoise", "00CED1" ),
			Tuple.Create( "DarkViolet", "9400D3" ),
			Tuple.Create( "DeepPink", "FF1493" ),
			Tuple.Create( "DeepSkyBlue", "00BFFF" ),
			Tuple.Create( "DimGray", "696969" ),
			Tuple.Create( "DimGrey", "696969" ),
			Tuple.Create( "DodgerBlue", "1E90FF" ),
			Tuple.Create( "FireBrick", "B22222" ),
			Tuple.Create( "FloralWhite", "FFFAF0" ),
			Tuple.Create( "ForestGreen", "228B22" ),
			Tuple.Create( "Fuchsia", "FF00FF" ),
			Tuple.Create( "Gainsboro", "DCDCDC" ),
			Tuple.Create( "GhostWhite", "F8F8FF" ),
			Tuple.Create( "Gold", "FFD700" ),
			Tuple.Create( "GoldenRod", "DAA520" ),
			Tuple.Create( "Gray", "808080" ),
			Tuple.Create( "Grey", "808080" ),
			Tuple.Create( "Green", "008000" ),
			Tuple.Create( "GreenYellow", "ADFF2F" ),
			Tuple.Create( "HoneyDew", "F0FFF0" ),
			Tuple.Create( "HotPink", "FF69B4" ),
			Tuple.Create( "IndianRed", "CD5C5C" ),
			Tuple.Create( "Indigo", "4B0082" ),
			Tuple.Create( "Ivory", "FFFFF0" ),
			Tuple.Create( "Khaki", "F0E68C" ),
			Tuple.Create( "Lavender", "E6E6FA" ),
			Tuple.Create( "LavenderBlush", "FFF0F5" ),
			Tuple.Create( "LawnGreen", "7CFC00" ),
			Tuple.Create( "LemonChiffon", "FFFACD" ),
			Tuple.Create( "LightBlue", "ADD8E6" ),
			Tuple.Create( "LightCoral", "F08080" ),
			Tuple.Create( "LightCyan", "E0FFFF" ),
			Tuple.Create( "LightGoldenRodYellow", "FAFAD2" ),
			Tuple.Create( "LightGray", "D3D3D3" ),
			Tuple.Create( "LightGrey", "D3D3D3" ),
			Tuple.Create( "LightGreen", "90EE90" ),
			Tuple.Create( "LightPink", "FFB6C1" ),
			Tuple.Create( "LightSalmon", "FFA07A" ),
			Tuple.Create( "LightSeaGreen", "20B2AA" ),
			Tuple.Create( "LightSkyBlue", "87CEFA" ),
			Tuple.Create( "LightSlateGray", "778899" ),
			Tuple.Create( "LightSlateGrey", "778899" ),
			Tuple.Create( "LightSteelBlue", "B0C4DE" ),
			Tuple.Create( "LightYellow", "FFFFE0" ),
			Tuple.Create( "Lime", "00FF00" ),
			Tuple.Create( "LimeGreen", "32CD32" ),
			Tuple.Create( "Linen", "FAF0E6" ),
			Tuple.Create( "Magenta", "FF00FF" ),
			Tuple.Create( "Maroon", "800000" ),
			Tuple.Create( "MediumAquaMarine", "66CDAA" ),
			Tuple.Create( "MediumBlue", "0000CD" ),
			Tuple.Create( "MediumOrchid", "BA55D3" ),
			Tuple.Create( "MediumPurple", "9370DB" ),
			Tuple.Create( "MediumSeaGreen", "3CB371" ),
			Tuple.Create( "MediumSlateBlue", "7B68EE" ),
			Tuple.Create( "MediumSpringGreen", "00FA9A" ),
			Tuple.Create( "MediumTurquoise", "48D1CC" ),
			Tuple.Create( "MediumVioletRed", "C71585" ),
			Tuple.Create( "MidnightBlue", "191970" ),
			Tuple.Create( "MintCream", "F5FFFA" ),
			Tuple.Create( "MistyRose", "FFE4E1" ),
			Tuple.Create( "Moccasin", "FFE4B5" ),
			Tuple.Create( "NavajoWhite", "FFDEAD" ),
			Tuple.Create( "Navy", "000080" ),
			Tuple.Create( "OldLace", "FDF5E6" ),
			Tuple.Create( "Olive", "808000" ),
			Tuple.Create( "OliveDrab", "6B8E23" ),
			Tuple.Create( "Orange", "FFA500" ),
			Tuple.Create( "OrangeRed", "FF4500" ),
			Tuple.Create( "Orchid", "DA70D6" ),
			Tuple.Create( "PaleGoldenRod", "EEE8AA" ),
			Tuple.Create( "PaleGreen", "98FB98" ),
			Tuple.Create( "PaleTurquoise", "AFEEEE" ),
			Tuple.Create( "PaleVioletRed", "DB7093" ),
			Tuple.Create( "PapayaWhip", "FFEFD5" ),
			Tuple.Create( "PeachPuff", "FFDAB9" ),
			Tuple.Create( "Peru", "CD853F" ),
			Tuple.Create( "Pink", "FFC0CB" ),
			Tuple.Create( "Plum", "DDA0DD" ),
			Tuple.Create( "PowderBlue", "B0E0E6" ),
			Tuple.Create( "Purple", "800080" ),
			Tuple.Create( "RebeccaPurple", "663399" ),
			Tuple.Create( "Red", "FF0000" ),
			Tuple.Create( "RosyBrown", "BC8F8F" ),
			Tuple.Create( "RoyalBlue", "4169E1" ),
			Tuple.Create( "SaddleBrown", "8B4513" ),
			Tuple.Create( "Salmon", "FA8072" ),
			Tuple.Create( "SandyBrown", "F4A460" ),
			Tuple.Create( "SeaGreen", "2E8B57" ),
			Tuple.Create( "SeaShell", "FFF5EE" ),
			Tuple.Create( "Sienna", "A0522D" ),
			Tuple.Create( "Silver", "C0C0C0" ),
			Tuple.Create( "SkyBlue", "87CEEB" ),
			Tuple.Create( "SlateBlue", "6A5ACD" ),
			Tuple.Create( "SlateGray", "708090" ),
			Tuple.Create( "SlateGrey", "708090" ),
			Tuple.Create( "Snow", "FFFAFA" ),
			Tuple.Create( "SpringGreen", "00FF7F" ),
			Tuple.Create( "SteelBlue", "4682B4" ),
			Tuple.Create( "Tan", "D2B48C" ),
			Tuple.Create( "Teal", "008080" ),
			Tuple.Create( "Thistle", "D8BFD8" ),
			Tuple.Create( "Tomato", "FF6347" ),
			Tuple.Create( "Turquoise", "40E0D0" ),
			Tuple.Create( "Violet", "EE82EE" ),
			Tuple.Create( "Wheat", "F5DEB3" ),
			Tuple.Create( "White", "FFFFFF" ),
			Tuple.Create( "WhiteSmoke", "F5F5F5" ),
			Tuple.Create( "Yellow", "FFFF00" ),
			Tuple.Create( "YellowGreen", "9ACD32")
		};

		void PrepareColorControls()
		{
			List<(string, ColorValue)> colors = new List<(string, ColorValue)>();
			ESet<ColorValue> used = new ESet<ColorValue>();
			foreach( var tuple in allColors )
			{
				var name = tuple.Item1;

				var str = tuple.Item2;
				int r = int.Parse( str.Substring( 0, 2 ), System.Globalization.NumberStyles.HexNumber );
				int g = int.Parse( str.Substring( 2, 2 ), System.Globalization.NumberStyles.HexNumber );
				int b = int.Parse( str.Substring( 4, 2 ), System.Globalization.NumberStyles.HexNumber );
				ColorValue value = new ColorValue( Color.FromArgb( r, g, b ) );

				if( !used.Contains( value ) )
				{
					used.Add( value );

					colors.Add( (name, value) );
				}
			}

			for( int n = 0; n < colors.Count; n++ )
			{
				var tuple = colors[ n ];
				string name = tuple.Item1;
				var value = tuple.Item2;

				var array = Controls.Find( $"button{n}", false );
				if( array.Length != 0 )
				{
					Button button = (Button)array[ 0 ];

					button.Enabled = !readOnly;
					button.BackColor = value.ToColor();
					button.Tag = value;

					button.Click += Button_Click;

					//!!!!tooltips. тогда повторяющиеся показывать?
				}
			}

			UpdateButtonCheckImages();
		}

		private void Button_Click( object sender, EventArgs e )
		{
			Button button = (Button)sender;
			if( button.Tag == null )
				return;

			ColorValue color = (ColorValue)button.Tag;

			noUpdate = true;
			numericUpDownRed.Value = (decimal)( color.Red * 255 );
			numericUpDownGreen.Value = (decimal)( color.Green * 255 );
			numericUpDownBlue.Value = (decimal)( color.Blue * 255 );
			noUpdate = false;
			NumericUpDown_TextChanged( numericUpDownBlue, null );

			UpdateButtonCheckImages();
		}

		void UpdateButtonCheckImages()
		{
			var current = new ColorValue( CurrentValue.Red, CurrentValue.Green, CurrentValue.Blue );

			foreach( var control in Controls )
			{
				Button button = control as Button;
				if( button != null )
				{
					if( button.Tag != null && button.Tag is ColorValue )
					{
						ColorValue color = (ColorValue)button.Tag;

						if( color.Equals( current, .001f ) )
							button.Image = EditorResourcesCache.GetImage( "Check_12" );
						else
							button.Image = null;
					}
				}
			}
		}

		//public override void OnCommitChanges()
		//{
		//	base.OnCommitChanges();

		//	if( valueWasChanged )
		//		propertyItemForUndoSupport.AddUndoActionWithSavedValuesToRestore();
		//}

		//public override void OnCancelChanges()
		//{
		//	base.OnCancelChanges();

		//	if( valueWasChanged )
		//		propertyItemForUndoSupport.RestoreSavedOldValues();
		//}
	}
}
