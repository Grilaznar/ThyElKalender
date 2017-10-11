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
using System.Diagnostics;

namespace Thy_El_Teknik_Kalender_9000
{
  public partial class CalendarEditor : Form
  {
    //List of all data
    private List<Person> calendarList = new List<Person>();

    private DateTime startDate, endDate;
    private List<int> holidayColumns = new List<int>();

    //Colors for calendar fields
    //private Color holidayColor = Color.Aquamarine;
    //private Color weekendColor = Color.Gray;

    //System Colors
    private Color errorColor = Color.Red;
    private Color unknownColor = Color.DeepPink;

    private bool unsavedChanges = false;
    private bool calendarLocked = false;

    public CalendarEditor()
    {
      Log.ResetLogFile();

      Log.Add("Calenar window starting");
      InitializeComponent();

      // Ask if you want to save changes
      FormClosing += CloseWindow;

      // Buttons will overlap if smaller
      MinimumSize = new Size(440, 400);

      // Enable double buffering on calendar datagrid
      // for massive damage (performance increase)
      typeof(DataGridView).InvokeMember(
        "DoubleBuffered",
        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
        null,
        calendarDataGird,
        new object[] { true });
      Log.Add("Calendar window started");
    }

    #region Init methods
    private void InitDataGrids(object sender, EventArgs e)
    {
      Log.Add("Calendar window interior initializing");

      LoadSettings();

      personDataGrid.Columns.Add("Names", "Navn");
      personDataGrid.Columns.Add("Department", "");

      int nameCellWidth = 120;
      int depCellWidth = 78;
      personDataGrid.Columns[0].Width = nameCellWidth;
      personDataGrid.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
      personDataGrid.Columns[1].Width = depCellWidth;
      personDataGrid.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

      Log.Add("personDataGrid Initialaze");

      InitInputs();

      CreateDateRow();

      Log.Add("Setting current time");
      UpdateCalendarActiveTimespan(this, null);
      Log.Add("Calendar window interior initialized");

      AddDataRows(Task<List<Person>>.Factory
          .StartNew(ActivityFileHandler.ReadData).Result);

      personDataGrid.Rows[0].DefaultCellStyle.BackColor = SystemColors.ControlDark;

      if (calendarLocked) LockCalendar();
    }

    private void InitInputs()
    {
      Log.Add("Initializing inputs");

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
      calendarContextMenu.Items.Add("Afmarkér", null, (object s, EventArgs ev) => { UnmarkSelected(this, null); });

      personContextMenu.Items
        .Add("Flyt", null, (object s, EventArgs ev) => { StartRowMovement(); }).Name = "Move";
      personContextMenu.Items
        .Add("Tilføj ny", null, (object s, EventArgs ev) => { AddDataRow(new Person("-", "")); }).Name = "Add";
      personContextMenu.Items
        .Add("Indsæt ny", null, (object s, EventArgs ev) => { InsertDataRow(); }).Name = "Insert";
      personContextMenu.Items
        .Add("Slet valgte", null, (object s, EventArgs ev) => { DeleteRowsClick(); }).Name = "Delete";

      personContextMenu.Items.Find("Move", false)[0].Enabled = false;
      personContextMenu.Items.Find("Insert", false)[0].Enabled = false;
      personContextMenu.Items.Find("Delete", false)[0].Enabled = false;

      activityPicker.SelectedIndex = 0;
    }
    #endregion

    #region Settings
    private void LoadSettings()
    {
      Log.Add("Loading window state");
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

      calendarLocked = Settings.Default.Locked;

      Log.Add("Window state:"
        + " Location '" + Location.X + " x " + Location.Y + "'"
        + " Size '" + Size.Width + " x " + Size.Height + "'");
    }

    private void SaveSettings(object sender, FormClosingEventArgs e)
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

      Settings.Default.Locked = calendarLocked;

      Settings.Default.Save();
    }

