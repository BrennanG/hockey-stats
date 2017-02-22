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
        private DataTable dataTable;
        private DataGridView dataGridView;
        private List<Dictionary<string, string>> savedDicts = new List<Dictionary<string, string>>();
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

        public void AddPlayerById(string playerId)
        {
            Dictionary<string, string> DisplayDict = new Dictionary<string, string>();
            Dictionary<string, string> SavedDict = new Dictionary<string, string>();
            AddPlayerStatsToDict(DisplayDict, SavedDict, playerId);
            AddDraftDataToDict(DisplayDict, SavedDict, playerId);
            AddRowToDataTable(DisplayDict);
        }

        public void AddPlayerByDisplayDict(Dictionary<string, string> DisplayDict)
        {
            AddRowToDataTable(DisplayDict);
            savedDicts.Add(DisplayDict);
            playerIds.Add(DisplayDict["ID"]);
        }

        public void ClearPlayersFromTable()
        {
            dataTable.Clear();
            savedDicts.Clear();
            playerIds.Clear();
        }

        public Dictionary<string, string> GetDisplayDictById(string playerId)
        {
            return savedDicts.Find((Dictionary<string, string> dict) => dict["ID"] == playerId);
        }

        private DataTable CreateDataTable(DataGridView dgv)
        {
            DataTable dataTable = new DataTable();
            foreach (string columnName in columnData)
            {
                dataTable.Columns.Add(new DataColumn(columnName));
            }

            DataColumn primaryKeyColumn = dataTable.Columns.Add("ID");
            //dataTable.PrimaryKey = new DataColumn[] { primaryKeyColumn };

            dgv.DataSource = dataTable;
            return dataTable;
        }

        private void FillDataTable()
        {
            foreach (string playerId in playerIds)
            {
                AddPlayerById(playerId);
            }
        }

        private void AddPlayerStatsToDict(Dictionary<string, string> displayDict, Dictionary<string, string> savedDict, string playerId)
        {
            JObject statsJson = EliteProspectsAPI.GetPlayerStats(playerId);
            foreach (JToken statLine in statsJson["data"])
            {
                StatLineParser stats = new StatLineParser(statLine);
                if (stats.GetGameType() == "REGULAR_SEASON")
                {
                    if (displayYears == null || displayYears.Count == 0 || displayYears.Contains(stats.GetSeasonName()))
                    {
                        FillDictWithStats(displayDict, playerId, stats);
                    }
                    savedDicts.Add(FillDictWithStats(savedDict, playerId, stats));
                }
                
            }
        }

        private void AddDraftDataToDict(Dictionary<string, string> displayDict, Dictionary<string, string> savedDict, string playerId)
        {
            JObject draftJson = EliteProspectsAPI.GetPlayerDraftData(playerId);
            JToken data = draftJson["data"];
            if (data != null)
            {
                DraftDataParser draftData = new DraftDataParser(data.First);
                FillDictWithDraftData(displayDict, draftData);
                FillDictWithDraftData(savedDict, draftData);
            }
        }

        private void AddRowToDataTable(Dictionary<string, string> displayDict)
        {
            string[] orderedRowValues = new string[dataTable.Columns.Count];
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                string junk = String.Empty;
                // If the column exists in the row, add the value
                if (displayDict.TryGetValue(dataTable.Columns[i].ColumnName, out junk))
                {
                    orderedRowValues[i] = displayDict[dataTable.Columns[i].ColumnName];
                }
            }
            dataTable.Rows.Add(orderedRowValues);
        }

        private Dictionary<string, string> FillDictWithStats(Dictionary<string, string> dict, string playerId, StatLineParser stats)
        {
            if (dict.Count == 0)
            {
                dict["ID"] = playerId;
                dict["First Name"] = stats.GetFirstName();
                dict["Last Name"] = stats.GetLastName();
                dict["Games Played"] = stats.GetGamesPlayed();
                dict["Goals"] = stats.GetGoals();
                dict["Assists"] = stats.GetAssists();
                dict["Total Points"] = stats.GetTotalPoints();
                dict["PPG"] = stats.GetPointsPerGame();
                dict["League"] = stats.GetLeagueName();
                dict["Team"] = stats.GetTeamName();
            }
            else
            {
                dict["Games Played"] += Environment.NewLine + stats.GetGamesPlayed();
                dict["Goals"] += Environment.NewLine + stats.GetGoals();
                dict["Assists"] += Environment.NewLine + stats.GetAssists();
                dict["Total Points"] += Environment.NewLine + stats.GetTotalPoints();
                dict["PPG"] += Environment.NewLine + stats.GetPointsPerGame();
                dict["League"] += Environment.NewLine + stats.GetLeagueName();
                dict["Team"] += Environment.NewLine + stats.GetTeamName();
            }

            return dict;
        }

        private Dictionary<string, string> FillDictWithDraftData(Dictionary<string, string> dict, DraftDataParser draftData)
        {
            dict["Draft Year"] = draftData.GetYear();
            dict["Draft Round"] = draftData.GetRound();
            dict["Draft Overall"] = draftData.GetOverall();
            dict["Draft Team"] = draftData.GetTeamName();

            return dict;
        }
    }
}
