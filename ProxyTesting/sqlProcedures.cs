using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Configuration;


namespace ProxyWorld
{
    class sqlProcedures
    {
        private static SqlConnection sqlmasterConnection()
        {
            //eqgaitwriter
            SqlConnection sqlCon = new SqlConnection(ConfigurationManager.ConnectionStrings["masterCon"].ToString());
            sqlCon.Open();
            return sqlCon;



        }

        public class GrabProxyList
        {

            public static DataTable GrabAllProxies()
            {
                DataTable _dtTable = new DataTable();
                using (SqlConnection sqlCon = sqlmasterConnection())
                {
                    SqlCommand _sqlcmd = new SqlCommand();
                    _sqlcmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter("sp_GrabAllActiveProxies", sqlCon);
                    da.Fill(_dtTable);


                }

                return _dtTable;
            }



        }

        public class InsertProxyResults
        {
            public static void InsertResult(string IP, int Port, DateTime TestDate, string TestBy, TimeSpan PingTimeResults, int PingTestGrade)
            {
                SqlCommand _sqlCommand = new SqlCommand("sp_insertProxyResult", sqlmasterConnection());
                _sqlCommand.CommandType = CommandType.StoredProcedure;
                string[] variables = { "IP", "Port", "TestDate", "TestBy", "PingTimeResults", "PingTestGrade" };
                for (int i = 0; i < variables.Length; i++)
                {
                    switch (variables[i])
                    {
                        case "IP":
                            _sqlCommand.Parameters.Add("IP", IP);
                            break;
                        case "Port":
                            _sqlCommand.Parameters.AddWithValue("Port", Port);
                            break;
                        case "TestDate":
                            _sqlCommand.Parameters.AddWithValue("TestDate", TestDate);
                            break;
                        case "TestBy":
                            _sqlCommand.Parameters.AddWithValue("TestBy", TestBy);
                            break;
                        case "PingTimeResults":
                            _sqlCommand.Parameters.AddWithValue("PingTimeResults", PingTimeResults);
                            break;
                        case "PingTestGrade":
                            _sqlCommand.Parameters.AddWithValue("PingTestGrade", PingTestGrade);
                            break;

                    }
                }
                _sqlCommand.ExecuteNonQuery();

            }


        }

        public class SQLCommands
        {
            public class StoredProcedure
            {
                private static SqlCommand _sqlCmd(CommandType cmdType, string CommandText)
                {
                    SqlCommand sqlcmd = new SqlCommand(CommandText, sqlmasterConnection());
                    sqlcmd.CommandType = cmdType;
                    return sqlcmd;

                }
                public static void sp_DeleteFailedProxyTests()
                {

                    SqlCommand myCmd = _sqlCmd(CommandType.StoredProcedure, "sp_DeleteFailedProxyTests");
                    myCmd.ExecuteNonQuery();
                }

            }


        }
    }
}