    private void FitLayout(object sender, LayoutEventArgs e)
    {
      calendarDataGird.Size =
        new Size(ClientSize.Width - (calendarDataGird.Location.X + 12), ClientSize.Height - 24);
      personDataGrid.Size =
        new Size(personDataGrid.Size.Width, ClientSize.Height - 24);
      SaveButton.Location =
        new Point(SaveButton.Location.X, ClientSize.Height - 12 - SaveButton.Size.Height);
      BackButton.Location =
        new Point(BackButton.Location.X, ClientSize.Height - 12 - BackButton.Size.Height);
      refreshButton.Location =
        new Point(refreshButton.Location.X, BackButton.Location.Y - 40);
      keyboardButton.Location =
        new Point(keyboardButton.Location.X, BackButton.Location.Y - 40);
    }
    #endregion

    #region Rows
    #region Add Rows
    //Create row intended to contain dates
    private void CreateDateRow()
    {
      if (calendarDataGird.ColumnCount == 0) calendarDataGird.ColumnCount = 1;
      personDataGrid.Rows.Add(new string[] { "", "" });
      calendarDataGird.Rows.Add();
      calendarDataGird.Rows[calendarDataGird.Rows.Count - 1].Height =
        personDataGrid.Rows[0].Height;
      Log.Add("Date row created");
    }

    //Add new row at the bottom, with the specified content
    private void AddDataRow(Person person)
    {
      AddPerson(person);

      personDataGrid.Rows.Add(
        new string[] { person.Name, person.Department });
      calendarDataGird.Rows.Add();
      calendarDataGird.Rows[calendarDataGird.Rows.Count - 1].Height =
        personDataGrid.Rows[0].Height;

      MarkedForSave();

      int numberofDays =
        person.ActivityList == null ? 0 : person.ActivityList.Count;
      Log.Add("Data row for: "
        + person.Name + " created, with "
        + person.ActivityList.Count + " days marked");

      UpdateCalendarContent();
    }

    //Add a number of rows, with the specified content. Used to fill data from file
    private void AddDataRows(List<Person> dataList)
    {
      Log.Add("Adding " + dataList.Count + " people to grid");
      foreach (Person person in dataList)
      {
        calendarList.Add(person);

        personDataGrid.Rows.Add(
          new string[] { person.Name, person.Department });
        calendarDataGird.Rows.Add();
        calendarDataGird.Rows[calendarDataGird.Rows.Count - 1].Height =
          personDataGrid.Rows[0].Height;

        int numberofDays =
          person.ActivityList == null ? 0 : person.ActivityList.Count;
        Log.Add("Data row for: "
          + person.Name + " created, with "
          + numberofDays + " days marked");
      }

      UpdateCalendarContent();
    }
    #endregion

    #region Insert Rows
    //Insert empty row above selected row
    private void InsertDataRow()
    {
      if (personDataGrid.SelectedCells.Count > 0)
      {
        Person person = new Person("-", "");
        int rowIndex = personDataGrid.SelectedCells[0].RowIndex;
        personDataGrid.Rows.Insert(
          rowIndex,
          new string[] { person.Name, person.Department });
        calendarDataGird.Rows.Insert(
          rowIndex);
        calendarDataGird.Rows[rowIndex].Height = personDataGrid.Rows[0].Height;

        calendarList.Insert(rowIndex - 1, person);

        MarkedForSave();
      }
    }

    //Insert row at specified destination, with specified content
    private void InsertDataRow(int rowIndex, Person person)
    {
      personDataGrid.Rows.Insert(
        rowIndex,
        new string[] { person.Name, person.Department });
      calendarDataGird.Rows.Insert(rowIndex);
      calendarDataGird.Rows[rowIndex].Height = personDataGrid.Rows[0].Height;

      calendarList.Insert(rowIndex - 1, person);

      MarkedForSave();
    }
    #endregion

    #region Remove Rows
    //Remove single selected row
    private void RemoveRow(int index)
    {
      if (index > 0)
      {
        string name = personDataGrid[0, index].Value.ToString();
        personDataGrid.Rows.RemoveAt(index);
        calendarList.RemoveAt(index - 1);
      }
    }

