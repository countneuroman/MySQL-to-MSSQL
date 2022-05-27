using System.Data;


namespace MySqlToMsSql
{
    internal static class UpdateTable
    {
        public static string UpdateCommand(string tableName, DataTable tableData)
        {
            var updateTable = "insert into " + tableName + "(";

            for (var i = 0; i < tableData.Columns.Count; i++)
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
