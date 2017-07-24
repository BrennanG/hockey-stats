using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class MenuStripsManager
    {
        public PlayerStatForm form { get; set; }
        public ToolStripMenuItem loadListDropDown { get; set; }
        public ToolStripMenuItem saveToolStripMenuItem { get; set; }
        public ToolStripMenuItem saveAsToolStripMenuItem { get; set; }
        public ToolStripMenuItem createListToolStripMenuItem { get; set; }
        public ToolStripMenuItem deleteListToolStripMenuItem { get; set; }
        public ToolStripMenuItem setAsDefaultListToolStripMenuItem { get; set; }
        public ToolStripMenuItem loadDraftToolStripMenuItem { get; set; }
        public ToolStripMenuItem selectPrimarySeasonTypeDropDown { get; set; }
        public ToolStripMenuItem selectSecondarySeasonTypeDropDown { get; set; }
        public ToolStripMenuItem selectSeasonDropDown { get; set; }
        public ToolStripMenuItem selectColumnsDropDown { get; set; }
        public ToolStripMenuItem filterToolStripMenuItem { get; set; }
        public DataGridView topTableDGV { get; set; }
        public DataGridView rightTableDGV { get; set; }
        public SaveFileDialog saveFileDialog { get; set; }
        public Label listTypeLabel { get; set; }
        public Label listNameLabel { get; set; }
        public TextBox renameListTextbox { get; set; }
        public Button changeSeasonButton { get; set; }

        public void Initialize()
        {
            SetupLoadListDropDown();
            SetupSaveDropDown();
            SetupCreateListButton();
            SetupDeleteListButton();
            SetupSetAsDefaultListButton();
            SetupLoadDraftButton();
            RefreshListType();
            SetupChangeSeasonButton();
            SetupRenameListLabel();
            SetupSelectSeasonTypeButtons();
            SetupSelectSeasonButton();
            SetupSelectColumnsButton();
            SetupFilterButton();
        }

        public void RefreshDropDownLists()
        {
            SetupLoadListDropDown();
            SetupSelectSeasonTypeButtons();
            SetupSelectSeasonButton();
            SetupSelectColumnsButton();
        }

        public void RefreshListTypeLabel()
        {
            string listType = form.currentPlayerList.listType.ToString();
            listType = listType.Replace("List", "");
            listType += " List";
            listTypeLabel.Text = listType;
        }

        public void RefreshListType()
        {
            RefreshListTypeLabel();
            changeSeasonButton.Visible = false;
            selectSeasonDropDown.Enabled = true;

            switch (form.currentPlayerList.listType)
            {
                case PlayerList.ListType.GeneralList:
                    break;
                case PlayerList.ListType.DraftList:
                    break;
                case PlayerList.ListType.TeamList:
                    changeSeasonButton.Visible = true;
                    selectSeasonDropDown.Enabled = false;
                    break;
            }
        }

        public void SetListLabel(string listName)
        {
            listNameLabel.Text = listName;
            listNameLabel.Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            while (listNameLabel.Bounds.IntersectsWith(selectPrimarySeasonTypeDropDown.Bounds))
            {
                listNameLabel.Font = new Font("Microsoft Sans Serif", listNameLabel.Font.Size - 1, FontStyle.Bold);
            }
        }

        private void SetupLoadListDropDown()
        {
            ToolStripItemCollection loadListDropDownItems = loadListDropDown.DropDownItems;
            loadListDropDownItems.Clear();
            string[] playerListFiles = form.GetPlayerListsInDirectory();
            foreach (string file in playerListFiles)
            {
                // Get the name without the file path or suffix
                string listName = file.Substring(0, file.Length - Constants.LIST_NAME_SUFFIX.Length);

                PlayerList playerListToLoad = Serializer.ReadXML<PlayerList>(listName + Constants.LIST_NAME_SUFFIX);
                EventHandler selectPlayerListHandler = new EventHandler((object sender, EventArgs e) =>
                {
                    Action LoadPlayer = () =>
                    {
                        // If it's the current/upcoming season, more players could be added as the season progresses, so get the full roster again
                        if (playerListToLoad.listType == PlayerList.ListType.TeamList && playerListToLoad.displaySeason == Constants.MostRecentSeason)
                        {
                            playerListToLoad.SetPlayerIds(TeamListManager.GetPlayerIdsOnTeam(playerListToLoad.teamId, playerListToLoad.displaySeason));
                            Serializer.WriteXML(playerListToLoad, listName + Constants.LIST_NAME_SUFFIX);
                        }

                        form.LoadPlayerList(playerListToLoad, listName);
                        RefreshDropDownLists();
                    };

                    form.TriggerLeaveRequest(LoadPlayer);
                });

                loadListDropDownItems.Add(listName, null, selectPlayerListHandler);
                if (form.currentListName == listName)
                {
                    ((ToolStripMenuItem)loadListDropDownItems[loadListDropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupSaveDropDown()
        {
            saveToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                if (form.currentListName != Constants.DEFAULT_LIST_NAME)
                {
                    SaveList(form.currentListName);
                    MessageBox.Show("Saved List.");
                }
            });

            saveFileDialog.Filter = "Player List|*" + Constants.LIST_NAME_SUFFIX;
            saveFileDialog.Title = "Save Player List";
            saveAsToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                saveFileDialog.FileName = (form.currentListName != Constants.DEFAULT_LIST_NAME) ? form.currentListName : "";
                DialogResult result = saveFileDialog.ShowDialog();
                if (result == DialogResult.OK || result == DialogResult.Yes)
                {
                    string listName = TrimFileNameSuffix(Path.GetFileName(saveFileDialog.FileName));
                    if (listName == Constants.DEFAULT_LIST_NAME)
                    {
                        MessageBox.Show("Invalid Name. The list was not saved.");
                    }
                    else
                    {
                        SaveList(listName);
                    }
                }
            });
        }

        private void SetupCreateListButton()
        {
            createListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Action CreateList = () =>
                {
                    form.LoadEmptyList();
                };

                form.TriggerLeaveRequest(CreateList);
            });
        }

        private void SetupDeleteListButton()
        {
            Action DeleteList = () =>
            {
                File.Delete(form.currentListName + Constants.LIST_NAME_SUFFIX);
                form.LoadDefaultOrEmptyList();
            };

            deleteListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                if (form.currentPlayerList.listStatus == PlayerList.ListStatus.Generated)
                {
                    MessageBox.Show("This is a Generated List that has not been saved. It cannot be deleted.");
                    return;
                }

                string message = "Are you sure you want to delete this list? This cannot be undone.";
                form.DisplayYesNoMessageBox(message, DeleteList);
            });
        }

        private void SetupSetAsDefaultListButton()
        {
            setAsDefaultListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Configuration configuration = form.configuration;
                if (form.currentPlayerList.listStatus == PlayerList.ListStatus.Generated)
                {
                    MessageBox.Show("This is a Generated List that has not been saved. It cannot be set as default.");
                }
                else if (configuration.defaultList == form.currentListName)
                {
                    MessageBox.Show("This is already the default list.");
                }
                else
                {
                    configuration.defaultList = form.currentListName;
                    Serializer.WriteXML<Configuration>(configuration, Constants.CONFIGURATION_FILE_NAME);
                    MessageBox.Show("Default list updated.");
                }
            });
        }

        private void SetupLoadDraftButton()
        {
            loadDraftToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                LoadDraftModal loadDraftModal = new LoadDraftModal(form);
                loadDraftModal.ShowDialog();
            });
        }

        private void SetupChangeSeasonButton()
        {
            changeSeasonButton.Click += new EventHandler((object sender, EventArgs e) =>
            {
                ChangeSeasonModal changeSeasonModal = new ChangeSeasonModal(form);
                List<string> seasons = TeamListManager.GetTeamSeasons(form.currentPlayerList.teamId);
                Action ChangeSeason = () =>
                {
                    form.TriggerLeaveRequest(() =>
                    {
                        string teamId = form.currentPlayerList.teamId;
                        string season = changeSeasonModal.GetDomainUpDownText();
                        List<string> playerIds = TeamListManager.GetPlayerIdsOnTeam(teamId, season);
                        if (playerIds.Count == 0)
                        {
                            MessageBox.Show("The team did not play/exist in the " + season + " season");
                            return;
                        }
                        form.currentPlayerList.SetPlayerIds(playerIds);
                        form.currentPlayerList.SetDisplaySeason(season);
                        if (form.currentPlayerList.listStatus == PlayerList.ListStatus.Generated)
                        {
                            form.currentListName = form.currentListName.Substring(0, form.currentListName.LastIndexOf('(') + 1) + season + ")";
                        }
                        else
                        {
                            form.currentListName = String.Format("{0} ({1})", TeamListManager.GetTeamName(teamId), season);
                        }
                        form.currentPlayerList.SetListStatus(PlayerList.ListStatus.Generated);
                        form.LoadPlayerList(form.currentPlayerList, form.currentListName);

                        changeSeasonModal.Close();
                    });
                    
                };
                changeSeasonModal.ShowDialog(ChangeSeason, seasons);
            });
        }

        private void SetupRenameListLabel()
        {
            listNameLabel.Click += new EventHandler((object sender, EventArgs e) => {
                if (form.lastSavedPlayerList.listStatus == PlayerList.ListStatus.Generated) { return; }

                listNameLabel.Visible = false;
                renameListTextbox.Visible = true;
                renameListTextbox.Enabled = true;
                renameListTextbox.SetBounds(listNameLabel.Left, 1, 200, listNameLabel.Height);
                renameListTextbox.Text = form.currentListName;
                renameListTextbox.Focus();
            });

            Action LeaveTextBox = () =>
            {
                renameListTextbox.Visible = false;
                renameListTextbox.Enabled = false;
                listNameLabel.Visible = true;
            };

            renameListTextbox.LostFocus += new EventHandler((object sender, EventArgs e) => {
                LeaveTextBox();
            });

            renameListTextbox.KeyUp += new KeyEventHandler((object sender, KeyEventArgs e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    // Rename file if is exists and the listName has been changed
                    if (File.Exists(form.currentListName + Constants.LIST_NAME_SUFFIX))
                    {
                        File.Move(form.currentListName + Constants.LIST_NAME_SUFFIX, renameListTextbox.Text + Constants.LIST_NAME_SUFFIX);
                    }

                    // If the renamed list is the default list, update the default list with the new name
                    if (form.configuration.defaultList == form.currentListName)
                    {
                        form.configuration.defaultList = renameListTextbox.Text;
                        Serializer.WriteXML<Configuration>(form.configuration, Constants.CONFIGURATION_FILE_NAME);
                    }

                    form.currentListName = renameListTextbox.Text;
                    string asterisk = (listNameLabel.Text.EndsWith("*")) ? "*" : ""; // Add the asterisk back onto the label if necessary
                    SetListLabel(renameListTextbox.Text + asterisk);
                    RefreshDropDownLists();

                    LeaveTextBox();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    LeaveTextBox();
                }
            });
        }

        private void SetupSelectSeasonTypeButtons()
        {
            Action<ToolStripMenuItem, PlayerStatTable> SetupButton = new Action<ToolStripMenuItem, PlayerStatTable>((ToolStripMenuItem menu, PlayerStatTable playerStatTable) => {
                menu.Text = playerStatTable.GetSeasonType();
                ToolStripItemCollection dropDownItems = menu.DropDownItems;
                dropDownItems.Clear();

                foreach (string seasonType in Constants.SeasonTypes)
                {
                    EventHandler selectSeasonTypeHandler = new EventHandler((object sender, EventArgs e) => {
                        playerStatTable.SetSeasonType(seasonType);
                        RefreshDropDownLists();
                        form.playerStatTablesManager.RedrawRowColors();
                        form.SetListStatus(PlayerList.ListStatus.Unsaved);
                    });
                    dropDownItems.Add(seasonType, null, selectSeasonTypeHandler);
                    if (playerStatTable.GetSeasonType() == seasonType)
                    {
                        ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                    }
                }
            });

            SetupButton(selectPrimarySeasonTypeDropDown, form.topTable);
            SetupButton(selectSecondarySeasonTypeDropDown, form.rightTable);
        }

        private void SetupSelectSeasonButton()
        {
            selectSeasonDropDown.Text = form.currentPlayerList.displaySeason;
            ToolStripItemCollection dropDownItems = selectSeasonDropDown.DropDownItems;
            dropDownItems.Clear();

            string[] years = Constants.MostRecentSeason.Split('-');
            int startYear = Int32.Parse(years[0]);
            int endYear = Int32.Parse(years[1]);

            for (int i = 0; i < 15; i++)
            {
                string season = String.Format("{0}-{1}", startYear - i, endYear - i);
                EventHandler selectSeasonHandler = new EventHandler((object sender, EventArgs e) => {
                    form.topTable.ChangeDisplaySeason(season);
                    form.currentPlayerList.SetDisplaySeason(season);
                    RefreshDropDownLists();
                    form.SetListStatus(PlayerList.ListStatus.Unsaved);
                });
                dropDownItems.Add(season, null, selectSeasonHandler);
                if (form.currentPlayerList.displaySeason == season)
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }

            // Add the "Other" option
            EventHandler selectOtherHandler = new EventHandler((object sender, EventArgs e) =>
            {
                ChangeSeasonModal changeSeasonModal = new ChangeSeasonModal(form);
                List<string> seasons = new List<string>();
                string loopSeason = Constants.MostRecentSeason;
                while (loopSeason != "1924-1925")
                {
                    seasons.Add(loopSeason);
                    string[] splitValues = loopSeason.Split('-');
                    string lowerValue = (Int32.Parse(splitValues[0]) - 1).ToString();
                    string upperValue = splitValues[0];
                    loopSeason = String.Format("{0}-{1}", lowerValue, upperValue);
                }
                Action ChangeSeason = () =>
                {
                    string season = changeSeasonModal.GetDomainUpDownText();
                    form.topTable.ChangeDisplaySeason(season);
                    form.currentPlayerList.SetDisplaySeason(season);
                    RefreshDropDownLists();
                    form.SetListStatus(PlayerList.ListStatus.Unsaved);

                    changeSeasonModal.Close();
                };
                changeSeasonModal.ShowDialog(ChangeSeason, seasons);
            });
            dropDownItems.Add("Other", null, selectOtherHandler);
        }

        private void SetupSelectColumnsButton()
        {
            ToolStripItemCollection dropDownItems = selectColumnsDropDown.DropDownItems;
            dropDownItems.Clear();
            foreach (string columnName in Constants.AllPossibleColumnsAlphebetized)
            {
                EventHandler selectColumnHandler = new EventHandler((object sender, EventArgs e) => {
                    ToolStripMenuItem dropDownItem = (ToolStripMenuItem)sender;
                    if (dropDownItem.Checked)
                    {
                        form.topTable.RemoveColumn(dropDownItem.Text);
                    }
                    else
                    {
                        form.topTable.AddColumn(dropDownItem.Text, form.currentPlayerList.displaySeason);
                    }
                    dropDownItem.Checked = !dropDownItem.Checked;
                    form.playerStatTablesManager.RedrawColumnWidths(topTableDGV, form.currentPlayerList.GetPrimaryColumnWidth, form.currentPlayerList.SetPrimaryColumnWidths);
                    form.SetListStatus(PlayerList.ListStatus.Unsaved);
                });
                dropDownItems.Add(columnName, null, selectColumnHandler);
                if (form.topTable.ContainsColumn(columnName))
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupFilterButton()
        {
            filterToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Action<FilterManager.FilterType> SetAllPossibleValues = (FilterManager.FilterType filterType) =>
                {
                    List<string> values = form.topTable.GetFilterableValuesBySeason(filterType, form.currentPlayerList.displaySeason);
                    //if (form.filter.autoFilterOut)
                    //{
                    //    List<string> newValues = values.Where((string value) => !form.filter.GetAllValues(filterType).Contains(value)).ToList();
                    //    foreach (string newValue in newValues)
                    //    {
                    //        form.filter.FilterOutValue(filterType, newValue);
                    //    }
                    //}
                    form.filter.SetAllValues(filterType, values);
                };
                SetAllPossibleValues(FilterManager.FilterType.League);
                SetAllPossibleValues(FilterManager.FilterType.Team);
                SetAllPossibleValues(FilterManager.FilterType.DraftTeam);

                FilterModal filterModal = new FilterModal(form, form.filter);
                filterModal.ShowDialog(() =>
                {
                    form.topTable.SetFilter(form.filter);
                });
            });
        }

        private string TrimFileNameSuffix(string fullFileName)
        {
            if (fullFileName.EndsWith(Constants.LIST_NAME_SUFFIX)) { fullFileName = fullFileName.Remove(fullFileName.IndexOf(Constants.LIST_NAME_SUFFIX)); }

            while (fullFileName.EndsWith(Constants.LIST_NAME_SUFFIX_NO_XML))
            {
                fullFileName = fullFileName.Remove(fullFileName.IndexOf(Constants.LIST_NAME_SUFFIX_NO_XML));
            }

            return fullFileName;
        }

        private void SaveList(string listName)
        {
            form.currentPlayerList.SetPrimaryColumnWidths(topTableDGV.Columns);
            form.currentPlayerList.SetSecondaryColumnWidths(rightTableDGV.Columns);

            form.currentPlayerList.SetListStatus(PlayerList.ListStatus.Saved);
            Serializer.WriteXML<PlayerList>(form.currentPlayerList, listName + Constants.LIST_NAME_SUFFIX);
            form.lastSavedPlayerList = form.currentPlayerList.Clone();
            form.currentListName = listName;
            RefreshDropDownLists();
            form.SetListStatus(PlayerList.ListStatus.Saved);
        }
    }
}
