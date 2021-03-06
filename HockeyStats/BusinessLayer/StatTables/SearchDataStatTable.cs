﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class SearchDataStatTable : PlayerStatTable
    {
        public enum SearchType { Player, Team }

        private List<Dictionary<string, string>> dictToIdMap;
        private static SearchDataParser searchDataParser = new SearchDataParser();

        public SearchType searchType = SearchType.Player;

        public SearchDataStatTable(DataGridView dataGridView, List<string> columnNames)
            : base(dataGridView, new List<string>())
        {
        }

        public bool DisplayPlayerSearch(string playerName)
        {
            searchType = SearchType.Player;
            ClearTable();
            AddColumns(Constants.DefaultSearchPlayerDataTableColumns);
            dataTable.DefaultView.Sort = String.Empty; // Reset the sorting incase a prior search was sorted
            dictToIdMap = new List<Dictionary<string, string>>();
            JObject statsJson = EliteProspectsAPI.SearchForPlayer(playerName);
            if (statsJson["players"] == null) { return false; }
            foreach (JToken searchData in statsJson["players"]["data"])
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                searchDataParser.SetSearchData(searchData);
                searchDataParser.SetDictionaryToFill(dict);

                searchDataParser.GetFirstName(Constants.FIRST_NAME);
                searchDataParser.GetLastName(Constants.LAST_NAME);
                searchDataParser.GetDateOfBirth(Constants.DATE_OF_BIRTH);
                searchDataParser.GetLatestSeason(Constants.LATEST_SEASON);
                searchDataParser.GetLatestTeam(Constants.LATEST_TEAM);
                searchDataParser.GetId(Constants.ID);

                dictToIdMap.Add(dict);
                AddRowToDataTable(dict);
            }
            return true;
        }

        public bool DisplayTeamSearch(string teamName)
        {
            searchType = SearchType.Team;
            ClearTable();
            AddColumns(Constants.DefaultSearchTeamDataTableColumns);
            dictToIdMap = new List<Dictionary<string, string>>();
            JObject statsJson = EliteProspectsAPI.SearchForTeam(teamName);
            if (statsJson["teams"] == null) { return false; }
            foreach (JToken searchData in statsJson["teams"]["data"])
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                searchDataParser.SetSearchData(searchData);
                searchDataParser.SetDictionaryToFill(dict);

                searchDataParser.GetLatestTeam(Constants.TEAM);
                searchDataParser.GetLatestLeague(Constants.LEAGUE);
                searchDataParser.GetId(Constants.ID);

                dictToIdMap.Add(dict);
                AddRowToDataTable(dict);
            }
            return true;
        }

        public string GetIdFromRow(DataGridViewRow row)
        {
            Dictionary<string, string> dict;
            if (searchType == SearchType.Player)
            {
                dict = dictToIdMap.Find(d =>
                    d[Constants.FIRST_NAME] == (string)row.Cells[Constants.FIRST_NAME].Value
                    && d[Constants.LAST_NAME] == (string)row.Cells[Constants.LAST_NAME].Value
                    && d[Constants.DATE_OF_BIRTH] == (string)row.Cells[Constants.DATE_OF_BIRTH].Value
                    && d[Constants.LATEST_SEASON] == (string)row.Cells[Constants.LATEST_SEASON].Value
                );
            }
            else
            {
                dict = dictToIdMap.Find(d =>
                    d[Constants.TEAM] == (string)row.Cells[Constants.TEAM].Value
                    && d[Constants.LEAGUE] == (string)row.Cells[Constants.LEAGUE].Value
                );
            }
            

            return dict[Constants.ID];
        }

        public void ClearTable()
        {
            dataTable.Clear();
            dataTable.Columns.Clear();
            dataTable.DefaultView.Sort = String.Empty;
        }
    }
}