    //bliver nok ikke brugt, virker måske
    private void RemoveRows(int[] index)
    {
      for (int i = index.Length - 1; i >= 0; i--)
      {
        if (index[i] > 0)
        {
          personDataGrid.Rows.RemoveAt(index[i]);
          calendarList.RemoveAt(index[i] - 1);
        }
      }
    }

    //Remove all datarows
    private void ClearDatagrid()
    {
      while (calendarDataGird.RowCount > 1)
      {
        personDataGrid.Rows.RemoveAt(1);
      }
      calendarList.Clear();
    }
    #endregion

    #region Move Rows
    private void StartRowMovement()
    {
      if (personDataGrid.SelectedCells.Count > 0)
      {
        DataGridViewCell selectedCell = personDataGrid.SelectedCells[0];

        //Mark possible targets locations for move
        for (int i = 1; i < personDataGrid.RowCount - 1; i++)
        {
          if (i != selectedCell.RowIndex)
            personDataGrid[0, i].Style.BackColor = Color.LawnGreen;
        }

        DataGridViewCellEventHandler clickOnTarget = null;
        EventHandler cancelClickEvent = null;

        //Move to clicked cell's position
        clickOnTarget = delegate (object sedner, DataGridViewCellEventArgs e)
        {
          if (e.RowIndex != selectedCell.RowIndex
            && e.RowIndex != 0)
          {
            MoveRow(selectedCell.RowIndex, e.RowIndex);

            //color white/Control
            for (int i = 1; i < personDataGrid.RowCount - 1; i++)
            {
              personDataGrid[0, i].Style.BackColor = SystemColors.Window;
            }

            //delete this thing
            personDataGrid.CellClick -= clickOnTarget;
            personDataGrid.LostFocus -= cancelClickEvent;

            //find cancel options
            UpdateCalendarContent();
          }
        };

        //Cancel move on loss of focus on datagrid
        cancelClickEvent = delegate (object sender, EventArgs e)
        {
          for (int i = 1; i < personDataGrid.RowCount - 1; i++)
          {
            personDataGrid[0, i].Style.BackColor = SystemColors.Window;
          }
          personDataGrid.CellClick -= clickOnTarget;
          personDataGrid.LostFocus -= cancelClickEvent;
        };

        personDataGrid.CellClick += clickOnTarget;
        personDataGrid.LostFocus += cancelClickEvent;
      }
    }

