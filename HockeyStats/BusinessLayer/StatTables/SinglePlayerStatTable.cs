using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class SinglePlayerStatTable : PlayerStatTable
    {
        private PlayerList playerList;
        private PlayerStats playerStats;

        public SinglePlayerStatTable(DataGridView dataGridView, PlayerList playerList)
            : base(dataGridView, playerList.secondaryColumnNames)
        {
            try
            {
                dataGridView.Sort(dataGridView.Columns[Constants.SEASON], System.ComponentModel.ListSortDirection.Ascending);
            }
            catch (Exception) { }

            this.playerList = playerList;
            DisableCellSelection();
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
            return playerList.secondarySeasonType;
        }

        public override void SetSeasonType(string newSeasonType)
        {
            playerList.SetSecondarySeasonType(newSeasonType);
            UpdateRowData();
        }

        public void ClearTable()
        {
            playerStats = null;
            dataTable.Clear();
        }

        private void UpdateRowData()
        {
            if (playerStats == null) { return; }
            dataTable.Rows.Clear();
            List<Dictionary<string, string>> dynamicColumnValues = playerStats.GetDynamicColumnValues(playerList.secondarySeasonType);
            foreach (Dictionary<string, string> dictOfDynamicColumnValues in dynamicColumnValues)
            {
                AddRowToDataTable(dictOfDynamicColumnValues);
            }
        }

        private void DisableCellSelection()
        {
            dataGridView.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                dataGridView.ClearSelection();
            });
        }
    }
}
