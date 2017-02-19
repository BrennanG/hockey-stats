using Newtonsoft.Json.Linq;

namespace HockeyStats
{
    public class DraftDataParser
    {
        private JToken draftData;
        private JObject teamObj;

        public DraftDataParser(JToken draftData)
        {
            this.draftData = draftData;
            teamObj = (JObject)draftData["team"];
        }

        // Stats from base object
        public string GetYear()
        {
            string date = (string)draftData["year"];
            string year = date.Substring(6, 4);
            return year;
        }

        public string GetRound()
        {
            return (string)draftData["round"];
        }

        public string GetOverall()
        {
            return (string)draftData["overall"];
        }

        // Stats from team object
        public string GetTeamName()
        {
            return (string)teamObj["name"];
        }
    }
}
