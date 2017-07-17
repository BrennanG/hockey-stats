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
    public partial class ChangeSeasonModal : Form
    {
        PlayerStatForm parent;

        public ChangeSeasonModal(PlayerStatForm parent)
        {
            InitializeComponent();

            this.parent = parent;
            StartPosition = FormStartPosition.CenterParent;
        }

        public void ShowDialog(Action confirmAction, List<string> seasons)
        {
            SetupChangeSeasonDomainUpDown(seasons);
            SetupButtons(confirmAction);

            base.ShowDialog();
        }

        public string GetDomainUpDownText()
        {
            return changeSeasonDomainUpDown.Text;
        }

        private void SetupChangeSeasonDomainUpDown(List<string> seasons)
        {
            foreach (string season in seasons)
            {
                changeSeasonDomainUpDown.Items.Add(season);
            }
            changeSeasonDomainUpDown.SelectedIndex = changeSeasonDomainUpDown.Items.IndexOf(parent.currentPlayerList.displaySeason);
        }

        private void SetupButtons(Action confirmAction)
        {
            changeSeasonDomainUpDown.KeyUp += new KeyEventHandler((object sender, KeyEventArgs e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    confirmAction();
                }
            });

            changeSeasonButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                confirmAction();
            });

            cancelButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Close();
            });
        }
    }
}
