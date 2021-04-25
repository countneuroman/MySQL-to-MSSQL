
using System.Data;

namespace MySqlToMsSql
{
    static class CreateTable
    {
        public static string CreateCommand(string tableName, DataTable table)
        {
            string createTable;

            createTable = "CREATE TABLE " + tableName + "(";

            for (int i = 0; i < table.Columns.Count; i++)
            {
                createTable += "\n [" + table.Columns[i].ColumnName + "] ";

                string columnType = table.Columns[i].DataType.ToString();

                switch (columnType)
                {
                    case "System.Int32":
                        createTable += " int ";
                        break;
                    case "System.String":
                    default:
                        createTable += string.Format(" nvarchar({0}) ", table.Columns[i].MaxLength == -1 ? "max" : table.Columns[i].MaxLength.ToString());
                        break;
                }

                if (table.Columns[i].ColumnName == "record_id")
                {
                    //По неизвестной мне причине при парсинге таблицы БД в DataTable неправильно указываются значекния AutoIncrementSeed и AutoIncrement.
                    //Из-за этого я не могу определить какой из столбцов является IDENTITY. Но в нашем случае это значение заранее извесно.
                    // createTable += " IDENTITY(" + table.Columns[i].AutoIncrementSeed.ToString() + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                    createTable += " IDENTITY(" + "1" + "," + table.Columns[i].AutoIncrementStep.ToString() + ") ";
                }

                if (!table.Columns[i].AllowDBNull)
                {
                    createTable += " NOT NULL ";
                }

                createTable += ",";
            }

            return createTable.Substring(0, createTable.Length - 1) + "\n)";
        }
    }
}
