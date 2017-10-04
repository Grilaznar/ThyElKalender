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
    //private ActivityData dataSaver = new ActivityData();
    private Dictionary<Person, List<Activity>> calendarData =
      new Dictionary<Person, List<Activity>>(new PersonComparer());

    private List<Person> calendarList = new List<Person>();

    private DateTime activeTimeStart, activeTimeEnd;
    private List<int> holidayColumns = new List<int>();

    private Color holidayColor = Color.Aquamarine;
    private Color weekendColor = Color.Gray;
    private Color offdayColor = Color.LightGreen;
    private Color balancedayColor = Color.Green;
    private Color courseColor = Color.Wheat;
    private Color projectColor = Color.SlateBlue;

    private Color errorColor = Color.Red;
    private Color unknownColor = Color.Pink;

    private bool unsavedChanges = false;

    public CalendarEditor()
    {
      Log.Add("Calenar window starting");
      InitializeComponent();

      Log.ResetLogFile();

      FormClosing += CloseWindow;

      MinimumSize = new Size(440, 340);

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

      AddDataRows(Task<List<Person>>.Factory
          .StartNew(ActivityFileHandler.ReadData).Result);

      dataGridView2.Rows[0].DefaultCellStyle.BackColor = SystemColors.ControlDark;
    }

    private void InitInputs()
    {
      Log.Add("Initializing inputs");

      //dateTimePicker1.MinDate = DateTime.Now.Date;
      dateTimePicker1.Value = DateTime.Now.Date;
      dateTimePicker1.ValueChanged += UpdateCalendarActiveTimespan;

      weekNumber.Value = Settings.Default.WeeksToShow;
      weekNumber.ValueChanged += weekNumber_ValueChanged;
      weekNumber.MouseWheel += weekNumber_MouseWheel;

      // Add activity types to the dropdown- and grid rightclick-menu
      foreach (Activity.activityType actType in Enum.GetValues(typeof(Activity.activityType)))
      {
        //Don't want to add None, as nothing would happen anyway
        if (actType == 0) continue;
        activityPicker.Items.Add(actType);
        calendarContextMenu.Items.Add(actType.ToString(), null, (object s, EventArgs ev) => { MarkSelected(actType); });
      }
      // And add the option to Clear the selected area by rightclick
      calendarContextMenu.Items.Add("Clear", null, (object s, EventArgs ev) => { UnmarkSelected(this, null); });

      //Person list rightclick options
      //personContextMenu.Items.Add("Add", null, (object s, EventArgs ev) => { ; });
      personContextMenu.Items
        .Add("Move", null, (object s, EventArgs ev) => { StartRowMovement(); }).Name = "Move";
      personContextMenu.Items
        .Add("Add New", null, (object s, EventArgs ev) => { AddDataRow(new Person("-", "")); }).Name = "Add";
      personContextMenu.Items
        .Add("Insert New", null, (object s, EventArgs ev) => { InsertDataRow(); }).Name = "Insert";
      personContextMenu.Items
        .Add("Delete Selected", null, (object s, EventArgs ev) => { DeleteRowsClick(); }).Name = "Delete";

      personContextMenu.Items.Find("Insert", false)[0].Enabled = false;
      personContextMenu.Items.Find("Delete", false)[0].Enabled = false;

      activityPicker.SelectedIndex = 0;
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
      dataGridView1.Size =
        new Size(ClientSize.Width - (dataGridView1.Location.X + 12), ClientSize.Height - 24);
      dataGridView2.Size =
        new Size(dataGridView2.Size.Width, ClientSize.Height - 24);
      SaveButton.Location =
        new Point(SaveButton.Location.X, ClientSize.Height - 12 - SaveButton.Size.Height);
      BackButton.Location =
        new Point(BackButton.Location.X, ClientSize.Height - 12 - BackButton.Size.Height);
      refreshButton.Location =
        new Point(refreshButton.Location.X, BackButton.Location.Y - 40);
    }
    #endregion

    #region Rows
    #region Add Rows
    private void CreateDateRow()
    {
      if (dataGridView1.ColumnCount == 0) dataGridView1.ColumnCount = 1;
      dataGridView2.Rows.Add(new string[] { "", "" });
      dataGridView1.Rows.Add();
      dataGridView1.Rows[dataGridView1.Rows.Count - 1].Height = dataGridView2.Rows[0].Height;
      //dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.Font = new Font("Ariel", 10);
      Log.Add("Date row created");
    }

    private void AddDataRow(Person person)
    {
      if (AddPerson(person) != 0)
      {
        dataGridView2[0, dataGridView2.Rows.Count - 2].Style.BackColor = errorColor;
      };

      dataGridView2.Rows.Add(new string[] { person.Name, person.Department });
      dataGridView1.Rows.Add();
      dataGridView1.Rows[dataGridView1.Rows.Count - 1].Height = dataGridView2.Rows[0].Height;

      MarkedForSave();

      int numberofDays = person.ActivityList == null ? 0 : person.ActivityList.Count;
      Log.Add("Data row for: " + person.Name + " created, with " + person.ActivityList.Count + " days marked");
      UpdateCalendarContent();
    }

    private void AddDataRows(List<Person> dataList)
    {
      Log.Add("Adding " + dataList.Count + " people to grid");
      foreach (Person person in dataList)
      {
        calendarList.Add(person);

        dataGridView2.Rows.Add(new string[] { person.Name, person.Department });
        dataGridView1.Rows.Add();
        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Height = dataGridView2.Rows[0].Height;

        int numberofDays = person.ActivityList == null ? 0 : person.ActivityList.Count;
        Log.Add("Data row for: " + person.Name + " created, with " + numberofDays + " days marked");
      }

      UpdateCalendarContent();
    }
    #endregion

    #region Insert Rows
    private void InsertDataRow()
    {
      if (dataGridView2.SelectedCells.Count > 0)
      {
        Person person = new Person("-", "");
        int rowIndex = dataGridView2.SelectedCells[0].RowIndex;
        dataGridView2.Rows.Insert(rowIndex, new string[] { person.Name, person.Department });
        dataGridView1.Rows.Insert(rowIndex);
        dataGridView1.Rows[rowIndex].Height = dataGridView2.Rows[0].Height;
        //dataGridView1.Rows[rowIndex].DefaultCellStyle.Font = new Font("Ariel", 10);

        calendarList.Insert(rowIndex - 1, person);

        MarkedForSave();
      }
    }

    private void InsertDataRow(int rowIndex, Person person)
    {
      dataGridView2.Rows.Insert(rowIndex, new string[] { person.Name, person.Department });
      dataGridView1.Rows.Insert(rowIndex);
      dataGridView1.Rows[rowIndex].Height = dataGridView2.Rows[0].Height;
      //dataGridView1.Rows[rowIndex].DefaultCellStyle.Font = new Font("Ariel", 10);

      calendarList.Insert(rowIndex - 1, person);

      MarkedForSave();
    }
    #endregion

    #region Remove Rows
    private void RemoveRow(int index)
    {
      if (index > 0)
      {
        string name = dataGridView2[0, index].Value.ToString();
        dataGridView2.Rows.RemoveAt(index);
        calendarList.RemoveAll(p => p.Name == name);
      }
    }

    //bliver nok ikke brugt, skal ikke bruges i nuværende stand
    private void RemoveRows(int[] index)
    {
      for (int i = index.Length - 1; i >= 0; i--)
      {
        if (index[i] > 0)
        {
          string name = dataGridView2[0, index[i]].Value.ToString();
          //dataGridView2.Rows.RemoveAt(index[i]);
          //calendarData.Remove(new Person(name));
        }
      }
    }

    private void ClearDatagrid()
    {
      while (dataGridView1.RowCount > 1)
      {
        dataGridView2.Rows.RemoveAt(1);
      }
      calendarList.Clear();
    }
    #endregion

    #region Move Rows
    private void StartRowMovement()
    {
      if (dataGridView2.SelectedCells.Count > 0)
      {
        DataGridViewCell selectedCell = dataGridView2.SelectedCells[0];

        foreach (DataGridViewRow row in dataGridView2.Rows)
        {
          if (row.Index != selectedCell.RowIndex)
          {
            row.Cells[0].Style.BackColor = Color.LawnGreen;
          }
        }

        DataGridViewCellEventHandler clickOnTarget = null;
        EventHandler cancelClickEvent = null;

        clickOnTarget = delegate (object sedner, DataGridViewCellEventArgs e)
        {
          if (e.RowIndex != selectedCell.RowIndex)
          {
            MoveRow(selectedCell.RowIndex, e.RowIndex);

            //color white/Control
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
              row.Cells[0].Style.BackColor = SystemColors.Control;
            }

            //delete this thing
            dataGridView2.CellClick -= clickOnTarget;
            dataGridView2.LostFocus -= cancelClickEvent;

            //find cancel options
            UpdateCalendarContent();
          }
        };

        dataGridView2.CellClick += clickOnTarget;

        cancelClickEvent = delegate (object sender, EventArgs e)
        {
          foreach (DataGridViewRow row in dataGridView2.Rows)
          {
            row.Cells[0].Style.BackColor = SystemColors.Control;
          }
          dataGridView2.CellClick -= clickOnTarget;
          dataGridView2.LostFocus -= cancelClickEvent;
        };
        dataGridView2.LostFocus += cancelClickEvent;
      }
    }

    private void MoveRow(int movingRowIndex, int arrivalRowIndex)
    {
      if (dataGridView2.SelectedCells.Count > 0)
      {
        Person person = calendarList[movingRowIndex - 1];
        calendarList.RemoveAt(movingRowIndex - 1);
        dataGridView2.Rows.RemoveAt(movingRowIndex);

        if (movingRowIndex > arrivalRowIndex) arrivalRowIndex--;

        InsertDataRow(arrivalRowIndex, person);
      }
    }
    #endregion
    #endregion

    #region CalendarMethods
    private void UpdateCalendarActiveTimespan(object sender, EventArgs e)
    {
      Log.Add("Update active time area");
      DateTime pickedDate = FetchStartDateFromTimepicker();

      activeTimeStart =
        pickedDate.AddDays(pickedDate.DayOfWeek == 0 ?
          -6 :
          1 - (double)pickedDate.DayOfWeek);

      int weeksToShow = (int)weekNumber.Value;
      int daysToShow = weeksToShow * 7;
      dataGridView1.ColumnCount = daysToShow;
      activeTimeEnd = activeTimeStart.AddDays(daysToShow);

      FindHolidays();
      SetupCalendarColumns();
      
      for (int i = 0; i < dataGridView1.ColumnCount; i++)
      {
        for (int j = 0; j < dataGridView1.RowCount; j++)
        {
          dataGridView1[i, j].Style.BackColor = //Color.White;
            dataGridView1.Columns[i].DefaultCellStyle.BackColor;
        }
      }

      UpdateCalendarContent();
    }

    private void UpdateCalendarContent()
    {
      Log.Add("Updateing calendar content");
      WriteDateRow();

      List<Activity> currentList;
      for (int i = 1; i < dataGridView1.RowCount; i++)
      {
        //string name = dataGridView2[0, i].Value.ToString();
        Person person = calendarList[i - 1];//.Find(p => p.Name == name);

        if (person.Name != ".")
        {
          dataGridView1.Rows[i].Height = 22;
          dataGridView2.Rows[i].Height = 22;
          if (person.ActivityList == null) currentList = person.ActivityList = new List<Activity>();
          else currentList = person.ActivityList;

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
        else
        {
          dataGridView1.Rows[i].Height = 8;
          dataGridView2.Rows[i].Height = 8;
          dataGridView2[1, i].Value = "";
          dataGridView2[1, i].Style.BackColor = SystemColors.ControlDark;
          foreach (DataGridViewCell cell in dataGridView1.Rows[i].Cells)
            cell.Style.BackColor = SystemColors.ControlDark;
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
          //dataGridView1.Columns[i].Tag = "Holiday";
        }
        else if (IsColumnWeekend(i))
        {
          dataGridView1.Columns[i].DefaultCellStyle.BackColor =
            weekendColor;
          //dataGridView1.Columns[i].Tag = "Weekend";
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
      for (int i = 0; i < dataGridView1.ColumnCount; i++)
      {
        dataGridView1[i, 0].Value = activeTimeStart.AddDays(i).ToString().Substring(0, 5);
      }
    }
    #endregion

    #region Click events
    private void DeleteRowsClick()
    {
      foreach (DataGridViewCell cell in dataGridView2.SelectedCells)
      {
        if (cell.ColumnIndex == 0 && dataGridView1.RowCount > 1)
          RemoveRow(cell.RowIndex);
      }
    }

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

    private void dataGridView2_Click(object sender, EventArgs e)
    {
      dataGridView1.ClearSelection();
    }

    private void dataGridView2_CellDoubleClick(object sender, EventArgs e)
    {
      dataGridView2.BeginEdit(false);
    }

    private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
    {
      HotkeyDetection(sender, e);
      if (!dataGridView2.IsCurrentCellInEditMode)
      {
        if (!(e.Alt || e.Control || e.KeyCode == Keys.Shift))
        {
          dataGridView2.BeginEdit(false);
          //dataGridView2.SelectedCells[0].;
        }
      }
    }

    private void DeselectGrids(object sender, EventArgs e)
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
      AddDataRows(Task<List<Person>>.Factory
          .StartNew(ActivityFileHandler.ReadData).Result);
      UnmarkedForSave();
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
          ActivityFileHandler.SaveData(calendarList);
        }
        else if (result == DialogResult.No)
        {
        }
        else if (result == DialogResult.Cancel)
        {
          if (e != null) e.Cancel = true;
          return;
        }
      }
      SaveWindowState(this, null);

      this.Dispose();
    }

    private void MarkClicked(object sender, EventArgs e)
    {
      MarkSelected((Activity.activityType)activityPicker.SelectedItem);
    }

    private void MarkSelected(Activity.activityType markType)
    {
      Console.WriteLine("started");
      foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
      {
        // If cell is a weekend, holiday or the None option is
        // somehow chosen, skip
        if (!IsCellMarkable(cell) || markType == 0) continue;

        Activity newActivity = new Activity(
            activeTimeStart.AddDays(cell.ColumnIndex), markType);

        //Find list of the marked person
        List<Activity> relevantList =
          calendarList[cell.RowIndex - 1]/*
          .Find(
            p => p.Name ==
            dataGridView2[0, cell.RowIndex].Value.ToString())*/
            .ActivityList;

        Activity foundActivity = relevantList.Find(act => act.Date == newActivity.Date);

        //If entry doesn't exist, add to list
        if (foundActivity == null)
        {
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

        calendarList[cell.RowIndex - 1]/*.Find(
          p => p.Name ==
          dataGridView2[0, cell.RowIndex].Value.ToString())*/
          .ActivityList
            .RemoveAll(
              d => d.Date ==
              activeTimeStart.AddDays(cell.ColumnIndex));

        cell.Style.BackColor =
          cell.OwningColumn.DefaultCellStyle.BackColor;
        cell.Value = "";

        MarkedForSave();
      }
      dataGridView1.ClearSelection();
    }

    private void Save_Click(object sender, EventArgs e)
    {
      DeselectGrids(sender, e);
      ActivityFileHandler.SaveData(calendarList);
      UnmarkedForSave();
    }

    private void ToConfig(object sender, EventArgs e)
    {
      DeselectGrids(sender, e);
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
      Settings.Default.WeeksToShow = (int)weekNumber.Value;
      UpdateCalendarActiveTimespan(sender, e);
    }

    private void weekNumber_MouseWheel(object sender, MouseEventArgs e)
    {
      if (e.Delta != 0)
      {
        ((HandledMouseEventArgs)e).Handled = true;
      }
    }

    private void MarkedForSave()
    {
      unsavedChanges = true;
      SaveButton.BackColor = Color.LightBlue;
    }

    private void UnmarkedForSave()
    {
      unsavedChanges = false;
      SaveButton.BackColor = SystemColors.Control;
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
      if (dataGridView2.ContainsFocus && e.RowIndex == dataGridView2.RowCount - 1)
      {
        dataGridView1.Rows.Add();

        DataGridViewCellEventHandler saveRowChanges = null;
        saveRowChanges = delegate (object s, DataGridViewCellEventArgs ev)
        {
          if (ev.RowIndex < dataGridView2.RowCount - 1)
          {
            Person person =
              new Person(
                (string)dataGridView2[ev.ColumnIndex, ev.RowIndex].Value,
                "");
            if (AddPerson(person) != 0)
            {
              dataGridView2[ev.ColumnIndex, ev.RowIndex].Value = person.Name;
              dataGridView2[ev.ColumnIndex, ev.RowIndex].Style.BackColor = errorColor;
            }

            MarkedForSave();

            dataGridView2.CellEndEdit -= saveRowChanges;
          }
        };
        dataGridView2.CellEndEdit += saveRowChanges;
      }
    }

    private void dataGridView2_SelectionChanged(object sender, EventArgs e)
    {
      if (dataGridView2.SelectedCells.Count > 0)
      {
        foreach (DataGridViewCell cell in dataGridView2.SelectedCells)
        {
          if (cell.RowIndex > 0)
          {
            personContextMenu.Items.Find("Insert", false)[0].Enabled = true;
            personContextMenu.Items.Find("Delete", false)[0].Enabled = true;
            break;
          }
        }
      }
      else
      {
        personContextMenu.Items.Find("Insert", false)[0].Enabled = false;
        personContextMenu.Items.Find("Delete", false)[0].Enabled = false;
      }
    }

    private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
      if ((e.RowIndex == dataGridView2.RowCount - 1 && e.ColumnIndex != 0) || e.RowIndex < 1)
        e.Cancel = true;
      if (e.RowIndex < dataGridView2.RowCount - 1 && e.RowIndex > 0)
      {
        string preName = (string)dataGridView2[0, e.RowIndex].Value;

        //selv fjernende eventhandlers er bullshit
        DataGridViewCellEventHandler saveRowChanges = null;
        saveRowChanges = delegate (object s, DataGridViewCellEventArgs ev)
        {
          Person person =
            calendarList[e.RowIndex - 1];//.Find(p => p.Name == preName);
          DataGridViewCell cell =
            dataGridView2[e.ColumnIndex, e.RowIndex];
          if (cell.Value == null) cell.Value = "";
          if (!cell.Value.ToString().Equals(person.Name))
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

            if (preName == "." || person.Name == ".") UpdateCalendarContent();
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
      field1.SetValue(dataGridView2, null);
      if (e.RowIndex == dataGridView1.RowCount)
      {
        dataGridView1.Rows.RemoveAt(e.RowIndex - 1);
      }
      else
        dataGridView1.Rows.RemoveAt(e.RowIndex);
    }

    private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
    {
      dataGridView2.FirstDisplayedScrollingRowIndex =
        dataGridView1.FirstDisplayedScrollingRowIndex;
      dataGridView1.Invalidate();
    }

    private void dataGridView2_Scroll(object sender, ScrollEventArgs e)
    {
      dataGridView1.FirstDisplayedScrollingRowIndex =
        dataGridView2.FirstDisplayedScrollingRowIndex;
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
    private int AddPerson(Person person, int retries = 0)
    {
      if (retries > 10) return -1;
      if (retries > 0)
      {
        person.Name += "-";
      }
      if (calendarList.FindIndex(p => p.Name == person.Name) == -1)
      {
        calendarList.Add(person);
      }
      else
      {
        return AddPerson(person, retries + 1);
      }
      return retries;
    }

    private bool ChangePersonName(Person person, string newName)
    {
      if (!calendarList.Contains(new Person(newName), new PersonComparer()))
      {
        calendarList.Find(p => p.Name == person.Name).Name = newName;

        return true;
      }
      return false;
    }

    private void UpdateTimerTick(object sender, EventArgs e)
    {
      if (!unsavedChanges && dataGridView1.SelectedCells.Count == 0)
      {
        UpdateButton(sender, e);
      }
    }

    private void FindHolidays()
    {
      holidayColumns.Clear();
      List<DateTime> holidays = HolidayCalculator.HolidayColumnsInPeriod(activeTimeStart, activeTimeEnd);
      foreach (DateTime date in holidays)
      {
        holidayColumns.Add((date - activeTimeStart).Days);
      }
    }

    private bool isColumnHoliday(int columnIndex)
    {
      //return HolidayCalculator.IsHoliday(activeTimeStart.AddDays(columnIndex));
      return holidayColumns.Contains(columnIndex);
    }

    private bool IsColumnWeekend(int columnIndex)
    {
      return (columnIndex % 7) > 4;
    }
    private void FocusConfig(object sender, EventArgs e, ConfigForm config)
    {
      config.Activate();
    }

    private DateTime FetchStartDateFromTimepicker()
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
          return "PRJ";
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

    private void HotkeyDetection(object sender, KeyEventArgs e)
    {
      if (e.KeyData == (Keys.S | Keys.Control))
      {
        Save_Click(this, e);
        e.Handled = true;
      }
      if (e.KeyData == (Keys.R | Keys.Control))
      {
        UpdateButton(this, e);
        e.Handled = true;
      }
    }

    private bool IsCellMarkable(DataGridViewCell cell)
    {
      return !(
        cell.RowIndex < 1 || 
        IsColumnWeekend(cell.ColumnIndex) || 
        isColumnHoliday(cell.ColumnIndex) || 
        calendarList[cell.RowIndex - 1].Name != ".");
      //return !(cell.RowIndex < 1 || (string)cell.OwningColumn.Tag == "Weekend" || (string)cell.OwningColumn.Tag == "Holiday");
    }
    #endregion

    #region Not Active
    /*
    public class ContextMenukEventArgs : EventArgs
    {
      public int ClickedRowIndex { get; private set; }

      public ContextMenukEventArgs(int clickedRowIndex)
      {
        ClickedRowIndex = clickedRowIndex;
      }
    }

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
  */
    #endregion
  }
}
