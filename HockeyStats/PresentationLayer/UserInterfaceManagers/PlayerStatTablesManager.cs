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
        public bool tableHasBeenClicked { get; set; }

        public void Initialize()
        {
            SetupShowSelectedPlayer();
            SetupUnshowUnselectedPlayer();
            SetupListenForTableClick();
            SetupColumnResizeListener();
            SetupColumnReorderListener();
        }

        public void ClearPlayerSelection()
        {
            topTableDGV.ClearSelection();
            leftTableDGV.ClearSelection();
            form.middleTable.ClearTable();
            form.rightTable.ClearTable();
        }

        public void RedrawColumnWidths(DataGridView dataGridView, Func<string, int> GetWidthFunction)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                int width = GetWidthFunction(column.Name);
                if (column.DisplayIndex != dataGridView.Columns.Count - 1 && width >= 0)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    column.Width = width;
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        public void RedrawRowColors()
        {
            if (form.topTable.GetSeasonType() == Constants.REGULAR_SEASON)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (form.topTable.GetSeasonType() == Constants.PLAYOFFS)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
            }

            if (form.rightTable.GetSeasonType() == Constants.REGULAR_SEASON)
            {
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (form.rightTable.GetSeasonType() == Constants.PLAYOFFS)
            {
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
            }

            HighlightDraftRowsInRightTable(form.rightTable.GetPlayerStats());
        }

        public void HighlightDraftRowsInRightTable(PlayerStats playerStats)
        {
            if (playerStats == null) { return; }
            foreach (DataGridViewRow DGVRow in rightTableDGV.Rows)
            {
                string season = DGVRow.Cells[Constants.SEASON].Value.ToString();
                string endYear = (season != String.Empty) ? season.Substring(5) : String.Empty;
                if (endYear == playerStats.GetDraftYear())
                {
                    DGVRow.DefaultCellStyle.BackColor = System.Drawing.Color.DeepSkyBlue;
                }
                else if (endYear == playerStats.GetFirstYearOfDraftEligibility())
                {
                    DGVRow.DefaultCellStyle.BackColor = System.Drawing.Color.Turquoise;
                }
            }
        }

        private void SetupShowSelectedPlayer()
        {
            Action<PlayerStats> ShowSelectedPlayer = new Action<PlayerStats>((PlayerStats playerStats) => {
                form.middleTable.ClearTable();
                form.middleTable.AddPlayerByPlayerStats(playerStats);

                form.rightTable.ClearTable();
                form.rightTable.AddPlayerByPlayerStats(playerStats);

                HighlightDraftRowsInRightTable(playerStats);
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
                    ClearPlayerSelection();
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

        private void SetupListenForTableClick()
        {
            MouseEventHandler eventHandler = new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                tableHasBeenClicked = true;
            });

            topTableDGV.MouseDown += eventHandler;
            leftTableDGV.MouseDown += eventHandler;
            middleTableDGV.MouseDown += eventHandler;
            rightTableDGV.MouseDown += eventHandler;
        }

        private void SetupColumnResizeListener()
        {
            topTableDGV.ColumnWidthChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            {
                if (tableHasBeenClicked)
                {
                    form.currentPlayerList.SetPrimaryColumnWidths(topTableDGV.Columns);
                    form.SetListIsSaved(false);
                    tableHasBeenClicked = false;
                }
            });

            rightTableDGV.ColumnWidthChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            {
                // This stuff is necessary so when the scrollbar appears, the list is not made unsaved
                DataGridViewColumn secondLastCol = new DataGridViewColumn();
                foreach (DataGridViewColumn col in rightTableDGV.Columns)
                {
                    if (col.DisplayIndex == e.Column.DisplayIndex - 1)
                    {
                        secondLastCol = col;
                        break;
                    }
                }
                if (e.Column.DisplayIndex == rightTableDGV.Columns.Count - 1 && secondLastCol.Width == form.currentPlayerList.GetSecondaryColumnWidth(secondLastCol.Name))
                {
                    return;
                }

                if (tableHasBeenClicked)
                {
                    form.currentPlayerList.SetSecondaryColumnWidths(rightTableDGV.Columns);
                    form.SetListIsSaved(false);
                    tableHasBeenClicked = false;
                }
            });
        }

        private void SetupColumnReorderListener()
        {
            topTableDGV.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            {
                if (tableHasBeenClicked && topTableDGV.Columns.Count == form.currentPlayerList.primaryColumnNames.Count)
                {
                    form.currentPlayerList.SetPrimaryColumnNames(topTableDGV.Columns);
                    form.SetListIsSaved(false);
                    tableHasBeenClicked = false;
                }
            });

            rightTableDGV.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            {
                if (tableHasBeenClicked && rightTableDGV.Columns.Count == form.currentPlayerList.secondaryColumnNames.Count)
                {
                    form.currentPlayerList.SetSecondaryColumnNames(rightTableDGV.Columns);
                    form.SetListIsSaved(false);
                    tableHasBeenClicked = false;
                }
            });
        }
    }
}
