using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HockeyStats
{
    public class PlayerConstantStatTable : PlayerStatTable
    {
        public PlayerConstantStatTable(DataGridView dataGridView)
            : base(dataGridView, new List<string>() { "Key", "Value" } )
        {
            dataGridView.ColumnHeadersVisible = false;
        }
        
        public void AddPlayerByDisplayDict(Dictionary<string, string> displayDict)
        {
            foreach (KeyValuePair<string, string> keyValuePair in displayDict)
            {
                if (keyValuePair.Value.Contains(Environment.NewLine) == false)
                {
                    Dictionary<string, string> entry = new Dictionary<string, string>() {
                        { "Key", keyValuePair.Key },
                        { "Value", keyValuePair.Value }
                    };
                    AddRowToDataTable(entry);
                }
            }
        }

        public void ClearTable()
        {
            dataTable.Clear();
        }
    }
}
