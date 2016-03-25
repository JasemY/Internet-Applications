using System;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Ia.Cs.Db
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// SQL Server CE support class.
    /// </summary>
    /// <remarks>
    /// Copyright © 2008-2013 Jasem Y. Al-Shamlan (info@ia.com.kw), Internet Applications - Kuwait. All Rights Reserved.
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
    public class SqlServerCe
    {
        private string connectionString;
        private ArrayList al, from_al, insert_al, delete_al;
        private SqlCeConnection sc;
        private SqlCeCommand sco;

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
            Sdt,
            /// <summary/>
            Sdt_Keep_Latest,
            /// <summary/>
            Cr,
            /// <summary/>
            Up,
            /// <summary/>
            Ni
        };

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize database with connection string from app.config.
        /// </summary>
        public SqlServerCe()
        {
            connectionString = ConfigurationManager.ConnectionStrings["SqlServerCeConnectionString"].ConnectionString;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initialize database with connection string from app.config but with the passed database file name.
        /// </summary>
        public SqlServerCe(string _databaseName)
        {
            connectionString = Connection_String_Specific(_databaseName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Database_Exist()
        {
            // below: check if database exists
            bool b;

            b = true;

            try
            {
                Sql("SELECT GETDATE() AS date");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("The database file cannot be found. ")) b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Create_Database()
        {
            // below:
            bool b;
            SqlCeEngine sce;

            b = true;

            try
            {
                sce = new SqlCeEngine(connectionString);
                sce.CreateDatabase();
                sce.Dispose();
            }
            catch (Exception)
            {
                b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the connection string given the database file name passed.
        /// </summary>
        private string Connection_String_Specific(string _database_file_name)
        {
            string s;

            // below: this will copy and replace the original database file name with the provided one.
            s = Regex.Replace(connectionString, @"(\w+?\.sdf)", _database_file_name);

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Sql(string sql)
        {
            // below: execute an SQL command
            bool b = true;

            sc = new SqlCeConnection(connectionString);
            sco = new SqlCeCommand();

            sco.CommandType = Server.HtmlEncode(CommandType.Text); // default
            sco.CommandText = sql;
            sco.Connection = sc;
            sc.Open();
            sco.ExecuteNonQuery();
            sc.Close();

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute an SQL statement over a database file. The files is assumed to be in the data directory.
        /// </summary>
        public bool Sql(string sql, string database_file)
        {
            // below: execute an SQL command
            bool b = true;

            sc = new SqlCeConnection(Connection_String_Specific(database_file));
            sco = new SqlCeCommand();

            sco.CommandType = Server.HtmlEncode(CommandType.Text); // default
            sco.CommandText = sql;
            sco.Connection = sc;
            sc.Open();
            sco.ExecuteNonQuery();
            sc.Close();

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public DataTable Sp(string sp_name, params object[] list)
        {
            // below: return data from a stored procedure

            // ERRORS

            object o;
            int i;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRow dr = null;
            SqlCeDataReader sdr = null;

            try
            {
                sc = new SqlCeConnection(connectionString);
                sco = new SqlCeCommand(sp_name, sc);

                sco.CommandType = CommandType.StoredProcedure;

                for (i = 0; i < list.Length; i += 2)
                {
                    o = list[i];
                    if (o.GetType() == typeof(string))
                    {
                        o = list[i + 1];
                        if (o.GetType() == typeof(string))
                        {
                            sco.Parameters.Add(new SqlCeParameter(list[i].ToString(), list[i + 1].ToString()));
                            dt.Columns.Add(new DataColumn(list[i].ToString().Replace("@", ""), System.Type.GetType("System.String")));
                        }
                        else if (o.GetType() == typeof(int))
                        {
                            sco.Parameters.Add(new SqlCeParameter(list[i].ToString(), (int)list[i + 1]));
                            dt.Columns.Add(new DataColumn(list[i].ToString().Replace("@", ""), System.Type.GetType("System.Int32")));
                        }
                    }
                }

                sc.Open();

                sdr = sco.ExecuteReader();

                while (sdr.Read())
                {
                    dr = dt.NewRow();

                    for (i = 0; i < dt.Columns.Count; i++)
                    {
                        dr[i] = sdr[dt.Columns[i].ColumnName];
                    }
                }

                sc.Close();
            }
            finally
            {

                if (sc != null) sc.Close();

                if (sdr != null) sdr.Close();
            }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public DataTable Select(string sql)
        {
            // below: return a DataTable of result rows

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlCeDataAdapter da = new SqlCeDataAdapter();

            try
            {
                sc = new SqlCeConnection(connectionString);
                sco = new SqlCeCommand(sql, sc);

                sc.Open();

                da.SelectCommand = sco;

                da.Fill(ds);

                sc.Close();

                dt = ds.Tables[0];
            }
            catch { dt = null; }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return a DataTable of result rows and use a database_file. This will assume the file to be in the data directory.
        /// </summary>
        public DataTable Select(string sql, string database_file)
        {
            // below:
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            SqlCeDataAdapter da = new SqlCeDataAdapter();

            try
            {
                sc = new SqlCeConnection(Connection_String_Specific(database_file));
                sco = new SqlCeCommand(sql, sc);

                sc.Open();

                da.SelectCommand = sco;

                da.Fill(ds);

                sc.Close();

                dt = ds.Tables[0];
            }
            catch (Exception)
            {
                dt = null;
            }

            return dt;
        }

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

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Misc_Update(string name, string value)
        {
            Sql(@"UPDATE ia_misc SET value = '" + value + "' WHERE name = '" + name + "'");
        }

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
        public string Scalar(string sql)
        {
            // below: return a scaler
            string s;

            sc = new SqlCeConnection(connectionString);
            sco = new SqlCeCommand(sql, sc);

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
        public int Scalar_Integer(string sql)
        {
            // below: return a DataTable of result rows
            int n;

            sc = new SqlCeConnection(connectionString);
            sco = new SqlCeCommand(sql, sc);

            sc.Open();

            try
            {
                n = (System.Int32)sco.ExecuteScalar();
            }
            catch
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
        public int Scalar_SmallInteger(string sql)
        {
            // below: return a DataTable of result rows
            int n;

            sc = new SqlCeConnection(connectionString);
            sco = new SqlCeCommand(sql, sc);

            sc.Open();

            try
            {
                n = (System.Int16)sco.ExecuteScalar();
            }
            catch
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
            // below: return an SQL Server friendly string of a smalldatetime value
            string s;

            //s = "'" + dt.ToString("yyyy-MM-ddTHH:mm:ss") + "'";
            s = dt.ToString("yyyy-MM-ddTHH:mm:ss");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Range_Sql(ArrayList al, string term)
        {
            // below: this will take a special ArrayList of ranges and will construct an SQL format that correspond to the array.
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
        public bool Xml_Import(string tableName, XDocument xd, out string r)
        {
            bool b;
            string parameters, values;
            string c;//, path;
            //DataSet ds;
            SqlCeDataAdapter sda;

            r = "";

            // below: first we delete all records in table
            Sql(@"DELETE FROM " + tableName);

            // below: iterate through Xml records and insert into database

            //ds.ReadXml(file);

            sc = new SqlCeConnection(connectionString);
            sc.Open();

            c = @"SELECT * FROM [" + tableName + @"]";
            sda = new SqlCeDataAdapter(c, sc);
            //scb = new SqlCeCommandBuilder(sda);

            try
            {
                foreach (XElement xe in xd.Descendants("row"))
                {
                    parameters = values = null;

                    foreach (XElement xe2 in xe.Descendants())
                    {
                        if (xe2.Name != "id")
                        {
                            parameters += xe2.Name + ",";
                            values += "'" + xe2.Value + "',";
                        }
                    }

                    parameters = parameters.Remove(parameters.Length - 1, 1);
                    values = values.Remove(values.Length - 1, 1);

                    Sql(@"INSERT INTO " + tableName + "(" + parameters + ") VALUES (" + values + ")");
                }

                b = true;
            }
            catch (Exception ex)
            {
                r = "Exception: in Xml_Import(): " + ex.Message;
                b = false;
            }
            finally
            {
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Xml_Export(string tableName, string file)
        {
            // below: perform dump or backup of database table data into an XML document
            bool b;
            string c, path;
            DataSet ds;
            SqlCeDataAdapter sda;

            c = @"SELECT * FROM [" + tableName + @"]";
            sc = new SqlCeConnection(connectionString);
            sc.Open();

            ds = new DataSet("ia_ngn");
            sda = new SqlCeDataAdapter(c, sc);

            try
            {
                sda.Fill(ds, tableName);

                path = Ia.Cs.Default.Absolute_Path();

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

        /// <summary>
        /// Log a standard logging entry into a special database table
        /// </summary>
        public void Log(int type_id, int direction_type_id, int system_id, int process_id, int function_id, string detail, DateTime created)
        {
            string sql;

            // See table ia_log and log.xml

            /*
            CREATE TABLE [dbo].[ia_log]
            (
               [id]					int	IDENTITY(1,1) CONSTRAINT [ia_log_id_pk] PRIMARY KEY,
               [type_id]				tinyint NULL,
               [direction_type_id]	tinyint NULL,
               [system_id]			smallint NULL,
               [process_id]			smallint NULL,
               [function_id]			smallint NULL,
               [detail]				ntext NULL,
               [created]				smalldatetime NULL
            )
            */

#if WINDOWS_FORM
            sql = "INSERT INTO [ia_log] ([type_id],[direction_type_id],[system_id],[process_id],[function_id],[detail],[created]) VALUES (" + type_id + "," + direction_type_id + "," + system_id + "," + process_id + "," + function_id + ",'" + detail + "','" + SmallDateTime(created) + "')";
#else
            sql = "INSERT INTO [ia_log] ([type_id],[direction_type_id],[system_id],[process_id],[function_id],[detail],[created]) VALUES (" + type_id + "," + direction_type_id + "," + system_id + "," + process_id + "," + function_id + ",'" + HttpUtility.HtmlEncode(detail) + "','" + SmallDateTime(created) + "')";
#endif

            Sql(sql);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int Update(DataTable in_dt, string tableName, string select_command, string primary_key, string[] in_field, string[] field, F[] field_rule, bool synch, string delete_rule, out string result)
        {
            bool identical, keep_latest;
            int op, c, count, count_in, count_delete;
            F rule;
            string command;

            // TEMP
            //string temp = "", temp_dt_str, temp_in_dt_str; // TEMP

            // TEMP
            //string temp = "";

            int i, j;
            Hashtable ht;

            keep_latest = false;

            al = new ArrayList(1000);
            from_al = new ArrayList(1000);
            insert_al = new ArrayList(1000);
            delete_al = new ArrayList(1000);

            ht = new Hashtable(1000);

            DateTime sdt, in_sdt;

            DataRow dr;
            DataTable dt;
            DataSet ds;
            SqlCeDataAdapter sda;
            SqlCeCommandBuilder scb;

            op = 0;
            c = count = count_in = count_delete = 0;

            //progress = 0;

            ds = new DataSet("ia");
            sc = new SqlCeConnection(connectionString);

            sc.Open();
            command = select_command;
            sda = new SqlCeDataAdapter();
            scb = new SqlCeCommandBuilder(sda);
            sda.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            sda.SelectCommand = new SqlCeCommand(command, sc);

            result = "";

            dt = null;

            // below: I will check if the records have a keep_latest field. This field means I will ignore the new record if the keep_latest date is older then
            // the existing record.
            for (int n = 0; n < in_field.Length; n++)
            {
                rule = field_rule[n];

                if (rule == F.Sdt_Keep_Latest)
                {
                    keep_latest = true; break;
                }
            }

            try
            {
                sda.Fill(ds, tableName);

                dt = ds.Tables[0];

                if (in_dt != null)
                {
                    count_in = in_dt.Rows.Count;

                    if (dt != null)
                    {
                        count = dt.Rows.Count;

                        if (in_dt.Rows.Count > 0)
                        {
                            if (synch)
                            {
                                // below: compair two lists to find records in in_dt that are not in dt
                                foreach (DataRow r in dt.Rows) al.Add(r[primary_key].ToString());
                                foreach (DataRow r in in_dt.Rows) from_al.Add(r[primary_key].ToString());

                                al.Sort();
                                from_al.Sort();

                                i = j = 0;

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

                                if (delete_al.Count > 0)
                                {
                                    for (i = 0; i < delete_al.Count && i < 100; i++)
                                    {
                                        // We will delete it, or its contents according to the deletion rules of the tableName

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

                                                    if (rule == F.Bit || rule == F.In || rule == F.St || rule == F.Sdt || rule == F.Sdt_Keep_Latest)
                                                    {
                                                        dr[field[n].ToString()] = DBNull.Value;
                                                    }
                                                    else if (rule == F.Up)
                                                    {
                                                        dr[field[n].ToString()] = SmallDateTime(DateTime.UtcNow.AddHours(3));
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

                                dr = dt.Rows.Find(long.Parse(in_dr[primary_key].ToString()));

                                //progress = c / count;

                                if (dr != null)
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
                                        else if (rule == F.Sdt)
                                        {
                                            sdt = DateTime.Parse(dr[field[n].ToString()].ToString());
                                            in_sdt = DateTime.Parse(in_dr[in_field[n].ToString()].ToString());

                                            // below: if in_sdt lays within 1 minute of sdt they are identical

                                            if (in_sdt > sdt.AddMinutes(1) || in_sdt < sdt.AddMinutes(-1))
                                            {
                                                identical = false;
                                                break;
                                            }
                                        }
                                        else { }
                                    }

                                    //if (dr["dn"].ToString() == "25645818") { }

                                    if (keep_latest)
                                    {
                                        // identical = true;

                                        for (int n = 0; n < in_field.Length; n++)
                                        {
                                            rule = field_rule[n];

                                            if (rule == F.Sdt_Keep_Latest)
                                            {
                                                // below: this will keep the record as same with no change if the new date is older than the old date

                                                sdt = DateTime.Parse(dr[field[n].ToString()].ToString());
                                                in_sdt = DateTime.Parse(in_dr[in_field[n].ToString()].ToString());

                                                // below: if in_sdt is less than sdt they are identical

                                                if (in_sdt >= sdt) identical = false;
                                                else identical = true;

                                                break;
                                            }
                                        }
                                    }

                                    if (identical)
                                    {
                                        // below: rows are the exact same. Do nothing
                                    }
                                    else
                                    {
                                        // below: row was updated
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
                                                if (in_dr[in_field[n].ToString()] == DBNull.Value) dr[field[n].ToString()] = DBNull.Value;
                                                else dr[field[n].ToString()] = in_dr[in_field[n].ToString()];
                                            }
                                            else if (rule == F.Sdt || rule == F.Sdt_Keep_Latest)
                                            {
                                                in_sdt = DateTime.Parse(in_dr[in_field[n].ToString()].ToString());
                                                dr[field[n].ToString()] = SmallDateTime(in_sdt);
                                            }
                                            else if (rule == F.Up)
                                            {
                                                dr[field[n].ToString()] = SmallDateTime(DateTime.UtcNow.AddHours(3));
                                            }
                                        }

                                        c++;
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
                                        else if (rule == F.Sdt || rule == F.Sdt_Keep_Latest)
                                        {
                                            in_sdt = DateTime.Parse(in_dr[in_field[n].ToString()].ToString());
                                            dr[field[n].ToString()] = SmallDateTime(in_sdt);
                                        }
                                        else if (rule == F.Cr || rule == F.Up)
                                        {
                                            dr[field[n].ToString()] = SmallDateTime(DateTime.UtcNow.AddHours(3));
                                        }
                                    }

                                    dt.Rows.Add(dr);
                                    c++;
                                }
                            }

                            scb.GetUpdateCommand();
                            sda.Update(ds, tableName);
                            sc.Close();

                            result = "(" + c + "-" + count_delete + "/" + count_in + "/" + count + ")";

                            if (c > 0 || count_delete > 0) op = 1;
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
                result = "Ia.Ngn.Cs.Application.Update(): " + ex.ToString(); // +"  TEMP=[" + temp + "]"; // TEMP
#else
                result = "Ia.Ngn.Cs.Application.Update(): " + ex.Message;
#endif

                op = -1;
            }

            return op;
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
		SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCeCommand cmd = new SqlCeCommand(sql,con);
		r = (int)cmd.ExecuteScalar();
		con.Close();    
		return r;
	}

    //EXAMPLE: int numberOfEmployees = GetInteger("SELECT COUNT(*) FROM Employees WHERE Country=@country","@country,varchar/255,UK");
    public int GetInteger(string sql, string parameterList) {    
		int r = 0;
		SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCeCommand cmd = new SqlCeCommand(sql,con);
		GetParameters(ref cmd, parameterList);
		r = (int)cmd.ExecuteScalar();
		con.Close();    
		return r;
	}
    
    //EXAMPLE: string currentLastName = GetString("SELECT LastName FROM Employees WHERE EmployeeId=8");
    public string GetString(string sql) {    
		string rs = "";
		SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCeCommand cmd = new SqlCeCommand(sql,con);
		rs = (string)cmd.ExecuteScalar();
		con.Close();    
		return rs;
	}
    
    //EXAMPLE: string currentLastName = GetString("SELECT LastName FROM Employees WHERE EmployeeId=@employeeId","@employeedId,int," + selectedId.ToString());
    public string GetString(string sql, string parameterList) {    
		string rs = "";
		SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCeCommand cmd = new SqlCeCommand(sql,con);
		GetParameters(ref cmd, parameterList);
		rs = (string)cmd.ExecuteScalar();
		con.Close();    
		return rs;
	}
	
	//EXAMPLE: DateTime shippedDate = GetDate("SELECT ShippedDate FROM Orders WHERE OrderId=10259");
    public DateTime GetDate(string sql) {    
		DateTime rd;
		SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCeCommand cmd = new SqlCeCommand(sql,con);
		rd = (DateTime)cmd.ExecuteScalar();
		con.Close();    
		return rd;
	}
    
	
	//EXAMPLE: DateTime shippedDate = GetDate("SELECT ShippedDate FROM Orders WHERE OrderId=@orderId","@orderId,int," + currentOrderId.ToString());
    public DateTime GetDate(string sql, string parameterList) {    
		DateTime rd;
		SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
		con.Open();
		SqlCeCommand cmd = new SqlCeCommand(sql,con);
		GetParameters(ref cmd, parameterList);
		rd = (DateTime)cmd.ExecuteScalar();
		con.Close();    
		return rd;
	}

    //EXAMPLE: DataRow drEmployee = GetDataRow("SELECT * FROM Employees WHERE EmployeeId=3");
    public DataRow GetDataRow(string sql) {
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        SqlCeDataAdapter da = new SqlCeDataAdapter();
        SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
        SqlCeCommand cmd = new SqlCeCommand(sql, con);
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
        SqlCeDataAdapter da = new SqlCeDataAdapter();
        SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
        SqlCeCommand cmd = new SqlCeCommand(sql, con);
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
        SqlCeDataAdapter da = new SqlCeDataAdapter();
        SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
        SqlCeCommand cmd = new SqlCeCommand(sql, con);
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
        SqlCeDataAdapter da = new SqlCeDataAdapter();
        SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
        SqlCeCommand cmd = new SqlCeCommand(sql, con);
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
        SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
        SqlCeCommand cmd = new SqlCeCommand();
        cmd.CommandText = sql;
        cmd.Connection = con;
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
    }

    //EXAMPLE: DoCommand("UPDATE Employees SET LastName='Thompsonnew' WHERE EmployeeId=@id","@id,int," + theId.ToString());
    public void DoCommand(string sql, string parameterList) {
        SqlCeConnection con = new SqlCeConnection(ConfigurationManager.AppSettings["con"]);
        SqlCeCommand cmd = new SqlCeCommand();
        cmd.CommandText = sql;
        GetParameters(ref cmd, parameterList);
        cmd.Connection = con;
        con.Open();
        cmd.ExecuteNonQuery();
        con.Close();
    }

    //used by the other methods
    public void GetParameters(ref SqlCeCommand cmd, string parameterList) {
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
*/
