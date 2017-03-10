using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HockeyStats
{
    public class StatLineParser
    {
        private string playerId;
        private List<string> displayYears;
        private Dictionary<string, Action> getStatMap = new Dictionary<string, Action>();
        private Dictionary<string, string> currentDict;
        private JToken statLine;
        private JObject playerObj;
        private JObject teamObj;
        private JObject leagueObj;
        private JObject seasonObj;
        private JToken draftData;
        private JObject draftTeamObj;

        public StatLineParser(string playerId, List<string> displayYears)
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
                draftData = data.First;
                draftTeamObj = (JObject)draftData["team"];
            }

            JObject statsJson = EliteProspectsAPI.GetPlayerStats(playerId);
            foreach (JToken statLine in statsJson["data"])
            {
                this.statLine = statLine;
                playerObj = (JObject)statLine["player"];
                teamObj = (JObject)statLine["team"];
                leagueObj = (JObject)statLine["league"];
                seasonObj = (JObject)statLine["season"];
                if (ReturnGameType() == "REGULAR_SEASON")
                {
                    if (displayYears == null || displayYears.Count == 0 || displayYears.Contains(ReturnYear()))
                    {
                        FillDictWithStats(displayDict);
                    }
                    FillDictWithStats(savedDict);
                }

            }
        }

        private void FillDictWithStats(Dictionary<string, string> dict)
        {
            currentDict = dict;
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
            getStatMap.Add("First Name", () => GetFirstName("First Name"));
            getStatMap.Add("Last Name", () => GetLastName("Last Name"));
            getStatMap.Add("Games Played", () => GetGamesPlayed("Games Played"));
            getStatMap.Add("Goals/GAA", () => GetGoalsOrGAA("Goals/GAA"));
            getStatMap.Add("Assists/Sv%", () => GetAssistsOrSVP("Assists/Sv%"));
            getStatMap.Add("Total Points", () => GetTotalPoints("Total Points"));
            getStatMap.Add("PPG", () => GetPointsPerGame("PPG"));
            getStatMap.Add("League", () => GetLeagueName("League"));
            getStatMap.Add("Team", () => GetTeamName("Team"));
            getStatMap.Add("Year", () => GetYear("Year"));
            getStatMap.Add("Draft Year", () => GetDraftYear("Draft Year"));
            getStatMap.Add("Draft Round", () => GetDraftRound("Draft Round"));
            getStatMap.Add("Draft Overall", () => GetDraftOverall("Draft Overall"));
            getStatMap.Add("Draft Team", () => GetDraftTeamName("Draft Team"));
        }

        private void AddOrAppendToDict(Dictionary<string, string> dict, string key, string value)
        {
            string junk;
            if (dict.TryGetValue(key, out junk))
            {
                dict[key] += Environment.NewLine + value;
            }
            else
            {
                dict[key] = value;
            }
        }

        // Stats from base object
        private string ReturnGameType()
        {
            return (string)statLine["gameType"];
        }

        private void GetGamesPlayed(string key)
        {
            string value = (string)statLine["GP"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetGoalsOrGAA(string key)
        {
            string goals = (string)statLine["G"];
            string value = (goals != null) ? goals : (string)statLine["GAA"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetAssistsOrSVP(string key)
        {
            string assists = (string)statLine["A"];
            string value = (assists != null) ? assists : (string)statLine["SVP"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetTotalPoints(string key)
        {
            string value = (string)statLine["TP"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetPointsPerGame(string key)
        {
            string value = (string)statLine["PPG"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetPIM(string key)
        {
            string value = (string)statLine["PIM"];
            AddOrAppendToDict(currentDict, key, value);
        }

        // Stats from player object
        private void GetFirstName(string key)
        {
            string value = (string)playerObj["firstName"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetLastName(string key)
        {
            string value = (string)playerObj["lastName"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetDateOfBirth(string key)
        {
            string value = (string)playerObj["dateOfBirth"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetHeight(string key)
        {
            string value = (string)playerObj["height"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetWeight(string key)
        {
            string value = (string)playerObj["weight"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetShoots(string key)
        {
            string value = (string)playerObj["shoots"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private void GetId(string key)
        {
            string value = (string)playerObj["id"];
            AddOrAppendToDict(currentDict, key, value);
        }

        // Stats from team object
        private void GetTeamName(string key)
        {
            string value = (string)teamObj["name"];
            AddOrAppendToDict(currentDict, key, value);
        }

        // Stats from league object
        private void GetLeagueName(string key)
        {
            string value = (string)leagueObj["name"];
            AddOrAppendToDict(currentDict, key, value);
        }

        // Stats from season object
        private void GetYear(string key)
        {
            string value = (string)seasonObj["name"];
            AddOrAppendToDict(currentDict, key, value);
        }

        private string ReturnYear()
        {
            return (string)seasonObj["name"];
        }

        // Stats from base draft object
        public void GetDraftYear(string key)
        {
            string date = (string)draftData["year"];
            string year = date.Substring(6, 4);
            AddOrAppendToDict(currentDict, key, year);
        }

        public void GetDraftRound(string key)
        {
            string value = (string)draftData["round"];
            AddOrAppendToDict(currentDict, key, value);
        }

        public void GetDraftOverall(string key)
        {
            string value = (string)draftData["overall"];
            AddOrAppendToDict(currentDict, key, value);
        }

        // Stats from draft team object
        public void GetDraftTeamName(string key)
        {
            string value = (string)draftTeamObj["name"];
            AddOrAppendToDict(currentDict, key, value);
        }
    }
}
