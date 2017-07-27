using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Drawing;

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

        public PlayerList currentPlayerList = new PlayerList();
        public PlayerList lastSavedPlayerList = new PlayerList();
        public Configuration configuration = new Configuration();

        public FilterManager filter = new FilterManager();
        public HistoryManager<PlayerList> historyManager = new HistoryManager<PlayerList>();
        
        public bool rowJustSelected = false;

        public PlayerStatForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            topTableDGV.DoubleBuffered(true);

            ReadConfigurationFile();

            CreateMenuStripsManager();
            CreatePlayerButtonsManager();
            CreatePlayerStatTablesManager();

            LoadDefaultOrEmptyList();

            menuStripsManager.Initialize();
            playerButtonsManager.Initialize();
            playerStatTablesManager.Initialize();
            
            SetupFormClosingHandler();
        }

        public void ReadConfigurationFile()
        {
            configuration = Serializer.ReadXML<Configuration>(Constants.CONFIGURATION_FILE_NAME);

            // Check that the configuration file has been updated with the most recent draft year
            string mostRecentDraftYear = DraftListManager.GetMostRecentDraftYear();
            if (configuration.draftYears == null || configuration.draftYears.Count == 0 || configuration.draftYears[0] != mostRecentDraftYear)
            {
                configuration.draftYears = DraftListManager.GetAllDraftYears();
            }

            // Fill any missing round number data in the configuration file
            if (configuration.draftYearToNumberOfRoundsMap == null) { configuration.draftYearToNumberOfRoundsMap = new SerializableDictionary<string, int>(); }
            foreach (string draftYear in configuration.draftYears)
            {
                if (!configuration.draftYearToNumberOfRoundsMap.ContainsKey(draftYear))
                {
                    int numOfRounds = DraftListManager.GetNumberOfRoundsInDraft(draftYear);
                    if (numOfRounds > 1)
                    {
                        configuration.draftYearToNumberOfRoundsMap[draftYear] = numOfRounds;
                    }
                }
            }

            Serializer.WriteXML(configuration, Constants.CONFIGURATION_FILE_NAME);
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
                loadDraftToolStripMenuItem = loadDraftToolStripMenuItem,
                selectPrimarySeasonTypeDropDown = selectPrimarySeasonTypeDropDown,
                selectSecondarySeasonTypeDropDown = selectSecondarySeasonTypeDropDown,
                selectSeasonDropDown = selectSeasonDropDown,
                selectColumnsDropDown = selectColumnsDropDown,
                filterToolStripMenuItem = filterToolStripMenuItem,
                topTableDGV = topTableDGV,
                rightTableDGV = rightTableDGV,
                saveFileDialog = saveFileDialog,
                listTypeLabel = listTypeLabel,
                listNameLabel = listNameLabel,
                renameListTextbox = renameListTextbox,
                backButton = backButton,
                forwardButton = forwardButton,
                changeTeamSeasonButton = changeSeasonButton
            };
        }

        public void CreatePlayerButtonsManager()
        {
            playerButtonsManager = new PlayerButtonsManager()
            {
                form = this,
                searchButton = searchButton,
                clearSearchButton = clearSearchButton,
                addPlayerOrLoadTeamButton = addPlayerOrLoadTeamButton,
                removeSelectedPlayerButton = removeSelectedPlayerButton,
                leftTableDGV = leftTableDGV,
                topTableDGV = topTableDGV,
                searchTextbox = searchTextbox,
                searchTypeDomainUpDown = searchTypeDomainUpDown
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

        public void LoadPlayerList(PlayerList playerListToLoad, bool fromHistory = false)
        {
            currentPlayerList = playerListToLoad;
            lastSavedPlayerList = currentPlayerList.Clone();
            menuStripsManager.SetListLabel(currentPlayerList.listName);
            filter = new FilterManager(currentPlayerList);

            playerStatTablesManager.tableHasBeenClicked = false;
            if (topTable != null) { topTable.AbortFillDataTableThread(); }

            topTable = new MultiPlayerStatTable(topTableDGV, currentPlayerList, filter);
            if (leftTable == null) { leftTable = new SearchDataStatTable(leftTableDGV, Constants.DefaultSearchPlayerDataTableColumns); }
            middleTable = new PlayerConstantsStatTable(middleTableDGV);
            rightTable = new SinglePlayerStatTable(rightTableDGV, currentPlayerList);

            menuStripsManager.RefreshDropDownLists();
            menuStripsManager.RefreshListType();
            playerStatTablesManager.RedrawColumnWidths(topTableDGV, lastSavedPlayerList.GetPrimaryColumnWidth, currentPlayerList.SetPrimaryColumnWidths);
            playerStatTablesManager.RedrawColumnWidths(rightTableDGV, lastSavedPlayerList.GetSecondaryColumnWidth, currentPlayerList.SetSecondaryColumnWidths);
            playerStatTablesManager.RedrawRowColors();

            SetListStatus(PlayerList.ListStatus.Saved);

            if (!fromHistory) { historyManager.AddItem(lastSavedPlayerList); }
            menuStripsManager.UpdateBackAndForwardButtons();
        }

        public void LoadDefaultOrEmptyList()
        {
            string[] playerListsInDirectory = GetPlayerListsInDirectory();
            if (playerListsInDirectory.Contains(configuration.defaultList + Constants.LIST_NAME_SUFFIX))
            {
                PlayerList playerList = Serializer.ReadXML<PlayerList>(configuration.defaultList + Constants.LIST_NAME_SUFFIX);
                LoadPlayerList(playerList);
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
            LoadPlayerList(newPlayerList);
        }

        public void SetListStatus(PlayerList.ListStatus status)
        {
            if (lastSavedPlayerList.listStatus == PlayerList.ListStatus.Generated) { return; }

            if (currentPlayerList.Equals(lastSavedPlayerList))
            {
                status = PlayerList.ListStatus.Saved;
            }
            currentPlayerList.SetListStatus(status);
            string currentListName = currentPlayerList.listName;
            string listName = (currentPlayerList.listStatus == PlayerList.ListStatus.Saved) ? currentListName : currentListName + "*";
            menuStripsManager.SetListLabel(listName);

            if (currentListName == Constants.DEFAULT_LIST_NAME)
            {
                deleteListToolStripMenuItem.Enabled = false;
            }
            else
            {
                deleteListToolStripMenuItem.Enabled = true;
            }

            if (currentPlayerList.listStatus == PlayerList.ListStatus.Saved && currentListName == Constants.DEFAULT_LIST_NAME)
            {
                createListToolStripMenuItem.Enabled = false;
            }
            else
            {
                createListToolStripMenuItem.Enabled = true;
            }

            saveToolStripMenuItem.Enabled = currentPlayerList.listStatus != PlayerList.ListStatus.Saved && currentListName != Constants.DEFAULT_LIST_NAME;
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
            if (currentPlayerList.listStatus == PlayerList.ListStatus.Unsaved)
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
