using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public class SearchDataParser : Parser
    {
        private JToken searchData;
        private JObject latestPlayerStats;
        private JObject latestTeamStats;

        public SearchDataParser()
        {
        }

        public void SetSearchData(JToken searchData)
        {
            this.searchData = searchData;
            latestPlayerStats = (searchData["latestPlayerStats"] == null) ? null : (JObject)searchData["latestPlayerStats"];
            latestTeamStats = (searchData["latestTeamStats"] == null) ? null : (JObject)searchData["latestTeamStats"];
        }

        public void GetFirstName(string key)
        {
            string value = (string)searchData["firstName"];
            AddToDict(key, value);
        }

        public void GetLastName(string key)
        {
            string value = (string)searchData["lastName"];
            AddToDict(key, value);
        }

        public void GetDateOfBirth(string key)
        {
            string value = (string)searchData["dateOfBirth"];
            if (value == null) { value = ""; }
            AddToDict(key, value);
        }

        public void GetId(string key)
        {
            string value = (string)searchData["id"];
            AddToDict(key, value);
        }

        public void GetLatestSeason(string key)
        {
            string value;
            try
            {
                value = (string)latestPlayerStats["season"]["name"];
            }
            catch
            {
                value = "";
            }
            AddToDict(key, value);
        }

        public void GetLatestTeam(string key)
        {
            string value;
            try
            {
                value = (string)latestPlayerStats["team"]["name"];
            }
            catch
            {
                value = "";
            }
            if (value == "")
            {
                try
                {
                    value = (string)searchData["name"];
                }
                catch
                {
                    value = "";
                }
            }
            AddToDict(key, value);
        }

        public void GetLatestLeague(string key)
        {
            string value;
            try
            {
                if (latestTeamStats["league"]["parentLeague"] != null)
                {
                    value = (string)latestTeamStats["league"]["parentLeague"]["name"];
                }
                else
                {
                    value = (string)latestTeamStats["league"]["name"];
                }
            }
            catch
            {
                value = "";
            }
            AddToDict(key, value);
        }
    }
}
