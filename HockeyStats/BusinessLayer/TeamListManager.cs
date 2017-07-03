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
            HashSet<string> playerIds = new HashSet<string>();
            if (playerIdsJson["data"] == null) { return playerIds.ToList(); }
            foreach (JToken playerIdData in playerIdsJson["data"])
            {
                statLineParser.SetStatLine(playerIdData);
                string playerId = statLineParser.ReturnId();
                playerIds.Add(playerId);
            }
            return playerIds.ToList();
        }

        public static List<string> GetTeamSeasons(string teamId)
        {
            JObject teamSeasons = EliteProspectsAPI.GetTeamSeasons(teamId);
            HashSet<string> seasonNames = new HashSet<string>();
            foreach (JToken seasonData in teamSeasons["data"])
            {
                string seasonName = seasonData["name"].ToString();
                seasonNames.Add(seasonName);
            }
            return seasonNames.ToList();
        }
    }
}
