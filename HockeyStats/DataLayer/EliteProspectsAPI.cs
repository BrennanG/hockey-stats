using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace HockeyStats
{
    public class EliteProspectsAPI
    {
        public static JObject GetPlayerStats(string playerId)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/players/" + playerId + "/stats";
            return GetEliteProspectsData(requestString);
        }

        public static JObject GetPlayerDraftData(string playerId)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?filter=player.id%3D" + playerId + "%26draftType.name%3DNHL%20Entry%20Draft&sort=year";
            return GetEliteProspectsData(requestString);
        }
        
        public static JObject SearchPlayer(string playerName)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/search?q=" + playerName +"&type=player&limit=25";
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
