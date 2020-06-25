namespace NeoAxis.Editor
{
    partial class StoreDocumentWindow
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
			this.panelToolbar = new System.Windows.Forms.Panel();
			this.kryptonButtonHome = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonRefresh = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonForward = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonBack = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonTextBoxAddress = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.panelToolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelToolbar
			// 
			this.panelToolbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelToolbar.Controls.Add(this.kryptonButtonHome);
			this.panelToolbar.Controls.Add(this.kryptonButtonRefresh);
			this.panelToolbar.Controls.Add(this.kryptonButtonForward);
			this.panelToolbar.Controls.Add(this.kryptonButtonBack);
			this.panelToolbar.Controls.Add(this.kryptonTextBoxAddress);
			this.panelToolbar.Location = new System.Drawing.Point(0, 0);
			this.panelToolbar.Name = "panelToolbar";
			this.panelToolbar.Size = new System.Drawing.Size(477, 36);
			this.panelToolbar.TabIndex = 1;
			// 
			// kryptonButtonHome
			// 
			this.kryptonButtonHome.Location = new System.Drawing.Point(2, 2);
			this.kryptonButtonHome.Name = "kryptonButtonHome";
			this.kryptonButtonHome.Size = new System.Drawing.Size(32, 32);
			this.kryptonButtonHome.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonHome.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.kryptonButtonHome.StateDisabled.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonHome.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
			this.kryptonButtonHome.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonHome.TabIndex = 0;
			this.kryptonButtonHome.Values.Image = global::NeoAxis.Properties.Resources.House_16;
			this.kryptonButtonHome.Values.Text = "";
			this.kryptonButtonHome.Click += new System.EventHandler(this.kryptonButtonHome_Click);
			// 
			// kryptonButtonRefresh
			// 
			this.kryptonButtonRefresh.Location = new System.Drawing.Point(117, 2);
			this.kryptonButtonRefresh.Name = "kryptonButtonRefresh";
			this.kryptonButtonRefresh.Size = new System.Drawing.Size(32, 32);
			this.kryptonButtonRefresh.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonRefresh.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.kryptonButtonRefresh.StateDisabled.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonRefresh.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
			this.kryptonButtonRefresh.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonRefresh.TabIndex = 3;
			this.kryptonButtonRefresh.Values.Image = global::NeoAxis.Properties.Resources.Refresh_16;
			this.kryptonButtonRefresh.Values.Text = "";
			this.kryptonButtonRefresh.Click += new System.EventHandler(this.kryptonButtonRefresh_Click);
			// 
			// kryptonButtonForward
			// 
			this.kryptonButtonForward.Location = new System.Drawing.Point(79, 2);
			this.kryptonButtonForward.Name = "kryptonButtonForward";
			this.kryptonButtonForward.Size = new System.Drawing.Size(32, 32);
			this.kryptonButtonForward.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonForward.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.kryptonButtonForward.StateDisabled.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonForward.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
			this.kryptonButtonForward.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonForward.TabIndex = 2;
			this.kryptonButtonForward.Values.Image = global::NeoAxis.Properties.Resources.Forward_16;
			this.kryptonButtonForward.Values.Text = "";
			this.kryptonButtonForward.Click += new System.EventHandler(this.kryptonButtonForward_Click);
			// 
			// kryptonButtonBack
			// 
			this.kryptonButtonBack.Location = new System.Drawing.Point(41, 2);
			this.kryptonButtonBack.Name = "kryptonButtonBack";
			this.kryptonButtonBack.Size = new System.Drawing.Size(32, 32);
			this.kryptonButtonBack.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonBack.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.kryptonButtonBack.StateDisabled.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonBack.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
			this.kryptonButtonBack.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.kryptonButtonBack.TabIndex = 1;
			this.kryptonButtonBack.Values.Image = global::NeoAxis.Properties.Resources.Back_16;
			this.kryptonButtonBack.Values.Text = "";
			this.kryptonButtonBack.Click += new System.EventHandler(this.kryptonButtonBack_Click);
			// 
			// kryptonTextBoxAddress
			// 
			this.kryptonTextBoxAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.kryptonTextBoxAddress.Location = new System.Drawing.Point(162, 6);
			this.kryptonTextBoxAddress.Name = "kryptonTextBoxAddress";
			this.kryptonTextBoxAddress.Size = new System.Drawing.Size(306, 21);
			this.kryptonTextBoxAddress.TabIndex = 4;
			this.kryptonTextBoxAddress.WordWrap = false;
			this.kryptonTextBoxAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.kryptonTextBoxAddress_KeyDown);
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// StoreDocumentWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelToolbar);
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "StoreDocumentWindow";
			this.Size = new System.Drawing.Size(477, 277);
			this.WindowTitle = "Asset Store";
			this.Load += new System.EventHandler(this.StoreDocumentWindow_Load);
			this.Controls.SetChildIndex(this.panelToolbar, 0);
			this.panelToolbar.ResumeLayout(false);
			this.panelToolbar.PerformLayout();
			this.ResumeLayout(false);

        }

		#endregion

		private System.Windows.Forms.Panel panelToolbar;
		private System.Windows.Forms.Timer timer1;
		private ComponentFactory.Krypton.Toolkit.KryptonTextBox kryptonTextBoxAddress;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonBack;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonForward;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonRefresh;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonHome;
	}
}