    private void MoveRow(int movingRowIndex, int arrivalRowIndex)
    {
      if (personDataGrid.SelectedCells.Count > 0)
      {
        Person person = calendarList[movingRowIndex - 1];
        calendarList.RemoveAt(movingRowIndex - 1);
        personDataGrid.Rows.RemoveAt(movingRowIndex);

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

      startDate =
        pickedDate.AddDays(pickedDate.DayOfWeek == 0 ?
          -6 :
          1 - (double)pickedDate.DayOfWeek);

      int weeksToShow = (int)weekNumber.Value;
      int daysToShow = weeksToShow * 7;
      calendarDataGird.ColumnCount = daysToShow;
      endDate = startDate.AddDays(daysToShow);

      FindHolidays();
      SetupCalendarColumns();

      for (int i = 0; i < calendarDataGird.ColumnCount; i++)
      {
        for (int j = 0; j < calendarDataGird.RowCount; j++)
        {
          calendarDataGird[i, j].Style.BackColor =
            calendarDataGird.Columns[i].DefaultCellStyle.BackColor;
        }
      }

      UpdateCalendarContent();
    }

    private void UpdateCalendarContent()
    {
      Log.Add("Updateing calendar content");
      WriteDateRow();

      List<Activity> currentList;
      for (int i = 1; i < calendarDataGird.RowCount; i++)
      {
        Person person = calendarList[i - 1];

        if (person.Name != ".")
        {
          calendarDataGird.Rows[i].Height = 22;
          personDataGrid.Rows[i].Height = 22;
          if (person.ActivityList == null) currentList =
              person.ActivityList = new List<Activity>();
          else currentList = person.ActivityList;

          foreach (Activity activity in currentList)
          {
            if (activity.Date < endDate
              && activity.Date >= startDate)
            {
              calendarDataGird[(activity.Date - startDate).Days, i]
                .Style.BackColor =
                  ActivityColor(activity.ActivityCode);

              calendarDataGird[(activity.Date - startDate).Days, i].Value =
                ActivityText(activity.ActivityCode);

            }
          }
        }
        else
        {
          calendarDataGird.Rows[i].Height = 8;
          personDataGrid.Rows[i].Height = 8;
          personDataGrid[1, i].Value = "";
          personDataGrid[1, i].Style.BackColor = SystemColors.ControlDark;
          foreach (DataGridViewCell cell in calendarDataGird.Rows[i].Cells)
            cell.Style.BackColor = SystemColors.ControlDark;
        }
      }
      if (startDate <= DateTime.Now && DateTime.Now < endDate)
      {
        calendarDataGird[(DateTime.Now - startDate).Days, 0]
          .Style.BackColor = Color.LightSkyBlue;
      }

      calendarDataGird.ClearSelection();
      personDataGrid.ClearSelection();
    }

    private void SetupCalendarColumns()
    {
      Log.Add("Setting up columns for calendar grid");
      int cellSize = personDataGrid.Rows[0].Height;
      for (int i = 0; i < calendarDataGird.ColumnCount; i++)
      {
        calendarDataGird.Columns[i].Width = cellSize;
        calendarDataGird.Columns[i].SortMode =
          DataGridViewColumnSortMode.NotSortable;

        if (IsColumnHoliday(i))
        {
          calendarDataGird.Columns[i].DefaultCellStyle.BackColor =
            Color.Aquamarine;
        }
        else if (IsColumnWeekend(i))
        {
          calendarDataGird.Columns[i].DefaultCellStyle.BackColor =
            Color.Gray;
        }
        else
        {
          calendarDataGird.Columns[i].DefaultCellStyle.BackColor =
            SystemColors.Window;
        }
        for (int j = 1; j < calendarDataGird.RowCount; j++)
        {
          calendarDataGird[i, j].Value = "";
        }
      }
    }

    private void WriteDateRow()
    {
      calendarDataGird.Rows[0].DefaultCellStyle.WrapMode =
        DataGridViewTriState.True;
      calendarDataGird.Rows[0].DefaultCellStyle.Font =
        new Font("Ariel", 7);
      for (int i = 0; i < calendarDataGird.ColumnCount; i++)
      {
        calendarDataGird[i, 0].Value =
          startDate.AddDays(i).ToString().Substring(0, 5);
      }
    }
    #endregion

    #region Click events
    private void DeleteRowsClick()
    {
      foreach (DataGridViewCell cell in personDataGrid.SelectedCells)
      {
        if (cell.ColumnIndex == 0 && calendarDataGird.RowCount > 1)
          RemoveRow(cell.RowIndex);
      }
    }

    private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
    {
      personDataGrid.ClearSelection();
      Console.WriteLine("Number of cells you have chosen:");
      Console.WriteLine(calendarDataGird.SelectedCells.Count);
    }

    private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      Color backcolor =
        calendarDataGird[e.ColumnIndex, e.RowIndex].Style.BackColor;
      if (backcolor !=
        calendarDataGird.Columns[e.ColumnIndex].DefaultCellStyle.BackColor)
        UnmarkSelected(sender, e);
      else
        MarkSelected((Activity.activityType)activityPicker.SelectedItem);
    }

    private void dataGridView2_Click(object sender, EventArgs e)
    {
      calendarDataGird.ClearSelection();
    }

    private void dataGridView2_CellDoubleClick(object sender, EventArgs e)
    {
      personDataGrid.BeginEdit(false);
    }

    private void dataGridView2_KeyDown(object sender, KeyEventArgs e)
    {
      HotkeyDetection(sender, e);
      if (!personDataGrid.IsCurrentCellInEditMode)
      {
        if (!(e.Alt || e.Control || e.KeyCode == Keys.Shift))
        {
          personDataGrid.BeginEdit(false);
        }
      }
    }

