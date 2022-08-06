using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace Data_Access_Layer
{
    public class DBEntity
    {
        //this field gets initialized at Praogram.cs
        public static string conStr;

        public static DataTable GetSqlData(string query, out string ErrorMsg)
        {
            ErrorMsg = string.Empty;
            try
            {
                DataTable dtResult = new DataTable();

                //string conStr = ConfigurationManager.ConnectionStrings["O3EntitiesDB"].ConnectionString;

                using (var connection = new SqlConnection(conStr))
                {
                    var command = new SqlCommand(query, connection);
                    command.CommandTimeout = 600;
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dtResult);
                    connection.Close();
                }
                return dtResult;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message.PadLeft(20);
                return null;
            }


        }


        public static DataSet GetSqlDataset(string query, out string ErrorMsg)
        {
            ErrorMsg = string.Empty;
            try
            {
                DataSet dsResult = new DataSet();
                //string conStr = ConfigurationManager.ConnectionStrings["O3EntitiesDB"].ConnectionString;

                using (var connection = new SqlConnection(conStr))
                {
                    var command = new SqlCommand(query, connection);
                    command.CommandTimeout = 600;
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dsResult);
                    connection.Close();
                }
                return dsResult;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message.PadLeft(20);
                return null;
            }


        }
        public static DataTable ExecuteTypeSp(string spName, List<OKASqlParam> sqlparam, out string ErrorMsg)
        {
            ErrorMsg = string.Empty;
            try
            {
                //string conStr = ConfigurationManager.ConnectionStrings["O3EntitiesDB"].ConnectionString;

                DataTable dsresult = new DataTable();

                using (var conn = new SqlConnection(conStr))
                using (var command = new SqlCommand(spName, conn)
                {
                    CommandType = CommandType.StoredProcedure

                })
                {
                    conn.Open();

                    for (int i = 0; i < sqlparam.Count(); i++)
                    {
                        OKASqlParam sqlpar = sqlparam[i];
                        SqlParameter parameter = new SqlParameter();
                        parameter.ParameterName = sqlpar.paramName;
                        parameter.Value = sqlpar.ParamValue;
                        command.Parameters.Add(parameter);
                    }
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dsresult);
                }

                return dsresult;


            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message.PadLeft(20);
                return null;

            }
        }


        public static DataTable ToDataTable<T>(IList<T> data, out string ErrorMsg)
        {
            ErrorMsg = string.Empty;
            try
            {
                PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
                DataTable table = new DataTable();
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, prop.PropertyType);
                }
                if (data != null)
                {

                    object[] values = new object[props.Count];
                    foreach (T item in data)
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }
                        table.Rows.Add(values);
                    }
                }
                return table;

            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message.PadLeft(20);
                return null;

            }
        }



        /// <summary>
        /// Gets single data table from DB
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static DataTable GetDataTableBySP(SqlCommand com, out string ErrorMsg)
        {
            ErrorMsg = string.Empty;
            DataTable dt = new DataTable();

            try
            {
                for (int i = 0; i < com.Parameters.Count; i++)
                {
                    if (com.Parameters[i].Value == null)
                    {
                        com.Parameters[i].Value = DBNull.Value;
                    }
                }

                using (var conn = new SqlConnection(conStr))
                {
                    com.Connection = conn;
                    com.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    da.Fill(dt);
                }
            }
            catch (Exception exx)
            {
                try
                {
                    StringBuilder callDefinition = new StringBuilder();
                    callDefinition.Append(string.Format("exec {0} ", com.CommandText));

                    for (int i = 0; i < com.Parameters.Count; i++)
                    {
                        callDefinition.Append(string.Format("{0}='{1}',", com.Parameters[i].ParameterName, com.Parameters[i].Value));
                    }

                    string finalCallDefinition = callDefinition.ToString().TrimEnd(',');

                    SqlCommand comL = new SqlCommand("InsertSpException", new SqlConnection(conStr));
                    comL.CommandType = CommandType.StoredProcedure;
                    comL.Parameters.AddWithValue("@SpCallDef", finalCallDefinition);
                    comL.Parameters.AddWithValue("@ExceptionDetails", exx.ToString());

                    if (comL.Connection.State != ConnectionState.Open)
                        comL.Connection.Open();
                    comL.ExecuteNonQuery();
                    comL.Connection.Close();


                }
                catch (Exception ex)
                {
                    ErrorMsg = ex.Message.PadLeft(20);
                    return null;

                }

                //throw exx;
            }



            return dt;
        }

        /// <summary>
        /// Gets multiple data tables from DB
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
        public static DataSet GetDataSetBySP(SqlCommand com)
        {
            DataSet ds = new DataSet();

            try
            {
                for (int i = 0; i < com.Parameters.Count; i++)
                {
                    if (com.Parameters[i].Value == null)
                    {
                        com.Parameters[i].Value = DBNull.Value;
                    }
                }

                using (var conn = new SqlConnection(conStr))
                {
                    com.Connection = conn;
                    com.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    da.Fill(ds);
                }
            }
            catch (Exception exx)
            {
                // Logged failed SPs here
                try
                {
                    StringBuilder callDefinition = new StringBuilder();
                    callDefinition.Append(string.Format("exec {0} ", com.CommandText));

                    for (int i = 0; i < com.Parameters.Count; i++)
                    {
                        callDefinition.Append(string.Format("{0}='{1}',", com.Parameters[i].ParameterName, com.Parameters[i].Value));
                    }

                    string finalCallDefinition = callDefinition.ToString().TrimEnd(',');

                    SqlCommand comL = new SqlCommand("InsertSpException", new SqlConnection(conStr));
                    comL.CommandType = CommandType.StoredProcedure;
                    comL.Parameters.AddWithValue("@SpCallDef", finalCallDefinition);
                    comL.Parameters.AddWithValue("@ExceptionDetails", exx.ToString());


                    if (comL.Connection.State != ConnectionState.Open)
                        comL.Connection.Open();
                    comL.ExecuteNonQuery();
                    comL.Connection.Close();


                }
                catch (Exception)
                {
                }

                throw exx;
            }



            return ds;
        }




        //Code added to request log on the DB.
        public static DataTable ExecuteTypeSp_APILog(string functionname, string request)
        {
            try
            {
                DataTable dsresult = new DataTable();
                using (var conn = new SqlConnection(conStr))
                using (var command = new SqlCommand("Insert_tblAPILog", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    conn.Open();
                    SqlParameter parameter1 = new SqlParameter();
                    parameter1.ParameterName = "@funcname";
                    parameter1.Value = functionname;
                    command.Parameters.Add(parameter1);
                    SqlParameter parameter2 = new SqlParameter();
                    parameter2.ParameterName = "@request";
                    parameter2.Value = request;
                    command.Parameters.Add(parameter2);
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dsresult);
                }
                return dsresult;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //Code updated to API response log on the DB.
        public static DataTable ExecuteTypeSp_APIRespLog(string id, string respone)
        {
            try
            {
                DataTable dsresult = new DataTable();
                using (var conn = new SqlConnection(conStr))
                using (var command = new SqlCommand("splocal_Update_tblAPILog", conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    conn.Open();
                    SqlParameter parameter1 = new SqlParameter();
                    parameter1.ParameterName = "@messageid";
                    parameter1.Value = id;
                    command.Parameters.Add(parameter1);
                    SqlParameter parameter2 = new SqlParameter();
                    parameter2.ParameterName = "@response";
                    parameter2.Value = respone;
                    command.Parameters.Add(parameter2);
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dsresult);
                }
                return dsresult;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable fillComboBox(string query)
        {

            try
            {
                DataTable dtResult = new DataTable();

                using (var connection = new SqlConnection(conStr))
                {
                    var command = new SqlCommand(query, connection);
                    command.CommandTimeout = 600;
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dtResult);
                    connection.Close();
                }
                return dtResult;
            }
            catch (Exception ex)
            {

                return null;
            }


        }

        public static object getField(string sql)
        {
            object result = string.Empty;
            //DataSet ds = new DataSet();
            DataTable dt;
            dt = fillComboBox(sql);

            if (dt.Rows.Count != 0)
                result = dt.Rows[0].ItemArray[0];

            if (result == string.Empty)
            { }
            return result;
        }

        public static Boolean IsDeleted(string tblName, string FieldName, int ID)
        {
            string msg = string.Empty;
            ExecuteNonQuery("Update " + tblName + " set IsDeleted = 1 where " + FieldName + "=" + ID + "", out msg);
            if (msg != String.Empty)
            {
                return false;
            }
            else
            {
                return true;
            }
        }



        public static int ExecuteNonQuery(string sSql, out string ErrorMsg)
        {
            ErrorMsg = string.Empty;
            try
            {
                SqlCommand comL = new SqlCommand(sSql, new SqlConnection(conStr));

                if (comL.Connection.State != ConnectionState.Open)
                    comL.Connection.Open();
                comL.ExecuteNonQuery();
                comL.Connection.Close();
                return 0;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message.PadLeft(20);
                return 0;
            }



            return 0;

        }
        public static string getMax(string fieldName, string tableName, string condition)
        {
            string result = string.Empty;
            string strSql = "Select isnull(max(" + fieldName + "),0)+1 from " + tableName + " " + condition;
            DataSet ds = new DataSet();
            try
            {

                using (var connection = new SqlConnection(conStr))
                {
                    var command = new SqlCommand(strSql, connection);
                    command.CommandTimeout = 600;
                    connection.Open();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(ds);
                    connection.Close();
                }

                result = ds.Tables[0].Rows[0].ItemArray[0].ToString();
            }
            catch (Exception E)
            {

                result = "";
            }
            return result;
        }

    }



    public class OKASqlParam
    {
        public string paramName;
        public object ParamValue;
    }
}
