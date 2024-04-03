#if !DEPLOY
namespace NeoAxis.Editor
{
    partial class SettingsCell_Properties
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
			this.hierarchicalContainerProperties = new NeoAxis.Editor.HierarchicalContainer();
			this.hierarchicalContainerEvents = new NeoAxis.Editor.HierarchicalContainer();
			this.toolStrip1 = new NeoAxis.Editor.EngineToolStrip();
			this.toolStripButtonProperties = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonEvents = new System.Windows.Forms.ToolStripButton();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// hierarchicalContainerProperties
			// 
			//this.hierarchicalContainerProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
   //         | System.Windows.Forms.AnchorStyles.Left) 
   //         | System.Windows.Forms.AnchorStyles.Right)));
			this.hierarchicalContainerProperties.ReverseGroups = false;
			this.hierarchicalContainerProperties.ContentMode = NeoAxis.Editor.HierarchicalContainer.ContentModeEnum.Properties;
			this.hierarchicalContainerProperties.Location = new System.Drawing.Point(0, 31);
			this.hierarchicalContainerProperties.Margin = new System.Windows.Forms.Padding(3, 1, 3, 1);
			this.hierarchicalContainerProperties.Name = "hierarchicalContainer1";
			this.hierarchicalContainerProperties.Size = new System.Drawing.Size(149, 102);
			this.hierarchicalContainerProperties.SplitterPosition = 67;
			this.hierarchicalContainerProperties.SplitterRatio = 0.4464286F;
			this.hierarchicalContainerProperties.TabIndex = 0;
			// 
			// hierarchicalContainerEvents
			// 
			//this.hierarchicalContainerEvents.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
			//| System.Windows.Forms.AnchorStyles.Left )
			//| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.hierarchicalContainerEvents.Visible = false;
			this.hierarchicalContainerEvents.ReverseGroups = true;
			this.hierarchicalContainerEvents.ContentMode = NeoAxis.Editor.HierarchicalContainer.ContentModeEnum.Events;
			this.hierarchicalContainerEvents.Location = new System.Drawing.Point( 0, 31 );
			this.hierarchicalContainerEvents.Margin = new System.Windows.Forms.Padding( 3, 1, 3, 1 );
			this.hierarchicalContainerEvents.Name = "hierarchicalContainer1";
			this.hierarchicalContainerEvents.Size = new System.Drawing.Size( 149, 102 );
			this.hierarchicalContainerEvents.SplitterPosition = 67;
			this.hierarchicalContainerEvents.SplitterRatio = 0.4464286F;
			this.hierarchicalContainerEvents.TabIndex = 0;
			// 
			// toolStrip1
			// 
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonProperties,
            this.toolStripButtonEvents});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.ShowItemToolTips = false;
			this.toolStrip1.AutoSize = false;
			this.toolStrip1.Size = new System.Drawing.Size(149, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonProperties
			// 
			this.toolStripButtonProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonProperties.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripButtonProperties.Name = "toolStripButtonProperties";
			this.toolStripButtonProperties.AutoSize = false;
			this.toolStripButtonProperties.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonProperties.Text = "Properties";
			this.toolStripButtonProperties.Click += new System.EventHandler(this.toolStripButtonProperties_Click);
			// 
			// toolStripButtonEvents
			// 
			this.toolStripButtonEvents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonEvents.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonEvents.ImageTransparentColor = System.Drawing.Color.Transparent;
			this.toolStripButtonEvents.Name = "toolStripButtonEvents";
			this.toolStripButtonEvents.AutoSize = false;
			this.toolStripButtonEvents.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonEvents.Text = "Events";
			this.toolStripButtonEvents.Click += new System.EventHandler(this.toolStripButtonEvents_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// SettingsCell_Properties
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CellsSortingPriority = -98F;
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.hierarchicalContainerProperties);
			this.Controls.Add(this.hierarchicalContainerEvents);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "SettingsCell_Properties";
			this.Size = new System.Drawing.Size(149, 135);
			this.Load += new System.EventHandler(this.SettingsCell_Properties_Load);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		private HierarchicalContainer hierarchicalContainerProperties;
		private HierarchicalContainer hierarchicalContainerEvents;
		private NeoAxis.Editor.EngineToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonProperties;
		private System.Windows.Forms.ToolStripButton toolStripButtonEvents;
		private System.Windows.Forms.Timer timer1;
	}
}

#endif