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
        public string listName;
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
            listName = Constants.DEFAULT_LIST_NAME;
            playerIds = new List<string>();
            displaySeason = Constants.CurrentSeason;
            primaryColumnNames = Constants.DefaultPrimaryColumns;
            secondaryColumnNames = Constants.DefaultSecondaryColumns;
            primaryColumnWidths = Constants.DefaultPrimaryColumnWidths;
            secondaryColumnWidths = Constants.DefaultSecondaryColumnWidths;
        }

        public void SetListName(string listName)
        {
            this.listName = listName;
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
            foreach (DataGridViewColumn column in columns)
            {
                primaryColumnWidths[column.Name] = (column.DisplayIndex != columns.Count - 1) ? column.Width : -1;
            }
        }

        public int GetPrimaryColumnWidth(string columnName)
        {
            if (primaryColumnWidths.ContainsKey(columnName))
            {
                return primaryColumnWidths[columnName];
            }
            else
            {
                return -1;
            }
        }

        public void SetSecondaryColumnWidths(DataGridViewColumnCollection columns)
        {
            foreach (DataGridViewColumn column in columns)
            {
                secondaryColumnWidths[column.Name] = (column.DisplayIndex != columns.Count - 1) ? column.Width : -1;
            }
        }

        public int GetSecondaryColumnWidth(string columnName)
        {
            if (secondaryColumnWidths.ContainsKey(columnName))
            {
                return secondaryColumnWidths[columnName];
            }
            else
            {
                return -1;
            }
        }

    }
}
