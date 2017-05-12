﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;
using System.Linq;
using System.ComponentModel;

namespace HockeyStats
{
    public partial class PlayerStatForm : Form
    {
        private const string defaultPlayerListName = "bluesProspectsShort";
        
        PlayerList playerList = new PlayerList();
        private MultiPlayerStatTable topTable;
        private SearchDataStatTable leftTable;
        private PlayerConstantsStatTable middleTable;
        private SinglePlayerStatTable rightTable;
        private string currentDisplaySeason;
        private bool listIsSaved;
        private bool tableHasBeenClicked;
        private bool rowJustSelected = false;

        public PlayerStatForm()
        {
            InitializeComponent();

            PlayerList defaultPlayerList = Serializer.ReadPlayerList<PlayerList>(defaultPlayerListName + Constants.FILENAME_SUFFIX);
            LoadPlayerList(defaultPlayerList);
            
            SetupLoadListDropDown();
            SetupSaveListButton();
            SetupCreateListButton();
            SetupSelectSeasonTypeButtons();
            SetupSelectSeasonButton();
            SetupAddRemoveColumnButton();
            SetupSearchPlayerButton();
            SetupClearSearchButton();
            SetupAddSelectedPlayerButton();
            SetupRemoveSelectedPlayerButton();
            SetupShowSelectedPlayer();
            SetupUnselectPlayer();
            SetupFormClosingHandler();
            SetupColumnResizeListener();
            SetupTableClickListener();
        }

        private void LoadPlayerList(PlayerList playerListToLoad)
        {
            playerList = playerListToLoad;
            currentDisplaySeason = playerList.displaySeason;
            listNameLabel.Text = playerList.listName;
            if (topTable != null) { topTable.AbortFillDataTableThread(); }

            topTable = new MultiPlayerStatTable(topTableDGV, playerList.primaryColumnNames, playerList.playerIds, playerList.displaySeason, playerList.primarySeasonType);
            if (leftTable == null) { leftTable = new SearchDataStatTable(leftTableDGV, Constants.DefaultSearchDataTableColumns); }
            middleTable = new PlayerConstantsStatTable(middleTableDGV);
            rightTable = new SinglePlayerStatTable(rightTableDGV, playerList.secondaryColumnNames, playerList.secondarySeasonType);
            
            RedrawColumnWidths(topTableDGV, playerList.GetPrimaryColumnWidth);
            RedrawColumnWidths(rightTableDGV, playerList.GetSecondaryColumnWidth);
            RedrawRowColors();

            SetListIsSaved(true);
            tableHasBeenClicked = false;
        }

