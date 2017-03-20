using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public static class Columns
    {
        public static readonly List<string> AllPossibleColumns = new List<string>()
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
            "PIM",
            "Date of Birth",
            "Height",
            "Weight",
            "Shoots/Catches",
            "Draft Year",
            "Draft Round",
            "Draft Overall",
            "Draft Team",
            "ID"
        };

        public static List<string> NumericColumns = new List<string>()
        {
            "PIM",
            "Height",
            "Weight",
            "Draft Year",
            "Draft Round",
            "Draft Overall",
            "ID"
        };

        public static List<string> DefaultColumns = new List<string>()
        {
            "First Name",
            "Last Name",
            "Games Played",
            "Goals/GAA",
            "Assists/Sv%",
            "Total Points",
            "PPG",
            "League",
            "Draft Year",
            "Draft Round",
            "Draft Overall",
            "Draft Team",
        };

        public static readonly List<string> ConstantColumns = new List<string>()
        {
            "First Name",
            "Last Name",
            "Date of Birth",
            "Height",
            "Weight",
            "Shoots/Catches",
            "Draft Year",
            "Draft Round",
            "Draft Overall",
            "Draft Team",
            "ID"
        };

        public static readonly List<string> DynamicColumns = AllPossibleColumns.Where((string col) => !ConstantColumns.Contains(col)).ToList();

        public static readonly List<string> AllPossibleColumnsAlphebetized = GetAllPossibleColumnsAlphebetized();

        private static List<string> GetAllPossibleColumnsAlphebetized()
        {
            string[] copy = new string[AllPossibleColumns.Count];
            AllPossibleColumns.CopyTo(copy);
            List<string> copyAsList = copy.ToList();
            copyAsList.Sort();
            return copyAsList;
        }
    }
}
