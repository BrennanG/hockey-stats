using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    [Serializable]
    public class PlayerList
    {
        public List<string> playerIds;
        public List<string> displayYears;
        public List<string> primaryTableColumnNames;
        public List<string> secondaryTableColumnNames;
        
        public PlayerList()
        {

        }
    }
}
