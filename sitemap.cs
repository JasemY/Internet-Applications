using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Specialized;

public class DynamicSiteMapProvider : StaticSiteMapProvider
{
    private string _siteMapFileName;
    private SiteMapNode _rootNode = null;
    private const string SiteMapNodeName = "siteMapNode";
    private Ia.Cs.Xml xml;

    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Sitemap support class.
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

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public DynamicSiteMapProvider()
    {
        xml = new Ia.Cs.Xml();
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public override SiteMapNode RootNode
    {
        get { return BuildSiteMap(); }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public override void Initialize(string name, NameValueCollection attributes)
    {
        base.Initialize(name, attributes);
        _siteMapFileName = attributes["siteMapFile"];
        _siteMapFileName = _siteMapFileName.Replace("~/", "");
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    protected override SiteMapNode GetRootNodeCore()
    {
        return RootNode;
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    protected override void Clear()
    {
        lock (this)
        {
            _rootNode = null;
            base.Clear();
        }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public override SiteMapNode BuildSiteMap()
    {
        lock (this)
        {
            if (null == _rootNode)
            {
                Clear();

                // Load the sitemap's xml from the file.
                XmlDocument siteMapXml = LoadSiteMapXml();

                // Create the first site map item from the top node in the xml.
                XmlElement rootElement = (XmlElement)siteMapXml.GetElementsByTagName(SiteMapNodeName)[0];

                // Create an XmlNamespaceManager to resolve the default namespace.
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(siteMapXml.NameTable);
                nsmgr.AddNamespace("bk", "http://schemas.microsoft.com/AspNet/SiteMap-File-1.0");

                XmlNode ref_xn;
                //XmlElement root = doc.DocumentElement; siteMapNode title="About Us"
                ref_xn = rootElement.SelectSingleNode("descendant::bk:siteMapNode[@title='Books']", nsmgr);

                // This is the key method - add the dynamic nodes to the xml
                AddDynamicNodes(rootElement, ref_xn);

                // Now build up the site map structure from the xml
                GenerateSiteMapNodes(rootElement);
            }
        }

        return _rootNode;
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    private XmlDocument LoadSiteMapXml()
    {
        XmlDocument siteMapXml = new XmlDocument();

        siteMapXml.Load(AppDomain.CurrentDomain.BaseDirectory + _siteMapFileName);

        return siteMapXml;
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    private void AddDynamicNodes(XmlElement rootElement, XmlNode ref_xn)
    {
        // below: build site map from data in this.xml file
        string id, url, xpath;
        XmlNode xn;
        XmlElement xe;
        DataTable dt;

        xpath = "";
        xn = xml.ReturnXmlNode(@"app_data\this.xml");

        //rootElement.RemoveChild(rootElement.SelectSingleNode("siteMap/siteMapNode/siteMapNode[@title='Books']"));       

        // below: collect type_id that exist in book table
        dt = Ia.Cs.Db.OleDb.Select("SELECT DISTINCT type_id FROM ia_book");

        if (dt.Rows.Count > 0)
        {
            foreach (DataRow r in dt.Rows) xpath += "@id=" + r["type_id"].ToString() + " or ";

            if (xpath.Length > 0) xpath = xpath.Remove(xpath.Length - 4, 4);

            foreach (XmlNode n in xn.SelectNodes("/this/book/type[descendant-or-self::type[" + xpath + "]]"))
            {
                id = n.Attributes["id"].Value;

                if (n.Attributes["url"] != null) url = n.Attributes["url"].Value;
                else if (n.ParentNode.Attributes["url"] != null) url = n.ParentNode.Attributes["url"].Value;
                else if (n.ParentNode.ParentNode.Attributes["url"] != null) url = n.ParentNode.ParentNode.Attributes["url"].Value;
                else url = "";

                if (id != "0")
                {
                    xe = AddDynamicChildElement(rootElement, ref_xn, url + "?id=" + id, n.Attributes["name"].Value, ""/*n.Attributes["description"].Value*/);

                    foreach (XmlNode o in n.SelectNodes("type[" + xpath + "]"))
                    {
                        id = o.Attributes["id"].Value;

                        if (o.Attributes["url"] != null) url = o.Attributes["url"].Value;
                        else if (o.ParentNode.Attributes["url"] != null) url = o.ParentNode.Attributes["url"].Value;
                        else if (o.ParentNode.ParentNode.Attributes["url"] != null) url = o.ParentNode.ParentNode.Attributes["url"].Value;
                        else url = "";

                        AddDynamicChildElement(xe, url + "?id=" + id, o.Attributes["name"].Value, ""/*n.Attributes["description"].Value*/);
                    }
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    private static XmlElement AddDynamicChildElement(XmlElement parentElement, XmlNode ref_xn, string url, string title, string description)
    {
        // Create new element from the parameters

        XmlElement childElement = parentElement.OwnerDocument.CreateElement(SiteMapNodeName);

        childElement.SetAttribute("url", url);

        childElement.SetAttribute("title", title);

        childElement.SetAttribute("description", description);

        // Add it to the parent
        //parentElement.AppendChild(childElement);
        parentElement.InsertAfter(childElement, ref_xn);

        return childElement;
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    private static XmlElement AddDynamicChildElement(XmlElement parentElement, string url, string title, string description)
    {
        // Create new element from the parameters

        XmlElement childElement = parentElement.OwnerDocument.CreateElement(SiteMapNodeName);

        childElement.SetAttribute("url", url);

        childElement.SetAttribute("title", title);

        childElement.SetAttribute("description", description);

        // Add it to the parent
        parentElement.AppendChild(childElement);

        return childElement;
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    private void GenerateSiteMapNodes(XmlElement rootElement)
    {
        _rootNode = GetSiteMapNodeFromElement(rootElement);

        AddNode(_rootNode);

        CreateChildNodes(rootElement, _rootNode);
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    private void CreateChildNodes(XmlElement parentElement, SiteMapNode parentNode)
    {
        foreach (XmlNode xmlElement in parentElement.ChildNodes)
        {
            if (xmlElement.Name == SiteMapNodeName)
            {
                SiteMapNode childNode = GetSiteMapNodeFromElement((XmlElement)xmlElement);

                AddNode(childNode, parentNode);

                CreateChildNodes((XmlElement)xmlElement, childNode);
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    private SiteMapNode GetSiteMapNodeFromElement(XmlElement rootElement)
    {
        SiteMapNode newSiteMapNode;

        string url = rootElement.GetAttribute("url");

        string title = rootElement.GetAttribute("title");

        string description = rootElement.GetAttribute("description");

        // The key needs to be unique, so hash the url and title.

        newSiteMapNode = new SiteMapNode(this, (url + title).GetHashCode().ToString(), url, title, description);

        return newSiteMapNode;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}
