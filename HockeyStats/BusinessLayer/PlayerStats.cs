using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public class PlayerStats
    {
        // Dictionary from year (as a string) to list of stat lines for that year
        private Dictionary<string, List<Dictionary<string, string>>> playerStats = new Dictionary<string, List<Dictionary<string, string>>>();
        private string playerId;

        private static Dictionary<string, Action> getStatMap = FillGetStatMap();
        private static StatLineParser statLineParser = new StatLineParser();
        private static DraftDataParser draftDataParser = new DraftDataParser();
        
        public PlayerStats(string playerId)
        {
            this.playerId = playerId;
            FillPlayerStats();
        }

        public Dictionary<string, string> GetCollapsedYear(string year)
        {
            Dictionary<string, string> returnDict = new Dictionary<string, string>();
            foreach (Dictionary<string, string> loopDict in playerStats[year])
            {
                foreach (string columnName in Columns.AllPossibleColumns)
                {
                    string junk;
                    if (returnDict.TryGetValue(columnName, out junk) && Columns.DynamicColumns.Contains(columnName))
                    {
                        returnDict[columnName] += Environment.NewLine + loopDict[columnName];
                    }
                    else
                    {
                        returnDict[columnName] = loopDict[columnName];
                    }
                }
            }
            return returnDict;
        }

        public string GetCollapsedColumnValue(string year, string columnName)
        {
            if (!playerStats.ContainsKey(year)) { return ""; }
            string collapsedColumn = "";
            foreach (Dictionary<string, string> dict in playerStats[year])
            {
                if (collapsedColumn == "")
                {
                    collapsedColumn = dict[columnName];
                }
                else
                {
                    collapsedColumn += Environment.NewLine + dict[columnName];
                }
            }
            return collapsedColumn;
        }

        public Dictionary<string, string> GetConstantColumnValues(string year)
        {
            Dictionary<string, string> returnDict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> keyValuePair in playerStats[year].First())
            {
                if (Columns.ConstantColumns.Contains(keyValuePair.Key))
                {
                    returnDict.Add(keyValuePair.Key, keyValuePair.Value);
                }
            }
            return returnDict;
        }

        public List<Dictionary<string, string>> GetDynamicColumnValues()
        {
            List<Dictionary<string, string>> dynamicColumnValues = new List<Dictionary<string, string>>();
            foreach (string year in playerStats.Keys)
            {
                Dictionary<string, string> collapsedYear = GetCollapsedYear(year);
                Dictionary<string, string> collapsedYearOnlyDynamicColumns = new Dictionary<string, string>();
                foreach (string columnName in collapsedYear.Keys)
                {
                    if (Columns.DynamicColumns.Contains(columnName))
                    {
                        collapsedYearOnlyDynamicColumns.Add(columnName, collapsedYear[columnName]);
                    }
                }
                dynamicColumnValues.Add(collapsedYearOnlyDynamicColumns);
            }
            return dynamicColumnValues;
        }

        private void FillPlayerStats()
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
                string year = statLineParser.ReturnYear();
                if (!playerStats.ContainsKey(year))
                {
                    playerStats[year] = new List<Dictionary<string, string>>();
                }
                if (statLineParser.ReturnGameType() == "REGULAR_SEASON")
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    playerStats[year].Add(dict);
                    FillDictWithStats(dict);
                }

            }
        }

        private void FillDictWithStats(Dictionary<string, string> dict)
        {
            statLineParser.SetDictionaryToFill(dict);
            draftDataParser.SetDictionaryToFill(dict);
            foreach (string columnName in Columns.AllPossibleColumns)
            {
                try
                {
                    getStatMap[columnName]();
                }
                catch (Exception) { /* Data was not found */}
            }
        }

        private static Dictionary<string, Action> FillGetStatMap()
        {
            Dictionary<string, Action> map = new Dictionary<string, Action>();
            map.Add("First Name", () => statLineParser.GetFirstName("First Name"));
            map.Add("Last Name", () => statLineParser.GetLastName("Last Name"));
            map.Add("Games Played", () => statLineParser.GetGamesPlayed("Games Played"));
            map.Add("Goals/GAA", () => statLineParser.GetGoalsOrGAA("Goals/GAA"));
            map.Add("Assists/Sv%", () => statLineParser.GetAssistsOrSVP("Assists/Sv%"));
            map.Add("Total Points", () => statLineParser.GetTotalPoints("Total Points"));
            map.Add("PPG", () => statLineParser.GetPointsPerGame("PPG"));
            map.Add("League", () => statLineParser.GetLeagueName("League"));
            map.Add("Team", () => statLineParser.GetTeamName("Team"));
            map.Add("Year", () => statLineParser.GetYear("Year"));
            map.Add("PIM", () => statLineParser.GetPIM("PIM"));
            map.Add("Date of Birth", () => statLineParser.GetDateOfBirth("Date of Birth"));
            map.Add("Height", () => statLineParser.GetHeight("Height"));
            map.Add("Weight", () => statLineParser.GetWeight("Weight"));
            map.Add("Shoots/Catches", () => statLineParser.GetShootsOrCatches("Shoots/Catches"));
            map.Add("Draft Year", () => draftDataParser.GetDraftYear("Draft Year"));
            map.Add("Draft Round", () => draftDataParser.GetDraftRound("Draft Round"));
            map.Add("Draft Overall", () => draftDataParser.GetDraftOverall("Draft Overall"));
            map.Add("Draft Team", () => draftDataParser.GetDraftTeamName("Draft Team"));
            map.Add("ID", () => statLineParser.GetId("ID"));
            return map;
        }
    }
}
