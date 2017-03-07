using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;

namespace HockeyStats
{
    public partial class PlayerStatForm : Form
    {
        private const string FILENAME_SUFFIX = ".playerList.xml";
        
        PlayerList playerList = new PlayerList();
        private MultiPlayerStatTable firstTable;
        private PlayerConstantStatTable secondTable;
        private SinglePlayerStatTable thirdTable;

        public PlayerStatForm()
        {
            InitializeComponent();

            PlayerList playerListToLoad = Serializer.ReadPlayerList<PlayerList>("bluesProspectsShort" + FILENAME_SUFFIX);
            LoadPlayerList(playerListToLoad);
            
            SetupLoadListDropDown();
            SetupSaveListButton();
            SetupCreateListButton();
            SetupAddPlayerButton();
            SetupShowSelectedPlayer();
        }

        private void LoadPlayerList(PlayerList playerListToLoad)
        {
            playerList = playerListToLoad;
            firstTable = new MultiPlayerStatTable(firstTableDGV, playerList);
            secondTable = new PlayerConstantStatTable(secondTableDGV);
            thirdTable = new SinglePlayerStatTable(thirdTableDGV, playerList.secondaryTableColumnNames);
        }

        private void SetupLoadListDropDown()
        {
            loadListDropDown.DropDownItems.Clear();
            string[] playerListFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + FILENAME_SUFFIX);
            foreach (string fileWithPath in playerListFiles)
            {
                // Get the name without the file path or suffix
                string file = Path.GetFileName(fileWithPath);
                string listName = file.Substring(0, file.Length - FILENAME_SUFFIX.Length);

                PlayerList playerListToLoad = Serializer.ReadPlayerList<PlayerList>(listName + FILENAME_SUFFIX);
                EventHandler selectHandler = new EventHandler((object sender, EventArgs e) => {
                    LoadPlayerList(playerListToLoad);
                    SetupLoadListDropDown();
                });
                string stringToWrite = (playerList.listName == listName) ? "*" + listName : listName;
                loadListDropDown.DropDownItems.Add(stringToWrite, null, selectHandler);
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
            });
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
                    playerList.playerIds.Add(playerId);
                    addPlayerTextbox.Text = String.Empty;
                }
            });
        }

        private void SetupShowSelectedPlayer()
        {
            firstTableDGV.CellDoubleClick += new DataGridViewCellEventHandler((object sender, DataGridViewCellEventArgs e) => {
                if (e.RowIndex < 0) { return; } // Ignore if a column was double clicked

                DataRow row = ((DataRowView)firstTableDGV.Rows[e.RowIndex].DataBoundItem).Row;
                string playerId = firstTable.GetIdByRow(row);
                Dictionary<string, string> existingPlayerDict = firstTable.GetSavedDictById(playerId);

                secondTable.ClearTable();
                secondTable.AddPlayerByDisplayDict(existingPlayerDict);

                thirdTable.ClearTable();
                thirdTable.AddPlayerByDisplayDict(existingPlayerDict);
            });
        }
    }
}
