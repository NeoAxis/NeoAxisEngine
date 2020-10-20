namespace NeoAxis.Editor
{
	partial class HCGridEvent
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
			this.components = new System.ComponentModel.Container();
			this.labelName = new System.Windows.Forms.LabelExtended();
			this.eventToolTip = new NeoAxis.Editor.EngineToolTip(this.components);
			this.buttonEditEventHandlers = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonAddEventHandler = new ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.SuspendLayout();
			// 
			// labelName
			// 
			this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelName.AutoEllipsis = true;
			this.labelName.Location = new System.Drawing.Point(21, 3);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(175, 22);
			this.labelName.TabIndex = 0;
			this.labelName.Text = "{Name}";
			// 
			// buttonEditEventHandlers
			// 
			this.buttonEditEventHandlers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonEditEventHandlers.Location = new System.Drawing.Point(203, 3);
			this.buttonEditEventHandlers.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.buttonEditEventHandlers.Name = "buttonEditEventHandlers";
			this.buttonEditEventHandlers.Size = new System.Drawing.Size(26, 24);
			this.buttonEditEventHandlers.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonEditEventHandlers.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.buttonEditEventHandlers.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
			this.buttonEditEventHandlers.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonEditEventHandlers.TabIndex = 1;
			this.eventToolTip.SetToolTip(this.buttonEditEventHandlers, "Edit event handlers");
			this.buttonEditEventHandlers.Values.Image = global::NeoAxis.Properties.Resources.Edit_16;
			this.buttonEditEventHandlers.Values.Text = "";
			// 
			// buttonAddEventHandler
			// 
			this.buttonAddEventHandler.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddEventHandler.Location = new System.Drawing.Point(232, 3);
			this.buttonAddEventHandler.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.buttonAddEventHandler.Name = "buttonAddEventHandler";
			this.buttonAddEventHandler.Size = new System.Drawing.Size(26, 24);
			this.buttonAddEventHandler.StateCommon.Border.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonAddEventHandler.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
			this.buttonAddEventHandler.StateNormal.Back.Color1 = System.Drawing.Color.WhiteSmoke;
			this.buttonAddEventHandler.StateNormal.Back.Draw = ComponentFactory.Krypton.Toolkit.InheritBool.False;
			this.buttonAddEventHandler.TabIndex = 2;
			this.eventToolTip.SetToolTip(this.buttonAddEventHandler, "Add event handler");
			this.buttonAddEventHandler.Values.Image = global::NeoAxis.Properties.Resources.New_16;
			this.buttonAddEventHandler.Values.Text = "";
			// 
			// HCGridEvent
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonEditEventHandlers);
			this.Controls.Add(this.buttonAddEventHandler);
			this.Controls.Add(this.labelName);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HCGridEvent";
			this.Size = new System.Drawing.Size(266, 28);
			this.ResumeLayout(false);

		}

		#endregion
		public ComponentFactory.Krypton.Toolkit.KryptonButton buttonAddEventHandler;
		private System.Windows.Forms.LabelExtended labelName;
		public ComponentFactory.Krypton.Toolkit.KryptonButton buttonEditEventHandlers;
		private NeoAxis.Editor.EngineToolTip eventToolTip;
	}
}
