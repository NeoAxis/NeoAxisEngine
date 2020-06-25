namespace NeoAxis.Editor
{
    partial class ScreenNotificationForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScreenNotificationForm));
			this.lifeTimer = new System.Windows.Forms.Timer(this.components);
			this.labelBody = new System.Windows.Forms.Label();
			this.labelTitle = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// lifeTimer
			// 
			this.lifeTimer.Tick += new System.EventHandler(this.lifeTimer_Tick);
			// 
			// labelBody
			// 
			this.labelBody.BackColor = System.Drawing.Color.Transparent;
			this.labelBody.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelBody.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelBody.ForeColor = System.Drawing.Color.White;
			this.labelBody.Location = new System.Drawing.Point(0, 0);
			this.labelBody.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelBody.Name = "labelBody";
			this.labelBody.Size = new System.Drawing.Size(459, 48);
			this.labelBody.TabIndex = 0;
			this.labelBody.Text = "Text Text Text";
			this.labelBody.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelBody.Click += new System.EventHandler(this.labelRO_Click);
			// 
			// labelTitle
			// 
			this.labelTitle.BackColor = System.Drawing.Color.Transparent;
			this.labelTitle.Font = new System.Drawing.Font("Calibri", 12.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTitle.ForeColor = System.Drawing.Color.Gainsboro;
			this.labelTitle.Location = new System.Drawing.Point(4, 1);
			this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(49, 26);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "title goes here";
			this.labelTitle.Visible = false;
			this.labelTitle.Click += new System.EventHandler(this.labelTitle_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 10;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// ScreenNotificationForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(87)))), ((int)(((byte)(154)))));
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(459, 48);
			this.ControlBox = false;
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.labelBody);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScreenNotificationForm";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Notification";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ScreenNotificationForm_FormClosed);
			this.Load += new System.EventHandler(this.ScreenNotificationForm_Load);
			this.Click += new System.EventHandler(this.ScreenNotificationForm_Click);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer lifeTimer;
        private System.Windows.Forms.Label labelBody;
        private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.Timer timer1;
	}
}