    private void DeselectGrids(object sender, EventArgs e)
    {
      calendarDataGird.ClearSelection();
      personDataGrid.ClearSelection();
      this.Focus();
    }
    #endregion

    #region Button events
    private void LockButton_Click(object sender, EventArgs e)
    {
      if (calendarLocked) UnlockCalendar();
      else LockCalendar();
    }

    private void UpdateButton(object sender, EventArgs e)
    {
      ClearDatagrid();
      AddDataRows(Task<List<Person>>.Factory
          .StartNew(ActivityFileHandler.ReadData).Result);
      UnmarkedForSave();
    }

    private void CloseButton(object sender, EventArgs e)
    {
      CloseWindow(
        sender,
        new FormClosingEventArgs(CloseReason.UserClosing, false));
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
      SaveSettings(this, null);

      this.Dispose();
    }

    //Method for Mark button
    private void MarkClicked(object sender, EventArgs e)
    {
      MarkSelected((Activity.activityType)activityPicker.SelectedItem);
    }

    //Method for Unmark button
    //TODO put in seperate method to call from here
    private void UnmarkSelected(object sender, EventArgs e)
    {
      foreach (DataGridViewCell cell in calendarDataGird.SelectedCells)
      {
        if (cell.RowIndex == 0) continue;

        calendarList[cell.RowIndex - 1]
          .ActivityList
            .RemoveAll(
              d => d.Date ==
              startDate.AddDays(cell.ColumnIndex));

        cell.Style.BackColor =
          cell.OwningColumn.DefaultCellStyle.BackColor;
        cell.Value = "";

        MarkedForSave();
      }
      calendarDataGird.ClearSelection();
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
      ConfigForm config = new ConfigForm();

      EventHandler evnthndlr =
        (object s, EventArgs ev) => FocusConfig(s, ev, config);

      Activated += evnthndlr;
      config.Disposed += (object s, EventArgs ev) =>
      {
        Activated -= evnthndlr;
        UpdateCalendarContent();
      };

      config.Show();
    }

    private void OnScreenKeyboardButton(object sender, EventArgs e)
    {
      Process ExternalProcess = new Process();
      ExternalProcess.StartInfo.FileName = "osk.exe";
      ExternalProcess.Start();
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
      SaveButton.BackColor = Color.Yellow;
    }

    private void UnmarkedForSave()
    {
      unsavedChanges = false;
      SaveButton.BackColor = SystemColors.Control;
    }
    #endregion

