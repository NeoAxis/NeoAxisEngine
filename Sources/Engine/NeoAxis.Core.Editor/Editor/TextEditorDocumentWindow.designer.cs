namespace NeoAxis.Editor
{
    partial class TextEditorDocumentWindow
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
			this.avalonTextEditor = new NeoAxis.Editor.TextEditorControl();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// avalonTextEditor
			// 
			this.avalonTextEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.avalonTextEditor.Location = new System.Drawing.Point(0, 0);
			this.avalonTextEditor.Name = "avalonTextEditor";
			this.avalonTextEditor.Size = new System.Drawing.Size(477, 277);
			this.avalonTextEditor.TabIndex = 2;
			// 
			// timer1
			// 
			this.timer1.Enabled = true;
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// TextEditorDocumentWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.avalonTextEditor);
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "TextEditorDocumentWindow";
			this.Size = new System.Drawing.Size(477, 277);
			this.Load += new System.EventHandler(this.TextEditorDocumentWindow_Load);
			this.ResumeLayout(false);

        }

        #endregion
		private NeoAxis.Editor.TextEditorControl avalonTextEditor;
		private System.Windows.Forms.Timer timer1;
	}
}
