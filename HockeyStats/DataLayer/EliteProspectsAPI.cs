using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace HockeyStats
{
    public class EliteProspectsAPI
    {
        private static string key = System.Environment.GetEnvironmentVariable("EP_API_KEY", System.EnvironmentVariableTarget.User);

        public static JObject GetPlayerStats(string playerId)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/players/" + playerId + "/stats?apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Gets draft data about a specific player
        public static JObject GetPlayerDraftData(string playerId)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?filter=player.id%3D" + playerId + "%26draftType.name%3DNHL%20Entry%20Draft&sort=year&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Gets draft data of an entire draft year
        public static JObject GetDraftByYear(string draftYear)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?filter=draftType.league.name%3DNHL%26year%3D" + draftYear + "-01-01T00%3A00%3A00.000Z&sort=overall&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Gets first overall in each available draft (searches the past 200 years)
        public static JObject GetAllDrafts()
        {
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?limit=200&filter=draftType.name%3DNHL%20Entry%20Draft%26overall%3D1&sort=year:desc&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Gets first overall in the most recent draft
        public static JObject GetMostRecentDraft()
        {
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?limit=1&filter=draftType.name%3DNHL%20Entry%20Draft%26overall%3D1&sort=year:desc&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        public static JObject SearchPlayer(string playerName)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/search?q=" + playerName + "&type=player&limit=25&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        private static JObject GetEliteProspectsData(string requestString)
        {
            WebRequest request = WebRequest.Create(requestString);
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string jsonData = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return JObject.Parse(jsonData);
        }
    }
}
