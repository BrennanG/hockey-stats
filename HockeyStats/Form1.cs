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
            Dictionary<string, JObject> playerDict = new Dictionary<string, JObject>();
            List<Dictionary<string, string>> rowData = new List<Dictionary<string, string>>();

            foreach (string playerId in playerIds)
            {
                string jsonData = GetEPStats(playerId);
                JObject parsedJson = JObject.Parse(jsonData);
                playerDict.Add(playerId, parsedJson);

                Dictionary<string, string> currentDict = new Dictionary<string, string>();
                foreach (JToken statLine in parsedJson["data"])
                {
                    string gameType = (string)statLine["gameType"];
                    JObject season = (JObject)statLine["season"];
                    string seasonName = (string)season["name"];
                    if (gameType == "REGULAR_SEASON" && seasonName == displayYear) {
                        if (currentDict.Count == 0)
                        {
                            currentDict = new Dictionary<string, string>
                            {
                                { "First Name", GetFirstName(statLine) },
                                { "Last Name", GetLastName(statLine) },
                                { "Games Played", GetGamesPlayed(statLine) },
                                { "Goals", GetGoals(statLine) },
                                { "Assists", GetAssists(statLine) },
                                { "Total Points", GetTotalPoints(statLine) },
                                { "PPG", GetPointsPerGame(statLine) }
                            };
                            rowData.Add(currentDict);
                        }
                        else
                        {
                            currentDict["First Name"] += Environment.NewLine + GetFirstName(statLine);
                            currentDict["Last Name"] += Environment.NewLine + GetLastName(statLine);
                            currentDict["Games Played"] += Environment.NewLine + GetGamesPlayed(statLine);
                            currentDict["Goals"] += Environment.NewLine + GetGoals(statLine);
                            currentDict["Assists"] += Environment.NewLine + GetAssists(statLine);
                            currentDict["Total Points"] += Environment.NewLine + GetTotalPoints(statLine);
                            currentDict["PPG"] += Environment.NewLine + GetPointsPerGame(statLine);
                        }
                    }
                }
            }

            List<string> columnNames = new List<string> { "First Name", "Last Name", "Games Played", "Goals", "Assists", "Total Points", "PPG" };

            FillTable(dataGridView1, columnNames, rowData);
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

        static void FillTable(DataGridView dataGridView, List<string> columnNames, List<Dictionary<string, string>> rowData)
        {
            // Columns
            List<DataColumn> columns = new List<DataColumn>();
            foreach (string columnName in columnNames)
            {
                columns.Add(new DataColumn(columnName));
            }

            // Convert to DataTable.
            DataTable table = ConvertListToDataTable(columns, rowData);
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
            foreach (Dictionary<string, string> rowDict in rows)
            {
                string[] rowValues = new string[columns.Count];
                for (int i = 0; i < columns.Count; i++)
                {
                    string outString = String.Empty;
                    // If the column exists in the row, add the value
                    if (rowDict.TryGetValue(columns[i].ColumnName, out outString))
                    {
                        rowValues[i] = rowDict[columns[i].ColumnName];
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

        static string GetPointsPerGame(JToken statLine)
        {
            return (string)statLine["PPG"];
        }
    }
}
