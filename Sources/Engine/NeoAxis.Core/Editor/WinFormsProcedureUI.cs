// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ComponentFactory.Krypton.Toolkit;

namespace NeoAxis.Editor
{
	/// <summary>
	/// Implements <see cref="ProcedureUI"/> for WinForms.
	/// </summary>
	public static class WinFormsProcedureUI
	{
		/// <summary>
		/// Implements button of <see cref="ProcedureUI"/> for WinForms.
		/// </summary>
		public class WinFormsButton : ProcedureUI.Button
		{
			public KryptonButton control;

			public override bool Enabled
			{
				get { return control.Enabled; }
				set { control.Enabled = value; }
			}

			public override bool Visible
			{
				get { return control.Visible; }
				set { control.Visible = value; }
			}

			public override string Text
			{
				get { return control.Text; }
				set { control.Text = value; }
			}

			public override event Action<ProcedureUI.Button> Click;
			public void PerformClick()
			{
				Click?.Invoke( this );
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Implements check box of <see cref="ProcedureUI"/> for WinForms.
		/// </summary>
		public class WinFormsCheck : ProcedureUI.Check
		{
			public KryptonCheckBox control;

			public override bool Enabled
			{
				get { return control.Enabled; }
				set { control.Enabled = value; }
			}

			public override bool Visible
			{
				get { return control.Visible; }
				set { control.Visible = value; }
			}

			public override string Text
			{
				get { return control.Text; }
				set { control.Text = value; }
			}

			public override CheckValue Checked
			{
				get { return (CheckValue)control.CheckState; }
				set { control.CheckState = (CheckState)value; }
			}

			public override event Action<ProcedureUI.Check> CheckedChanged;
			public void PerformCheckedChanged()
			{
				CheckedChanged?.Invoke( this );
			}

			public override event Action<ProcedureUI.Check> Click;
			public void PerformClick()
			{
				Click?.Invoke( this );
			}
		}

		/////////////////////////////////////////

		/// <summary>
		/// Implements form of <see cref="ProcedureUI"/> for WinForms.
		/// </summary>
		public class WinFormsForm : ProcedureUI.Form
		{
			public Control owner;
			int controlCount;
			public int positionY = 6;

			//

			public WinFormsForm( Control owner )
			{
				this.owner = owner;
			}

			public override ProcedureUI.Button CreateButton( string text, ProcedureUI.Button.SizeEnum size )
			{
				var control = new KryptonButton();
				//!!!!new
				control.Location = new Point( DpiHelper.Default.ScaleValue( 3 ), DpiHelper.Default.ScaleValue( 3 ) );
				//control.Location = new Point( 3, 3 );
				control.Name = "control" + controlCount;
				controlCount++;
				control.Size = DpiHelper.Default.ScaleValue( new Size( size == ProcedureUI.Button.SizeEnum.Long ? 117 : 93, 26 ) );
				//control.Size = new Size( size == ProcedureUI.Button.SizeEnum.Long ? 147 : 117, 32 );
				control.TabIndex = controlCount;
				control.Values.Text = text;
				control.Click += Button_Click;

				owner.Controls.Add( control );

				var result = new WinFormsButton();
				result.control = control;

				control.Tag = result;

				return result;
			}

			private void Button_Click( object sender, EventArgs e )
			{
				var control = (WinFormsButton)( (KryptonButton)sender ).Tag;
				control.PerformClick();
			}

			public override ProcedureUI.Check CreateCheck( string text )
			{
				var control = new KryptonCheckBox();
				control.Location = new Point( DpiHelper.Default.ScaleValue( 3 ), DpiHelper.Default.ScaleValue( 3 ) );
				control.Name = "control" + controlCount;
				controlCount++;
				control.AutoSize = true;//control.Size = new Size( size == ProcedureUI.Button.SizeEnum.Long ? 147 : 117, 32 );
				control.TabIndex = controlCount;
				control.Values.Text = text;
				control.CheckStateChanged += CheckBox_CheckStateChanged;
				control.Click += CheckBox_Click;

				owner.Controls.Add( control );

				var result = new WinFormsCheck();
				result.control = control;

				control.Tag = result;

				return result;
			}

			private void CheckBox_CheckStateChanged( object sender, EventArgs e )
			{
				var control = (WinFormsCheck)( (KryptonCheckBox)sender ).Tag;
				control.PerformCheckedChanged();
			}

			private void CheckBox_Click( object sender, EventArgs e )
			{
				var control = (WinFormsCheck)( (KryptonCheckBox)sender ).Tag;
				control.PerformClick();
			}

			public override void AddRow( IEnumerable<ProcedureUI.Control> controls )
			{
				int positionX = DpiHelper.Default.ScaleValue( 3 );
				int maxSize = 0;

				foreach( var control in controls )
				{
					var button = control as WinFormsButton;
					if( button != null )
					{
						button.control.Location = new Point( positionX, positionY );
						positionX += button.control.Width + DpiHelper.Default.ScaleValue( 6 );// 8;

						maxSize = Math.Max( maxSize, button.control.Height );
					}

					var _check = control as WinFormsCheck;
					if( _check != null )
					{
						_check.control.Location = new Point( positionX, positionY + DpiHelper.Default.ScaleValue( 6 ) );// 7 );
						positionX += _check.control.Width + DpiHelper.Default.ScaleValue( 6 );// 8;

						maxSize = Math.Max( maxSize, _check.control.Height );
					}
				}

				positionY += maxSize + DpiHelper.Default.ScaleValue( 5 );// 6;
			}
		}
	}
}
