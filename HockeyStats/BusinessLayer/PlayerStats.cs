using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public class PlayerStats
    {
        // Map from year (as a string) to list of stat lines for that year
        private Dictionary<string, List<Dictionary<string, string>>> playerStats = new Dictionary<string, List<Dictionary<string, string>>>();
        private Dictionary<string, string> constantPlayerStats = new Dictionary<string, string>();
        private string playerId;

        private static Dictionary<string, Action> getStatMap = FillGetStatMap();
        private static StatLineParser statLineParser = new StatLineParser();
        private static DraftDataParser draftDataParser = new DraftDataParser();
        
        public PlayerStats(string playerId)
        {
            this.playerId = playerId;
            FillPlayerStats();
            FillConstantPlayerStats();
        }

        public Dictionary<string, string> GetCollapsedYear(string year)
        {
            if (!playerStats.ContainsKey(year)) { return constantPlayerStats;  }

            Dictionary<string, string> returnDict = new Dictionary<string, string>();
            foreach (Dictionary<string, string> loopDict in playerStats[year])
            {
                foreach (string columnName in Constants.AllPossibleColumns)
                {
                    string junk;
                    if (returnDict.TryGetValue(columnName, out junk) && Constants.DynamicColumns.Contains(columnName) && columnName != Constants.SEASON)
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
                if (collapsedColumn == "" || Constants.ConstantColumns.Contains(columnName))
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

        public Dictionary<string, string> GetConstantColumnValues()
        {
            return constantPlayerStats;
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
                    if (Constants.DynamicColumns.Contains(columnName))
                    {
                        collapsedYearOnlyDynamicColumns.Add(columnName, collapsedYear[columnName]);
                    }
                }
                dynamicColumnValues.Add(collapsedYearOnlyDynamicColumns);
            }
            return dynamicColumnValues;
        }

        public string GetPlayerId()
        {
            return playerId;
        }

        public string GetDraftYear()
        {
            if (constantPlayerStats.ContainsKey(Constants.DRAFT_YEAR))
            {
                return constantPlayerStats[Constants.DRAFT_YEAR];
            }
            else
            {
                return null;
            }
        }

        private void FillPlayerStats()
        {
            JObject draftJson = EliteProspectsAPI.GetPlayerDraftData(playerId);
            JToken data = draftJson["data"];
            draftDataParser.SetDraftData(data);

            JObject statsJson = EliteProspectsAPI.GetPlayerStats(playerId);
            foreach (JToken statLine in statsJson["data"])
            {
                statLineParser.SetStatLine(statLine);
                string year = statLineParser.ReturnYear();
                if (statLineParser.ReturnGameType() == "REGULAR_SEASON")
                {
                    if (!playerStats.ContainsKey(year))
                    {
                        playerStats[year] = new List<Dictionary<string, string>>();
                    }
                    Dictionary<string, string> dict = new Dictionary<string, string>();
                    playerStats[year].Add(dict);
                    FillDictWithStats(dict);
                }

            }
        }

        private void FillConstantPlayerStats()
        {
            Dictionary<string, string> firstDictWithData = new Dictionary<string, string>();
            foreach (List<Dictionary<string, string>> dicts in playerStats.Values)
            {
                if (dicts.Count > 0 && dicts[0].Keys.Count > 0)
                {
                    firstDictWithData = dicts[0];
                    break;
                }
            }
            foreach (string columnName in Constants.ConstantColumns)
            {
                constantPlayerStats[columnName] = firstDictWithData[columnName];
            }
        }

        private void FillDictWithStats(Dictionary<string, string> dict)
        {
            statLineParser.SetDictionaryToFill(dict);
            draftDataParser.SetDictionaryToFill(dict);
            foreach (string columnName in Constants.AllPossibleColumns)
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
            map.Add(Constants.FIRST_NAME, () => statLineParser.GetFirstName(Constants.FIRST_NAME));
            map.Add(Constants.LAST_NAME, () => statLineParser.GetLastName(Constants.LAST_NAME));
            map.Add(Constants.GAMES_PLAYED, () => statLineParser.GetGamesPlayed(Constants.GAMES_PLAYED));
            map.Add(Constants.GOALS_GAA, () => statLineParser.GetGoalsOrGAA(Constants.GOALS_GAA));
            map.Add(Constants.ASSISTS_SVP, () => statLineParser.GetAssistsOrSVP(Constants.ASSISTS_SVP));
            map.Add(Constants.TOTAL_POINTS, () => statLineParser.GetTotalPoints(Constants.TOTAL_POINTS));
            map.Add(Constants.PPG, () => statLineParser.GetPointsPerGame(Constants.PPG));
            map.Add(Constants.PLUS_MINUS, () => statLineParser.GetPlusMinus(Constants.PLUS_MINUS));
            map.Add(Constants.POSITION, () => statLineParser.GetPosition(Constants.POSITION));
            map.Add(Constants.LEAGUE, () => statLineParser.GetLeagueName(Constants.LEAGUE));
            map.Add(Constants.TEAM, () => statLineParser.GetTeamName(Constants.TEAM));
            map.Add(Constants.SEASON, () => statLineParser.GetSeason(Constants.SEASON));
            map.Add(Constants.PIM, () => statLineParser.GetPIM(Constants.PIM));
            map.Add(Constants.DATE_OF_BIRTH, () => statLineParser.GetDateOfBirth(Constants.DATE_OF_BIRTH));
            map.Add(Constants.HEIGHT, () => statLineParser.GetHeight(Constants.HEIGHT));
            map.Add(Constants.WEIGHT, () => statLineParser.GetWeight(Constants.WEIGHT));
            map.Add(Constants.SHOOTS_CATCHES, () => statLineParser.GetShootsOrCatches(Constants.SHOOTS_CATCHES));
            map.Add(Constants.DRAFT_YEAR, () => draftDataParser.GetDraftYear(Constants.DRAFT_YEAR));
            map.Add(Constants.DRAFT_ROUND, () => draftDataParser.GetDraftRound(Constants.DRAFT_ROUND));
            map.Add(Constants.DRAFT_OVERALL, () => draftDataParser.GetDraftOverall(Constants.DRAFT_OVERALL));
            map.Add(Constants.DRAFT_TEAM, () => draftDataParser.GetDraftTeamName(Constants.DRAFT_TEAM));
            map.Add(Constants.ID, () => statLineParser.GetId(Constants.ID));
            return map;
        }
    }
}
