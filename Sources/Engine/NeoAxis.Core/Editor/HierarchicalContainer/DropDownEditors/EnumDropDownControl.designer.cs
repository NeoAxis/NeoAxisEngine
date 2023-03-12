#if !DEPLOY
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
			this.listViewEnum = new NeoAxis.Editor.EngineListView();
			this.SuspendLayout();
			// 
			// listViewEnum
			// 
			this.listViewEnum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewEnum.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listViewEnum.Location = new System.Drawing.Point(0, 0);
			this.listViewEnum.Name = "listViewEnum";
			this.listViewEnum.Size = new System.Drawing.Size(299, 150);
			this.listViewEnum.TabIndex = 0;
			this.listViewEnum.ItemCheckedChanged += new NeoAxis.Editor.EngineListView.ItemCheckedChangedDelegate( this.listViewEnum_ItemCheckedChanged);
			this.listViewEnum.SelectedItemsChanged += new NeoAxis.Editor.EngineListView.SelectedItemsChangedDelegate( this.listViewEnum_SelectedItemsChanged );
			this.listViewEnum.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewEnum_MouseClick);
			this.listViewEnum.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewEnum_MouseDoubleClick);
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
		private NeoAxis.Editor.EngineListView listViewEnum;
	}
}

#endif