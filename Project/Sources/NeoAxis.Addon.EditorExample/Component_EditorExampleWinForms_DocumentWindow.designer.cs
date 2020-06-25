namespace NeoAxis.Addon.EditorExample
{
    partial class Component_EditorExampleWinForms_DocumentWindow
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
			this.kryptonTextBox1 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
			this.button1 = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.SuspendLayout();
			// 
			// kryptonTextBox1
			// 
			this.kryptonTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonTextBox1.Location = new System.Drawing.Point(15, 65);
			this.kryptonTextBox1.Multiline = true;
			this.kryptonTextBox1.Name = "kryptonTextBox1";
			this.kryptonTextBox1.Size = new System.Drawing.Size(691, 357);
			this.kryptonTextBox1.TabIndex = 1;
			this.kryptonTextBox1.Text = "TextBox";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(15, 19);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(147, 32);
			this.button1.TabIndex = 0;
			this.button1.Values.Text = "Text";
			// 
			// Component_EditorExampleWinForms_DocumentWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button1);
			this.Controls.Add(this.kryptonTextBox1);
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "Component_EditorExampleWinForms_DocumentWindow";
			this.Size = new System.Drawing.Size(722, 436);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		private ComponentFactory.Krypton.Toolkit.KryptonTextBox kryptonTextBox1;
		private ComponentFactory.Krypton.Toolkit.KryptonButton button1;
	}
}
