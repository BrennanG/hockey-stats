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
        private string displayYear = "2016-2017";
        private List<string> playerIds = new List<string>
        {
            // Forwards
            "301349", // Tage Thompson
            "269034", // Jordan Kyrou
            "201245", // Tanner Kaspick
            "146116", // Nolan Stevens
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
        private DataTable dataTable = new DataTable();
        private List<string> columnData = new List<string>
        {
            "Last Name", "Games Played", "Goals", "Assists", "Total Points", "PPG", "League", "Draft Year", "Draft Round", "Draft Overall", "Draft Team"
        };
        private List<Dictionary<string, string>> rowData = new List<Dictionary<string, string>>();

        public PlayerStatForm()
        {
            InitializeComponent();
            CreateTable();
            new Thread(() => FillTable()).Start(); // Fill the table in a separate thread
            CreateAddPlayerButton();
        }

        private void CreateTable()
        {
            foreach (string columnName in columnData)
            {
                dataTable.Columns.Add(new DataColumn(columnName));
            }
            dataGridView1.DataSource = dataTable;
        }

        private void CreateAddPlayerButton()
        {
            addPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                string playerId = playerIdTextbox.Text;
                int junk;
                if (!playerId.Equals(String.Empty) && int.TryParse(playerId, out junk))
                {
                    AddPlayer(playerId);
                    playerIdTextbox.Text = String.Empty;
                }
            });
        }

        private void FillTable()
        {
            // Get Elite Prospects data for each player
            foreach (string playerId in playerIds)
            {
                Dictionary<string, string> playerDict = new Dictionary<string, string>();
                AddPlayerStatsToDict(playerDict, playerId);
                AddDraftDataToDict(playerDict, playerId);
                AddRowToDataTable(playerDict);
            }
        }

        private void AddPlayerStatsToDict(Dictionary<string, string> playerDict, string playerId)
        {
            JObject statsJson = EliteProspectsAPI.GetPlayerStats(playerId);
            foreach (JToken statLine in statsJson["data"])
            {
                StatLineParser stats = new StatLineParser(statLine);
                if (stats.GetGameType() == "REGULAR_SEASON" && stats.GetSeasonName() == displayYear)
                {
                    if (playerDict.Count == 0)
                    {
                        playerDict["First Name"] = stats.GetFirstName();
                        playerDict["Last Name"] = stats.GetLastName();
                        playerDict["Games Played"] = stats.GetGamesPlayed();
                        playerDict["Goals"] = stats.GetGoals();
                        playerDict["Assists"] = stats.GetAssists();
                        playerDict["Total Points"] = stats.GetTotalPoints();
                        playerDict["PPG"] = stats.GetPointsPerGame();
                        playerDict["League"] = stats.GetLeagueName();
                        playerDict["Team"] = stats.GetTeamName();
                    }
                    else
                    {
                        playerDict["Games Played"] += Environment.NewLine + stats.GetGamesPlayed();
                        playerDict["Goals"] += Environment.NewLine + stats.GetGoals();
                        playerDict["Assists"] += Environment.NewLine + stats.GetAssists();
                        playerDict["Total Points"] += Environment.NewLine + stats.GetTotalPoints();
                        playerDict["PPG"] += Environment.NewLine + stats.GetPointsPerGame();
                        playerDict["League"] += Environment.NewLine + stats.GetLeagueName();
                        playerDict["Team"] += Environment.NewLine + stats.GetTeamName();
                    }
                }
            }
        }

        private void AddDraftDataToDict(Dictionary<string, string> playerDict, string playerId)
        {
            JObject draftJson = EliteProspectsAPI.GetPlayerDraftData(playerId);
            JToken data = draftJson["data"];
            if (data != null)
            {
                DraftDataParser draftData = new DraftDataParser(data.First);
                playerDict["Draft Year"] = draftData.GetYear();
                playerDict["Draft Round"] = draftData.GetRound();
                playerDict["Draft Overall"] = draftData.GetOverall();
                playerDict["Draft Team"] = draftData.GetTeamName();
            }
        }

        private void AddRowToDataTable(Dictionary<string, string> rowDict)
        {
            string[] orderedRowValues = new string[columnData.Count];
            for (int i = 0; i < columnData.Count; i++)
            {
                string outString = String.Empty;
                // If the column exists in the row, add the value
                if (rowDict.TryGetValue(columnData[i], out outString))
                {
                    orderedRowValues[i] = rowDict[columnData[i]];
                }
            }
            dataTable.Rows.Add(orderedRowValues);
        }

        private void AddPlayer(string playerId)
        {
            Dictionary<string, string> playerDict = new Dictionary<string, string>();
            AddPlayerStatsToDict(playerDict, playerId);
            AddDraftDataToDict(playerDict, playerId);
            AddRowToDataTable(playerDict);
        }

    }
}
