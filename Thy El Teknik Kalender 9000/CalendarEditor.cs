using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Thy_El_Teknik_Kalender_9000.ModelLayer;
using Thy_El_Teknik_Kalender_9000.DataLayer;
using Thy_El_Teknik_Kalender_9000.Properties;

namespace Thy_El_Teknik_Kalender_9000
{
  public partial class CalendarEditor : Form
  {
    private Dictionary<Person, List<Activity>> calendarData =
      new Dictionary<Person, List<Activity>>(new PersonComparer());
    private DateTime activeTimeStart, activeTimeEnd;
    //private ActivityData dataSaver = new ActivityData();
    private List<int> holidayColumns = new List<int>();

    private Color holidayColor = Color.Aquamarine;
    private Color weekendColor = Color.Gray;
    private Color offdayColor = Color.LightGreen;
    private Color balancedayColor = Color.Green;
    private Color courseColor = Color.Honeydew;
    private Color projectColor = Color.Beige;

    private Color errorColor = Color.Red;
    private Color unknownColor = Color.Pink;

    private bool unsavedChanges = false;

    public CalendarEditor()
    {
      Log.Add("Calenar window starting");
      InitializeComponent();

      Log.ResetLogFile();

      FormClosing += CloseWindow;

      MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;
      MinimumSize = new Size(440, 250);

      //SizeChanged += new EventHandler(test);

      // Enable double buffering on calendar datagrid for massive damage (to drawing time)
      typeof(DataGridView).InvokeMember(
        "DoubleBuffered",
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
        null,
        dataGridView1,
        new object[] { true });
      Log.Add("Calendar window started");
    }

    #region Init methods
    private void InitDataGrid(object sender, EventArgs e)
    {
      Log.Add("Calendar window interior initializing");
      Log.Add("Loading windowstate");
      LoadWindowState();

      Log.Add("Add comlumns to People grid");
      dataGridView2.Columns.Add("Names", "Names");
      dataGridView2.Columns.Add("Department", "Department");

      int nameCellWidth = 118;
      int depCellWidth = 78;
      dataGridView2.Columns[0].Width = nameCellWidth;
      dataGridView2.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
      dataGridView2.Columns[1].Width = depCellWidth;
      dataGridView2.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

      InitInputs();

      //filltestdata();

      Log.Add("Initializing calendargrid");
      CreateDateRow();

      Log.Add("Setting current time");
      UpdateCalendarActiveTimespan(this, null);
      Log.Add("Calendar window interior initialized");

      //Task<Dictionary<Person, List<Activity>>> t =
      //  Task<Dictionary<Person, List<Activity>>>.Factory
      //    .StartNew(ActivityFileHandler.ReadData);
      //Thread t = new Thread(new ParameterizedThreadStart(ThreadAccessToAddRows));
      //AddDataRows(ActivityFileHandler.ReadData());
      AddDataRows(Task<Dictionary<Person, List<Activity>>>.Factory
          .StartNew(ActivityFileHandler.ReadData).Result);

    }

    private void InitInputs()
    {
      Log.Add("Initializing inputs");
      //Activity comparison test
      if (new Activity(DateTime.Now.Date, Activity.activityType.Offday) ==
          new Activity(DateTime.Now.Date, Activity.activityType.Offday))
        Console.WriteLine("THIS HAPPENED");

      //Set earliest day to today, as activetimespan can't handle negatives just yet
      dateTimePicker1.MinDate = DateTime.Now.Date;
      dateTimePicker1.ValueChanged += UpdateCalendarActiveTimespan;

      foreach (Activity.activityType actType in Enum.GetValues(typeof(Activity.activityType)))
      {
        if (actType == 0) continue;
        activityPicker.Items.Add(actType);
        contextMenuStrip1.Items.Add(actType.ToString(), null, (object s, EventArgs ev) => { MarkSelected(actType); });
      }
      contextMenuStrip1.Items.Add("Clear", null, (object s, EventArgs ev) => { UnmarkSelected(this, null); });

      activityPicker.SelectedIndex = 0;
    }

