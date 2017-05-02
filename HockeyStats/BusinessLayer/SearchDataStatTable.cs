using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class SearchDataStatTable : PlayerStatTable
    {
        private static SearchDataParser searchDataParser = new SearchDataParser();

        public SearchDataStatTable(DataGridView dataGridView, List<string> columnNames)
            : base(dataGridView, columnNames)
        {
        }

        public void SearchPlayer(string playerName)
        {
            JObject statsJson = EliteProspectsAPI.SearchPlayer(playerName);
            foreach (JToken searchData in statsJson["players"]["data"])
            {
                searchDataParser.SetSearchData(searchData);
                Dictionary<string, string> dict = new Dictionary<string, string>();
                searchDataParser.SetDictionaryToFill(dict);

                searchDataParser.GetFirstName(Constants.FIRST_NAME);
                searchDataParser.GetLastName(Constants.LAST_NAME);
                searchDataParser.GetDateOfBirth(Constants.DATE_OF_BIRTH);
                searchDataParser.GetLatestSeason(Constants.LATEST_SEASON);
                searchDataParser.GetLatestTeam(Constants.LATEST_TEAM);

                AddRowToDataTable(dict);
            }
        }
    }
}
