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
        private Dictionary<int, Dictionary<string, string>> rowHashToSavedDictMap = new Dictionary<int, Dictionary<string, string>>();

        public MultiPlayerStatTable(DataGridView dataGridView, PlayerList playerList)
            : base(dataGridView, playerList.primaryTableColumnNames)
        {
            this.playerList = playerList;
            new Thread(() => FillDataTable()).Start(); // Fill the table in a separate thread
        }

        public void AddPlayerById(string playerId)
        {
            Dictionary<string, string> DisplayDict = new Dictionary<string, string>();
            Dictionary<string, string> SavedDict = new Dictionary<string, string>();

            PlayerStats playerStats = new PlayerStats(playerId, playerList.displayYears);
            playerStats.AddPlayerStatsToDicts(DisplayDict, SavedDict);

            DataRow newDataRow = AddRowToDataTable(DisplayDict);
            rowHashToSavedDictMap[newDataRow.GetHashCode()] = SavedDict;
        }

        public Dictionary<string, string> GetSavedDictFromRow(DataRow dataRow)
        {
            return rowHashToSavedDictMap[dataRow.GetHashCode()];
        }

        public new void AddColumn(string columnName)
        {
            if (!Columns.AllPossibleColumns.Contains(columnName) || dataTable.Columns.Contains(columnName)) { return; }

            dataTable.Columns.Add(columnName);
            foreach (DataGridViewRow dgvRow in dataGridView.Rows)
            {
                DataRow row = GetDataRowFromDGVRow(dgvRow);
                Dictionary<string, string> savedDictForRow = rowHashToSavedDictMap[row.GetHashCode()];
                string value;
                if (savedDictForRow.TryGetValue(columnName, out value))
                {
                    row[columnName] = value;
                }
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
