using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public class PlayerStats
    {
        // Map from season to list of stat lines for that year
        private Dictionary<string, List<Dictionary<string, string>>> regularSeasonPlayerStats = new Dictionary<string, List<Dictionary<string, string>>>();
        private Dictionary<string, List<Dictionary<string, string>>> playoffsPlayerStats = new Dictionary<string, List<Dictionary<string, string>>>();

        private Dictionary<string, HashSet<string>> leaguesBySeason = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, Dictionary<string, string>> teamIds = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> constantPlayerStats = new Dictionary<string, string>();

        private string playerId;

        private Dictionary<string, Action> getStatMap;
        private StatLineParser statLineParser = new StatLineParser();
        private DraftDataParser draftDataParser = new DraftDataParser();
        
        public PlayerStats(string playerId)
        {
            this.playerId = playerId;
            getStatMap = FillGetStatMap();
            FillPlayerStats();
            FillConstantPlayerStats();
        }

        public Dictionary<string, string> GetCollapsedYear(string season, string seasonType)
        {
            Dictionary<string, List<Dictionary<string, string>>> playerStats = GetPlayerStatsForSeasonType(seasonType);
            if (playerStats == null || !playerStats.ContainsKey(season)) { return constantPlayerStats;  }

            Dictionary<string, string> returnDict = new Dictionary<string, string>();
            foreach (Dictionary<string, string> loopDict in playerStats[season])
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

        public string GetCollapsedColumnValue(string season, string columnName, string seasonType)
        {
            if (Constants.ConstantColumns.Contains(columnName))
            {
                return constantPlayerStats[columnName];
            }

            Dictionary<string, List<Dictionary<string, string>>> playerStats = GetPlayerStatsForSeasonType(seasonType);
            if (playerStats == null || !playerStats.ContainsKey(season)) { return String.Empty; }
            
            string collapsedColumn = String.Empty;
            foreach (Dictionary<string, string> dict in playerStats[season])
            {
                if (collapsedColumn == String.Empty)
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

        public List<Dictionary<string, string>> GetDynamicColumnValues(string seasonType)
        {
            List<Dictionary<string, string>> dynamicColumnValues = new List<Dictionary<string, string>>();
            foreach (string season in regularSeasonPlayerStats.Keys)
            {
                if (seasonType == Constants.REGULAR_SEASON || playoffsPlayerStats.ContainsKey(season))
                {
                    Dictionary<string, string> collapsedYear = GetCollapsedYear(season, seasonType);
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
                else
                {
                    Dictionary<string, string> collapsedYearOnlyDynamicColumns = new Dictionary<string, string>() { { Constants.SEASON, season } };
                    dynamicColumnValues.Add(collapsedYearOnlyDynamicColumns);
                }
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

        public string GetFirstYearOfDraftEligibility()
        {
            if (!constantPlayerStats.ContainsKey(Constants.DATE_OF_BIRTH) || String.IsNullOrEmpty(constantPlayerStats[Constants.DATE_OF_BIRTH])) { return null; }
            
            DateTime dateOfBirth = DateTime.Parse(constantPlayerStats[Constants.DATE_OF_BIRTH]);
            if (dateOfBirth.Month < 9 || (dateOfBirth.Month == 9 && dateOfBirth.Day <= 15))
            {
                return (dateOfBirth.Year + 18).ToString();
            }
            else
            {
                return (dateOfBirth.Year + 19).ToString();
            }
        }

        public string GetTeamId(string year, string teamName)
        {
            return teamIds[year][teamName];
        }

        public Dictionary<string, HashSet<string>> GetLeaguesBySeason()
        {
            return leaguesBySeason;
        }

        private void FillPlayerStats()
        {
            JObject draftJson = EliteProspectsAPI.GetPlayerDraftData(playerId);
            JToken data = draftJson["data"];
            draftDataParser.SetDraftData(data == null ? null : data.First);

            JObject statsJson = EliteProspectsAPI.GetPlayerStats(playerId);
            foreach (JToken statLine in statsJson["data"])
            {
                statLineParser.SetStatLine(statLine);

                string seasonType = statLineParser.ReturnSeasonType();
                if (!Constants.SeasonTypes.Contains(seasonType)) { continue; }
                Dictionary<string, List<Dictionary<string, string>>> playerStatsToFill = GetPlayerStatsForSeasonType(seasonType);
                
                string season = statLineParser.ReturnYear();
                if (!playerStatsToFill.ContainsKey(season))
                {
                    playerStatsToFill[season] = new List<Dictionary<string, string>>();
                }
                Dictionary<string, string> dict = new Dictionary<string, string>();
                playerStatsToFill[season].Add(dict);
                FillDictWithStats(dict);

                if (!teamIds.ContainsKey(season))
                {
                    teamIds[season] = new Dictionary<string, string>();
                }
                teamIds[season][statLineParser.ReturnTeamName()] = statLineParser.ReturnTeamId();
                
                if (!leaguesBySeason.ContainsKey(season))
                {
                    leaguesBySeason[season] = new HashSet<string>();
                }
                leaguesBySeason[season].Add(statLineParser.ReturnLeagueName());
            }
        }
        
        private void FillConstantPlayerStats()
        {
            Dictionary<string, string> firstDictWithData = new Dictionary<string, string>();
            foreach (List<Dictionary<string, string>> dicts in regularSeasonPlayerStats.Values)
            {
                if (dicts.Count > 0 && dicts[0].Keys.Count > 0)
                {
                    firstDictWithData = dicts[0];
                    break;
                }
            }
            foreach (string columnName in Constants.ConstantColumns)
            {
                if (firstDictWithData.ContainsKey(columnName))
                {
                    constantPlayerStats[columnName] = firstDictWithData[columnName];
                }
            }
        }

        private Dictionary<string, List<Dictionary<string, string>>> GetPlayerStatsForSeasonType(string seasonType)
        {
            if (seasonType == Constants.REGULAR_SEASON)
            {
                return regularSeasonPlayerStats;
            }
            else if (seasonType == Constants.PLAYOFFS)
            {
                return playoffsPlayerStats;
            }
            else
            {
                return null;
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
                catch (Exception) { /* Data was not found */ }
            }
        }
        
        private Dictionary<string, Action> FillGetStatMap()
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
            //map.Add(Constants.ID, () => statLineParser.GetId(Constants.ID));
            return map;
        }
    }
}
