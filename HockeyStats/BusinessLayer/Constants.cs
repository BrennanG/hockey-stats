using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public static class Constants
    {
        public static readonly string DEFAULT_LIST_NAME = "NewList";

        public static readonly string REGULAR_SEASON = "Regular Season";
        public static readonly string PLAYOFFS = "Playoffs";

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
            Constants.FIRST_NAME,
            Constants.LAST_NAME,
            Constants.GAMES_PLAYED,
            Constants.GOALS_GAA,
            Constants.ASSISTS_SVP,
            Constants.TOTAL_POINTS,
            Constants.PPG,
            Constants.PLUS_MINUS,
            Constants.POSITION,
            Constants.LEAGUE,
            Constants.TEAM,
            Constants.SEASON,
            Constants.PIM,
            Constants.DATE_OF_BIRTH,
            Constants.HEIGHT,
            Constants.WEIGHT,
            Constants.SHOOTS_CATCHES,
            Constants.DRAFT_YEAR,
            Constants.DRAFT_ROUND,
            Constants.DRAFT_OVERALL,
            Constants.DRAFT_TEAM,
            Constants.ID
        };

        public static List<string> NumericColumns = new List<string>()
        {
            Constants.DRAFT_YEAR,
            Constants.DRAFT_ROUND,
            Constants.DRAFT_OVERALL,
            Constants.ID
        };

        public static List<string> DefaultPrimaryColumns = new List<string>()
        {
            Constants.FIRST_NAME,
            Constants.LAST_NAME,
            Constants.GAMES_PLAYED,
            Constants.GOALS_GAA,
            Constants.ASSISTS_SVP,
            Constants.TOTAL_POINTS,
            Constants.PPG,
            Constants.POSITION,
            Constants.LEAGUE,
            Constants.DRAFT_YEAR,
            Constants.DRAFT_ROUND,
            Constants.DRAFT_OVERALL,
            Constants.DRAFT_TEAM,
        };


        public static List<string> DefaultSecondaryColumns = new List<string>()
        {
            Constants.SEASON,
            Constants.GAMES_PLAYED,
            Constants.GOALS_GAA,
            Constants.ASSISTS_SVP,
            Constants.TOTAL_POINTS,
            Constants.PPG,
            Constants.LEAGUE,
            Constants.TEAM,

        };

        public static readonly List<string> ConstantColumns = new List<string>()
        {
            Constants.FIRST_NAME,
            Constants.LAST_NAME,
            Constants.POSITION,
            Constants.DATE_OF_BIRTH,
            Constants.HEIGHT,
            Constants.WEIGHT,
            Constants.SHOOTS_CATCHES,
            Constants.DRAFT_YEAR,
            Constants.DRAFT_ROUND,
            Constants.DRAFT_OVERALL,
            Constants.DRAFT_TEAM,
            Constants.ID
        };

        public static readonly List<string> DynamicColumns = AllPossibleColumns.Where((string col) => !ConstantColumns.Contains(col)).ToList();

        public static readonly List<string> AllPossibleColumnsAlphebetized = GetAllPossibleColumnsAlphebetized();

        public static readonly SerializableDictionary<string, int> DefaultPrimaryColumnWidths = GetDefaultPrimaryColumnWidths();

        public static readonly SerializableDictionary<string, int> DefaultSecondaryColumnWidths = new SerializableDictionary<string, int>()
        {
            { Constants.SEASON, 65 },
            { Constants.GAMES_PLAYED, 44 },
            { Constants.GOALS_GAA, 38 },
            { Constants.ASSISTS_SVP, 44 },
            { Constants.TOTAL_POINTS, 38 },
            { Constants.PPG, 62 },
            { Constants.LEAGUE, 130 },
            { Constants.TEAM, -1 }
        };

        public static readonly List<string> SeasonTypes = new List<string>()
        {
            Constants.REGULAR_SEASON,
            Constants.PLAYOFFS
        };

        public static readonly string CurrentSeason = GetCurrentSeason();



        // Methods
        private static List<string> GetAllPossibleColumnsAlphebetized()
        {
            string[] copy = new string[AllPossibleColumns.Count];
            AllPossibleColumns.CopyTo(copy);
            List<string> copyAsList = copy.ToList();
            copyAsList.Sort();
            return copyAsList;
        }

        public static SerializableDictionary<string, int> GetDefaultPrimaryColumnWidths()
        {
            SerializableDictionary<string, int> widths = new SerializableDictionary<string, int>();
            foreach (string column in AllPossibleColumns)
            {
                widths.Add(column, 83);
            }
            return widths;
        }

        private static string GetCurrentSeason()
        {
            DateTime today = DateTime.Today;
            int seasonStart, seasonEnd;
            if (today.Month < 6)
            {
                seasonStart = today.Year - 1;
                seasonEnd = today.Year;
            }
            else
            {
                seasonStart = today.Year;
                seasonEnd = today.Year + 1;
            }
            return String.Format("{0}-{1}", seasonStart, seasonEnd);
        }
    }
}
