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
        private MultiPlayerStatTable firstTable;
        private PlayerConstantStatTable secondTable;
        private SinglePlayerStatTable thirdTable;
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
            SetupAddPlayerButton();
            SetupRemoveSelectedPlayerButton();
            SetupShowSelectedPlayer();
        }

        private void LoadPlayerList(PlayerList playerListToLoad)
        {
            playerList = playerListToLoad;
            currentDisplaySeason = playerList.displaySeason;
            currentSeasonType = playerList.seasonType;
            listNameLabel.Text = playerList.listName;
            if (firstTable != null) { firstTable.AbortFillDataTableThread(); }

            firstTable = new MultiPlayerStatTable(firstTableDGV, playerList.primaryColumnNames, playerList.playerIds, playerList.displaySeason, playerList.seasonType);
            secondTable = new PlayerConstantStatTable(secondTableDGV);
            thirdTable = new SinglePlayerStatTable(thirdTableDGV, playerList.secondaryColumnNames);

            RedrawPrimaryColumnWidths();
            RedrawSecondaryColumnWidths();
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
                saveFileDialog.FileName = playerList.listName;
                DialogResult result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    string fileName = saveFileDialog.FileName;
                    string listName = Path.GetFileName(fileName).Substring(0, Path.GetFileName(fileName).Length - FILENAME_SUFFIX.Length);
                    playerList.SetListName(listName);
                    playerList.SetSeasonType(currentSeasonType);
                    playerList.SetDisplaySeason(currentDisplaySeason);
                    playerList.SetPlayerIds(firstTable.GetPlayerIds());
                    playerList.SetPrimaryColumns(firstTableDGV.Columns);
                    playerList.SetPrimaryColumnWidths(firstTableDGV.Columns);
                    playerList.SetSecondaryColumnWidths(thirdTableDGV.Columns);
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
                    firstTable.ChangeSeasonType(seasonType);
                    currentSeasonType = seasonType;
                    RefreshDropDownLists();
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
                    firstTable.ChangeDisplaySeason(season);
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
                        firstTable.RemoveColumn(dropDownItem.Text);
                    }
                    else
                    {
                        firstTable.AddColumn(dropDownItem.Text, currentDisplaySeason);
                    }
                    dropDownItem.Checked = !dropDownItem.Checked;
                    RedrawPrimaryColumnWidths();
                });
                dropDownItems.Add(columnName, null, selectColumnHandler);
                if (firstTable.ContainsColumn(columnName))
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupAddPlayerButton()
        {
            addPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                string playerId = addPlayerTextbox.Text;
                int junk;
                if (!playerId.Equals(String.Empty) && int.TryParse(playerId, out junk))
                {
                    addPlayerTextbox.Text = "Loading player...";
                    firstTable.AddPlayerById(playerId);
                    addPlayerTextbox.Text = String.Empty;
                }
            });
        }

        private void SetupRemoveSelectedPlayerButton()
        {
            // Enabling and disabling the button
            firstTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                removeSelectedPlayerButton.Enabled = (firstTableDGV.SelectedRows.Count == 1);
            });

            // Adding logic to the button
            removeSelectedPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (firstTableDGV.SelectedRows.Count != 1) { return; }

                int rowIndex = firstTableDGV.SelectedRows[0].Index;
                DataRow row = MultiPlayerStatTable.GetDataRowFromDGVRow(firstTableDGV.Rows[rowIndex]);
                firstTable.RemoveRow(row);

                ClearPlayerSelection();
            });
        }

        private void SetupShowSelectedPlayer()
        {
            firstTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                if (firstTableDGV.SelectedRows.Count != 1 || firstTableDGV.Rows.Count < 1) { return; }
                int rowIndex = firstTableDGV.SelectedRows[0].Index;

                DataRow row = MultiPlayerStatTable.GetDataRowFromDGVRow(firstTableDGV.Rows[rowIndex]);
                PlayerStats existingPlayerStats = firstTable.GetPlayerStatsFromRow(row);
                if (existingPlayerStats == null) { return; }

                secondTable.ClearTable();
                secondTable.AddPlayerByPlayerStats(existingPlayerStats);

                thirdTable.ClearTable();
                thirdTable.AddPlayerByPlayerStats(existingPlayerStats);

                // Change color of draft year in third table
                foreach (DataGridViewRow DGVRow in thirdTableDGV.Rows)
                {
                    string season = DGVRow.Cells[Constants.SEASON].Value.ToString();
                    string endYear = (season != String.Empty) ? season.Substring(5) : String.Empty;
                    if (endYear == existingPlayerStats.GetDraftYear())
                    {
                        DGVRow.DefaultCellStyle.BackColor = System.Drawing.Color.Khaki;
                    }
                    else if (endYear == existingPlayerStats.GetFirstYearOfDraftEligibility())
                    {
                        DGVRow.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                    }
                }
            });
        }

        private void ClearPlayerSelection()
        {
            firstTableDGV.ClearSelection();
            secondTable.ClearTable();
            thirdTable.ClearTable();
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
            foreach (DataGridViewColumn column in firstTableDGV.Columns)
            {
                int width = playerList.GetPrimaryColumnWidth(column.Name);
                if (column.DisplayIndex != firstTableDGV.Columns.Count - 1 && width >= 0)
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
            foreach (DataGridViewColumn column in thirdTableDGV.Columns)
            {
                int width = playerList.GetSecondaryColumnWidth(column.Name);
                if (column.DisplayIndex != thirdTableDGV.Columns.Count - 1 && width >= 0)
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
    }
}
