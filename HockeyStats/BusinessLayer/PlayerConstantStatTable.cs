﻿using System;
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
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
        }

        public void AddPlayerByDisplayDict(Dictionary<string, string> displayDict)
        {
            foreach (KeyValuePair<string, string> keyValuePair in displayDict)
            {
                if (keyValuePair.Value != null && keyValuePair.Value.Contains(Environment.NewLine) == false)
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

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView.ClearSelection();
        }
    }
}
