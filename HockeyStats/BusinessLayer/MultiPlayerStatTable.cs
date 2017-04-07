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
        private Thread fillDataTableThread;

        public MultiPlayerStatTable(DataGridView dataGridView, PlayerList playerList)
            : base(dataGridView, playerList.primaryColumnNames)
        {
            this.playerList = playerList;

            // Fill the table in a separate thread so the GUI will be displayed while the data is loading
            fillDataTableThread = new Thread(() => FillDataTable());
            fillDataTableThread.Start();
        }

        public void AddPlayerById(string playerId, string season)
        {
            PlayerStats playerStats = new PlayerStats(playerId);

            Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(season);
            DataRow newDataRow = AddRowToDataTable(collapsedYear);
            rowHashToPlayerStatsMap[newDataRow.GetHashCode()] = playerStats;
        }

        public void ChangeDisplaySeason(string season)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(season);
                foreach (DataColumn column in dataTable.Columns)
                {
                    string columnName = column.ColumnName;
                    row[columnName] = (collapsedYear.ContainsKey(columnName)) ? collapsedYear[columnName] : String.Empty;
                }
            }
        }

        public void RemoveRow(DataRow row)
        {
            PlayerStats playerStats = GetPlayerStatsFromRow(row);
            dataTable.Rows.Remove(row);
        }

        public void AddColumn(string columnName, string season)
        {
            if (!Constants.AllPossibleColumns.Contains(columnName) || dataTable.Columns.Contains(columnName)) { return; }

            base.AddColumn(columnName);
            foreach (DataGridViewRow dgvRow in dataGridView.Rows)
            {
                DataRow row = GetDataRowFromDGVRow(dgvRow);
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                row[columnName] = playerStats.GetCollapsedColumnValue(season, columnName);
            }
        }

        public new void RemoveColumn(string columnName)
        {
            base.RemoveColumn(columnName);
        }

        public void SetCollectionChangedHandler(System.ComponentModel.CollectionChangeEventHandler handler)
        {
            dataTable.Columns.CollectionChanged += handler;
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

        public static DataRow GetDataRowFromDGVRow(DataGridViewRow dgvRow)
        {
            return ((DataRowView)dgvRow.DataBoundItem).Row;
        }

        private void FillDataTable()
        {
            string[] copiedPlayerIds = new string[playerList.playerIds.Count];
            playerList.playerIds.CopyTo(copiedPlayerIds);
            foreach (string playerId in copiedPlayerIds)
            {
                AddPlayerById(playerId, playerList.displaySeason);
            }
        }
    }
}
