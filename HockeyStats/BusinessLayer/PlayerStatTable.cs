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
            DisableSortingOnDynamicColumns();
        }

        public void RemoveColumn(string columnName)
        {
            dataTable.Columns.Remove(columnName);
        }

        public void AddColumn(string columnName)
        {
            if (Columns.AllPossibleColumns.Contains(columnName) && !dataTable.Columns.Contains(columnName))
            {
                if (Columns.NumericColumns.Contains(columnName))
                {
                    dataTable.Columns.Add(new DataColumn(columnName, new int().GetType()));
                }
                else
                {
                    dataTable.Columns.Add(new DataColumn(columnName));
                }
            }
        }

        protected DataRow AddRowToDataTable(Dictionary<string, string> displayDict)
        {
            string[] orderedRowValues = new string[dataTable.Columns.Count];
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
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
                if (Columns.NumericColumns.Contains(columnName))
                {
                    dataTable.Columns.Add(new DataColumn(columnName, new int().GetType()));
                }
                else
                {
                    dataTable.Columns.Add(new DataColumn(columnName));
                }
            }
            
            dgv.DataSource = dataTable;
            return dataTable;
        }

        private void DisableSortingOnDynamicColumns()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (Columns.DynamicColumns.Contains(column.Name))
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
        }
    }
}
