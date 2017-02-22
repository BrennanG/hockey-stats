using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace HockeyStats
{
    public partial class PlayerStatForm : Form
    {
        private List<string> mainTableDisplayYears = new List<string> { "2016-2017" };
        private List<string> mainTablePlayerIds = new List<string>
        {
            // Forwards
            "301349", // Tage Thompson
            "269034", // Jordan Kyrou
            //"201245", // Tanner Kaspick
            //"146116", // Nolan Stevens
            //"84020", // Connor Bleackley
            //"207452", // Nikolaj Krag Christensen
            //"167520", // Filip Helt
            //"189422", // Adam Musil
            //"192245", // Glenn Gawdin
            //"234450", // Liam Dunda
            //"156923", // Robby Fabbri
            //"108659", // Ivan Barbashev
            //"231724", // Austin Poganski
            //"213830", // Samuel Blais
            //"191247", // Dwyer Tschantz
            //"183505", // Mackenzie MacEachern
            //"26385", // Ty Rattie
            //"65564", // Dmitrij Jaskin
            //"90315", // Justin Selman
            //"233028", // Maxim Letunov
            //// Defenders
            //"161816", // Vince Dunn
            //"94439", // Niko Mikkola
            //"248066", // Jake Walman
            //"196704", // Thomas Vannelli
            //"60924", // Santeri Saari
            //"50291", // Jordan Schmaltz
            //"89411", // Colton Parayko
            //"45347", // Petteri Lindbohm
            //"59478", // Joel Edmundson
            //"168690", // Dmitri Sergeyev
            //"45342", // Jani Hakanpaa
            //"34836", // Konrad Abeltshauser
        };
        private List<string> mainTableColumnData = new List<string>
        {
            "Last Name", "Games Played", "Goals", "Assists", "Total Points", "PPG", "League", "Draft Year", "Draft Round", "Draft Overall", "Draft Team"
        };

        private List<string> secondaryTableDisplayYears = null;
        private List<string> secondaryTablePlayerIds = new List<string>{ };
        private List<string> secondaryTableColumnData = new List<string>
        {
            "First Name", "Last Name", "Games Played", "Goals", "Assists", "Total Points", "PPG", "League", "Draft Year", "Draft Round", "Draft Overall", "Draft Team"
        };

        private PlayerStatTable mainTable;
        private PlayerStatTable secondaryTable;

        public PlayerStatForm()
        {
            InitializeComponent();
            mainTable = new PlayerStatTable(mainTableDGV, mainTablePlayerIds, mainTableColumnData, mainTableDisplayYears);
            secondaryTable = new PlayerStatTable(secondaryTableDGV, secondaryTablePlayerIds, secondaryTableColumnData, secondaryTableDisplayYears);
            CreateAddPlayerButton(mainTable);
            
            mainTableDGV.CellDoubleClick += new DataGridViewCellEventHandler((object sender, DataGridViewCellEventArgs e) => ShowPlayer(sender, e));
        }

        private void CreateAddPlayerButton(PlayerStatTable playerStatTable)
        {
            addPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                string playerId = playerIdTextbox.Text;
                int junk;
                if (!playerId.Equals(String.Empty) && int.TryParse(playerId, out junk))
                {
                    playerIdTextbox.Text = "Loading player...";
                    playerStatTable.AddPlayerById(playerId);
                    playerIdTextbox.Text = String.Empty;
                }
            });
        }

        private void ShowPlayer(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            string playerId = dgv.Rows[e.RowIndex].Cells[dgv.Columns["ID"].Index].Value.ToString();
            Dictionary<string, string> existingPlayerDict = mainTable.GetDisplayDictById(playerId);
            secondaryTable.ClearPlayersFromTable();
            secondaryTable.AddPlayerByDisplayDict(existingPlayerDict);
        }
    }
}
