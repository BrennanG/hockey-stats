using Newtonsoft.Json.Linq;

namespace HockeyStats
{
    public class DraftDataParser : Parser
    {
        private JToken draftData;
        private JObject draftTeamObj;

        public DraftDataParser()
        {
        }

        public void SetDraftData(JToken draftData)
        {
            this.draftData = draftData;
            draftTeamObj = (JObject)draftData["team"];
        }

        // Stats from base draft object
        public void GetDraftYear(string key)
        {
            string date = (string)draftData["year"];
            string year = date.Substring(6, 4);
            AddOrAppendToDict(dictionaryToFill, key, year);
        }

        public void GetDraftRound(string key)
        {
            string value = (string)draftData["round"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        public void GetDraftOverall(string key)
        {
            string value = (string)draftData["overall"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }

        // Stats from draft team object
        public void GetDraftTeamName(string key)
        {
            string value = (string)draftTeamObj["name"];
            AddOrAppendToDict(dictionaryToFill, key, value);
        }
    }
}
