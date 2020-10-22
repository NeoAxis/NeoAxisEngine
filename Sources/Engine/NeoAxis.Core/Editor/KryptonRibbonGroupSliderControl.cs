// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	public partial class KryptonRibbonGroupSliderControl : UserControl
	{
		double minimum;
		double maximum;
		double exponentialPower;
		double value;

		public event EventHandler ValueChanged;

		bool disableEvents;

		//

		public KryptonRibbonGroupSliderControl()
		{
			InitializeComponent();

			EditorThemeUtility.ApplyDarkThemeToForm( this );

			if( kryptonLabel1.Height < kryptonLabel1.PreferredSize.Height )
				kryptonLabel1.Height = kryptonLabel1.PreferredSize.Height;
		}

		public void Init( double minimum, double maximum, double exponentialPower )
		{
			this.minimum = minimum;
			this.maximum = maximum;
			this.exponentialPower = exponentialPower;
		}

		public double GetValue()
		{
			return value;
		}

		public void SetValue( double value )
		{
			this.value = value;

			disableEvents = true;
			SetTrackBarValue();
			SetTextBoxValue();
			disableEvents = false;
		}

		void SetTrackBarValue()
		{
			double v = ( value - minimum ) / ( maximum - minimum );
			if( exponentialPower != 0 )
				v = Math.Pow( v, 1.0 / exponentialPower );
			kryptonTrackBar1.Value = MathEx.Clamp( (int)( v * 1000 ), 0, 1000 );
		}

		void SetTextBoxValue()
		{
			var text = value.ToString( "G29" );
			//var text = value.ToString( "F2" );
			//if(value > 0 && value < xx )
			if( text.IndexOf( "-" ) == -1 && text.IndexOf( "." ) != -1 && text.IndexOf( "." ) <= 3 && text.Length > 4 )
			{
				text = text.Substring( 0, 4 );
				if( text[ text.Length - 1 ] == '.' )
					text = text.Substring( 0, text.Length - 1 );
			}
			kryptonTextBox1.Text = text;
			//kryptonTextBox1.Text = value.ToString( "F2" );
		}

		private void kryptonTrackBar1_ValueChanged( object sender, EventArgs e )
		{
			if( disableEvents )
				return;

			double v = (double)kryptonTrackBar1.Value / 1000;
			if( exponentialPower != 0 )
				v = Math.Pow( v, exponentialPower );
			v = minimum + v * ( maximum - minimum );
			var newValue = MathEx.Clamp( v, minimum, maximum );

			if( newValue != value )
			{
				value = newValue;

				disableEvents = true;
				SetTextBoxValue();
				disableEvents = false;

				ValueChanged?.Invoke( this, EventArgs.Empty );
			}
		}

		private void kryptonTextBox1_TextChanged( object sender, EventArgs e )
		{
			if( disableEvents )
				return;

			if( double.TryParse( kryptonTextBox1.Text, out var newValue ) )
			{
				if( newValue != value )
				{
					value = newValue;

					disableEvents = true;
					SetTrackBarValue();
					disableEvents = false;

					ValueChanged?.Invoke( this, EventArgs.Empty );
				}
			}
		}

		private void kryptonLabel1_Paint( object sender, PaintEventArgs e )
		{

		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams handleParam = base.CreateParams;
				handleParam.ExStyle |= 0x02000000;//WS_EX_COMPOSITED       
				return handleParam;
			}
		}
	}
}
