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
        private static List<string> integerColumns = new List<string>()
        {
            "Draft Year",
            "Draft Round",
            "Draft Overall"
        };

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
                string value;
                if (displayDict.TryGetValue(dataTable.Columns[i].ColumnName, out value))
                {
                    orderedRowValues[i] = value;
                }
            }
            return dataTable.Rows.Add(orderedRowValues);
        }

        private DataTable CreateDataTable(DataGridView dgv)
        {
            DataTable dataTable = new DataTable();
            foreach (string columnName in columnData)
            {
                if (integerColumns.Contains(columnName))
                {
                    dataTable.Columns.Add(new DataColumn(columnName, 1.GetType()));
                }
                else
                {
                    dataTable.Columns.Add(new DataColumn(columnName));
                }
            }

            dgv.DataSource = dataTable;
            return dataTable;
        }
    }
}
