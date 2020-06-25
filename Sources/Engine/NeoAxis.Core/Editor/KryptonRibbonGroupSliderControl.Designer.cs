namespace NeoAxis.Editor
{
	partial class KryptonRibbonGroupSliderControl
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
			this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonTrackBar1 = new ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
			this.kryptonTextBox1 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
			this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			this.kryptonSplitContainer2 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
			this.kryptonSplitContainer1.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
			this.kryptonSplitContainer1.Panel2.SuspendLayout();
			this.kryptonSplitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel1)).BeginInit();
			this.kryptonSplitContainer2.Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel2)).BeginInit();
			this.kryptonSplitContainer2.Panel2.SuspendLayout();
			this.kryptonSplitContainer2.SuspendLayout();
			this.SuspendLayout();
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.AutoSize = false;
			this.kryptonLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonLabel1.Location = new System.Drawing.Point(0, 0);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(110, 62);
			this.kryptonLabel1.StateCommon.ShortText.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.kryptonLabel1.StateCommon.ShortText.TextH = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Center;
			this.kryptonLabel1.StateCommon.ShortText.TextV = ComponentFactory.Krypton.Toolkit.PaletteRelativeAlign.Near;
			this.kryptonLabel1.TabIndex = 0;
			this.kryptonLabel1.Values.Text = "kryptonLabel1";
			this.kryptonLabel1.Paint += new System.Windows.Forms.PaintEventHandler(this.kryptonLabel1_Paint);
			// 
			// kryptonTrackBar1
			// 
			this.kryptonTrackBar1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonTrackBar1.DrawBackground = true;
			this.kryptonTrackBar1.LargeChange = 100;
			this.kryptonTrackBar1.Location = new System.Drawing.Point(0, 0);
			this.kryptonTrackBar1.Maximum = 1000;
			this.kryptonTrackBar1.Name = "kryptonTrackBar1";
			this.kryptonTrackBar1.Size = new System.Drawing.Size(75, 25);
			this.kryptonTrackBar1.SmallChange = 10;
			this.kryptonTrackBar1.TabIndex = 1;
			this.kryptonTrackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
			this.kryptonTrackBar1.ValueChanged += new System.EventHandler(this.kryptonTrackBar1_ValueChanged);
			// 
			// kryptonTextBox1
			// 
			this.kryptonTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonTextBox1.Location = new System.Drawing.Point(0, 0);
			this.kryptonTextBox1.Name = "kryptonTextBox1";
			this.kryptonTextBox1.Size = new System.Drawing.Size(30, 23);
			this.kryptonTextBox1.TabIndex = 2;
			this.kryptonTextBox1.Text = "1.0";
			this.kryptonTextBox1.TextChanged += new System.EventHandler(this.kryptonTextBox1_TextChanged);
			// 
			// kryptonSplitContainer1
			// 
			this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.kryptonSplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.kryptonSplitContainer1.IsSplitterFixed = true;
			this.kryptonSplitContainer1.Location = new System.Drawing.Point(0, 0);
			this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
			// 
			// kryptonSplitContainer1.Panel1
			// 
			this.kryptonSplitContainer1.Panel1.Controls.Add(this.kryptonTrackBar1);
			// 
			// kryptonSplitContainer1.Panel2
			// 
			this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonTextBox1);
			this.kryptonSplitContainer1.Panel2MinSize = 28;
			this.kryptonSplitContainer1.Size = new System.Drawing.Size(110, 25);
			this.kryptonSplitContainer1.SplitterDistance = 75;
			this.kryptonSplitContainer1.SplitterPercent = 0.68181818181818177D;
			this.kryptonSplitContainer1.TabIndex = 3;
			// 
			// kryptonSplitContainer2
			// 
			this.kryptonSplitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonSplitContainer2.Cursor = System.Windows.Forms.Cursors.Default;
			this.kryptonSplitContainer2.IsSplitterFixed = true;
			this.kryptonSplitContainer2.Location = new System.Drawing.Point(0, 0);
			this.kryptonSplitContainer2.Name = "kryptonSplitContainer2";
			this.kryptonSplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// kryptonSplitContainer2.Panel1
			// 
			this.kryptonSplitContainer2.Panel1.Controls.Add(this.kryptonSplitContainer1);
			this.kryptonSplitContainer2.Panel1MinSize = 22;
			// 
			// kryptonSplitContainer2.Panel2
			// 
			this.kryptonSplitContainer2.Panel2.Controls.Add(this.kryptonLabel1);
			this.kryptonSplitContainer2.Panel2MinSize = 10;
			this.kryptonSplitContainer2.Size = new System.Drawing.Size(110, 92);
			this.kryptonSplitContainer2.SplitterDistance = 25;
			this.kryptonSplitContainer2.SplitterPercent = 0.27173913043478259D;
			this.kryptonSplitContainer2.TabIndex = 4;
			// 
			// KryptonRibbonGroupSliderControl
			// 
			this.Controls.Add(this.kryptonSplitContainer2);
			this.Name = "KryptonRibbonGroupSliderControl";
			this.Size = new System.Drawing.Size(110, 92);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
			this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
			this.kryptonSplitContainer1.Panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
			this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
			this.kryptonSplitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
			this.kryptonSplitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel1)).EndInit();
			this.kryptonSplitContainer2.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2.Panel2)).EndInit();
			this.kryptonSplitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer2)).EndInit();
			this.kryptonSplitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		public ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
		public ComponentFactory.Krypton.Toolkit.KryptonTrackBar kryptonTrackBar1;
		public ComponentFactory.Krypton.Toolkit.KryptonTextBox kryptonTextBox1;
		public ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer2;
		public ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
	}
}
