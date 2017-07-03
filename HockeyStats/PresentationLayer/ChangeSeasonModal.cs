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

            SetupChangeSeasonDomainUpDown();
            SetupButtons();
        }

        private void SetupChangeSeasonDomainUpDown()
        {
            List<string> seasons = TeamListManager.GetTeamSeasons(parent.currentPlayerList.teamId);
            foreach (string season in seasons)
            {
                changeSeasonDomainUpDown.Items.Add(season);
            }
            changeSeasonDomainUpDown.Text = parent.currentPlayerList.displaySeason;
        }

        private void SetupButtons()
        {
            Action ChangeSeason = () =>
            {
                string teamId = parent.currentPlayerList.teamId;
                string season = changeSeasonDomainUpDown.Text;
                List<string> playerIds = TeamListManager.GetPlayerIdsOnTeam(teamId, season);
                if (playerIds.Count == 0)
                {
                    MessageBox.Show("The team did not play/exist in the " + season + " season");
                    return;
                }
                parent.currentPlayerList.SetPlayerIds(playerIds);
                parent.currentPlayerList.SetDisplaySeason(season);
                if (parent.currentPlayerList.listStatus == PlayerList.ListStatus.Generated)
                {
                    parent.currentListName = parent.currentListName.Substring(0, parent.currentListName.LastIndexOf('(') + 1) + season + ")";
                }
                else
                {
                    parent.currentListName = String.Format("{0} ({1})", TeamListManager.GetTeamName(teamId), season);
                }
                parent.currentPlayerList.SetListStatus(PlayerList.ListStatus.Generated);
                parent.LoadPlayerList(parent.currentPlayerList, parent.currentListName);

                Close();
            };

            changeSeasonDomainUpDown.KeyUp += new KeyEventHandler((object sender, KeyEventArgs e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    parent.TriggerLeaveRequest(ChangeSeason);
                }
            });

            changeSeasonButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                parent.TriggerLeaveRequest(ChangeSeason);
            });

            cancelButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Close();
            });
        }
    }
}
