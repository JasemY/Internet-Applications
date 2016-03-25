using System;
using System.Web;
using System.Xml;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml.Linq;
using System.Deployment;

namespace Ia.Cl.Model.Db
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// XML Database support class. This handles storing and retrieving an XDocument, DataSet and Text storage.
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

    /*
     *  Sample
     *  ////////////////////////////////////////////////////////////////////////////
     *  
     * /// <summary>
       ///
       /// </summary>
       public static void StoreContent(string name, string content)
       {
           global::Ia.Cl.Model.Db.Xml n = new global::Ia.Cl.Model.Db.Xml(name);

           n.Text = content;

           n.SaveText();
       }

       ////////////////////////////////////////////////////////////////////////////

       /// <summary>
       ///
       /// </summary>
       public static string ReadContent(string name)
       {
           global::Ia.Cl.Model.Db.Xml n = new global::Ia.Cl.Model.Db.Xml(name);

           return n.Text;
       }
    */

    public class Xml
    {
        private string fileName, filePath;
        private DataSet ds;
        private XDocument xd;
        private string text;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Xml()
        {
            fileName = "default";

            filePath = Path() + fileName;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Xml(string _fileName)
        {
            fileName = _fileName;

            filePath = Path() + fileName;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public XDocument XDocument
        {
            get
            {
                if (xd == null) ReadXDocument();

                return xd;
            }

            set
            {
                xd = value;

                SaveXDocument();
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public DataSet DataSet
        {
            get
            {
                if (ds == null) ReadDataSet();

                return ds;
            }

            set
            {
                ds = value;

                SaveDataSet();
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Text
        {
            get
            {
                if (text == null) ReadText();

                return text;
            }

            set
            {
                text = value;

                SaveText();
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool ReadXDocument()
        {
            bool b;
            string r;

            xd = null;

            try
            {
                if (File.Exists(filePath + ".xdocument.xml"))
                {
                    xd = XDocument.Load(filePath + ".xdocument.xml");
                }
                else
                {
                    xd = new XDocument();

                    xd.Save(filePath + ".xdocument.xml");
                }

                b = true;
            }
            catch (Exception ex)
            {
                b = false;
#if DEBUG
                r = "Error: " + ex.ToString();
#else
                r = "Error: " + ex.Message;
#endif
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
        public bool ReadDataSet()
        {
            bool b;
            string r;

            try
            {
                if (File.Exists(filePath + ".dataset.xml"))
                {
                    ds.ReadXml(filePath + ".dataset.xml", XmlReadMode.ReadSchema);
                }
                else
                {
                    ds = new DataSet(fileName);

                    ds.WriteXml(filePath + ".dataset.xml", XmlWriteMode.WriteSchema);
                }

                b = true;
            }
            catch (Exception ex)
            {
                b = false;
#if DEBUG
                r = "Error: " + ex.ToString();
#else
                r = "Error: " + ex.Message;
#endif
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
        public bool ReadText()
        {
            bool b;
            string r;

            text = null;

            try
            {
                if (File.Exists(filePath + ".text.xml"))
                {
                    text = System.IO.File.ReadAllText(filePath + ".text.xml");
                }
                else
                {
                    System.IO.File.WriteAllText(filePath + ".text.xml", text);
                }

                b = true;
            }
            catch (Exception ex)
            {
                b = false;
#if DEBUG
                r = "Error: " + ex.ToString();
#else
                r = "Error: " + ex.Message;
#endif
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
        public bool SaveXDocument()
        {
            bool b;
            string r;

            try
            {
                xd.Save(filePath + ".xdocument.xml");

                b = true;
            }
            catch (Exception ex)
            {
                b = false;
#if DEBUG
                r = "Error: " + ex.ToString();
#else
                r = "Error: " + ex.Message;
#endif
            }
            finally
            {
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool SaveDataSet()
        {
            bool b;
            string r;

            try
            {
                ds.WriteXml(filePath + ".dataset.xml", XmlWriteMode.WriteSchema);

                b = true;
            }
            catch (Exception ex)
            {
                b = false;
#if DEBUG
                r = "Error: " + ex.ToString();
#else
                r = "Error: " + ex.Message;
#endif
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
        public bool SaveText()
        {
            bool b;
            string r;

            try
            {
                System.IO.File.WriteAllText(filePath + ".text.xml", text);

                b = true;
            }
            catch (Exception ex)
            {
                b = false;
#if DEBUG
                r = "Error: " + ex.ToString();
#else
                r = "Error: " + ex.Message;
#endif
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
        private string DateTime(DateTime dt)
        {
            // return a friendly string of a datetime value
            string s;

            s = dt.ToString("yyyy-MM-dd HH:mm:ss");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private string Path()
        {
            string path;

#if WINDOWS_FORM
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) path = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DataDirectory + @"\";
            else path = AppDomain.CurrentDomain.BaseDirectory;
#else
            path = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"app_data\";
#endif

            return path;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}

