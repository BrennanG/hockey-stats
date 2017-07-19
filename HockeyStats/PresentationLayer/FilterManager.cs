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

        public List<string> GetAllValues(FilterType type)
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

        public bool ValueIsFilteredOut(FilterType type, string value)
        {
            return filterMap[type].filteredOutValues.Contains(value);
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
