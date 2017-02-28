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

            LoadPlayerList("bluesProspectsShort");
            
            SetupLoadListDropDown();
            SetupSaveListButton();
            SetupCreateListButton();
            SetupAddPlayerButton();
            SetupShowSelectedPlayer();
        }

        private void LoadPlayerList(string listToLoad)
        {
            playerList = Serializer.DeserializeObject<PlayerList>(listToLoad + FILENAME_SUFFIX);
            firstTable = new MultiPlayerStatTable(firstTableDGV, playerList.primaryTableColumnNames, playerList.displayYears, playerList.playerIds);
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
                
                EventHandler selectHandler = new EventHandler((object sender, EventArgs e) => LoadPlayerList(listName));
                loadListDropDown.DropDownItems.Add(listName, null, selectHandler);
            }
        }

        private void SetupSaveListButton()
        {
            saveFileDialog.Filter = "PlayerList|*.playerList.xml";
            saveFileDialog.Title = "Save Player List";
            saveListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                saveFileDialog.FileName = playerList.listName;
                DialogResult result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    Serializer.SerializeObject<PlayerList>(playerList, playerList.listName + FILENAME_SUFFIX);
                }
            });
        }

        private void SetupCreateListButton()
        {
            createListButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (!createListTextbox.Text.Equals(String.Empty))
                {
                    string fileName = createListTextbox.Text + FILENAME_SUFFIX;
                    playerList = new PlayerList();
                    playerList.FillWithDefaults();
                    Serializer.SerializeObject(playerList, fileName);

                    LoadPlayerList(createListTextbox.Text);
                    SetupLoadListDropDown();

                    createListTextbox.Text = String.Empty;
                    createListTextbox.Visible = false;
                    createListButton.Visible = false;
                }
            });

            createListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) => {
                createListTextbox.Visible = true;
                createListButton.Visible = true;
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
                string playerId = firstTableDGV.Rows[e.RowIndex].Cells[firstTableDGV.Columns["ID"].Index].Value.ToString();
                Dictionary<string, string> existingPlayerDict = firstTable.GetDisplayDictById(playerId);

                secondTable.ClearTable();
                secondTable.AddPlayerByDisplayDict(existingPlayerDict);

                thirdTable.ClearTable();
                thirdTable.AddPlayerByDisplayDict(existingPlayerDict);
            });
        }
    }
}
