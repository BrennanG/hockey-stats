using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public partial class FilterModal : Form
    {
        PlayerStatForm parent;
        Filter filter;

        public FilterModal(PlayerStatForm parent, Filter filter)
        {
            InitializeComponent();

            this.parent = parent;
            this.filter = filter;
            StartPosition = FormStartPosition.CenterParent;
        }

        public void ShowDialog(Action confirmAction)
        {
            SetupDropDown(filterLeaguesDropDown, filter.GetAllLeagues(), filter.LeagueIsFilteredOut);
            SetupDropDown(filterTeamsDropDown, filter.GetAllTeams(), filter.TeamIsFilteredOut);

            SetupButtons(confirmAction);

            base.ShowDialog();
        }

        private void SetupDropDown(ToolStripMenuItem filterDropDown, List<string> allPossibleValues, Func<string, bool> CheckIfFilteredOut)
        {
            foreach (string possibleValue in allPossibleValues)
            {
                EventHandler itemSelectedHandler = new EventHandler((object sender, EventArgs e) => {
                    ToolStripMenuItem dropDownItem = (ToolStripMenuItem)sender;
                    dropDownItem.Checked = !dropDownItem.Checked;
                });
                filterDropDown.DropDownItems.Add(possibleValue, null, itemSelectedHandler);

                if (!CheckIfFilteredOut(possibleValue))
                {
                    ((ToolStripMenuItem)filterDropDown.DropDownItems[filterDropDown.DropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupButtons(Action confirmAction)
        {
            Action<ToolStripMenuItem, Action<string>, Action<string>> ApplyFilters = (ToolStripMenuItem filterDropDown, Action<string> filterIn, Action<string> filterOut) =>
            {
                foreach (ToolStripMenuItem item in filterDropDown.DropDownItems)
                {
                    if (item.Checked)
                    {
                        filterIn(item.Text);
                    }
                    else
                    {
                        filterOut(item.Text);
                    }
                }
            };

            applyFiltersButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                ApplyFilters(filterLeaguesDropDown, filter.FilterInLeague, filter.FilterOutLeague);
                ApplyFilters(filterTeamsDropDown, filter.FilterInTeam, filter.FilterOutTeam);
                
                confirmAction();
                Close();
            });

            cancelButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Close();
            });
        }
    }
}
