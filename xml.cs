using System;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using System.Text;
using System.Data;
using System.Linq;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// XML support class.
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
    public class Xml
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Xml() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public XmlNode ReturnXmlNode(string file)
        {
            return ReturnXmlNode("/", file);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public XmlNode ReturnXmlNode(string node_str, string file)
        {
            string path;
            XmlNode n, m = null;
            XmlTextReader r = null;
            XmlDocument d = new XmlDocument();

            path = Cl.Model.Default.AbsolutePath();

            path = path + file;

            try
            {
                r = new XmlTextReader(path);
                //r.WhitespaceHandling = WhitespaceHandling.Significant;
                d.Load(r);
                n = d.DocumentElement;
                m = n.SelectSingleNode(node_str);
            }
            catch (Exception)
            {
#if DEBUG
                //line += "Error: " + ex.ToString();
#else
                //line += "Error: " + ex.Message;
#endif
            }
            finally
            {
                if (r != null) r.Close();
            }

            return m;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void WriteXmlNode(XmlNode xn, string file)
        {
            string path;
            //XmlDocument d = new XmlDocument();
            //XmlTextWriter w = null;

            //xn.WriteContentTo(w);

            try
            {
                //r = new XmlTextReader(HttpContext.Current.Server.MapPath(file));
                //w.w.WhitespaceHandling = WhitespaceHandling.None;
                //n = d.DocumentElement;

                path = global::Ia.Cl.Model.Default.AbsolutePath();

                path = path + file;

                string xmlContents = xn.InnerXml;
                System.IO.StreamWriter writer = new System.IO.StreamWriter(path, false, System.Text.Encoding.UTF8);
                writer.Write(xmlContents);
                writer.Close();

                //       d.DocumentElement = xn.InnerXml;
                //       FileStream fsxml = new FileStream(path,FileMode.Truncate,FileAccess.Write,FileShare.ReadWrite);

                //       d.Save(fsxml);

                //m = n.SelectSingleNode(node_str);

                //if (w!=null) w.Close();
            }
            catch (Exception)
            {
#if DEBUG
                //line += "Error: " + ex.ToString();
#else
                //line += "Error: " + ex.Message;
#endif
            }
            finally
            {
                //if (w!=null) w.Close();
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public XmlNode Literal(string s)
        {
            XmlNode xn = null;
            XmlDocument xd = new XmlDocument();

            try
            {
                xd.LoadXml(s);
                xn = xd.DocumentElement;
            }
            catch (Exception)
            {
#if DEBUG
                //line += "Error: " + ex.ToString();
#else
                //line += "Error: " + ex.Message;
#endif
            }

            return xn;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public DataSet Read(string file)
        {
            // read and return the contents of the XML file as a DataSet
            string path;
            XmlTextReader xtr = null;
            DataSet ds = new DataSet();

            path = global::Ia.Cl.Model.Default.AbsolutePath();

            path = path + file;

            try
            {
                xtr = new XmlTextReader(path);
                ds.ReadXml(xtr, XmlReadMode.ReadSchema);

                if (xtr != null) xtr.Close();
            }
            catch (Exception)
            {
#if DEBUG
                //line += "Error: " + ex.ToString();
#else
                //line += "Error: " + ex.Message;
#endif
                ds = null;
            }
            finally
            {
                if (xtr != null) xtr.Close();
            }

            return ds;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Write(DataSet ds, string file)
        {
            string path;
            XmlTextWriter xtw = null;

            try
            {
                path = global::Ia.Cl.Model.Default.AbsolutePath();

                path = path + file;

                xtw = new XmlTextWriter(path, Encoding.UTF8);

                ds.WriteXml(xtw, XmlWriteMode.WriteSchema);

                if (xtw != null) xtw.Close();
            }
            catch (Exception)
            {
#if DEBUG
                //line += "Error: " + ex.ToString();
#else
                //line += "Error: " + ex.Message;
#endif
            }
            finally
            {
                if (xtw != null) xtw.Close();
            }
        }

        /*
            ////////////////////////////////////////////////////////////////////////////
    
            /// <summary>
            ///
            /// </summary>
            public void Initialize_Data()
            {
              // Read XML document
              xmltr = null;

              try
              {
                xmltr = new XmlTextReader(HttpContext.Current.Server.MapPath("..\\data\\data.xml")); // ????
                xmltr.WhitespaceHandling = WhitespaceHandling.None;
                xmld = new XmlDocument();
                xmld.Load(xmltr);
          
                // this part will set special id elements for the XML document. The id of a node is thea concatenation
                // of ids of ancestors. This will make the handling of XML data simpler. Note that this does not apply
                // for a document that does not have an id attribute.
      
                // Note that this assumes any id value in the XML file will not exceed 999 (three digits)
      
                XmlNode m,n = xmld.DocumentElement;
                XmlNodeList ance,list=n.SelectNodes("//*");
        
                if(list.Count > 0)
                {
                  // 
                  foreach(XmlNode ni in list)
                  {
                    if(ni.Name.ToString() == "Stock") //Page")
                    {
                      // add the path_id attribute to node of type "Page" only:
                      m = xmld.CreateNode(XmlNodeType.Attribute,"path_id",ni.NamespaceURI);
                      ni.Attributes.SetNamedItem(m);

                      // 
                      foreach(XmlAttribute l in ni.Attributes)
                      {
                        if(l.Name.ToString() == "id")
                        {
                          // if this is the id attribute add the id of the parents as a prefix to node path_id
                          ance=ni.SelectNodes("ancestor::*");
                          string id="";
                  
                          // l.Value = l.Value.PadLeft(3,'0');
                          ni.Attributes["path_id"].Value = l.Value.PadLeft(3,'0');
               
                          foreach(XmlNode ce in ance)
                          {
                            try
                            {
                              if(ce.Attributes.Count>0) id = ce.Attributes["id"].Value + id;
                            }
                            catch(Exception ex)
                            {
        #if DEBUG
                              //result_l.Text += "Error: " + ex.ToString();
        #else
                                //result_l.Text += "Error: " + ex.Message;
        #endif
                            }
                    
                            // replace ce.Attributes.Count with something like ce.Attributes.Contains("id")
                          }

                          // this has a very important effect; it removes any leading zeros and makes the number padded but int like
                          // and will be consistant with Javascript and the database tables:
                          ni.Attributes["path_id"].Value = (long.Parse(id + ni.Attributes["path_id"].Value)).ToString();
                        }
                
                      }
                    }
                  }
                }
          


              }
              catch (Exception ex)
              {
        #if DEBUG
                int i=0;
                i++;//result_l.Text += "Error: " + ex.ToString();
        #else
                  //result_l.Text += "Error: " + ex.Message;
        #endif
              }    
              finally
              {
                if (xmltr!=null) xmltr.Close();
              }

            }

            ////////////////////////////////////////////////////////////////////////////
    
            /// <summary>
            /// Returns tables from the XML document using the XPath. This will make the handling of XML data simpler.
            /// </summary>
            public DataTable Xmld(string xpath)
            {
              // Note that the id of elements from the XML document are special. The id of a node is a concatenation
              // of ids of ancestors. This will make the handling of XML data simpler. Note that this does not apply
              // for a document that does not have an id attribute.
      
              // Note that this assumes any id value in the XML file will not exceed 999 (three digits)
      
              DataTable dt = new DataTable();
              DataRow dr;

              Initialize_Data();
        
              try
              {
                XmlNode n = xmld.DocumentElement;
                XmlNodeList list=n.SelectNodes(xpath);
        
                if(list.Count > 0)
                {
                  // read the names of attributes and create datatable rows for them
                  foreach(XmlAttribute l in list[0].Attributes)
                  {
                    dt.Columns.Add(new DataColumn(l.Name,typeof(string)));
                  }
          
                  // now fill the newly created table with values from the XML
                  foreach(XmlNode ni in list)
                  {
                    dr = dt.NewRow();
                    foreach(XmlAttribute a in ni.Attributes) dr[a.Name] = a.Value;
                    dt.Rows.Add(dr);
                  }
          
                  DataColumn[] keys = new DataColumn[1];
                  keys[0] = dt.Columns["id"];
                  dt.PrimaryKey = keys;
                }
              }
              catch (Exception ex)
              {
        #if DEBUG
                //result_l.Text += "Error: " + ex.ToString();
        #else
                //result_l.Text += "Error: " + ex.Message;
        #endif
              }    

              return dt;
            }

         * 
         
         
             ////////////////////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Returns tables from the XML document using the XPath. This will make the handling of XML data simpler.
    /// </summary>
    public DataTable Xmld(string xpath)
    {
      // Note that the id of elements from the XML document are special. The id of a node is a concatenation
      // of ids of ancestors. This will make the handling of XML data simpler. Note that this does not apply
      // for a document that does not have an id attribute.
      
      // Note that this assumes any id value in the XML file will not exceed 999 (three digits)
      
      DataTable dt = new DataTable();
      DataRow dr;

      Initialize_Data();
        
      try
      {
        XmlNode n = xmld.DocumentElement;
        XmlNodeList list=n.SelectNodes(xpath);
        
        if(list.Count > 0)
        {
          // read the names of attributes and create datatable rows for them
          foreach(XmlAttribute l in list[0].Attributes)
          {
            dt.Columns.Add(new DataColumn(l.Name,typeof(string)));
          }
          
          // now fill the newly created table with values from the XML
          foreach(XmlNode ni in list)
          {
            dr = dt.NewRow();
            foreach(XmlAttribute a in ni.Attributes) dr[a.Name] = a.Value;
            dt.Rows.Add(dr);
          }
          
          DataColumn[] keys = new DataColumn[1];
          keys[0] = dt.Columns["id"];
          dt.PrimaryKey = keys;
        }
      }
      catch (Exception ex)
      {
#if DEBUG
        //error_l.Text += "Error: " + ex.ToString();
#else
        //error_l.Text += "Error: " + ex.Message;
#endif
      }    

      return dt;
    }

*/

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public XmlNode Xml_Xslt(string file_xml, string file_xsl)
        {
            // Read and XML and XSLT transformation and return a root node to the result
            string path;
            StringBuilder sb;
            XmlNode xn = null;
            XslCompiledTransform xct;
            XPathDocument xpd;
            XPathNavigator xpn;
            XmlDocument xd;
            StringWriter sw = null;

            sb = new StringBuilder(10000);
            sb.Length = 0;

            path = global::Ia.Cl.Model.Default.AbsolutePath();

            // load the XML document
            xpd = new XPathDocument(path + file_xml);
            xpn = xpd.CreateNavigator();
            xct = new XslCompiledTransform();

            sw = new StringWriter(sb);

            xct.Load(path + file_xsl, XsltSettings.TrustedXslt, null);

            xct.Transform(xpn, null, sw);

            xd = new XmlDocument();

            sw.Close();

            try
            {
                xd.LoadXml(sb.ToString());
                xn = xd.DocumentElement;
                xn = xn.SelectSingleNode("/");
            }
            catch (Exception)
            {
#if DEBUG
                //line += "Error: " + ex.ToString();
#else
                //line += "Error: " + ex.Message;
#endif
            }
            finally
            {
            }

            return xn;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static XDocument Load(string filePath)
        {
            return XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + filePath);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert XmlDocument to XDocument
        /// </summary>
        public static XDocument DocumentToXDocumentReader(XmlDocument doc)
        {
            return XDocument.Load(new XmlNodeReader(doc));
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static XDocument Load(string filePath, string fileXslt)
        {
            //return XDocument.Load(global::Ia.Cl.Model.Default.Absolute_Path() + filePath);

            // Read and XML and XSLT transformation and return a root node to the result
            string path, r;
            StringBuilder sb;
            XslCompiledTransform xct;
            XPathDocument xpd;
            XPathNavigator xpn;
            XDocument xd;
            StringWriter sw = null;

            sb = new StringBuilder(10000);
            sb.Length = 0;

            xd = null;

            path = global::Ia.Cl.Model.Default.AbsolutePath();

            // load the XML document
            xpd = new XPathDocument(path + filePath);
            xpn = xpd.CreateNavigator();
            xct = new XslCompiledTransform();

            sw = new StringWriter(sb);

            xct.Load(path + fileXslt, XsltSettings.TrustedXslt, null);

            xct.Transform(xpn, null, sw);

            sw.Close();

            try
            {
                xd = XDocument.Parse(sb.ToString());
            }
            catch (Exception ex)
            {
#if DEBUG
                r = "Error: " + ex.ToString();
#else
                r = "Error: " + ex.Message;
#endif
            }
            finally
            {
            }

            return xd;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
