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
        
        private FilterManager filter;

        private Thread fillDataTableThread;
        public class AbortThread { public bool abort; }; // Needs to be a class, because you can only lock on instances of classes
        private AbortThread abortThread = new AbortThread() { abort = false };
        
        public MultiPlayerStatTable(DataGridView dataGridView, PlayerList playerList)
            : base(dataGridView, playerList.primaryColumnNames)
        {
            this.playerList = playerList;

            // Fill the table in a separate thread so the GUI will be displayed while the data is loading
            fillDataTableThread = new Thread(() => FillDataTable(playerList.playerIds));
            fillDataTableThread.Start();
        }

        public void AddRow(string playerId)
        {
            AddPlayerById(playerId);
            playerList.AddPlayer(playerId);
        }

        public void RemoveRow(DataRow row)
        {
            PlayerStats removedPlayerStats = GetPlayerStatsFromRow(row);
            string playerId = removedPlayerStats.GetPlayerId();
            playerList.RemovePlayer(playerId);
            rowHashToPlayerStatsMap.Remove(row.GetHashCode());
            dataTable.Rows.Remove(row);

            Action<FilterManager.FilterType, string> UpdateFilterManager = (FilterManager.FilterType filterType, string collapsedValues) =>
            {
                foreach (string value in collapsedValues.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    filter.FilterInValue(filterType, value);
                }
            };
            UpdateFilterManager(FilterManager.FilterType.League,
                removedPlayerStats.GetCollapsedColumnValue(playerList.displaySeason, Constants.LEAGUE, playerList.primarySeasonType));

            UpdateFilterManager(FilterManager.FilterType.Team,
                removedPlayerStats.GetCollapsedColumnValue(playerList.displaySeason, Constants.TEAM, playerList.primarySeasonType));

            UpdateFilterManager(FilterManager.FilterType.DraftTeam,
                removedPlayerStats.GetCollapsedColumnValue(playerList.displaySeason, Constants.DRAFT_TEAM, playerList.primarySeasonType));
        }

        public void AddColumn(string columnName, string season)
        {
            if (!Constants.AllPossibleColumns.Contains(columnName) || dataTable.Columns.Contains(columnName)) { return; }

            base.AddColumn(columnName);
            playerList.AddPrimaryColumn(columnName);
            foreach (DataGridViewRow dgvRow in dataGridView.Rows)
            {
                DataRow row = GetDataRowFromDGVRow(dgvRow);
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                row[columnName] = playerStats.GetCollapsedColumnValue(season, columnName, playerList.primarySeasonType, filter);
            }
        }

        public new void RemoveColumn(string columnName)
        {
            base.RemoveColumn(columnName);
            playerList.RemovePrimaryColumn(columnName);
        }

        public void SetFilter(FilterManager filter)
        {
            this.filter = filter;
            foreach (DataGridViewRow dgvRow in dataGridView.Rows)
            {
                DataRow row = GetDataRowFromDGVRow(dgvRow);
                ApplyFilterToDataRow(row);
            }
        }

        public void ChangeDisplaySeason(string displaySeason)
        {
            playerList.SetDisplaySeason(displaySeason);
            UpdateRowData();
        }

        public override string GetSeasonType()
        {
            return playerList.primarySeasonType;
        }

        public override void SetSeasonType(string newSeasonType)
        {
            playerList.SetPrimarySeasonType(newSeasonType);
            UpdateRowData();
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

        public int GetMostRecentRowIndex()
        {
            return dataTable.Rows.Count - 1;
        }

        public List<string> GetFilterableValuesBySeason(FilterManager.FilterType filterType, string season)
        {
            HashSet<string> valuesInTable = new HashSet<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                if (playerStats == null) { continue; }
                Dictionary<string, HashSet<string>> filterValuesBySeason = playerStats.GetValuesBySeason(filterType);
                if (filterValuesBySeason.ContainsKey(season))
                {
                    valuesInTable.UnionWith(filterValuesBySeason[season]);
                }
            }
            return valuesInTable.ToList();
        }

        public void AutoFilterOutAllRowsAfterRow(int rowIndex)
        {
            for (int i = rowIndex + 1; i < dataTable.Rows.Count; i++)
            {
                DataRow row = dataTable.Rows[i];
                PlayerStats playerStats = GetPlayerStatsFromRow(row);
                HandleAutoFilterForPlayer(playerStats);
                ApplyFilterToDataRow(row);
            }
        }

        public void AbortFillDataTableThread()
        {
            lock (abortThread)
            {
                abortThread.abort = true;
            }
        }

        public bool ThreadIsRunning()
        {
            return fillDataTableThread.ThreadState == ThreadState.Running;
        }

        private void AddPlayerById(string playerId)
        {
            PlayerStats playerStats = new PlayerStats(playerId);

            Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(playerList.displaySeason, playerList.primarySeasonType);
            DataRow newDataRow = AddRowToDataTable(collapsedYear);
            rowHashToPlayerStatsMap[newDataRow.GetHashCode()] = playerStats;
            
            HandleAutoFilterForPlayer(playerStats);
            ApplyFilterToDataRow(newDataRow, playerStats);
        }

        private void ApplyFilterToDataRow(DataRow dataRow, PlayerStats playerStats = null)
        {
            if (filter == null) { return; }
            playerStats = (playerStats == null) ? rowHashToPlayerStatsMap[dataRow.GetHashCode()] : playerStats;

            foreach (string column in Constants.DynamicColumns)
            {
                try
                {
                    dataRow[column] = playerStats.GetCollapsedColumnValue(playerList.displaySeason, column, playerList.primarySeasonType, filter);
                }
                catch { }
            }
        }

        private void HandleAutoFilterForPlayer(PlayerStats playerStats)
        {
            Action<FilterManager.FilterType, string> HandleAutoFilter = (FilterManager.FilterType filterType, string column) =>
            {
                Dictionary<string, HashSet<string>> valuesBySeason = playerStats.GetValuesBySeason(filterType);
                if (filter != null && filter.IsAutoFilterOut(filterType) && valuesBySeason.ContainsKey(playerList.displaySeason))
                {
                    foreach (string value in valuesBySeason[playerList.displaySeason].ToList())
                    {
                        if (!filter.GetAllValues(filterType).Contains(value))
                        {
                            filter.FilterOutValue(filterType, value);
                        }
                    }
                }
            };
            HandleAutoFilter(FilterManager.FilterType.League, Constants.LEAGUE);
            HandleAutoFilter(FilterManager.FilterType.Team, Constants.TEAM);
            HandleAutoFilter(FilterManager.FilterType.DraftTeam, Constants.DRAFT_TEAM);
        }

        private void UpdateRowData()
        {
            DataRow[] copyOfRows = new DataRow[dataTable.Rows.Count];
            dataTable.Rows.CopyTo(copyOfRows, 0);
            foreach (DataRow row in copyOfRows)
            {
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                HandleAutoFilterForPlayer(playerStats);
                Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(playerList.displaySeason, playerList.primarySeasonType, filter);
                foreach (DataColumn column in dataTable.Columns)
                {
                    string columnName = column.ColumnName;
                    if (Constants.NumericColumns.Contains(columnName))
                    {
                        row[columnName] = (collapsedYear.ContainsKey(columnName) && collapsedYear[columnName] != null) ? (object)Convert.ToInt32(collapsedYear[columnName]) : DBNull.Value;
                    }
                    else
                    {
                        row[columnName] = (collapsedYear.ContainsKey(columnName) && collapsedYear[columnName] != null) ? collapsedYear[columnName] : String.Empty;
                    }
                }
            }
        }

        private void FillDataTable(List<string> playerIds)
        {
            lock (abortThread)
            {
                abortThread.abort = false;
            }
            string[] copyOfPlayerIds = new string[playerIds.Count];
            playerIds.CopyTo(copyOfPlayerIds);
            foreach (string playerId in copyOfPlayerIds)
            {
                AddPlayerById(playerId);
                lock(abortThread)
                {
                    if (abortThread.abort)
                    {
                        break;
                    }
                }
            }
        }
    }
}
