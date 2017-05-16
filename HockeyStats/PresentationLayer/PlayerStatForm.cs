using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;
using System.Linq;
using System.ComponentModel;

namespace HockeyStats
{
    public partial class PlayerStatForm : Form
    {
        private MenuStripsManager menuStripsManager;
        private PlayerButtonsManager playerButtonsManager;
        private PlayerStatTablesManager playerStatTablesManager;

        public MultiPlayerStatTable topTable;
        public SearchDataStatTable leftTable;
        public PlayerConstantsStatTable middleTable;
        public SinglePlayerStatTable rightTable;
        
        public PlayerList playerList = new PlayerList();
        public Configuration configuration = new Configuration();
        public string currentDisplaySeason;
        public string currentListName;
        public bool listIsSaved;
        public bool tableHasBeenClicked;
        public bool rowJustSelected = false;

        public PlayerStatForm()
        {
            InitializeComponent();

            ReadConfigurationFile();

            CreateMenuStripsManager();
            CreatePlayerButtonsManager();
            CreatePlayerStatTablesManager();

            LoadDefaultList();

            menuStripsManager.Initialize();
            playerButtonsManager.Initialize();
            playerStatTablesManager.Initialize();
            
            SetupFormClosingHandler();
        }

        public void ReadConfigurationFile()
        {
            configuration = Serializer.ReadXML<Configuration>(Constants.CONFIGURATION_FILE_NAME);
        }

        public void CreateMenuStripsManager()
        {
            menuStripsManager = new MenuStripsManager()
            {
                form = this,
                loadListDropDown = loadListDropDown,
                saveToolStripMenuItem = saveToolStripMenuItem,
                saveAsToolStripMenuItem = saveAsToolStripMenuItem,
                saveFileDialog = saveFileDialog,
                createListToolStripMenuItem = createListToolStripMenuItem,
                deleteListToolStripMenuItem = deleteListToolStripMenuItem,
                setAsDefaultListToolStripMenuItem = setAsDefaultListToolStripMenuItem,
                listNameLabel = listNameLabel,
                renameListTextbox = renameListTextbox,
                selectPrimarySeasonTypeDropDown = selectPrimarySeasonTypeDropDown,
                selectSecondarySeasonTypeDropDown = selectSecondarySeasonTypeDropDown,
                selectSeasonDropDown = selectSeasonDropDown,
                addRemoveColumnDropDown = addRemoveColumnDropDown,
                topTableDGV = topTableDGV,
                rightTableDGV = rightTableDGV
            };
        }

        public void CreatePlayerButtonsManager()
        {
            playerButtonsManager = new PlayerButtonsManager()
            {
                form = this,
                searchPlayerTextbox = searchPlayerTextbox,
                searchPlayerButton = searchPlayerButton,
                clearSearchButton = clearSearchButton,
                leftTableDGV = leftTableDGV,
                addSelectedPlayerButton = addSelectedPlayerButton,
                topTableDGV = topTableDGV,
                removeSelectedPlayerButton = removeSelectedPlayerButton
            };
        }

        public void CreatePlayerStatTablesManager()
        {
            playerStatTablesManager = new PlayerStatTablesManager()
            {
                form = this,
                topTableDGV = topTableDGV,
                leftTableDGV = leftTableDGV,
                middleTableDGV = middleTableDGV,
                rightTableDGV = rightTableDGV
            };
        }

        public void LoadPlayerList(PlayerList playerListToLoad, string listName)
        {
            playerList = playerListToLoad;
            currentDisplaySeason = playerList.displaySeason;
            currentListName = listName;
            listNameLabel.Text = listName;
            if (topTable != null) { topTable.AbortFillDataTableThread(); }

            topTable = new MultiPlayerStatTable(topTableDGV, playerList.primaryColumnNames, playerList.playerIds, playerList.displaySeason, playerList.primarySeasonType);
            if (leftTable == null) { leftTable = new SearchDataStatTable(leftTableDGV, Constants.DefaultSearchDataTableColumns); }
            middleTable = new PlayerConstantsStatTable(middleTableDGV);
            rightTable = new SinglePlayerStatTable(rightTableDGV, playerList.secondaryColumnNames, playerList.secondarySeasonType);

            menuStripsManager.RefreshDropDownLists();
            RedrawColumnWidths(topTableDGV, playerList.GetPrimaryColumnWidth);
            RedrawColumnWidths(rightTableDGV, playerList.GetSecondaryColumnWidth);
            RedrawRowColors();

            SetListIsSaved(true);
            tableHasBeenClicked = false;
        }

