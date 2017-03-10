using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public class StatLineParser : Parser
    {
        private JToken statLine;
        private JObject playerObj;
        private JObject teamObj;
        private JObject leagueObj;
        private JObject seasonObj;

        public StatLineParser()
        {
        }

        public void SetStatLine(JToken statLine)
        {
            this.statLine = statLine;
            playerObj = (JObject)statLine["player"];
            teamObj = (JObject)statLine["team"];
            leagueObj = (JObject)statLine["league"];
            seasonObj = (JObject)statLine["season"];
        }

        // Stats from base object
        public string ReturnGameType()
        {
            return (string)statLine["gameType"];
        }

        public void GetGamesPlayed(string key)
        {
            string value = (string)statLine["GP"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetGoalsOrGAA(string key)
        {
            string goals = (string)statLine["G"];
            string value = (goals != null) ? goals : (string)statLine["GAA"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetAssistsOrSVP(string key)
        {
            string assists = (string)statLine["A"];
            string value = (assists != null) ? assists : (string)statLine["SVP"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetTotalPoints(string key)
        {
            string value = (string)statLine["TP"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetPointsPerGame(string key)
        {
            string value = (string)statLine["PPG"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetPIM(string key)
        {
            string value = (string)statLine["PIM"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        // Stats from player object
        public void GetFirstName(string key)
        {
            string value = (string)playerObj["firstName"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetLastName(string key)
        {
            string value = (string)playerObj["lastName"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetDateOfBirth(string key)
        {
            string value = (string)playerObj["dateOfBirth"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetHeight(string key)
        {
            string value = (string)playerObj["height"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetWeight(string key)
        {
            string value = (string)playerObj["weight"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetShoots(string key)
        {
            string value = (string)playerObj["shoots"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetId(string key)
        {
            string value = (string)playerObj["id"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        // Stats from team object
        public void GetTeamName(string key)
        {
            string value = (string)teamObj["name"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        // Stats from league object
        public void GetLeagueName(string key)
        {
            string value = (string)leagueObj["name"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        // Stats from season object
        public void GetYear(string key)
        {
            string value = (string)seasonObj["name"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public string ReturnYear()
        {
            return (string)seasonObj["name"];
        }
    }
}
