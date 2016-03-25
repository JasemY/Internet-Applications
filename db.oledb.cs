using System;
using System.Web;
using System.Xml;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Collections;
using System.Text;

namespace Ia.Cl.Model.Db
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// OLEDB support class
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
    public class OleDb
    {
        private static ArrayList al, from_al, insert_al, delete_al;

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
            Ni
        };

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public OleDb() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Sql(string sql)
        {
            // return a DataTable of result rows
            return Sql(sql, false, ConfigurationManager.ConnectionStrings["OledbConnectionString"].ConnectionString);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Sql(string sql, bool empty_string_single_quote_to_null)
        {
            // 
            return Sql(sql, empty_string_single_quote_to_null, ConfigurationManager.ConnectionStrings["OledbConnectionString"].ConnectionString);
        }


        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Sql(string sql, string connection_string)
        {
            return Sql(sql, false, connection_string);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute and SQL command
        /// </summary>
        /// <param name="sql">SQL string</param>
        /// <param name="empty_string_single_quote_to_null">Indicator weather single quotes '' should be replaced with NULL string</param>
        /// <param name="connection_string">connection string</param>
        /// <returns>Boolean</returns>
        public static bool Sql(string sql, bool empty_string_single_quote_to_null, string connection_string)
        {
            // execute an SQL command
            bool b = true;
            OleDbConnection odc = null;
            OleDbCommand odco;

            if (empty_string_single_quote_to_null) sql = sql.Replace("''", "NULL");

            odc = new OleDbConnection(connection_string);
            odco = new OleDbCommand();

            odco.CommandType = CommandType.Text; // default
            odco.CommandText = sql;
            odco.Connection = odc;
            odc.Open();
            odco.ExecuteNonQuery();
            odc.Close();

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static DataTable Select(string sql)
        {
            // return a DataTable of result rows
            return Select(sql, ConfigurationManager.ConnectionStrings["OledbConnectionString"].ConnectionString);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static DataTable Select(string sql, string connection_string)
        {
            // return a DataTable of result rows

            OleDbConnection odc = null;
            OleDbCommand odco;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            OleDbDataAdapter da = new OleDbDataAdapter();

            odc = new OleDbConnection(connection_string);
            odco = new OleDbCommand(sql, odc);

            odc.Open();

            da.SelectCommand = odco;

            da.Fill(ds);

            odc.Close();

            try { dt = ds.Tables[0]; }
            catch { dt = null; }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Execute SQL and return a scalar.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>string</returns>
        public static string Scalar(string sql)
        {
            // return a scalar
            string s;

            s = null;

            OleDbConnection odc = null;
            OleDbCommand odco;

            odc = new OleDbConnection(ConfigurationManager.ConnectionStrings["OledbConnectionString"].ConnectionString);
            odco = new OleDbCommand();

            odco.CommandType = CommandType.Text; // default
            odco.CommandText = sql;
            odco.Connection = odc;
            odc.Open();

            try { s = odco.ExecuteScalar().ToString(); }
            catch { s = null; }

            odc.Close();

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string SmallDateTime(DateTime dt)
        {
            // return an OleDb friendly string of a smalldatetime value
            string s;

            //s = "'" + dt.ToString("dd/MM/yyyy HH:mm:ss") + "'";
            //s = dt.ToString("dd/MM/yyyy HH:mm:ss");
            s = dt.ToString("MM/dd/yyyy HH:mm:ss");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static int Update(DataTable in_dt, string table_name, string select_command, string primary_key, string[] in_field, string[] field, F[] field_rule, bool synch, string delete_rule, out string result)
        {
            bool identical;
            int op, c, count, count_in, count_delete;
            F rule;
            string command;

            //string temp = "", temp_dt_str, temp_in_dt_str; // TEMP

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
            OleDbDataAdapter odda;
            OleDbConnection odc;
            OleDbCommandBuilder odcb;

            op = 0;
            c = count = count_in = count_delete = 0;

            ds = new DataSet("ia");
            odc = new OleDbConnection(ConfigurationManager.ConnectionStrings["OledbConnectionString"].ConnectionString);

            //sc = new SqlConnection(path);

            odc.Open();
            command = select_command;
            odda = new OleDbDataAdapter();
            odda.SelectCommand = new OleDbCommand(command, odc);
            odda.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            odcb = new OleDbCommandBuilder(odda);

            result = "";

            //temp_in_dt_str = temp_dt_str = "";

            dt = null;

            try
            {
                odda.Fill(ds, table_name);

                dt = ds.Tables[0];

                if (in_dt != null)
                {
                    count_in = in_dt.Rows.Count;

                    // TEMP
                    //foreach (DataRow r in in_dt.Rows)
                    //{
                    //   temp_in_dt_str += "\n";
                    //   foreach (DataColumn c2 in in_dt.Columns) temp_in_dt_str += ":" + r[c2].ToString();
                    //}

                    if (dt != null)
                    {
                        count = dt.Rows.Count;

                        // TEMP
                        //foreach (DataRow r in dt.Rows)
                        //{
                        //   temp_dt_str += "\n";
                        //   foreach (DataColumn c2 in dt.Columns) temp_dt_str += ":" + r[c2].ToString();
                        //}

                        if (in_dt.Rows.Count > 0)
                        {
                            //if (dt.Rows.Count > 0)
                            //{
                            if (synch)
                            {
                                // compair two lists to find records in in_dt that are not in dt
                                foreach (DataRow r in dt.Rows) al.Add(r[primary_key].ToString());
                                foreach (DataRow r in in_dt.Rows) from_al.Add(r[primary_key].ToString());

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

                                        if (delete_rule == "all")
                                        {
                                            dr.Delete();
                                        }
                                        else if (delete_rule == "keep primary key")
                                        {
                                            // this will delete everything but keep only the primary key

                                            for (int n = 0; n < in_field.Length; n++)
                                            {
                                                if (field[n].ToString() != primary_key)
                                                {
                                                    rule = field_rule[n];

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
                                                        dr[field[n].ToString()] = global::Ia.Cl.Model.Db.OleDb.SmallDateTime(DateTime.UtcNow.AddHours(3));
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
                                // collect relevent values:

                                //if (in_dr[primary_key].ToString() == "95126013") op++;

                                dr = dt.Rows.Find(in_dr[primary_key].ToString());

                                if (dr != null)
                                {
                                    // check if rows are identical

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

                                            // if in_sdt lays within 1 minute of sdt they are identical

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
                                        // rows are the exact same. Do nothing
                                    }
                                    else
                                    {
                                        // row was updated
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
                                            else if (rule == F.Sdt)
                                            {
                                                in_sdt = DateTime.Parse(in_dr[in_field[n].ToString()].ToString());
                                                dr[field[n].ToString()] = global::Ia.Cl.Model.Db.OleDb.SmallDateTime(in_sdt);
                                            }
                                            else if (rule == F.Up)
                                            {
                                                dr[field[n].ToString()] = global::Ia.Cl.Model.Db.OleDb.SmallDateTime(DateTime.UtcNow.AddHours(3));
                                            }
                                        }

                                        c++;
                                    }
                                }
                                else
                                {
                                    // row does not exists, we will add it to database

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
                                        else if (rule == F.Sdt)
                                        {
                                            in_sdt = DateTime.Parse(in_dr[in_field[n].ToString()].ToString());
                                            dr[field[n].ToString()] = global::Ia.Cl.Model.Db.OleDb.SmallDateTime(in_sdt);
                                        }
                                        else if (rule == F.Cr || rule == F.Up)
                                        {
                                            dr[field[n].ToString()] = global::Ia.Cl.Model.Db.OleDb.SmallDateTime(DateTime.UtcNow.AddHours(3));
                                        }
                                    }

                                    // TEMP
                                    //temp = "";
                                    //foreach (DataColumn dc in dr.Table.Columns) temp += "|" + dr[dc.ColumnName];

                                    dt.Rows.Add(dr);
                                    c++;
                                }
                            }

                            //odcb.GetUpdateCommand();

                            //odcb.GetDeleteCommand();

                            odda.Update(ds, table_name);
                            odc.Close();

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
            }
            catch (Exception ex)
            {
#if DEBUG
                result = "global::Ia.Cl.Model.Db.OleDb.Update(): " + ex.ToString();
#else
                result = "global::Ia.Cl.Model.Db.OleDb.Update(): " + ex.Message;
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
        ////////////////////////////////////////////////////////////////////////////
    }
}
