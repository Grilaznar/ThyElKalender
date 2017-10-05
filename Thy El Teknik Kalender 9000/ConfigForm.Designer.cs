namespace Thy_El_Teknik_Kalender_9000
{
  partial class ConfigForm
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
      this.BackButton = new System.Windows.Forms.Button();
      this.OkButton = new System.Windows.Forms.Button();
      this.SavePathBox = new System.Windows.Forms.TextBox();
      this.BrowseButton = new System.Windows.Forms.Button();
      this.PathLabel = new System.Windows.Forms.Label();
      this.debugCheck = new System.Windows.Forms.CheckBox();
      this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
      this.colorDialog1 = new System.Windows.Forms.ColorDialog();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.colorButton1 = new System.Windows.Forms.Button();
      this.colorButton2 = new System.Windows.Forms.Button();
      this.colorButton3 = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.customText1 = new System.Windows.Forms.TextBox();
      this.customText2 = new System.Windows.Forms.TextBox();
      this.customText3 = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // BackButton
      // 
      this.BackButton.Location = new System.Drawing.Point(303, 293);
      this.BackButton.Name = "BackButton";
      this.BackButton.Size = new System.Drawing.Size(75, 23);
      this.BackButton.TabIndex = 0;
      this.BackButton.Text = "Cancel";
      this.BackButton.UseVisualStyleBackColor = true;
      this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
      // 
      // OkButton
      // 
      this.OkButton.Location = new System.Drawing.Point(403, 293);
      this.OkButton.Name = "OkButton";
      this.OkButton.Size = new System.Drawing.Size(75, 23);
      this.OkButton.TabIndex = 1;
      this.OkButton.Text = "Ok";
      this.OkButton.UseVisualStyleBackColor = true;
      this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
      // 
      // SavePathBox
      // 
      this.SavePathBox.Location = new System.Drawing.Point(22, 56);
      this.SavePathBox.Name = "SavePathBox";
      this.SavePathBox.Size = new System.Drawing.Size(413, 20);
      this.SavePathBox.TabIndex = 2;
      // 
      // BrowseButton
      // 
      this.BrowseButton.Location = new System.Drawing.Point(441, 56);
      this.BrowseButton.Name = "BrowseButton";
      this.BrowseButton.Size = new System.Drawing.Size(25, 20);
      this.BrowseButton.TabIndex = 3;
      this.BrowseButton.Text = "...";
      this.BrowseButton.UseVisualStyleBackColor = true;
      this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
      // 
      // PathLabel
      // 
      this.PathLabel.AutoSize = true;
      this.PathLabel.Location = new System.Drawing.Point(22, 37);
      this.PathLabel.Name = "PathLabel";
      this.PathLabel.Size = new System.Drawing.Size(206, 13);
      this.PathLabel.TabIndex = 4;
      this.PathLabel.Text = "Custom save path (leave blank for default)";
      // 
      // debugCheck
      // 
      this.debugCheck.AutoSize = true;
      this.debugCheck.Location = new System.Drawing.Point(22, 259);
      this.debugCheck.Name = "debugCheck";
      this.debugCheck.Size = new System.Drawing.Size(258, 17);
      this.debugCheck.TabIndex = 5;
      this.debugCheck.Text = "Enable debug log (Reqiures restart to take effect)";
      this.debugCheck.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(22, 130);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(81, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Custom Color 1:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(22, 155);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(81, 13);
      this.label2.TabIndex = 7;
      this.label2.Text = "Custom Color 2:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(22, 180);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(81, 13);
      this.label3.TabIndex = 8;
      this.label3.Text = "Custom Color 3:";
      // 
      // colorButton1
      // 
      this.colorButton1.BackColor = System.Drawing.SystemColors.Control;
      this.colorButton1.Location = new System.Drawing.Point(109, 125);
      this.colorButton1.Name = "colorButton1";
      this.colorButton1.Size = new System.Drawing.Size(23, 23);
      this.colorButton1.TabIndex = 9;
      this.colorButton1.UseVisualStyleBackColor = false;
      this.colorButton1.Click += new System.EventHandler(this.colorButton_clicked);
      // 
      // colorButton2
      // 
      this.colorButton2.Location = new System.Drawing.Point(109, 150);
      this.colorButton2.Name = "colorButton2";
      this.colorButton2.Size = new System.Drawing.Size(23, 23);
      this.colorButton2.TabIndex = 10;
      this.colorButton2.UseVisualStyleBackColor = true;
      this.colorButton2.Click += new System.EventHandler(this.colorButton_clicked);
      // 
      // colorButton3
      // 
      this.colorButton3.Location = new System.Drawing.Point(109, 175);
      this.colorButton3.Name = "colorButton3";
      this.colorButton3.Size = new System.Drawing.Size(23, 23);
      this.colorButton3.TabIndex = 11;
      this.colorButton3.UseVisualStyleBackColor = true;
      this.colorButton3.Click += new System.EventHandler(this.colorButton_clicked);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(156, 130);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(78, 13);
      this.label4.TabIndex = 12;
      this.label4.Text = "Custom Text 1:";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(156, 155);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(78, 13);
      this.label5.TabIndex = 13;
      this.label5.Text = "Custom Text 2:";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(156, 180);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(78, 13);
      this.label6.TabIndex = 14;
      this.label6.Text = "Custom Text 3:";
      // 
      // customText1
      // 
      this.customText1.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.customText1.Location = new System.Drawing.Point(240, 127);
      this.customText1.MaxLength = 5;
      this.customText1.Name = "customText1";
      this.customText1.Size = new System.Drawing.Size(25, 18);
      this.customText1.TabIndex = 15;
      // 
      // customText2
      // 
      this.customText2.Font = new System.Drawing.Font("Arial", 6.75F);
      this.customText2.Location = new System.Drawing.Point(240, 152);
      this.customText2.MaxLength = 5;
      this.customText2.Name = "customText2";
      this.customText2.Size = new System.Drawing.Size(25, 18);
      this.customText2.TabIndex = 16;
      // 
      // customText3
      // 
      this.customText3.Font = new System.Drawing.Font("Arial", 6.75F);
      this.customText3.Location = new System.Drawing.Point(240, 177);
      this.customText3.MaxLength = 5;
      this.customText3.Name = "customText3";
      this.customText3.Size = new System.Drawing.Size(25, 18);
      this.customText3.TabIndex = 17;
      // 
      // ConfigForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(490, 328);
      this.Controls.Add(this.customText3);
      this.Controls.Add(this.customText2);
      this.Controls.Add(this.customText1);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.colorButton3);
      this.Controls.Add(this.colorButton2);
      this.Controls.Add(this.colorButton1);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.debugCheck);
      this.Controls.Add(this.PathLabel);
      this.Controls.Add(this.BrowseButton);
      this.Controls.Add(this.SavePathBox);
      this.Controls.Add(this.OkButton);
      this.Controls.Add(this.BackButton);
      this.Name = "ConfigForm";
      this.Text = "Config";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button BackButton;
    private System.Windows.Forms.Button OkButton;
    private System.Windows.Forms.TextBox SavePathBox;
    private System.Windows.Forms.Button BrowseButton;
    private System.Windows.Forms.Label PathLabel;
    private System.Windows.Forms.CheckBox debugCheck;
    private System.Windows.Forms.FolderBrowserDialog folderBrowser;
    private System.Windows.Forms.ColorDialog colorDialog1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button colorButton1;
    private System.Windows.Forms.Button colorButton2;
    private System.Windows.Forms.Button colorButton3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox customText1;
    private System.Windows.Forms.TextBox customText2;
    private System.Windows.Forms.TextBox customText3;
  }
}