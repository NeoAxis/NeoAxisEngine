#if CLOUD
#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class RepositoryItemsForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryItemsForm));
			this.buttonOK = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonCancel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.engineListView1 = new NeoAxis.Editor.EngineListView();
			this.bordersContainer1 = new NeoAxis.Editor.BordersContainer();
			this.kryptonButtonSelectAll = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonDeselectAll = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.labelText = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.labelSelected = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(552, 671);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(117, 32);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Values.Text = "OK";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(678, 671);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(117, 32);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Values.Text = "Cancel";
			// 
			// engineListView1
			// 
			this.engineListView1.CanDrag = false;
			this.engineListView1.CheckBoxes = true;
			this.engineListView1.CurrentItem = null;
			this.engineListView1.CurrentItemIndex = 0;
			this.engineListView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
			this.engineListView1.Location = new System.Drawing.Point(20, 37);
			this.engineListView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.engineListView1.MultiSelect = true;
			this.engineListView1.Name = "engineListView1";
			this.engineListView1.SelectedItem = null;
			this.engineListView1.Size = new System.Drawing.Size(774, 619);
			this.engineListView1.TabIndex = 1;
			// 
			// bordersContainer1
			// 
			this.bordersContainer1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(213)))), ((int)(((byte)(213)))));
			this.bordersContainer1.Location = new System.Drawing.Point(19, 36);
			this.bordersContainer1.Name = "bordersContainer1";
			this.bordersContainer1.Size = new System.Drawing.Size(776, 621);
			this.bordersContainer1.TabIndex = 6;
			this.bordersContainer1.Text = "bordersContainer1";
			// 
			// kryptonButtonSelectAll
			// 
			this.kryptonButtonSelectAll.Location = new System.Drawing.Point(20, 671);
			this.kryptonButtonSelectAll.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonSelectAll.Name = "kryptonButtonSelectAll";
			this.kryptonButtonSelectAll.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonSelectAll.TabIndex = 2;
			this.kryptonButtonSelectAll.Values.Text = "Select All";
			this.kryptonButtonSelectAll.Click += new System.EventHandler(this.kryptonButtonSelectAll_Click);
			// 
			// kryptonButtonDeselectAll
			// 
			this.kryptonButtonDeselectAll.Location = new System.Drawing.Point(145, 671);
			this.kryptonButtonDeselectAll.Margin = new System.Windows.Forms.Padding(4);
			this.kryptonButtonDeselectAll.Name = "kryptonButtonDeselectAll";
			this.kryptonButtonDeselectAll.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonDeselectAll.TabIndex = 3;
			this.kryptonButtonDeselectAll.Values.Text = "Deselect All";
			this.kryptonButtonDeselectAll.Click += new System.EventHandler(this.kryptonButtonDeselectAll_Click);
			// 
			// labelText
			// 
			this.labelText.AutoSize = true;
			this.labelText.Location = new System.Drawing.Point(19, 16);
			this.labelText.Name = "labelText";
			this.labelText.Size = new System.Drawing.Size(35, 17);
			this.labelText.TabIndex = 7;
			this.labelText.Text = "Text";
			// 
			// timer1
			// 
			this.timer1.Interval = 50;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// labelSelected
			// 
			this.labelSelected.AutoSize = true;
			this.labelSelected.Location = new System.Drawing.Point(272, 679);
			this.labelSelected.Name = "labelSelected";
			this.labelSelected.Size = new System.Drawing.Size(73, 17);
			this.labelSelected.TabIndex = 8;
			this.labelSelected.Text = "0 selected";
			// 
			// RepositoryItemsForm
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(811, 718);
			this.Controls.Add(this.labelSelected);
			this.Controls.Add(this.labelText);
			this.Controls.Add(this.engineListView1);
			this.Controls.Add(this.bordersContainer1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.kryptonButtonDeselectAll);
			this.Controls.Add(this.kryptonButtonSelectAll);
			this.Controls.Add(this.buttonOK);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "RepositoryItemsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RenameResourceDialog_FormClosing);
			this.Load += new System.EventHandler(this.RepositoryItemsForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion

		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonOK;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonCancel;
		private EngineListView engineListView1;
		private BordersContainer bordersContainer1;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonSelectAll;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonDeselectAll;
		private System.Windows.Forms.Label labelText;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Label labelSelected;
	}
}
#endif
#endif