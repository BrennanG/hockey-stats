using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HockeyStats
{
    public class MultiPlayerStatTable : PlayerStatTable
    {
        private PlayerList playerList;
        private Dictionary<int, Dictionary<string, string>> rowHashToSavedDictMap = new Dictionary<int, Dictionary<string, string>>();

        public MultiPlayerStatTable(DataGridView dataGridView, PlayerList playerList)
            : base(dataGridView, playerList.primaryTableColumnNames)
        {
            this.playerList = playerList;
            new Thread(() => FillDataTable()).Start(); // Fill the table in a separate thread
        }

        public void AddPlayerById(string playerId)
        {
            Dictionary<string, string> DisplayDict = new Dictionary<string, string>();
            Dictionary<string, string> SavedDict = new Dictionary<string, string>();

            PlayerStats playerStats = new PlayerStats(playerId, playerList.displayYears);
            playerStats.AddPlayerStatsToDicts(DisplayDict, SavedDict);

            DataRow newDataRow = AddRowToDataTable(DisplayDict);
            rowHashToSavedDictMap[newDataRow.GetHashCode()] = SavedDict;
        }

        public Dictionary<string, string> GetSavedDictFromRow(DataRow dataRow)
        {
            return rowHashToSavedDictMap[dataRow.GetHashCode()];
        }

        private void FillDataTable()
        {
            foreach (string playerId in playerList.playerIds)
            {
                AddPlayerById(playerId);
            }
        }
    }
}
