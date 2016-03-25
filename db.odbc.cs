using System;
using System.Web;
using System.Xml;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Collections;
using System.Text;

namespace Ia.Cl.Model.Db
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// ODBC support class.
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
    public class Odbc
    {
        private string connectionString;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Odbc()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Odbc(string _connectionString)
        {
            connectionString = _connectionString;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Sql(string sql)
        {
            // execute an SQL command
            bool b = true;
            OdbcConnection sc = null;
            OdbcCommand sco;

            sc = new OdbcConnection(connectionString);
            sco = new OdbcCommand();

            sco.CommandType = CommandType.Text; // default
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
        public DataTable Select(string sql)
        {
            // return a DataTable of result rows

            OdbcConnection sc = null;
            OdbcCommand sco;

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            OdbcDataAdapter da = new OdbcDataAdapter();

            try
            {
                sc = new OdbcConnection(connectionString);
                sco = new OdbcCommand(sql, sc);

                if (sc.State == ConnectionState.Open) sc.Close();
                if (sco.Connection.State == ConnectionState.Open) sco.Connection.Close();

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
        ///
        /// </summary>
        public string Scalar(string sql)
        {
            // return a scaler
            string s;

            OdbcConnection sc = null;
            OdbcCommand sco;

            sc = new OdbcConnection(connectionString);
            sco = new OdbcCommand(sql, sc);

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
        public string SmallDateTime(DateTime dt)
        {
            // return an SQL friendly string of a smalldatetime value
            string s;

            s = dt.ToString("yyyy-MM-ddTHH:mm:ss");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
