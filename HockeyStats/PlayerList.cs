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

        public void FillWithDefaults()
        {
            playerIds = new List<string>();
            displayYears = new List<string>() { "2016-2017" };
            primaryTableColumnNames = new List<string>
            {
                "Last Name", "Games Played", "Goals", "Assists", "Total Points", "PPG", "League", "Draft Year", "Draft Round", "Draft Overall", "Draft Team"
            };
            secondaryTableColumnNames = new List<string>
            {
                "Year", "Games Played", "Goals", "Assists", "Total Points", "PPG", "League"
            };
        }
    }
}
