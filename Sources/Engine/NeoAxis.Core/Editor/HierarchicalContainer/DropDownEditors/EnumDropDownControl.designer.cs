namespace NeoAxis.Editor
{
	partial class EnumDropDownControl
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
			this.listViewEnum = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// listViewEnum
			// 
			this.listViewEnum.Alignment = System.Windows.Forms.ListViewAlignment.Left;
			this.listViewEnum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewEnum.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listViewEnum.CheckBoxes = true;
			this.listViewEnum.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewEnum.FullRowSelect = true;
			this.listViewEnum.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewEnum.HideSelection = false;
			this.listViewEnum.Location = new System.Drawing.Point(0, 0);
			this.listViewEnum.Margin = new System.Windows.Forms.Padding(4);
			this.listViewEnum.MultiSelect = false;
			this.listViewEnum.Name = "listViewEnum";
			this.listViewEnum.ShowGroups = false;
			this.listViewEnum.ShowItemToolTips = true;
			this.listViewEnum.Size = new System.Drawing.Size(299, 150);
			this.listViewEnum.TabIndex = 0;
			this.listViewEnum.UseCompatibleStateImageBehavior = false;
			this.listViewEnum.View = System.Windows.Forms.View.Details;
			this.listViewEnum.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listViewEnum_ItemCheck);
			this.listViewEnum.SelectedIndexChanged += new System.EventHandler(this.listViewEnum_SelectedIndexChanged);
			this.listViewEnum.SizeChanged += new System.EventHandler(this.listViewEnum_SizeChanged);
			this.listViewEnum.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewEnum_MouseClick);
			this.listViewEnum.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewEnum_MouseDoubleClick);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 200;
			// 
			// EnumDropDownControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.listViewEnum);
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "EnumDropDownControl";
			this.Size = new System.Drawing.Size(299, 150);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ListView listViewEnum;
		private System.Windows.Forms.ColumnHeader columnHeader1;
	}
}
