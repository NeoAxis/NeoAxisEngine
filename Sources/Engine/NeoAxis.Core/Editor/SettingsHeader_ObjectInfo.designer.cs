namespace NeoAxis.Editor
{
    partial class SettingsHeader_ObjectInfo
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
			this.buttonTypeSettings = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.kryptonLabel1 = new NeoAxis.Editor.LabelEx();
			this.kryptonLabel2 = new NeoAxis.Editor.LabelEx();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.buttonTypeSettingsDefaultValue = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.SuspendLayout();
			// 
			// buttonTypeSettings
			// 
			this.buttonTypeSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonTypeSettings.Location = new System.Drawing.Point(317, 0);
			this.buttonTypeSettings.Name = "buttonTypeSettings";
			this.buttonTypeSettings.Size = new System.Drawing.Size(30, 26);
			this.buttonTypeSettings.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettings.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.buttonTypeSettings.StateDisabled.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettings.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
			this.buttonTypeSettings.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettings.TabIndex = 2;
			this.toolTip1.SetToolTip(this.buttonTypeSettings, "Type Settings");
			this.buttonTypeSettings.Values.Image = global::NeoAxis.Properties.Resources.Class;
			this.buttonTypeSettings.Values.Text = "";
			this.buttonTypeSettings.Click += new System.EventHandler(this.buttonTypeSettings_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 10;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// kryptonLabel1
			// 
			this.kryptonLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F);
			this.kryptonLabel1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.BoldControl;
			this.kryptonLabel1.Location = new System.Drawing.Point(0, -1);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(295, 26);
			this.kryptonLabel1.StateCommon.Content.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.kryptonLabel1.TabIndex = 6;
			this.kryptonLabel1.Text = "{Object} {Object} {Object} {Object}";
			// 
			// kryptonLabel2
			// 
			this.kryptonLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonLabel2.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.kryptonLabel2.Location = new System.Drawing.Point(0, 24);
			this.kryptonLabel2.Name = "kryptonLabel2";
			this.kryptonLabel2.Size = new System.Drawing.Size(295, 23);
			this.kryptonLabel2.TabIndex = 6;
			this.kryptonLabel2.Text = "{Object} {Object} {Object} {Object}";
			// 
			// buttonTypeSettingsDefaultValue
			// 
			this.buttonTypeSettingsDefaultValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonTypeSettingsDefaultValue.Location = new System.Drawing.Point(299, 5);
			this.buttonTypeSettingsDefaultValue.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.buttonTypeSettingsDefaultValue.Name = "buttonTypeSettingsDefaultValue";
			this.buttonTypeSettingsDefaultValue.Size = new System.Drawing.Size(15, 15);
			this.buttonTypeSettingsDefaultValue.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettingsDefaultValue.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.buttonTypeSettingsDefaultValue.StateDisabled.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettingsDefaultValue.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettingsDefaultValue.TabIndex = 7;
			this.toolTip1.SetToolTip(this.buttonTypeSettingsDefaultValue, "Type Settings reset to default");
			this.buttonTypeSettingsDefaultValue.Values.Text = "";
			this.buttonTypeSettingsDefaultValue.Click += new System.EventHandler(this.buttonTypeSettingsDefaultValue_Click);
			// 
			// SettingsHeader_ObjectInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonTypeSettingsDefaultValue);
			this.Controls.Add(this.kryptonLabel1);
			this.Controls.Add(this.kryptonLabel2);
			this.Controls.Add(this.buttonTypeSettings);
			this.Name = "SettingsHeader_ObjectInfo";
			this.Size = new System.Drawing.Size(347, 48);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonTypeSettings;
		private System.Windows.Forms.Timer timer1;
		private LabelEx kryptonLabel1;
		private LabelEx kryptonLabel2;
		private System.Windows.Forms.ToolTip toolTip1;
		private ComponentFactory.Krypton.Toolkit.KryptonButton buttonTypeSettingsDefaultValue;
	}
}
