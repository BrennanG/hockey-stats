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

        public PlayerList()
        {
            
        }

        public void FillWithDefaults()
        {
            listName = "";
            playerIds = new List<string>();
            displaySeason = PlayerStatForm.GetCurrentSeason();
            primaryColumnNames = Columns.DefaultColumns;
            secondaryColumnNames = Columns.DynamicColumns;
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
    }
}