    private void HotkeyDetection(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
      {
        Save_Click(this, e);
        e.Handled = true;
      }
      if (e.KeyCode == Keys.R && e.Modifiers == Keys.Control)
      {
        UpdateButton(this, e);
        e.Handled = true;
      }
    }
    #endregion

    #region Window State
    private void LoadWindowState()
    {
      // Set window location
      if (Settings.Default.CalendarLocation != null)
      {
        this.Location = Settings.Default.CalendarLocation;
      }

      // Set window size
      if (Settings.Default.CalendarSize != null)
      {
        this.Size = Settings.Default.CalendarSize;
      }
    }

    private void SaveWindowState(object sender, FormClosingEventArgs e)
    {
      // Copy window location to app settings
      Settings.Default.CalendarLocation = this.Location;

      // Copy window size to app settings
      if (this.WindowState == FormWindowState.Normal)
      {
        Settings.Default.CalendarSize = this.Size;
      }
      else
      {
        Settings.Default.CalendarSize = this.RestoreBounds.Size;
      }

      Settings.Default.Save();
    }

    private void FitDatagrid(object sender, LayoutEventArgs e)
    {
      dataGridView1.Size = new Size(ClientSize.Width - (dataGridView1.Location.X + 12), ClientSize.Height - 24);
      dataGridView2.Size = new Size(dataGridView2.Size.Width, ClientSize.Height - 24);
      SaveButton.Location = new Point(SaveButton.Location.X, ClientSize.Height - 12 - SaveButton.Size.Height);
      BackButton.Location = new Point(BackButton.Location.X, ClientSize.Height - 12 - BackButton.Size.Height);
    }
    #endregion

    #region Addrows
    private void CreateDateRow()
    {
      if (dataGridView1.ColumnCount == 0) dataGridView1.ColumnCount = 1;
      dataGridView2.Rows.Add(new string[] { "", "" });
      dataGridView1.Rows.Add();
      dataGridView1.Rows[dataGridView1.Rows.Count - 1].Height = dataGridView2.Rows[0].Height;
      dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.Font = new Font("Ariel", 10);
      Log.Add("Date row created");
    }

    private void AddDataRow(Person person, List<Activity> activities)
    {
      if (AddPerson(person, activities) != 0)
      {
        dataGridView2[0, dataGridView2.Rows.Count - 2].Style.BackColor = errorColor;
      };

      dataGridView2.Rows.Add(new string[] { person.Name, person.Department });
      dataGridView1.Rows.Add();
      dataGridView1.Rows[dataGridView1.Rows.Count - 1].Height = dataGridView2.Rows[0].Height;

      int numberofDays = activities == null ? 0 : activities.Count;
      Log.Add("Data row for: " + person.Name + " created, with " + activities.Count + " days marked");
      UpdateCalendarContent();
    }

    private void AddDataRows(Dictionary<Person, List<Activity>> dataDict)
    {
      Log.Add("Adding " + dataDict.Count + " people to grid");
      foreach (KeyValuePair<Person, List<Activity>> pair in dataDict)
      {
        calendarData.Add(pair.Key, pair.Value);

        dataGridView2.Rows.Add(new string[] { pair.Key.Name, pair.Key.Department });
        dataGridView1.Rows.Add();
        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Height = dataGridView2.Rows[0].Height;

        int numberofDays = pair.Value == null ? 0 : pair.Value.Count;
        Log.Add("Data row for: " + pair.Key.Name + " created, with " + numberofDays + " days marked");
      }

      UpdateCalendarContent();
    }

    private int AddPerson(Person person, List<Activity> activities, int retries = 0)
    {
      if (retries > 10) return -1;
      if (retries > 0)
      {
        person.Name += "-";
      }
      try
      {
        calendarData.Add(person, activities);
      }
      catch (ArgumentException e)
      {
        Console.WriteLine(e.Message);
        return AddPerson(person, activities, retries + 1);
      }
      return retries;
    }

