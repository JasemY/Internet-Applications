using System;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Trace function to try to identifiy a user using IP addresses, cookies, and session states.
    /// </summary>
    /// <value>
    /// Put Ia.Cl.Model.Trace.Inspect(this.Request); in Session_Start()
    /// Or use a service reference
    /// </value>
    /// 
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
    public partial class Trace
    {
        private const string traceCookieName = "traceCookie";

        /// <summary/>
        public Trace() { }

        /// <summary/>
        public int Id { get; set; }
        /// <summary/>
        public string Ip { get; set; }
        /// <summary/>
        public string Host { get; set; }
        /// <summary/>
        public string ServerVariables { get; set; }
        /// <summary/>
        public System.Guid Guid { get; set; }
        /// <summary/>
        public DateTime Created { get; set; }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Create(Trace newItem, out string result)
        {
            bool b;

            b = false;
            result = "";

            using (var db = new global::Ia.Cl.Model.Ia())
            {
                db.Traces.Add(newItem);
                db.SaveChanges();

                b = true;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Inspect(System.Web.HttpRequest request)
        {
            Guid guid;

            // read trace guid from cookie value (with ""). If does not exists (or is invalid) create a new one (guid & cookie).
            //if (!Guid.TryParse(Cookie.Read(traceCookieName), out guid))
            //{
            guid = Guid.NewGuid();
            //   Cookie.Create(traceCookieName, guid.ToString());
            //}

            guid = Guid.NewGuid();

            Inspect(request.UserHostAddress, request.Url, guid, FormatServerVariables(request));
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initiates a trace by passing the API Trace function
        /// </summary>
        public static void Inspect(System.Web.HttpRequest request, Func<string, Uri, Guid, string, int> ApiTrace)
        {
            Guid guid;

            // read trace guid from cookie value (with ""). If does not exists (or is invalid) create a new one (guid & cookie).
            //if (!Guid.TryParse(Cookie.Read(traceCookieName), out guid))
            //{
            guid = Guid.NewGuid();
            //   Cookie.Create(traceCookieName, guid.ToString());
            //}

            ApiTrace(request.UserHostAddress, request.Url, guid, FormatServerVariables(request));
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Inspect(string userHostAddress, Uri url, Guid guid, string serverVariables)
        {
            bool newItemCreated;
            string result;
            Trace newItem;

            newItem = new Trace();

            // insert new record
            newItem.Ip = userHostAddress;
            newItem.Host = global::Ia.Cl.Model.Default.ReadDomainFromUri(url);
            newItem.Guid = guid;
            newItem.ServerVariables = serverVariables;
            newItem.Created = DateTime.UtcNow.AddHours(3);

            newItemCreated = Trace.Create(newItem, out result);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Read traced records
        /// </summary>
        public static List<Trace> Read()
        {
            List<Trace> list;

            using (var db = new global::Ia.Cl.Model.Ia())
            {
                list = (from q in db.Traces orderby q.Created descending select q).Take(100).ToList<Trace>();
            }

            return list;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string FormatServerVariables(System.Web.HttpRequest request)
        {
            string serverVariables = null;

            try
            {
                serverVariables = "";

                foreach (string key in request.ServerVariables.AllKeys)
                {
                    serverVariables += "[" + key + ": " + request.ServerVariables[key] + "]\r\n";
                }
            }
            catch (Exception)
            {
#if DEBUG
                //line += "Error: " + ex.ToString();
#else
                //line += "Error: " + ex.Message;
#endif
            }
            finally { }

            return serverVariables;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
