namespace HockeyStats
{
    partial class PlayerStatForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.topTableDGV = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.renameListTextbox = new System.Windows.Forms.TextBox();
            this.listNameLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadListDropDown = new System.Windows.Forms.ToolStripMenuItem();
            this.saveListDropDown = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRemoveColumnDropDown = new System.Windows.Forms.ToolStripMenuItem();
            this.selectSeasonDropDown = new System.Windows.Forms.ToolStripMenuItem();
            this.selectPrimarySeasonTypeDropDown = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.rightTableDGV = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.searchPlayerButton = new System.Windows.Forms.Button();
            this.searchPlayerTextbox = new System.Windows.Forms.TextBox();
            this.addSelectedPlayerButton = new System.Windows.Forms.Button();
            this.clearSearchButton = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.leftTableDGV = new System.Windows.Forms.DataGridView();
            this.middleTableDGV = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.removeSelectedPlayerButton = new System.Windows.Forms.Button();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.selectSecondarySeasonTypeDropDown = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.setAsDefaultListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.topTableDGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightTableDGV)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leftTableDGV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.middleTableDGV)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // topTableDGV
            // 
            this.topTableDGV.AllowUserToAddRows = false;
            this.topTableDGV.AllowUserToDeleteRows = false;
            this.topTableDGV.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightGray;
            this.topTableDGV.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.topTableDGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.topTableDGV.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.topTableDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.topTableDGV.DefaultCellStyle = dataGridViewCellStyle2;
            this.topTableDGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topTableDGV.Location = new System.Drawing.Point(0, 24);
            this.topTableDGV.MultiSelect = false;
            this.topTableDGV.Name = "topTableDGV";
            this.topTableDGV.ReadOnly = true;
            this.topTableDGV.RowHeadersVisible = false;
            this.topTableDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.topTableDGV.Size = new System.Drawing.Size(1084, 432);
            this.topTableDGV.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.renameListTextbox);
            this.splitContainer1.Panel1.Controls.Add(this.listNameLabel);
            this.splitContainer1.Panel1.Controls.Add(this.topTableDGV);
            this.splitContainer1.Panel1.Controls.Add(this.menuStrip1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1084, 861);
            this.splitContainer1.SplitterDistance = 456;
            this.splitContainer1.SplitterWidth = 10;
            this.splitContainer1.TabIndex = 3;
            // 
            // renameListTextbox
            // 
            this.renameListTextbox.Enabled = false;
            this.renameListTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.renameListTextbox.Location = new System.Drawing.Point(558, 0);
            this.renameListTextbox.Name = "renameListTextbox";
            this.renameListTextbox.Size = new System.Drawing.Size(100, 23);
            this.renameListTextbox.TabIndex = 3;
            this.renameListTextbox.Visible = false;
            // 
            // listNameLabel
            // 
            this.listNameLabel.AutoSize = true;
            this.listNameLabel.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.listNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listNameLabel.Location = new System.Drawing.Point(463, 4);
            this.listNameLabel.Name = "listNameLabel";
            this.listNameLabel.Size = new System.Drawing.Size(89, 20);
            this.listNameLabel.TabIndex = 2;
            this.listNameLabel.Text = "List Name";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadListDropDown,
            this.saveListDropDown,
            this.createListToolStripMenuItem,
            this.addRemoveColumnDropDown,
            this.selectSeasonDropDown,
            this.selectPrimarySeasonTypeDropDown,
            this.deleteListToolStripMenuItem,
            this.setAsDefaultListToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1084, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadListDropDown
            // 
            this.loadListDropDown.Name = "loadListDropDown";
            this.loadListDropDown.Size = new System.Drawing.Size(66, 20);
            this.loadListDropDown.Text = "Load List";
            // 
            // saveListDropDown
            // 
            this.saveListDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.saveListDropDown.Name = "saveListDropDown";
            this.saveListDropDown.Size = new System.Drawing.Size(64, 20);
            this.saveListDropDown.Text = "Save List";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            // 
            // createListToolStripMenuItem
            // 
            this.createListToolStripMenuItem.Name = "createListToolStripMenuItem";
            this.createListToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.createListToolStripMenuItem.Text = "Create List";
            // 
            // addRemoveColumnDropDown
            // 
            this.addRemoveColumnDropDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.addRemoveColumnDropDown.Name = "addRemoveColumnDropDown";
            this.addRemoveColumnDropDown.Size = new System.Drawing.Size(135, 20);
            this.addRemoveColumnDropDown.Text = "Add/Remove Column";
            // 
            // selectSeasonDropDown
            // 
            this.selectSeasonDropDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.selectSeasonDropDown.Name = "selectSeasonDropDown";
            this.selectSeasonDropDown.Size = new System.Drawing.Size(90, 20);
            this.selectSeasonDropDown.Text = "Select Season";
            // 
            // selectPrimarySeasonTypeDropDown
            // 
            this.selectPrimarySeasonTypeDropDown.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.selectPrimarySeasonTypeDropDown.Name = "selectPrimarySeasonTypeDropDown";
            this.selectPrimarySeasonTypeDropDown.Size = new System.Drawing.Size(118, 20);
            this.selectPrimarySeasonTypeDropDown.Text = "Select Season Type";
            // 
            // deleteListToolStripMenuItem
            // 
            this.deleteListToolStripMenuItem.Name = "deleteListToolStripMenuItem";
            this.deleteListToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.deleteListToolStripMenuItem.Text = "Delete List";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 536F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.rightTableDGV, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1084, 395);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // rightTableDGV
            // 
            this.rightTableDGV.AllowUserToAddRows = false;
            this.rightTableDGV.AllowUserToDeleteRows = false;
            this.rightTableDGV.AllowUserToOrderColumns = true;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.LightGray;
            this.rightTableDGV.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.rightTableDGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.rightTableDGV.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.rightTableDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.rightTableDGV.DefaultCellStyle = dataGridViewCellStyle4;
            this.rightTableDGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightTableDGV.Location = new System.Drawing.Point(539, 35);
            this.rightTableDGV.Name = "rightTableDGV";
            this.rightTableDGV.ReadOnly = true;
            this.rightTableDGV.RowHeadersVisible = false;
            this.rightTableDGV.Size = new System.Drawing.Size(542, 357);
            this.rightTableDGV.TabIndex = 3;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.40881F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.59119F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
            this.tableLayoutPanel3.Controls.Add(this.searchPlayerButton, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.searchPlayerTextbox, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.addSelectedPlayerButton, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.clearSearchButton, 2, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(530, 26);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // searchPlayerButton
            // 
            this.searchPlayerButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchPlayerButton.Location = new System.Drawing.Point(212, 3);
            this.searchPlayerButton.Name = "searchPlayerButton";
            this.searchPlayerButton.Size = new System.Drawing.Size(105, 20);
            this.searchPlayerButton.TabIndex = 1;
            this.searchPlayerButton.Text = "Search";
            this.searchPlayerButton.UseVisualStyleBackColor = true;
            // 
            // searchPlayerTextbox
            // 
            this.searchPlayerTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchPlayerTextbox.Location = new System.Drawing.Point(3, 3);
            this.searchPlayerTextbox.Name = "searchPlayerTextbox";
            this.searchPlayerTextbox.Size = new System.Drawing.Size(203, 20);
            this.searchPlayerTextbox.TabIndex = 3;
            // 
            // addSelectedPlayerButton
            // 
            this.addSelectedPlayerButton.Enabled = false;
            this.addSelectedPlayerButton.Location = new System.Drawing.Point(407, 3);
            this.addSelectedPlayerButton.Name = "addSelectedPlayerButton";
            this.addSelectedPlayerButton.Size = new System.Drawing.Size(111, 20);
            this.addSelectedPlayerButton.TabIndex = 4;
            this.addSelectedPlayerButton.Text = "Add Selected Player";
            this.addSelectedPlayerButton.UseVisualStyleBackColor = true;
            // 
            // clearSearchButton
            // 
            this.clearSearchButton.Enabled = false;
            this.clearSearchButton.Location = new System.Drawing.Point(323, 3);
            this.clearSearchButton.Name = "clearSearchButton";
            this.clearSearchButton.Size = new System.Drawing.Size(78, 20);
            this.clearSearchButton.TabIndex = 5;
            this.clearSearchButton.Text = "Clear Search";
            this.clearSearchButton.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 35);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.leftTableDGV);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.middleTableDGV);
            this.splitContainer2.Size = new System.Drawing.Size(530, 357);
            this.splitContainer2.SplitterDistance = 329;
            this.splitContainer2.TabIndex = 4;
            // 
            // leftTableDGV
            // 
            this.leftTableDGV.AllowUserToAddRows = false;
            this.leftTableDGV.AllowUserToDeleteRows = false;
            this.leftTableDGV.AllowUserToOrderColumns = true;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.LightGray;
            this.leftTableDGV.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.leftTableDGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.leftTableDGV.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.leftTableDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.leftTableDGV.DefaultCellStyle = dataGridViewCellStyle6;
            this.leftTableDGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftTableDGV.Location = new System.Drawing.Point(0, 0);
            this.leftTableDGV.Name = "leftTableDGV";
            this.leftTableDGV.ReadOnly = true;
            this.leftTableDGV.RowHeadersVisible = false;
            this.leftTableDGV.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.leftTableDGV.Size = new System.Drawing.Size(329, 357);
            this.leftTableDGV.TabIndex = 4;
            // 
            // middleTableDGV
            // 
            this.middleTableDGV.AllowUserToAddRows = false;
            this.middleTableDGV.AllowUserToDeleteRows = false;
            this.middleTableDGV.AllowUserToOrderColumns = true;
            this.middleTableDGV.AllowUserToResizeColumns = false;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.LightGray;
            this.middleTableDGV.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle7;
            this.middleTableDGV.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.middleTableDGV.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.middleTableDGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.middleTableDGV.DefaultCellStyle = dataGridViewCellStyle8;
            this.middleTableDGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.middleTableDGV.Location = new System.Drawing.Point(0, 0);
            this.middleTableDGV.Name = "middleTableDGV";
            this.middleTableDGV.ReadOnly = true;
            this.middleTableDGV.RowHeadersVisible = false;
            this.middleTableDGV.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.middleTableDGV.Size = new System.Drawing.Size(197, 357);
            this.middleTableDGV.TabIndex = 4;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.removeSelectedPlayerButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.menuStrip2, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(539, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(542, 26);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // removeSelectedPlayerButton
            // 
            this.removeSelectedPlayerButton.Enabled = false;
            this.removeSelectedPlayerButton.Location = new System.Drawing.Point(3, 3);
            this.removeSelectedPlayerButton.Name = "removeSelectedPlayerButton";
            this.removeSelectedPlayerButton.Size = new System.Drawing.Size(134, 20);
            this.removeSelectedPlayerButton.TabIndex = 0;
            this.removeSelectedPlayerButton.Text = "Remove Selected Player";
            this.removeSelectedPlayerButton.UseVisualStyleBackColor = true;
            // 
            // menuStrip2
            // 
            this.menuStrip2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.menuStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectSecondarySeasonTypeDropDown});
            this.menuStrip2.Location = new System.Drawing.Point(416, 1);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(126, 24);
            this.menuStrip2.TabIndex = 1;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // selectSecondarySeasonTypeDropDown
            // 
            this.selectSecondarySeasonTypeDropDown.Name = "selectSecondarySeasonTypeDropDown";
            this.selectSecondarySeasonTypeDropDown.Size = new System.Drawing.Size(118, 20);
            this.selectSecondarySeasonTypeDropDown.Text = "Select Season Type";
            // 
            // setAsDefaultListToolStripMenuItem
            // 
            this.setAsDefaultListToolStripMenuItem.Name = "setAsDefaultListToolStripMenuItem";
            this.setAsDefaultListToolStripMenuItem.Size = new System.Drawing.Size(113, 20);
            this.setAsDefaultListToolStripMenuItem.Text = "Set As Default List";
            // 
            // PlayerStatForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 861);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "PlayerStatForm";
            this.Text = "Hockey Stats";
            ((System.ComponentModel.ISupportInitialize)(this.topTableDGV)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightTableDGV)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.leftTableDGV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.middleTableDGV)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView topTableDGV;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView rightTableDGV;
        private System.Windows.Forms.DataGridView middleTableDGV;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem loadListDropDown;
        private System.Windows.Forms.ToolStripMenuItem createListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveListDropDown;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem addRemoveColumnDropDown;
        private System.Windows.Forms.ToolStripMenuItem selectSeasonDropDown;
        private System.Windows.Forms.Label listNameLabel;
        private System.Windows.Forms.ToolStripMenuItem selectPrimarySeasonTypeDropDown;
        private System.Windows.Forms.DataGridView leftTableDGV;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button searchPlayerButton;
        private System.Windows.Forms.TextBox searchPlayerTextbox;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button addSelectedPlayerButton;
        private System.Windows.Forms.Button clearSearchButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button removeSelectedPlayerButton;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem selectSecondarySeasonTypeDropDown;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteListToolStripMenuItem;
        private System.Windows.Forms.TextBox renameListTextbox;
        private System.Windows.Forms.ToolStripMenuItem setAsDefaultListToolStripMenuItem;
    }
}

