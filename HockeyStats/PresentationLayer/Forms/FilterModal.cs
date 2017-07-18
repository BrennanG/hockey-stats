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
            SetupDropDowns();
            SetupButtons(confirmAction);

            base.ShowDialog();
        }

        private void SetupDropDowns()
        {
            foreach (string league in filter.GetAllLeagues())
            {
                EventHandler selectLeagueHandler = new EventHandler((object sender, EventArgs e) => {
                    ToolStripMenuItem dropDownItem = (ToolStripMenuItem)sender;
                    dropDownItem.Checked = !dropDownItem.Checked;
                });
                filterLeaguesDropDown.DropDownItems.Add(league, null, selectLeagueHandler);

                if (!filter.LeagueIsFilteredOut(league))
                {
                    ((ToolStripMenuItem)filterLeaguesDropDown.DropDownItems[filterLeaguesDropDown.DropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupButtons(Action confirmAction)
        {
            applyFiltersButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                foreach (ToolStripMenuItem item in filterLeaguesDropDown.DropDownItems)
                {
                    if (item.Checked)
                    {
                        filter.FilterInLeague(item.Text);
                    }
                    else
                    {
                        filter.FilterOutLeague(item.Text);
                    }
                }
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
