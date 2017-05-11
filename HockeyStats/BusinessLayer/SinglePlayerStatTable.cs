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
        private string seasonType;

        public SinglePlayerStatTable(DataGridView dataGridView, List<string> columnData, string seasonType)
            : base(dataGridView, columnData)
        {
            try
            {
                dataGridView.Sort(dataGridView.Columns[Constants.SEASON], System.ComponentModel.ListSortDirection.Ascending);
            }
            catch (Exception) { }

            this.seasonType = seasonType;
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
        }

        public void AddPlayerByPlayerStats(PlayerStats playerStats)
        {
            this.playerStats = playerStats;
            UpdateRowData();
        }

        public PlayerStats GetPlayerStats()
        {
            return playerStats;
        }

        public override string GetSeasonType()
        {
            return seasonType;
        }

        public override void SetSeasonType(string newSeasonType)
        {
            seasonType = newSeasonType;
            if (playerStats != null) { UpdateRowData(); }
        }

        public void ClearTable()
        {
            dataTable.Clear();
        }

        private void UpdateRowData()
        {
            dataTable.Rows.Clear();
            List<Dictionary<string, string>> dynamicColumnValues = playerStats.GetDynamicColumnValues(seasonType);
            foreach (Dictionary<string, string> dictOfDynamicColumnValues in dynamicColumnValues)
            {
                AddRowToDataTable(dictOfDynamicColumnValues);
            }
        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView.ClearSelection();
        }
    }
}
