#if CLOUD
#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class LauncherEditRepositoryForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LauncherEditRepositoryForm));
			//this.buttonOK = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonCancel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.contentBrowser1 = new NeoAxis.Editor.ContentBrowser();// EngineListView();
			//this.bordersContainer1 = new NeoAxis.Editor.BordersContainer();
			//this.kryptonButtonSelectAll = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			//this.kryptonButtonDeselectAll = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			//this.labelText = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			//this.labelSelected = new System.Windows.Forms.Label();

			this.toolStripForTreeView = new NeoAxis.Editor.EngineToolStrip();
			this.toolStripButtonOptions = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonChange = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonGet = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonCommit = new System.Windows.Forms.ToolStripButton();

			this.toolStripForTreeView.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStrip
			// 
			this.toolStripForTreeView.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStripForTreeView.ImageScalingSize = new System.Drawing.Size( 20, 20 );
			this.toolStripForTreeView.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
			this.toolStripButtonOptions,
			this.toolStripButtonChange,
			this.toolStripButtonOpen,
			this.toolStripButtonGet,
			this.toolStripButtonCommit} );
			this.toolStripForTreeView.Location = new System.Drawing.Point( 16, 17 );
			this.toolStripForTreeView.Padding = new System.Windows.Forms.Padding( 1, 1, 1, 1 );
			//!!!!
			this.toolStripForTreeView.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStripForTreeView.Name = "toolStripForTreeView";
			this.toolStripForTreeView.ShowItemToolTips = false;
			this.toolStripForTreeView.CanOverflow = false;
			this.toolStripForTreeView.AutoSize = false;
			this.toolStripForTreeView.Size = new System.Drawing.Size( 511, 26 );
			this.toolStripForTreeView.TabIndex = 3;
			this.toolStripForTreeView.Text = "toolStrip1";

			// 
			// toolStripButtonOptions
			// 
			this.toolStripButtonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonOptions.Image = global::NeoAxis.Editor.Properties.Resources.Options_16;
			this.toolStripButtonOptions.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonOptions.Name = "toolStripButtonOptions";
			this.toolStripButtonOptions.AutoSize = false;
			this.toolStripButtonOptions.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonOptions.Text = "Options";
			this.toolStripButtonOptions.Click += new System.EventHandler( this.toolStripButtonOptions_Click );
			// 
			// toolStripButtonChange
			// 
			this.toolStripButtonChange.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonChange.Image = global::NeoAxis.Editor.Properties.Resources.EditFolder_16;
			this.toolStripButtonChange.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonChange.Name = "toolStripButtonChange";
			this.toolStripButtonChange.AutoSize = false;
			this.toolStripButtonChange.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonChange.Text = "Change the local repository folder.";
			this.toolStripButtonChange.Click += new System.EventHandler( this.toolStripButtonChange_Click );
			// 
			// toolStripButtonOpen
			// 
			this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonOpen.Image = global::NeoAxis.Editor.Properties.Resources.Folder_16;
			this.toolStripButtonOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonOpen.Name = "toolStripButtonOpen";
			this.toolStripButtonOpen.AutoSize = false;
			this.toolStripButtonOpen.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonOpen.Text = "Open the local repository folder in the Explorer.";
			this.toolStripButtonOpen.Click += new System.EventHandler( this.toolStripButtonOpen_Click );
			// 
			// toolStripButtonGet
			// 
			this.toolStripButtonGet.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonGet.Image = global::NeoAxis.Editor.Properties.Resources.MoveDown_16;
			this.toolStripButtonGet.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonGet.Name = "toolStripButtonGet";
			this.toolStripButtonGet.AutoSize = false;
			this.toolStripButtonGet.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonGet.Text = "Get all changes.";
			this.toolStripButtonGet.Click += new System.EventHandler( this.toolStripButtonGet_Click );
			// 
			// toolStripButtonCommit
			// 
			this.toolStripButtonCommit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonCommit.Image = global::NeoAxis.Editor.Properties.Resources.MoveUp_16;
			this.toolStripButtonCommit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripButtonCommit.Name = "toolStripButtonCommit";
			this.toolStripButtonCommit.AutoSize = false;
			this.toolStripButtonCommit.Size = new System.Drawing.Size( 23, 22 );
			this.toolStripButtonCommit.Text = "Commit all changes.";
			this.toolStripButtonCommit.Click += new System.EventHandler( this.toolStripButtonCommit_Click );

			// 
			// buttonOK
			// 
			//this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			//this.buttonOK.Location = new System.Drawing.Point(552, 671);
			//this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
			//this.buttonOK.Name = "buttonOK";
			//this.buttonOK.Size = new System.Drawing.Size(117, 32);
			//this.buttonOK.TabIndex = 4;
			//this.buttonOK.Values.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(678, 671);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(117, 32);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Values.Text = "Close";//"Cancel";
			// 
			// engineListView1
			// 
			//this.engineListView1.CanDrag = false;
			//this.engineListView1.CheckBoxes = true;
			//this.engineListView1.CurrentItem = null;
			//this.engineListView1.CurrentItemIndex = 0;
			this.contentBrowser1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.contentBrowser1.Location = new System.Drawing.Point( 16, 17 );
			//this.engineListView1.Location = new System.Drawing.Point(20, 37);
			this.contentBrowser1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.contentBrowser1.MultiSelect = true;
			this.contentBrowser1.Name = "contentBrowser";
			//this.engineListView1.SelectedItem = null;
			this.contentBrowser1.Size = new System.Drawing.Size(774, 619);
			this.contentBrowser1.TabIndex = 1;
			this.contentBrowser1.FilteringMode = null;
			this.contentBrowser1.Mode = NeoAxis.Editor.ContentBrowser.ModeEnum.Resources;
			this.contentBrowser1.ReadOnlyHierarchy = false;
			this.contentBrowser1.ThisIsSettingsWindow = false;
			// 
			// bordersContainer1
			// 
			//this.bordersContainer1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(213)))), ((int)(((byte)(213)))));
			//this.bordersContainer1.Location = new System.Drawing.Point(19, 36);
			//this.bordersContainer1.Name = "bordersContainer1";
			//this.bordersContainer1.Size = new System.Drawing.Size(776, 621);
			//this.bordersContainer1.TabIndex = 6;
			//this.bordersContainer1.Text = "bordersContainer1";
			// 
			// kryptonButtonSelectAll
			// 
			//this.kryptonButtonSelectAll.Location = new System.Drawing.Point(16/*20*/, 671);
			//this.kryptonButtonSelectAll.Margin = new System.Windows.Forms.Padding(4);
			//this.kryptonButtonSelectAll.Name = "kryptonButtonSelectAll";
			//this.kryptonButtonSelectAll.Size = new System.Drawing.Size(117, 32);
			//this.kryptonButtonSelectAll.TabIndex = 2;
			//this.kryptonButtonSelectAll.Values.Text = "Select All";
			//this.kryptonButtonSelectAll.Click += new System.EventHandler(this.kryptonButtonSelectAll_Click);
			// 
			// kryptonButtonDeselectAll
			// 
			//this.kryptonButtonDeselectAll.Location = new System.Drawing.Point( 141/*145*/, 671 );
			//this.kryptonButtonDeselectAll.Margin = new System.Windows.Forms.Padding(4);
			//this.kryptonButtonDeselectAll.Name = "kryptonButtonDeselectAll";
			//this.kryptonButtonDeselectAll.Size = new System.Drawing.Size(117, 32);
			//this.kryptonButtonDeselectAll.TabIndex = 3;
			//this.kryptonButtonDeselectAll.Values.Text = "Deselect All";
			//this.kryptonButtonDeselectAll.Click += new System.EventHandler(this.kryptonButtonDeselectAll_Click);
			// 
			// labelText
			// 
			//this.labelText.AutoSize = true;
			//this.labelText.Location = new System.Drawing.Point(19, 16);
			//this.labelText.Name = "labelText";
			//this.labelText.Size = new System.Drawing.Size(35, 17);
			//this.labelText.TabIndex = 7;
			//this.labelText.Text = "Text";
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// labelSelected
			// 
			//this.labelSelected.AutoSize = true;
			//this.labelSelected.Location = new System.Drawing.Point(272, 679);
			//this.labelSelected.Name = "labelSelected";
			//this.labelSelected.Size = new System.Drawing.Size(73, 17);
			//this.labelSelected.TabIndex = 8;
			//this.labelSelected.Text = "0 selected";
			// 
			// LauncherEditRepositoryForm
			// 
			//this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(1311, 918);
			//this.Controls.Add(this.labelSelected);
			//this.Controls.Add(this.labelText);
			this.Controls.Add(this.contentBrowser1);
			this.Controls.Add( this.toolStripForTreeView );
			//this.Controls.Add(this.bordersContainer1);
			this.Controls.Add(this.buttonCancel);
			//this.Controls.Add(this.kryptonButtonDeselectAll);
			//this.Controls.Add(this.kryptonButtonSelectAll);
			//this.Controls.Add(this.buttonOK);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LauncherEditRepositoryForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Cloudbox Repository Tool";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LauncherEditRepositoryForm_FormClosing);
			this.Load += new System.EventHandler(this.LauncherEditRepositoryForm_Load);
			this.toolStripForTreeView.ResumeLayout( false );
			this.toolStripForTreeView.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion

		//private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonOK;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonCancel;
		private ContentBrowser contentBrowser1;
		//private BordersContainer bordersContainer1;
		//private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonSelectAll;
		//private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonDeselectAll;
		//private System.Windows.Forms.Label labelText;
		private System.Windows.Forms.Timer timer1;
		//private System.Windows.Forms.Label labelSelected;

		private NeoAxis.Editor.EngineToolStrip toolStripForTreeView;
		private System.Windows.Forms.ToolStripButton toolStripButtonOptions;
		private System.Windows.Forms.ToolStripButton toolStripButtonChange;
		private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
		private System.Windows.Forms.ToolStripButton toolStripButtonGet;
		private System.Windows.Forms.ToolStripButton toolStripButtonCommit;
	}
}
#endif
#endif