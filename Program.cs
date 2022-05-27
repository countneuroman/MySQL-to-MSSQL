using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
 
namespace MySqlToMsSql
{
    class Program
    {
        static void Main()
        {
            var connectionMySql = @"server=127.0.0.1;uid=root;pwd=19953107;database=usersdb";
            var connectionMsSql = @"Server=(localdb)\mssqllocaldb;Database=usersdb;Trusted_Connection=True;";

            var tableData = new DataSet();  //Хранит данные, полученные из таблицы БД.

            using (var connection = new MySqlConnection(connectionMySql))
            {
                connection.Open(); //Подключаемся к БД.

                //Получаем данные из БД.
                using (var tableDataAdapter = new MySqlDataAdapter(
                           "select * from users order by record_id desc limit 500",
                           connection))
                {
                    tableDataAdapter.Fill(tableData);
                }
            }


            using (var con = new SqlConnection(connectionMsSql))
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
                using (var addTable = new SqlCommand(addTableCommand, con))
                {
                    try
                    {
                        addTable.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Ошибка! {0}",ex.Message);
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                }

                //Отдельно парсим значения из таблицы и заполняем ими созданную ранее таблицу.
                foreach (DataTable dt in tableData.Tables)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string addData = null;
                        addData += addDataTableCommand + "values (";

                        for (var i = 0; i < dt.Columns.Count; i++)
                        {
                            if (dt.Columns[i].ColumnName != "record_id")
                            {
                                addData += "'" + Convert.ToString(row[i]) + "',";
                            }
                        }

                        addData = addData.Substring(0, addData.Length - 1) + ")";

                        using (var addDataCommand = new SqlCommand(addData, con))
                        {
                            try
                            {
                                addDataCommand.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Ошибка! {0}", ex.Message);
                                Console.ReadLine();
                                Environment.Exit(0);
                            }
                        }
                    }
                }
            }

            Console.WriteLine("Готово!");
            Console.ReadLine();
        }     
    }

}