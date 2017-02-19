using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace HockeyStats
{
    public class EliteProspectsAPI
    {
        public static JObject GetPlayerStats(string playerId)
        {
            WebRequest request = WebRequest.Create("http://api.eliteprospects.com:80/beta/players/" + playerId + "/stats");
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string jsonData = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return JObject.Parse(jsonData);
        }

        public static JObject GetPlayerDraftData(string playerId)
        {
            WebRequest request = WebRequest.Create("http://api.eliteprospects.com:80/beta/drafts?filter=player.id%3D" + playerId + "%26draftType.name%3DNHL%20Entry%20Draft");
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
