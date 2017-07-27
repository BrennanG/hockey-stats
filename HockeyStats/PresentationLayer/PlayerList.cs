using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HockeyStats
{
    [Serializable]
    public class PlayerList
    {
        public enum ListType { GeneralList, DraftList, TeamList };
        public enum ListStatus { Saved, Unsaved, Generated };

        [Serializable]
        public class FilterValues
        {
            public List<string> allPossibleValues = new List<string>();
            public List<string> filteredOutValues = new List<string>();
            public bool autoFilterOut = false;

            public bool Equals(FilterValues other)
            {
                return allPossibleValues.SequenceEqual(other.allPossibleValues)
                    && filteredOutValues.SequenceEqual(other.filteredOutValues)
                    && autoFilterOut == other.autoFilterOut;
            }

            public FilterValues Clone()
            {
                FilterValues clone = new FilterValues();
                clone.allPossibleValues = new List<string>(allPossibleValues);
                clone.filteredOutValues = new List<string>(filteredOutValues);
                clone.autoFilterOut = autoFilterOut;
                return clone;
            }
        };

        // When adding a new field: make sure to update FillWithDefaults(), Equals(), and Clone()
        public string listName;
        public ListType listType;
        public ListStatus listStatus;
        public string teamId;
        public string primarySeasonType;
        public string secondarySeasonType;
        public string displaySeason;
        public List<string> playerIds;
        public List<string> primaryColumnNames;
        public List<string> secondaryColumnNames;
        public SerializableDictionary<string, int> primaryColumnWidths;
        public SerializableDictionary<string, int> secondaryColumnWidths;
        public FilterValues leagueFilterValues = new FilterValues();
        public FilterValues teamFilterValues = new FilterValues();
        public FilterValues draftTeamFilterValues = new FilterValues();

        public PlayerList()
        {
        }

        public void FillWithDefaults()
        {
            listName = Constants.DEFAULT_LIST_NAME;
            listType = ListType.GeneralList;
            listStatus = ListStatus.Generated;
            teamId = null;
            primarySeasonType = Constants.REGULAR_SEASON;
            secondarySeasonType = Constants.REGULAR_SEASON;
            displaySeason = Constants.MostRecentSeason;
            playerIds = new List<string>();
            primaryColumnNames = Constants.DefaultPrimaryColumns;
            secondaryColumnNames = Constants.DefaultSecondaryColumns;
            primaryColumnWidths = Constants.DefaultPrimaryColumnWidths;
            secondaryColumnWidths = Constants.DefaultSecondaryColumnWidths;
            leagueFilterValues = new FilterValues();
            teamFilterValues = new FilterValues();
            draftTeamFilterValues = new FilterValues();
        }

        public bool Equals(PlayerList other)
        {
            return listName == other.listName
                && listType == other.listType
                && teamId == other.teamId
                && primarySeasonType == other.primarySeasonType
                && secondarySeasonType == other.secondarySeasonType
                && displaySeason == other.displaySeason
                && playerIds.SequenceEqual(other.playerIds)
                && primaryColumnNames.SequenceEqual(other.primaryColumnNames)
                && secondaryColumnNames.SequenceEqual(other.secondaryColumnNames)
                && primaryColumnWidths.Equals(other.primaryColumnWidths)
                && secondaryColumnWidths.Equals(other.secondaryColumnWidths)
                && ((leagueFilterValues == null && other.leagueFilterValues == null) || leagueFilterValues.Equals(other.leagueFilterValues))
                && ((teamFilterValues == null && other.teamFilterValues == null) || teamFilterValues.Equals(other.teamFilterValues))
                && ((draftTeamFilterValues == null && other.draftTeamFilterValues == null) || draftTeamFilterValues.Equals(other.draftTeamFilterValues));
        }

        public PlayerList Clone()
        {
            PlayerList playerList = new PlayerList();
            playerList.listName = listName;
            playerList.listType = listType;
            playerList.listStatus = listStatus;
            playerList.teamId = teamId;
            playerList.primarySeasonType = primarySeasonType;
            playerList.secondarySeasonType = secondarySeasonType;
            playerList.displaySeason = displaySeason;

            string[] playerIds = new string[this.playerIds.Count];
            this.playerIds.CopyTo(playerIds);
            playerList.playerIds = playerIds.ToList();

            string[] primaryColumnNames = new string[this.primaryColumnNames.Count];
            this.primaryColumnNames.CopyTo(primaryColumnNames);
            playerList.primaryColumnNames = primaryColumnNames.ToList();

            string[] secondaryColumnNames = new string[this.secondaryColumnNames.Count];
            this.secondaryColumnNames.CopyTo(secondaryColumnNames);
            playerList.secondaryColumnNames = secondaryColumnNames.ToList();

            playerList.primaryColumnWidths = primaryColumnWidths.Clone();
            playerList.secondaryColumnWidths = secondaryColumnWidths.Clone();

            playerList.leagueFilterValues = leagueFilterValues?.Clone();
            playerList.teamFilterValues = teamFilterValues?.Clone();
            playerList.draftTeamFilterValues = draftTeamFilterValues?.Clone();

            return playerList;
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

        public void SetListType(ListType type)
        {
            if (type != ListType.TeamList) { teamId = null; }
            listType = type;
        }

        public void SetListStatus(ListStatus status)
        {
            listStatus = status;
        }

        public void SetTeamId(string id)
        {
            teamId = id;
        }

        public void SetPrimarySeasonType(string seasonType)
        {
            this.primarySeasonType = seasonType;
        }

        public void SetSecondarySeasonType(string seasonType)
        {
            this.secondarySeasonType = seasonType;
        }

        public void SetDisplaySeason(string season)
        {
            displaySeason = season;
        }

        public void SetPlayerIds(List<string> playerIds)
        {
            this.playerIds = playerIds;
        }

        public void SetPrimaryColumnNames(DataGridViewColumnCollection columns)
        {
            SetColumnNames(columns, ref primaryColumnNames);
        }

        public void SetSecondaryColumnNames(DataGridViewColumnCollection columns)
        {
            SetColumnNames(columns, ref secondaryColumnNames);
        }

        public void SetPrimaryColumnWidths(DataGridViewColumnCollection columns)
        {
            SetColumnWidths(columns, primaryColumnWidths);
        }

        public void SetSecondaryColumnWidths(DataGridViewColumnCollection columns)
        {
            SetColumnWidths(columns, secondaryColumnWidths);
        }

        public int GetPrimaryColumnWidth(string columnName)
        {
            return GetColumnWidth(columnName, primaryColumnWidths);
        }

        public int GetSecondaryColumnWidth(string columnName)
        {
            return GetColumnWidth(columnName, secondaryColumnWidths);
        }

        public List<string> GetAllPossibleValues(FilterManager.FilterType filterType)
        {
            switch (filterType)
            {
                case FilterManager.FilterType.League:
                    return new List<string>(leagueFilterValues.allPossibleValues);
                case FilterManager.FilterType.Team:
                    return new List<string>(teamFilterValues.allPossibleValues);
                case FilterManager.FilterType.DraftTeam:
                    return new List<string>(draftTeamFilterValues.allPossibleValues);
                default:
                    throw new Exception("Case not listed for FilterType");
            }
        }

        public void SetAllPossibleValues(FilterManager.FilterType filterType, List<string> allPossibleValues)
        {
            switch (filterType)
            {
                case FilterManager.FilterType.League:
                    leagueFilterValues.allPossibleValues = new List<string>(allPossibleValues);
                    break;
                case FilterManager.FilterType.Team:
                    teamFilterValues.allPossibleValues = new List<string>(allPossibleValues);
                    break;
                case FilterManager.FilterType.DraftTeam:
                    draftTeamFilterValues.allPossibleValues = new List<string>(allPossibleValues);
                    break;
                default:
                    throw new Exception("Case not listed for FilterType");
            }
        }

        public HashSet<string> GetFilteredOutValues(FilterManager.FilterType filterType)
        {
            switch (filterType)
            {
                case FilterManager.FilterType.League:
                    return new HashSet<string>(leagueFilterValues.filteredOutValues);
                case FilterManager.FilterType.Team:
                    return new HashSet<string>(teamFilterValues.filteredOutValues);
                case FilterManager.FilterType.DraftTeam:
                    return new HashSet<string>(draftTeamFilterValues.filteredOutValues);
                default:
                    throw new Exception("Case not listed for FilterType");
            }
        }

        public void SetFilteredOutValues(FilterManager.FilterType filterType, HashSet<string> filteredOutValues)
        {
            switch (filterType)
            {
                case FilterManager.FilterType.League:
                    leagueFilterValues.filteredOutValues = filteredOutValues.ToList();
                    break;
                case FilterManager.FilterType.Team:
                    teamFilterValues.filteredOutValues = filteredOutValues.ToList();
                    break;
                case FilterManager.FilterType.DraftTeam:
                    draftTeamFilterValues.filteredOutValues = filteredOutValues.ToList();
                    break;
                default:
                    throw new Exception("Case not listed for FilterType");
            }
        }

        public bool GetAutoFilterOut(FilterManager.FilterType filterType)
        {
            switch (filterType)
            {
                case FilterManager.FilterType.League:
                    return leagueFilterValues.autoFilterOut;
                case FilterManager.FilterType.Team:
                    return teamFilterValues.autoFilterOut;
                case FilterManager.FilterType.DraftTeam:
                    return draftTeamFilterValues.autoFilterOut;
                default:
                    throw new Exception("Case not listed for FilterType");
            }
        }

        public void SetAutoFilterOut(FilterManager.FilterType filterType, bool autoFilterOut)
        {
            switch (filterType)
            {
                case FilterManager.FilterType.League:
                    leagueFilterValues.autoFilterOut = autoFilterOut;
                    break;
                case FilterManager.FilterType.Team:
                    teamFilterValues.autoFilterOut = autoFilterOut;
                    break;
                case FilterManager.FilterType.DraftTeam:
                    draftTeamFilterValues.autoFilterOut = autoFilterOut;
                    break;
                default:
                    throw new Exception("Case not listed for FilterType");
            }
        }

        private void SetColumnNames(DataGridViewColumnCollection columns, ref List<string> originalColumnNames)
        {
            string[] columnNames = new string[columns.Count];
            foreach (DataGridViewColumn column in columns)
            {
                columnNames[column.DisplayIndex] = column.Name;
            }
            originalColumnNames = columnNames.ToList();
        }

        private void SetColumnWidths(DataGridViewColumnCollection columns, SerializableDictionary<string, int> columnWidths)
        {
            columnWidths.Clear();
            foreach (DataGridViewColumn column in columns)
            {
                columnWidths[column.Name] = (column.DisplayIndex != columns.Count - 1) ? column.Width : -1;
            }
        }

        private int GetColumnWidth(string columnName, SerializableDictionary<string, int> columnWidths)
        {
            if (columnWidths.ContainsKey(columnName))
            {
                return columnWidths[columnName];
            }
            else
            {
                return -1;
            }
        }
    }
}
