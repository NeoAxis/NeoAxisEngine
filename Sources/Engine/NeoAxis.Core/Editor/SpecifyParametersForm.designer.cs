namespace NeoAxis.Editor
{
	partial class SpecifyParametersForm
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
			this.buttonOK = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonCancel = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.labelError = new System.Windows.Forms.Label();
			this.hierarchicalContainer1 = new NeoAxis.Editor.HierarchicalContainer();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(283, 432);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(117, 32);
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Values.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(409, 432);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(117, 32);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Values.Text = "Cancel";
			// 
			// labelError
			// 
			this.labelError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelError.AutoSize = true;
			this.labelError.ForeColor = System.Drawing.Color.Red;
			this.labelError.Location = new System.Drawing.Point(16, 411);
			this.labelError.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelError.Name = "labelError";
			this.labelError.Size = new System.Drawing.Size(66, 17);
			this.labelError.TabIndex = 3;
			this.labelError.Text = "Error text";
			// 
			// hierarchicalContainer1
			// 
			this.hierarchicalContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hierarchicalContainer1.Location = new System.Drawing.Point(19, 16);
			this.hierarchicalContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.hierarchicalContainer1.Name = "hierarchicalContainer1";
			this.hierarchicalContainer1.Size = new System.Drawing.Size(507, 392);
			this.hierarchicalContainer1.SplitterPosition = 203;
			this.hierarchicalContainer1.SplitterRatio = 0.4F;
			this.hierarchicalContainer1.TabIndex = 4;
			// 
			// SpecifyParametersForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(542, 479);
			this.Controls.Add(this.hierarchicalContainer1);
			this.Controls.Add(this.labelError);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SpecifyParametersForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenameResourceDialog_FormClosing);
			this.Load += new System.EventHandler(this.OKCancelTextBoxForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonOK;
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonCancel;
		private System.Windows.Forms.Label labelError;
		private HierarchicalContainer hierarchicalContainer1;
	}
}