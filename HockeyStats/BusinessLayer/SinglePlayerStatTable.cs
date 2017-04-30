using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class SinglePlayerStatTable : PlayerStatTable
    {
        private PlayerStats playerStats;

        public SinglePlayerStatTable(DataGridView dataGridView, List<string> columnData)
            : base(dataGridView, columnData)
        {
            try
            {
                dataGridView.Sort(dataGridView.Columns[Constants.SEASON], System.ComponentModel.ListSortDirection.Ascending);
            }
            catch (Exception) { }
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
        }

        public void AddPlayerByPlayerStats(PlayerStats playerStats, string seasonType)
        {
            this.playerStats = playerStats;
            UpdateRowData(seasonType);
        }

        public void ChangeSeasonType(string seasonType)
        {
            UpdateRowData(seasonType);
        }

        public PlayerStats GetPlayerStats()
        {
            return playerStats;
        }

        private void UpdateRowData(string seasonType)
        {
            dataTable.Rows.Clear();
            List<Dictionary<string, string>> dynamicColumnValues = playerStats.GetDynamicColumnValues(seasonType);
            foreach (Dictionary<string, string> dictOfDynamicColumnValues in dynamicColumnValues)
            {
                AddRowToDataTable(dictOfDynamicColumnValues);
            }
        }

        public void ClearTable()
        {
            dataTable.Clear();
        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView.ClearSelection();
        }
    }
}
