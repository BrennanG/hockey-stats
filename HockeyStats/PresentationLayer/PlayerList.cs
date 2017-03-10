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
        public List<string> playerIds;
        public List<string> displayYears;
        public List<string> primaryTableColumnNames;
        public List<string> secondaryTableColumnNames;
        
        public PlayerList()
        {
            
        }

        public void FillWithDefaults()
        {
            listName = "";
            playerIds = new List<string>();
            displayYears = new List<string>() { "2016-2017" };
            primaryTableColumnNames = Columns.AllPossibleColumns;
            secondaryTableColumnNames = Columns.DynamicColumns;
        }
    }
}
