﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public class FilterManager
    {
        public enum FilterType { League, Team, DraftTeam };

        private class Filter
        {
            public List<string> allPossibleValues = new List<string>();
            public HashSet<string> filteredOutValues = new HashSet<string>();
            public bool autoFilterOut = false;
        };

        private Filter leagueFilter = new Filter();
        private Filter teamFilter = new Filter();
        private Filter draftTeamFilter = new Filter();
        private Dictionary<FilterType, Filter> filterMap;
        private List<Action> onFilterChange = new List<Action>();

        public FilterManager()
        {
            filterMap = new Dictionary<FilterType, Filter>
            {
                {FilterType.League, leagueFilter},
                {FilterType.Team, teamFilter},
                {FilterType.DraftTeam, draftTeamFilter},
            };
        }

        public FilterManager(PlayerList playerList)
        {
            leagueFilter.allPossibleValues = playerList.GetAllPossibleValues(FilterType.League);
            teamFilter.allPossibleValues = playerList.GetAllPossibleValues(FilterType.Team);
            draftTeamFilter.allPossibleValues = playerList.GetAllPossibleValues(FilterType.DraftTeam);

            leagueFilter.filteredOutValues = playerList.GetFilteredOutValues(FilterType.League);
            teamFilter.filteredOutValues = playerList.GetFilteredOutValues(FilterType.Team);
            draftTeamFilter.filteredOutValues = playerList.GetFilteredOutValues(FilterType.DraftTeam);

            leagueFilter.autoFilterOut = playerList.GetAutoFilterOut(FilterType.League);
            teamFilter.autoFilterOut = playerList.GetAutoFilterOut(FilterType.Team);
            draftTeamFilter.autoFilterOut = playerList.GetAutoFilterOut(FilterType.DraftTeam);

            filterMap = new Dictionary<FilterType, Filter>
            {
                {FilterType.League, leagueFilter},
                {FilterType.Team, teamFilter},
                {FilterType.DraftTeam, draftTeamFilter},
            };
        }

        public List<string> GetAllPossibleValues(FilterType type)
        {
            return filterMap[type].allPossibleValues;
        }

        public void SetAllValues(FilterType type, List<string> values)
        {
            filterMap[type].allPossibleValues = values;
        }

        public void FilterOutValue(FilterType type, string value)
        {
            filterMap[type].filteredOutValues.Add(value);
            PerformFilterChangeActions();
        }

        public void FilterInValue(FilterType type, string value)
        {
            filterMap[type].filteredOutValues.Remove(value); // HashSets don't thow exceptions if not found
            PerformFilterChangeActions();
        }

        public HashSet<string> GetFilteredOutValues(FilterType type)
        {
            return filterMap[type].filteredOutValues;
        }

        public bool ValueIsFilteredOut(FilterType type, string value)
        {
            return filterMap[type].filteredOutValues.Contains(value);
        }

        public bool IsAutoFilterOut(FilterType type)
        {
            return filterMap[type].autoFilterOut;
        }

        public void SetAutoFilterOut(FilterType type, bool boolean)
        {
            filterMap[type].autoFilterOut = boolean;
        }

        public bool AnyValueIsFilteredOut()
        {
            return leagueFilter.filteredOutValues.Count > 0
                || teamFilter.filteredOutValues.Count > 0
                || draftTeamFilter.filteredOutValues.Count > 0;
        }
        
        public void OnFilterChange(Action action)
        {
            onFilterChange.Add(action);
        }

        private void PerformFilterChangeActions()
        {
            foreach (Action action in onFilterChange)
            {
                action();
            }
        }
    }
}
