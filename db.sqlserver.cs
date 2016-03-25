using System;
using System.Web;
using System.Xml;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text;

namespace Ia.Cl.Model.Db
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// SQL Server support class.
    /// </summary>
    /// <remarks> 
    /// Copyright © 2001-2015 Jasem Y. Al-Shamlan (info@ia.com.kw), Internet Applications - Kuwait. All Rights Reserved.
    ///
    /// This library is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
    /// the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
    ///
    /// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
    /// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
    /// 
    /// You should have received a copy of the GNU General Public License along with this library. If not, see http://www.gnu.org/licenses.
    /// 
    /// Copyright notice: This notice may not be removed or altered from any source distribution.
    /// </remarks> 
    public class SqlServer
    {
        private string connectionString;
        private SqlConnection sqlConnection;
        private StringBuilder logStringBuilder;
        private ArrayList al, from_al, insert_al, delete_al;

        /// <summary/>
        public enum F
        {
            /// <summary/>
            Bit,

            /// <summary/>
            In,

            /// <summary/>
            St,

            /// <summary/>
            Sdt,

            /// <summary/>
            Cr,

            /// <summary/>
            Up,

            /// <summary/>
            Ni,

            /// <summary/>
            Srs
        };

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public SqlServer()
        {
            Initialize();

            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            Log("SqlServer(): Connection string: " + connectionString);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public SqlServer(string _connectionString)
        {
            Initialize();

            connectionString = _connectionString;

            Log("SqlServer(string): Connection string: " + connectionString);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void Initialize()
        {
            sqlConnection = new SqlConnection();

            logStringBuilder = new StringBuilder();

            Log("Initialize(): SqlServer object created");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void Connect()
        {
            if (sqlConnection.State == ConnectionState.Open && connectionString.Contains(sqlConnection.ConnectionString)) // important like this
            {
                Log("Connect(): Connect already open with same connection string");
            }
            else
            {
                sqlConnection.Close();
                sqlConnection.Dispose();

                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                // oracleConnection.GetSessionInfo();

                Log("Connect(): Connect opened");
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void Close()
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();

                Log("Close(): Connection closed");
            }
            else
            {
                Log("Close(): Connection already closed");
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Sql(string sql)
        {
            /*
            // 
            bool b;

            try
            {
                Connect();

                OracleCommand oco = new OracleCommand();

                oco.Connection = oracleConnection;
                oco.CommandText = sql;
                oco.CommandType = Server.HtmlEncode(CommandType.Text);

                oco.ExecuteNonQuery();

                b = true;
            }
            catch (OracleException ex)
            {
                b = false;
                Close();

                Log("Sql(): OracleException: " + ex.Source);
            }
            catch (Exception ex)
            {
                b = false;
                Close();

                Log("Sql(): Exception: " + ex.Source);
            }
            finally
            {
            }

            return b;
             */

            // execute an SQL command
            bool b = true;
            SqlCommand sco;

            sqlConnection = new SqlConnection(connectionString);
            sco = new SqlCommand();

            sco.CommandType = CommandType.Text; // default
            sco.CommandText = sql;
            sco.Connection = sqlConnection;
            sqlConnection.Open();
            sco.ExecuteNonQuery();
            sqlConnection.Close();

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public DataTable Select(string sql)
        {
            /*
            // return a DataTable of result rows
            //bool b = true;
            //string s;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            OracleDataAdapter da;
            //OracleDataReader dr;

            // sc = new OracleConnection(ConfigurationManager.ConnectionStrings["connection_string_oracle"].ConnectionString);
            //OracleDataReader r = null;

            try
            {
                Connect();

                OracleCommand oco = new OracleCommand();

                oco.Connection = oracleConnection;
                oco.CommandText = sql;
                oco.CommandType = Server.HtmlEncode(CommandType.Text);

                /*
                dr = oco.ExecuteReader();

                if (dr.Read())
                {
                    s = dr.GetString(0);
                }
                else b = false;
                 * /

                da = new OracleDataAdapter(oco);
                //da.SelectCommand = oco;

                //da.ReturnProviderSpecificTypes = false;
                //da.MissingMappingAction = MissingMappingAction.Passthrough;
                //da.MissingSchemaAction = MissingSchemaAction.AddWithKey;

                da.Fill(ds);

                dt = ds.Tables[0];
            }
            catch (OracleException ex)
            {
                //b = false;
                Close();

                Log("Select(): OracleException: " + ex.Source);
            }
            catch (Exception ex)
            {
                //b = false;
                Close();

                Log("Select(): Exception: " + ex.Source);
            }
            finally
            {
            }

            return dt;
             */

            // return a DataTable of result rows

            SqlConnection sc = null;
            SqlCommand sco;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();

            try
            {
                sc = new SqlConnection(connectionString);
                sco = new SqlCommand(sql, sc);

                sc.Open();

                da.SelectCommand = sco;

                da.Fill(ds);

                sc.Close();

                dt = ds.Tables[0];
            }
            catch (Exception e)
            {
                dt = null;
            }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute a stored procedure
        /// <param name="sp_name">Name of stored procedure</param>
        /// <param name="name">List of database field names</param>
        /// <param name="value">List of database field names' values</param>
        /// <param name="r"></param>
        /// </summary>

        public bool StoredProcedure(string sp_name, object[] name, object[] value, out string r)
        {
            // execute a stored procedure

            bool b;
            object o;
            int i;
            SqlConnection sc = null;
            SqlCommand sco;

            r = "";

            if (name.Length == value.Length && name.Length != 0)
            {
                try
                {
                    sc = new SqlConnection(connectionString);
                    sco = new SqlCommand(sp_name, sc);

                    sco.CommandType = CommandType.StoredProcedure;

                    for (i = 0; i < value.Length; i++)
                    {
                        o = value[i];

                        if (o == null)
                        {
                            sco.Parameters.Add(new SqlParameter(name[i].ToString(), ""));
                        }
                        else if (o.GetType() == typeof(string)) sco.Parameters.Add(new SqlParameter(name[i].ToString(), value[i].ToString()));
                        else if (o.GetType() == typeof(Guid)) sco.Parameters.Add(new SqlParameter(name[i].ToString(), value[i].ToString()));
                        else if (o.GetType() == typeof(bool)) sco.Parameters.Add(new SqlParameter(name[i].ToString(), value[i]));
                        else if (o.GetType() == typeof(int)) sco.Parameters.Add(new SqlParameter(name[i].ToString(), (int)value[i]));
                        else if (o.GetType() == typeof(long)) sco.Parameters.Add(new SqlParameter(name[i].ToString(), (long)value[i]));
                        else if (o.GetType() == typeof(DateTime)) sco.Parameters.Add(new SqlParameter(name[i].ToString(), value[i]));
                    }

                    sc.Open();

                    sco.ExecuteNonQuery();

                    sc.Close();

                    b = true;
                }
                catch (Exception ex)
                {
                    b = false;
                    r = ex.Message;
                }
                finally
                {
                    if (sc != null) sc.Close();
                }
            }
            else
            {
                b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return a DataTable of properties of tables that start with "ia_" 
        /// </summary>
        public DataTable Schema()
        {
            // 
            DataTable dt;

            dt = Select("SELECT TABLE_NAME + '_' + COLUMN_NAME AS id, TABLE_NAME, COLUMN_NAME, COLUMN_DEFAULT, IS_NULLABLE, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE (TABLE_NAME LIKE 'ia_%')");

            DataColumn[] keys = new DataColumn[1];
            keys[0] = dt.Columns["id"];
            dt.PrimaryKey = keys;

            return dt;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Verify(System.Web.UI.Page p, object[] name, object[] value, out string result)
        {
            // 

            bool b;
            object n,v;
            int i;
            string data_type;
            DataRow dr;
            DataTable dt;

            result = "";

            if (name.Length == value.Length && name.Length != 0)
            {
                dt = global::Ia.Cl.Model.This.schema_dt;

                try
                {
                    for (i = 0; i < name.Length; i++)
                    {
                        n = name[i];
                        v = value[i];

                        dr = dt.Rows.Find("ia_" + n.ToString());

                        if (dr != null)
                        {
                            data_type = dr["DATA_TYPE"].ToString();

                            if (data_type == "nvarchar")
                            {
                                if (v.ToString().Length > int.Parse(dr["CHARACTER_MAXIMUM_LENGTH"].ToString()))
                                {
                                    result += "'" + n.ToString() + "' is too long. ";
                                }
                            }
                            else if (v.GetType() == typeof(Guid) && data_type == "nvarchar")
                            {
                            }
                            else if (v.GetType() == typeof(bool) && data_type == "nvarchar")
                            {
                            }
                            else if (v.GetType() == typeof(int) && data_type == "nvarchar")
                            {
                            }
                            else if (v.GetType() == typeof(long) && data_type == "nvarchar")
                            {
                            }
                            else if (v.GetType() == typeof(DateTime) && data_type == "nvarchar")
                            {
                            }
                        }
                        else
                        {
                            result += "'" + n.ToString() + "' not found in table fields. ";
                        }
                    }

                    if (result.Length > 0) b = false;
                    else b = true;
                }
                catch (Exception ex)
                {
                    b = false;
                }
                finally
                {
                }
            }
            else
            {
                b = false;
            }

            return b;
        }
        */

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string SelectFromMiscellaneousTable(string name)
        {
            string s;

            s = Scalar(@"SELECT value FROM ia_misc WHERE name = '" + name + "'");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int SelectFromMiscellaneousTable(string name, out ArrayList al)
        {
            int op;
            string s;
            string[] st;

            op = 0;

            al = new ArrayList(1);

            s = Scalar(@"SELECT value FROM ia_misc WHERE name = '" + name + "'");

            if (s != null)
            {
                st = s.Split('|');
                al = new ArrayList(st.Length);
                al.Clear();

                foreach (string t in st) al.Add(t);

                if (al.Count > 0) op = 1;
                else op = 0;
            }
            else op = -1;

            return op;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int SelectFromMiscellaneousTable(string name, out DataTable dt)
        {
            int op;
            string s;
            DataRow dr;
            XmlNode xn;
            XmlDocument xd;

            op = 0;

            dt = new DataTable(name);

            s = Scalar(@"SELECT value_xml FROM ia_misc WHERE name = '" + name + "'");

            if (s != null && s != "")
            {
                xd = new XmlDocument();
                xd.LoadXml(s);

                dt = new DataTable(xd.DocumentElement.Name);

                // collect table columns
                xn = xd.DocumentElement.FirstChild;
                foreach (XmlNode n in xn.ChildNodes)
                {
                    if (n.Name == "user_id") dt.Columns.Add("user_id", typeof(System.Guid));
                    else dt.Columns.Add(n.Name);
                }

                // collect row values
                foreach (XmlNode n in xd.DocumentElement.ChildNodes)
                {
                    dr = dt.NewRow();

                    foreach (XmlNode o in n.ChildNodes)
                    {
                        if (o.Name == "user_id")
                        {
                            if(o.InnerText == "") dr[o.Name] = DBNull.Value;
                            else dr[o.Name] = o.InnerText;
                        }
                        else dr[o.Name] = o.InnerText;
                    }

                    dt.Rows.Add(dr);
                }

                dt.AcceptChanges();

                if (dt.Rows.Count > 0) op = 1;
                else op = 0;
            }
            else op = -1;

            return op;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void UpdateFromMiscellaneousTable(string name, string value)
        {
            Sql(@"UPDATE ia_misc SET value = '" + value + "' WHERE name = '" + name + "'");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void UpdateFromMiscellaneousTable(string name, ArrayList al)
        {
            StringBuilder sb;

            if (al.Count > 0)
            {
                sb = new StringBuilder(al.Count + 1);
                sb.Length = 0;

                foreach (string t in al) sb.Append(t + "|");
                sb = sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                sb = new StringBuilder(1);
                sb.Length = 0;
            }

            Sql(@"UPDATE ia_misc SET value = '" + sb.ToString() + "' WHERE name = '" + name + "'");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void UpdateFromMiscellaneousTable(string name, SortedList sl)
        {
            StringBuilder sb;

            if (sl.Count > 0)
            {
                sb = new StringBuilder(sl.Count + 1);
                sb.Length = 0;

                foreach (string t in sl.Keys) sb.Append(t + "|");
                sb = sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                sb = new StringBuilder(1);
                sb.Length = 0;
            }

            Sql(@"UPDATE ia_misc SET value = '" + sb.ToString() + "' WHERE name = '" + name + "'");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void UpdateFromMiscellaneousTable(string name, DataTable dt)
        {
            StringBuilder sb;

            if (dt.Rows.Count > 0)
            {
                sb = new StringBuilder(dt.Rows.Count + 1);
                sb.Length = 0;

                sb = sb.Append("<" + name + ">");

                foreach (DataRow r in dt.Rows)
                {
                    sb = sb.Append("<row>");

                    foreach (DataColumn c in dt.Columns)
                    {
                        sb = sb.Append("<" + c.ColumnName + ">");

                        sb.Append(r[c.ColumnName].ToString());

                        sb = sb.Append("</" + c.ColumnName + ">");
                    }

                    sb = sb.Append("</row>");
                }

                sb = sb.Append("</" + name + ">");
            }
            else
            {
                sb = new StringBuilder(1);
                sb.Length = 0;
            }

            Sql(@"UPDATE ia_misc SET value_xml = '" + sb.ToString() + "' WHERE name = '" + name + "'");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void DeleteFromMiscellaneousTable(string name)
        {
            Sql(@"DELETE FROM ia_misc WHERE name = '" + name + "'");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void InsertFromMiscellaneousTable(string name)
        {
            Sql(@"INSERT INTO ia_misc (name) VALUES ('" + name + "')");
        }
         */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Scalar(string sql)
        {
            // return a scaler
            string s;

            SqlConnection sc = null;
            SqlCommand sco;

            sc = new SqlConnection(connectionString);
            sco = new SqlCommand(sql, sc);

            sc.Open();

            try { s = sco.ExecuteScalar().ToString(); }
            catch { s = null; }

            sc.Close();

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Scalar_(string sql)
        {
            // return a scaler
            string s;

            SqlConnection sc = null;
            SqlCommand sco;

            sc = new SqlConnection(connectionString);
            sco = new SqlCommand(sql, sc);

            sc.Open();

            try { s = sco.ExecuteScalar().ToString(); }
            catch { s = null; }

            sc.Close();

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int ScalarInteger(string sql)
        {
            // return a DataTable of result rows
            int n;

            SqlConnection sc = null;
            SqlCommand sco;

            sc = new SqlConnection(connectionString);
            sco = new SqlCommand(sql, sc);

            sc.Open();

            try
            {
                n = (System.Int32)sco.ExecuteScalar();
            }
            catch (Exception)
            {
                n = 0;
            }

            sc.Close();

            return n;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int ScalarSmallInteger(string sql)
        {
            // return a DataTable of result rows
            int n;

            SqlConnection sc = null;
            SqlCommand sco;

            sc = new SqlConnection(connectionString);
            sco = new SqlCommand(sql, sc);

            sc.Open();

            try
            {
                n = (System.Int16)sco.ExecuteScalar();
            }
            catch (Exception)
            {
                n = 0;
            }

            sc.Close();

            return n;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string SmallDateTime(DateTime dt)
        {
            // return an SQL Server friendly string of a smalldatetime value
            string s;

            //s = "'" + dt.ToString("yyyy-MM-ddTHH:mm:ss") + "'";
            s = dt.ToString("yyyy-MM-ddTHH:mm:ss");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string RangeSql(ArrayList al, string term)
        {
            // this will take a special ArrayList of ranges and will construct an SQL format that correspond to the array.
            // input will look like al = [1,2,4,6,7,15,20-22,34-36,38], output will look like "(sr.dn>16 AND sr.dn<19) OR sr.dn=22"

            string sql, u, v;
            string[] sp;
            StringBuilder sb;

            sql = "";

            if (al.Count > 0)
            {
                sb = new StringBuilder((term.Length + 15) * al.Count);
                sb.Length = 0;

                foreach (string s in al)
                {
                    sp = s.Split('-');

                    if (sp.Length == 1)
                    {
                        // single value
                        sb.Append(term + "=" + s + " OR ");
                    }
                    else if (sp.Length == 2)
                    {
                        // range
                        u = sp[0]; v = sp[1];
                        sb.Append("(" + term + ">=" + u + " AND " + term + "<=" + v + ") OR ");
                    }
                }

                sql = sb.ToString();
                sql = sql.Remove(sql.Length - 4, 4);
            }


            return sql;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool ImportXml(string tableName, string file)
        {
            bool b;
            int i;
            string c, path;
            DataSet ds;
            SqlDataAdapter sda;
            SqlConnection sc;
            SqlCommandBuilder scb;
            DataRow dr;

            ds = new DataSet("ia_ngn");

            path = global::Ia.Cl.Model.Default.AbsolutePath();

            file = path + file;

            ds.ReadXml(file);

            sc = new SqlConnection(connectionString);
            sc.Open();

            c = @"SELECT * FROM [" + tableName + @"]";
            sda = new SqlDataAdapter(c, sc);
            scb = new SqlCommandBuilder(sda);

            try
            {
                sda.Fill(ds, tableName);

                foreach (DataRow r in ds.Tables[tableName].Rows)
                {
                    dr = ds.Tables[tableName].NewRow();

                    for (i = 0; i < r.ItemArray.Length; i++) dr[i] = r[i];

                    ds.Tables[0].Rows.Add(dr);
                }

                scb.GetUpdateCommand();
                sda.Update(ds, tableName);

                b = true;
            }
            catch (Exception) { b = false; }
            finally
            {
                sc.Close();
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool ExportXml(string tableName, string file)
        {
            // perform dump or backup of database table data into an XML document
            bool b;
            string c, path;
            DataSet ds;
            SqlConnection sc;
            SqlDataAdapter sda;

            c = @"SELECT * FROM [" + tableName + @"]";
            sc = new SqlConnection(connectionString);
            sc.Open();

            ds = new DataSet("ia_ngn");
            sda = new SqlDataAdapter(c, sc);

            try
            {
                sda.Fill(ds, tableName);

                path = global::Ia.Cl.Model.Default.AbsolutePath();

                file = path + file;
                ds.WriteXml(file, XmlWriteMode.WriteSchema);

                b = true;
            }
            catch (Exception) { b = false; }
            finally
            {
                sc.Close();
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////






        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////










        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /* below: later remove service_xn */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int Update(DataTable inDt, string tableName, string selectCommand, string primaryKey, string[] inField, string[] field, F[] fieldRule, bool synch, string deleteRule, out string result)
        {
            return Update(inDt, tableName, selectCommand, primaryKey, inField, field, fieldRule, synch, deleteRule, null, out result);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int Update(DataTable inDt, string tableName, string selectCommand, string primaryKey, string[] inField, string[] field, F[] fieldRule, bool synch, string deleteRule, XmlNode serviceXn, out string result)
        {
            bool identical;
            int op, c, count, count_in, count_delete;
            F rule;
            string command;

            // TEMP
            //string temp = "", temp_dt_str, temp_in_dt_str; // TEMP

            // TEMP
            //string temp = "";

            int i, j;
            Hashtable ht;

            al = new ArrayList(1000);
            from_al = new ArrayList(1000);
            insert_al = new ArrayList(1000);
            delete_al = new ArrayList(1000);

            ht = new Hashtable(1000);

            DateTime sdt, in_sdt;

            DataRow dr;
            DataTable dt;
            DataSet ds;
            SqlDataAdapter sda;
            SqlConnection sc;
            SqlCommandBuilder scb;

            op = 0;
            c = count = count_in = count_delete = 0;

            ds = new DataSet("ia_ngn");
            sc = new SqlConnection(connectionString);

            sc.Open();
            command = selectCommand;
            sda = new SqlDataAdapter();
            scb = new SqlCommandBuilder(sda);
            sda.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            sda.SelectCommand = new SqlCommand(command, sc);

            result = "";

            // TEMP
            //temp_in_dt_str = temp_dt_str = "";

            dt = null;

            //temp += "1|"; // TEMP

            try
            {
                sda.Fill(ds, tableName);

                //temp += "1a|"; // TEMP

                dt = ds.Tables[0];

                //temp += "1b|"; // TEMP

                if (inDt != null)
                {
                    count_in = inDt.Rows.Count;

                    // TEMP
                    /*
                    foreach (DataRow r in in_dt.Rows)
                    {
                        temp_in_dt_str += "\n";
                        foreach (DataColumn c2 in in_dt.Columns) temp_in_dt_str += ":" + r[c2].ToString();
                    }
                    */

                    if (dt != null)
                    {
                        count = dt.Rows.Count;

                        // TEMP
                        //foreach (DataRow r in dt.Rows)
                        //{
                        //   temp_dt_str += "\n";
                        //   foreach (DataColumn c2 in dt.Columns) temp_dt_str += ":" + r[c2].ToString();
                        //}

                        if (inDt.Rows.Count > 0)
                        {
                            //if (dt.Rows.Count > 0)
                            //{
                            if (synch)
                            {
                                // compair two lists to find records in in_dt that are not in dt
                                foreach (DataRow r in dt.Rows) al.Add(r[primaryKey].ToString());
                                foreach (DataRow r in inDt.Rows) from_al.Add(r[primaryKey].ToString());

                                al.Sort();
                                from_al.Sort();

                                i = j = 0;

                                // I will assume that from_al is longer than al
                                //if (from_al.Count > al.Count)
                                //{
                                ArrayList list1, list2;

                                list1 = from_al;
                                list2 = al;

                                while (i < list1.Count)
                                {
                                    if (j == list2.Count) break;
                                    IComparable obj1 = list1[i] as IComparable;
                                    IComparable obj2 = list2[j] as IComparable;

                                    int cmp = obj1.CompareTo(obj2);

                                    switch (Math.Sign(cmp))
                                    {
                                        case 0: ++i; ++j; break;
                                        case 1: delete_al.Add(list2[j].ToString()); ++j; break;
                                        case -1: insert_al.Add(list1[i].ToString()); ++i; break;
                                    }
                                }

                                while (i < list1.Count) // we reached the end of list 2 first.
                                {
                                    insert_al.Add(list1[i].ToString()); ++i;
                                }

                                while (j < list2.Count) // we reached the end of list 1 first.
                                {
                                    delete_al.Add(list2[j].ToString()); ++j;
                                }
                                //}

                                if (delete_al.Count > 0)
                                {
                                    for (i = 0; i < delete_al.Count && i < 100; i++)
                                    {
                                        // We will delete it, or its contents according to the deletion rules of the table_name

                                        dr = dt.Rows.Find(delete_al[i].ToString());

                                        if (deleteRule == "all")
                                        {
                                            dr.Delete();
                                        }
                                        else if (deleteRule == "keep primary key")
                                        {
                                            // this will delete everything but keep only the primary key

                                            for (int n = 0; n < inField.Length; n++)
                                            {
                                                if (field[n].ToString() != primaryKey)
                                                {
                                                    rule = fieldRule[n];

                                                    if (rule == F.Bit)
                                                    {
                                                        dr[field[n].ToString()] = DBNull.Value;
                                                    }
                                                    else if (rule == F.In)
                                                    {
                                                        dr[field[n].ToString()] = DBNull.Value;
                                                    }
                                                    else if (rule == F.St)
                                                    {
                                                        dr[field[n].ToString()] = DBNull.Value;
                                                    }
                                                    else if (rule == F.Sdt)
                                                    {
                                                        dr[field[n].ToString()] = DBNull.Value;
                                                    }
                                                    else if (rule == F.Up)
                                                    {
                                                        dr[field[n].ToString()] = SmallDateTime(DateTime.UtcNow.AddHours(3));
                                                    }
                                                    else if (rule == F.Srs)
                                                    {
                                                        dr[field[n].ToString()] = DBNull.Value;
                                                    }
                                                }
                                            }
                                        }

                                        count_delete++;
                                    }
                                }
                            }

                            //temp += "1h|"; // TEMP

                            foreach (DataRow in_dr in inDt.Rows)
                            {
                                // collect relevent values:

                                //if (in_dr[primary_key].ToString() == "995126013") op++;

                                //temp += "1i|"; // TEMP

                                dr = dt.Rows.Find(long.Parse(in_dr[primaryKey].ToString()));

                                //temp += "1j|"; // TEMP

                                if (dr != null)
                                {
                                    // check if rows are identical

                                    identical = true;

                                    //temp += "1k|"; // TEMP

                                    for (int n = 0; n < inField.Length; n++)
                                    {
                                        rule = fieldRule[n];

                                        if (rule == F.Bit)
                                        {
                                            //temp += "1l|"; // TEMP

                                            try
                                            {
                                                if (dr[field[n].ToString()].ToString() != in_dr[inField[n].ToString()].ToString())
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                            }
                                            catch (InvalidCastException)
                                            {
                                                identical = false;
                                                break;
                                            }

                                        }
                                        else if (rule == F.In)
                                        {
                                            //temp += "1m|"; // TEMP

                                            try
                                            {
                                                if (dr[field[n].ToString()].ToString() != in_dr[inField[n].ToString()].ToString())
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                            }
                                            catch (FormatException)
                                            {
                                                identical = false;
                                                break;
                                            }
                                        }
                                        else if (rule == F.St)
                                        {
                                            //temp += "1n|"; // TEMP

                                            if (dr[field[n].ToString()] == DBNull.Value && in_dr[inField[n].ToString()] == DBNull.Value) { }
                                            else if (dr[field[n].ToString()] == DBNull.Value && in_dr[inField[n].ToString()] != DBNull.Value) { identical = false; break; }
                                            else if (dr[field[n].ToString()] != DBNull.Value && in_dr[inField[n].ToString()] == DBNull.Value) { identical = false; break; }
                                            else if (dr[field[n].ToString()].ToString() != in_dr[inField[n].ToString()].ToString())
                                            {
                                                identical = false;
                                                break;
                                            }
                                        }
                                        else if (rule == F.Sdt)
                                        {
                                            //temp += "1o|"; // TEMP

                                            sdt = DateTime.Parse(dr[field[n].ToString()].ToString());
                                            in_sdt = DateTime.Parse(in_dr[inField[n].ToString()].ToString());

                                            // if in_sdt lays within 1 minute of sdt they are identical

                                            if (in_sdt > sdt.AddMinutes(1) || in_sdt < sdt.AddMinutes(-1))
                                            {
                                                identical = false;
                                                break;
                                            }
                                        }
                                        else if (rule == F.Srs)
                                        {
                                            //temp += "1q|"; // TEMP

                                            try
                                            {
                                                //temp += "1q[" + field[n].ToString() + ":" + in_field[n].ToString() + "]"; // TEMP
                                                //temp += "1q[" + dr[field[n].ToString()].ToString() + ":" + in_dr[in_field[n].ToString()].ToString() + "]"; // TEMP

                                                //temp += "lq[" + service_xn.SelectSingleNode("service/mapping/table[@name='ia_service_request']/field[@name='status']/if[@id='" + dr[field[n].ToString()].ToString() + "']").Attributes["id_from"].Value;

                                                if (dr[field[n].ToString()].ToString() == "" || serviceXn.SelectSingleNode("service/mapping/table[@name='ia_service_request']/field[@name='status']/if[@id='" + dr[field[n].ToString()].ToString() + "']").Attributes["id_from"].Value != in_dr[inField[n].ToString()].ToString())
                                                {
                                                    identical = false;
                                                    break;
                                                }

                                                //temp += "1q:end|"; // TEMP
                                            }
                                            catch (FormatException)
                                            {
                                                identical = false;
                                                break;
                                            }
                                        }
                                        else { }
                                    }

                                    //temp += "1r|"; // TEMP

                                    if (identical)
                                    {
                                        // rows are the exact same. Do nothing
                                    }
                                    else
                                    {
                                        // row was updated
                                        for (int n = 0; n < inField.Length; n++)
                                        {
                                            rule = fieldRule[n];

                                            //temp += "2|"; // TEMP

                                            if (rule == F.Bit)
                                            {
                                                //temp += "2a|"; // TEMP
                                                try { dr[field[n].ToString()] = (bool)in_dr[inField[n].ToString()]; }
                                                catch (InvalidCastException) { dr[field[n].ToString()] = DBNull.Value; }
                                            }
                                            else if (rule == F.In)
                                            {
                                                //temp += "2b|"; // TEMP
                                                try { dr[field[n].ToString()] = long.Parse(in_dr[inField[n].ToString()].ToString()); }
                                                catch (FormatException) { dr[field[n].ToString()] = DBNull.Value; }
                                            }
                                            else if (rule == F.St)
                                            {
                                                //temp += "2c|"; // TEMP
                                                if (in_dr[inField[n].ToString()] == DBNull.Value) dr[field[n].ToString()] = DBNull.Value;
                                                else dr[field[n].ToString()] = in_dr[inField[n].ToString()];
                                            }
                                            else if (rule == F.Sdt)
                                            {
                                                //temp += "2d|"; // TEMP
                                                in_sdt = DateTime.Parse(in_dr[inField[n].ToString()].ToString());
                                                dr[field[n].ToString()] = SmallDateTime(in_sdt);
                                            }
                                            else if (rule == F.Srs)
                                            {
                                                //temp += "2f|"; // TEMP
                                                try { dr[field[n].ToString()] = long.Parse(serviceXn.SelectSingleNode("service/mapping/table[@name='ia_service_request']/field[@name='status']/if[@id_from='" + in_dr["status"].ToString() + "']").Attributes["id"].Value); }
                                                catch (FormatException) { dr[field[n].ToString()] = 0; }
                                            }
                                            else if (rule == F.Up)
                                            {
                                                //temp += "2g|"; // TEMP
                                                dr[field[n].ToString()] = SmallDateTime(DateTime.UtcNow.AddHours(3));
                                            }

                                            //temp += "3|"; // TEMP
                                        }

                                        c++;
                                    }
                                }
                                else
                                {
                                    // row does not exists, we will add it to database

                                    dr = dt.NewRow();

                                    for (int n = 0; n < inField.Length; n++)
                                    {
                                        rule = fieldRule[n];

                                        if (rule == F.Bit)
                                        {
                                            try { dr[field[n].ToString()] = (bool)in_dr[inField[n].ToString()]; }
                                            catch (InvalidCastException) { dr[field[n].ToString()] = DBNull.Value; }
                                        }
                                        else if (rule == F.In)
                                        {
                                            try { dr[field[n].ToString()] = long.Parse(in_dr[inField[n].ToString()].ToString()); }
                                            catch (FormatException) { dr[field[n].ToString()] = DBNull.Value; }
                                        }
                                        else if (rule == F.St)
                                        {
                                            dr[field[n].ToString()] = in_dr[inField[n].ToString()];
                                        }
                                        else if (rule == F.Sdt)
                                        {
                                            in_sdt = DateTime.Parse(in_dr[inField[n].ToString()].ToString());
                                            dr[field[n].ToString()] = SmallDateTime(in_sdt);
                                        }
                                        else if (rule == F.Srs)
                                        {
                                            try { dr[field[n].ToString()] = long.Parse(serviceXn.SelectSingleNode("service/mapping/table[@name='ia_service_request']/field[@name='status']/if[@id_from='" + in_dr["status"].ToString() + "']").Attributes["id"].Value); }
                                            catch (FormatException) { dr[field[n].ToString()] = 0; }
                                        }
                                        else if (rule == F.Cr || rule == F.Up)
                                        {
                                            dr[field[n].ToString()] = SmallDateTime(DateTime.UtcNow.AddHours(3));
                                        }
                                    }

                                    // TEMP
                                    //temp = "";
                                    //foreach (DataColumn dc in dr.Table.Columns) temp += "|" + dr[dc.ColumnName];

                                    dt.Rows.Add(dr);
                                    c++;
                                }
                            }

                            scb.GetUpdateCommand();
                            sda.Update(ds, tableName);
                            sc.Close();

                            result = "(" + c + "-" + count_delete + "/" + count_in + "/" + count + ")";

                            if (c > 0 || count_delete > 0) op = 1;
                            //}
                            //else
                            //{
                            //   result += "(0-0/*/0)";
                            //   op = 0;
                            //}
                        }
                        else
                        {
                            result += "(0-0/0/*)";
                            op = 0;
                        }
                    }
                    else
                    {
                        result += "(0-0/*/null)";
                        op = -1;
                    }
                }
                else
                {
                    result += "(0-0/null/*)";
                    op = -1;
                }

                // TEMP
                //temp += "SOFT END|"; // TEMP
                //result += "  TEMP=[" + temp + "]"; // TEMP
            }
            catch (Exception ex)
            {
#if DEBUG
                // TEMP
                //temp += "CATCH|"; // TEMP
                result = "global::Ia.Cl.Model.Db.SqlServer.Update(): " + ex.ToString(); // +"  TEMP=[" + temp + "]"; // TEMP
#else
                result = "global::Ia.Cl.Model.Db.SqlServer.Update(): " + ex.Message;
#endif

                // for debugging
                /*
                string d;

                d = "Select: [" + select_command + "] " + now.ToString() + "\n";
                d += "Row: [" + temp + "]\n";
                d += "in_dt:---------------------\n";
                d += temp_in_dt_str + "\n";
                d += "dt:---------------------\n";
                d += temp_dt_str + "\n";

                global::Ia.Cl.Model.Log.Append("error.txt", d + "\n");

                foreach (DataRow r in dt.Rows)
                {
                    d = "\n";
                    foreach (DataColumn c2 in dt.Columns) d += ":" + r[c2].ToString();
                    global::Ia.Cl.Model.Log.Append("error.txt", d + "\n");
                }
                */

                op = -1;
            }

            return op;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void Log(string text)
        {
            string line;
            DateTime now;

            now = DateTime.UtcNow.AddHours(3);

            line = now.ToString("yyyy-MM-dd HH:mm:ss: ") + text;

            logStringBuilder.AppendLine(line);
            Console.WriteLine(line);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Message
        {
            get
            {
                return logStringBuilder.ToString();
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}

/*

<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Import Namespace="System.ComponentModel" %>

<script runat="server">

    public void Page_Load(Object sender, EventArgs e) {  

        //QUICK OVERVIEW OF SYNTAX:
        / *
        int numberOfEmployees = GetInteger("SELECT COUNT(*) FROM Employees");
        int numberOfEmployees = GetInteger("SELECT COUNT(*) FROM Employees WHERE Country=@country","@country,varchar/255," + selectedCountry);
        string currentLastName = GetString("SELECT LastName FROM Employees WHERE EmployeeId=8");
        string currentLastName = GetString("SELECT LastName FROM Employees WHERE EmployeeId=@employeeId","@employeeId,int," + selectedId.ToString());      
        DateTime shippedDate = GetDate("SELECT ShippedDate FROM Orders WHERE OrderId=10259");      
        DateTime shippedDate = GetDate("SELECT ShippedDate FROM Orders WHERE OrderId=@orderId","@orderId,int," + currentOrderId.ToString());
        DataRow drEmployee = GetDataRow("SELECT * FROM Employees WHERE EmployeeId=3");
        DataRow drEmployee = GetDataRow("SELECT * FROM Employees WHERE EmployeeId=@employeeId","@employeeId,int," + currentEmployeeId.ToString());
        DataTable dtEmployees = GetDataTable("SELECT * FROM Employees ORDER BY LastName");
        DataTable dtEmployees = GetDataTable("SELECT * FROM Employees WHERE Country=@currentCountry AND BirthDate<@cutOffDate AND EmployeeId>@employeeId","@currentCountry,varchar/255,USA;@cutOffDate,date," + cutOffDate.ToString() + ";@employeeId,int,2");
        DoCommand("UPDATE Employees SET LastName='Smithinew' WHERE EmployeeId=9");
        DoCommand("UPDATE Employees SET LastName='Thompsonnew' WHERE EmployeeId=@id","@id,int," + theId.ToString());
        * /

        //get an integer
        int numberOfEmployees = GetInteger("SELECT COUNT(*) FROM Employees");
        litShow1.Text = "There are " + numberOfEmployees.ToString() + " employees";
        
        //get an integer with parameters
        string selectedCountry = "UK";
        int numberOfEmployees2 = GetInteger("SELECT COUNT(*) FROM Employees WHERE Country=@country","@country,varchar/255," + selectedCountry);
        litShow2.Text = "There are " + numberOfEmployees2.ToString() + " employees from: " + selectedCountry;
        
        //get a string
        string currentLastName = GetString("SELECT LastName FROM Employees WHERE EmployeeId=8");
        litShow3.Text = "The last name of the person selected is <b>" + currentLastName + "</b>. ";
        
        //get a string with parameters
        int selectedId = 4;
        string currentLastName2 = GetString("SELECT LastName FROM Employees WHERE EmployeeId=@employeeId","@employeeId,int," + selectedId.ToString());
        litShow4.Text = "The last name of the person with id " + selectedId.ToString() + " is <b>" + currentLastName2 + "</b>. ";
        
        //get a date
        CultureInfo ci = new CultureInfo("en-US");        
        DateTime shippedDate = GetDate("SELECT ShippedDate FROM Orders WHERE OrderId=10259");
        litShow5.Text = "Your order shipped on <b>" + shippedDate.ToString("dddd, MMMM d, yyyy",ci) + "</b>. ";

        //get a date with parameters
        int currentOrderId = 10264;
        CultureInfo ci2 = new CultureInfo("en-US");        
        DateTime shippedDate2 = GetDate("SELECT ShippedDate FROM Orders WHERE OrderId=@orderId","@orderId,int," + currentOrderId.ToString());
        litShow6.Text = "Your order shipped on <b>" + shippedDate2.ToString("dddd, MMMM d, yyyy",ci2) + "</b>. ";

        //get one record
        DataRow drEmployee = GetDataRow("SELECT * FROM Employees WHERE EmployeeId=3");
        litShow7.Text = "The selected employee is <b>" + drEmployee["FirstName"].ToString() + " " + drEmployee["LastName"].ToString() + "</b>";
        
        //get one record with parameters
        int currentEmployeeId = 8;
        DataRow drEmployee2 = GetDataRow("SELECT * FROM Employees WHERE EmployeeId=@employeeId","@employeeId,int," + currentEmployeeId.ToString());
        litShow8.Text = "The employee with id#" + currentEmployeeId.ToString() + " is <b>" + drEmployee2["FirstName"].ToString() + " " + drEmployee2["LastName"].ToString() + "</b>";
        
        //get several records
        DataTable dtEmployees = GetDataTable("SELECT * FROM Employees ORDER BY LastName");
        foreach(DataRow row in dtEmployees.Rows) {
            litShow9.Text += row["LastName"].ToString() + ", " + row["FirstName"].ToString() + "<br>";
        }
        
        //get several records with parameters
        DateTime cutOffDate = DateTime.Parse("1/1/1963");
        DataTable dtEmployees2 = GetDataTable("SELECT * FROM Employees WHERE Country=@currentCountry AND BirthDate<@cutOffDate AND EmployeeId>@employeeId","@currentCountry,varchar/255,USA;@cutOffDate,date," + cutOffDate.ToString() + ";@employeeId,int,2");
        foreach(DataRow row in dtEmployees2.Rows) {
            litShow10.Text += row["EmployeeId"].ToString() + ". " + row["LastName"].ToString() + ", " + row["FirstName"].ToString() + " (" + row["Country"].ToString() + ")<br>";
        }
        
        //perform an SQL command
        DoCommand("UPDATE Employees SET LastName='Smithinew' WHERE EmployeeId=9");
        
        //perform an SQL command
        int theId = 1;
        DoCommand("UPDATE Employees SET LastName='Thompsonnew' WHERE EmployeeId=@id","@id,int," + theId.ToString());
        
    }
    
    //EXAMPLE: int numberOfEmployees = GetInteger("SELECT COUNT(*) FROM Employees");
    public int GetInteger(string sql) {    
		int r = 0;
		SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCommand cmd = new SqlCommand(sql,con);
		r = (int)cmd.ExecuteScalar();
		con.Close();    
		return r;
	}

    //EXAMPLE: int numberOfEmployees = GetInteger("SELECT COUNT(*) FROM Employees WHERE Country=@country","@country,varchar/255,UK");
    public int GetInteger(string sql, string parameterList) {    
		int r = 0;
		SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCommand cmd = new SqlCommand(sql,con);
		GetParameters(ref cmd, parameterList);
		r = (int)cmd.ExecuteScalar();
		con.Close();    
		return r;
	}
    
    //EXAMPLE: string currentLastName = GetString("SELECT LastName FROM Employees WHERE EmployeeId=8");
    public string GetString(string sql) {    
		string rs = "";
		SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCommand cmd = new SqlCommand(sql,con);
		rs = (string)cmd.ExecuteScalar();
		con.Close();    
		return rs;
	}
    
    //EXAMPLE: string currentLastName = GetString("SELECT LastName FROM Employees WHERE EmployeeId=@employeeId","@employeedId,int," + selectedId.ToString());
    public string GetString(string sql, string parameterList) {    
		string rs = "";
		SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCommand cmd = new SqlCommand(sql,con);
		GetParameters(ref cmd, parameterList);
		rs = (string)cmd.ExecuteScalar();
		con.Close();    
		return rs;
	}
	
	//EXAMPLE: DateTime shippedDate = GetDate("SELECT ShippedDate FROM Orders WHERE OrderId=10259");
    public DateTime GetDate(string sql) {    
		DateTime rd;
		SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCommand cmd = new SqlCommand(sql,con);
		rd = (DateTime)cmd.ExecuteScalar();
		con.Close();    
		return rd;
	}
    
	
	//EXAMPLE: DateTime shippedDate = GetDate("SELECT ShippedDate FROM Orders WHERE OrderId=@orderId","@orderId,int," + currentOrderId.ToString());
    public DateTime GetDate(string sql, string parameterList) {    
		DateTime rd;
		SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCommand cmd = new SqlCommand(sql,con);
		GetParameters(ref cmd, parameterList);
		rd = (DateTime)cmd.ExecuteScalar();
		con.Close();    
		return rd;
	}

    //EXAMPLE: DataRow drEmployee = GetDataRow("SELECT * FROM Employees WHERE EmployeeId=3");
    public DataRow GetDataRow(string sql) {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        SqlCommand cmd = new SqlCommand(sql, con);
        da.SelectCommand = cmd;
        da.Fill(ds);
        try {
            dt = ds.Tables[0];
            return dt.Rows[0];
        }
        catch {
            return null;
        }
    }

    //EXAMPLE: DataRow drEmployee2 = GetDataRow("SELECT * FROM Employees WHERE EmployeeId=@employeeId","@employeeId,int," + currentEmployeeId.ToString());
    public DataRow GetDataRow(string sql,string parameterList) {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        SqlCommand cmd = new SqlCommand(sql, con);
		GetParameters(ref cmd, parameterList);
        da.SelectCommand = cmd;
        da.Fill(ds);
        try {
            dt = ds.Tables[0];
            return dt.Rows[0];
        }
        catch {
            return null;
        }
    }

    //EXAMPLE: DataTable dtEmployees = GetDataTable("SELECT * FROM Employees ORDER BY LastName");
    public DataTable GetDataTable(string sql) {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        SqlCommand cmd = new SqlCommand(sql, con);
        da.SelectCommand = cmd;
        da.Fill(ds);
        try {
            dt = ds.Tables[0];
            return dt;
        }
        catch {
            return null;
        }
    }
    
    //EXAMPLE: DataTable dtEmployees2 = GetDataTable("SELECT * FROM Employees WHERE Country=@currentCountry AND BirthDate<@cutOffDate AND EmployeeId>@employeeId","@currentCountry,varchar/255,USA;@cutOffDate,date," + cutOffDate.ToString() + ";@employeeId,int,2");
    public DataTable GetDataTable(string sql, string parameterList) {
        //parameterList will be in this form: "@currentCountry,varchar/255,USA;@cutOffDate,date,1/1/1963"
        
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        SqlCommand cmd = new SqlCommand(sql, con);
        GetParameters(ref cmd, parameterList);
        da.SelectCommand = cmd;
        da.Fill(ds);
        try {
            dt = ds.Tables[0];
            return dt;
        }
        catch {
            return null;
        }
    }

    //EXAMPLE: DoCommand("UPDATE Employees SET LastName='Smithinew' WHERE EmployeeId=9");
    public void DoCommand(string sql) {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = sql;
        cmd.Connection = con;
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
    }

    //EXAMPLE: DoCommand("UPDATE Employees SET LastName='Thompsonnew' WHERE EmployeeId=@id","@id,int," + theId.ToString());
    public void DoCommand(string sql, string parameterList) {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["con"]);
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = sql;
        GetParameters(ref cmd, parameterList);
        cmd.Connection = con;
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
    }

    //used by the other methods
    public void GetParameters(ref SqlCommand cmd, string parameterList) {
        //build parameters from the parameter list
        string[] parameterLines = parameterList.Split(';');
        foreach(string parameterLine in parameterLines) { 
            //break up individual line
            string[] parts = parameterLine.Split(',');
            switch(parts[1].ToString().Substring(0,3).ToUpper()) {
                case "VAR":
                    //get the size from e.g. "varchar/255"
                    string[] half = parts[1].ToString().Split('/');
                    int size = Int32.Parse(half[1]);
                    cmd.Parameters.Add(parts[0],SqlDbType.VarChar,size).Value = parts[2];
                    break;
                case "DAT":
                    cmd.Parameters.Add(parts[0],SqlDbType.DateTime).Value = DateTime.Parse(parts[2]);
                    break;
                case "INT":
                    cmd.Parameters.Add(parts[0],SqlDbType.Int).Value = Int32.Parse(parts[2]);
                    break;
            }
        }       
    }

</script>
<html>
    <head>
    </head>
    <body>
        <form runat="server">
            <asp:Literal id="litShow1" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow2" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow3" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow4" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow5" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow6" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow7" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow8" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow9" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow10" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow11" runat="server"></asp:Literal>
            <hr>
            <asp:Literal id="litShow12" runat="server"></asp:Literal>
            <hr>
            
        </form>
    </body>
</html>
*/
