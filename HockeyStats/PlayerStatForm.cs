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
        //private List<string> firstTableDisplayYears = new List<string> { "2016-2017" };
        //private List<string> firstTablePlayerIds = new List<string>
        //{
        //    // Forwards
        //    "301349", // Tage Thompson
        //    "269034", // Jordan Kyrou
        //    //"201245", // Tanner Kaspick
        //    //"146116", // Nolan Stevens
        //    //"84020", // Connor Bleackley
        //    //"207452", // Nikolaj Krag Christensen
        //    //"167520", // Filip Helt
        //    //"189422", // Adam Musil
        //    //"192245", // Glenn Gawdin
        //    //"234450", // Liam Dunda
        //    //"156923", // Robby Fabbri
        //    //"108659", // Ivan Barbashev
        //    //"231724", // Austin Poganski
        //    //"213830", // Samuel Blais
        //    //"191247", // Dwyer Tschantz
        //    //"183505", // Mackenzie MacEachern
        //    //"26385", // Ty Rattie
        //    //"65564", // Dmitrij Jaskin
        //    //"90315", // Justin Selman
        //    //"233028", // Maxim Letunov
        //    //// Defenders
        //    //"161816", // Vince Dunn
        //    //"94439", // Niko Mikkola
        //    //"248066", // Jake Walman
        //    //"196704", // Thomas Vannelli
        //    //"60924", // Santeri Saari
        //    //"50291", // Jordan Schmaltz
        //    //"89411", // Colton Parayko
        //    //"45347", // Petteri Lindbohm
        //    //"59478", // Joel Edmundson
        //    //"168690", // Dmitri Sergeyev
        //    //"45342", // Jani Hakanpaa
        //    //"34836", // Konrad Abeltshauser
        //};
        //private List<string> firstTableColumnData = new List<string>
        //{
        //    "Last Name", "Games Played", "Goals", "Assists", "Total Points", "PPG", "League", "Draft Year", "Draft Round", "Draft Overall", "Draft Team"
        //};

        //private List<string> thirdTableColumnData = new List<string>
        //{
        //    "Year", "Games Played", "Goals", "Assists", "Total Points", "PPG", "League"
        //};

        private const string FILENAME_SUFFIX = ".playerList.xml";

        private MultiPlayerStatTable firstTable;
        private PlayerConstantStatTable secondTable;
        private SinglePlayerStatTable thirdTable;

        public PlayerStatForm()
        {
            InitializeComponent();

            LoadPlayerList("bluesProspectsShort");

            //PlayerList playerList = new PlayerList();
            //playerList.displayYears = firstTableDisplayYears;
            //playerList.playerIds = firstTablePlayerIds;
            //playerList.primaryTableColumnNames = firstTableColumnData;
            //playerList.secondaryTableColumnNames = thirdTableColumnData;
            //Serializer.SerializeObject<PlayerList>(playerList, "bluesProspects.playerList.xml");
            FillSelectListDropDown();
            SetupCreateListButton();
            SetupAddPlayerButton();
            SetupShowSelectedPlayer();
        }

        private void LoadPlayerList(string fileName)
        {
            PlayerList playerList = Serializer.DeserializeObject<PlayerList>(fileName + FILENAME_SUFFIX);
            firstTable = new MultiPlayerStatTable(firstTableDGV, playerList.primaryTableColumnNames, playerList.displayYears, playerList.playerIds);
            secondTable = new PlayerConstantStatTable(secondTableDGV);
            thirdTable = new SinglePlayerStatTable(thirdTableDGV, playerList.secondaryTableColumnNames);
        }

        private void FillSelectListDropDown()
        {
            selectListDropDown.DropDownItems.Clear();
            string[] playerListFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + FILENAME_SUFFIX);
            foreach (string fileWithPath in playerListFiles)
            {
                // Get the name without the file path or suffix
                string file = Path.GetFileName(fileWithPath);
                string listName = file.Substring(0, file.Length - FILENAME_SUFFIX.Length);
                
                EventHandler selectHandler = new EventHandler((object sender, EventArgs e) => LoadPlayerList(listName));
                selectListDropDown.DropDownItems.Add(listName, null, selectHandler);
            }
        }

        private void OpenSaveDialog()
        {
            saveFileDialog.Filter = "PlayerList|*.playerList.xml";
            saveFileDialog.Title = "Save Player List";
            saveFileDialog.ShowDialog();

            //if (saveFileDialog.FileName != "")
            //{
            //    // Saves the Image via a FileStream created by the OpenFile method.  
            //    FileStream fs = (FileStream)saveFileDialog.OpenFile();
            //    // Saves the Image in the appropriate ImageFormat based upon the  
            //    // File type selected in the dialog box.  
            //    // NOTE that the FilterIndex property is one-based.  
            //    switch (saveFileDialog.FilterIndex)
            //    {
            //        case 1:
            //            this.button2.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
            //            break;

            //        case 2:
            //            this.button2.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
            //            break;

            //        case 3:
            //            this.button2.Image.Save(fs, System.Drawing.Imaging.ImageFormat.Gif);
            //            break;
            //    }

            //    fs.Close();
            //}
        }

        private void SetupCreateListButton()
        {
            createListButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (!createListTextbox.Text.Equals(String.Empty))
                {
                    string fileName = createListTextbox.Text + FILENAME_SUFFIX;
                    PlayerList playerList = new PlayerList();
                    playerList.FillWithDefaults();
                    Serializer.SerializeObject(playerList, fileName);

                    LoadPlayerList(createListTextbox.Text);
                    FillSelectListDropDown();

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
