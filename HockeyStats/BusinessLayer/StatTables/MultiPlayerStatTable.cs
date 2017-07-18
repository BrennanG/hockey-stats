using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HockeyStats
{
    public class MultiPlayerStatTable : PlayerStatTable
    {
        private PlayerList playerList;
        private Dictionary<int, PlayerStats> rowHashToPlayerStatsMap = new Dictionary<int, PlayerStats>();
        private Dictionary<string, HashSet<string>> leaguesInTableBySeason = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, HashSet<string>> teamsInTableBySeason = new Dictionary<string, HashSet<string>>();
        private Thread fillDataTableThread;

        // Needs to be a class, because you can only lock on instances of classes
        public class AbortThread { public bool abort; };
        public AbortThread abortThread = new AbortThread() { abort = false };
        
        public MultiPlayerStatTable(DataGridView dataGridView, PlayerList playerList)
            : base(dataGridView, playerList.primaryColumnNames)
        {
            this.playerList = playerList;

            // Fill the table in a separate thread so the GUI will be displayed while the data is loading
            fillDataTableThread = new Thread(() => FillDataTable(playerList.playerIds));
            fillDataTableThread.Start();
        }

        public void AddRow(string playerId)
        {
            AddPlayerById(playerId);
            playerList.AddPlayer(playerId);
        }

        public void RemoveRow(DataRow row)
        {
            dataTable.Rows.Remove(row);

            PlayerStats playerStats = GetPlayerStatsFromRow(row);
            string playerId = playerStats.GetPlayerId();
            playerList.RemovePlayer(playerId);
        }

        public void AddColumn(string columnName, string season)
        {
            if (!Constants.AllPossibleColumns.Contains(columnName) || dataTable.Columns.Contains(columnName)) { return; }

            base.AddColumn(columnName);
            playerList.AddPrimaryColumn(columnName);
            foreach (DataGridViewRow dgvRow in dataGridView.Rows)
            {
                DataRow row = GetDataRowFromDGVRow(dgvRow);
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                row[columnName] = playerStats.GetCollapsedColumnValue(season, columnName, playerList.primarySeasonType);
            }
        }

        public new void RemoveColumn(string columnName)
        {
            base.RemoveColumn(columnName);
            playerList.RemovePrimaryColumn(columnName);
        }

        public void ApplyFilter(Filter filter)
        {
            foreach (DataGridViewRow dgvRow in dataGridView.Rows)
            {
                DataRow row = GetDataRowFromDGVRow(dgvRow);
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];

                foreach (string column in Constants.DynamicColumns)
                {
                    try
                    {
                        row[column] = playerStats.GetCollapsedColumnValue(playerList.displaySeason, column, playerList.primarySeasonType, filter);
                    }
                    catch { }
                }
            }
        }

        public void ChangeDisplaySeason(string displaySeason)
        {
            playerList.SetDisplaySeason(displaySeason);
            UpdateRowData();
        }

        public override string GetSeasonType()
        {
            return playerList.primarySeasonType;
        }

        public override void SetSeasonType(string newSeasonType)
        {
            playerList.SetPrimarySeasonType(newSeasonType);
            UpdateRowData();
        }

        public PlayerStats GetPlayerStatsFromRow(DataRow dataRow)
        {
            if (rowHashToPlayerStatsMap.ContainsKey(dataRow.GetHashCode()))
            {
                return rowHashToPlayerStatsMap[dataRow.GetHashCode()];
            }
            else
            {
                return null;
            }
        }

        public List<string> GetLeaguesBySeason(string season)
        {
            return leaguesInTableBySeason.ContainsKey(season) ? leaguesInTableBySeason[season].ToList() : new List<string>();
        }

        public List<string> GetTeamsBySeason(string season)
        {
            return teamsInTableBySeason.ContainsKey(season) ? teamsInTableBySeason[season].ToList() : new List<string>();
        }

        public void AbortFillDataTableThread()
        {
            lock (abortThread)
            {
                abortThread.abort = true;
            }
        }

        public bool ThreadIsRunning()
        {
            return fillDataTableThread.ThreadState == ThreadState.Running;
        }

        private void AddPlayerById(string playerId)
        {
            PlayerStats playerStats = new PlayerStats(playerId);

            Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(playerList.displaySeason, playerList.primarySeasonType);
            DataRow newDataRow = AddRowToDataTable(collapsedYear);
            rowHashToPlayerStatsMap[newDataRow.GetHashCode()] = playerStats;

            Dictionary<string, HashSet<string>> leaguesBySeason = playerStats.GetLeaguesBySeason();
            foreach (string season in leaguesBySeason.Keys)
            {
                if (!leaguesInTableBySeason.ContainsKey(season))
                {
                    leaguesInTableBySeason[season] = leaguesBySeason[season];
                }
                else
                {
                    leaguesInTableBySeason[season].UnionWith(leaguesBySeason[season]);
                }
            }

            Dictionary<string, HashSet<string>> teamsBySeason = playerStats.GetTeamsBySeason();
            foreach (string season in teamsBySeason.Keys)
            {
                if (!teamsInTableBySeason.ContainsKey(season))
                {
                    teamsInTableBySeason[season] = teamsBySeason[season];
                }
                else
                {
                    teamsInTableBySeason[season].UnionWith(teamsBySeason[season]);
                }
            }
        }

        private void UpdateRowData()
        {
            DataRow[] copyOfRows = new DataRow[dataTable.Rows.Count];
            dataTable.Rows.CopyTo(copyOfRows, 0);
            foreach (DataRow row in copyOfRows)
            {
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(playerList.displaySeason, playerList.primarySeasonType);
                foreach (DataColumn column in dataTable.Columns)
                {
                    string columnName = column.ColumnName;
                    if (Constants.NumericColumns.Contains(columnName))
                    {
                        row[columnName] = (collapsedYear.ContainsKey(columnName) && collapsedYear[columnName] != null) ? (object)Convert.ToInt32(collapsedYear[columnName]) : DBNull.Value;
                    }
                    else
                    {
                        row[columnName] = (collapsedYear.ContainsKey(columnName) && collapsedYear[columnName] != null) ? collapsedYear[columnName] : String.Empty;
                    }
                }
            }
        }

        private void FillDataTable(List<string> playerIds)
        {
            lock (abortThread)
            {
                abortThread.abort = false;
            }
            string[] copyOfPlayerIds = new string[playerIds.Count];
            playerIds.CopyTo(copyOfPlayerIds);
            foreach (string playerId in copyOfPlayerIds)
            {
                AddPlayerById(playerId);
                lock(abortThread)
                {
                    if (abortThread.abort)
                    {
                        break;
                    }
                }
            }
        }
    }
}
