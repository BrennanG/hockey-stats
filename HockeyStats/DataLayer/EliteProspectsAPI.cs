﻿using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System;

namespace HockeyStats
{
    public class EliteProspectsAPI
    {
        private static string key = Environment.GetEnvironmentVariable("EP_API_KEY", EnvironmentVariableTarget.User);

        // Gets stats for a specific player
        public static JObject GetPlayerStats(string playerId)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/players/" + playerId + "/stats?limit=500&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Searches for the given player name
        public static JObject SearchForPlayer(string playerName)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/search?q=" + playerName + "&type=player&limit=25&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Searches for the given team name
        public static JObject SearchForTeam(string teamName)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/search?q=" + teamName + "&type=team&limit=25&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Returns the team's general data (no stats/rosters)
        public static JObject GetTeamData(string teamId)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/teams/" + teamId + "?apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Returns the player IDs of every player on the team during the given season
        public static JObject GetPlayerIdsOnTeam(string teamId, string season)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/teams/" + teamId + "/playerstats?season=" + season + "&limit=500&fields=player.id&sort=TP:desc&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Returns all seasons that the team has participated in
        public static JObject GetTeamSeasons(string teamId)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/teams/" + teamId + "/seasons?apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Gets draft data about a specific player
        public static JObject GetPlayerDraftData(string playerId)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?filter=player.id%3D" + playerId + "%26draftType.name%3DNHL%20Entry%20Draft&sort=year&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Gets draft data of an entire draft year of the given rounds
        public static JObject GetDraftByYear(string draftYear, int roundLowerBound, int roundUpperBound)
        {
            string rounds = roundLowerBound.ToString();
            for (int i = roundLowerBound + 1; i <= roundUpperBound; i++)
            {
                rounds += "%2C" + i;
            }
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?filter=round%3D" + rounds + "%26draftType.name%3DNHL%20Entry%20Draft%26year%3D" + draftYear + "-01-01T00%3A00%3A00.000Z&sort=overall&limit=1000&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Gets first overall in each available draft (searches the past 1000 years)
        public static JObject GetAllDrafts()
        {
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?limit=1000&filter=draftType.name%3DNHL%20Entry%20Draft%26overall%3D1&sort=year:desc&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Gets first overall in the most recent draft
        public static JObject GetMostRecentDraft()
        {
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?limit=1&filter=draftType.name%3DNHL%20Entry%20Draft%26overall%3D1&sort=year:desc&apikey=" + key;
            return GetEliteProspectsData(requestString);
        }

        // Gets a single player in a specific draft round of a specific year
        public static JObject GetSinglePlayerInDraftRound(string year, int round)
        {
            string requestString = "http://api.eliteprospects.com:80/beta/drafts?limit=1&filter=round%3D" + round.ToString() + "%26draftType.name%3DNHL%20Entry%20Draft%26year%3D" + year + "-01-01T00%3A00%3A00.000Z&apikey=" + key;
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
