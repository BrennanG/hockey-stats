using Newtonsoft.Json.Linq;

namespace HockeyStats
{
    public class StatLineParser
    {
        private JToken statLine;
        private JObject playerObj;
        private JObject teamObj;
        private JObject leagueObj;
        private JObject seasonObj;
        private string playerId;

        public StatLineParser(JToken statLine)
        {
            this.statLine = statLine;
            playerObj = (JObject)statLine["player"];
            teamObj = (JObject)statLine["team"];
            leagueObj = (JObject)statLine["league"];
            seasonObj = (JObject)statLine["season"];
            playerId = GetId();
        }

        // Stats from base object
        public string GetGameType()
        {
            return (string)statLine["gameType"];
        }

        public string GetGamesPlayed()
        {
            return (string)statLine["GP"];
        }

        public string GetGoalsOrGAA()
        {
            string goals = (string)statLine["G"];
            return (goals != null) ? goals : (string)statLine["GAA"];
        }

        public string GetAssists()
        {
            string assists = (string)statLine["A"];
            return (assists != null) ? assists : (string)statLine["SVP"];
        }

        public string GetTotalPoints()
        {
            return (string)statLine["TP"];
        }

        public string GetPointsPerGame()
        {
            return (string)statLine["PPG"];
        }

        public string GetPIM()
        {
            return (string)statLine["PIM"];
        }

        // Stats from player object
        public string GetFirstName()
        {
            return (string)playerObj["firstName"];
        }

        public string GetLastName()
        {
            return (string)playerObj["lastName"];
        }

        public string GetDateOfBirth()
        {
            return (string)playerObj["dateOfBirth"];
        }

        public string GetHeight()
        {
            return (string)playerObj["height"];
        }

        public string GetWeight()
        {
            return (string)playerObj["weight"];
        }

        public string GetShoots()
        {
            return (string)playerObj["shoots"];
        }

        private string GetId()
        {
            return (string)playerObj["id"];
        }

        // Stats from team object
        public string GetTeamName()
        {
            return (string)teamObj["name"];
        }

        // Stats from league object
        public string GetLeagueName()
        {
            return (string)leagueObj["name"];
        }

        // Stats from season object
        public string GetYear()
        {
            return (string)seasonObj["name"];
        }
    }
}