        private void SetupLoadListDropDown()
        {
            ToolStripItemCollection dropDownItems = loadListDropDown.DropDownItems;
            dropDownItems.Clear();
            string[] playerListFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + Constants.FILENAME_SUFFIX);
            foreach (string fileWithPath in playerListFiles)
            {
                // Get the name without the file path or suffix
                string file = Path.GetFileName(fileWithPath);
                string listName = file.Substring(0, file.Length - Constants.FILENAME_SUFFIX.Length);

                PlayerList playerListToLoad = Serializer.ReadPlayerList<PlayerList>(listName + Constants.FILENAME_SUFFIX);
                EventHandler selectPlayerListHandler = new EventHandler((object sender, EventArgs e) => {
                    Action LoadPlayer = () => {
                        LoadPlayerList(playerListToLoad);
                        RefreshDropDownLists();
                    };

                    TriggerLeaveRequest(LoadPlayer);
                });

                dropDownItems.Add(listName, null, selectPlayerListHandler);
                if (playerList.listName == listName)
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupSaveListButton()
        {
            saveFileDialog.Filter = "Player List|*" + Constants.FILENAME_SUFFIX;
            saveFileDialog.Title = "Save Player List";
            saveListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                saveFileDialog.FileName = playerList.listName;
                DialogResult result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    string fileName = Path.GetFileName(saveFileDialog.FileName);
                    string listName = TrimFileNameSuffix(fileName);
                    string listNameWithSuffix = listName + Constants.FILENAME_SUFFIX;
                    playerList.SetListName(listName);
                    playerList.SetPrimarySeasonType(topTable.GetSeasonType());
                    playerList.SetSecondarySeasonType(rightTable.GetSeasonType());
                    playerList.SetDisplaySeason(currentDisplaySeason);
                    playerList.SetPlayerIds(topTable.GetPlayerIds());
                    playerList.SetPrimaryColumns(topTableDGV.Columns);
                    playerList.SetPrimaryColumnWidths(topTableDGV.Columns);
                    playerList.SetSecondaryColumnWidths(rightTableDGV.Columns);
                    Serializer.WritePlayerList<PlayerList>(playerList, listNameWithSuffix);
                    RefreshDropDownLists();
                    SetListIsSaved(true);
                }
            });
        }

        private void SetupCreateListButton()
        {
            createListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) => {
                Action CreateList = () => {
                    PlayerList playerListToLoad = new PlayerList();
                    playerListToLoad.FillWithDefaults();
                    LoadPlayerList(playerListToLoad);
                    RefreshDropDownLists();
                    SetListIsSaved(false);
                };

                TriggerLeaveRequest(CreateList);
            });
        }

        private void SetupSelectSeasonTypeButtons()
        {
            Action<ToolStripMenuItem, PlayerStatTable> SetupButton = new Action<ToolStripMenuItem, PlayerStatTable>((ToolStripMenuItem menu, PlayerStatTable playerStatTable) => {
                menu.Text = playerStatTable.GetSeasonType();
                ToolStripItemCollection dropDownItems = menu.DropDownItems;
                dropDownItems.Clear();

                foreach (string seasonType in Constants.SeasonTypes)
                {
                    EventHandler selectSeasonTypeHandler = new EventHandler((object sender, EventArgs e) => {
                        playerStatTable.SetSeasonType(seasonType);
                        RefreshDropDownLists();
                        RedrawRowColors();
                        SetListIsSaved(false);
                    });
                    dropDownItems.Add(seasonType, null, selectSeasonTypeHandler);
                    if (playerStatTable.GetSeasonType() == seasonType)
                    {
                        ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                    }
                }
            });

            SetupButton(selectPrimarySeasonTypeDropDown, topTable);
            SetupButton(selectSecondarySeasonTypeDropDown, rightTable);
        }
        
        private void SetupSelectSeasonButton()
        {
            selectSeasonDropDown.Text = currentDisplaySeason;
            ToolStripItemCollection dropDownItems = selectSeasonDropDown.DropDownItems;
            dropDownItems.Clear();
            
            string[] years = Constants.CurrentSeason.Split('-');
            int startYear = Int32.Parse(years[0]);
            int endYear = Int32.Parse(years[1]);

            for (int i = 0; i < 15; i++)
            {
                string season = String.Format("{0}-{1}", startYear - i, endYear - i);
                EventHandler selectSeasonHandler = new EventHandler((object sender, EventArgs e) => {
                    topTable.ChangeDisplaySeason(season);
                    currentDisplaySeason = season;
                    RefreshDropDownLists();
                    SetListIsSaved(false);
                });
                dropDownItems.Add(season, null, selectSeasonHandler);
                if (currentDisplaySeason == season)
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupAddRemoveColumnButton()
        {
            ToolStripItemCollection dropDownItems = addRemoveColumnDropDown.DropDownItems;
            dropDownItems.Clear();
            foreach (string columnName in Constants.AllPossibleColumnsAlphebetized)
            {
                EventHandler selectColumnHandler = new EventHandler((object sender, EventArgs e) => {
                    ToolStripMenuItem dropDownItem = (ToolStripMenuItem)sender;
                    if (dropDownItem.Checked)
                    {
                        topTable.RemoveColumn(dropDownItem.Text);
                    }
                    else
                    {
                        topTable.AddColumn(dropDownItem.Text, currentDisplaySeason);
                    }
                    dropDownItem.Checked = !dropDownItem.Checked;
                    RedrawColumnWidths(topTableDGV, playerList.GetPrimaryColumnWidth);
                    SetListIsSaved(false);
                });
                dropDownItems.Add(columnName, null, selectColumnHandler);
                if (topTable.ContainsColumn(columnName))
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupSearchPlayerButton()
        {
            Action SearchPlayer = () => {
                string playerName = searchPlayerTextbox.Text;
                if (String.IsNullOrWhiteSpace(playerName) || playerName.Contains("'") || playerName.Contains("\""))
                {
                    MessageBox.Show("Invalid Search.");
                    searchPlayerTextbox.Text = "";
                }
                else
                {
                    string previousText = searchPlayerButton.Text;
                    searchPlayerButton.Text = "Searching...";
                    searchPlayerButton.Enabled = false;
                    clearSearchButton.Enabled = false;

                    bool successful = leftTable.DisplayPlayerSearch(playerName);
                    if (!successful)
                    {
                        MessageBox.Show("No Results Found.");
                    }

                    rowJustSelected = false;
                    searchPlayerButton.Text = previousText;
                    searchPlayerButton.Enabled = true;
                    clearSearchButton.Enabled = true;
                }
            };

            searchPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                SearchPlayer();
            });

            // Listen for Enter key when textbox is selected
            searchPlayerTextbox.KeyUp += new KeyEventHandler((object sender, KeyEventArgs e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    SearchPlayer();
                }
            });
        }

        private void SetupClearSearchButton()
        {
            clearSearchButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (leftTableDGV.SelectedRows.Count == 1)
                {
                    middleTable.ClearTable();
                    rightTable.ClearTable();
                    ClearPlayerSelection();
                }
                leftTable.ClearTable();
                searchPlayerTextbox.Text = String.Empty;
                clearSearchButton.Enabled = false;
            });
        }

        private void SetupAddSelectedPlayerButton()
        {
            // Enabling and disabling the button
            leftTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                addSelectedPlayerButton.Enabled = (leftTableDGV.SelectedRows.Count == 1);
            });

            // Adding logic to the button
            addSelectedPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (leftTableDGV.SelectedRows.Count != 1) { return; }

                string previousText = addSelectedPlayerButton.Text;
                addSelectedPlayerButton.Text = "Adding Player...";
                addSelectedPlayerButton.Enabled = false;

                int rowIndex = leftTableDGV.SelectedRows[0].Index;
                DataGridViewRow row = leftTableDGV.Rows[rowIndex];
                string playerId = leftTable.GetPlayerIdFromRow(row);
                topTable.AddRow(playerId);

                addSelectedPlayerButton.Text = previousText;
                SetListIsSaved(false);
            });
        }

        private void SetupRemoveSelectedPlayerButton()
        {
            // Enabling and disabling the button
            topTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                removeSelectedPlayerButton.Enabled = (topTableDGV.SelectedRows.Count == 1);
            });

            // Adding logic to the button
            removeSelectedPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (topTableDGV.SelectedRows.Count != 1) { return; }

                int rowIndex = topTableDGV.SelectedRows[0].Index;
                DataRow row = MultiPlayerStatTable.GetDataRowFromDGVRow(topTableDGV.Rows[rowIndex]);

                string message = String.Format("Are you sure you want to remove {0} {1} from the list?", row[Constants.FIRST_NAME], row[Constants.LAST_NAME]);
                Action RemovePlayer = () => {
                    topTable.RemoveRow(row);
                    ClearPlayerSelection();
                };
                DisplayYesNoMessageBox(message, RemovePlayer);

                SetListIsSaved(false);
            });
        }

        private void SetupShowSelectedPlayer()
        {
            Action<PlayerStats> ShowSelectedPlayer = new Action<PlayerStats>((PlayerStats playerStats) => {
                middleTable.ClearTable();
                middleTable.AddPlayerByPlayerStats(playerStats);

                rightTable.ClearTable();
                rightTable.AddPlayerByPlayerStats(playerStats);

                HighlightDraftRowsInThirdTable(playerStats);
                rowJustSelected = true;
            });

            topTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                if (topTableDGV.SelectedRows.Count != 1 || topTableDGV.Rows.Count < 1) { return; }
                leftTableDGV.ClearSelection();

                int rowIndex = topTableDGV.SelectedRows[0].Index;
                DataRow row = PlayerStatTable.GetDataRowFromDGVRow(topTableDGV.Rows[rowIndex]);
                PlayerStats existingPlayerStats = topTable.GetPlayerStatsFromRow(row);
                if (existingPlayerStats == null) { return; }

                ShowSelectedPlayer(existingPlayerStats);
            });

            leftTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                if (leftTableDGV.SelectedRows.Count != 1 || leftTableDGV.Rows.Count < 1) { return; }
                topTableDGV.ClearSelection();

                int rowIndex = leftTableDGV.SelectedRows[0].Index;
                DataGridViewRow row = leftTableDGV.Rows[rowIndex];
                string playerId = leftTable.GetPlayerIdFromRow(row);
                PlayerStats searchedPlayerStats = new PlayerStats(playerId);

                ShowSelectedPlayer(searchedPlayerStats);
            });
        }

        private void SetupUnselectPlayer()
        {
            Action<DataGridView, int, MouseButtons> UnselectPlayer = new Action<DataGridView, int, MouseButtons>((DataGridView dataGridView, int rowIndex, MouseButtons button) => {
                if (dataGridView.SelectedRows.Count != 1 || rowJustSelected)
                {
                    rowJustSelected = false;
                    return;
                }

                if (button == MouseButtons.Left && dataGridView.SelectedRows[0].Index == rowIndex)
                {
                    ClearPlayerSelection();
                    rowJustSelected = false;
                }
            });

            topTableDGV.CellMouseUp += new DataGridViewCellMouseEventHandler((object sender, DataGridViewCellMouseEventArgs e) =>
            {
                UnselectPlayer(topTableDGV, e.RowIndex, e.Button);
            });

            leftTableDGV.CellMouseUp += new DataGridViewCellMouseEventHandler((object sender, DataGridViewCellMouseEventArgs e) =>
            {
                UnselectPlayer(leftTableDGV, e.RowIndex, e.Button);
            });
        }

        private void SetupFormClosingHandler()
        {
            FormClosing += new FormClosingEventHandler((object sender, FormClosingEventArgs e) => {
                Action CancelLeave = () =>
                {
                    e.Cancel = true;
                };
                TriggerLeaveRequest(null, CancelLeave);
            });
        }

        private void SetupColumnResizeListener()
        {
            Action HandleResize = () =>
            {
                if (tableHasBeenClicked)
                {
                    SetListIsSaved(false);
                }
            };

            topTableDGV.ColumnWidthChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            {
                HandleResize();
            });

            rightTableDGV.ColumnWidthChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            {
                HandleResize();
            });
        }

        private void SetupTableClickListener()
        {
            MouseEventHandler eventHandler = new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                tableHasBeenClicked = true;
            });

            topTableDGV.MouseDown += eventHandler;
            leftTableDGV.MouseDown += eventHandler;
            middleTableDGV.MouseDown += eventHandler;
            rightTableDGV.MouseDown += eventHandler;
        }

        private void ClearPlayerSelection()
        {
            topTableDGV.ClearSelection();
            leftTableDGV.ClearSelection();
            middleTable.ClearTable();
            rightTable.ClearTable();
        }

        private void RefreshDropDownLists()
        {
            SetupLoadListDropDown();
            SetupSelectSeasonTypeButtons();
            SetupSelectSeasonButton();
            SetupAddRemoveColumnButton();
        }

        private void RedrawColumnWidths(DataGridView dataGridView, Func<string, int> GetWidthFunction)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                int width = GetWidthFunction(column.Name);
                if (column.DisplayIndex != dataGridView.Columns.Count - 1 && width >= 0)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    column.Width = width;
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        private void HighlightDraftRowsInThirdTable(PlayerStats playerStats)
        {
            foreach (DataGridViewRow DGVRow in rightTableDGV.Rows)
            {
                string season = DGVRow.Cells[Constants.SEASON].Value.ToString();
                string endYear = (season != String.Empty) ? season.Substring(5) : String.Empty;
                if (endYear == playerStats.GetDraftYear())
                {
                    DGVRow.DefaultCellStyle.BackColor = System.Drawing.Color.Khaki;
                }
                else if (endYear == playerStats.GetFirstYearOfDraftEligibility())
                {
                    DGVRow.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                }
            }
        }

        private void RedrawRowColors()
        {
            if (topTable.GetSeasonType() == Constants.REGULAR_SEASON)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (topTable.GetSeasonType() == Constants.PLAYOFFS)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
            }

            if (rightTable.GetSeasonType() == Constants.REGULAR_SEASON)
            {
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (rightTable.GetSeasonType() == Constants.PLAYOFFS)
            {
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
            }

            HighlightDraftRowsInThirdTable(rightTable.GetPlayerStats());
        }

        private void SetListIsSaved(bool boolean)
        {
            listIsSaved = boolean;
            listNameLabel.Text = (listIsSaved) ? playerList.listName : playerList.listName + "*";
        }

        private void DisplayYesNoMessageBox(string message, Action yesAction = null, Action noAction = null)
        {
            DialogResult confirmResult = MessageBox.Show(message, "", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes && yesAction != null)
            {
                yesAction();
            }
            else if (confirmResult == DialogResult.No && noAction != null)
            {
                noAction();
            }
        }

        private void TriggerLeaveRequest(Action leaveAction = null, Action stayAction = null)
        {
            if (!listIsSaved)
            {
                DisplayYesNoMessageBox(Constants.ARE_YOU_SURE_LEAVE_MESSAGE, leaveAction, stayAction);
            }
            else if (leaveAction != null)
            {
                leaveAction();
            }
        }

        private string TrimFileNameSuffix(string fullFileName)
        {
            if (fullFileName.EndsWith(Constants.FILENAME_SUFFIX)) { fullFileName = fullFileName.Remove(fullFileName.IndexOf(Constants.FILENAME_SUFFIX)); }

            while(fullFileName.EndsWith(Constants.FILENAME_SUFFIX_NO_XML))
            {
                fullFileName = fullFileName.Remove(fullFileName.IndexOf(Constants.FILENAME_SUFFIX_NO_XML));
            }

            return fullFileName;
        }
    }
}
