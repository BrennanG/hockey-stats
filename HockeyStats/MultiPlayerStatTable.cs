using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HockeyStats
{
    public class MultiPlayerStatTable : PlayerStatTable
    {
        private List<Dictionary<string, string>> savedDicts = new List<Dictionary<string, string>>();
        private PlayerList playerList;

        public MultiPlayerStatTable(DataGridView dataGridView, PlayerList playerList)
            : base(dataGridView, playerList.primaryTableColumnNames)
        {
            this.playerList = playerList;
            new Thread(() => FillDataTable()).Start(); // Fill the table in a separate thread
        }

        public void AddPlayerById(string playerId)
        {
            Dictionary<string, string> DisplayDict = new Dictionary<string, string>();
            Dictionary<string, string> SavedDict = new Dictionary<string, string>();
            AddPlayerStatsToDict(DisplayDict, SavedDict, playerId);
            AddDraftDataToDict(DisplayDict, SavedDict, playerId);
            AddRowToDataTable(DisplayDict);
        }

        public Dictionary<string, string> GetSavedDictById(string playerId)
        {
            return savedDicts.Find((Dictionary<string, string> dict) => dict["ID"] == playerId);
        }

        private void FillDataTable()
        {
            foreach (string playerId in playerList.playerIds)
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
                    if (playerList.displayYears == null || playerList.displayYears.Count == 0 || playerList.displayYears.Contains(stats.GetYear()))
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
                dict["Year"] = stats.GetYear();
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
                dict["Year"] += Environment.NewLine + stats.GetYear();
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
