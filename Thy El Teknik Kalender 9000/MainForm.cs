using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Thy_El_Teknik_Kalender_9000.DataLayer;

namespace Thy_El_Teknik_Kalender_9000
{
  public partial class MainForm : Form
  {
    private void test(object sender, EventArgs e)
    {
      CalendarButton.Location = new Point(ClientSize.Width - (CalendarButton.Size.Width + 12), 12);

      Console.WriteLine(Width + " " + Height);
      Console.WriteLine(ClientSize.Width + ", " + ClientSize.Height);
    }
    public MainForm()
    {
      InitializeComponent();

      Log.ResetLogFile();

      SizeChanged += new EventHandler(test);
    }

    private void ShowMain(object sender, EventArgs e)
    {
      this.Show();
    }

    private void ToCalendar(object sender, EventArgs e)
    {
      //TODO maybe i should try to find different ways to bookmark
      CalendarEditor calendar = new CalendarEditor();
      calendar.Disposed += new EventHandler(ShowMain);
      calendar.VisibleChanged += new EventHandler(ShowMain);

      calendar.Show();
      this.Hide();
    }

    private void FocusConfig(object sender, EventArgs e, ConfigForm config)
    {
      config.Activate();
    }

    private void ToConfig(object sender, EventArgs e)
    {
      //TODO maybe i should try to find different ways to bookmark
      ConfigForm config = new ConfigForm();

      EventHandler evnthndlr = (object s, EventArgs ev) => FocusConfig(s, ev, config);

      Activated += evnthndlr;
      config.Disposed += (object s, EventArgs ev) =>
      {
        Activated -= evnthndlr;
      };

      config.Show();
    }

    private void button1_Click(object sender, EventArgs e)
    {

    }
  }
}
