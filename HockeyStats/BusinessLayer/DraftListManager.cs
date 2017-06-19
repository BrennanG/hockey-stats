using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace HockeyStats
{
    public static class DraftListManager
    {
        public static string GetMostRecentDraftYear()
        {
            DraftDataParser draftDataParser = new DraftDataParser();
            JObject draftYearJson = EliteProspectsAPI.GetMostRecentDraft();
            JToken draftData = draftYearJson["data"].First;
            draftDataParser.SetDraftData(draftData);
            return draftDataParser.ReturnDraftYear();
        }

        public static List<string> GetAllDraftYears()
        {
            DraftDataParser draftDataParser = new DraftDataParser();
            JObject draftYearsJson = EliteProspectsAPI.GetAllDrafts();
            List<string> draftYears = new List<string>();
            foreach (JToken draftData in draftYearsJson["data"])
            {
                draftDataParser.SetDraftData(draftData);
                string year = draftDataParser.ReturnDraftYear();
                draftYears.Add(year);
            }
            return draftYears;
        }

        public static List<string> GetPlayersInDraftYear(string year, int roundLowerBound, int roundUpperBound)
        {
            if (roundUpperBound < roundLowerBound) { throw new Exception("Lower bound was higher than upper bound."); }
            DraftDataParser draftDataParser = new DraftDataParser();
            JObject draftYearJson = EliteProspectsAPI.GetDraftByYear(year, roundLowerBound, roundUpperBound);
            List<string> playerIds = new List<string>();
            foreach (JToken draftData in draftYearJson["data"])
            {
                draftDataParser.SetDraftData(draftData);
                string id = draftDataParser.ReturnPlayerId();
                playerIds.Add(id);
            }
            return playerIds;
        }

        public static int GetNumberOfRoundsInDraft(string year)
        {
            DraftDataParser draftDataParser = new DraftDataParser();
            int i = 1;
            while (true)
            {
                JObject draftYearJson = EliteProspectsAPI.GetSinglePlayerInDraftRound(year, i);
                if ((int)draftYearJson["metadata"]["count"] == 0)
                {
                    return i - 1;
                }
                i++;
            }
        }
    }
}
