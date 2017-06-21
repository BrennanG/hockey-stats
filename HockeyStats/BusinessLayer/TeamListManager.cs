using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace HockeyStats
{
    public static class TeamListManager
    {
        public static List<string> GetPlayerIdsOnTeam(string teamId, string season)
        {
            StatLineParser statLineParser = new StatLineParser();
            JObject playerIdsJson = EliteProspectsAPI.GetPlayerIdsOnTeam(teamId, season);
            List<string> playerIds = new List<string>();
            foreach (JToken playerIdData in playerIdsJson["data"])
            {
                statLineParser.SetStatLine(playerIdData);
                string playerId = statLineParser.ReturnId();
                playerIds.Add(playerId);
            }
            return playerIds;
        }
    }
}
