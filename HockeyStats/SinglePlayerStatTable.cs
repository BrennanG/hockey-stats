using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class SinglePlayerStatTable : PlayerStatTable
    {
        public SinglePlayerStatTable(DataGridView dataGridView, List<string> columnData)
            : base(dataGridView, columnData)
        {
        }

        public void AddPlayerByDisplayDict(Dictionary<string, string> displayDict)
        {
            List<List<string>> listOfSplitValues = new List<List<string>>();
            int maxNumberOfValues = 0;
            foreach (string value in displayDict.Values)
            {
                List<string> splitValues = value.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                listOfSplitValues.Add(splitValues);
                maxNumberOfValues = Math.Max(maxNumberOfValues, splitValues.Count());
            }
            
            for (int i = 0; i < maxNumberOfValues; i++)
            {
                Dictionary<string, string> tempDict = new Dictionary<string, string>();
                int j = 0;
                foreach (string key in displayDict.Keys)
                {
                    List<string> valuesForKey = listOfSplitValues[j];
                    int index = (i <= valuesForKey.Count() - 1) ? i : valuesForKey.Count() - 1;
                    tempDict.Add(key, valuesForKey[index]);
                    j++;
                }
                AddRowToDataTable(tempDict);
            }
        }

        public void ClearPlayersFromTable()
        {
            dataTable.Clear();
        }
    }
}
