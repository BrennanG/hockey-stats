using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public partial class LoadDraftModal : Form
    {
        PlayerStatForm parent;

        // Auto-generated fields
        private NumericUpDown draftRoundLowerNumericUpDown;
        private NumericUpDown draftRoundUpperNumericUpDown;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button cancelButton;
        private Button loadDraftButton;
        private NumericUpDown draftYearNumericUpDown;

        public LoadDraftModal(PlayerStatForm parent)
        {
            InitializeComponent();

            this.parent = parent;
            StartPosition = FormStartPosition.CenterParent;

            SetupNumericUpDowns();
            SetupButtons();
        }

        private void SetupNumericUpDowns()
        {
            // Set starting values for Draft Year UpDown
            List<string> draftYears = parent.configuration.draftYears;
            draftYearNumericUpDown.Minimum = Int32.Parse(draftYears[draftYears.Count - 1]);
            draftYearNumericUpDown.Maximum = Int32.Parse(draftYears[0]);
            draftYearNumericUpDown.Value = draftYearNumericUpDown.Maximum;

            // Set starting values for both Draft Round UpDowns
            decimal numOfRounds = parent.configuration.draftYearToNumberOfRoundsMap[draftYearNumericUpDown.Value.ToString()];
            draftRoundLowerNumericUpDown.Maximum = draftRoundUpperNumericUpDown.Maximum = numOfRounds;
            draftRoundLowerNumericUpDown.Value = draftRoundLowerNumericUpDown.Minimum;
            draftRoundUpperNumericUpDown.Value = draftRoundUpperNumericUpDown.Maximum;

            // Update the max and min round values when the selected draft year is changed
            draftYearNumericUpDown.ValueChanged += new EventHandler((object sender, EventArgs e) =>
            {
                draftRoundLowerNumericUpDown.Maximum = parent.configuration.draftYearToNumberOfRoundsMap[draftYearNumericUpDown.Value.ToString()];
                draftRoundUpperNumericUpDown.Maximum = parent.configuration.draftYearToNumberOfRoundsMap[draftYearNumericUpDown.Value.ToString()];
                draftRoundLowerNumericUpDown.Value = Math.Min(draftRoundLowerNumericUpDown.Value, draftRoundLowerNumericUpDown.Maximum);
                draftRoundUpperNumericUpDown.Value = Math.Min(draftRoundUpperNumericUpDown.Value, draftRoundUpperNumericUpDown.Maximum);
            });

            // Prevent the range's lower value from being higher that the upper value
            decimal lastAllowableLowerValue = draftRoundLowerNumericUpDown.Minimum;
            decimal lastAllowableUpperValue = draftRoundUpperNumericUpDown.Maximum;
            EventHandler roundValueChecker = new EventHandler((object sender, EventArgs e) =>
            {
                if (draftRoundLowerNumericUpDown.Value > draftRoundUpperNumericUpDown.Value)
                {
                    draftRoundLowerNumericUpDown.Value = lastAllowableLowerValue;
                    draftRoundUpperNumericUpDown.Value = lastAllowableUpperValue;
                }
                lastAllowableLowerValue = draftRoundLowerNumericUpDown.Value;
                lastAllowableUpperValue = draftRoundUpperNumericUpDown.Value;
            });
            draftRoundLowerNumericUpDown.ValueChanged += roundValueChecker;
            draftRoundUpperNumericUpDown.ValueChanged += roundValueChecker;
        }

        private void SetupButtons()
        {
            Action LoadDraftList = () =>
            {
                cancelButton.Enabled = false;
                loadDraftButton.Enabled = false;
                loadDraftButton.Text = "Loading...";

                PlayerList playerList = new PlayerList();
                playerList.FillWithDefaults();
                playerList.SetListType(PlayerList.ListType.DraftList);
                playerList.primaryColumnNames = Constants.DefaultDraftPrimaryColumns;
                playerList.primaryColumnWidths = Constants.DefaultDraftPrimaryColumnWidths;
                string year = draftYearNumericUpDown.Value.ToString();
                int lowerRound = (int)draftRoundLowerNumericUpDown.Value;
                int upperRound = (int)draftRoundUpperNumericUpDown.Value;
                playerList.playerIds = DraftListManager.GetPlayersInDraftYear(year, lowerRound, upperRound);

                string listName = year + " Draft";
                if (lowerRound == upperRound)
                {
                    listName += String.Format(" (Round {0})", lowerRound.ToString());
                }
                else
                {
                    listName += String.Format(" (Rounds {0}-{1})", lowerRound.ToString(), upperRound.ToString());
                }
                playerList.listName = listName;
                parent.LoadPlayerList(playerList);

                Close();
            };

            KeyEventHandler keyEvent = new KeyEventHandler((object sender, KeyEventArgs e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    parent.TriggerLeaveRequest(LoadDraftList);
                }
            });
            draftYearNumericUpDown.KeyUp += keyEvent;
            draftRoundLowerNumericUpDown.KeyUp += keyEvent;
            draftRoundUpperNumericUpDown.KeyUp += keyEvent;

            loadDraftButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                parent.TriggerLeaveRequest(LoadDraftList);
            });

            cancelButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Close();
            });
        }

        // Auto-generated function
        private void InitializeComponent()
        {
            this.draftYearNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.draftRoundLowerNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.draftRoundUpperNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.loadDraftButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.draftYearNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.draftRoundLowerNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.draftRoundUpperNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // draftYearNumericUpDown
            // 
            this.draftYearNumericUpDown.BackColor = System.Drawing.Color.White;
            this.draftYearNumericUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.draftYearNumericUpDown.Location = new System.Drawing.Point(21, 40);
            this.draftYearNumericUpDown.Name = "draftYearNumericUpDown";
            this.draftYearNumericUpDown.Size = new System.Drawing.Size(80, 20);
            this.draftYearNumericUpDown.TabIndex = 5;
            // 
            // draftRoundLowerNumericUpDown
            // 
            this.draftRoundLowerNumericUpDown.BackColor = System.Drawing.Color.White;
            this.draftRoundLowerNumericUpDown.Location = new System.Drawing.Point(21, 88);
            this.draftRoundLowerNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.draftRoundLowerNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.draftRoundLowerNumericUpDown.Name = "draftRoundLowerNumericUpDown";
            this.draftRoundLowerNumericUpDown.Size = new System.Drawing.Size(43, 20);
            this.draftRoundLowerNumericUpDown.TabIndex = 6;
            this.draftRoundLowerNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // draftRoundUpperNumericUpDown
            // 
            this.draftRoundUpperNumericUpDown.BackColor = System.Drawing.Color.White;
            this.draftRoundUpperNumericUpDown.Location = new System.Drawing.Point(92, 88);
            this.draftRoundUpperNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.draftRoundUpperNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.draftRoundUpperNumericUpDown.Name = "draftRoundUpperNumericUpDown";
            this.draftRoundUpperNumericUpDown.Size = new System.Drawing.Size(43, 20);
            this.draftRoundUpperNumericUpDown.TabIndex = 7;
            this.draftRoundUpperNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Draft Year";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Draft Rounds";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(70, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "to";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(100, 133);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 11;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // loadDraftButton
            // 
            this.loadDraftButton.Location = new System.Drawing.Point(19, 133);
            this.loadDraftButton.Name = "loadDraftButton";
            this.loadDraftButton.Size = new System.Drawing.Size(75, 23);
            this.loadDraftButton.TabIndex = 12;
            this.loadDraftButton.Text = "Load Draft";
            this.loadDraftButton.UseVisualStyleBackColor = true;
            // 
            // LoadDraftModal
            // 
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.ClientSize = new System.Drawing.Size(187, 168);
            this.Controls.Add(this.loadDraftButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.draftRoundUpperNumericUpDown);
            this.Controls.Add(this.draftRoundLowerNumericUpDown);
            this.Controls.Add(this.draftYearNumericUpDown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadDraftModal";
            this.Text = "Load Draft";
            ((System.ComponentModel.ISupportInitialize)(this.draftYearNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.draftRoundLowerNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.draftRoundUpperNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
