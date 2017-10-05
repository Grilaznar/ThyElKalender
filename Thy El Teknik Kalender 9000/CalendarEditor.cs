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
    //List of all data
    private List<Person> calendarList = new List<Person>();

    private DateTime startDate, endDate;
    private List<int> holidayColumns = new List<int>();

    //Colors for calendar fields
    //private Color holidayColor = Color.Aquamarine;
    //private Color weekendColor = Color.Gray;

    //System Colors
    private Color errorColor = Color.Red;
    private Color unknownColor = Color.Pink;

    private bool unsavedChanges = false;

    public CalendarEditor()
    {
      Console.WriteLine("LOOK HERE:");
      Console.WriteLine((int)Activity.activityType.Offday);
      Log.ResetLogFile();

      Log.Add("Calenar window starting");
      InitializeComponent();

      // Ask if you want to save changes
      FormClosing += CloseWindow;

      // Buttons will overlap if smaller
      MinimumSize = new Size(440, 340);

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

      LoadWindowState();

      personDataGrid.Columns.Add("Names", "Names");
      personDataGrid.Columns.Add("Department", "Department");

      int nameCellWidth = 118;
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
      Log.Add("Window state:"
        + " Location '" + Location.X + " x " + Location.Y + "'"
        + " Size '" + Size.Width + " x " + Size.Height + "'");
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

        if (isColumnHoliday(i))
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
      SaveWindowState(this, null);

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
    #endregion

    #region Datagrid events
    private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      if (personDataGrid.ContainsFocus
        && e.RowIndex == personDataGrid.RowCount - 1)
      {
        calendarDataGird.Rows.Add();

        DataGridViewCellEventHandler saveRowChanges = null;
        saveRowChanges = delegate (object s, DataGridViewCellEventArgs ev)
        {
          if (ev.RowIndex < personDataGrid.RowCount - 1)
          {
            Person person =
              new Person(
                (string)personDataGrid[ev.ColumnIndex, ev.RowIndex].Value,
                "");

            AddPerson(person);

            if (person.Name == ".")
            {
              UpdateCalendarContent();
            }

            MarkedForSave();

            personDataGrid.CellEndEdit -= saveRowChanges;
          }
        };
        personDataGrid.CellEndEdit += saveRowChanges;
      }
    }

    private void dataGridView2_SelectionChanged(object sender, EventArgs e)
    {
      if (personDataGrid.SelectedCells.Count > 0)
      {
        personContextMenu.Items.Find("Insert", false)[0].Enabled = true;
        personContextMenu.Items.Find("Delete", false)[0].Enabled = true;
      }
      else
      {
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
    private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
    {
      bool useV2 = true;
      if (e.RowIndex == -1)
      {
        int mondayColumnIndex = e.ColumnIndex - e.ColumnIndex % 7;
        bool isMondayColumn = e.ColumnIndex % 7 == 0;

        if (useV2)
        {
          e.PaintBackground(e.CellBounds, false);
          if (e.ColumnIndex % 7 < 2)
          {
            string headerText =
                "Uge " + HolidayCalculator.WeekNumber(
                  startDate.AddDays(mondayColumnIndex));

            SizeF sz = e.Graphics.MeasureString(
                headerText,
                calendarDataGird.Font);

            Point drawStart = new Point(
              (int)e.CellBounds.Left + 4 - (e.ColumnIndex % 7 * 22),
              (int)(e.CellBounds.Bottom / 2 - sz.Height / 2));
            if (e.ColumnIndex == 0) drawStart.X += 1;

            e.Graphics.SetClip(e.ClipBounds);
            e.Graphics.DrawString(
              headerText,
              calendarDataGird.ColumnHeadersDefaultCellStyle.Font,
              Brushes.Black,
              drawStart);
          }
        }

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
    private void AddPerson(Person person)
    {
      calendarList.Add(person);
    }

    private bool ChangePersonName(int index, string newName)
    {
      calendarList[index].Name = newName;
      return true;
    }

    private void UpdateTimerTick(object sender, EventArgs e)
    {
      if (!unsavedChanges && calendarDataGird.SelectedCells.Count == 0)
      {
        UpdateButton(sender, e);
      }
    }

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

    private bool isColumnHoliday(int columnIndex)
    {
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

        case Activity.activityType.Custom1:
          return Settings.Default.CustomText1;

        case Activity.activityType.Custom2:
          return Settings.Default.CustomText2;

        case Activity.activityType.Custom3:
          return Settings.Default.CustomText3;

        default:
          return "PRJ";
      }
    }

    private Color ActivityColor(Activity.activityType activityCode)
    {
      switch (activityCode)
      {
        case Activity.activityType.Offday:
          return Color.LightGreen;

        case Activity.activityType.Counterbalance:
          return Color.Green;

        case Activity.activityType.Project:
          return Color.DarkCyan;

        case Activity.activityType.Course:
          return Color.Wheat;

        case Activity.activityType.Custom1:
          return Settings.Default.CustomColor1;

        case Activity.activityType.Custom2:
          return Settings.Default.CustomColor2;

        case Activity.activityType.Custom3:
          return Settings.Default.CustomColor3;

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
        calendarList[cell.RowIndex - 1].Name == ".");
    }
    #endregion

    #region Not Active
    /*
    
    private bool ChangePersonName(Person person, string newName)
    {
      if (!calendarList.Contains(new Person(newName), new PersonComparer()))
      {
        calendarList.Find(p => p.Name == person.Name).Name = newName;

        return true;
      }
      return false;
    }

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
