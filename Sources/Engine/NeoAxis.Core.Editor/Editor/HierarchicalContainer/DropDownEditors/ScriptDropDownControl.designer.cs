namespace NeoAxis.Editor
{
	partial class ScriptDropDownControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.runScriptKryptonButton = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.scriptEditorControl = new NeoAxis.Editor.ScriptEditorControl();
			this.SuspendLayout();
			// 
			// runScriptKryptonButton
			// 
			this.runScriptKryptonButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.runScriptKryptonButton.Location = new System.Drawing.Point(5, 217);
			this.runScriptKryptonButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.runScriptKryptonButton.Name = "runScriptKryptonButton";
			this.runScriptKryptonButton.Size = new System.Drawing.Size(183, 31);
			this.runScriptKryptonButton.TabIndex = 1;
			this.runScriptKryptonButton.Values.Text = "Run";
			this.runScriptKryptonButton.Visible = false;
			// 
			// scriptEditorControl
			// 
			this.scriptEditorControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.scriptEditorControl.Location = new System.Drawing.Point(5, 4);
			this.scriptEditorControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.scriptEditorControl.Name = "scriptEditorControl";
			this.scriptEditorControl.Size = new System.Drawing.Size(615, 206);
			this.scriptEditorControl.TabIndex = 2;
			// 
			// ScriptDropDownControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.scriptEditorControl);
			this.Controls.Add(this.runScriptKryptonButton);
			this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this.Name = "ScriptDropDownControl";
			this.Size = new System.Drawing.Size(624, 260);
			this.ResumeLayout(false);

		}

		#endregion
		private ComponentFactory.Krypton.Toolkit.KryptonButton runScriptKryptonButton;
		private NeoAxis.Editor.ScriptEditorControl scriptEditorControl;
	}
}
