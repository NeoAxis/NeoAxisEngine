namespace NeoAxis.Editor
{
    partial class NewObjectSettingsCell
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
			this.hierarchicalContainer1 = new NeoAxis.Editor.HierarchicalContainer();
			this.SuspendLayout();
			// 
			// hierarchicalContainer1
			// 
			this.hierarchicalContainer1.ContentMode = NeoAxis.Editor.HierarchicalContainer.ContentModeEnum.Properties;
			this.hierarchicalContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.hierarchicalContainer1.Location = new System.Drawing.Point(0, 0);
			this.hierarchicalContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.hierarchicalContainer1.Name = "hierarchicalContainer1";
			this.hierarchicalContainer1.Size = new System.Drawing.Size(492, 163);
			this.hierarchicalContainer1.SplitterPosition = 197;
			this.hierarchicalContainer1.SplitterRatio = 0.4F;
			this.hierarchicalContainer1.TabIndex = 0;
			// 
			// NewObjectSettingsCell
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.hierarchicalContainer1);
			this.Name = "NewObjectSettingsCell";
			this.Size = new System.Drawing.Size(492, 163);
			this.ResumeLayout(false);

        }

		#endregion

		private HierarchicalContainer hierarchicalContainer1;
	}
}
