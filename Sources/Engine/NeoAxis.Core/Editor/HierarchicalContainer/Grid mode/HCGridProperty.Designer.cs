namespace NeoAxis.Editor
{
	partial class HCGridProperty
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
			this.propertyToolTip = new NeoAxis.Editor.EngineToolTip(this.components);
			this.SuspendLayout();
			// 
			// labelName
			// 
			this.labelName.AutoEllipsis = true;
			this.labelName.Location = new System.Drawing.Point(22, 6);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(55, 17);
			this.labelName.TabIndex = 1;
			this.labelName.Text = "{Name}";
			// 
			// HCGridProperty
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelName);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "HCGridProperty";
			this.Size = new System.Drawing.Size(516, 28);
			this.ResumeLayout(false);

		}

		#endregion
		//public ComponentFactory.Krypton.Toolkit.KryptonButton buttonReference;
		//public ComponentFactory.Krypton.Toolkit.KryptonButton buttonExpand;
		private System.Windows.Forms.LabelExtended labelName;
		//private ComponentFactory.Krypton.Toolkit.KryptonButton buttonDefaultValue;
		//public ComponentFactory.Krypton.Toolkit.KryptonButton buttonType;
		private NeoAxis.Editor.EngineToolTip propertyToolTip;
	}
}
