using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class PlayerStatTablesManager
    {
        public PlayerStatForm form { get; set; }
        public DataGridView topTableDGV { get; set; }
        public DataGridView leftTableDGV { get; set; }
        public DataGridView middleTableDGV { get; set; }
        public DataGridView rightTableDGV { get; set; }

        public void Initialize()
        {
            SetupShowSelectedPlayer();
            SetupUnshowUnselectedPlayer();
            SetupColumnResizeListener();
        }

        private void SetupShowSelectedPlayer()
        {
            Action<PlayerStats> ShowSelectedPlayer = new Action<PlayerStats>((PlayerStats playerStats) => {
                form.middleTable.ClearTable();
                form.middleTable.AddPlayerByPlayerStats(playerStats);

                form.rightTable.ClearTable();
                form.rightTable.AddPlayerByPlayerStats(playerStats);

                form.HighlightDraftRowsInRightTable(playerStats);
                form.rowJustSelected = true;
            });

            topTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                if (topTableDGV.SelectedRows.Count != 1 || topTableDGV.Rows.Count < 1) { return; }
                leftTableDGV.ClearSelection();

                int rowIndex = topTableDGV.SelectedRows[0].Index;
                DataRow row = PlayerStatTable.GetDataRowFromDGVRow(topTableDGV.Rows[rowIndex]);
                PlayerStats existingPlayerStats = form.topTable.GetPlayerStatsFromRow(row);
                if (existingPlayerStats == null) { return; }

                ShowSelectedPlayer(existingPlayerStats);
            });

            leftTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                if (leftTableDGV.SelectedRows.Count != 1 || leftTableDGV.Rows.Count < 1) { return; }
                topTableDGV.ClearSelection();

                int rowIndex = leftTableDGV.SelectedRows[0].Index;
                DataGridViewRow row = leftTableDGV.Rows[rowIndex];
                string playerId = form.leftTable.GetPlayerIdFromRow(row);
                PlayerStats searchedPlayerStats = new PlayerStats(playerId);

                ShowSelectedPlayer(searchedPlayerStats);
            });
        }

        private void SetupUnshowUnselectedPlayer()
        {
            Action<DataGridView, int, MouseButtons> UnselectPlayer = new Action<DataGridView, int, MouseButtons>((DataGridView dataGridView, int rowIndex, MouseButtons button) => {
                if (dataGridView.SelectedRows.Count != 1 || form.rowJustSelected)
                {
                    form.rowJustSelected = false;
                    return;
                }

                if (button == MouseButtons.Left && dataGridView.SelectedRows[0].Index == rowIndex)
                {
                    form.ClearPlayerSelection();
                    form.rowJustSelected = false;
                }
            });

            topTableDGV.CellMouseUp += new DataGridViewCellMouseEventHandler((object sender, DataGridViewCellMouseEventArgs e) =>
            {
                UnselectPlayer(topTableDGV, e.RowIndex, e.Button);
            });

            leftTableDGV.CellMouseUp += new DataGridViewCellMouseEventHandler((object sender, DataGridViewCellMouseEventArgs e) =>
            {
                UnselectPlayer(leftTableDGV, e.RowIndex, e.Button);
            });
        }

        private void SetupColumnResizeListener()
        {
            MouseEventHandler eventHandler = new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                form.tableHasBeenClicked = true;
            });

            topTableDGV.MouseDown += eventHandler;
            leftTableDGV.MouseDown += eventHandler;
            middleTableDGV.MouseDown += eventHandler;
            rightTableDGV.MouseDown += eventHandler;

            Action HandleResize = () =>
            {
                if (form.tableHasBeenClicked)
                {
                    form.SetListIsSaved(false);
                }
            };

            topTableDGV.ColumnWidthChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            {
                HandleResize();
            });

            //rightTableDGV.ColumnWidthChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            //{
            //    HandleResize();
            //});
        }
    }
}
