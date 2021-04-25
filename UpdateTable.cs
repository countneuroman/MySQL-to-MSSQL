using System.Data;


namespace MySqlToMsSql
{
    static class UpdateTable
    {
        public static string UpdateCommand(string tableName, DataTable tableData)
        {
            string updateTable = "insert into " + tableName + "(";

            for (int i = 0; i < tableData.Columns.Count; i++)
            {
                if (tableData.Columns[i].ColumnName != "record_id")
                {
                    updateTable += tableData.Columns[i].ColumnName + ",";
                }
            }

            return updateTable.Substring(0, updateTable.Length - 1) + ")";
        }
    }
}