    private bool ChangePersonName(Person person, string name)
    {
      if (!calendarData.ContainsKey(new Person(name)))
      {
        List<Activity> list = calendarData[person];
        calendarData.Remove(person);
        person.Name = name;

        calendarData.Add(person, list);

        return true;
      }
      return false;
    }
    #endregion

    #region Remove rows
    private void RemoveRow(int index)
    {
      if (index < 1)
      {
        calendarData.Remove(new Person(dataGridView2[0, index].Value.ToString()));
        dataGridView2.Rows.RemoveAt(index);
      }
    }

    private void ClearDatagrid()
    {
      while (dataGridView1.RowCount > 1)
      {
        calendarData.Remove(new Person(dataGridView2[0, 1].Value.ToString()));
        dataGridView2.Rows.RemoveAt(1);
      }
    }
    #endregion

    #region CalendarMethods

    private void UpdateCalendarActiveTimespan(object sender, EventArgs e)
    {
      Log.Add("Update active time area");
      DateTime pickedDate = FetchDateFromTimepicker();

      activeTimeStart =
        pickedDate.AddDays(pickedDate.DayOfWeek == 0 ?
          -6 :
          1 - (double)pickedDate.DayOfWeek);

      Console.WriteLine(activeTimeStart);

      //Console.WriteLine(startDate);
      //Console.WriteLine(startDate.DayOfWeek);

      int weeksToShow = (int)weekNumber.Value;
      int daysToShow = weeksToShow * 7;
      for (int i = 0; i < dataGridView1.ColumnCount; i++)
      {
        for (int j = 0; j < dataGridView1.RowCount; j++)
        {
          dataGridView1[i, j].Style.BackColor = //Color.White;
            dataGridView1.Columns[i].DefaultCellStyle.BackColor;
        }
      }
      dataGridView1.ColumnCount = daysToShow;
      activeTimeEnd = activeTimeStart.AddDays(daysToShow);

      //FindHolidays();

      UpdateCalendarContent();
    }

    private void UpdateCalendarContent()
    {
      Log.Add("Updateing calendar content");
      SetupCalendarColumns();

      List<Activity> currentList;
      for (int i = 0; i < dataGridView1.RowCount; i++)
      {
        if (i == 0)
        {
          WriteDateRow();
          continue;
        }
        string name = dataGridView2[0, i].Value.ToString();
        currentList = calendarData[new Person(name)];
        if (currentList == null) currentList = calendarData[new Person(name)] = new List<Activity>();
        foreach (Activity activity in currentList)
        {
          if (activity.Date < activeTimeEnd && activity.Date >= activeTimeStart)
          {
            dataGridView1[(activity.Date - activeTimeStart).Days, i]
              .Style.BackColor =
                ActivityColor(activity.ActivityCode);

            dataGridView1[(activity.Date - activeTimeStart).Days, i].Value =
              ActivityText(activity.ActivityCode);

          }
        }
      }
      if (activeTimeStart <= DateTime.Now && DateTime.Now < activeTimeEnd)
      {
        dataGridView1[(DateTime.Now - activeTimeStart).Days, 0].Style.BackColor = Color.LightSkyBlue;
      }

      dataGridView1.ClearSelection();
      dataGridView2.ClearSelection();
    }

    private void SetupCalendarColumns()
    {
      Log.Add("Setting up columns for calendar grid");
      int cellSize = dataGridView2.Rows[0].Height;
      for (int i = 0; i < dataGridView1.ColumnCount; i++)
      {
        dataGridView1.Columns[i].Width = cellSize;
        dataGridView1.Columns[i].SortMode =
          DataGridViewColumnSortMode.NotSortable;

        if (isColumnHoliday(i))
        {
          dataGridView1.Columns[i].DefaultCellStyle.BackColor =
            holidayColor;
        }
        else if (IsColumnWeekend(i))
        {
          dataGridView1.Columns[i].DefaultCellStyle.BackColor =
            weekendColor;
        }
        else
        {
          dataGridView1.Columns[i].DefaultCellStyle.BackColor =
            SystemColors.Window;
        }
        for (int j = 1; j < dataGridView1.RowCount; j++)
        {
          dataGridView1[i, j].Value = "";
        }
      }
    }

