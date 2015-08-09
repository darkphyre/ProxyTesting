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
                using (SqlConnection sqlCon = sqlmasterConnection())
                {
                    SqlCommand _sqlCommand = new SqlCommand("sp_insertProxyResult", sqlCon);
                    _sqlCommand.CommandType = CommandType.StoredProcedure;
                    string[] variables = { "IP", "Port", "TestDate", "TestBy", "PingTimeResults", "PingTestGrade" };
                    for (int i = 0; i < variables.Length; i++)
                    {
                        switch (variables[i])
                        {
                            case "IP":
                                _sqlCommand.Parameters.AddWithValue("IP", IP);
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


        }

        public class SQLCommands
        {
            public class StoredProcedure
            {
                private static SqlCommand _sqlCmd(CommandType cmdType, string CommandText, SqlConnection sqlCon)
                {
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.CommandText = CommandText;
                    if (sqlCon != null)
                        sqlcmd.Connection = sqlCon;
                    sqlcmd.CommandType = cmdType;
                    return sqlcmd;

                }
                public static void sp_DeleteFailedProxyTests()
                {
                    using (SqlConnection sqlCon = sqlmasterConnection())
                    {
                        SqlCommand myCmd = _sqlCmd(CommandType.StoredProcedure, "sp_DeleteFailedProxyTests", sqlCon);
                        myCmd.ExecuteNonQuery();
                    }
                }

                public static void sp_InsetNewProxyFromFile(string IP, int port)
                {
                    using (SqlConnection sqlCon = sqlmasterConnection())
                    {
                        SqlCommand sqlCmd = _sqlCmd(CommandType.StoredProcedure, "sp_InsetNewProxyFromFile", sqlCon);
                        string[] sqlparams = { "IP", "Port" };
                        for (int i = 0; i < sqlparams.Length; i++)
                        {
                            switch (sqlparams[i])
                            {
                                case "IP":
                                    sqlCmd.Parameters.AddWithValue(sqlparams[i], IP);
                                    break;
                                case "Port":
                                    sqlCmd.Parameters.AddWithValue(sqlparams[i], port);
                                    break;


                            }
                        }
                        sqlCmd.ExecuteNonQuery();
                    }


                }

            }


        }
    }
}