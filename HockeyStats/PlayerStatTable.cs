using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HockeyStats
{
    public class PlayerStatTable
    {
        DataTable dataTable;
        private DataGridView dataGridView;
        private List<string> playerIds;
        private List<string> columnData;
        private List<string> displayYears;

        public PlayerStatTable(DataGridView dataGridView, List<string> playerIds, List<string> columnData, List<string> displayYears)
        {
            this.dataGridView = dataGridView;
            this.playerIds = playerIds;
            this.columnData = columnData;
            this.displayYears = displayYears;

            dataTable = CreateDataTable(this.dataGridView);
            new Thread(() => FillDataTable()).Start(); // Fill the main table in a separate thread
        }

        public void AddPlayerToDataTable(string playerId)
        {
            Dictionary<string, string> playerDict = new Dictionary<string, string>();
            AddPlayerStatsToDict(playerDict, playerId);
            AddDraftDataToDict(playerDict, playerId);
            AddRowToDataTable(playerDict);
        }

        private DataTable CreateDataTable(DataGridView dgv)
        {
            DataTable dataTable = new DataTable();
            foreach (string columnName in columnData)
            {
                dataTable.Columns.Add(new DataColumn(columnName));
            }
            dgv.DataSource = dataTable;
            return dataTable;
        }

        private void FillDataTable()
        {
            // Get Elite Prospects data for each player
            foreach (string playerId in playerIds)
            {
                AddPlayerToDataTable(playerId);
            }
        }

        private void AddPlayerStatsToDict(Dictionary<string, string> playerDict, string playerId)
        {
            JObject statsJson = EliteProspectsAPI.GetPlayerStats(playerId);
            foreach (JToken statLine in statsJson["data"])
            {
                StatLineParser stats = new StatLineParser(statLine);
                if (stats.GetGameType() == "REGULAR_SEASON" && (displayYears == null || displayYears.Count == 0 || displayYears.Contains(stats.GetSeasonName())))
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
    }
}
