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

        public FilterModal(PlayerStatForm parent)
        {
            InitializeComponent();

            this.parent = parent;
            StartPosition = FormStartPosition.CenterParent;
        }

        public void ShowDialog(Action confirmAction, List<string> leagues)
        {
            SetupDropDowns(leagues);
            SetupButtons(confirmAction);

            base.ShowDialog();
        }

        private void SetupDropDowns(List<string> seasons)
        {
            foreach (string season in seasons)
            {
                filterLeaguesDropDown.DropDownItems.Add(season);
            }
        }

        private void SetupButtons(Action confirmAction)
        {
            
        }
    }
}
