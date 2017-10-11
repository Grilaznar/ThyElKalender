using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Thy_El_Teknik_Kalender_9000
{
  public partial class PasswordDialog : Form
  {
    public PasswordDialog()
    {
      InitializeComponent();
    }

    public new DialogResult Show()
    {
      return (ShowDialog());
    }

    public string Password()
    {
      DialogResult result = ShowDialog();
      if (result == DialogResult.OK)
        return textBox1.Text;

      return null;
    }
  }
}
