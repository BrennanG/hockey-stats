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

        public MultiPlayerStatTable(DataGridView dataGridView, PlayerList playerList)
            : base(dataGridView, playerList.primaryTableColumnNames)
        {
            this.playerList = playerList;
            new Thread(() => FillDataTable()).Start(); // Fill the table in a separate thread
        }

        public void AddPlayerById(string playerId)
        {
            PlayerStats playerStats = new PlayerStats(playerId);

            Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(playerList.displayYears.First());
            DataRow newDataRow = AddRowToDataTable(collapsedYear);
            rowHashToPlayerStatsMap[newDataRow.GetHashCode()] = playerStats;
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

        public new void AddColumn(string columnName)
        {
            if (!Columns.AllPossibleColumns.Contains(columnName) || dataTable.Columns.Contains(columnName)) { return; }

            if (Columns.NumericColumns.Contains(columnName))
            {
                dataTable.Columns.Add(new DataColumn(columnName, new int().GetType()));
            }
            else
            {
                dataTable.Columns.Add(new DataColumn(columnName));
            }

            foreach (DataGridViewRow dgvRow in dataGridView.Rows)
            {
                DataRow row = GetDataRowFromDGVRow(dgvRow);
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                row[columnName] = playerStats.GetCollapsedColumnValue(playerList.displayYears.First(), columnName);
            }
        }

        public static DataRow GetDataRowFromDGVRow(DataGridViewRow dgvRow)
        {
            return ((DataRowView)dgvRow.DataBoundItem).Row;
        }

        private void FillDataTable()
        {
            foreach (string playerId in playerList.playerIds)
            {
                AddPlayerById(playerId);
            }
        }
    }
}
