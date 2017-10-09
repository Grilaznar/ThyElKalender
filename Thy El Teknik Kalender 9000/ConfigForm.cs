using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Thy_El_Teknik_Kalender_9000.Properties;
using Thy_El_Teknik_Kalender_9000.DataLayer;

namespace Thy_El_Teknik_Kalender_9000
{
  public partial class ConfigForm : Form
  {
    public ConfigForm()
    {
      InitializeComponent();

      //Load values from settings
      colorButton1.BackColor = Settings.Default.CustomColor1;
      colorButton2.BackColor = Settings.Default.CustomColor2;
      colorButton3.BackColor = Settings.Default.CustomColor3;

      customText1.Text = Settings.Default.CustomText1;
      customText2.Text = Settings.Default.CustomText2;
      customText3.Text = Settings.Default.CustomText3;

      SavePathBox.Text = DataLayer.ActivityFileHandler.CurrentSavePath;
      debugCheck.Checked = Settings.Default.Debug;
    }

    private void BrowseButton_Click(object sender, EventArgs e)
    {
      //Find folder
      DialogResult result = folderBrowser.ShowDialog();
      if (result == DialogResult.OK)
      {
        SavePathBox.Text = folderBrowser.SelectedPath;
      }
    }

    private void colorButton_clicked(object sender, EventArgs e)
    {
      //Find colors
      DialogResult result = colorDialog1.ShowDialog();
      if (result == DialogResult.OK)
      {
        ((Button)sender).BackColor = colorDialog1.Color;
      }
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
      //Save new path is it's the same one
      if (SavePathBox.Text != ActivityFileHandler.CurrentSavePath
        && SavePathBox.Text != "")
        ActivityFileHandler.CurrentSavePath = 
          Path.GetDirectoryName(SavePathBox.Text);

      //Save new settings
      Settings.Default.Debug = debugCheck.Checked;

      Settings.Default.CustomColor1 = colorButton1.BackColor;
      Settings.Default.CustomColor2 = colorButton2.BackColor;
      Settings.Default.CustomColor3 = colorButton3.BackColor;

      Settings.Default.CustomText1 = customText1.Text != null ?
        customText1.Text : "";
      Settings.Default.CustomText2 = customText2.Text != null ?
        customText2.Text : "";
      Settings.Default.CustomText3 = customText3.Text != null ?
        customText3.Text : "";

      this.Dispose();
    }

    private void BackButton_Click(object sender, EventArgs e)
    {
      //Don't save anything
      this.Dispose();
    }
  }
}