        public void LoadDefaultList()
        {
            string[] playerListsInDirectory = GetPlayerListsInDirectory();
            if (playerListsInDirectory.Contains(configuration.defaultList + Constants.LIST_NAME_SUFFIX))
            {
                PlayerList playerList = Serializer.ReadXML<PlayerList>(configuration.defaultList + Constants.LIST_NAME_SUFFIX);
                LoadPlayerList(playerList, configuration.defaultList);
            }
            else
            {
                LoadEmptyList();
            }
        }

        public void LoadEmptyList()
        {
            PlayerList newPlayerList = new PlayerList();
            newPlayerList.FillWithDefaults();
            LoadPlayerList(newPlayerList, Constants.DEFAULT_LIST_NAME);
            SetListIsSaved(true);
        }

        private void SetupFormClosingHandler()
        {
            FormClosing += new FormClosingEventHandler((object sender, FormClosingEventArgs e) => {
                Action CancelLeave = () =>
                {
                    e.Cancel = true;
                };
                TriggerLeaveRequest(null, CancelLeave);
            });
        }

        public void ClearPlayerSelection()
        {
            topTableDGV.ClearSelection();
            leftTableDGV.ClearSelection();
            middleTable.ClearTable();
            rightTable.ClearTable();
        }

        public void RedrawColumnWidths(DataGridView dataGridView, Func<string, int> GetWidthFunction)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                int width = GetWidthFunction(column.Name);
                if (column.DisplayIndex != dataGridView.Columns.Count - 1 && width >= 0)
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    column.Width = width;
                }
                else
                {
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        public void RedrawRowColors()
        {
            if (topTable.GetSeasonType() == Constants.REGULAR_SEASON)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (topTable.GetSeasonType() == Constants.PLAYOFFS)
            {
                topTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
            }

            if (rightTable.GetSeasonType() == Constants.REGULAR_SEASON)
            {
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
            }
            else if (rightTable.GetSeasonType() == Constants.PLAYOFFS)
            {
                middleTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
                rightTableDGV.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(217, 235, 249);
            }

            HighlightDraftRowsInRightTable(rightTable.GetPlayerStats());
        }

        public void HighlightDraftRowsInRightTable(PlayerStats playerStats)
        {
            if (playerStats == null) { return; }
            foreach (DataGridViewRow DGVRow in rightTableDGV.Rows)
            {
                string season = DGVRow.Cells[Constants.SEASON].Value.ToString();
                string endYear = (season != String.Empty) ? season.Substring(5) : String.Empty;
                if (endYear == playerStats.GetDraftYear())
                {
                    DGVRow.DefaultCellStyle.BackColor = System.Drawing.Color.Turquoise;
                }
                else if (endYear == playerStats.GetFirstYearOfDraftEligibility())
                {
                    DGVRow.DefaultCellStyle.BackColor = System.Drawing.Color.DeepSkyBlue;
                }
            }
        }

        public void SetListIsSaved(bool boolean)
        {
            listIsSaved = boolean;
            listNameLabel.Text = (listIsSaved) ? currentListName : currentListName + "*";

            if (currentListName == Constants.DEFAULT_LIST_NAME)
            {
                deleteListToolStripMenuItem.Enabled = false;
            }
            else
            {
                deleteListToolStripMenuItem.Enabled = true;
            }

            if (listIsSaved && currentListName == Constants.DEFAULT_LIST_NAME)
            {
                createListToolStripMenuItem.Enabled = false;
            }
            else
            {
                createListToolStripMenuItem.Enabled = true;
            }

            saveToolStripMenuItem.Enabled = !boolean && currentListName != Constants.DEFAULT_LIST_NAME;
        }

        public void DisplayYesNoMessageBox(string message, Action yesAction = null, Action noAction = null)
        {
            DialogResult confirmResult = MessageBox.Show(message, "", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes && yesAction != null)
            {
                yesAction();
            }
            else if (confirmResult == DialogResult.No && noAction != null)
            {
                noAction();
            }
        }

        public void TriggerLeaveRequest(Action leaveAction = null, Action stayAction = null)
        {
            if (!listIsSaved)
            {
                DisplayYesNoMessageBox(Constants.ARE_YOU_SURE_LEAVE_MESSAGE, leaveAction, stayAction);
            }
            else if (leaveAction != null)
            {
                leaveAction();
            }
        }

        public string[] GetPlayerListsInDirectory()
        {
            string[] playerListFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + Constants.LIST_NAME_SUFFIX);
            string[] playerListFileNames = new string[playerListFiles.Length];
            int count = 0;
            foreach (string fileWithPath in playerListFiles)
            {
                playerListFileNames[count] = Path.GetFileName(fileWithPath);
                count++;
            }
            return playerListFileNames;
        }
    }
}
