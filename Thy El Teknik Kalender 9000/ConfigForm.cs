using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Thy_El_Teknik_Kalender_9000.Properties;

namespace Thy_El_Teknik_Kalender_9000
{
  public partial class ConfigForm : Form
  {
    public ConfigForm()
    {
      InitializeComponent();

      SavePathBox.Text = DataLayer.ActivityFileHandler.CurrentSavePath;
      debugCheck.Checked = Settings.Default.Debug;
    }

    private void BrowseButton_Click(object sender, EventArgs e)
    {
      DialogResult result = folderBrowser.ShowDialog();
      if (result == DialogResult.OK)
      {
        SavePathBox.Text = folderBrowser.SelectedPath;
      }
      Console.WriteLine(result);
      Console.WriteLine(folderBrowser.SelectedPath);
    }

    private void OkButton_Click(object sender, EventArgs e)
    {
      if (SavePathBox.Text != DataLayer.ActivityFileHandler.CurrentSavePath
        && SavePathBox.Text != "")
        Settings.Default.CustomSavePath = SavePathBox.Text;
      Settings.Default.Debug = debugCheck.Checked;
      this.Dispose();
    }

    private void BackButton_Click(object sender, EventArgs e)
    {
      this.Dispose();
    }
  }
}
