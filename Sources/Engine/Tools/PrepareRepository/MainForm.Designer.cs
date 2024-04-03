
namespace PrepareRepository
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBoxProjectPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonPrepare = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBoxProjectPath
			// 
			this.textBoxProjectPath.Location = new System.Drawing.Point(15, 41);
			this.textBoxProjectPath.Name = "textBoxProjectPath";
			this.textBoxProjectPath.Size = new System.Drawing.Size(661, 27);
			this.textBoxProjectPath.TabIndex = 0;
			this.textBoxProjectPath.Text = "C:\\_Temp";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(15, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(188, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "Folder of prepared project:";
			// 
			// buttonPrepare
			// 
			this.buttonPrepare.Location = new System.Drawing.Point(14, 85);
			this.buttonPrepare.Name = "buttonPrepare";
			this.buttonPrepare.Size = new System.Drawing.Size(117, 38);
			this.buttonPrepare.TabIndex = 2;
			this.buttonPrepare.Text = "Prepare";
			this.buttonPrepare.UseVisualStyleBackColor = true;
			this.buttonPrepare.Click += new System.EventHandler(this.buttonPrepare_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(955, 660);
			this.Controls.Add(this.buttonPrepare);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxProjectPath);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Prepare Repository";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxProjectPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonPrepare;
	}
}

