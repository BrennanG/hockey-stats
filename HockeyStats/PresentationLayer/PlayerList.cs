using System;
using System.Collections.Generic;
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

        public PlayerList()
        {
            
        }

        public void FillWithDefaults()
        {
            listName = "";
            playerIds = new List<string>();
            displaySeason = Constants.CurrentSeason;
            primaryColumnNames = Constants.DefaultColumns;
            secondaryColumnNames = Constants.DynamicColumns;
            primaryColumnWidths = new SerializableDictionary<string, int>();
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
                primaryColumnWidths[column.Name] = column.Width;
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
    }
}
