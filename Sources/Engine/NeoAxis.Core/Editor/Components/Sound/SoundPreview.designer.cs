#if !DEPLOY
namespace NeoAxis.Editor
{
    partial class SoundPreview
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			this.buttonPlay = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.buttonLoopPlay = new Internal.ComponentFactory.Krypton.Toolkit.KryptonButton();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// buttonPlay
			// 
			this.buttonPlay.Location = new System.Drawing.Point(12, 12);
			this.buttonPlay.Name = "buttonPlay";
			this.buttonPlay.Size = new System.Drawing.Size(117, 32);
			this.buttonPlay.TabIndex = 1;
			this.buttonPlay.Values.Text = "Play";
			this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
			// 
			// buttonLoopPlay
			// 
			this.buttonLoopPlay.Location = new System.Drawing.Point(12, 50);
			this.buttonLoopPlay.Name = "buttonLoopPlay";
			this.buttonLoopPlay.Size = new System.Drawing.Size(117, 32);
			this.buttonLoopPlay.TabIndex = 2;
			this.buttonLoopPlay.Values.Text = "Loop Play";
			this.buttonLoopPlay.Click += new System.EventHandler(this.buttonLoopPlay_Click);
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// Sound_PreviewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonLoopPlay);
			this.Controls.Add(this.buttonPlay);
			this.Name = "Sound_PreviewControl";
			this.Size = new System.Drawing.Size(216, 113);
			this.Load += new System.EventHandler(this.Sound_SettingsCell_Load);
			this.ResumeLayout(false);

        }

		#endregion
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonPlay;
		private Internal.ComponentFactory.Krypton.Toolkit.KryptonButton buttonLoopPlay;
		private System.Windows.Forms.Timer timer1;
	}
}

#endif