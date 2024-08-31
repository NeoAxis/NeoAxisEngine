#if !DEPLOY
// Copyright (C) NeoAxis Group Ltd. 8 Copthall, Roseau Valley, 00152 Commonwealth of Dominica.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Internal.ComponentFactory.Krypton.Toolkit;

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

			public WinFormsButton( ProcedureUI.Form parent ) : base( parent ) { }
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

			public WinFormsCheck( ProcedureUI.Form parent ) : base( parent ) { }
		}

		/////////////////////////////////////////

		/// <summary>
		/// Implements edit box of <see cref="ProcedureUI"/> for WinForms.
		/// </summary>
		public class WinFormsEdit : ProcedureUI.Edit
		{
			public KryptonTextBox control;

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

			//public override CheckValue Checked
			//{
			//	get { return (CheckValue)control.CheckState; }
			//	set { control.CheckState = (CheckState)value; }
			//}

			//public override event Action<ProcedureUI.Check> CheckedChanged;
			//public void PerformCheckedChanged()
			//{
			//	CheckedChanged?.Invoke( this );
			//}

			//public override event Action<ProcedureUI.Check> Click;
			//public void PerformClick()
			//{
			//	Click?.Invoke( this );
			//}

			public WinFormsEdit( ProcedureUI.Form parent ) : base( parent ) { }
		}

		/////////////////////////////////////////

		/// <summary>
		/// Implements text label of <see cref="ProcedureUI"/> for WinForms.
		/// </summary>
		public class WinFormsText : ProcedureUI.Text
		{
			public EngineLabel control;

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

			public override bool Bold
			{
				get { return base.Bold; }
				set
				{
					if( Bold == value )
						return;
					base.Bold = value;

					control.LabelStyle = Bold ? LabelStyle.BoldControl : LabelStyle.NormalControl;
				}
			}

			//public override CheckValue Checked
			//{
			//	get { return (CheckValue)control.CheckState; }
			//	set { control.CheckState = (CheckState)value; }
			//}

			//public override event Action<ProcedureUI.Check> CheckedChanged;
			//public void PerformCheckedChanged()
			//{
			//	CheckedChanged?.Invoke( this );
			//}

			//public override event Action<ProcedureUI.Check> Click;
			//public void PerformClick()
			//{
			//	Click?.Invoke( this );
			//}

			public WinFormsText( ProcedureUI.Form parent ) : base( parent ) { }
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

				owner.Resize += Owner_Resize;
			}

			private void Owner_Resize( object sender, EventArgs e )
			{
				foreach( var control in owner.Controls )
				{
					var edit = control as KryptonTextBox;
					if( edit != null )
						EditUpdateSize( edit );

					var text = control as EngineLabel;
					if( text != null )
						TextUpdateSize( text );
				}
			}

			void EditUpdateSize( KryptonTextBox control )
			{
				control.Width = owner.ClientSize.Width - control.Location.X * 2;
			}

			void TextUpdateSize( EngineLabel control )
			{
				control.Width = owner.ClientSize.Width - control.Location.X * 2;
			}

			public override ProcedureUI.Button CreateButton( string text, ProcedureUI.Button.SizeEnum size )
			{
				var control = new KryptonButton();
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

				var result = new WinFormsButton( this );
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

				var result = new WinFormsCheck( this );
				result.control = control;
				control.Tag = result;

				return result;
			}

			public override ProcedureUI.Edit CreateEdit( string text = "" )
			{
				var control = new KryptonTextBox();
				control.Location = new Point( DpiHelper.Default.ScaleValue( 3 ), DpiHelper.Default.ScaleValue( 3 ) );
				control.Name = "control" + controlCount;
				controlCount++;
				//control.AutoSize = true;//control.Size = new Size( size == ProcedureUI.Button.SizeEnum.Long ? 147 : 117, 32 );
				control.TabIndex = controlCount;
				control.Text = text;
				//control.CheckStateChanged += CheckBox_CheckStateChanged;
				//control.Click += CheckBox_Click;
				owner.Controls.Add( control );

				var result = new WinFormsEdit( this );
				result.control = control;
				control.Tag = result;

				EditUpdateSize( control );

				return result;
			}

			public override ProcedureUI.Text CreateText( string text )
			{
				var control = new EngineLabel();
				control.Location = new Point( DpiHelper.Default.ScaleValue( 3 ), DpiHelper.Default.ScaleValue( 3 ) );
				control.Name = "control" + controlCount;
				controlCount++;
				//control.AutoSize = true;
				control.TabIndex = controlCount;
				control.Text = text;
				owner.Controls.Add( control );

				var result = new WinFormsText( this );
				result.control = control;
				control.Tag = result;

				TextUpdateSize( control );

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

					var edit = control as WinFormsEdit;
					if( edit != null )
					{
						edit.control.Location = new Point( positionX, positionY );// + DpiHelper.Default.ScaleValue( 6 ) );
						positionX += edit.control.Width + DpiHelper.Default.ScaleValue( 6 );

						maxSize = Math.Max( maxSize, edit.control.Height );
					}

					var text = control as WinFormsText;
					if( text != null )
					{
						text.control.Location = new Point( positionX, positionY );// + DpiHelper.Default.ScaleValue( 6 ) );
						positionX += text.control.Width + DpiHelper.Default.ScaleValue( 6 );

						maxSize = Math.Max( maxSize, text.control.Height );
					}
				}

				positionY += maxSize + DpiHelper.Default.ScaleValue( 5 );// 6;
			}

		}
	}
}

#endif