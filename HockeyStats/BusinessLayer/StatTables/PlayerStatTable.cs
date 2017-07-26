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
        protected DataTable dataTable = new DataTable();
        protected DataGridView dataGridView;
        
        public PlayerStatTable(DataGridView dataGridView, List<string> columnNames)
        {
            this.dataGridView = dataGridView;

            InitializeTable(columnNames);
            DisableSortingOnDynamicColumns();
        }

        public bool ContainsColumn(string columnName)
        {
            foreach (DataColumn column in dataTable.Columns)
            {
                if (column.ColumnName == columnName) { return true; }
            }
            return false;
        }

        public static DataRow GetDataRowFromDGVRow(DataGridViewRow dgvRow)
        {
            return ((DataRowView)dgvRow.DataBoundItem).Row;
        }

        public virtual string GetSeasonType()
        {
            throw new NotImplementedException();
        }

        public virtual void SetSeasonType(string newSeasonType)
        {
            throw new NotImplementedException();
        }

        protected DataRow AddRowToDataTable(Dictionary<string, string> statDict)
        {
            string[] orderedRowValues = new string[dataTable.Columns.Count];
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                // If the column exists in the row, add the value
                string value;
                if (statDict.TryGetValue(dataTable.Columns[i].ColumnName, out value))
                {
                    orderedRowValues[i] = value;
                }
            }
            return dataTable.Rows.Add(orderedRowValues);
        }

        protected void RemoveColumn(string columnName)
        {
            dataTable.Columns.Remove(columnName);
        }

        protected void AddColumn(string columnName)
        {
            if (Constants.NumericColumns.Contains(columnName))
            {
                dataTable.Columns.Add(new DataColumn(columnName, new int().GetType()));
            }
            else
            {
                dataTable.Columns.Add(new DataColumn(columnName));
            }
            dataGridView.Columns[dataGridView.Columns.Count - 1].ToolTipText = columnName;
        }

        protected void AddColumns(List<string> columnNames)
        {
            foreach (string columnName in columnNames)
            {
                AddColumn(columnName);
            }
        }

        private void InitializeTable(List<string> columnNames)
        {
            dataGridView.Columns.Clear();
            dataGridView.DataSource = dataTable;
            foreach (string columnName in columnNames)
            {
                AddColumn(columnName);
            }
        }

        private void DisableSortingOnDynamicColumns()
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (Constants.DynamicColumns.Contains(column.Name))
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
        }
    }
}
