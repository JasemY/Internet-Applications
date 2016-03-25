using System;
using System.Xml;
using System.IO;
using System.Web;
using System.Web.UI;

namespace Ia.Cl.Model.AspNetState
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// ASP.NET state support functions: Applications, Session, Coockies, Control.
    /// </summary>
    /// <value>
    /// https://msdn.microsoft.com/en-us/library/z1hkazw7(v=vs.100).aspx
    /// </value>
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
    public class View
    {
        public View() { }

        public static void Write(StateBag viewState, string name, object value)
        {
            viewState[name] = value;
        }

        public static object Read(StateBag viewState, string name)
        {
            return viewState[name];
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////






    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class Control
    {
        public Control() { }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////






    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class Cookie
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Cookie() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Create(string name, string value)
        {
            Create(name, value, 9999);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Create(string name, string value, int lifeInDays)
        {
            // this appends a cookie to the collection

            HttpCookie cookie = new HttpCookie(name, value);

            cookie.Expires = DateTime.UtcNow.AddHours(3 + 24 * lifeInDays);

            HttpContext.Current.Response.AppendCookie(cookie);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Read(string name)
        {
            // this reads the value of the cookie
            string value;
            HttpCookie cookie;

            cookie = HttpContext.Current.Request.Cookies[name];

            if (cookie == null) value = null;
            else value = cookie.Value;

            return value;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Exists(string name)
        {
            // 

            return (Read(name) != null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Update(string name, string value)
        {
            int lifeInDays;
            HttpCookie cookie;

            if (HttpContext.Current.Request.Cookies[name] != null)
            {
                HttpContext.Current.Response.Cookies[name].Value = value;
            }
            else
            {
                lifeInDays = 30;
                cookie = new HttpCookie(name);

                cookie.Expires = DateTime.UtcNow.AddHours(3 + 24 * lifeInDays);

                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////








        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public class Application
        {
            public Application() { }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////






        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public class Session
        {
            public Session() { }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

    }
}
