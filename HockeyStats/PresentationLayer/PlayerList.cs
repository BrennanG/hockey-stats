using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    [Serializable]
    public class PlayerList
    {
        public string listName;
        public string displaySeason;
        public List<string> playerIds;
        public List<string> primaryTableColumnNames;
        public List<string> secondaryTableColumnNames;
        
        public PlayerList()
        {
            
        }

        public void FillWithDefaults()
        {
            listName = "";
            playerIds = new List<string>();
            displaySeason = PlayerStatForm.GetCurrentSeason();
            primaryTableColumnNames = Columns.DefaultColumns;
            secondaryTableColumnNames = Columns.DynamicColumns;
        }
    }
}
