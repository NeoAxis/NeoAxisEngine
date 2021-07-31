namespace GenerateReferencePropertyCode
{
	partial class Form1
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
			this.textBoxClass = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textBoxPropertyName = new System.Windows.Forms.TextBox();
			this.textBoxPropertyType = new System.Windows.Forms.TextBox();
			this.textBoxDefaultValue = new System.Windows.Forms.TextBox();
			this.richTextBoxOutput = new System.Windows.Forms.RichTextBox();
			this.checkBoxHowToExpand = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.checkBoxCompact = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// textBoxClass
			// 
			this.textBoxClass.Location = new System.Drawing.Point(177, 17);
			this.textBoxClass.Name = "textBoxClass";
			this.textBoxClass.Size = new System.Drawing.Size(320, 22);
			this.textBoxClass.TabIndex = 0;
			this.textBoxClass.Text = "Component_";
			this.textBoxClass.TextChanged += new System.EventHandler(this.textBoxClass_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Class Name";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(103, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "Property Name";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 76);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(98, 17);
			this.label3.TabIndex = 2;
			this.label3.Text = "Property Type";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 104);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(157, 17);
			this.label4.TabIndex = 2;
			this.label4.Text = "Default Value (optional)";
			// 
			// textBoxPropertyName
			// 
			this.textBoxPropertyName.Location = new System.Drawing.Point(177, 45);
			this.textBoxPropertyName.Name = "textBoxPropertyName";
			this.textBoxPropertyName.Size = new System.Drawing.Size(320, 22);
			this.textBoxPropertyName.TabIndex = 1;
			this.textBoxPropertyName.Text = "Name";
			this.textBoxPropertyName.TextChanged += new System.EventHandler(this.textBoxPropertyName_TextChanged);
			// 
			// textBoxPropertyType
			// 
			this.textBoxPropertyType.Location = new System.Drawing.Point(177, 73);
			this.textBoxPropertyType.Name = "textBoxPropertyType";
			this.textBoxPropertyType.Size = new System.Drawing.Size(320, 22);
			this.textBoxPropertyType.TabIndex = 2;
			this.textBoxPropertyType.Text = "double";
			this.textBoxPropertyType.TextChanged += new System.EventHandler(this.textBoxPropertyType_TextChanged);
			// 
			// textBoxDefaultValue
			// 
			this.textBoxDefaultValue.Location = new System.Drawing.Point(177, 101);
			this.textBoxDefaultValue.Name = "textBoxDefaultValue";
			this.textBoxDefaultValue.Size = new System.Drawing.Size(320, 22);
			this.textBoxDefaultValue.TabIndex = 3;
			this.textBoxDefaultValue.Text = "0.0";
			this.textBoxDefaultValue.TextChanged += new System.EventHandler(this.textBoxDefaultValue_TextChanged);
			// 
			// richTextBoxOutput
			// 
			this.richTextBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.richTextBoxOutput.Location = new System.Drawing.Point(12, 160);
			this.richTextBoxOutput.Name = "richTextBoxOutput";
			this.richTextBoxOutput.Size = new System.Drawing.Size(887, 540);
			this.richTextBoxOutput.TabIndex = 6;
			this.richTextBoxOutput.Text = "";
			// 
			// checkBoxHowToExpand
			// 
			this.checkBoxHowToExpand.AutoSize = true;
			this.checkBoxHowToExpand.Location = new System.Drawing.Point(568, 48);
			this.checkBoxHowToExpand.Name = "checkBoxHowToExpand";
			this.checkBoxHowToExpand.Size = new System.Drawing.Size(164, 21);
			this.checkBoxHowToExpand.TabIndex = 5;
			this.checkBoxHowToExpand.Text = "Help. How to expand.";
			this.checkBoxHowToExpand.UseVisualStyleBackColor = true;
			this.checkBoxHowToExpand.CheckedChanged += new System.EventHandler(this.checkBoxHowToExpand_CheckedChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 133);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(63, 17);
			this.label5.TabIndex = 7;
			this.label5.Text = "Compact";
			// 
			// checkBoxCompact
			// 
			this.checkBoxCompact.AutoSize = true;
			this.checkBoxCompact.Checked = true;
			this.checkBoxCompact.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxCompact.Location = new System.Drawing.Point(177, 133);
			this.checkBoxCompact.Name = "checkBoxCompact";
			this.checkBoxCompact.Size = new System.Drawing.Size(18, 17);
			this.checkBoxCompact.TabIndex = 4;
			this.checkBoxCompact.UseVisualStyleBackColor = true;
			this.checkBoxCompact.CheckedChanged += new System.EventHandler(this.checkBoxCompact_CheckedChanged);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(911, 712);
			this.Controls.Add(this.checkBoxCompact);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.checkBoxHowToExpand);
			this.Controls.Add(this.richTextBoxOutput);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxDefaultValue);
			this.Controls.Add(this.textBoxPropertyType);
			this.Controls.Add(this.textBoxPropertyName);
			this.Controls.Add(this.textBoxClass);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Generate Reference Property Code";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox textBoxClass;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxPropertyName;
		private System.Windows.Forms.TextBox textBoxPropertyType;
		private System.Windows.Forms.TextBox textBoxDefaultValue;
		private System.Windows.Forms.RichTextBox richTextBoxOutput;
		private System.Windows.Forms.CheckBox checkBoxHowToExpand;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox checkBoxCompact;
	}
}

