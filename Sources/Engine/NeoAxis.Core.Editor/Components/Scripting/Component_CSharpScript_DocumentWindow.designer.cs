namespace NeoAxis.Editor
{
    partial class Component_CSharpScript_DocumentWindow
	{
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.scriptEditorControl = new NeoAxis.Editor.ScriptEditorControl();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// scriptEditorControl
			// 
			this.scriptEditorControl.Dock = System.Windows.Forms.DockStyle.Fill;
			//this.scriptEditorControl.IsDirty = false;
			this.scriptEditorControl.Location = new System.Drawing.Point(0, 0);
			this.scriptEditorControl.Margin = new System.Windows.Forms.Padding(4);
			this.scriptEditorControl.Name = "scriptEditorControl";
			this.scriptEditorControl.Size = new System.Drawing.Size(477, 277);
			this.scriptEditorControl.TabIndex = 0;
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// Component_CSharpScript_DocumentWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.scriptEditorControl);
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "Component_CSharpScript_DocumentWindow";
			this.Size = new System.Drawing.Size(477, 277);
			this.Load += new System.EventHandler(this.CSharpDocumentWindow_Load);
			this.ResumeLayout(false);

        }

        #endregion

		private NeoAxis.Editor.ScriptEditorControl scriptEditorControl;
		private System.Windows.Forms.Timer timer1;
	}
}
