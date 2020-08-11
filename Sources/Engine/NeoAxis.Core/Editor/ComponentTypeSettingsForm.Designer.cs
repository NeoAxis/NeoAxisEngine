namespace NeoAxis.Editor
{
	partial class ComponentTypeSettingsForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.kryptonButtonCancel = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonOK = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonReset = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.labelEx1 = new NeoAxis.Editor.EngineLabel();
			this.hierarchicalContainer1 = new NeoAxis.Editor.HierarchicalContainer();
			this.SuspendLayout();
			// 
			// kryptonButtonCancel
			// 
			this.kryptonButtonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.kryptonButtonCancel.Location = new System.Drawing.Point(334, 581);
			this.kryptonButtonCancel.Name = "kryptonButtonCancel";
			this.kryptonButtonCancel.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonCancel.TabIndex = 3;
			this.kryptonButtonCancel.Values.Text = "Cancel";
			this.kryptonButtonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// kryptonButtonOK
			// 
			this.kryptonButtonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.kryptonButtonOK.Location = new System.Drawing.Point(211, 581);
			this.kryptonButtonOK.Name = "kryptonButtonOK";
			this.kryptonButtonOK.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonOK.TabIndex = 2;
			this.kryptonButtonOK.Values.Text = "OK";
			this.kryptonButtonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// kryptonButtonReset
			// 
			this.kryptonButtonReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonButtonReset.Location = new System.Drawing.Point(12, 581);
			this.kryptonButtonReset.Name = "kryptonButtonReset";
			this.kryptonButtonReset.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonReset.TabIndex = 1;
			this.kryptonButtonReset.Values.Text = "Reset";
			this.kryptonButtonReset.Click += new System.EventHandler(this.kryptonButtonReset_Click);
			// 
			// labelEx1
			// 
			this.labelEx1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelEx1.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
			this.labelEx1.Location = new System.Drawing.Point(12, 12);
			this.labelEx1.Name = "labelEx1";
			this.labelEx1.Size = new System.Drawing.Size(439, 23);
			this.labelEx1.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
			this.labelEx1.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.labelEx1.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.labelEx1.TabIndex = 4;
			this.labelEx1.Text = "Make visible:";
			// 
			// hierarchicalContainer1
			// 
			this.hierarchicalContainer1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
			| System.Windows.Forms.AnchorStyles.Left )
			| System.Windows.Forms.AnchorStyles.Right ) ) );
			this.hierarchicalContainer1.ContentMode = NeoAxis.Editor.HierarchicalContainer.ContentModeEnum.Properties;
			this.hierarchicalContainer1.Location = new System.Drawing.Point(12, 39);
			this.hierarchicalContainer1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.hierarchicalContainer1.Name = "hierarchicalContainer1";
			this.hierarchicalContainer1.Size = new System.Drawing.Size(439, 532);
			this.hierarchicalContainer1.SplitterPosition = 307;
			this.hierarchicalContainer1.SplitterRatio = 0.7F;
			this.hierarchicalContainer1.TabIndex = 0;
			// 
			// ComponentTypeSettingsForm
			// 
			this.AcceptButton = this.kryptonButtonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.kryptonButtonCancel;
			this.ClientSize = new System.Drawing.Size(463, 625);
			this.Controls.Add(this.labelEx1);
			this.Controls.Add(this.kryptonButtonReset);
			this.Controls.Add(this.kryptonButtonOK);
			this.Controls.Add(this.kryptonButtonCancel);
			this.Controls.Add(this.hierarchicalContainer1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ComponentTypeSettingsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Type Settings";
			this.Load += new System.EventHandler(this.ContentTypeSettingsForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private HierarchicalContainer hierarchicalContainer1;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonCancel;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonOK;
		private ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonReset;
		private EngineLabel labelEx1;
	}
}