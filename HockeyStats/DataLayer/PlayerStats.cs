using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HockeyStats
{
    public class PlayerStats
    {
        private string playerId;
        private List<string> displayYears;
        private Dictionary<string, Action> getStatMap = new Dictionary<string, Action>();
        private StatLineParser statLineParser = new StatLineParser();
        private DraftDataParser draftDataParser = new DraftDataParser();

        public PlayerStats(string playerId, List<string> displayYears)
        {
            this.playerId = playerId;
            this.displayYears = displayYears;
            FillGetStatMap();
        }

        public void AddPlayerStatsToDicts(Dictionary<string, string> displayDict, Dictionary<string, string> savedDict)
        {
            JObject draftJson = EliteProspectsAPI.GetPlayerDraftData(playerId);
            JToken data = draftJson["data"];
            if (data != null)
            {
                draftDataParser.SetDraftData(data.First);
            }

            JObject statsJson = EliteProspectsAPI.GetPlayerStats(playerId);
            foreach (JToken statLine in statsJson["data"])
            {
                statLineParser.SetStatLine(statLine);
                if (statLineParser.ReturnGameType() == "REGULAR_SEASON")
                {
                    if (displayYears == null || displayYears.Count == 0 || displayYears.Contains(statLineParser.ReturnYear()))
                    {
                        FillDictWithStats(displayDict);
                    }
                    FillDictWithStats(savedDict);
                }

            }
        }

        private void FillDictWithStats(Dictionary<string, string> dict)
        {
            statLineParser.SetDictionaryToFill(dict);
            draftDataParser.SetDictionaryToFill(dict);
            List<string> columnsToAdd = (dict.Count == 0) ? Columns.AllPossibleColumns : Columns.DynamicColumns;
            foreach (string columnName in columnsToAdd)
            {
                try
                {
                    getStatMap[columnName]();
                }
                catch (Exception) { /* Data was not found */}
            }
        }

        private void FillGetStatMap()
        {
            getStatMap.Add("First Name", () => statLineParser.GetFirstName("First Name"));
            getStatMap.Add("Last Name", () => statLineParser.GetLastName("Last Name"));
            getStatMap.Add("Games Played", () => statLineParser.GetGamesPlayed("Games Played"));
            getStatMap.Add("Goals/GAA", () => statLineParser.GetGoalsOrGAA("Goals/GAA"));
            getStatMap.Add("Assists/Sv%", () => statLineParser.GetAssistsOrSVP("Assists/Sv%"));
            getStatMap.Add("Total Points", () => statLineParser.GetTotalPoints("Total Points"));
            getStatMap.Add("PPG", () => statLineParser.GetPointsPerGame("PPG"));
            getStatMap.Add("League", () => statLineParser.GetLeagueName("League"));
            getStatMap.Add("Team", () => statLineParser.GetTeamName("Team"));
            getStatMap.Add("Year", () => statLineParser.GetYear("Year"));
            getStatMap.Add("Draft Year", () => draftDataParser.GetDraftYear("Draft Year"));
            getStatMap.Add("Draft Round", () => draftDataParser.GetDraftRound("Draft Round"));
            getStatMap.Add("Draft Overall", () => draftDataParser.GetDraftOverall("Draft Overall"));
            getStatMap.Add("Draft Team", () => draftDataParser.GetDraftTeamName("Draft Team"));
        }
    }
}
