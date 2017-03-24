﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;
using System.Linq;

namespace HockeyStats
{
    public partial class PlayerStatForm : Form
    {
        private const string FILENAME_SUFFIX = ".playerList.xml";
        private string defaultPlayerList = "bluesProspectsShort";
        
        PlayerList playerList = new PlayerList();
        private MultiPlayerStatTable firstTable;
        private PlayerConstantStatTable secondTable;
        private SinglePlayerStatTable thirdTable;

        public PlayerStatForm()
        {
            InitializeComponent();

            PlayerList playerListToLoad = Serializer.ReadPlayerList<PlayerList>(defaultPlayerList + FILENAME_SUFFIX);
            LoadPlayerList(playerListToLoad);
            
            SetupLoadListDropDown();
            SetupSaveListButton();
            SetupCreateListButton();
            SetupSelectSeasonButton();
            SetupAddRemoveColumnButton();
            SetupAddPlayerButton();
            SetupRemoveSelectedPlayerButton();
            SetupShowSelectedPlayer();
        }

        private void LoadPlayerList(PlayerList playerListToLoad)
        {
            playerList = playerListToLoad;
            if (firstTable != null) { firstTable.AbortFillDataTableThread(); }
            firstTable = new MultiPlayerStatTable(firstTableDGV, playerList);
            secondTable = new PlayerConstantStatTable(secondTableDGV);
            thirdTable = new SinglePlayerStatTable(thirdTableDGV, playerList.secondaryColumnNames);
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
                    playerList.listName = Path.GetFileName(fileName).Substring(0, Path.GetFileName(fileName).Length - FILENAME_SUFFIX.Length);
                    playerList.SetPrimaryColumns(firstTableDGV.Columns);
                    Serializer.WritePlayerList<PlayerList>(playerList, fileName);
                    SetupLoadListDropDown();
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

        private void SetupSelectSeasonButton()
        {
            ToolStripItemCollection dropDownItems = selectSeasonDropDown.DropDownItems;
            dropDownItems.Clear();

            string currentSeason = GetCurrentSeason();
            string[] years = currentSeason.Split('-');
            int startYear = Int32.Parse(years[0]);
            int endYear = Int32.Parse(years[1]);

            for (int i = 0; i < 15; i++)
            {
                string season = String.Format("{0}-{1}", startYear - i, endYear - i);
                EventHandler selectSeasonHandler = new EventHandler((object sender, EventArgs e) => {
                    playerList.SetDisplaySeason(season);
                    LoadPlayerList(playerList);
                    RefreshDropDownLists();
                });
                dropDownItems.Add(season, null, selectSeasonHandler);
                if (playerList.displaySeason == season)
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupAddRemoveColumnButton()
        {
            ToolStripItemCollection dropDownItems = addRemoveColumnDropDown.DropDownItems;
            dropDownItems.Clear();
            foreach (string columnName in Columns.AllPossibleColumnsAlphebetized)
            {
                EventHandler selectColumnHandler = new EventHandler((object sender, EventArgs e) => {
                    ToolStripMenuItem dropDownItem = (ToolStripMenuItem)sender;
                    if (dropDownItem.Checked)
                    {
                        firstTable.RemoveColumn(dropDownItem.Text);
                        playerList.RemovePrimaryColumn(columnName);
                    }
                    else
                    {
                        firstTable.AddColumn(dropDownItem.Text);
                        playerList.AddPrimaryColumn(columnName);
                    }
                    dropDownItem.Checked = !dropDownItem.Checked;
                });
                dropDownItems.Add(columnName, null, selectColumnHandler);
                if (playerList.primaryColumnNames.Contains(columnName))
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
                    playerList.AddPlayer(playerId);
                    addPlayerTextbox.Text = String.Empty;
                }
            });
        }

        private void SetupRemoveSelectedPlayerButton()
        {
            // Enabling and disabling the button
            firstTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                if (firstTableDGV.SelectedRows.Count == 1)
                {
                    removeSelectedPlayerButton.Enabled = true;
                }
                else
                {
                    removeSelectedPlayerButton.Enabled = false;
                }
            });

            // Adding logic to the button
            removeSelectedPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (firstTableDGV.SelectedRows.Count != 1) { return; }

                int rowIndex = firstTableDGV.SelectedRows[0].Index;
                DataRow row = MultiPlayerStatTable.GetDataRowFromDGVRow(firstTableDGV.Rows[rowIndex]);
                firstTable.RemoveRow(row);

                PlayerStats playerStats = firstTable.GetPlayerStatsFromRow(row);
                playerList.RemovePlayer(playerStats.GetPlayerId());

                ClearPlayerSelection();
            });
        }

        private void SetupShowSelectedPlayer()
        {
            firstTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                if (firstTableDGV.SelectedRows.Count != 1 || playerList.playerIds.Count < 1) { return; }
                int rowIndex = firstTableDGV.SelectedRows[0].Index;

                DataRow row = MultiPlayerStatTable.GetDataRowFromDGVRow(firstTableDGV.Rows[rowIndex]);
                PlayerStats existingPlayerStats = firstTable.GetPlayerStatsFromRow(row);
                if (existingPlayerStats == null) { return; }

                secondTable.ClearTable();
                secondTable.AddPlayerByPlayerStats(existingPlayerStats);

                thirdTable.ClearTable();
                thirdTable.AddPlayerByPlayerStats(existingPlayerStats);
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
            SetupSelectSeasonButton();
            SetupAddRemoveColumnButton();
        }

        public static string GetCurrentSeason()
        {
            DateTime today = DateTime.Today;
            int seasonStart, seasonEnd;
            if (today.Month < 6)
            {
                seasonStart = today.Year - 1;
                seasonEnd = today.Year;
            }
            else
            {
                seasonStart = today.Year;
                seasonEnd = today.Year + 1;
            }
            return String.Format("{0}-{1}", seasonStart, seasonEnd);
        }
    }
}
