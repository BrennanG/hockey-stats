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
        private Dictionary<int, string> rowHashToIdMap = new Dictionary<int, string>();

        public PlayerStatTable(DataGridView dataGridView, List<string> columnData)
        {
            this.dataGridView = dataGridView;
            this.columnData = columnData;

            dataTable = CreateDataTable(this.dataGridView);
        }

        public string GetIdByRow(DataRow dataRow)
        {
            return rowHashToIdMap[dataRow.GetHashCode()];
        }

        protected void AddRowToDataTable(Dictionary<string, string> displayDict)
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
            DataRow newRow = dataTable.Rows.Add(orderedRowValues);
            string playerId;
            if (displayDict.TryGetValue("ID", out playerId))
            {
                rowHashToIdMap[newRow.GetHashCode()] = playerId;
            }
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
