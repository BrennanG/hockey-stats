using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class PlayerButtonsManager
    {
        public PlayerStatForm form { get; set; }
        public Button searchPlayerButton { get; set; }
        public Button clearSearchButton { get; set; }
        public Button addSelectedPlayerButton { get; set; }
        public Button removeSelectedPlayerButton { get; set; }
        public DataGridView leftTableDGV { get; set; }
        public DataGridView topTableDGV { get; set; }
        public TextBox searchPlayerTextbox { get; set; }

        public void Initialize()
        {
            SetupSearchPlayerButton();
            SetupClearSearchButton();
            SetupAddSelectedPlayerButton();
            SetupRemoveSelectedPlayerButton();
        }

        private void SetupSearchPlayerButton()
        {
            Action SearchPlayer = () => {
                string playerName = searchPlayerTextbox.Text;
                if (String.IsNullOrWhiteSpace(playerName) || playerName.Contains("'") || playerName.Contains("\""))
                {
                    MessageBox.Show("Invalid Search.");
                    searchPlayerTextbox.Text = "";
                }
                else
                {
                    string previousText = searchPlayerButton.Text;
                    searchPlayerButton.Text = "Searching...";
                    searchPlayerButton.Enabled = false;
                    clearSearchButton.Enabled = false;

                    bool successful = form.leftTable.DisplayPlayerSearch(playerName);
                    if (!successful)
                    {
                        MessageBox.Show("No Results Found.");
                    }

                    form.rowJustSelected = false;
                    searchPlayerButton.Text = previousText;
                    searchPlayerButton.Enabled = true;
                    clearSearchButton.Enabled = true;
                }
            };

            searchPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                SearchPlayer();
            });

            // Listen for Enter key when textbox is selected
            searchPlayerTextbox.KeyUp += new KeyEventHandler((object sender, KeyEventArgs e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    SearchPlayer();
                }
            });
        }

        private void SetupClearSearchButton()
        {
            clearSearchButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (leftTableDGV.SelectedRows.Count == 1)
                {
                    form.middleTable.ClearTable();
                    form.rightTable.ClearTable();
                    form.ClearPlayerSelection();
                }
                form.leftTable.ClearTable();
                searchPlayerTextbox.Text = String.Empty;
                clearSearchButton.Enabled = false;
            });
        }

        private void SetupAddSelectedPlayerButton()
        {
            // Enabling and disabling the button
            leftTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                addSelectedPlayerButton.Enabled = (leftTableDGV.SelectedRows.Count == 1);
            });

            // Adding logic to the button
            addSelectedPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (leftTableDGV.SelectedRows.Count != 1) { return; }

                string previousText = addSelectedPlayerButton.Text;
                addSelectedPlayerButton.Text = "Adding Player...";
                addSelectedPlayerButton.Enabled = false;

                int rowIndex = leftTableDGV.SelectedRows[0].Index;
                DataGridViewRow row = leftTableDGV.Rows[rowIndex];
                string playerId = form.leftTable.GetPlayerIdFromRow(row);
                form.topTable.AddRow(playerId);

                addSelectedPlayerButton.Text = previousText;
                form.SetListIsSaved(false);
            });
        }

        private void SetupRemoveSelectedPlayerButton()
        {
            // Enabling and disabling the button
            topTableDGV.SelectionChanged += new EventHandler((object sender, EventArgs e) => {
                removeSelectedPlayerButton.Enabled = (topTableDGV.SelectedRows.Count == 1);
            });

            // Adding logic to the button
            removeSelectedPlayerButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (topTableDGV.SelectedRows.Count != 1) { return; }

                int rowIndex = topTableDGV.SelectedRows[0].Index;
                DataRow row = MultiPlayerStatTable.GetDataRowFromDGVRow(topTableDGV.Rows[rowIndex]);

                string message = String.Format("Are you sure you want to remove {0} {1} from the list?", row[Constants.FIRST_NAME], row[Constants.LAST_NAME]);
                Action RemovePlayer = () => {
                    form.topTable.RemoveRow(row);
                    form.ClearPlayerSelection();
                };
                form.DisplayYesNoMessageBox(message, RemovePlayer);

                form.SetListIsSaved(false);
            });
        }
    }
}
