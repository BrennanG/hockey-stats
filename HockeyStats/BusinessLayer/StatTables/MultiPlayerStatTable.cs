﻿using Newtonsoft.Json.Linq;
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

        private Dictionary<string, HashSet<string>> leaguesInTableBySeason = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, HashSet<string>> teamsInTableBySeason = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, HashSet<string>> draftTeamsInTableBySeason = new Dictionary<string, HashSet<string>>();
        private Dictionary<FilterManager.FilterType, Dictionary<string, HashSet<string>>> filterMap;
        private FilterManager currentFilter;

        private Thread fillDataTableThread;
        public class AbortThread { public bool abort; }; // Needs to be a class, because you can only lock on instances of classes
        public AbortThread abortThread = new AbortThread() { abort = false };
        
        public MultiPlayerStatTable(DataGridView dataGridView, PlayerList playerList)
            : base(dataGridView, playerList.primaryColumnNames)
        {
            this.playerList = playerList;

            filterMap = new Dictionary<FilterManager.FilterType, Dictionary<string, HashSet<string>>>
            {
                { FilterManager.FilterType.League, leaguesInTableBySeason },
                { FilterManager.FilterType.Team, teamsInTableBySeason },
                { FilterManager.FilterType.DraftTeam, draftTeamsInTableBySeason }
            };

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
            dataTable.Rows.Remove(row);

            PlayerStats removedPlayerStats = GetPlayerStatsFromRow(row);
            string playerId = removedPlayerStats.GetPlayerId();
            playerList.RemovePlayer(playerId);

            UpdateFilterValuesInTable(FilterManager.FilterType.League);
            UpdateFilterValuesInTable(FilterManager.FilterType.Team);
            UpdateFilterValuesInTable(FilterManager.FilterType.DraftTeam);
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
                row[columnName] = playerStats.GetCollapsedColumnValue(season, columnName, playerList.primarySeasonType);
            }
        }

        public new void RemoveColumn(string columnName)
        {
            base.RemoveColumn(columnName);
            playerList.RemovePrimaryColumn(columnName);
        }

        public void ApplyFilterToAllRows(FilterManager filter)
        {
            currentFilter = filter;
            foreach (DataGridViewRow dgvRow in dataGridView.Rows)
            {
                DataRow row = GetDataRowFromDGVRow(dgvRow);
                ApplyFilterToDataRow(row, currentFilter);
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

        public List<string> GetValuesBySeason(FilterManager.FilterType filterType, string season)
        {
            Dictionary<string, HashSet<string>> valuesInTableBySeason = filterMap[filterType];
            return valuesInTableBySeason.ContainsKey(season) ? valuesInTableBySeason[season].ToList() : new List<string>();
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
            ApplyFilterToDataRow(newDataRow, currentFilter, playerStats);

            Action<Dictionary<string, HashSet<string>>, Dictionary<string, HashSet<string>>> FillValuesInTable = 
                (Dictionary<string, HashSet<string>> valuesInTable, Dictionary<string, HashSet<string>> filterValuesBySeason) =>
            {
                rowHashToPlayerStatsMap.Count();
                foreach (string season in filterValuesBySeason.Keys)
                {
                    if (!valuesInTable.ContainsKey(season))
                    {
                        valuesInTable[season] = filterValuesBySeason[season];
                    }
                    else
                    {
                        valuesInTable[season].UnionWith(filterValuesBySeason[season]);
                    }
                }
            };
            FillValuesInTable(leaguesInTableBySeason, playerStats.GetValuesBySeason(FilterManager.FilterType.League));
            FillValuesInTable(teamsInTableBySeason, playerStats.GetValuesBySeason(FilterManager.FilterType.Team));
            FillValuesInTable(draftTeamsInTableBySeason, playerStats.GetValuesBySeason(FilterManager.FilterType.DraftTeam));
        }

        private void ApplyFilterToDataRow(DataRow dataRow, FilterManager filter, PlayerStats playerStats = null)
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

        private void UpdateFilterValuesInTable(FilterManager.FilterType filterType)
        {
            Dictionary<string, HashSet<string>> valuesInTableBySeason = filterMap[filterType];
            valuesInTableBySeason.Clear();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                PlayerStats playerStats = GetPlayerStatsFromRow(dataRow);
                foreach (KeyValuePair<string, HashSet<string>> valuesBySeason in playerStats.GetValuesBySeason(filterType))
                {
                    string season = valuesBySeason.Key;
                    foreach (string value in valuesBySeason.Value)
                    {
                        if (!valuesInTableBySeason.ContainsKey(season))
                        {
                            valuesInTableBySeason[season] = new HashSet<string>();
                        }
                        valuesInTableBySeason[season].Add(value);
                    }
                }
            }
        }

        private void UpdateRowData()
        {
            DataRow[] copyOfRows = new DataRow[dataTable.Rows.Count];
            dataTable.Rows.CopyTo(copyOfRows, 0);
            foreach (DataRow row in copyOfRows)
            {
                PlayerStats playerStats = rowHashToPlayerStatsMap[row.GetHashCode()];
                Dictionary<string, string> collapsedYear = playerStats.GetCollapsedYear(playerList.displaySeason, playerList.primarySeasonType);
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
