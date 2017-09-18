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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.BackButton = new System.Windows.Forms.Button();
      this.MarkButton = new System.Windows.Forms.Button();
      this.dataGridView2 = new System.Windows.Forms.DataGridView();
      this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.SaveButton = new System.Windows.Forms.Button();
      this.UnmarkButton = new System.Windows.Forms.Button();
      this.Startdate = new System.Windows.Forms.Label();
      this.WeeksToShow = new System.Windows.Forms.Label();
      this.activityPicker = new System.Windows.Forms.ComboBox();
      this.configButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
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
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
      dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
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
      // 
      // BackButton
      // 
      this.BackButton.Location = new System.Drawing.Point(9, 324);
      this.BackButton.Name = "BackButton";
      this.BackButton.Size = new System.Drawing.Size(49, 23);
      this.BackButton.TabIndex = 7;
      this.BackButton.Text = "Back";
      this.BackButton.UseVisualStyleBackColor = true;
      this.BackButton.Click += new System.EventHandler(this.CloseWindow);
      // 
      // MarkButton
      // 
      this.MarkButton.Location = new System.Drawing.Point(12, 12);
      this.MarkButton.Name = "MarkButton";
      this.MarkButton.Size = new System.Drawing.Size(107, 23);
      this.MarkButton.TabIndex = 1;
      this.MarkButton.Text = "Mark";
      this.MarkButton.UseVisualStyleBackColor = true;
      this.MarkButton.Click += new System.EventHandler(this.MarkSelected);
      // 
      // dataGridView2
      // 
      this.dataGridView2.AllowDrop = true;
      this.dataGridView2.AllowUserToDeleteRows = false;
      this.dataGridView2.AllowUserToResizeColumns = false;
      this.dataGridView2.AllowUserToResizeRows = false;
      this.dataGridView2.ColumnHeadersHeight = 25;
      this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      this.dataGridView2.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
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
      // 
      // dateTimePicker1
      // 
      this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
      this.dateTimePicker1.Location = new System.Drawing.Point(67, 109);
      this.dateTimePicker1.Name = "dateTimePicker1";
      this.dateTimePicker1.Size = new System.Drawing.Size(80, 20);
      this.dateTimePicker1.TabIndex = 4;
      this.dateTimePicker1.Value = new System.DateTime(2017, 9, 18, 0, 0, 0, 0);
      this.dateTimePicker1.ValueChanged += new System.EventHandler(this.UpdateCalendarActiveTimespan);
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(94, 135);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(47, 20);
      this.textBox1.TabIndex = 5;
      this.textBox1.Text = "4";
      this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
      this.textBox1.Leave += new System.EventHandler(this.textBox1_Leave);
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
      // 
      // configButton
      // 
      this.configButton.Location = new System.Drawing.Point(9, 194);
      this.configButton.Name = "configButton";
      this.configButton.Size = new System.Drawing.Size(75, 23);
      this.configButton.TabIndex = 6;
      this.configButton.Text = "Config";
      this.configButton.UseVisualStyleBackColor = true;
      this.configButton.Click += new System.EventHandler(this.ToConfig);
      // 
      // CalendarEditor
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(863, 359);
      this.Controls.Add(this.configButton);
      this.Controls.Add(this.activityPicker);
      this.Controls.Add(this.WeeksToShow);
      this.Controls.Add(this.Startdate);
      this.Controls.Add(this.UnmarkButton);
      this.Controls.Add(this.SaveButton);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.dateTimePicker1);
      this.Controls.Add(this.dataGridView2);
      this.Controls.Add(this.MarkButton);
      this.Controls.Add(this.BackButton);
      this.Controls.Add(this.dataGridView1);
      this.Name = "CalendarEditor";
      this.Text = "Calendar Editor";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SaveWindowState);
      this.Load += new System.EventHandler(this.InitDataGrid);
      this.Click += new System.EventHandler(this.DeselectAll);
      this.Layout += new System.Windows.Forms.LayoutEventHandler(this.FitDatagrid);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.Button BackButton;
    private System.Windows.Forms.Button MarkButton;
    private System.Windows.Forms.DataGridView dataGridView2;
    private System.Windows.Forms.DateTimePicker dateTimePicker1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Button UnmarkButton;
    private System.Windows.Forms.Label Startdate;
    private System.Windows.Forms.Label WeeksToShow;
    private System.Windows.Forms.ComboBox activityPicker;
    private System.Windows.Forms.Button SaveButton;
    private System.Windows.Forms.Button configButton;
  }
}