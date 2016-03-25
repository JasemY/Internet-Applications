using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Data;
using System.Globalization;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Web.Caching;
using System.Xml;
using System.Configuration;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Search Engine Optimization (SEO) support class.
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
    public abstract class Seo
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Seo() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Title()
        {
            string s;

            s = null;

            try
            {
                s = ConfigurationManager.AppSettings["siteName"].ToString() + ": " + ConfigurationManager.AppSettings["companyName"].ToString() + " - "
                    + ConfigurationManager.AppSettings["legalCountry"].ToString() + " " + SiteMap.CurrentNode.Title.ToString();
            }
            catch (NullReferenceException)
            {
                try
                {
                    s = ConfigurationManager.AppSettings["siteName"].ToString() + ": " + ConfigurationManager.AppSettings["companyName"].ToString() + " - "
                        + ConfigurationManager.AppSettings["legalCountry"].ToString();
                }
                catch (Exception)
                {
                }
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ArabicTitle()
        {
            string s;

            s = null;

            try
            {

                s = ConfigurationManager.AppSettings["arabicSiteName"].ToString() + ": " + ConfigurationManager.AppSettings["arabicCompanyName"].ToString() + " - "
                    + ConfigurationManager.AppSettings["arabicLegalCountry"].ToString() + " " + SiteMap.CurrentNode.Title.ToString();
            }
            catch (NullReferenceException)
            {
                try
                {
                    s = ConfigurationManager.AppSettings["arabicSiteName"].ToString() + ": " + ConfigurationManager.AppSettings["arabicCompanyName"].ToString() + " - "
                        + ConfigurationManager.AppSettings["arabicLegalCountry"].ToString();
                }
                catch (Exception)
                {
                }
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Link()
        {
            string s;

            s = "http://*";

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

    }
}
