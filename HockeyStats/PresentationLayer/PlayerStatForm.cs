using System;
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
        private const string FILENAME_SUFFIX = ".playerList.xml";
        private const string defaultPlayerList = "bluesProspectsShort";
        
        PlayerList playerList = new PlayerList();
        private MultiPlayerStatTable topTable;
        private SearchDataStatTable leftTable;
        private PlayerConstantsStatTable middleTable;
        private SinglePlayerStatTable rightTable;
        private string currentDisplaySeason;
        private string currentSeasonType;

        public PlayerStatForm()
        {
            InitializeComponent();

            PlayerList playerListToLoad = Serializer.ReadPlayerList<PlayerList>(defaultPlayerList + FILENAME_SUFFIX);
            LoadPlayerList(playerListToLoad);
            
            SetupLoadListDropDown();
            SetupSaveListButton();
            SetupCreateListButton();
            SetupSelectSeasonTypeButton();
            SetupSelectSeasonButton();
            SetupAddRemoveColumnButton();
            SetupSearchPlayerButton();
            SetupClearSearchButton();
            SetupAddSelectedPlayerButton();
            SetupRemoveSelectedPlayerButton();
            SetupShowSelectedPlayer();
        }

        private void LoadPlayerList(PlayerList playerListToLoad)
        {
            playerList = playerListToLoad;
            currentDisplaySeason = playerList.displaySeason;
            currentSeasonType = playerList.seasonType;
            listNameLabel.Text = playerList.listName;
            if (topTable != null) { topTable.AbortFillDataTableThread(); }

            topTable = new MultiPlayerStatTable(topTableDGV, playerList.primaryColumnNames, playerList.playerIds, playerList.displaySeason, playerList.seasonType);
            if (leftTable == null) { leftTable = new SearchDataStatTable(leftTableDGV, Constants.DefaultSearchDataTableColumns); }
            middleTable = new PlayerConstantsStatTable(middleTableDGV);
            rightTable = new SinglePlayerStatTable(rightTableDGV, playerList.secondaryColumnNames);

            RedrawPrimaryColumnWidths();
            RedrawSecondaryColumnWidths();
            RedrawRowColors();
        }

        private void SetupLoadListDropDown()
        {
            ToolStripItemCollection dropDownItems = loadListDropDown.DropDownItems;
            dropDownItems.Clear();
            string[] playerListFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + FILENAME_SUFFIX);
            foreach (string fileWithPath in playerListFiles)
            {
                // Get the name without the file path or suffix
                string file = Path.GetFileName(fileWithPath);
                string listName = file.Substring(0, file.Length - FILENAME_SUFFIX.Length);

                PlayerList playerListToLoad = Serializer.ReadPlayerList<PlayerList>(listName + FILENAME_SUFFIX);
                EventHandler selectPlayerListHandler = new EventHandler((object sender, EventArgs e) => {
                    LoadPlayerList(playerListToLoad);
                    RefreshDropDownLists();
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
            saveFileDialog.Filter = "Player List XML|*" + FILENAME_SUFFIX;
            saveFileDialog.Title = "Save Player List";
            saveListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                if (topTable.ThreadIsRunning())
                {
                    MessageBox.Show("You must wait until all players are loaded before saving.");
                    return;
                }
                saveFileDialog.FileName = playerList.listName;
                DialogResult result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    string fileName = saveFileDialog.FileName;
                    string listName = Path.GetFileName(fileName).Substring(0, Path.GetFileName(fileName).Length - FILENAME_SUFFIX.Length);
                    playerList.SetListName(listName);
                    playerList.SetSeasonType(currentSeasonType);
                    playerList.SetDisplaySeason(currentDisplaySeason);
                    playerList.SetPlayerIds(topTable.GetPlayerIds());
                    playerList.SetPrimaryColumns(topTableDGV.Columns);
                    playerList.SetPrimaryColumnWidths(topTableDGV.Columns);
                    playerList.SetSecondaryColumnWidths(rightTableDGV.Columns);
                    Serializer.WritePlayerList<PlayerList>(playerList, fileName);
                    RefreshDropDownLists();
                }
            });
        }

        private void SetupCreateListButton()
        {
            createListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) => {
                PlayerList playerListToLoad = new PlayerList();
                playerListToLoad.FillWithDefaults();
                LoadPlayerList(playerListToLoad);
                RefreshDropDownLists();
            });
        }

        private void SetupSelectSeasonTypeButton()
        {
            selectSeasonTypeDropDown.Text = currentSeasonType;
            ToolStripItemCollection dropDownItems = selectSeasonTypeDropDown.DropDownItems;
            dropDownItems.Clear();

            foreach (string seasonType in Constants.SeasonTypes)
            {
                EventHandler selectSeasonTypeHandler = new EventHandler((object sender, EventArgs e) => {
                    topTable.ChangeSeasonType(seasonType);
                    rightTable.ChangeSeasonType(seasonType);
                    currentSeasonType = seasonType;
                    RefreshDropDownLists();
                    RedrawRowColors();
                });
                dropDownItems.Add(seasonType, null, selectSeasonTypeHandler);
                if (currentSeasonType == seasonType)
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }
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
                    RedrawPrimaryColumnWidths();
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

                if (topTable.ThreadIsRunning())
                {
                    MessageBox.Show("You must wait until all players are loaded before adding another.");
                }
                else
                {
                    int rowIndex = leftTableDGV.SelectedRows[0].Index;
                    DataGridViewRow row = leftTableDGV.Rows[rowIndex];
                    string playerId = leftTable.GetPlayerIdFromRow(row);
                    topTable.AddPlayerById(playerId);
                }
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
                var confirmResult = MessageBox.Show(message, "", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.Yes)
                {
                    topTable.RemoveRow(row);
                    ClearPlayerSelection();
                }
            });
        }

        private void SetupShowSelectedPlayer()
        {
            topTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                if (topTableDGV.SelectedRows.Count != 1 || topTableDGV.Rows.Count < 1) { return; }
                leftTableDGV.ClearSelection();
                int rowIndex = topTableDGV.SelectedRows[0].Index;

                DataRow row = PlayerStatTable.GetDataRowFromDGVRow(topTableDGV.Rows[rowIndex]);
                PlayerStats existingPlayerStats = topTable.GetPlayerStatsFromRow(row);
                if (existingPlayerStats == null) { return; }

                middleTable.ClearTable();
                middleTable.AddPlayerByPlayerStats(existingPlayerStats);

                rightTable.ClearTable();
                rightTable.AddPlayerByPlayerStats(existingPlayerStats, currentSeasonType);

                HighlightDraftRowsInThirdTable(existingPlayerStats);
            });

            leftTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                if (leftTableDGV.SelectedRows.Count != 1 || leftTableDGV.Rows.Count < 1) { return; }
                topTableDGV.ClearSelection();

                int rowIndex = leftTableDGV.SelectedRows[0].Index;
                DataGridViewRow row = leftTableDGV.Rows[rowIndex];
                string playerId = leftTable.GetPlayerIdFromRow(row);
                PlayerStats searchedPlayerStats = new PlayerStats(playerId);

                middleTable.ClearTable();
                middleTable.AddPlayerByPlayerStats(searchedPlayerStats);

                rightTable.ClearTable();
                rightTable.AddPlayerByPlayerStats(searchedPlayerStats, currentSeasonType);

                HighlightDraftRowsInThirdTable(searchedPlayerStats);
            });
        }

        private void ClearPlayerSelection()
        {
            topTableDGV.ClearSelection();
            middleTable.ClearTable();
            rightTable.ClearTable();
        }

        private void RefreshDropDownLists()
        {
            SetupLoadListDropDown();
            SetupSelectSeasonTypeButton();
            SetupSelectSeasonButton();
            SetupAddRemoveColumnButton();
        }

        private void RedrawPrimaryColumnWidths()
        {
            foreach (DataGridViewColumn column in topTableDGV.Columns)
            {
                int width = playerList.GetPrimaryColumnWidth(column.Name);
                if (column.DisplayIndex != topTableDGV.Columns.Count - 1 && width >= 0)
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

        private void RedrawSecondaryColumnWidths()
        {
            foreach (DataGridViewColumn column in rightTableDGV.Columns)
            {
                int width = playerList.GetSecondaryColumnWidth(column.Name);
                if (column.DisplayIndex != rightTableDGV.Columns.Count - 1 && width >= 0)
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
            if (currentSeasonType == Constants.REGULAR_SEASON)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (currentSeasonType == Constants.PLAYOFFS)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
            }

            HighlightDraftRowsInThirdTable(rightTable.GetPlayerStats());
        }

        private void SearchPlayer()
        {
            string playerName = searchPlayerTextbox.Text;
            if (String.IsNullOrWhiteSpace(playerName))
            {
                MessageBox.Show("Invalid Search.");
                searchPlayerTextbox.Text = "";
            }
            else
            {
                string previousText = searchPlayerButton.Text;
                searchPlayerButton.Text = "Searching...";
                searchPlayerButton.Enabled = false;
                bool successful = leftTable.DisplayPlayerSearch(playerName);
                if (!successful)
                {
                    MessageBox.Show("No Results Found.");
                }
                searchPlayerButton.Text = previousText;
                searchPlayerButton.Enabled = true;
            }
        }
    }
}
