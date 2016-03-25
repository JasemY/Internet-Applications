using System;
using System.Web;
using System.Xml;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace Ia.Model.Db
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// MySQL supporting class.
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
    public class MySql
    {
        private string connectionString, databaseName;
        private ArrayList al, from_al, insert_al, delete_al;
        private MySqlConnection msc;
        private MySqlCommand msco;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public enum F
        {
            /// <summary/>
            Bit,
            /// <summary/>
            In,
            /// <summary/>
            St,
            /// <summary/>
            Dt,
            /// <summary/>
            Dt_Accept_Newer,
            /// <summary/>
            Cr,
            /// <summary/>
            Up,
            /// <summary/>
            Re,
            /// <summary/>
            Ni
        };

        ////////////////////////////////////////////////////////////////////////////

        /*
         * The MISC function does not work I did not create the misc database nor did I test it.
         * 
         * Later create a webserivce to update and read misc values
         */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize database with connection string from web.config.
        /// </summary>
        public MySql()
        {
            if (ConfigurationManager.ConnectionStrings["MySqlConnectionString"] != null)
            {
                connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize database with connection string from web.config but with the passed database file name.
        /// </summary>
        public MySql(string _databaseName)
        {
            connectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            connectionString = Database_String(_databaseName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool TableExist(string tableName)
        {
            // below: check if database exists
            bool b;
            DataTable dt;

            b = true;

            try
            {
                dt = Select("show tables like '" + tableName + "';");

                if (dt.Rows.Count > 0) b = true;
                else b = false;
            }
            catch (Exception)
            {
                b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private string Database_String(string _databaseName)
        {
            // below: check if there is a database specified in command
            if (_databaseName != null && _databaseName.Length > 0)
            {
                // below: check if connection_string has a database specified
                if (connectionString.Contains("database"))
                {
                    databaseName = _databaseName;

                    connectionString = Regex.Replace(connectionString, @"database\s*=\s*[a-z]+?;", "database=" + databaseName + ";");
                }
                else
                {
                    connectionString += "database=" + _databaseName + ";";
                }
            }
            else
            {
                if (_databaseName != null && _databaseName.Length > 0)
                {
                    if (connectionString.Contains("database"))
                    {
                        connectionString = Regex.Replace(connectionString, @"database\s*=\s*[a-z]+?;", "database=" + _databaseName + ";");
                    }
                    else
                    {
                        connectionString += "database=" + _databaseName + ";";
                    }
                }
            }

            return connectionString;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string DateTime(DateTime dt)
        {
            // below: return a friendly string of a datetime value
            string s;

            //s = "'" + dt.ToString("dd/MM/yyyy HH:mm:ss") + "'";
            //s = dt.ToString("dd/MM/yyyy HH:mm:ss");
            s = dt.ToString("yyyy-MM-dd HH:mm:ss");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Sql(string sql)
        {
            return Sql(sql, null, false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Sql(string sql, bool b)
        {
            return Sql(sql, null, b);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Sql(string sql, string database)
        {
            return Sql(sql, database, false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute and SQL command
        /// </summary>
        /// <param name="sql">SQL string</param>
        /// <param name="changeEmptyStringQuoteToNull">Indicator weather single quotes '' should be replaced with NULL string</param>
        /// <returns>Boolean</returns>
        public bool Sql(string sql, string database, bool changeEmptyStringQuoteToNull)
        {
            // below: execute an SQL command
            bool b;
            string s;

            b = true;

            if (changeEmptyStringQuoteToNull) sql = sql.Replace("''", "NULL");

            s = Database_String(database);

            msc = new MySqlConnection(s);

            msco = new MySqlCommand();

            msco.CommandType = Server.HtmlEncode(CommandType.Text); // default
            msco.CommandText = sql;
            msco.Connection = msc;
            msc.Open();

            try
            {
                msco.ExecuteNonQuery();
                b = true;
            }
            catch (Exception)
            {
                b = false;
            }
            finally
            {
                msc.Close();
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public DataTable Select(string sql)
        {
            return Select(sql, null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public DataTable Select(string sql, string database)
        {
            // below: return a DataTable of result rows
            string s;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            MySqlDataAdapter da = new MySqlDataAdapter();

            s = Database_String(database);

            try
            {
                msc = new MySqlConnection(s);

                msco = new MySqlCommand(sql, msc);

                msc.Open();

                da.SelectCommand = msco;

                da.Fill(ds);

                msc.Close();

                dt = ds.Tables[0];
            }
            catch { dt = null; }

            return dt;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public DataTable Select(string sql)
        {
            // below: return a DataTable of result rows

            OdbcConnection sc = null;
            OdbcCommand sco;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            OdbcDataAdapter da = new OdbcDataAdapter();

            sc = new OdbcConnection(ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString);
            sco = new OdbcCommand(sql, sc);

            if (sc.State == ConnectionState.Open) sc.Close();
            if (sco.Connection.State == ConnectionState.Open) sco.Connection.Close();

            try
            {
                sc.Open();

                da.SelectCommand = sco;

                da.Fill(ds);

                sc.Close();

                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                dt = null;
            }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        public DataTable Select(string sql)
        {
            string s;
            string connection_string = "server=iis;uid=*;pwd=*;persist security info=True;database=*";
            DataTable dt = new DataTable();

            dt = null;

            OleDbConnection connection = new OleDbConnection(connection_string);
            OleDbCommand command = new OleDbCommand();

            command.Connection = connection;
            command.CommandText = sql;

            connection.Open();

            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                s = " <br><br>Error occured in Execute_Non_Query: <br>" + ex.ToString();
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }
        */


        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Misc_Select(string name)
        {
            string s;

            s = Scalar(@"SELECT value FROM ia_misc WHERE name = '" + name + "'");

            return s;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int Misc_Select(string name, out ArrayList al)
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
        public int Misc_Select(string name, out DataTable dt)
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

                // below: collect table columns
                xn = xd.DocumentElement.FirstChild;
                foreach (XmlNode n in xn.ChildNodes)
                {
                    if (n.Name == "user_id") dt.Columns.Add("user_id", typeof(System.Guid));
                    else dt.Columns.Add(n.Name);
                }

                // below: collect row values
                foreach (XmlNode n in xd.DocumentElement.ChildNodes)
                {
                    dr = dt.NewRow();

                    foreach (XmlNode o in n.ChildNodes)
                    {
                        if (o.Name == "user_id")
                        {
                            if (o.InnerText == "") dr[o.Name] = DBNull.Value;
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
        */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Misc_Update(string name, string value)
        {
            Sql(@"UPDATE ia_misc SET value = '" + value + "' WHERE name = '" + name + "'");
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Misc_Update(string name, ArrayList al)
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
        public void Misc_Update(string name, SortedList sl)
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
        public void Misc_Update(string name, DataTable dt)
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
        */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Misc_Delete(string name)
        {
            Sql(@"DELETE FROM ia_misc WHERE name = '" + name + "'");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Misc_Insert(string name)
        {
            Sql(@"INSERT INTO ia_misc (name) VALUES ('" + name + "')");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Misc_Insert(string name, string value)
        {
            Sql(@"INSERT INTO ia_misc (name,value) VALUES ('" + name + "','" + value + "')");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Scalar(string sql)
        {
            return Scalar(sql, null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Scalar(string sql, string database)
        {
            // below: return a scaler
            string s;

            s = Database_String(database);

            msc = new MySqlConnection(s);
            msco = new MySqlCommand(sql, msc);

            msc.Open();

            try
            {
                s = msco.ExecuteScalar().ToString();
            }
            catch
            {
                s = null;
            }
            finally
            {
                msc.Close();
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int Update(DataTable in_dt, string table_name, string select_command, string primary_key, string[] in_field, string[] field, F[] field_rule, bool synch, string delete_rule, out string result)
        {
            return Update(in_dt, table_name, select_command, primary_key, in_field, field, field_rule, synch, delete_rule, null, out result);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int Update(DataTable in_dt, string table_name, string select_command, string primary_key, string[] in_field, string[] field, F[] field_rule, bool synch, string delete_rule, string database, out string result)
        {
            bool identical, ignore, accept_newer;
            int op, c, count, count_in, count_delete;
            F rule;
            string s, u, command;

            //string temp = "", temp_dt_str, temp_in_dt_str; // TEMP

            int i, j;
            Hashtable ht;

            accept_newer = false;

            al = new ArrayList(1000);
            from_al = new ArrayList(1000);
            insert_al = new ArrayList(1000);
            delete_al = new ArrayList(1000);

            ht = new Hashtable(1000);

            DateTime sdt, in_sdt;

            DataRow dr;
            DataTable dt;
            DataSet ds;
            MySqlDataAdapter msda;
            MySqlConnection msc;
            MySqlCommandBuilder mscb;

            op = 0;
            c = count = count_in = count_delete = 0;

            ds = new DataSet("ia");
            s = Database_String(database);
            msc = new MySqlConnection(s);

            //sc = new SqlConnection(path);

            msc.Open();
            command = select_command;
            msda = new MySqlDataAdapter();
            msda.SelectCommand = new MySqlCommand(command, msc);
            msda.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            mscb = new MySqlCommandBuilder(msda);

            result = "";

            //temp_in_dt_str = temp_dt_str = "";

            dt = null;

            // below: I will check if the records have a accept_newer field. This field means I will ignore the new record if the accept_newer date is older then
            // the existing record.
            for (int n = 0; n < in_field.Length; n++)
            {
                rule = field_rule[n];

                if (rule == F.Dt_Accept_Newer)
                {
                    accept_newer = true; break;
                }
            }

            try
            {
                msda.Fill(ds, table_name);

                dt = ds.Tables[0];

                if (in_dt != null)
                {
                    count_in = in_dt.Rows.Count;

                    // TEMP
                    //foreach (DataRow r in in_dt.Rows)
                    //{
                    //    temp_in_dt_str += "\n";
                    //    foreach (DataColumn c2 in in_dt.Columns) temp_in_dt_str += ":" + r[c2].ToString();
                    //}

                    if (dt != null)
                    {
                        count = dt.Rows.Count;

                        // TEMP
                        //foreach (DataRow r in dt.Rows)
                        //{
                        //    temp_dt_str += "\n";
                        //    foreach (DataColumn c2 in dt.Columns) temp_dt_str += ":" + r[c2].ToString();
                        //}

                        if (in_dt.Rows.Count > 0)
                        {
                            //if (dt.Rows.Count > 0)
                            //{
                            if (synch)
                            {
                                // below: compair two lists to find records in in_dt that are not in dt
                                foreach (DataRow r in dt.Rows) al.Add(r[primary_key].ToString());
                                foreach (DataRow r in in_dt.Rows) from_al.Add(r[primary_key].ToString());

                                al.Sort();
                                from_al.Sort();

                                i = j = 0;

                                // below: I will assume that from_al is longer than al
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

                                        if (delete_rule == "all")
                                        {
                                            dr.Delete();
                                        }
                                        else if (delete_rule == "keep primary key")
                                        {
                                            // below: this will delete everything but keep only the primary key

                                            for (int n = 0; n < in_field.Length; n++)
                                            {
                                                if (field[n].ToString() != primary_key)
                                                {
                                                    rule = field_rule[n];

                                                    if (rule == F.Bit || rule == F.In || rule == F.St || rule == F.Dt || rule == F.Dt_Accept_Newer)
                                                    {
                                                        dr[field[n].ToString()] = DBNull.Value;
                                                    }
                                                    else if (rule == F.Up || rule == F.Re)
                                                    {
                                                        dr[field[n].ToString()] = DateTime(System.DateTime.UtcNow.AddHours(3));
                                                    }
                                                }
                                            }
                                        }

                                        count_delete++;
                                    }
                                }
                            }

                            foreach (DataRow in_dr in in_dt.Rows)
                            {
                                // below: collect relevent values:

                                //if (in_dr[primary_key].ToString() == "95126013") op++;

                                dr = dt.Rows.Find(in_dr[primary_key].ToString());

                                if (dr != null)
                                {
                                    // below: if the accept newer flag is on we will ignore records that are older than the current record
                                    ignore = false;
                                    if (accept_newer)
                                    {
                                        // identical = true;

                                        for (int n = 0; n < in_field.Length; n++)
                                        {
                                            rule = field_rule[n];

                                            if (rule == F.Dt_Accept_Newer)
                                            {
                                                // below: this will keep the record as same with no change if the new date is older than the old date

                                                sdt = System.DateTime.Parse(dr[field[n].ToString()].ToString());
                                                in_sdt = System.DateTime.Parse(in_dr[in_field[n].ToString()].ToString());

                                                // below: if in_sdt is less than sdt they are identical

                                                if (in_sdt > sdt) ignore = false;
                                                else ignore = true;

                                                break;
                                            }
                                        }
                                    }

                                    if (ignore)
                                    {
                                    }
                                    else
                                    {
                                        // below: check if rows are identical

                                        identical = true;

                                        for (int n = 0; n < in_field.Length; n++)
                                        {
                                            rule = field_rule[n];

                                            if (rule == F.Bit)
                                            {
                                                try
                                                {
                                                    if (dr[field[n].ToString()].ToString() != in_dr[in_field[n].ToString()].ToString())
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
                                                try
                                                {
                                                    if (dr[field[n].ToString()].ToString() != in_dr[in_field[n].ToString()].ToString())
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
                                                if (dr[field[n].ToString()] == DBNull.Value && in_dr[in_field[n].ToString()] == DBNull.Value) { }
                                                else if (dr[field[n].ToString()] == DBNull.Value && in_dr[in_field[n].ToString()] != DBNull.Value) { identical = false; break; }
                                                else if (dr[field[n].ToString()] != DBNull.Value && in_dr[in_field[n].ToString()] == DBNull.Value) { identical = false; break; }
                                                else if (dr[field[n].ToString()].ToString() != in_dr[in_field[n].ToString()].ToString())
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                            }
                                            else if (rule == F.Dt)
                                            {
                                                sdt = System.DateTime.Parse(dr[field[n].ToString()].ToString());
                                                in_sdt = System.DateTime.Parse(in_dr[in_field[n].ToString()].ToString());

                                                // below: if in_sdt lays within 1 minute of sdt they are identical

                                                if (in_sdt > sdt.AddMinutes(1) || in_sdt < sdt.AddMinutes(-1))
                                                {
                                                    identical = false;
                                                    break;
                                                }
                                            }
                                            else { }
                                        }

                                        if (identical)
                                        {
                                            // below: rows are the exact same

                                            for (int n = 0; n < in_field.Length; n++)
                                            {
                                                rule = field_rule[n];

                                                if (rule == F.Re)
                                                {
                                                    dr[field[n].ToString()] = DateTime(System.DateTime.UtcNow.AddHours(3));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // below: row was updated
                                            for (int n = 0; n < in_field.Length; n++)
                                            {
                                                rule = field_rule[n];

                                                if (rule == F.Bit)
                                                {
                                                    // below: I can not use bool. The true and false values are 1 and 0.
                                                    try
                                                    {
                                                        u = in_dr[in_field[n].ToString()].ToString();

                                                        if (u == "1") dr[field[n].ToString()] = true;
                                                        else if (u == "0") dr[field[n].ToString()] = false;
                                                        else dr[field[n].ToString()] = DBNull.Value;
                                                    }
                                                    catch (Exception) { dr[field[n].ToString()] = DBNull.Value; }
                                                }
                                                else if (rule == F.In)
                                                {
                                                    try { dr[field[n].ToString()] = long.Parse(in_dr[in_field[n].ToString()].ToString()); }
                                                    catch (FormatException) { dr[field[n].ToString()] = DBNull.Value; }
                                                }
                                                else if (rule == F.St)
                                                {
                                                    if (in_dr[in_field[n].ToString()] == DBNull.Value) dr[field[n].ToString()] = DBNull.Value;
                                                    else dr[field[n].ToString()] = in_dr[in_field[n].ToString()];
                                                }
                                                else if (rule == F.Dt || rule == F.Dt_Accept_Newer)
                                                {
                                                    in_sdt = System.DateTime.Parse(in_dr[in_field[n].ToString()].ToString());
                                                    dr[field[n].ToString()] = DateTime(in_sdt);
                                                }
                                                else if (rule == F.Up || rule == F.Re)
                                                {
                                                    dr[field[n].ToString()] = DateTime(System.DateTime.UtcNow.AddHours(3));
                                                }
                                            }

                                            c++;
                                        }
                                    }
                                }
                                else
                                {
                                    // below: row does not exists, we will add it to database

                                    dr = dt.NewRow();

                                    for (int n = 0; n < in_field.Length; n++)
                                    {
                                        rule = field_rule[n];

                                        if (rule == F.Bit)
                                        {
                                            try { dr[field[n].ToString()] = (bool)in_dr[in_field[n].ToString()]; }
                                            catch (InvalidCastException) { dr[field[n].ToString()] = DBNull.Value; }
                                        }
                                        else if (rule == F.In)
                                        {
                                            try { dr[field[n].ToString()] = long.Parse(in_dr[in_field[n].ToString()].ToString()); }
                                            catch (FormatException) { dr[field[n].ToString()] = DBNull.Value; }
                                        }
                                        else if (rule == F.St)
                                        {
                                            dr[field[n].ToString()] = in_dr[in_field[n].ToString()];
                                        }
                                        else if (rule == F.Dt || rule == F.Dt_Accept_Newer)
                                        {
                                            in_sdt = System.DateTime.Parse(in_dr[in_field[n].ToString()].ToString());
                                            dr[field[n].ToString()] = DateTime(in_sdt);
                                        }
                                        else if (rule == F.Cr || rule == F.Up || rule == F.Re)
                                        {
                                            dr[field[n].ToString()] = DateTime(System.DateTime.UtcNow.AddHours(3));
                                        }
                                    }

                                    // TEMP
                                    //temp = "";
                                    //foreach (DataColumn dc in dr.Table.Columns) temp += "|" + dr[dc.ColumnName];

                                    dt.Rows.Add(dr);
                                    c++;
                                }
                            }

                            //msda.GetUpdateCommand();
                            msda.Update(ds, table_name);
                            msc.Close();

                            result = "(" + c + "-" + count_delete + "/" + count_in + "/" + count + ")";

                            if (c > 0 || count_delete > 0) op = 1;
                            //}
                            //else
                            //{
                            //    result += "(0-0/*/0)";
                            //    op = 0;
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
            }
            catch (Exception ex)
            {
#if DEBUG
                result = "Ia.Model.Db.MySql.Update(): " + ex.ToString();
#else
                result = "Ia.Model.Db.MySql.Update(): " + ex.Message;
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

                Ia.Model.Log.Append("error.txt", d + "\n");

                foreach (DataRow r in dt.Rows)
                {
                    d = "\n";
                    foreach (DataColumn c2 in dt.Columns) d += ":" + r[c2].ToString();
                    Ia.Model.Log.Append("error.txt", d + "\n");
                }
                */

                op = -1;
            }

            return op;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}

/*
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.OleDb;
using System.Text;

using System.Collections;  // for ArrayList

using System.Text.RegularExpressions;
using System.Globalization;

namespace IA
{
 public class MySQL : System.Web.UI.Page
 {
  protected DataSet ds;
  protected OleDbDataAdapter da;
  protected DataTable dt;

  protected Label error_l;

////////////////////////////////////////////////////////////////////////////

  protected void Page_Load(object sender, EventArgs e) 
  {
/ *
   int word_group,i=0;
   string word,language,type;

   Execute_Non_Query("DROP TABLE IF EXISTS Word");
   Execute_Non_Query(@"CREATE TABLE Word (id INT(11) AUTO_INCREMENT PRIMARY KEY,word VARCHAR(255) BINARY,word_group INT,language CHAR(2),type VARCHAR(20),frequency INT DEFAULT '0' NOT NULL )");

   word_group=0;

   language="en";
   type="PLOT";

   for(i=0;i<1000;i++)
   {
    word=i.ToString();
    try
    {
//     Execute_Non_Query(@"INSERT INTO Word (word,word_group,language,type) VALUES ('"+word+@"',"+(word_group++)+@",'"+language+@"','"+type+@"')");
    }
    catch (Exception ex)
    {
     error_l.Text += "<br><br> Error occured in Execute_Non_Query: " + ex.ToString();
    }
   }

   error_l.Text += " FINISH ";
* /

   // below: execute scalar tests
   error_l.Text += "["+Execute_Scalar("SELECT id FROM User WHERE login = 'ahmad'")+"]";
   error_l.Text += "<br><br>";
   error_l.Text += "["+Execute_Scalar("SELECT COUNT(*) FROM Word WHERE language = 'jp'")+"]";
  
  }

////////////////////////////////////////////////////////////////////////////

  protected string Execute_Scalar(string command_string)
  {
   string id;
   object obj = null; // Return Value

   OleDbConnection connection = new OleDbConnection(connection_string);
   OleDbCommand command = new OleDbCommand(command_string,connection);

   connection.Open();
   obj = command.ExecuteScalar();
   connection.Close();


   if(obj == null) { id="null"; }
   else id = Convert.ToString(obj);

   return id;

   / *

public object GetObject(string connStr, string sqlCmd) 
{
    object obj = null; // Return Value 
    SqlConnection m_SqlCn = new SqlConnection(connStr); 
    SqlCommand m_SqlCommand = new SqlCommand(sqlCmd,m_SqlCn);
    try 
    {    m_SqlCommand.Connection.Open();
        obj = m_SqlCommand.ExecuteScalar();
    } // end try
    catch (Exception e)
    {   string Er = "Error in GetObject()-> " + e.ToString();
        throw new Exception(Er); 
    } 
    finally 
    {    m_SqlCommand.Dispose();
        m_SqlConnection.Close();
    }
    return obj; 
}
 

--------------

   string id;
   object obj = null; // Return Value

   OleDbConnection connection = new OleDbConnection(connection_string);
   OleDbCommand command = new OleDbCommand();

   command.Connection=connection;
   command.CommandText=command_string;

   connection.Open();
   obj = command.ExecuteScalar();
   connection.Close();

   if(obj == null) { id="null"; }
   else id = Convert.ToString(obj);

   return id;
   * /
  }

////////////////////////////////////////////////////////////////////////////

 }
}

*/