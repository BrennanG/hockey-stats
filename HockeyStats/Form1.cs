using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace HockeyStats
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string displayYear = "2016-2017";
            List<string> playerIds = new List<string> { "156923", "108659" };
            Dictionary<string, string> playerDict = new Dictionary<string, string>();

            foreach (string playerId in playerIds)
            {
                string jsonData = GetEPStats(playerId);
                playerDict.Add(playerId, jsonData);
                JObject results = JObject.Parse(jsonData);

                foreach (JToken statLine in results["data"])
                {
                    string gameType = (string)statLine["gameType"];
                    JObject season = (JObject)statLine["season"];
                    string seasonName = (string)season["name"];
                    if (gameType == "REGULAR_SEASON" && seasonName == displayYear) {
                        string gamesPlayed = GetGamesPlayed(statLine);
                        string goals = GetGoals(statLine);
                        string assists = GetAssists(statLine);
                        string totalPoints = GetTotalPoints(statLine);
                        string firstName = GetFirstName(statLine);
                        string lastName = GetLastName(statLine);
                        string teamName = GetTeamName(statLine);

                        System.Diagnostics.Debug.Write(String.Format("{5} {6} - team: {4}, gamesPlayed: {0}, goals: {1}, assists: {2}, points: {3}\n", gamesPlayed, goals, assists, totalPoints, teamName, firstName, lastName));
                    }
                }
            }

            FillTable(dataGridView1);
        }

        static string GetEPStats(string playerId)
        {
            WebRequest request = WebRequest.Create("http://api.eliteprospects.com:80/beta/players/" + playerId + "/stats");
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string jsonData = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return jsonData;
        }

        static void FillTable(DataGridView dataGridView)
        {
            // Columns
            List<DataColumn> columns = new List<DataColumn>();
            columns.Add(new DataColumn("First Name"));
            columns.Add(new DataColumn("Last Name"));
            columns.Add(new DataColumn("Games Played"));

            // Rows
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            rows.Add(new Dictionary<string, string>
            {
                { "First Name", "Brennan" },
                { "Last Name", "Govreau" }
            });
            rows.Add(new Dictionary<string, string>
            {
                { "First Name", "Jordan" },
                { "Games Played", "1" }
            });

            // Convert to DataTable.
            DataTable table = ConvertListToDataTable(columns, rows);
            dataGridView.DataSource = table;
        }

        static DataTable ConvertListToDataTable(List<DataColumn> columns, List<Dictionary<string, string>> rows)
        {
            DataTable table = new DataTable();

            // Add columns
            for (int i = 0; i < columns.Count; i++)
            {
                table.Columns.Add(columns[i]);
            }

            // Add rows
            foreach (Dictionary<string, string> dictOfRows in rows)
            {
                string[] rowValues = new string[columns.Count];
                for (int i = 0; i < columns.Count; i++)
                {
                    string outString = String.Empty;
                    // If the column exists in the row, add the value
                    if (dictOfRows.TryGetValue(columns[i].ColumnName, out outString))
                    {
                        rowValues[i] = dictOfRows[columns[i].ColumnName];
                    }
                }
                table.Rows.Add(rowValues);
            }

            return table;
        }

        static string GetGoals(JToken statLine)
        {
            return (string)statLine["G"];
        }

        static string GetFirstName(JToken statLine)
        {
            JObject player = (JObject)statLine["player"];
            return (string)player["firstName"];
        }

        static string GetLastName(JToken statLine)
        {
            JObject player = (JObject)statLine["player"];
            return (string)player["lastName"];
        }

        static string GetAssists(JToken statLine)
        {
            return (string)statLine["A"];
        }

        static string GetTeamName(JToken statLine)
        {
            JObject team = (JObject)statLine["team"];
            return (string)team["name"];
        }

        static string GetLeagueName(JToken statLine)
        {
            JObject league = (JObject)statLine["league"];
            return (string)league["name"];
        }

        static string GetTotalPoints(JToken statLine)
        {
            return (string)statLine["TP"];
        }

        static string GetGamesPlayed(JToken statLine)
        {
            return (string)statLine["GP"];
        }

        static string GetPointsPerGame(JObject statLine)
        {
            return (string)statLine["PPG"];
        }
    }
}
