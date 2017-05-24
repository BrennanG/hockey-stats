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
        public MenuStripsManager menuStripsManager;
        public PlayerButtonsManager playerButtonsManager;
        public PlayerStatTablesManager playerStatTablesManager;

        public MultiPlayerStatTable topTable;
        public SearchDataStatTable leftTable;
        public PlayerConstantsStatTable middleTable;
        public SinglePlayerStatTable rightTable;
        
        public PlayerList playerList = new PlayerList();
        public Configuration configuration = new Configuration();
        public string currentDisplaySeason;
        public string currentListName;
        public bool listIsSaved;
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
                createListToolStripMenuItem = createListToolStripMenuItem,
                deleteListToolStripMenuItem = deleteListToolStripMenuItem,
                setAsDefaultListToolStripMenuItem = setAsDefaultListToolStripMenuItem,
                selectPrimarySeasonTypeDropDown = selectPrimarySeasonTypeDropDown,
                selectSecondarySeasonTypeDropDown = selectSecondarySeasonTypeDropDown,
                selectSeasonDropDown = selectSeasonDropDown,
                addRemoveColumnDropDown = addRemoveColumnDropDown,
                topTableDGV = topTableDGV,
                rightTableDGV = rightTableDGV,
                saveFileDialog = saveFileDialog,
                listNameLabel = listNameLabel,
                renameListTextbox = renameListTextbox
            };
        }

        public void CreatePlayerButtonsManager()
        {
            playerButtonsManager = new PlayerButtonsManager()
            {
                form = this,
                searchPlayerButton = searchPlayerButton,
                clearSearchButton = clearSearchButton,
                addSelectedPlayerButton = addSelectedPlayerButton,
                removeSelectedPlayerButton = removeSelectedPlayerButton,
                leftTableDGV = leftTableDGV,
                topTableDGV = topTableDGV,
                searchPlayerTextbox = searchPlayerTextbox
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
            playerStatTablesManager.RedrawColumnWidths(topTableDGV, playerList.GetPrimaryColumnWidth);
            playerStatTablesManager.RedrawColumnWidths(rightTableDGV, playerList.GetSecondaryColumnWidth);
            playerStatTablesManager.RedrawRowColors();

            SetListIsSaved(true);
            playerStatTablesManager.tableHasBeenClicked = false;
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
    }
}
