namespace Thy_El_Teknik_Kalender_9000
{
  partial class CalendarEditor
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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.calendarContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.BackButton = new System.Windows.Forms.Button();
      this.MarkButton = new System.Windows.Forms.Button();
      this.dataGridView2 = new System.Windows.Forms.DataGridView();
      this.personContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
      this.SaveButton = new System.Windows.Forms.Button();
      this.UnmarkButton = new System.Windows.Forms.Button();
      this.Startdate = new System.Windows.Forms.Label();
      this.WeeksToShow = new System.Windows.Forms.Label();
      this.activityPicker = new System.Windows.Forms.ComboBox();
      this.configButton = new System.Windows.Forms.Button();
      this.weekNumber = new System.Windows.Forms.NumericUpDown();
      this.refreshButton = new System.Windows.Forms.Button();
      this.updateTimer = new System.Windows.Forms.Timer(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.weekNumber)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AllowUserToResizeColumns = false;
      this.dataGridView1.AllowUserToResizeRows = false;
      this.dataGridView1.ColumnHeadersHeight = 25;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      this.dataGridView1.ContextMenuStrip = this.calendarContextMenu;
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle2.Font = new System.Drawing.Font("Arial", 6F);
      dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
      this.dataGridView1.Location = new System.Drawing.Point(350, 12);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.ReadOnly = true;
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.Size = new System.Drawing.Size(501, 335);
      this.dataGridView1.TabIndex = 10;
      this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
      this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
      this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
      this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
      this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // calendarContextMenu
      // 
      this.calendarContextMenu.Name = "contextMenuStrip1";
      this.calendarContextMenu.Size = new System.Drawing.Size(61, 4);
      // 
      // BackButton
      // 
      this.BackButton.Location = new System.Drawing.Point(9, 324);
      this.BackButton.Name = "BackButton";
      this.BackButton.Size = new System.Drawing.Size(49, 23);
      this.BackButton.TabIndex = 7;
      this.BackButton.Text = "Close";
      this.BackButton.UseVisualStyleBackColor = true;
      this.BackButton.Click += new System.EventHandler(this.CloseButton);
      this.BackButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // MarkButton
      // 
      this.MarkButton.Location = new System.Drawing.Point(12, 12);
      this.MarkButton.Name = "MarkButton";
      this.MarkButton.Size = new System.Drawing.Size(107, 23);
      this.MarkButton.TabIndex = 1;
      this.MarkButton.Text = "Mark";
      this.MarkButton.UseVisualStyleBackColor = true;
      this.MarkButton.Click += new System.EventHandler(this.MarkClicked);
      this.MarkButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // dataGridView2
      // 
      this.dataGridView2.AllowDrop = true;
      this.dataGridView2.AllowUserToDeleteRows = false;
      this.dataGridView2.AllowUserToResizeColumns = false;
      this.dataGridView2.AllowUserToResizeRows = false;
      this.dataGridView2.ColumnHeadersHeight = 25;
      this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      this.dataGridView2.ContextMenuStrip = this.personContextMenu;
      this.dataGridView2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
      this.dataGridView2.Location = new System.Drawing.Point(150, 12);
      this.dataGridView2.MultiSelect = false;
      this.dataGridView2.Name = "dataGridView2";
      this.dataGridView2.RowHeadersVisible = false;
      this.dataGridView2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
      this.dataGridView2.Size = new System.Drawing.Size(200, 335);
      this.dataGridView2.TabIndex = 9;
      this.dataGridView2.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView2_CellBeginEdit);
      this.dataGridView2.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView2_RowsAdded);
      this.dataGridView2.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataGridView2_RowsRemoved);
      this.dataGridView2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView2_Scroll);
      this.dataGridView2.Click += new System.EventHandler(this.dataGridView2_Click);
      this.dataGridView2.DoubleClick += new System.EventHandler(this.dataGridView2_CellDoubleClick);
      this.dataGridView2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // personContextMenu
      // 
      this.personContextMenu.Name = "personContextMenu";
      this.personContextMenu.Size = new System.Drawing.Size(61, 4);
      // 
      // dateTimePicker1
      // 
      this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimePicker1.Location = new System.Drawing.Point(65, 109);
      this.dateTimePicker1.Name = "dateTimePicker1";
      this.dateTimePicker1.Size = new System.Drawing.Size(80, 20);
      this.dateTimePicker1.TabIndex = 4;
      this.dateTimePicker1.Value = new System.DateTime(2017, 9, 18, 0, 0, 0, 0);
      this.dateTimePicker1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // SaveButton
      // 
      this.SaveButton.Location = new System.Drawing.Point(66, 324);
      this.SaveButton.Name = "SaveButton";
      this.SaveButton.Size = new System.Drawing.Size(75, 23);
      this.SaveButton.TabIndex = 8;
      this.SaveButton.Text = "Save";
      this.SaveButton.UseVisualStyleBackColor = true;
      this.SaveButton.Click += new System.EventHandler(this.Save_Click);
      this.SaveButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // UnmarkButton
      // 
      this.UnmarkButton.Location = new System.Drawing.Point(12, 68);
      this.UnmarkButton.Name = "UnmarkButton";
      this.UnmarkButton.Size = new System.Drawing.Size(107, 23);
      this.UnmarkButton.TabIndex = 3;
      this.UnmarkButton.Text = "Unmark";
      this.UnmarkButton.UseVisualStyleBackColor = true;
      this.UnmarkButton.Click += new System.EventHandler(this.UnmarkSelected);
      this.UnmarkButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // Startdate
      // 
      this.Startdate.AutoSize = true;
      this.Startdate.Location = new System.Drawing.Point(8, 112);
      this.Startdate.Name = "Startdate";
      this.Startdate.Size = new System.Drawing.Size(56, 13);
      this.Startdate.TabIndex = 8;
      this.Startdate.Text = "Start date:";
      // 
      // WeeksToShow
      // 
      this.WeeksToShow.AutoSize = true;
      this.WeeksToShow.Location = new System.Drawing.Point(9, 139);
      this.WeeksToShow.Name = "WeeksToShow";
      this.WeeksToShow.Size = new System.Drawing.Size(82, 13);
      this.WeeksToShow.TabIndex = 9;
      this.WeeksToShow.Text = "Weeks forward:";
      // 
      // activityPicker
      // 
      this.activityPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.activityPicker.FormattingEnabled = true;
      this.activityPicker.Location = new System.Drawing.Point(12, 41);
      this.activityPicker.Name = "activityPicker";
      this.activityPicker.Size = new System.Drawing.Size(107, 21);
      this.activityPicker.TabIndex = 2;
      this.activityPicker.SelectedIndexChanged += new System.EventHandler(this.SelectedActivity_Changed);
      this.activityPicker.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // configButton
      // 
      this.configButton.Location = new System.Drawing.Point(8, 190);
      this.configButton.Name = "configButton";
      this.configButton.Size = new System.Drawing.Size(75, 23);
      this.configButton.TabIndex = 6;
      this.configButton.Text = "Config";
      this.configButton.UseVisualStyleBackColor = true;
      this.configButton.Click += new System.EventHandler(this.ToConfig);
      this.configButton.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // weekNumber
      // 
      this.weekNumber.Location = new System.Drawing.Point(98, 137);
      this.weekNumber.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
      this.weekNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.weekNumber.Name = "weekNumber";
      this.weekNumber.Size = new System.Drawing.Size(47, 20);
      this.weekNumber.TabIndex = 11;
      this.weekNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.weekNumber.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
      this.weekNumber.ValueChanged += new System.EventHandler(this.weekNumber_ValueChanged);
      this.weekNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      // 
      // refreshButton
      // 
      this.refreshButton.Location = new System.Drawing.Point(66, 284);
      this.refreshButton.Name = "refreshButton";
      this.refreshButton.Size = new System.Drawing.Size(75, 23);
      this.refreshButton.TabIndex = 12;
      this.refreshButton.Text = "Refresh";
      this.refreshButton.UseVisualStyleBackColor = true;
      this.refreshButton.Click += new System.EventHandler(this.UpdateButton);
      // 
      // updateTimer
      // 
      this.updateTimer.Enabled = true;
      this.updateTimer.Interval = 600000;
      this.updateTimer.Tick += new System.EventHandler(this.UpdateTimerTick);
      // 
      // CalendarEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(863, 359);
      this.Controls.Add(this.refreshButton);
      this.Controls.Add(this.weekNumber);
      this.Controls.Add(this.configButton);
      this.Controls.Add(this.activityPicker);
      this.Controls.Add(this.WeeksToShow);
      this.Controls.Add(this.Startdate);
      this.Controls.Add(this.UnmarkButton);
      this.Controls.Add(this.SaveButton);
      this.Controls.Add(this.dateTimePicker1);
      this.Controls.Add(this.dataGridView2);
      this.Controls.Add(this.MarkButton);
      this.Controls.Add(this.BackButton);
      this.Controls.Add(this.dataGridView1);
      this.Name = "CalendarEditor";
      this.Text = "Calendar Editor";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveWindowState);
      this.Load += new System.EventHandler(this.InitDataGrid);
      this.Click += new System.EventHandler(this.DeselectGrids);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HotkeyDetection);
      this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FitDatagrid);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.weekNumber)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.Button BackButton;
    private System.Windows.Forms.Button MarkButton;
    private System.Windows.Forms.DataGridView dataGridView2;
    private System.Windows.Forms.DateTimePicker dateTimePicker1;
    private System.Windows.Forms.Button UnmarkButton;
    private System.Windows.Forms.Label Startdate;
    private System.Windows.Forms.Label WeeksToShow;
    private System.Windows.Forms.ComboBox activityPicker;
    private System.Windows.Forms.Button SaveButton;
    private System.Windows.Forms.Button configButton;
    private System.Windows.Forms.ContextMenuStrip calendarContextMenu;
    private System.Windows.Forms.NumericUpDown weekNumber;
    private System.Windows.Forms.Button refreshButton;
    private System.Windows.Forms.Timer updateTimer;
    private System.Windows.Forms.ContextMenuStrip personContextMenu;
  }
}