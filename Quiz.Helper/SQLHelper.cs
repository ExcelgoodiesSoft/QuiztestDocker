using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Helper
{
    public class SQLHelper
    {
        private static readonly Random _rng = new Random();
        private const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        public static SqlConnection ExecuteReaderConnection()
        {
            try
            {
                SqlConnection sqlCon = new SqlConnection(GetDBConnection());

                return sqlCon;
            }
            catch (SqlException exMe)
            {
                throw exMe;
            }
        }
        public static string GetDBConnection()
        {
            try
            {
                return ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            }
            catch (Exception exMe)
            {
                return exMe.ToString();
            }
        }
        public static SqlDataReader ExecuteReader(string commandText, SqlConnection sqlCon)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(commandText, sqlCon);
                if (sqlCon.State != ConnectionState.Open)
                    sqlCon.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                return dr;
            }
            catch (SqlException exMe)
            {
                if (sqlCon.State == ConnectionState.Open)
                    sqlCon.Close();                
                throw exMe;
            }
        }
        public static object ExecuteScalar(string commandText, params SqlParameter[] cmdParams)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(GetDBConnection()))
                {
                    SqlCommand cmd = new SqlCommand(commandText, sqlCon);
                    cmd.CommandTimeout = 600;
                    sqlCon.Open();
                    cmd.Prepare();
                    if (cmdParams != null)
                        foreach (SqlParameter p in cmdParams)
                        {
                            if (p != null)
                                cmd.Parameters.Add(p);
                        }
                    object returnval = cmd.ExecuteScalar();
                    sqlCon.Close();
                    return returnval;
                }
            }
            catch (SqlException exMe)
            {

                return exMe.ToString();
            }
        }
        public static object ExecuteScalar(string commandText)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(GetDBConnection()))
                {
                    SqlCommand cmd = new SqlCommand(commandText, sqlCon);
                    sqlCon.Open();
                    //object returnval = 0;
                    object returnval = cmd.ExecuteScalar();
                    sqlCon.Close();
                    return returnval;
                }
            }
            catch (SqlException exMe)
            {
                throw exMe;
            }
        }
        public static void ExecuteReaderConnectionClose(SqlConnection sqlCon)
        {
            try
            {
                sqlCon.Close();
            }
            catch (SqlException exMe)
            {
                throw exMe;
            }
        }
        public static int ConvertInt(string value)
        {
            int result = 0;
            try
            {
                result = Convert.ToInt32(value.Trim());
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static int ConvertInt(object value)
        {
            int result = 0;
            try
            {
                result = Convert.ToInt32(value);
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            return result;
        }
        public static DateTime ConvertDateTime(string value)
        {
            DateTime result=Convert.ToDateTime("01/01/1900");
            try
            {
                result = Convert.ToDateTime(value);
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static string SqlDateDisplay(string MySqlDate)
        {
            if (MySqlDate != null && MySqlDate != "")
            {
                DateTime TempDate = Convert.ToDateTime(MySqlDate);
                if (TempDate.ToString("dd-MMM-yyyy") == "01-Jan-1900")
                {
                    return " ";
                }
                else
                {
                    return TempDate.ToString("dd-MMM-yyyy");
                }
            }
            else
            {
                return MySqlDate;
            }
        }
        public static string RandomString(int size)
        {
            char[] buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return new string(buffer);
        }
        public static string DoCheckNull(string val)
        {
            string strRetVal = "";
            if (val != null && val != "") strRetVal = val;
            return strRetVal;
        }
        public static string EscapeString(string value)
        {
            try
            {
                if (value != "") value = value.Replace("\"", "\\\"");
                return value;
            }
            catch (Exception exMe)
            {
                return exMe.ToString();
            }
        }
    }
}
