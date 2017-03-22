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
            AddToDict(key, value);
        }

        public void GetGoalsOrGAA(string key)
        {
            string goals = (string)statLine["G"];
            string value = (goals != null) ? goals : (string)statLine["GAA"];
            AddToDict(key, value);
        }

        public void GetAssistsOrSVP(string key)
        {
            string assists = (string)statLine["A"];
            string value = (assists != null) ? assists : (string)statLine["SVP"];
            AddToDict(key, value);
        }

        public void GetTotalPoints(string key)
        {
            string value = (string)statLine["TP"];
            AddToDict(key, value);
        }

        public void GetPointsPerGame(string key)
        {
            string value = (string)statLine["PPG"];
            AddToDict(key, value);
        }

        public void GetPIM(string key)
        {
            string value = (string)statLine["PIM"];
            AddToDict(key, value);
        }

        // Stats from player object
        public void GetFirstName(string key)
        {
            string value = (string)playerObj["firstName"];
            AddToDict(key, value);
        }

        public void GetLastName(string key)
        {
            string value = (string)playerObj["lastName"];
            AddToDict(key, value);
        }

        public void GetDateOfBirth(string key)
        {
            string value = (string)playerObj["dateOfBirth"];
            AddToDict(key, value);
        }

        public void GetHeight(string key)
        {
            string value = (string)playerObj["height"];
            int height;
            Int32.TryParse(value, out height);
            height = Convert.ToInt32(height * 0.3937007874); // Convert to pounds
            int feet = height / 12;
            int inches = height - (feet * 12);
            AddToDict(key, feet.ToString() + "' " + inches.ToString() + "\"");
        }

        public void GetWeight(string key)
        {
            string value = (string)playerObj["weight"];
            int weight;
            Int32.TryParse(value, out weight);
            weight = Convert.ToInt32(weight * 2.20462262185); // Convert to inches
            AddToDict(key, weight.ToString() + " lbs");
        }

        public void GetShootsOrCatches(string key)
        {
            string shoots = (string)playerObj["shoots"];
            string value = (shoots != null) ? shoots : (string)playerObj["catches"];
            AddToDict(key, value[0].ToString());
        }

        public void GetId(string key)
        {
            string value = (string)playerObj["id"];
            AddToDict(key, value);
        }

        // Stats from team object
        public void GetTeamName(string key)
        {
            string value = (string)teamObj["name"];
            AddToDict(key, value);
        }

        // Stats from league object
        public void GetLeagueName(string key)
        {
            string value = (string)leagueObj["name"];
            AddToDict(key, value);
        }

        // Stats from season object
        public void GetYear(string key)
        {
            string value = (string)seasonObj["name"];
            AddToDict(key, value);
        }

        public string ReturnYear()
        {
            return (string)seasonObj["name"];
        }
    }
}
