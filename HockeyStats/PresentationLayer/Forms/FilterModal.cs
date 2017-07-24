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
        FilterManager filter;

        public FilterModal(PlayerStatForm parent, FilterManager filter)
        {
            InitializeComponent();

            this.parent = parent;
            this.filter = filter;
            StartPosition = FormStartPosition.CenterParent;
        }

        public void ShowDialog(Action confirmAction)
        {
            SetupCheckBox(FilterManager.FilterType.League, autoFilterOutLeaguesCheckBox);
            SetupCheckBox(FilterManager.FilterType.Team, autoFilterOutTeamsCheckBox);
            SetupCheckBox(FilterManager.FilterType.DraftTeam, autoFilterOutDraftTeamsCheckBox);

            SetupDropDown(filterLeaguesDropDown, filter.GetAllValues(FilterManager.FilterType.League), FilterManager.FilterType.League);
            SetupDropDown(filterTeamsDropDown, filter.GetAllValues(FilterManager.FilterType.Team), FilterManager.FilterType.Team);
            SetupDropDown(filterDraftTeamsDropDown, filter.GetAllValues(FilterManager.FilterType.DraftTeam), FilterManager.FilterType.DraftTeam);

            SetupButtons(confirmAction);

            base.ShowDialog();
        }

        private void SetupCheckBox(FilterManager.FilterType filterType, CheckBox checkBox)
        {
            checkBox.Checked = filter.IsAutoFilterOut(filterType);
            checkBox.CheckedChanged += new EventHandler((object sender, EventArgs e) =>
            {
                filter.SetAutoFilterOut(filterType, checkBox.Checked);
            });
        }

        private void SetupDropDown(ToolStripMenuItem filterDropDown, List<string> allPossibleValues, FilterManager.FilterType filterType)
        {
            foreach (string possibleValue in allPossibleValues)
            {
                EventHandler itemSelectedHandler = new EventHandler((object sender, EventArgs e) => {
                    ToolStripMenuItem dropDownItem = (ToolStripMenuItem)sender;
                    dropDownItem.Checked = !dropDownItem.Checked;
                });
                filterDropDown.DropDownItems.Add(possibleValue, null, itemSelectedHandler);

                if (!filter.ValueIsFilteredOut(filterType, possibleValue))
                {
                    ((ToolStripMenuItem)filterDropDown.DropDownItems[filterDropDown.DropDownItems.Count - 1]).Checked = true;
                }
            }

            Action<bool> ChangeSelectionOfAllValues = (bool boolean) =>
            {
                foreach (ToolStripMenuItem item in filterDropDown.DropDownItems)
                {
                    if (item.Text == "Select All" || item.Text == "Unselect All") { continue; }
                    item.Checked = boolean;
                }
            };

            filterDropDown.DropDownItems.Add("Select All", null, new EventHandler((object sender, EventArgs e) =>
            {
                ChangeSelectionOfAllValues(true);
            }))
            .BackColor = Color.LightGray;

            filterDropDown.DropDownItems.Add("Unselect All", null, new EventHandler((object sender, EventArgs e) =>
            {
                ChangeSelectionOfAllValues(false);
            }))
            .BackColor = Color.LightGray;
        }

        private void SetupButtons(Action confirmAction)
        {
            Action<ToolStripMenuItem, FilterManager.FilterType> ApplyFilters = (ToolStripMenuItem filterDropDown, FilterManager.FilterType filterType) =>
            {
                foreach (ToolStripMenuItem item in filterDropDown.DropDownItems)
                {
                    if (item.Text == "Select All" || item.Text == "Unselect All") { continue; }
                    if (item.Checked)
                    {
                        filter.FilterInValue(filterType, item.Text);
                    }
                    else
                    {
                        filter.FilterOutValue(filterType, item.Text);
                    }
                }
            };

            applyFiltersButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                ApplyFilters(filterLeaguesDropDown, FilterManager.FilterType.League);
                ApplyFilters(filterTeamsDropDown, FilterManager.FilterType.Team);
                ApplyFilters(filterDraftTeamsDropDown, FilterManager.FilterType.DraftTeam);

                confirmAction();
                Close();
            });

            Action<ToolStripMenuItem, FilterManager.FilterType> ClearFilters = (ToolStripMenuItem filterDropDown, FilterManager.FilterType filterType) =>
            {
                foreach (ToolStripMenuItem item in filterDropDown.DropDownItems)
                {
                    if (item.Text == "Select All" || item.Text == "Unselect All") { continue; }
                    if (filter.ValueIsFilteredOut(filterType, item.Text))
                    {
                        filter.FilterInValue(filterType, item.Text);
                    }
                    item.Checked = true;
                }
            };

            if (!filter.AnyValueIsFilteredOut()) { clearFiltersButton.Enabled = false; }
            clearFiltersButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                ClearFilters(filterLeaguesDropDown, FilterManager.FilterType.League);
                ClearFilters(filterTeamsDropDown, FilterManager.FilterType.Team);
                ClearFilters(filterDraftTeamsDropDown, FilterManager.FilterType.DraftTeam);

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
