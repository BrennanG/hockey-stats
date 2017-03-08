using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public static class Columns
    {
        public static readonly List<string> PossibleColumns = new List<string>()
        {
            "First Name",
            "Last Name",
            "Games Played",
            "Goals/GAA",
            "Assists/Sv%",
            "Total Points",
            "PPG",
            "League",
            "Team",
            "Year",
            "Draft Year",
            "Draft Round",
            "Draft Overall",
            "Draft Team"
        };

        public static List<string> NumericColumns = new List<string>()
        {
            "Draft Year",
            "Draft Round",
            "Draft Overall"
        };

        public static readonly List<string> ConstantColumns = new List<string>()
        {
            "First Name",
            "Last Name",
            "Draft Year",
            "Draft Round",
            "Draft Overall",
            "Draft Team"
        };

        public static readonly List<string> DynamicColumns = PossibleColumns.Where((string col) => !ConstantColumns.Contains(col)).ToList();
    }
}
