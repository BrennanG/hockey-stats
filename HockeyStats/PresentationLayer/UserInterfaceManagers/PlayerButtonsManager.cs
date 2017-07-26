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
        public Button searchButton { get; set; }
        public Button clearSearchButton { get; set; }
        public Button addSelectedPlayerButton { get; set; }
        public Button removeSelectedPlayerButton { get; set; }
        public DataGridView leftTableDGV { get; set; }
        public DataGridView topTableDGV { get; set; }
        public TextBox searchTextbox { get; set; }
        public DomainUpDown searchTypeDomainUpDown { get; set; }

        public void Initialize()
        {
            SetupSearchButton();
            SetupSearchTypeDomainUpDown();
            SetupClearSearchButton();
            SetupAddSelectedPlayerButton();
            SetupRemoveSelectedPlayerButton();
        }

        private void SetupSearchButton()
        {
            Action Search = () => {
                string textboxValue = searchTextbox.Text;
                if (String.IsNullOrWhiteSpace(textboxValue) || textboxValue.Contains("'") || textboxValue.Contains("\""))
                {
                    MessageBox.Show("Invalid Search.");
                    searchTextbox.Text = "";
                }
                else
                {
                    string previousText = searchButton.Text;
                    searchButton.Text = "Searching...";
                    searchButton.Enabled = false;
                    clearSearchButton.Enabled = false;

                    bool successful = false;
                    if (searchTypeDomainUpDown.SelectedItem.ToString() == "Player")
                    {
                        successful = form.leftTable.DisplayPlayerSearch(textboxValue);
                    }
                    else if (searchTypeDomainUpDown.SelectedItem.ToString() == "Team")
                    {
                        successful = form.leftTable.DisplayTeamSearch(textboxValue);
                    }

                    if (!successful)
                    {
                        MessageBox.Show("No Results Found.");
                    }

                    form.rowJustSelected = false;
                    searchButton.Text = previousText;
                    searchButton.Enabled = true;
                    clearSearchButton.Enabled = true;
                }
            };

            searchButton.Click += new EventHandler((object sender, EventArgs e) => {
                Search();
            });

            // Listen for Enter key when textbox is selected
            searchTextbox.KeyUp += new KeyEventHandler((object sender, KeyEventArgs e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    Search();
                }
            });
        }

        private void SetupSearchTypeDomainUpDown()
        {
            searchTypeDomainUpDown.Items.Add("Player");
            searchTypeDomainUpDown.Items.Add("Team");
            searchTypeDomainUpDown.SelectedItem = searchTypeDomainUpDown.Items[0];
        }

        private void SetupClearSearchButton()
        {
            clearSearchButton.Click += new EventHandler((object sender, EventArgs e) => {
                if (leftTableDGV.SelectedRows.Count == 1)
                {
                    form.playerStatTablesManager.ClearPlayerSelection();
                }
                form.leftTable.ClearTable();
                searchTextbox.Text = String.Empty;
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
                
                Action AddPlayer = () =>
                {
                    string previousText = addSelectedPlayerButton.Text;
                    addSelectedPlayerButton.Text = "Adding Player...";
                    addSelectedPlayerButton.Enabled = false;

                    int rowIndex = leftTableDGV.SelectedRows[0].Index;
                    DataGridViewRow row = leftTableDGV.Rows[rowIndex];
                    string playerId = form.leftTable.GetIdFromRow(row);
                    form.topTable.AddRow(playerId);

                    addSelectedPlayerButton.Text = previousText;
                    form.currentPlayerList.SetListType(PlayerList.ListType.GeneralList);
                    form.menuStripsManager.RefreshListType();
                    form.SetListStatus(PlayerList.ListStatus.Unsaved);
                };
                
                if (form.currentPlayerList.listType != PlayerList.ListType.GeneralList)
                {
                    string message = String.Format("Are you sure you want to add a player to this list? Doing so will change the list type to General.");
                    form.DisplayYesNoMessageBox(message, AddPlayer);
                }
                else
                {
                    AddPlayer();
                }
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
                if (form.currentPlayerList.listType != PlayerList.ListType.GeneralList)
                {
                    message = String.Format("Are you sure you want to remove {0} {1} from the list? Doing so will change the list type to General.", row[Constants.FIRST_NAME], row[Constants.LAST_NAME]);
                }
                Action RemovePlayer = () => {
                    form.topTable.RemoveRow(row);
                    form.playerStatTablesManager.ClearPlayerSelection();

                    form.currentPlayerList.SetListType(PlayerList.ListType.GeneralList);
                    form.SetListStatus(PlayerList.ListStatus.Unsaved);
                    form.menuStripsManager.RefreshListType();
                };
                form.DisplayYesNoMessageBox(message, RemovePlayer);
            });
        }
    }
}
