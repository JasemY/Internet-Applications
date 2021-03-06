﻿using System;
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
    /// Country geographic coordinates and standard UN naming conventions.
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
    public class Country
    {
        private static XDocument xd;

        // # FIPS 10-4 to ISO 3166-1 country codes
        // # Created by OpenGeoCode.Org, Submitted into the Public Domain June 4, 2014 (Version 5)
        // #
        // # Last FIPS 10-4 Revision, as Published by CIA World Factbook, Jan. 2014
        // # Metadata
        // # FIPS 10-4, 
        // # ISO 3166-1 alpha-2, 
        // # ISO 3166-1 alpha-3, 
        // # ISO 3166-1 numeric-3,
        // # Inclusive in Country 
        // #
        
        private static List<string> fipsIsoList = new List<string> { "AF", "AF", "AFB", "004", "AX", "", "", "", "GB","AL", "AL", "ALB", "008", "AG", "DZ", "DZA", "012", "AQ", "AS", "ASM", "016", "AN", "AD", "AND", "020", "AO", "AO", "AGO", "024", "AV", "AI", "AIA", "660", "AY", "AQ", "ATA", "010", "AC", "AG", "ATG", "028", "AR", "AR", "ARG", "032", "AM", "AM", "ARM", "051", "AA", "AW", "ABW", "533", "AT", "", "", "", "AU","AS", "AU", "AUS", "036", "AU", "AT", "AUT", "040", "AJ", "AZ", "AZE", "031", "BF", "BS", "BHS", "044", "BA", "BH", "BHR", "048", "FQ", "", "", "", "UM","BG", "BD", "BGD", "050", "BB", "BB", "BRB", "052", "BS", "", "", "", "FR","BO", "BY", "BLR", "112", "BE", "BE", "BEL", "056", "BH", "BZ", "BLZ", "084", "BN", "BJ", "BEN", "204", "BD", "BM", "BMU", "060","BT", "BT", "BTN", "064", "BL", "BO", "BOL", "068", "BK", "BA", "BIH", "070", "BC", "BW", "BWA", "072", "BV", "BV", "BVT", "074", "BR", "BR", "BRA", "076", "IO", "IO", "IOT", "086", "VI", "VG", "VGB", "092", "BX", "BN", "BRN", "096", "BU", "BG", "BGR", "100", "UV", "BF", "BFA", "854", "BM", "MM", "MMR", "104", "BY", "BI", "BDI", "108", "CV", "CV", "CPV", "132", "CB", "KH", "KHM", "116", "CM", "CM", "CMR", "120", "CA", "CA", "CAN", "124", "CJ", "KY", "CYM", "136", "CT", "CF", "CAF", "140", "CD", "TD", "TCD", "148", "CI", "CL", "CHL", "152", "CH", "CN", "CHN", "156", "KT", "CX", "CXR", "162", "IP", "", "", "", "FR","CK", "CC", "CCK", "166", "CO", "CO", "COL", "170", "CN", "KM", "COM", "174", "CG", "CD", "COD", "180", "CF", "CG", "COG", "178", "CW", "CK", "COK", "184", "CR", "", "", "", "AU","CS", "CR", "CRI", "188", "IV", "CI", "CIV", "384", "HR", "HR", "HRV", "191", "CU", "CU", "CUB", "192", "UC", "CW", "CUW", "531", "CY", "CY", "CYP", "196", "EZ", "CZ", "CZE", "203", "DA", "DK", "DNK", "208", "DX", "", "", "", "GB","DJ", "DJ", "DJI", "262", "DO", "DM", "DMA", "212", "DR", "DO", "DOM", "214", "EC", "EC", "ECU", "218", "EG", "EG", "EGY", "818", "ES", "SV", "SLV", "222", "EK", "GQ", "GNQ", "226", "ER", "ER", "ERI", "232", "EN", "EE", "EST", "233", "ET", "ET", "ETH", "231", "EU", "", "", "", "FR","FK", "FK", "FLK", "238", "FO", "FO", "FRO", "234", "FJ", "FJ", "FJI", "242", "FI", "FI", "FIN", "246", "FR", "FR", "FRA", "250", "", "FX", "FXX", "249", "FG", "GF", "GUF", "254", "FP", "PF", "PYF", "258", "FS", "TF", "ATF", "260", "GB", "GA", "GAB", "266", "GA", "GM", "GMB", "270", "GZ", "PS", "PSE", "275", "GG", "GE", "GEO", "268", "GM", "DE", "DEU", "276", "GH", "GH", "GHA", "288", "GI", "GI", "GIB", "292", "GO", "", "", "", "FR","GR", "GR", "GRC", "300", "GL", "GL", "GRL", "304", "GJ", "GD", "GRD", "308", "GP", "GP", "GLP", "312", "GQ", "GU", "GUM", "316", "GT", "GT", "GTM", "320", "GK", "GG", "GGY", "831", "GV", "GN", "GIN", "324", "PU", "GW", "GNB", "624", "GY", "GY", "GUY", "328", "HA", "HT", "HTI", "332", "HM", "HM", "HMD", "334", "VT", "VA", "VAT", "336", "HO", "HN", "HND", "340", "HK", "HK", "HKG", "344", "HQ", "", "", "", "UM","HU", "HU", "HUN", "348", "IC", "IS", "ISL", "352", "IN", "IN", "IND", "356", "ID", "ID", "IDN", "360", "IR", "IR", "IRN", "364", "IZ", "IQ", "IRQ", "368", "EI", "IE", "IRL", "372", "IM", "IM", "IMN", "833", "IS", "IL", "ISR", "376", "IT", "IT", "ITA", "380", "JM", "JM", "JAM", "388", "JN", "", "", "", "SV","JA", "JP", "JPN", "392", "DQ", "", "", "", "UM","JE", "JE", "JEY", "832", "JQ", "", "", "", "UM","JO", "JO", "JOR", "400", "JU", "", "", "", "FR","KZ", "KZ", "KAZ", "398", "KE", "KE", "KEN", "404", "KQ", "", "", "", "UM","KR", "KI", "KIR", "296", "KN", "KP", "PRK", "408", "KS", "KR", "KOR", "410", "KV", "XK", "XKS", "", "KU", "KW", "KWT", "414", "KG", "KG", "KGZ", "417", "LA", "LA", "LAO", "418", "LG", "LV", "LVA", "428", "LE", "LB", "LBN", "422", "LT", "LS", "LSO", "426", "LI", "LR", "LBR", "430", "LY", "LY", "LBY", "434", "LS", "LI", "LIE", "438", "LH", "LT", "LTU", "440", "LU", "LU", "LUX", "442", "MC", "MO", "MAC", "446", "MK", "MK", "MKD", "807", "MA", "MG", "MDG", "450", "MI", "MW", "MWI", "454", "MY", "MY", "MYS", "458", "MV", "MV", "MDV", "462", "ML", "ML", "MKI", "466", "MT", "MT", "MLT", "470", "RM", "MH", "MHL", "584", "MB", "MQ", "MTQ", "474", "MR", "MR", "MRT", "478", "MP", "MU", "MUS", "480", "MF", "YT", "MYT", "175", "MX", "MX", "MEX", "484", "FM", "FM", "FSM", "583", "MQ", "", "", "", "UM","MD", "MD", "MDA", "498", "MN", "MC", "MCO", "492", "MG", "MN", "MNG", "496", "MJ", "ME", "MNE", "499", "MH", "MS", "MSR", "500", "MO", "MA", "MAR", "504", "MZ", "MZ", "MOZ", "508", "WA", "NA", "NAM", "516", "NR", "NR", "NRU", "520", "BQ", "", "", "", "UM","NP", "NP", "NPL", "524", "NL", "NL", "NLT", "528", "NT", "", "", "", "NC", "NC", "NCL", "540", "NZ", "NZ", "NZL", "554", "NU", "NI", "NIC", "558", "NG", "NE", "NER", "562", "NI", "NG", "NGA", "566", "NE", "NU", "NIU", "570", "NF", "NF", "NFK", "574", "CQ", "MP", "MNP", "580", "NO", "NO", "NOR", "578", "MU", "OM", "OMN", "512", "PK", "PK", "PAK", "586", "PS", "PW", "PLW", "585", "LQ", "", "", "", "UM","PM", "PA", "PAN", "591", "PP", "PG", "PNG", "592", "PF", "", "", "", "PA", "PY", "PRY", "600", "PE", "PE", "PER", "604", "RP", "PH", "PHL", "608", "PC", "PN", "PCN", "612", "PL", "PL", "POL", "616", "PO", "PT", "PRT", "620", "RQ", "PR", "PRI", "630", "QA", "QA", "QAT", "634", "RE", "RE", "REU", "638", "RO", "RO", "ROU", "642", "RS", "RU", "RUS", "643", "RW", "RW", "RWA", "646", "TB", "BL", "BLM", "652", "SH", "SH", "SHN", "654", "SC", "KN", "KNA", "659", "ST", "LC", "LCA", "662", "RN", "MF", "MAF", "663", "SB", "PM", "SPM", "666", "VC", "VC", "VCT", "670", "WS", "WS", "WSM", "882", "SM", "SM", "SMR", "674", "TP", "ST", "STP", "678", "SA", "SA", "SAU", "682", "SG", "SN", "SEN", "686", "RI", "RS", "SRB", "688", "SE", "SC", "SYC", "690", "SL", "SL", "SLE", "694", "SN", "SG", "SGP", "702", "NN", "SX", "SXM", "534", "LO", "SK", "SVK", "703", "SI", "SI", "SVN", "705", "BP", "SB", "SLB", "090", "SO", "SO", "SOM", "706", "SF", "ZA", "ZAF", "710", "SX", "GS", "SGS", "239", "OD", "SS", "SSD", "728", "SP", "ES", "ESP", "724", "PG", "", "", "", "CE", "LK", "LKA", "144", "SU", "SD", "SDN", "729", "NS", "SR", "SUR", "740", "SV", "SJ", "SJM", "744", "WZ", "SZ", "SWZ", "748", "SW", "SE", "SWE", "752", "SZ", "CH", "CHE", "756", "SY", "SY", "SYR", "760", "TW", "TW", "TWN", "158", "TI", "TJ", "TJK", "762", "TZ", "TZ", "TZA", "834", "TH", "TH", "THA", "764", "TT", "TL", "TLS", "626", "TO", "TG", "TGO", "768", "TL", "TK", "TKL", "772", "TN", "TO", "TON", "776", "TD", "TT", "TTO", "780", "TE", "", "", "", "FR","TS", "TN", "TUN", "788", "TU", "TR", "TUR", "792", "TX", "TM", "TKM", "795", "TK", "TC", "TCA", "796", "TV", "TV", "TUV", "798", "UG", "UG", "UGA", "800", "UP", "UA", "UKR", "804", "AE", "AE", "ARE", "784", "UK", "GB", "GBR", "826", "US", "US", "USA", "840", "", "UM", "UMI", "581", "UY", "UY", "URY", "858", "UZ", "UZ", "UZB", "860", "NH", "VU", "VUT", "548", "VE", "VE", "VEN", "862", "VM", "VN", "VNH", "704", "VQ", "VI", "VIR", "850", "WQ", "", "", "", "UM","WF", "WF", "WLF", "876", "WE", "PS", "PSE", "275", "WI", "EH", "ESH", "732", "YM", "YE", "YEM", "887", "ZA", "ZM", "ZMB", "894", "ZI", "ZW", "ZWE", "716" };
        private static List<Country> countryList;

        /// <summary/>
        /// [Key]
        public int Itu { get; set; }
        /// <summary/>
        public string Name { get; set; }
        /// <summary/>
        public string ArabicName { get; set; }
        /// <summary/>
        public string Latitude { get; set; }
        /// <summary/>
        public string Longitude { get; set; }
        /// <summary/>
        public string Iso2 { get; set; }
        /// <summary/>
        public string Iso3 { get; set; }
        /// <summary/>
        public string Iana { get; set; }
        /// <summary/>
        public string Un { get; set; }
        /// <summary/>
        public string Ioc { get; set; }
        /// <summary/>
        public string Fips { get; set; } 

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public Country() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public Country(int itu)
        {
            Country country;

            country = CountryByItu(itu);

            this.Itu = country.Itu;
            this.Name = country.Name;
            this.ArabicName = country.ArabicName;
            this.Latitude = country.Latitude;
            this.Longitude = country.Longitude;
            this.Iso2 = country.Iso2;
            this.Iso3 = country.Iso3;
            this.Iana = country.Iana;
            this.Un = country.Un;
            this.Ioc = country.Ioc;
            this.Fips = country.Fips;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Country CountryByItu(int itu)
        {
            Country country;

            country = (from q in XDocument.Elements("countryList").Elements("country")
                       where q.Attribute("itu").Value == itu.ToString()
                       select new Country
                       {
                           Itu = itu,
                           Name = q.Attribute("name").Value,
                           ArabicName = q.Attribute("arabicName").Value,
                           Latitude = q.Attribute("latitude").Value,
                           Longitude = q.Attribute("longitude").Value,
                           Iso2 = q.Attribute("iso2").Value,
                           Iso3 = q.Attribute("iso3").Value,
                           Iana = q.Attribute("iana").Value,
                           Un = q.Attribute("un").Value,
                           Ioc = q.Attribute("ioc").Value,
                           Fips = q.Attribute("fips").Value
                       }
            ).First<Country>();

            return country;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<Country> CountryList
        {
            get
            {
                if (countryList == null || countryList.Count == 0)
                {
                    countryList = (from q in XDocument.Elements("countryList").Elements("country")
                                   where q.Attribute("itu").Value != ""
                                   select new Country
                                   {
                                       Itu = int.Parse(q.Attribute("itu").Value.Replace("-", "")),
                                       Name = q.Attribute("name").Value,
                                       ArabicName = q.Attribute("arabicName").Value,
                                       Latitude = q.Attribute("latitude").Value,
                                       Longitude = q.Attribute("longitude").Value,
                                       Iso2 = q.Attribute("iso2").Value,
                                       Iso3 = q.Attribute("iso3").Value,
                                       Iana = q.Attribute("iana").Value,
                                       Un = q.Attribute("un").Value,
                                       Ioc = q.Attribute("ioc").Value,
                                       Fips = FipsFromIso2(q.Attribute("iso2").Value)
                                   }
                    ).ToList<Country>();
                }

                return countryList;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static string FipsFromIso2(string iso2)
        {
            int i;
            string fips;

            if (iso2 != null && iso2.Length == 2)
            {
                i = fipsIsoList.IndexOf(iso2);

                if (i >= 0)
                {
                    // below: special case for "AF"
                    if (i == 0) i++;

                    fips = fipsIsoList[i - 1].ToString();
                }
                else fips = null;
            }
            else fips = null;

            return fips;
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
                streamReader = new StreamReader(_assembly.GetManifestResourceStream("Ia.Cl.model.country.country.xml"));

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
