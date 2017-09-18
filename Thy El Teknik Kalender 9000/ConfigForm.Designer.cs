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
      this.label1 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // BackButton
      // 
      this.BackButton.Location = new System.Drawing.Point(303, 293);
      this.BackButton.Name = "BackButton";
      this.BackButton.Size = new System.Drawing.Size(75, 23);
      this.BackButton.TabIndex = 0;
      this.BackButton.Text = "Back";
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
      this.debugCheck.Location = new System.Drawing.Point(22, 140);
      this.debugCheck.Name = "debugCheck";
      this.debugCheck.Size = new System.Drawing.Size(109, 17);
      this.debugCheck.TabIndex = 5;
      this.debugCheck.Text = "Enable debug log";
      this.debugCheck.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.ForeColor = System.Drawing.Color.Red;
      this.label1.Location = new System.Drawing.Point(167, 268);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(311, 13);
      this.label1.TabIndex = 6;
      this.label1.Text = "Any change requires a full restart of the application to take effect";
      // 
      // ConfigForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(490, 328);
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
    private System.Windows.Forms.Label label1;
  }
}