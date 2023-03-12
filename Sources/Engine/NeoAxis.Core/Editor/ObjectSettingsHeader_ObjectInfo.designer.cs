#if !DEPLOY
namespace NeoAxis.Editor
{
    partial class ObjectSettingsHeader_ObjectInfo
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
			this.kryptonLabel1 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.kryptonLabel2 = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.buttonTypeSettingsDefaultValue = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonTypeSettings = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.toolTip1 = new NeoAxis.Editor.EngineToolTip(this.components);
			this.SuspendLayout();
			// 
			// kryptonLabel1
			// 
			//this.kryptonLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
   //         | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonLabel1.AutoSize = false;
			this.kryptonLabel1.LabelStyle = Internal.ComponentFactory.Krypton.Toolkit.LabelStyle.BoldControl;
			this.kryptonLabel1.Location = new System.Drawing.Point(2, 2);
			this.kryptonLabel1.Name = "kryptonLabel1";
			this.kryptonLabel1.Size = new System.Drawing.Size(422, 25);
			this.kryptonLabel1.TabIndex = 7;
			this.kryptonLabel1.Values.Text = "{Object} {Object} {Object} {Object}";
			// 
			// kryptonLabel2
			// 
			//this.kryptonLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
   //         | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonLabel2.AutoSize = false;
			this.kryptonLabel2.Location = new System.Drawing.Point(112, 22);
			this.kryptonLabel2.Name = "kryptonLabel2";
			this.kryptonLabel2.Size = new System.Drawing.Size(480, 25);
			this.kryptonLabel2.TabIndex = 8;
			this.kryptonLabel2.Values.Text = "{Object} {Object} {Object} {Object}";
			this.kryptonLabel2.Visible = false;
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// buttonTypeSettingsDefaultValue
			// 
			//this.buttonTypeSettingsDefaultValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonTypeSettingsDefaultValue.Location = new System.Drawing.Point(432, 5);
			this.buttonTypeSettingsDefaultValue.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.buttonTypeSettingsDefaultValue.Name = "buttonTypeSettingsDefaultValue";
			this.buttonTypeSettingsDefaultValue.Size = new System.Drawing.Size(15, 15);
			this.buttonTypeSettingsDefaultValue.StateCommon.Border.Draw = Internal.ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettingsDefaultValue.StateCommon.Border.DrawBorders = ((Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.buttonTypeSettingsDefaultValue.StateNormal.Back.Draw = Internal.ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettingsDefaultValue.TabIndex = 10;
			this.buttonTypeSettingsDefaultValue.Values.Text = "";
			this.buttonTypeSettingsDefaultValue.Click += new System.EventHandler(this.buttonTypeSettingsDefaultValue_Click);
			// 
			// buttonTypeSettings
			// 
			//this.buttonTypeSettings.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
			//this.buttonTypeSettings.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonTypeSettings.Location = new System.Drawing.Point(450, 0);
			this.buttonTypeSettings.Name = "buttonTypeSettings";
			this.buttonTypeSettings.Size = new System.Drawing.Size(30, 28);
			this.buttonTypeSettings.StateCommon.Border.Draw = Internal.ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettings.StateCommon.Border.DrawBorders = ((Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | Internal.ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.buttonTypeSettings.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
			this.buttonTypeSettings.StateNormal.Back.Draw = Internal.ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonTypeSettings.TabIndex = 9;
			this.buttonTypeSettings.Values.Image = global::NeoAxis.Properties.Resources.Class;
			this.buttonTypeSettings.Values.Text = "";
			this.buttonTypeSettings.Click += new System.EventHandler(this.buttonTypeSettings_Click);
			// 
			// ObjectSettingsHeader_ObjectInfo
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonTypeSettingsDefaultValue);
			this.Controls.Add(this.buttonTypeSettings);
			this.Controls.Add(this.kryptonLabel1);
			this.Controls.Add(this.kryptonLabel2);
			this.Name = "ObjectSettingsHeader_ObjectInfo";
			this.Size = new System.Drawing.Size(480, 28);
			this.ResumeLayout(false);

        }

		#endregion

		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
		private System.Windows.Forms.Timer timer1;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonTypeSettingsDefaultValue;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonTypeSettings;
		private NeoAxis.Editor.EngineToolTip toolTip1;
	}
}

#endif