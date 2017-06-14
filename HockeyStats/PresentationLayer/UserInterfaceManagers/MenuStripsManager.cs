using System;
using System.Collections.Generic;
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
        public ToolStripMenuItem selectPrimarySeasonTypeDropDown { get; set; }
        public ToolStripMenuItem selectSecondarySeasonTypeDropDown { get; set; }
        public ToolStripMenuItem selectSeasonDropDown { get; set; }
        public ToolStripMenuItem addRemoveColumnDropDown { get; set; }
        public DataGridView topTableDGV { get; set; }
        public DataGridView rightTableDGV { get; set; }
        public SaveFileDialog saveFileDialog { get; set; }
        public Label listNameLabel { get; set; }
        public TextBox renameListTextbox { get; set; }

        public void Initialize()
        {
            SetupLoadListDropDown();
            SetupSaveDropDown();
            SetupCreateListButton();
            SetupDeleteListButton();
            SetupSetAsDefaultListButton();
            SetupRenameListLabel();
            SetupSelectSeasonTypeButtons();
            SetupSelectSeasonButton();
            SetupAddRemoveColumnButton();
        }

        public void RefreshDropDownLists()
        {
            SetupLoadListDropDown();
            SetupSelectSeasonTypeButtons();
            SetupSelectSeasonButton();
            SetupAddRemoveColumnButton();
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
                form.LoadEmptyList();
            };

            deleteListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                string message = "Are you sure you want to delete this list? This cannot be undone.";
                form.DisplayYesNoMessageBox(message, DeleteList);
            });
        }

        private void SetupSetAsDefaultListButton()
        {
            setAsDefaultListToolStripMenuItem.Click += new EventHandler((object sender, EventArgs e) =>
            {
                Configuration configuration = form.configuration;
                if (configuration.defaultList == form.currentListName)
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

        private void SetupRenameListLabel()
        {
            listNameLabel.Click += new EventHandler((object sender, EventArgs e) => {
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
                    listNameLabel.Text = renameListTextbox.Text + asterisk;
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
                        form.SetListIsSaved(false);
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
            selectSeasonDropDown.Text = form.currentDisplaySeason;
            ToolStripItemCollection dropDownItems = selectSeasonDropDown.DropDownItems;
            dropDownItems.Clear();

            string[] years = Constants.CurrentSeason.Split('-');
            int startYear = Int32.Parse(years[0]);
            int endYear = Int32.Parse(years[1]);

            for (int i = 0; i < 15; i++)
            {
                string season = String.Format("{0}-{1}", startYear - i, endYear - i);
                EventHandler selectSeasonHandler = new EventHandler((object sender, EventArgs e) => {
                    form.topTable.ChangeDisplaySeason(season);
                    form.currentDisplaySeason = season;
                    RefreshDropDownLists();
                    form.SetListIsSaved(false);
                });
                dropDownItems.Add(season, null, selectSeasonHandler);
                if (form.currentDisplaySeason == season)
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }
        }

        private void SetupAddRemoveColumnButton()
        {
            ToolStripItemCollection dropDownItems = addRemoveColumnDropDown.DropDownItems;
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
                        form.topTable.AddColumn(dropDownItem.Text, form.currentDisplaySeason);
                    }
                    dropDownItem.Checked = !dropDownItem.Checked;
                    form.playerStatTablesManager.RedrawColumnWidths(topTableDGV, form.currentPlayerList.GetPrimaryColumnWidth, form.currentPlayerList.SetPrimaryColumnWidths);
                    form.SetListIsSaved(false);
                });
                dropDownItems.Add(columnName, null, selectColumnHandler);
                if (form.topTable.ContainsColumn(columnName))
                {
                    ((ToolStripMenuItem)dropDownItems[dropDownItems.Count - 1]).Checked = true;
                }
            }
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

            Serializer.WriteXML<PlayerList>(form.currentPlayerList, listName + Constants.LIST_NAME_SUFFIX);
            form.lastSavedPlayerList = form.currentPlayerList.Clone();
            form.currentListName = listName;
            RefreshDropDownLists();
            form.SetListIsSaved(true);
        }
    }
}
