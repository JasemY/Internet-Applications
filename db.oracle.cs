using System;
using System.Web;
using System.Xml;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Types;
using Oracle.DataAccess.Client;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ia.Cl.Model.Db
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Oracle support class.
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
    public class Oracle
    {
        private static OracleConnection oracleConnection;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Oracle()
        {
            oracleConnection = new OracleConnection();

            oracleConnection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            oracleConnection.Open();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Oracle(string connectionString)
        {
            oracleConnection = new OracleConnection();

            oracleConnection.ConnectionString = connectionString;
            oracleConnection.Open();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        ~Oracle()
        {
            oracleConnection.Close();
            oracleConnection.Dispose();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool Sql(string sql)
        {
            // 
            bool b;
            OracleCommand oco;

            oco = null;               

            try
            {
                oco = new OracleCommand();

                oco.Connection = oracleConnection;
                oco.CommandText = sql;
                oco.CommandType = CommandType.Text;

                oco.ExecuteNonQuery();

                b = true;
            }
            catch (OracleException ex)
            {
                b = false;
            }
            catch (Exception ex)
            {
                b = false;
            }
            finally
            {
                oco.Dispose();
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public DataTable Select(string sql)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            OracleCommand oco;
            OracleDataReader dr;

            dt = null;
            oco = null;
            dr = null;

            try
            {
                dt = new DataTable();
                oco = new OracleCommand();

                oco.Connection = oracleConnection;
                oco.CommandText = sql;
                oco.CommandType = CommandType.Text;

                dr = oco.ExecuteReader();
                dt.Load(dr);
            }
            catch (OracleException ex)
            {
            }
            catch (Exception ex)
            {
            }
            finally
            {
                dr.Dispose();
                oco.Dispose();
            }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
