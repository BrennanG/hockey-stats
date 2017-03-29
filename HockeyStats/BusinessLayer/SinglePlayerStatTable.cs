using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class SinglePlayerStatTable : PlayerStatTable
    {
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

        public void AddPlayerByPlayerStats(PlayerStats playerStats)
        {
            List<Dictionary<string, string>> dynamicColumnValues = playerStats.GetDynamicColumnValues();
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
