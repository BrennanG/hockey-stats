using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    [Serializable]
    public class PlayerList
    {
        public string primarySeasonType;
        public string secondarySeasonType;
        public string displaySeason;
        public List<string> playerIds;
        public List<string> primaryColumnNames;
        public List<string> secondaryColumnNames;
        public SerializableDictionary<string, int> primaryColumnWidths;
        public SerializableDictionary<string, int> secondaryColumnWidths;

        public PlayerList()
        {
            
        }

        public void FillWithDefaults()
        {
            primarySeasonType = Constants.REGULAR_SEASON;
            displaySeason = Constants.CurrentSeason;
            playerIds = new List<string>();
            primaryColumnNames = Constants.DefaultPrimaryColumns;
            secondaryColumnNames = Constants.DefaultSecondaryColumns;
            primaryColumnWidths = Constants.DefaultPrimaryColumnWidths;
            secondaryColumnWidths = Constants.DefaultSecondaryColumnWidths;
        }

        public void AddPlayer(string playerId)
        {
            playerIds.Add(playerId);
        }

        public void RemovePlayer(string playerId)
        {
            playerIds.Remove(playerId);
        }

        public void AddPrimaryColumn(string columnName)
        {
            primaryColumnNames.Add(columnName);
        }

        public void RemovePrimaryColumn(string columnName)
        {
            primaryColumnNames.Remove(columnName);
        }

        public void SetPrimarySeasonType(string seasonType)
        {
            this.primarySeasonType = seasonType;
        }

        public void SetSecondarySeasonType(string seasonType)
        {
            this.secondarySeasonType = seasonType;
        }

        public void SetDisplaySeason(string season)
        {
            displaySeason = season;
        }

        public void SetPlayerIds(List<string> playerIds)
        {
            this.playerIds = playerIds;
        }

        public void SetPrimaryColumns(DataGridViewColumnCollection columns)
        {
            string[] columnNames = new string[columns.Count];
            Dictionary<string, int> columnWidths = new Dictionary<string, int>();
            foreach (DataGridViewColumn column in columns)
            {
                columnNames[column.DisplayIndex] = column.Name;
            }
            primaryColumnNames = columnNames.ToList();
        }

        public void SetPrimaryColumnWidths(DataGridViewColumnCollection columns)
        {
            SetColumnWidths(columns, primaryColumnWidths);
        }

        public void SetSecondaryColumnWidths(DataGridViewColumnCollection columns)
        {
            SetColumnWidths(columns, secondaryColumnWidths);
        }

        public int GetPrimaryColumnWidth(string columnName)
        {
            return GetColumnWidth(columnName, primaryColumnWidths);
        }

        public int GetSecondaryColumnWidth(string columnName)
        {
            return GetColumnWidth(columnName, secondaryColumnWidths);
        }

        private void SetColumnWidths(DataGridViewColumnCollection columns, SerializableDictionary<string, int> columnWidths)
        {
            foreach (DataGridViewColumn column in columns)
            {
                columnWidths[column.Name] = (column.DisplayIndex != columns.Count - 1) ? column.Width : -1;
            }
        }

        private int GetColumnWidth(string columnName, SerializableDictionary<string, int> columnWidths)
        {
            if (columnWidths.ContainsKey(columnName))
            {
                return columnWidths[columnName];
            }
            else
            {
                return -1;
            }
        }
    }
}
