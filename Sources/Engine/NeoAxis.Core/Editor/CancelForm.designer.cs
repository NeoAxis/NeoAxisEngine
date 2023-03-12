#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class CancelForm
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
			this.buttonCancel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.labelText = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(170, 87);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(117, 32);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Values.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelText
			// 
			this.labelText.Location = new System.Drawing.Point(16, 23);
			this.labelText.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelText.Name = "labelText";
			this.labelText.Size = new System.Drawing.Size(424, 43);
			this.labelText.TabIndex = 2;
			this.labelText.Text = "Label text ";
			this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// CancelForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(456, 134);
			this.Controls.Add(this.labelText);
			this.Controls.Add(this.buttonCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CancelForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form";
			this.TopMost = true;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CancelForm_FormClosing);
			this.Load += new System.EventHandler(this.OKCancelTextBoxForm_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonCancel;
		private System.Windows.Forms.Label labelText;
	}
}
#endif