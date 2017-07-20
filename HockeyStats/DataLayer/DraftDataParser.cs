using Newtonsoft.Json.Linq;

namespace HockeyStats
{
    public class DraftDataParser : Parser
    {
        private JToken draftData;
        private JObject draftTeamObj;
        private JObject playerObj;

        public DraftDataParser()
        {
        }

        public void SetDraftData(JToken data)
        {
            if (data == null)
            {
                draftData = null;
            }
            else
            {
                draftData = data;
                draftTeamObj = (JObject)draftData["team"];
                playerObj = (JObject)draftData["player"];
            }
        }

        // Stats from base draft object
        public void GetDraftYear(string key)
        {
            if (draftData == null) { AddToDict(key, null); }
            string date = (string)draftData["year"];
            string year = date.Substring(6, 4);
            AddToDict(key, year);
        }

        public string ReturnDraftYear()
        {
            string date = (string)draftData["year"];
            string year = date.Substring(6, 4);
            return year;
        }

        public void GetDraftRound(string key)
        {
            if (draftData == null) { AddToDict(key, null); }
            string value = (string)draftData["round"];
            AddToDict(key, value);
        }

        public void GetDraftOverall(string key)
        {
            if (draftData == null) { AddToDict(key, null); }
            string value = (string)draftData["overall"];
            AddToDict(key, value);
        }

        // Stats from draft team object
        public void GetDraftTeamName(string key)
        {
            if (draftData == null) { AddToDict(key, null); }
            string value = (string)draftTeamObj["name"];
            AddToDict(key, value);
        }

        public string ReturnDraftTeamName()
        {
            if (draftData == null) { return ""; }
            return (string)draftTeamObj["name"];
        }

        // Stats from player object
        public string ReturnPlayerId()
        {
            string value = (string)playerObj["id"];
            return value;
        }
    }
}
