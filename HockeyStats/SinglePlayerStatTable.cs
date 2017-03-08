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
            try
            {
                dataGridView.Sort(dataGridView.Columns["Year"], System.ComponentModel.ListSortDirection.Ascending);
            }
            catch (Exception e) { }
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
        }

        public void AddPlayerByDisplayDict(Dictionary<string, string> displayDict)
        {
            Dictionary<string, List<string>> keyToListOfValues = new Dictionary<string, List<string>>();
            int maxNumberOfValues = 0;
            foreach (KeyValuePair<string, string> entry in displayDict)
            {
                List<string> splitValues = entry.Value.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                maxNumberOfValues = Math.Max(maxNumberOfValues, splitValues.Count());
                keyToListOfValues[entry.Key] = splitValues;
            }

            List<Dictionary<string, string>> listOfDictsToAdd = new List<Dictionary<string, string>>();
            for (int i = 0; i < maxNumberOfValues; i++)
            {
                Dictionary<string, string> dictForCurrentYear = listOfDictsToAdd.FirstOrDefault(d => keyToListOfValues["Year"][i] == d["Year"]);
                Dictionary<string, string> dictToAdd = new Dictionary<string, string>();
                foreach (KeyValuePair<string, List<string>> entry in keyToListOfValues)
                {
                    int index = (i <= entry.Value.Count() - 1) ? i : entry.Value.Count() - 1;
                    if (dictForCurrentYear == null)
                    {
                        dictToAdd[entry.Key] = entry.Value[index];
                    }
                    else if (entry.Key != "Year")
                    {
                        dictForCurrentYear[entry.Key] += Environment.NewLine + entry.Value[index];
                    }
                }
                if (dictToAdd.Count() > 0) { listOfDictsToAdd.Add(dictToAdd); }
            }

            listOfDictsToAdd.ForEach((Dictionary<string, string> d) => AddRowToDataTable(d));
        }

        public void ClearTable()
        {
            dataTable.Clear();
        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView.ClearSelection();
        }
    }
}
