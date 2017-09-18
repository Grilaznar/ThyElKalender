using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Super_Great_Display_Client
{
  public partial class CalendarDisplay : Form
  {
    public CalendarDisplay()
    {
      InitializeComponent();
    }

    private void CalendarDisplayFindFile(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        Console.WriteLine("How about this");

        DialogResult result = fileFinder.ShowDialog();
        if(result == DialogResult.OK)
        {

        }
      }
    }

    private void dataGridView1_Layout(object sender, LayoutEventArgs e)
    {
      dataGridView1.Size = new Size(ClientSize.Width - 2, ClientSize.Height - 2);
      label1.Location = 
        new Point(
          dataGridView1.Width / 2 - label1.Width / 2, 
          dataGridView1.Height / 2 - label1.Height / 2);
    }
  }
}
