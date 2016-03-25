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

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// German cities and states. 
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
    public class Germany
    {
        private static XDocument xd;

        // <area id="5" name="Bneid Al Gar" arabicName="بنيد القار" latitude="29.3730" longitude="48.0047" zoom="14"/>

        /// <summary/>
        public int Id { get; set; }
        /// <summary/>
        public string Name { get; set; }
        /// <summary/>
        public string ArabicName { get; set; }
        /// <summary/>
        public string Latitude { get; set; }
        /// <summary/>
        public string Longitude { get; set; }
        /// <summary/>
        public int Zoom { get; set; }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public Germany() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public Germany(int itu)
        {
            Germany country;

            country = GermanyById(itu);

            this.Id = country.Id;
            this.Name = country.Name;
            this.ArabicName = country.ArabicName;
            this.Latitude = country.Latitude;
            this.Longitude = country.Longitude;
            this.Zoom = country.Zoom;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Germany GermanyById(int id)
        {
            Germany kuwaitArea;

            kuwaitArea = (from q in XDocument.Elements("countryList").Elements("country")
                          where q.Attribute("itu").Value == id.ToString()
                          select new Germany
                          {
                              Id = id,
                              Name = q.Attribute("name").Value,
                              ArabicName = q.Attribute("arabicName").Value,
                              Latitude = q.Attribute("latitude").Value,
                              Longitude = q.Attribute("longitude").Value,
                              Zoom = int.Parse(q.Attribute("zoom").Value)
                          }
            ).First<Germany>();

            return kuwaitArea;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static SortedList GermanySortedList
        {
            get
            {
                int id;
                string name;
                SortedList sl;

                sl = new SortedList(300);

                foreach (XElement xe in XDocument.Elements("kuwait").Elements("province").Elements("city").Elements("area"))
                {
                    try
                    {
                        id = int.Parse(xe.Parent.Parent.Attribute("id").Value.PadLeft(2, '0') + xe.Parent.Attribute("id").Value.PadLeft(2, '0') + xe.Attribute("id").Value.PadLeft(2, '0'));
                        name = xe.Attribute("name").Value;
                    }
                    catch (Exception)
                    {
                        id = 0;
                        name = null;
                    }

                    sl[id] = name;
                }

                return sl;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<Germany> GermanyList
        {
            get
            {
                List<Germany> kuwaitAreaList;

                kuwaitAreaList = (from q in XDocument.Elements("kuwait").Elements("province").Elements("city").Elements("area")
                                  select new Germany
                                  {
                                      Id = int.Parse(q.Parent.Parent.Attribute("id").Value.PadLeft(2, '0') + q.Parent.Attribute("id").Value.PadLeft(2, '0') + q.Attribute("id").Value.PadLeft(2, '0')),
                                      Name = q.Attribute("name").Value,
                                      ArabicName = q.Attribute("arabicName").Value,
                                      Latitude = (q.Attribute("latitude") != null) ? q.Attribute("latitude").Value : "",
                                      Longitude = (q.Attribute("longitude") != null) ? q.Attribute("longitude").Value : "",
                                      Zoom = (q.Attribute("zoom") != null) ? int.Parse(q.Attribute("zoom").Value) : 0
                                  }
                ).ToList<Germany>();

                return kuwaitAreaList;
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

        public static XDocument XDocument
        {
            get
            {
                Assembly _assembly;
                StreamReader streamReader;

                xd = null;
                _assembly = Assembly.GetExecutingAssembly();
                streamReader = new StreamReader(_assembly.GetManifestResourceStream("Ia.Cl.model.country.germany.xml"));

                try
                {
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
}
