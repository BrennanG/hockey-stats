using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public static class Columns
    {
        public static readonly string FIRST_NAME = "First Name";
        public static readonly string LAST_NAME = "Last Name";
        public static readonly string GAMES_PLAYED = "Games Played";
        public static readonly string GOALS_GAA = "Goals/GAA";
        public static readonly string ASSISTS_SVP = "Assists/Sv%";
        public static readonly string TOTAL_POINTS = "Total Points";
        public static readonly string PPG = "PPG";
        public static readonly string PLUS_MINUS = "+/-";
        public static readonly string POSITION = "Position";
        public static readonly string LEAGUE = "League";
        public static readonly string TEAM = "Team";
        public static readonly string SEASON = "Season";
        public static readonly string PIM = "PIM";
        public static readonly string DATE_OF_BIRTH = "Date of Birth";
        public static readonly string HEIGHT = "Height";
        public static readonly string WEIGHT = "Weight";
        public static readonly string SHOOTS_CATCHES = "Shoots/Catches";
        public static readonly string DRAFT_YEAR = "Draft Year";
        public static readonly string DRAFT_ROUND = "Draft Round";
        public static readonly string DRAFT_OVERALL = "Draft Overall";
        public static readonly string DRAFT_TEAM = "Draft Team";
        public static readonly string ID = "ID";

        public static readonly List<string> AllPossibleColumns = new List<string>()
        {
            Columns.FIRST_NAME,
            Columns.LAST_NAME,
            Columns.GAMES_PLAYED,
            Columns.GOALS_GAA,
            Columns.ASSISTS_SVP,
            Columns.TOTAL_POINTS,
            Columns.PPG,
            Columns.PLUS_MINUS,
            Columns.POSITION,
            Columns.LEAGUE,
            Columns.TEAM,
            Columns.SEASON,
            Columns.PIM,
            Columns.DATE_OF_BIRTH,
            Columns.HEIGHT,
            Columns.WEIGHT,
            Columns.SHOOTS_CATCHES,
            Columns.DRAFT_YEAR,
            Columns.DRAFT_ROUND,
            Columns.DRAFT_OVERALL,
            Columns.DRAFT_TEAM,
            Columns.ID
        };

        public static List<string> NumericColumns = new List<string>()
        {
            //Columns.HEIGHT,
            //Columns.WEIGHT,
            Columns.DRAFT_YEAR,
            Columns.DRAFT_ROUND,
            Columns.DRAFT_OVERALL,
            Columns.ID
        };

        public static List<string> DefaultColumns = new List<string>()
        {
            Columns.FIRST_NAME,
            Columns.LAST_NAME,
            Columns.GAMES_PLAYED,
            Columns.GOALS_GAA,
            Columns.ASSISTS_SVP,
            Columns.TOTAL_POINTS,
            Columns.PPG,
            Columns.POSITION,
            Columns.LEAGUE,
            Columns.DRAFT_YEAR,
            Columns.DRAFT_ROUND,
            Columns.DRAFT_OVERALL,
            Columns.DRAFT_TEAM,
        };

        public static readonly List<string> ConstantColumns = new List<string>()
        {
            Columns.FIRST_NAME,
            Columns.LAST_NAME,
            Columns.POSITION,
            Columns.DATE_OF_BIRTH,
            Columns.HEIGHT,
            Columns.WEIGHT,
            Columns.SHOOTS_CATCHES,
            Columns.DRAFT_YEAR,
            Columns.DRAFT_ROUND,
            Columns.DRAFT_OVERALL,
            Columns.DRAFT_TEAM,
            Columns.ID
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
