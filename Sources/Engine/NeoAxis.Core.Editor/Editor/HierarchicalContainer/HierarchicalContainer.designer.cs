#if !DEPLOY
namespace NeoAxis.Editor
{
    partial class HierarchicalContainer
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
			this.timer50ms = new System.Windows.Forms.Timer(this.components);
			this.engineScrollBar1 = new NeoAxis.Editor.EngineScrollBar();
			this.panel1 = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// timer50ms
			// 
			this.timer50ms.Interval = 10;
			this.timer50ms.Tick += new System.EventHandler(this.timer10ms_Tick);
			// 
			// engineScrollBar1
			// 
			this.engineScrollBar1.Location = new System.Drawing.Point(500, 0);
			this.engineScrollBar1.Name = "engineScrollBar1";
			this.engineScrollBar1.Size = new System.Drawing.Size(19, 624);
			this.engineScrollBar1.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(450, 531);
			this.panel1.TabIndex = 1;
			// 
			// HierarchicalContainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.engineScrollBar1);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HierarchicalContainer";
			this.Size = new System.Drawing.Size(519, 624);
			this.Load += new System.EventHandler(this.HierarchicalContainer_Load);
			this.ResumeLayout(false);

        }

        #endregion
		private System.Windows.Forms.Timer timer50ms;
		private EngineScrollBar engineScrollBar1;
		private System.Windows.Forms.Panel panel1;
	}
}

#endif