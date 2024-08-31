#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class ProcedureUIDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.workareaControl = new NeoAxis.Editor.EUserControl();
			this.buttonOK = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonCancel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.SuspendLayout();
			// 
			// workareaControl
			// 
			this.workareaControl.Location = new System.Drawing.Point( 8, 8 );
			this.workareaControl.Margin = new System.Windows.Forms.Padding( 4 );
			this.workareaControl.Name = "workareaControl";
			this.workareaControl.Size = new System.Drawing.Size( 500, 500 );
			this.workareaControl.TabIndex = 1;
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(197, 87);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(117, 32);
			this.buttonOK.TabIndex = 2;
			this.buttonOK.Values.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(323, 87);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(117, 32);
			this.buttonCancel.TabIndex = 3;
			this.buttonCancel.Values.Text = "Cancel";
			// 
			// ProcedureUIDialog
			//this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			//this.CancelButton = this.buttonCancel;
			//this.ClientSize = new System.Drawing.Size(456, 134);
			this.Controls.Add( this.workareaControl );
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProcedureUIDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProcedureUIDialog_FormClosing);
			this.Load += new System.EventHandler(this.ProcedureUIDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion

		private NeoAxis.Editor.EUserControl workareaControl;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonOK;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonCancel;
	}
}
#endif