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
                foreach (string columnName in Columns.AllPossibleColumns)
                {
                    string junk;
                    if (returnDict.TryGetValue(columnName, out junk) && Columns.DynamicColumns.Contains(columnName) && columnName != Columns.SEASON)
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
                if (collapsedColumn == "" || Columns.ConstantColumns.Contains(columnName))
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
                    if (Columns.DynamicColumns.Contains(columnName))
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
            foreach (string columnName in Columns.ConstantColumns)
            {
                constantPlayerStats[columnName] = firstDictWithData[columnName];
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
            map.Add(Columns.FIRST_NAME, () => statLineParser.GetFirstName(Columns.FIRST_NAME));
            map.Add(Columns.LAST_NAME, () => statLineParser.GetLastName(Columns.LAST_NAME));
            map.Add(Columns.GAMES_PLAYED, () => statLineParser.GetGamesPlayed(Columns.GAMES_PLAYED));
            map.Add(Columns.GOALS_GAA, () => statLineParser.GetGoalsOrGAA(Columns.GOALS_GAA));
            map.Add(Columns.ASSISTS_SVP, () => statLineParser.GetAssistsOrSVP(Columns.ASSISTS_SVP));
            map.Add(Columns.TOTAL_POINTS, () => statLineParser.GetTotalPoints(Columns.TOTAL_POINTS));
            map.Add(Columns.PPG, () => statLineParser.GetPointsPerGame(Columns.PPG));
            map.Add(Columns.PLUS_MINUS, () => statLineParser.GetPlusMinus(Columns.PLUS_MINUS));
            map.Add(Columns.POSITION, () => statLineParser.GetPosition(Columns.POSITION));
            map.Add(Columns.LEAGUE, () => statLineParser.GetLeagueName(Columns.LEAGUE));
            map.Add(Columns.TEAM, () => statLineParser.GetTeamName(Columns.TEAM));
            map.Add(Columns.SEASON, () => statLineParser.GetSeason(Columns.SEASON));
            map.Add(Columns.PIM, () => statLineParser.GetPIM(Columns.PIM));
            map.Add(Columns.DATE_OF_BIRTH, () => statLineParser.GetDateOfBirth(Columns.DATE_OF_BIRTH));
            map.Add(Columns.HEIGHT, () => statLineParser.GetHeight(Columns.HEIGHT));
            map.Add(Columns.WEIGHT, () => statLineParser.GetWeight(Columns.WEIGHT));
            map.Add(Columns.SHOOTS_CATCHES, () => statLineParser.GetShootsOrCatches(Columns.SHOOTS_CATCHES));
            map.Add(Columns.DRAFT_YEAR, () => draftDataParser.GetDraftYear(Columns.DRAFT_YEAR));
            map.Add(Columns.DRAFT_ROUND, () => draftDataParser.GetDraftRound(Columns.DRAFT_ROUND));
            map.Add(Columns.DRAFT_OVERALL, () => draftDataParser.GetDraftOverall(Columns.DRAFT_OVERALL));
            map.Add(Columns.DRAFT_TEAM, () => draftDataParser.GetDraftTeamName(Columns.DRAFT_TEAM));
            map.Add(Columns.ID, () => statLineParser.GetId(Columns.ID));
            return map;
        }
    }
}
