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
        private List<string> playerIds;
        private string seasonType;
        private string displaySeason;
        private Dictionary<int, PlayerStats> rowHashToPlayerStatsMap = new Dictionary<int, PlayerStats>();
        private Thread fillDataTableThread;

        public MultiPlayerStatTable(DataGridView dataGridView, List<string> columnNames, List<string> playerIds, string displaySeason, string seasonType)
            : base(dataGridView, columnNames)
        {
            this.playerIds = playerIds;
            this.seasonType = seasonType;
            this.displaySeason = displaySeason;

            // Fill the table in a separate thread so the GUI will be displayed while the data is loading
            fillDataTableThread = new Thread(() => FillDataTable(playerIds));
            fillDataTableThread.Start();
        }

        public void AddRow(string playerId)
        {
            AddPlayerById(playerId);
            playerIds.Add(playerId);
        }

        public void RemoveRow(DataRow row)
        {
            dataTable.Rows.Remove(row);

            PlayerStats playerStats = GetPlayerStatsFromRow(row);
            string playerId = playerStats.GetPlayerId();
            playerIds.Remove(playerId);
        }

        public void AddColumn(string columnName, string season)
        {
            if (!Constants.AllPossibleColumns.Contains(columnName) || dataTable.Columns.Contains(columnName)) { return; }

            base.AddColumn(columnName);
            foreach (DataGridViewRow dgvRow in dataGridView.Rows)
            {
                DataRow row = GetDataRowFromDGVRow(dgvRow);
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                row[columnName] = playerStats.GetCollapsedColumnValue(season, columnName, seasonType);
            }
        }

        public new void RemoveColumn(string columnName)
        {
            base.RemoveColumn(columnName);
        }

        public void ChangeDisplaySeason(string displaySeason)
        {
            this.displaySeason = displaySeason;
            UpdateRowData();
        }

        public void ChangeSeasonType(string seasonType)
        {
            this.seasonType = seasonType;
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

        public List<string> GetPlayerIds()
        {
            //List<string> playerIds = new List<string>();
            //foreach (DataRow row in dataTable.Rows)
            //{
            //    PlayerStats playerStats = GetPlayerStatsFromRow(row);
            //    if (playerStats == null) { continue; }
            //    playerIds.Add(playerStats.GetPlayerId());
            //}
            return playerIds;
        }

        public void AbortFillDataTableThread()
        {
            if (fillDataTableThread != null && fillDataTableThread.ThreadState == ThreadState.Running)
            {
                fillDataTableThread.Abort();
            }
            while (fillDataTableThread.ThreadState != ThreadState.Aborted && fillDataTableThread.ThreadState != ThreadState.Stopped)
            {
                // loop until it's aborted
            }
        }

        public bool ThreadIsRunning()
        {
            return fillDataTableThread.ThreadState == ThreadState.Running;
        }

        private void AddPlayerById(string playerId)
        {
            PlayerStats playerStats = new PlayerStats(playerId);

            Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(displaySeason, seasonType);
            DataRow newDataRow = AddRowToDataTable(collapsedYear);
            rowHashToPlayerStatsMap[newDataRow.GetHashCode()] = playerStats;
        }

        private void UpdateRowData()
        {
            DataRow[] copyOfRows = new DataRow[dataTable.Rows.Count];
            dataTable.Rows.CopyTo(copyOfRows, 0);
            foreach (DataRow row in copyOfRows)
            {
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(displaySeason, seasonType);
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
            string[] copyOfPlayerIds = new string[playerIds.Count];
            playerIds.CopyTo(copyOfPlayerIds);
            foreach (string playerId in copyOfPlayerIds)
            {
                AddPlayerById(playerId);
            }
        }
    }
}
