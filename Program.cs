using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
 
namespace MySqlToMsSql
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionMySql = @"server=127.0.0.1;uid=root;pwd=19953107;database=usersdb";
            string connectionMsSql = @"Server=(localdb)\mssqllocaldb;Database=usersdb;Trusted_Connection=True;";

            DataSet tableData = new DataSet();  //Хранит данные, полученные из таблицы БД.

            using (MySqlConnection con = new MySql.Data.MySqlClient.MySqlConnection(connectionMySql))
            {
                con.Open(); //Подключаемся к БД.

                //Получаем данные из БД.
                using (MySqlDataAdapter tableDataAdapter = new MySqlDataAdapter("select * from users order by record_id desc limit 500", con))
                {
                    tableDataAdapter.AcceptChangesDuringFill = false;
                    tableDataAdapter.Fill(tableData);
                }
            }


            using (SqlConnection con = new SqlConnection(connectionMsSql))
            {
                string addTableCommand = null;
                string addDataTableCommand = null;

                con.Open();

                //Парсим данные, полученные из БД.
                foreach (DataTable dt in tableData.Tables)
                {
                    addTableCommand = CreateTable.CreateCommand("users ", dt);
                    addDataTableCommand = UpdateTable.UpdateCommand("users", dt);
                }

                //Создаем таблицу в БД.
                using (SqlCommand addTable = new SqlCommand(addTableCommand, con))
                {
                    addTable.ExecuteNonQuery();
                }

                //Отдельно парсим значения из таблицы и заполняем ими созданную ранее таблицу.
                foreach (DataTable dt in tableData.Tables)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string addData = null;
                        addData += addDataTableCommand + "values (";

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            if (dt.Columns[i].ColumnName != "record_id")
                            {
                                addData += "'" + Convert.ToString(row[i]) + "',";
                            }
                        }

                        addData = addData.Substring(0, addData.Length - 1) + ")";

                        using (SqlCommand addDataCommand = new SqlCommand(addData, con))
                        {
                            addDataCommand.ExecuteNonQuery();
                        }
                    }
                }
            }

            Console.WriteLine("Готово!");
            Console.ReadLine();
        }     
    }

}