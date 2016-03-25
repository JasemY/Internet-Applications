using System;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Configuration;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Globalization;

namespace Ia.Cl.Model.Data
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 
    /// </summary>
    public class CodeLibrary
    {
        private static XDocument xd;
        private static List<CodeLibrary> codeLibraryList;

        public string FileName { get; set; }
        /// <summary/>
        public string Namespace { get; set; }
        /// <summary/>
        public string ClassName { get; set; }
        /// <summary/>
        public string Summary { get; set; }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public CodeLibrary() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public static List<CodeLibrary> List
        {
            get
            {
                if (codeLibraryList == null || codeLibraryList.Count == 0)
                {
                    CodeLibrary codeLibrary;

                    codeLibraryList = new List<CodeLibrary>();

                    foreach (XElement xe in XDocument.Element("codeLibrary").Elements("codeFile"))
                    {
                        codeLibrary = new CodeLibrary();

                        codeLibrary.FileName = xe.Attribute("fileName").Value;
                        codeLibrary.Namespace = xe.Attribute("namespace").Value;
                        codeLibrary.Summary = xe.Attribute("summary").Value;
                        codeLibrary.ClassName = xe.Attribute("className").Value;

                        codeLibraryList.Add(codeLibrary);
                    }
                }

                return codeLibraryList;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// How to embed and access resources by using Visual C# http://support.microsoft.com/kb/319292/en-us
        /// 
        /// 1. Change the "Build Action" property of your XML file from "Content" to "Embedded Resource".
        /// 2. Add "using System.Reflection".
        /// 3. See sample below.
        /// 
        /// </summary>

        private static XDocument XDocument
        {
            get
            {
                Assembly _assembly;
                StreamReader streamReader;

                try
                {
                    xd = null;
                    _assembly = Assembly.GetExecutingAssembly();
                    streamReader = new StreamReader(_assembly.GetManifestResourceStream("Ia.Cl.model.data.code-library.xml")); 

                    if (streamReader.Peek() != -1)
                    {
                        xd = System.Xml.Linq.XDocument.Load(streamReader);
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                }

                return xd;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}