    private void WriteDateRow()
    {
      dataGridView1.Rows[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
      dataGridView1.Rows[0].DefaultCellStyle.Font = new Font("Ariel", 7);
      for (int j = 0; j < dataGridView1.ColumnCount; j++)
      {
        dataGridView1[j, 0].Value = activeTimeStart.AddDays(j).ToString().Substring(0, 5);
      }
    }
    #endregion

    #region Click events

    private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      dataGridView2.ClearSelection();
      Console.WriteLine("Number of cells you have chosen:");
      Console.WriteLine(dataGridView1.SelectedCells.Count);
    }

    private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      Color backcolor = dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor;
      if (backcolor == offdayColor || backcolor == projectColor)
        UnmarkSelected(sender, e);
      else
        MarkSelected((Activity.activityType)activityPicker.SelectedItem);
    }

    private void DeselectAll(object sender, EventArgs e)
    {
      dataGridView1.ClearSelection();
      dataGridView2.ClearSelection();
      this.Focus();
    }
    #endregion

    #region Button events
    private void UpdateButton(object sender, EventArgs e)
    {
      ClearDatagrid();
      AddDataRows(ActivityFileHandler.ReadData());
    }

    private void CloseButton(object sender, EventArgs e)
    {
      CloseWindow(sender, null);
    }

    private void CloseWindow(object sender, FormClosingEventArgs e)
    {
      if (unsavedChanges)
      {
        DialogResult result = MessageBox.Show(
            "You have unsaved changes, do you want to save before closing?",
            "Unsaved changes",
            MessageBoxButtons.YesNoCancel);
        if (result == DialogResult.Yes)
        {
          ActivityFileHandler.SaveData(calendarData);
          SaveWindowState(this, null);
        }
        else if (result == DialogResult.No)
        {
          SaveWindowState(this, null);
        }
        else if (result == DialogResult.Cancel)
        {
          if (e != null) e.Cancel = true;
          return;
        }
      }

      this.Dispose();
    }

    private void MarkClicked(object sender, EventArgs e)
    {
      MarkSelected((Activity.activityType)activityPicker.SelectedItem);
    }

    private void MarkSelected(Activity.activityType markType)
    {
      Console.WriteLine(Size.Width);
      foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
      {
        if (!IsCellMarkable(cell) || markType == 0) continue;
        Log.Add("marking date: " + activeTimeStart.AddDays(cell.ColumnIndex));

        Activity newActivity = new Activity(
            activeTimeStart.AddDays(cell.ColumnIndex), markType);

        //Find relevant list
        List<Activity> relevantList =
          calendarData[new Person(dataGridView2[0, cell.RowIndex].Value.ToString())];

        Activity foundActivity = relevantList.Find(act => act.Date == newActivity.Date);

        //If entry doesn't exist, add to list
        if (foundActivity == null)
        {
          //calendarData[new Person(dataGridView2[0, cell.RowIndex].Value.ToString())]
          //  .Add(new Activity(
          //    activeTimeStart.AddDays(cell.ColumnIndex),
          //    (Activity.activityType)activityType));
          relevantList.Add(newActivity);

          cell.Style.BackColor = ActivityColor(markType);
          cell.Value = ActivityText(markType);

          MarkedForSave();
        }
        else if (foundActivity.ActivityCode != newActivity.ActivityCode)
        {
          foundActivity.ActivityCode = newActivity.ActivityCode;

          cell.Style.BackColor = ActivityColor(markType);
          cell.Value = ActivityText(markType);

          MarkedForSave();
        }
      }
      dataGridView1.ClearSelection();
    }

