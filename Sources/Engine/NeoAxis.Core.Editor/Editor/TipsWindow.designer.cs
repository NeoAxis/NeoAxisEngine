#if !DEPLOY
namespace NeoAxis.Editor
{
	partial class TipsWindow
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
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.kryptonCheckBoxShowTipsAtStartup = new Internal.ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
			this.kryptonButtonPrevious = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonNext = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.kryptonButtonClose = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.tipNumberLabel = new Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// kryptonCheckBoxShowTipsAtStartup
			// 
			this.kryptonCheckBoxShowTipsAtStartup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.kryptonCheckBoxShowTipsAtStartup.Location = new System.Drawing.Point(14, 18);
			this.kryptonCheckBoxShowTipsAtStartup.Name = "kryptonCheckBoxShowTipsAtStartup";
			this.kryptonCheckBoxShowTipsAtStartup.Size = new System.Drawing.Size(155, 20);
			this.kryptonCheckBoxShowTipsAtStartup.TabIndex = 0;
			this.kryptonCheckBoxShowTipsAtStartup.Values.Text = "Show tips at startup";
			this.kryptonCheckBoxShowTipsAtStartup.CheckedChanged += new System.EventHandler(this.kryptonCheckBoxShowTipsAtStartup_CheckedChanged);
			// 
			// kryptonButtonPrevious
			// 
			this.kryptonButtonPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonPrevious.Location = new System.Drawing.Point(384, 12);
			this.kryptonButtonPrevious.Name = "kryptonButtonPrevious";
			this.kryptonButtonPrevious.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonPrevious.TabIndex = 1;
			this.kryptonButtonPrevious.Values.Text = "Previous";
			this.kryptonButtonPrevious.Click += new System.EventHandler(this.kryptonButtonPrevious_Click);
			// 
			// kryptonButtonNext
			// 
			this.kryptonButtonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonNext.Location = new System.Drawing.Point(507, 12);
			this.kryptonButtonNext.Name = "kryptonButtonNext";
			this.kryptonButtonNext.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonNext.TabIndex = 2;
			this.kryptonButtonNext.Values.Text = "Next";
			this.kryptonButtonNext.Click += new System.EventHandler(this.kryptonButtonNext_Click);
			// 
			// kryptonButtonClose
			// 
			this.kryptonButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.kryptonButtonClose.Location = new System.Drawing.Point(630, 12);
			this.kryptonButtonClose.Name = "kryptonButtonClose";
			this.kryptonButtonClose.Size = new System.Drawing.Size(117, 32);
			this.kryptonButtonClose.TabIndex = 3;
			this.kryptonButtonClose.Values.Text = "Close";
			this.kryptonButtonClose.Click += new System.EventHandler(this.kryptonButtonClose_Click);
			// 
			// tipNumberLabel
			// 
			this.tipNumberLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.tipNumberLabel.Location = new System.Drawing.Point(303, 14);
			this.tipNumberLabel.Name = "tipNumberLabel";
			this.tipNumberLabel.Size = new System.Drawing.Size(69, 28);
			this.tipNumberLabel.StateCommon.ShortText.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tipNumberLabel.TabIndex = 7;
			this.tipNumberLabel.Values.Text = "10/10";
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.Location = new System.Drawing.Point(12, 42);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(734, 439);
			this.panel1.TabIndex = 8;
			this.panel1.Visible = false;
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.Controls.Add(this.kryptonCheckBoxShowTipsAtStartup);
			this.panel2.Controls.Add(this.kryptonButtonPrevious);
			this.panel2.Controls.Add(this.tipNumberLabel);
			this.panel2.Controls.Add(this.kryptonButtonNext);
			this.panel2.Controls.Add(this.kryptonButtonClose);
			this.panel2.Location = new System.Drawing.Point(0, 480);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(759, 56);
			this.panel2.TabIndex = 9;
			// 
			// TipsWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			//this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.471698F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.Margin = new System.Windows.Forms.Padding(5);
			this.Name = "TipsWindow";
			this.Size = new System.Drawing.Size(759, 535);
			this.WindowTitle = "Tips";
			this.Load += new System.EventHandler(this.TipsWindow_Load);
			this.Controls.SetChildIndex(this.panel1, 0);
			this.Controls.SetChildIndex(this.panel2, 0);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Timer timer1;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonCheckBox kryptonCheckBoxShowTipsAtStartup;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonPrevious;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonNext;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton kryptonButtonClose;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonLabel tipNumberLabel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
	}
}

#endif