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
    public abstract class PlayerStatTable
    {
        protected DataTable dataTable;
        protected DataGridView dataGridView;
        protected List<string> columnData;

        public PlayerStatTable(DataGridView dataGridView, List<string> columnData)
        {
            this.dataGridView = dataGridView;
            this.columnData = columnData;

            dataTable = CreateDataTable(this.dataGridView);
        }

        protected DataRow AddRowToDataTable(Dictionary<string, string> displayDict)
        {
            string[] orderedRowValues = new string[dataTable.Columns.Count];
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                string junk = String.Empty;
                // If the column exists in the row, add the value
                string columnName;
                if (displayDict.TryGetValue(dataTable.Columns[i].ColumnName, out columnName))
                {
                    orderedRowValues[i] = columnName;
                }
            }
            return dataTable.Rows.Add(orderedRowValues);
        }

        private DataTable CreateDataTable(DataGridView dgv)
        {
            DataTable dataTable = new DataTable();
            foreach (string columnName in columnData)
            {
                dataTable.Columns.Add(new DataColumn(columnName));
            }

            dgv.DataSource = dataTable;
            return dataTable;
        }
    }
}
