using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
            SetupRightClickListener();
        }

        public void ClearPlayerSelection()
        {
            topTableDGV.ClearSelection();
            leftTableDGV.ClearSelection();
            form.middleTable.ClearTable();
            form.rightTable.ClearTable();
        }

        public void RedrawColumnWidths(DataGridView dataGridView, Func<string, int> GetWidthFunction, Action<DataGridViewColumnCollection> SetWidthsFunction)
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
            SetWidthsFunction(dataGridView.Columns);
        }

        public void RedrawRowColors()
        {
            if (form.topTable.GetSeasonType() == Constants.REGULAR_SEASON)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (form.topTable.GetSeasonType() == Constants.PLAYOFFS)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightSteelBlue;
            }

            if (form.rightTable.GetSeasonType() == Constants.REGULAR_SEASON)
            {
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (form.rightTable.GetSeasonType() == Constants.PLAYOFFS)
            {
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightSteelBlue;
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightSteelBlue;
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

                if (form.leftTable.searchType == SearchDataStatTable.SearchType.Player)
                {
                    int rowIndex = leftTableDGV.SelectedRows[0].Index;
                    DataGridViewRow row = leftTableDGV.Rows[rowIndex];
                    string playerId = form.leftTable.GetIdFromRow(row);
                    PlayerStats searchedPlayerStats = new PlayerStats(playerId);

                    ShowSelectedPlayer(searchedPlayerStats);
                }
                else if (form.leftTable.searchType == SearchDataStatTable.SearchType.Team)
                {
                    int rowIndex = leftTableDGV.SelectedRows[0].Index;
                    DataGridViewRow row = leftTableDGV.Rows[rowIndex];
                    string teamId = form.leftTable.GetIdFromRow(row);

                    form.middleTable.ClearTable();
                    form.rightTable.ClearTable();
                    form.rowJustSelected = true;
                }
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
            topTableDGV.MouseDown += new MouseEventHandler((object sender, MouseEventArgs e) =>
            {
                tableHasBeenClicked = true;
            });
        }

        private void SetupColumnResizeListener()
        {
            topTableDGV.ColumnWidthChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            {
                if (tableHasBeenClicked && topTableDGV.Columns.Count == form.currentPlayerList.primaryColumnNames.Count)
                {
                    form.currentPlayerList.SetPrimaryColumnWidths(topTableDGV.Columns);
                    form.SetListStatus(PlayerList.ListStatus.Unsaved);
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

                if (tableHasBeenClicked && rightTableDGV.Columns.Count == form.currentPlayerList.secondaryColumnNames.Count)
                {
                    form.currentPlayerList.SetSecondaryColumnWidths(rightTableDGV.Columns);
                    form.SetListStatus(PlayerList.ListStatus.Unsaved);
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
                    form.SetListStatus(PlayerList.ListStatus.Unsaved);
                    tableHasBeenClicked = false;
                }
            });

            rightTableDGV.ColumnDisplayIndexChanged += new DataGridViewColumnEventHandler((object sender, DataGridViewColumnEventArgs e) =>
            {
                if (tableHasBeenClicked && rightTableDGV.Columns.Count == form.currentPlayerList.secondaryColumnNames.Count)
                {
                    form.currentPlayerList.SetSecondaryColumnNames(rightTableDGV.Columns);
                    form.SetListStatus(PlayerList.ListStatus.Unsaved);
                    tableHasBeenClicked = false;
                }
            });
        }

        private void SetupRightClickListener()
        {
            Action<DataGridView, Func<DataRow, PlayerStats>> HandleClick = new Action<DataGridView, Func<DataRow, PlayerStats>>((DataGridView dgv, Func<DataRow, PlayerStats> PlayerStatsGetter) => {
                dgv.MouseClick += new MouseEventHandler((object sender, MouseEventArgs e) =>
                {
                    if (e.Button != MouseButtons.Right) { return; }

                    int row = dgv.HitTest(e.X, e.Y).RowIndex;
                    int column = dgv.HitTest(e.X, e.Y).ColumnIndex;
                    if (row < 0) { return; }

                    // If a cell in the "Team" column is right clicked
                    if (dgv.Columns.Contains(Constants.TEAM) && column == dgv.Columns[Constants.TEAM].Index)
                    {
                        OpenLoadTeamsRightClickMenu(dgv, row, column, PlayerStatsGetter, e);
                    }
                    // If a cell in the "First Name" or "Last Name" column is right clicked
                    else if ((dgv.Columns.Contains(Constants.FIRST_NAME) && column == dgv.Columns[Constants.FIRST_NAME].Index)
                        || (dgv.Columns.Contains(Constants.LAST_NAME) && column == dgv.Columns[Constants.LAST_NAME].Index))
                    {
                        OpenAddPlayerToListRightClickMenu(dgv, row, PlayerStatsGetter, e);
                    }
                });
            });

            HandleClick(topTableDGV, delegate(DataRow dataRow)
            {
                return form.topTable.GetPlayerStatsFromRow(dataRow);
            });
            HandleClick(rightTableDGV, delegate (DataRow dataRow)
            {
                return form.rightTable.GetPlayerStats();
            });
        }

        private void OpenLoadTeamsRightClickMenu(DataGridView dgv, int row, int column, Func<DataRow, PlayerStats> PlayerStatsGetter, MouseEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            string[] teamNames = dgv[column, row].Value.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (teamNames.Count() == 1 && String.IsNullOrWhiteSpace(teamNames[0])) { return; }
            foreach (string teamName in teamNames)
            {
                string season = (dgv.Columns.Contains(Constants.SEASON)) ? dgv.Rows[row].Cells[Constants.SEASON].Value.ToString() : form.currentPlayerList.displaySeason;
                string listName = teamName + " (" + season + ")";
                EventHandler eventHandler = new EventHandler((object sender2, EventArgs e2) =>
                {
                    Action loadTeam = () =>
                    {
                        DataRow dataRow = PlayerStatTable.GetDataRowFromDGVRow(dgv.Rows[row]);
                        PlayerStats playerStats = PlayerStatsGetter(dataRow);
                        string teamId = playerStats.GetTeamId(season, teamName);

                        List<string> playerIds = TeamListManager.GetPlayerIdsOnTeam(teamId, season);
                        PlayerList playerList = new PlayerList();
                        playerList.FillWithDefaults();
                        playerList.SetListType(PlayerList.ListType.TeamList);
                        playerList.SetTeamId(teamId);
                        playerList.SetPlayerIds(playerIds);
                        playerList.SetDisplaySeason(season);
                        playerList.listName = listName;
                        form.LoadPlayerList(playerList);
                    };
                    form.TriggerLeaveRequest(loadTeam);
                });
                MenuItem item = new MenuItem(listName, eventHandler);
                menu.MenuItems.Add(item);
            }

            menu.Show(dgv, new Point(e.X, e.Y));
        }

        private void OpenAddPlayerToListRightClickMenu(DataGridView dgv, int row, Func<DataRow, PlayerStats> PlayerStatsGetter, MouseEventArgs e)
        {
            ContextMenu menu = new ContextMenu();
            string[] listNames = form.GetPlayerListsInDirectory();
            if (listNames.Count() < 1) { return; }
            foreach (string listName in listNames)
            {
                if (listName == form.currentPlayerList.listName) { return; }

                DataRow dataRow = PlayerStatTable.GetDataRowFromDGVRow(dgv.Rows[row]);
                PlayerStats playerStats = PlayerStatsGetter(dataRow);
                Dictionary<string, string> columnValues = playerStats.GetConstantColumnValues();
                string firstName = (columnValues.ContainsKey(Constants.FIRST_NAME)) ? columnValues[Constants.FIRST_NAME] : String.Empty;
                string lastName = (columnValues.ContainsKey(Constants.LAST_NAME)) ? columnValues[Constants.LAST_NAME] : String.Empty;

                EventHandler eventHandler = new EventHandler((object sender2, EventArgs e2) =>
                {
                    string playerId = playerStats.GetPlayerId();

                    PlayerList playerList = Serializer.ReadXML<PlayerList>(listName);
                    playerList.AddPlayer(playerId);
                    Serializer.WriteXML(playerList, listName);
                    MessageBox.Show(String.Format("{0} {1} was added to list: {2}", firstName, lastName, listName));
                });
                MenuItem item = new MenuItem(String.Format("Add {0} {1} to list: {2}", firstName, lastName, listName), eventHandler);
                menu.MenuItems.Add(item);
            }

            menu.Show(dgv, new Point(e.X, e.Y));
        }
    }
}
