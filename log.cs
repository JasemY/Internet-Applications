using System;
using System.Web;
using System.Xml;
using System.IO;
using System.Configuration;
using System.Text;

namespace Ia.Cl.Model
{
    /// <summary publish="true">
    /// Log file support class.
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
    /// </summary>
    public class Log
    {
        public Log() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Open()
        {
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Close()
        {
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Append(string file_path, string first_line, string line)
        {
            // 
            string path;
            StreamWriter sw = null;

            path = global::Ia.Cl.Model.Default.AbsolutePath();

            path = path + file_path;

            try
            {
                if (!System.IO.File.Exists(path))
                {
                    using (sw = System.IO.File.CreateText(path)) sw.WriteLine(first_line);
                }

                using (sw = System.IO.File.AppendText(path)) sw.WriteLine(line);
            }
            catch (Exception) { }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Append(string file_path, string line)
        {
            // 
            string path;
            StreamWriter sw = null;

            path = global::Ia.Cl.Model.Default.AbsolutePath();

            path = path + file_path;

            try
            {
                if (!System.IO.File.Exists(path))
                {
                    using (sw = System.IO.File.CreateText(path)) sw.WriteLine(line);
                }
                else
                {
                    using (sw = System.IO.File.AppendText(path)) sw.WriteLine(line);
                }
            }
            catch (Exception) { }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Read(string file_path)
        {
            string path, sa;
            StreamReader sr = null;
            StringBuilder sb = new StringBuilder(110000);

            path = global::Ia.Cl.Model.Default.AbsolutePath();

            path = path + file_path;

            try
            {
                if (System.IO.File.Exists(path))
                {
                    using (sr = System.IO.File.OpenText(path))
                    {
                        while ((sa = sr.ReadLine()) != null) sb.Append(sa + "\n");
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return sb.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Log a standard logging entry into a special database table
        /// </summary>
        public long Log2(int TypeId, string user_id, string refe, long reference_log_id, int direction_id, int system_id, int process_id, int function_id, string detail, DateTime created)
        {
            long l;
            string sql, s;

            // See table ia_log and log.xml

            /*
CREATE TABLE [dbo].[ia_log]
(
 [id]					int	IDENTITY(1,1) CONSTRAINT [ia_log_id_pk] PRIMARY KEY,
 [TypeId]				tinyint NULL,
 [user_id]				uniqueidentifier NULL,
 [ref]					nvarchar(32) NULL,
 [ia_log_id]			int NULL,
 [direction_id]	tinyint NULL,
 [system_id]			smallint NULL,
 [process_id]			smallint NULL,
 [function_id]			smallint NULL,
 [detail]				ntext NULL,
 [created]				smalldatetime NULL
)
            */

            if (user_id == null) user_id = "NULL";
            else user_id = "'" + user_id + "'";

            if (reference_log_id == 0) s = "NULL";
            else s = reference_log_id.ToString();

            //sql = "INSERT INTO [ia_log] ([TypeId],[user_id],[ref],[ia_log_id],[direction_id],[system_id],[process_id],[function_id],[detail],[created]) VALUES (" + TypeId + "," + user_id + ",'" + refe + "'," + s + "," + direction_id + "," + system_id + "," + process_id + "," + function_id + ",'" + HttpUtility.HtmlEncode(detail) + "','" + SmallDateTime(created) + "');SELECT SCOPE_IDENTITY()";

            //Sql(sql);

            //s = Scalar(sql);

            l = long.Parse(s);

            return l;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