    #region Datagrid events
    /// <summary>
    /// Will add a new row to both person list and calendar grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      if (personDataGrid.ContainsFocus
        && e.RowIndex == personDataGrid.RowCount - 1)
      {
        //Add coresponding row in calendar grid
        calendarDataGird.Rows.Add();

        //Set up event handler to save the data when done
        DataGridViewCellEventHandler saveRowChanges = null;
        saveRowChanges = delegate (object s, DataGridViewCellEventArgs ev)
        {
          //Double checking aint trying weird shit
          if (ev.RowIndex < personDataGrid.RowCount - 1)
          {
            //Create person based on input
            Person person =
              new Person(
                (string)personDataGrid[ev.ColumnIndex, ev.RowIndex].Value,
                "");

            AddPerson(person);

            //If intended to be splitter line, update datagrids
            if (person.Name == ".")
            {
              UpdateCalendarContent();
            }

            MarkedForSave();

            //Remove this eventhandler after execution
            personDataGrid.CellEndEdit -= saveRowChanges;
          }
        };
        //Add handler to event
        personDataGrid.CellEndEdit += saveRowChanges;
      }
    }

    /// <summary>
    /// Enable/Disable context menu buttons that 
    /// require something to be selected cells
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void dataGridView2_SelectionChanged(object sender, EventArgs e)
    {
      if (personDataGrid.SelectedCells.Count > 0)
      {
        personContextMenu.Items.Find("Move", false)[0].Enabled = true;
        personContextMenu.Items.Find("Insert", false)[0].Enabled = true;
        personContextMenu.Items.Find("Delete", false)[0].Enabled = true;
      }
      else
      {
        personContextMenu.Items.Find("Move", false)[0].Enabled = false;
        personContextMenu.Items.Find("Insert", false)[0].Enabled = false;
        personContextMenu.Items.Find("Delete", false)[0].Enabled = false;
      }
    }

    private void dataGridView2_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
      if ((e.RowIndex == personDataGrid.RowCount - 1
        && e.ColumnIndex != 0) || e.RowIndex < 1)
        e.Cancel = true;
      if (e.RowIndex < personDataGrid.RowCount - 1 && e.RowIndex > 0)
      {
        string preName = (string)personDataGrid[0, e.RowIndex].Value;

        DataGridViewCellEventHandler saveRowChanges = null;
        saveRowChanges = delegate (object s, DataGridViewCellEventArgs ev)
        {
          Person person =
            calendarList[e.RowIndex - 1];
          DataGridViewCell cell =
            personDataGrid[e.ColumnIndex, e.RowIndex];
          if (cell.Value == null) cell.Value = "";
          if (!cell.Value.ToString().Equals(person.Name))
          {
            if (ev.ColumnIndex == 0)
            {
              ChangePersonName(ev.RowIndex - 1, cell.Value.ToString());
            }
            else if (ev.ColumnIndex == 1)
            {
              person.Department = cell.Value.ToString();
            }

            if (preName == "." || person.Name == ".") UpdateCalendarContent();
            MarkedForSave();
          }

          personDataGrid.CellEndEdit -= saveRowChanges;
        };
        personDataGrid.CellEndEdit += saveRowChanges;
      }
    }

    private void dataGridView2_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
    {
      FieldInfo field1 =
        typeof(DataGridView).GetField(
          "EVENT_DATAGRIDVIEWCELLENDEDIT",
          BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
      object obj =
        field1.GetValue(calendarDataGird);
      field1.SetValue(personDataGrid, null);
      if (e.RowIndex == calendarDataGird.RowCount)
      {
        calendarDataGird.Rows.RemoveAt(e.RowIndex - 1);
      }
      else
        calendarDataGird.Rows.RemoveAt(e.RowIndex);
    }

    private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
    {
      personDataGrid.FirstDisplayedScrollingRowIndex =
        calendarDataGird.FirstDisplayedScrollingRowIndex;
      calendarDataGird.Invalidate();
    }

    private void dataGridView2_Scroll(object sender, ScrollEventArgs e)
    {
      calendarDataGird.FirstDisplayedScrollingRowIndex =
        personDataGrid.FirstDisplayedScrollingRowIndex;
      calendarDataGird.Invalidate();
    }
    #endregion

    #region Grid1 paint
    //Custom paint event to paint header weeknumber and dates in daterow
    private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      //For header
      if (e.RowIndex == -1)
      {
        int mondayColumnIndex = e.ColumnIndex - e.ColumnIndex % 7;

        //Don't need custom background, though maybe i should
        e.PaintBackground(e.CellBounds, false);

        if (e.ColumnIndex % 7 < 2)
        {
          string headerText =
              "Uge " + HolidayCalculator.WeekNumber(
                startDate.AddDays(mondayColumnIndex));

          SizeF textSize = e.Graphics.MeasureString(
              headerText,
              calendarDataGird.Font);

          Point drawStart = new Point(
            (int)e.CellBounds.Left + 4 - (e.ColumnIndex % 7 * 22),
            (int)(e.CellBounds.Bottom / 2 - textSize.Height / 2));

          //For reasons i don't understand the first row has an extra pixel
          if (e.ColumnIndex == 0) drawStart.X += 1;

          //Set clip to only show what is inside the cell
          e.Graphics.SetClip(e.ClipBounds);

          e.Graphics.DrawString(
            headerText,
            calendarDataGird.ColumnHeadersDefaultCellStyle.Font,
            Brushes.Black,
            drawStart);
        }
        //Done
        e.Handled = true;
      }
      // If date row paint date
      // This does rely on the date string being at least 5 characters long, may cause OutOfRange
      if (e.RowIndex == 0 && e.Value != null)
      {
        try
        {
          //Date exists in the cell, extract it
          string Datetext = e.FormattedValue.ToString();
          //Don't want custom background
          e.PaintBackground(e.CellBounds, true);
          //Move our reference point, really need look colser at this
          e.Graphics.TranslateTransform(e.CellBounds.Left, e.CellBounds.Top);
          //Draw day
          e.Graphics.DrawString(
            Datetext.Substring(0, 2),
            e.CellStyle.Font, Brushes.Black,
            -1,
            -2);
          //Draw seperating character
          e.Graphics.DrawString("/", e.CellStyle.Font, Brushes.Black, 7, 4);
          //Draw Month
          e.Graphics.DrawString(
            Datetext.Substring(3, 2),
            e.CellStyle.Font, Brushes.Black,
            9,
            9);
          //Reset reference point
          e.Graphics.ResetTransform();
          //Done
          e.Handled = true;
        }
        catch (ArgumentOutOfRangeException ex)
        {
          Log.Add(ex.Message + "\n @Attempt to write date row with" + e.Value);
        }
      }
    }
    #endregion

    #region Internal functionality
    /// <summary>
    /// Add the given person to internal list
    /// </summary>
    /// <param name="person">Person to add</param>
    private void AddPerson(Person person)
    {
      calendarList.Add(person);
    }

    /// <summary>
    /// Change the name of the person at index to the given string
    /// </summary>
    /// <param name="index">Index of intended person</param>
    /// <param name="newName">Name to replace with</param>
    /// <returns></returns>
    private void ChangePersonName(int index, string newName)
    {
      calendarList[index].Name = newName;
    }

    /// <summary>
    /// Update from file, if there are no unsaved changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateTimerTick(object sender, EventArgs e)
    {
      if (!unsavedChanges && calendarDataGird.SelectedCells.Count == 0)
      {
        UpdateButton(sender, e);
      }
    }

    /// <summary>
    /// Fills holidayColumns with relevant holidays
    /// </summary>
    private void FindHolidays()
    {
      holidayColumns.Clear();
      List<DateTime> holidays =
        HolidayCalculator.HolidayColumnsInPeriod(startDate, endDate);
      foreach (DateTime date in holidays)
      {
        holidayColumns.Add((date - startDate).Days);
      }
    }

    /// <summary>
    /// Marks the selected cells of calendarDataGrid with the given ActivityType
    /// </summary>
    /// <param name="markType">ActivityType from Activity enum</param>
    private void MarkSelected(Activity.activityType markType)
    {
      Console.WriteLine("started");
      foreach (DataGridViewCell cell in calendarDataGird.SelectedCells)
      {
        // If cell is a weekend, holiday or the None option is
        // somehow chosen, skip
        if (!IsCellMarkable(cell) || markType == 0) continue;

        Activity newActivity = new Activity(
            startDate.AddDays(cell.ColumnIndex), markType);

        //Find list of the marked person
        List<Activity> relevantList =
          calendarList[cell.RowIndex - 1].ActivityList;

        Activity foundActivity =
          relevantList.Find(act => act.Date == newActivity.Date);

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
      calendarDataGird.ClearSelection();
    }

    /// <summary>
    /// Returns true if the column is a holiday
    /// </summary>
    /// <param name="columnIndex">Index of column to check</param>
    /// <returns></returns>
    private bool IsColumnHoliday(int columnIndex)
    {
      return holidayColumns.Contains(columnIndex);
    }

    /// <summary>
    /// Returns true if the column is a weekend
    /// </summary>
    /// <param name="columnIndex">Index of column to check</param>
    /// <returns></returns>
    private bool IsColumnWeekend(int columnIndex)
    {
      return (columnIndex % 7) > 4;
    }
    private void FocusConfig(object sender, EventArgs e, ConfigForm config)
    {
      config.Activate();
    }

    /// <summary>
    /// Returns value of the timePicker in whole days
    /// </summary>
    /// <returns></returns>
    private DateTime FetchStartDateFromTimepicker()
    {
      Log.Add("Date fetched from time picker: " + dateTimePicker1.Value);
      return dateTimePicker1.Value.Date;
    }

    /// <summary>
    /// Returns a three letter string associated with the given activityType
    /// </summary>
    /// <param name="activityCode"></param>
    /// <returns></returns>
    private string ActivityText(Activity.activityType activityCode)
    {
      switch (activityCode)
      {
        case Activity.activityType.Fridag:
          return "FRI";

        case Activity.activityType.Afspadsering:
          return "AFS";

        case Activity.activityType.Kursus:
          return "KRS";

        case Activity.activityType.Selvvalgt1:
          return Settings.Default.CustomText1;

        case Activity.activityType.Selvvalgt2:
          return Settings.Default.CustomText2;

        case Activity.activityType.Selvvalgt3:
          return Settings.Default.CustomText3;

        default:
          return "PRJ";
      }
    }

    /// <summary>
    /// Returns the color associsated with the given activityType
    /// </summary>
    /// <param name="activityCode"></param>
    /// <returns></returns>
    private Color ActivityColor(Activity.activityType activityCode)
    {
      switch (activityCode)
      {
        case Activity.activityType.Fridag:
          return Color.LightGreen;

        case Activity.activityType.Afspadsering:
          return Color.Green;

        case Activity.activityType.Projekt:
          return Color.DarkCyan;

        case Activity.activityType.Kursus:
          return Color.Wheat;

        case Activity.activityType.Selvvalgt1:
          return Settings.Default.CustomColor1;

        case Activity.activityType.Selvvalgt2:
          return Settings.Default.CustomColor2;

        case Activity.activityType.Selvvalgt3:
          return Settings.Default.CustomColor3;

        default:
          Log.Add("Found unknown color: " + activityCode);
          return unknownColor;
      }
    }

    /// <summary>
    /// Checks pressed keys for hotkey combinations
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
      if (e.KeyData == (Keys.L | Keys.Control))
      {

        if (!calendarLocked)
        {
          LockCalendar();
          e.Handled = true;
        }
        else
        {
          UnlockCalendar();
          e.Handled = true;
        }
      }
    }
    private void LockCalendar()
    {
      calendarLocked = true;
      Save_Click(this, null);
      MarkButton.Enabled = false;
      UnmarkButton.Enabled = false;
      activityPicker.Enabled = false;
      lockButton.Image = Resources.unlock1600;

      calendarDataGird.ClearSelection();
      personDataGrid.ClearSelection();

      EventHandler deselectHandler = null;
      deselectHandler = delegate (object sender, EventArgs e)
      {
        if (!calendarLocked)
        {
          ((DataGridView)sender).SelectionChanged -= deselectHandler;
        }
        else
        {
          ((DataGridView)sender).ClearSelection();
        }
      };

      personDataGrid.SelectionChanged += deselectHandler;
      calendarDataGird.SelectionChanged += deselectHandler;
    }

    private void UnlockCalendar()
    {
      using (PasswordDialog passDia = new PasswordDialog())
      {
        string pass = "";
        do
        {
          pass = passDia.Password();
          if (pass == "Pass")
          {
            calendarLocked = false;
            MarkButton.Enabled = true;
            UnmarkButton.Enabled = true;
            activityPicker.Enabled = true;
            lockButton.Image = Resources.lock1600;
            break;
          }
        } while (pass != null);
      }
    }

    /// <summary>
    /// Returns true if the given cell is an eligible workday
    /// </summary>
    /// <param name="cell">Cell to be checked</param>
    /// <returns></returns>
    private bool IsCellMarkable(DataGridViewCell cell)
    {
      return !(
        cell.RowIndex < 1 ||
        IsColumnWeekend(cell.ColumnIndex) ||
        IsColumnHoliday(cell.ColumnIndex) ||
        calendarList[cell.RowIndex - 1].Name == ".");
    }
    #endregion
  }
}
