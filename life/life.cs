using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;

namespace Ia.Cl.Model.Life
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// General base class for life entities. Make it link through delegates to create and update database objects.
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
    public class Main
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Main() { }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public sealed class Area
    {
        private static TextInfo textInfo;
        private static XDocument countryArea;

        /// <summary/>
        public Area() { }

        /// <summary/>
        public static List<Area> Initialize(City city)
        {
            countryArea = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\country_area.xml");
            Area area;
            List<Area> areas;

            areas = new List<Area>();

            textInfo = new CultureInfo("en-US", false).TextInfo;
            // create a TextInfo based on the "en-US" culture.

            foreach (XElement xe in countryArea.Descendants("city").Descendants("area"))
            {
                if (city.XmlId == int.Parse(xe.Parent.Attribute("id").Value))
                {
                    area = new Area();

                    area.City = city;

                    area.XmlId = int.Parse(xe.Attribute("id").Value);

                    // convert that name to a nice title case format
                    area.Name = xe.Attribute("name").Value;
                    area.Name = textInfo.ToTitleCase(area.Name.ToLower());

                    //area.NameInArabic = xe.Attribute("name_ar").Value;

                    areas.Add(area);
                }
            }

            return areas;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public City City { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public sealed class City
    {
        private static TextInfo textInfo;
        private static XDocument countryArea;

        /// <summary/>
        public City() { }

        /// <summary/>
        public static List<City> Initialize(Province province)
        {
            countryArea = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\country_area.xml");
            City city;
            List<City> cities;

            cities = new List<City>();

            textInfo = new CultureInfo("en-US", false).TextInfo;
            // create a TextInfo based on the "en-US" culture.

            foreach (XElement xe in countryArea.Descendants("city"))
            {
                if (province.XmlId == int.Parse(xe.Parent.Attribute("id").Value))
                {
                    city = new City();

                    city.Province = province;

                    city.XmlId = int.Parse(xe.Attribute("id").Value);

                    // convert that name to a nice title case format
                    city.Name = xe.Attribute("name").Value;
                    city.Name = textInfo.ToTitleCase(city.Name.ToLower());

                    //city.NameInArabic = xe.Attribute("name_ar").Value;

                    cities.Add(city);
                }
            }

            return cities;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public Province Province { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public sealed class Province
    {
        private static TextInfo textInfo;
        private static XDocument countryArea;

        /// <summary/>
        public Province() { }

        /// <summary/>
        public static List<Province> Initialize(Country country)
        {
            countryArea = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\country_area.xml");
            Province province;
            List<Province> provinces;

            provinces = new List<Province>();

            textInfo = new CultureInfo("en-US", false).TextInfo;
            // create a TextInfo based on the "en-US" culture.

            foreach (XElement xe in countryArea.Descendants("province"))
            {
                if (country.XmlId == int.Parse(xe.Parent.Attribute("id").Value))
                {
                    province = new Province();

                    province.Country = country;

                    province.XmlId = int.Parse(xe.Attribute("id").Value);

                    // convert that name to a nice title case format
                    province.Name = xe.Attribute("name").Value;
                    province.Name = textInfo.ToTitleCase(province.Name.ToLower());

                    //province.NameInArabic = xe.Attribute("name_ar").Value;

                    provinces.Add(province);
                }
            }

            return provinces;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public Country Country { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public sealed class Country
    {
        private static TextInfo textInfo;
        private static XDocument countryArea;

        /// <summary/>
        public Country() { }

        /// <summary/>
        public static List<Country> Initialize()
        {
            countryArea = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\country_area.xml");
            Country country;
            List<Country> countries;

            countries = new List<Country>();

            textInfo = new CultureInfo("en-US", false).TextInfo;
            // create a TextInfo based on the "en-US" culture.

            foreach (XElement xe in countryArea.Descendants("country"))
            {
                country = new Country();

                country.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                country.Name = xe.Attribute("name").Value;
                country.Name = textInfo.ToTitleCase(country.Name.ToLower());

                country.NameInArabic = xe.Attribute("name_ar").Value;
                country.Iso2 = xe.Attribute("iso2").Value;
                country.Iso3 = xe.Attribute("iso3").Value;
                country.Iana = xe.Attribute("iana").Value;
                country.Un = xe.Attribute("un").Value;
                country.Ioc = xe.Attribute("ioc").Value;
                country.Itu = xe.Attribute("itu").Value;

                countries.Add(country);
            }

            return countries;
        }

        /// <summary/>
        //[System.Data.Linq.Mapping.Column(Name = "Id", IsPrimaryKey = true, IsDbGenerated = false))]
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }

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
        public string Itu { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Address
    /// </summary>
    public class Address
    {
        // address format varies depending of country

        /// <summary/>
        public Address() { }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public Country Country { get; set; }

        /// <summary/>
        public Province Province { get; set; }

        /// <summary/>
        public City City { get; set; }

        /// <summary/>
        public Area Area { get; set; }

        /// <summary/>
        public string PostalCode { get; set; }

        /// <summary/>
        public string Line { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class ContactType
    {
        private static XDocument xd;

        /// <summary/>
        public ContactType() { }

        /// <summary/>
        public static List<ContactType> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            ContactType contactType;
            List<ContactType> contactTypes;

            contactTypes = new List<ContactType>();

            foreach (XElement xe in xd.Descendants("contact").Descendants("type"))
            {
                contactType = new ContactType();

                contactType.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                contactType.Name = xe.Attribute("name").Value;
                contactType.NameInArabic = xe.Attribute("name_ar").Value;

                contactTypes.Add(contactType);
            }

            return contactTypes;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class CorrespondenceType
    {
        private static XDocument xd;

        /// <summary/>
        public CorrespondenceType() { }

        /// <summary/>
        public static List<CorrespondenceType> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            CorrespondenceType correspondenceType;
            List<CorrespondenceType> correspondenceTypes;

            correspondenceTypes = new List<CorrespondenceType>();

            foreach (XElement xe in xd.Descendants("correspondence").Descendants("type"))
            {
                correspondenceType = new CorrespondenceType();

                correspondenceType.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                correspondenceType.Name = xe.Attribute("name").Value;
                correspondenceType.NameInArabic = xe.Attribute("name_ar").Value;

                correspondenceTypes.Add(correspondenceType);
            }

            return correspondenceTypes;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class CorrespondenceState
    {
        private static XDocument xd;

        /// <summary/>
        public CorrespondenceState() { }

        /// <summary/>
        public static List<CorrespondenceState> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            CorrespondenceState correspondenceState;
            List<CorrespondenceState> correspondenceStates;

            correspondenceStates = new List<CorrespondenceState>();

            foreach (XElement xe in xd.Descendants("correspondence").Descendants("state"))
            {
                correspondenceState = new CorrespondenceState();

                correspondenceState.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                correspondenceState.Name = xe.Attribute("name").Value;
                correspondenceState.NameInArabic = xe.Attribute("name_ar").Value;

                correspondenceStates.Add(correspondenceState);
            }

            return correspondenceStates;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class CorrespondenceDirection
    {
        private static XDocument xd;

        /// <summary/>
        public CorrespondenceDirection() { }

        /// <summary/>
        public static List<CorrespondenceDirection> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            CorrespondenceDirection correspondenceDirection;
            List<CorrespondenceDirection> correspondenceDirections;

            correspondenceDirections = new List<CorrespondenceDirection>();

            foreach (XElement xe in xd.Descendants("correspondence").Descendants("direction"))
            {
                correspondenceDirection = new CorrespondenceDirection();

                correspondenceDirection.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                correspondenceDirection.Name = xe.Attribute("name").Value;
                correspondenceDirection.NameInArabic = xe.Attribute("name_ar").Value;

                correspondenceDirections.Add(correspondenceDirection);
            }

            return correspondenceDirections;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class Correspondence
    {
        /// <summary/>
        public Correspondence() { }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public CorrespondenceType Type { get; set; }

        /// <summary/>
        public CorrespondenceState State { get; set; }

        /// <summary/>
        public CorrespondenceDirection Direction { get; set; }

        /// <summary/>
        public Correspondence CorrespondenceId { get; set; }

        /// <summary/>
        //public Contact ContactId { get; set; }

        /// <summary/>
        public Advertisement AdvertisementId { get; set; }

        /// <summary/>
        public Request RequestId { get; set; }

        /// <summary/>
        public string Subject { get; set; }

        /// <summary/>
        public string Content { get; set; }

        /// <summary/>
        public string Note { get; set; }

        /// <summary/>
        public DateTime Created { get; set; }

        /// <summary/>
        public DateTime Updated { get; set; }

    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class PropertyType
    {
        private static XDocument xd;

        /// <summary/>
        public PropertyType() { }

        /// <summary/>
        public static List<PropertyType> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            PropertyType propertyType;
            List<PropertyType> propertyTypes;

            propertyTypes = new List<PropertyType>();

            foreach (XElement xe in xd.Descendants("property").Descendants("type"))
            {
                propertyType = new PropertyType();

                propertyType.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                propertyType.Name = xe.Attribute("name").Value;
                propertyType.NameInArabic = xe.Attribute("name_ar").Value;

                propertyTypes.Add(propertyType);
            }

            return propertyTypes;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class PropertyState
    {
        private static XDocument xd;

        /// <summary/>
        public PropertyState() { }

        /// <summary/>
        public static List<PropertyState> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            PropertyState propertyState;
            List<PropertyState> propertyStates;

            propertyStates = new List<PropertyState>();

            foreach (XElement xe in xd.Descendants("property").Descendants("state"))
            {
                propertyState = new PropertyState();

                propertyState.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                propertyState.Name = xe.Attribute("name").Value;
                propertyState.NameInArabic = xe.Attribute("name_ar").Value;

                propertyStates.Add(propertyState);
            }

            return propertyStates;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Property
    /// </summary>
    public class Property
    {
        /// <summary/>
        public Property() { }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public PropertyType Type { get; set; }

        /// <summary/>
        public PropertyState State { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public Address Address { get; set; }

        /// <summary/>
        public string Summary { get; set; }

        /// <summary/>
        public string SummaryInArabic { get; set; }

        /// <summary/>
        public string Detail { get; set; }

        /// <summary/>
        public string DetailInArabic { get; set; }

        /// <summary/>
        //public Contact Contact { get; set; }

        /// <summary/>
        public bool Approved { get; set; }

        /// <summary/>
        public string Note { get; set; }

        /// <summary/>
        public DateTime Created { get; set; }

        /// <summary/>
        public DateTime Updated { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class PriceType
    {
        private static XDocument xd;

        /// <summary/>
        public PriceType() { }

        /// <summary/>
        public static List<PriceType> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            PriceType priceType;
            List<PriceType> priceTypes;

            priceTypes = new List<PriceType>();

            foreach (XElement xe in xd.Descendants("property").Descendants("advertisement").Descendants("price").Descendants("type"))
            {
                priceType = new PriceType();

                priceType.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                priceType.Name = xe.Attribute("name").Value;
                priceType.NameInArabic = xe.Attribute("name_ar").Value;

                priceTypes.Add(priceType);
            }

            return priceTypes;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class AdvertisementType
    {
        private static XDocument xd;

        /// <summary/>
        public AdvertisementType() { }

        /// <summary/>
        public static List<AdvertisementType> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            AdvertisementType advertisementType;
            List<AdvertisementType> advertisementTypes;

            advertisementTypes = new List<AdvertisementType>();

            foreach (XElement xe in xd.Descendants("property").Descendants("advertisement").Descendants("type"))
            {
                advertisementType = new AdvertisementType();

                advertisementType.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                advertisementType.Name = xe.Attribute("name").Value;
                advertisementType.NameInArabic = xe.Attribute("name_ar").Value;

                advertisementTypes.Add(advertisementType);
            }

            return advertisementTypes;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class AdvertisementState
    {
        private static XDocument xd;

        /// <summary/>
        public AdvertisementState() { }

        /// <summary/>
        public static List<AdvertisementState> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            AdvertisementState advertisementState;
            List<AdvertisementState> advertisementStates;

            advertisementStates = new List<AdvertisementState>();

            foreach (XElement xe in xd.Descendants("property").Descendants("advertisement").Descendants("state"))
            {
                advertisementState = new AdvertisementState();

                advertisementState.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                advertisementState.Name = xe.Attribute("name").Value;
                advertisementState.NameInArabic = xe.Attribute("name_ar").Value;

                advertisementStates.Add(advertisementState);
            }

            return advertisementStates;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Advertisement
    /// </summary>
    public class Advertisement
    {
        /// <summary/>
        public Advertisement() { }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public AdvertisementType Type { get; set; }

        /// <summary/>
        public AdvertisementState State { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public Property Property { get; set; }

        /// <summary/>
        public Language Language { get; set; }

        /// <summary/>
        public string Price { get; set; }

        /// <summary/>
        public PriceType PriceType { get; set; }

        /// <summary/>
        public string Summary { get; set; }

        /// <summary/>
        public string Detail { get; set; }

        /// <summary/>
        public bool Approved { get; set; }

        /// <summary/>
        public bool Show { get; set; }

        /// <summary/>
        public string Note { get; set; }

        /// <summary/>
        public string User { get; set; }

        /// <summary/>
        public DateTime Created { get; set; }

        /// <summary/>
        public DateTime Updated { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class RequestType
    {
        private static XDocument xd;

        /// <summary/>
        public RequestType() { }

        /// <summary/>
        public static List<RequestType> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            RequestType requestType;
            List<RequestType> requestTypes;

            requestTypes = new List<RequestType>();

            foreach (XElement xe in xd.Descendants("property").Descendants("request").Descendants("type"))
            {
                requestType = new RequestType();

                requestType.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                requestType.Name = xe.Attribute("name").Value;
                requestType.NameInArabic = xe.Attribute("name_ar").Value;

                requestTypes.Add(requestType);
            }

            return requestTypes;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class RequestState
    {
        private static XDocument xd;

        /// <summary/>
        public RequestState() { }

        /// <summary/>
        public static List<RequestState> Initialize()
        {
            xd = XDocument.Load(global::Ia.Cl.Model.Default.AbsolutePath() + @"app_data\this.xml");
            RequestState requestState;
            List<RequestState> requestStates;

            requestStates = new List<RequestState>();

            foreach (XElement xe in xd.Descendants("property").Descendants("request").Descendants("state"))
            {
                requestState = new RequestState();

                requestState.XmlId = int.Parse(xe.Attribute("id").Value);

                // convert that name to a nice title case format
                requestState.Name = xe.Attribute("name").Value;
                requestState.NameInArabic = xe.Attribute("name_ar").Value;

                requestStates.Add(requestState);
            }

            return requestStates;
        }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public int XmlId { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public string NameInArabic { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Request
    /// </summary>
    public class Request
    {
        /// <summary/>
        public Request() { }

        /// <summary/>
        public int Id { get; set; }

        /// <summary/>
        public RequestType Type { get; set; }

        /// <summary/>
        public RequestState State { get; set; }

        /// <summary/>
        public string Name { get; set; }

        /// <summary/>
        public Address Address { get; set; }

        /// <summary/>
        public string Summary { get; set; }

        /// <summary/>
        public string Detail { get; set; }

        /// <summary/>
        public string Price { get; set; }

        /// <summary/>
        //public Contact Contact { get; set; }

        /// <summary/>
        public bool Approved { get; set; }

        /// <summary/>
        public bool Show { get; set; }

        /// <summary/>
        public string Note { get; set; }

        /// <summary/>
        public string User { get; set; }

        /// <summary/>
        public DateTime Created { get; set; }

        /// <summary/>
        public DateTime Updated { get; set; }
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    #region enum

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// O+	 A+	 B+	AB+	 O-	 A-	 B-	AB-
    /// </summary>
    public enum Blood
    {
        /// <summary/>
        O_positive = 1,

        /// <summary/>
        A_positive = 2,

        /// <summary/>
        B_positive = 3,

        /// <summary/>
        AB_positive = 4,

        /// <summary/>
        O_negative = 5,

        /// <summary/>
        A_negative = 6,

        /// <summary/>
        B_negative = 7,

        /// <summary/>
        AB_negative = 8
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public enum Social_State
    {
        /// <summary/>
        Single = 1,

        /// <summary/>
        Married = 2
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public enum Educational_State
    {
        /// <summary/>
        High_School = 9,

        /// <summary/>
        University = 10
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public enum Passport_Type
    {
        /// <summary/>
        Standard = 1,

        /// <summary/>
        Diplomatic = 2
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}