    private void UnmarkSelected(object sender, EventArgs e)
    {
      foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
      {
        if (cell.RowIndex == 0) continue;
        calendarData[new Person(dataGridView2[0, cell.RowIndex].Value.ToString())]
          .RemoveAll(d => d.Date == activeTimeStart.AddDays(cell.ColumnIndex));
        cell.Style.BackColor =
          cell.OwningColumn.DefaultCellStyle.BackColor;
        cell.Value = "";

        MarkedForSave();
      }
      dataGridView1.ClearSelection();
    }

    private void Save_Click(object sender, EventArgs e)
    {
      ActivityFileHandler.SaveData(calendarData);
      unsavedChanges = false;
      SaveButton.BackColor = BackButton.BackColor;
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

    private void weekNumber_ValueChanged(object sender, EventArgs e)
    {
      UpdateCalendarActiveTimespan(sender, e);
      Settings.Default.WeeksToShow = (int)weekNumber.Value;
    }

    private void MarkedForSave()
    {
      unsavedChanges = true;
      SaveButton.BackColor = Color.LightBlue;
    }

    private void SelectedActivity_Changed(object sender, EventArgs e)
    {
      Console.WriteLine(Activity.activityType.Offday);
      Console.WriteLine((int)Activity.activityType.Offday);
      Console.WriteLine((int)activityPicker.SelectedItem);
    }
    #endregion

    #region Datagrid events
    private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      if (dataGridView2.ContainsFocus)
      {
        dataGridView1.Rows.Add();

        DataGridViewCellEventHandler saveRowChanges = null;
        saveRowChanges = delegate (object s, DataGridViewCellEventArgs ev)
        {
          if (ev.RowIndex < dataGridView2.RowCount - 1)
          {
            Person person =
              new Person(
                dataGridView2[ev.ColumnIndex, ev.RowIndex].Value.ToString(),
                "");
            if (AddPerson(person, new List<Activity>()) != 0)
            {
              dataGridView2[ev.ColumnIndex, ev.RowIndex].Value = person.Name;
              dataGridView2[ev.ColumnIndex, ev.RowIndex].Style.BackColor = errorColor;
            }

            MarkedForSave();

            Console.WriteLine("EY!" + dataGridView2[ev.ColumnIndex, ev.RowIndex].Value.ToString());
            dataGridView2.CellEndEdit -= saveRowChanges;
          }
        };
        dataGridView2.CellEndEdit += saveRowChanges;
        Console.WriteLine("ya" + dataGridView2[0, e.RowIndex].Value);
      }
    }

