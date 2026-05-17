namespace NeoAxis.Editor
{
	partial class ChatPromptForm
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
			this.buttonOK = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonCancel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.labelText = new System.Windows.Forms.Label();
			this.textBoxName = new Internal.ComponentFactory.Krypton.Toolkit.KryptonTextBox();
			this.checkBoxEditSelectedOnly = new Internal.ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
			this.labelError = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point( 197, 87 );
			this.buttonOK.Margin = new System.Windows.Forms.Padding( 4 );
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size( 117, 32 );
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Values.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point( 323, 87 );
			this.buttonCancel.Margin = new System.Windows.Forms.Padding( 4 );
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size( 117, 32 );
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Values.Text = "Cancel";
			// 
			// labelText
			// 
			this.labelText.AutoSize = true;
			this.labelText.Location = new System.Drawing.Point( 14, 11 );
			this.labelText.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
			this.labelText.Name = "labelText";
			this.labelText.Size = new System.Drawing.Size( 69, 17 );
			this.labelText.TabIndex = 0;
			this.labelText.Text = "labelText:";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point( 16, 31 );
			this.textBoxName.Margin = new System.Windows.Forms.Padding( 4 );
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Multiline = true;
			this.textBoxName.AcceptsReturn = true;
			this.textBoxName.Size = new System.Drawing.Size( 419, 64 );
			this.textBoxName.TabIndex = 1;
			this.textBoxName.TextChanged += new System.EventHandler( this.textBoxName_TextChanged );
			//
			// checkBoxEditSelectedOnly
			//			
			this.checkBoxEditSelectedOnly.Location = new System.Drawing.Point( 16, 100 );
			this.checkBoxEditSelectedOnly.Margin = new System.Windows.Forms.Padding( 8, 0, 8, 0 );
			this.checkBoxEditSelectedOnly.Name = "checkBoxEditSelectedOnly";
			this.checkBoxEditSelectedOnly.Size = new System.Drawing.Size( 200, 20 );
			this.checkBoxEditSelectedOnly.TabIndex = 2;
			this.checkBoxEditSelectedOnly.Values.Text = "Edit only the selected objects";
			// 
			// labelError
			// 
			this.labelError.AutoSize = true;
			this.labelError.Location = new System.Drawing.Point( 14, 130 );
			this.labelError.Margin = new System.Windows.Forms.Padding( 4, 0, 4, 0 );
			this.labelError.MaximumSize = new System.Drawing.Size( 370, 40 );
			this.labelError.Name = "labelError";
			//this.labelError.Size = new System.Drawing.Size( 66, 40 );
			this.labelError.TabIndex = 3;
			this.labelError.Text = "";// "To generate resources, use NeoX Forge in the NeoX app and the Stores window of the editor.";
			// 
			// ChatPromptForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF( 8F, 16F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size( 656, 168 );
			this.Controls.Add( this.labelText );
			this.Controls.Add( this.textBoxName );
			this.Controls.Add( this.checkBoxEditSelectedOnly );
			this.Controls.Add( this.labelError );
			this.Controls.Add( this.buttonCancel );
			this.Controls.Add( this.buttonOK );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding( 4 );
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ChatPromptForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Chat AI";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler( this.ChatPromptForm_FormClosing );
			this.Load += new System.EventHandler( this.ChatPromptForm_Load );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonOK;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonCancel;
		private System.Windows.Forms.Label labelText;
		internal Internal.ComponentFactory.Krypton.Toolkit.KryptonTextBox textBoxName;
		internal Internal.ComponentFactory.Krypton.Toolkit.KryptonCheckBox checkBoxEditSelectedOnly;
		private System.Windows.Forms.Label labelError;
	}
}