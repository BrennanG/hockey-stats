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
            SetupDropDown(filterLeaguesDropDown, filter.GetAllValues(FilterManager.FilterType.League), FilterManager.FilterType.League);
            SetupDropDown(filterTeamsDropDown, filter.GetAllValues(FilterManager.FilterType.Team), FilterManager.FilterType.Team);

            SetupButtons(confirmAction);

            base.ShowDialog();
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
        }

        private void SetupButtons(Action confirmAction)
        {
            Action<ToolStripMenuItem, FilterManager.FilterType> ApplyFilters = (ToolStripMenuItem filterDropDown, FilterManager.FilterType filterType) =>
            {
                foreach (ToolStripMenuItem item in filterDropDown.DropDownItems)
                {
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
