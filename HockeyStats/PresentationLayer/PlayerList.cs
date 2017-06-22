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
        public enum ListType { GeneralList, DraftList, TeamList };
        public enum ListStatus { Saved, Unsaved, Generated };

        // When adding a new field: make sure to update FillWithDefaults(), Equals(), and Clone()
        public ListType listType;
        public ListStatus listStatus;
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
            listType = ListType.GeneralList;
            listStatus = ListStatus.Generated;
            primarySeasonType = Constants.REGULAR_SEASON;
            secondarySeasonType = Constants.REGULAR_SEASON;
            displaySeason = Constants.MostRecentSeason;
            playerIds = new List<string>();
            primaryColumnNames = Constants.DefaultPrimaryColumns;
            secondaryColumnNames = Constants.DefaultSecondaryColumns;
            primaryColumnWidths = Constants.DefaultPrimaryColumnWidths;
            secondaryColumnWidths = Constants.DefaultSecondaryColumnWidths;
        }

        public bool Equals(PlayerList other)
        {
            return listType == other.listType
                && listStatus == other.listStatus
                && primarySeasonType == other.primarySeasonType
                && secondarySeasonType == other.secondarySeasonType
                && displaySeason == other.displaySeason
                && playerIds.SequenceEqual(other.playerIds)
                && primaryColumnNames.SequenceEqual(other.primaryColumnNames)
                && secondaryColumnNames.SequenceEqual(other.secondaryColumnNames)
                && primaryColumnWidths.Equals(other.primaryColumnWidths)
                && secondaryColumnWidths.Equals(other.secondaryColumnWidths);
        }

        public PlayerList Clone()
        {
            PlayerList playerList = new PlayerList();
            playerList.listType = listType;
            playerList.listStatus = listStatus;
            playerList.primarySeasonType = primarySeasonType;
            playerList.secondarySeasonType = secondarySeasonType;
            playerList.displaySeason = displaySeason;

            string[] playerIds = new string[this.playerIds.Count];
            this.playerIds.CopyTo(playerIds);
            playerList.playerIds = playerIds.ToList();

            string[] primaryColumnNames = new string[this.primaryColumnNames.Count];
            this.primaryColumnNames.CopyTo(primaryColumnNames);
            playerList.primaryColumnNames = primaryColumnNames.ToList();

            string[] secondaryColumnNames = new string[this.secondaryColumnNames.Count];
            this.secondaryColumnNames.CopyTo(secondaryColumnNames);
            playerList.secondaryColumnNames = secondaryColumnNames.ToList();

            playerList.primaryColumnWidths = primaryColumnWidths.Clone();
            playerList.secondaryColumnWidths = secondaryColumnWidths.Clone();

            return playerList;
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

        public void SetListType(ListType type)
        {
            listType = type;
        }

        public void SetListStatus(ListStatus status)
        {
            listStatus = status;
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

        public void SetPrimaryColumnNames(DataGridViewColumnCollection columns)
        {
            SetColumnNames(columns, ref primaryColumnNames);
        }

        public void SetSecondaryColumnNames(DataGridViewColumnCollection columns)
        {
            SetColumnNames(columns, ref secondaryColumnNames);
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

        private void SetColumnNames(DataGridViewColumnCollection columns, ref List<string> originalColumnNames)
        {
            string[] columnNames = new string[columns.Count];
            foreach (DataGridViewColumn column in columns)
            {
                columnNames[column.DisplayIndex] = column.Name;
            }
            originalColumnNames = columnNames.ToList();
        }

        private void SetColumnWidths(DataGridViewColumnCollection columns, SerializableDictionary<string, int> columnWidths)
        {
            columnWidths.Clear();
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
