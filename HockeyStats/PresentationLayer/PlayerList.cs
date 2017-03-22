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
        public string displayYear;
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
            displayYear = "2016-2017";
            primaryTableColumnNames = Columns.DefaultColumns;
            secondaryTableColumnNames = Columns.DynamicColumns;
        }
    }
}