    private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
      if ((e.RowIndex == dataGridView2.RowCount - 1 && e.ColumnIndex != 0) || e.RowIndex < 1)
        e.Cancel = true;
      if (e.RowIndex < dataGridView2.RowCount - 1 && e.RowIndex > 0)
      {
        string preName = dataGridView2[0, e.RowIndex].Value.ToString();

        //selv fjernende eventhandlers er bullshit
        DataGridViewCellEventHandler saveRowChanges = null;
        saveRowChanges = delegate (object s, DataGridViewCellEventArgs ev)
        {
          Person person =
            calendarData.Keys.First(p => p.Name == preName);
          DataGridViewCell cell =
            dataGridView2[e.ColumnIndex, e.RowIndex];
          if (cell.Value == null) cell.Value = "";
          if (!cell.Value.ToString().Equals(preName))
          {
            if (ev.ColumnIndex == 0)
            {
              if (!ChangePersonName(person, cell.Value.ToString()))
              {
                cell.Value = person.Name;
                cell.Style.BackColor = errorColor;
              }
              else
              {
                cell.Style.BackColor =
                  cell.OwningColumn.DefaultCellStyle.BackColor;
              }
            }
            else if (ev.ColumnIndex == 1)
            {
              person.Department = cell.Value.ToString();
            }

            MarkedForSave();
          }

          dataGridView2.CellEndEdit -= saveRowChanges;
        };
        dataGridView2.CellEndEdit += saveRowChanges;
      }
    }

    private void dataGridView2_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
    {
      FieldInfo field1 = typeof(DataGridView).GetField("EVENT_DATAGRIDVIEWCELLENDEDIT", BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
      object obj =
        field1.GetValue(dataGridView1);
      if (e.RowIndex == dataGridView1.RowCount)
      {
        field1.SetValue(dataGridView2, null);
        dataGridView1.Rows.RemoveAt(e.RowIndex - 1);
      }
      else
        dataGridView1.Rows.RemoveAt(e.RowIndex);
    }

    private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
    {
      dataGridView1.Invalidate();
    }
    #endregion

    #region Grid1 paint
    private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      bool useV2 = true;
      if (e.RowIndex == -1)
      {
        int mondayColumnIndex = e.ColumnIndex - e.ColumnIndex % 7;
        bool isMondayColumn = e.ColumnIndex % 7 == 0;

        #region Weekheader V2
        if (useV2)
        {
          e.PaintBackground(e.CellBounds, false);
          //e.Graphics.FillRectangle(new SolidBrush(e.CellStyle.BackColor), e.CellBounds);
          if (e.ColumnIndex % 7 < 2)
          {
            string headerText =
                "Uge " + HolidayCalculator.WeekNumber(
                  activeTimeStart.AddDays(mondayColumnIndex));

            SizeF sz = e.Graphics.MeasureString(
                headerText,
                dataGridView1.Font);

            Point drawStart = new Point(
              (int)e.CellBounds.Left + 4 - (e.ColumnIndex % 7 * 22),
              (int)(e.CellBounds.Bottom / 2 - sz.Height / 2));
            if (e.ColumnIndex == 0) drawStart.X += 1;

            e.Graphics.SetClip(e.ClipBounds);
            e.Graphics.DrawString(
              headerText,
              dataGridView1.ColumnHeadersDefaultCellStyle.Font,
              Brushes.Black,
              drawStart);
          }
        }
        #endregion
        #region Weekheader V1
        // Med invalidate på scroll burdde denne også virke
        if (!useV2)
        {
          Rectangle headerRect =
            new Rectangle(
              e.ColumnIndex == 0 ? e.CellBounds.Left + 2 : e.CellBounds.Left + 1,
              e.CellBounds.Top + 1,
              e.CellBounds.Width,
              e.CellBounds.Height - 2);

          string headerText =
              "Uge " + HolidayCalculator.WeekNumber(
                activeTimeStart.AddDays(mondayColumnIndex));

          SizeF sz = e.Graphics.MeasureString(
              headerText,
              dataGridView1.Font);
          int headerTop = (headerRect.Height / 2) - (int)(sz.Height / 2) + 2;
          e.PaintBackground(headerRect, false);
          e.Graphics.FillRectangle(new SolidBrush(e.CellStyle.BackColor), headerRect);
          e.Graphics.DrawString(
            headerText,
            dataGridView1.ColumnHeadersDefaultCellStyle.Font,
            Brushes.Black,
            headerRect.Left + 2 - e.ColumnIndex % 7 * headerRect.Width,
            headerTop);
        }
        #endregion

        e.Handled = true;
      }
      // If date row paint date
      // This does rely on the date string being at least 5 characters long, may cause OutOfRange
      if (e.RowIndex == 0 && e.Value != null)
      {
        try
        {
          string Datetext = e.FormattedValue.ToString();
          e.PaintBackground(e.CellBounds, true);
          e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Top);
          e.Graphics.DrawString(Datetext.Substring(0, 2), e.CellStyle.Font, Brushes.Black, -1, -2);
          e.Graphics.DrawString("/", e.CellStyle.Font, Brushes.Black, 7, 4);
          e.Graphics.DrawString(Datetext.Substring(3, 2), e.CellStyle.Font, Brushes.Black, 9, 9);
          e.Graphics.ResetTransform();
          e.Handled = true;
        }
        catch (ArgumentOutOfRangeException ex)
        {
          Log.Add(ex.Message + "\n @Attempt to write date row with" + e.Value);
        }
      }
    }
    private void dataGridView1_Paint(object sender, PaintEventArgs e)
    {

    }
    #endregion

    #region Internal functionality
    private void FindHolidays()
    {
      holidayColumns.Clear();
      List<DateTime> holidays = HolidayCalculator.HolidayColumnsInPeriod(activeTimeStart, activeTimeEnd);
      foreach(DateTime date in holidays)
      {
        holidayColumns.Add((date - activeTimeStart).Days);
      }
    }
    
    private bool IsColumnWeekend(int columnIndex)
    {
      return (columnIndex % 7) > 4;
    }

    private bool isColumnHoliday(int columnIndex)
    {
      return HolidayCalculator.IsHoliday(activeTimeStart.AddDays(columnIndex));
      //return holidayColumns.Contains(columnIndex);
    }
    private void FocusConfig(object sender, EventArgs e, ConfigForm config)
    {
      config.Activate();
    }

    private DateTime FetchDateFromTimepicker()
    {
      Log.Add("Date fetched from time picker: " + dateTimePicker1.Value);
      return dateTimePicker1.Value.Date;
    }

    private string ActivityText(Activity.activityType activityCode)
    {
      switch (activityCode)
      {
        case Activity.activityType.Offday:
          return "FRI";

        case Activity.activityType.Counterbalance:
          return "AFS";

        case Activity.activityType.Course:
          return "KRS";

        default:
          return "";
      }
    }

    private Color ActivityColor(Activity.activityType activityCode)
    {
      switch (activityCode)
      {
        case Activity.activityType.Offday:
          return offdayColor;

        case Activity.activityType.Counterbalance:
          return balancedayColor;

        case Activity.activityType.Project:
          return projectColor;

        case Activity.activityType.Course:
          return courseColor;

        default:
          Log.Add("Found unknown color: " + activityCode);
          return unknownColor;
      }
    }

    private bool IsCellMarkable(DataGridViewCell cell)
    {
      return !(cell.RowIndex < 1 || IsColumnWeekend(cell.ColumnIndex) || isColumnHoliday(cell.ColumnIndex));
    }
    #endregion


    #region Test data
    private int weekHeaderWidth(int rowIndex)
    {
      int width = 0;
      for (int i = 0; i < 7; i++)
      {
        width += dataGridView1.Columns[rowIndex + i].Width;
      }
      return width;
    }

    private void filltestdata()
    {

      AddDataRow(
        new Person("Mogens", "Værksted"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 11), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 12), Activity.activityType.Offday) });
      AddDataRow(
        new Person("Sven", "Værksted"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 10), Activity.activityType.Offday) });
      AddDataRow(
        new Person("Kurt", "Værksted"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 19), Activity.activityType.Offday) });
      AddDataRow(
        new Person("Knud", "Værksted"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 15), Activity.activityType.Offday) });
      AddDataRow(
        new Person("Ib", "Salg"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 20), Activity.activityType.Offday) });
      AddDataRow(
        new Person("Karl", "den Mægtige"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 18), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 19), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 20), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 21), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 22), Activity.activityType.Offday) });
      AddDataRow(
        new Person("Åge", "Salg"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 18), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 22), Activity.activityType.Offday) });
      AddDataRow(
        new Person("Jalte", "Kontor"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 18), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 22), Activity.activityType.Offday) });
      AddDataRow(
        new Person("Jeanette", "Værksted"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 18), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 22), Activity.activityType.Offday) });
      AddDataRow(
        new Person("Adam", "Kontor"),
        new List<Activity>() {
          new Activity(new DateTime(2017, 9, 18), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 19), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 20), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 21), Activity.activityType.Offday),
          new Activity(new DateTime(2017, 9, 22), Activity.activityType.Offday) });
    }
  }
  #endregion
}
