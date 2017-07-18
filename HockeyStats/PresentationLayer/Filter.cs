﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HockeyStats
{
    public class Filter
    {
        private List<string> allLeagues = new List<string>();
        private HashSet<string> filteredOutLeagues = new HashSet<string>();
        private List<string> allTeams = new List<string>();
        private HashSet<string> filteredOutTeams = new HashSet<string>();
        private List<Action> onFilterChange = new List<Action>();

        public Filter()
        {

        }

        public List<string> GetAllLeagues()
        {
            return allLeagues;
        }

        public void SetAllLeagues(List<string> leagues)
        {
            allLeagues = leagues;
        }

        public void FilterOutLeague(string league)
        {
            filteredOutLeagues.Add(league);
            PerformFilterChangeActions();
        }

        public void FilterInLeague(string league)
        {
            filteredOutLeagues.Remove(league); // HashSets don't thow exceptions if not found
            PerformFilterChangeActions();
        }

        public bool LeagueIsFilteredOut(string league)
        {
            return filteredOutLeagues.Contains(league);
        }

        public List<string> GetAllTeams()
        {
            return allTeams;
        }

        public void SetAllTeams(List<string> teams)
        {
            allTeams = teams;
        }

        public void FilterOutTeam(string team)
        {
            filteredOutTeams.Add(team);
            PerformFilterChangeActions();
        }

        public void FilterInTeam(string team)
        {
            filteredOutTeams.Remove(team); // HashSets don't thow exceptions if not found
            PerformFilterChangeActions();
        }

        public bool TeamIsFilteredOut(string team)
        {
            return filteredOutTeams.Contains(team);
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
