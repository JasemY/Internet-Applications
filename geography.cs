using System;
using System.Web;
using System.Xml;
using System.IO;
using System.Data;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Ia.Cl.Model.Geography
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Geographic location related function, location, coordinates (latitude, longitude), bearing, degree and radian conversions, CMap value for resolution, and country geographic info-IP from MaxMind.
    /// </summary>
    /// 
    /// <see cref="http://en.wikipedia.org/wiki/Geographic_coordinate_conversion"/>
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
    public class Location
    {
        private Coordinate latitude, longitude;
        private CoordinateType coordinateType { get; set; }

        private enum CoordinateType { Latitude, Longitude, Unknown };

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Location(double latitudeLocationInDegreeDecimal, double longitudeLocationInDegreeDecimal)
        {
            latitude = new Coordinate(latitudeLocationInDegreeDecimal);
            longitude = new Coordinate(longitudeLocationInDegreeDecimal);
        }

        /// <summary/>
        public double Latitude { get { return latitude.DegreeDecimal; } }

        /// <summary/>
        public double Longitude { get { return longitude.DegreeDecimal; } }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Accepts two coordinates in degrees.
        /// </summary>
        /// <returns>A double value in degrees from 0 to 360.</returns>
        public static Double Bearing(Location coordinate1, Location coordinate2)
        {
            var latitude1 = DegreeToRadian(coordinate1.Latitude);
            var latitude2 = DegreeToRadian(coordinate2.Latitude);

            var longitudeDifference = DegreeToRadian((coordinate2.Longitude - coordinate1.Longitude));

            var y = Math.Sin(longitudeDifference) * Math.Cos(latitude2);
            var x = Math.Cos(latitude1) * Math.Sin(latitude2) -
                    Math.Sin(latitude1) * Math.Cos(latitude2) * Math.Cos(longitudeDifference);

            return (RadianToDegree(Math.Atan2(y, x)) + 360) % 360;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public static string ReturnCMapValueForResolutionAndCoordinates(int resolution, double latitude, double longitude)
        {
            Location location;

            location = new Location(latitude, longitude);

            return location.ReturnCMapValueForResolution(resolution);
        }
     
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Converting latitude/longitude pairs to equivalent C-Squares
        /// <remark ref="http://en.wikipedia.org/wiki/C-squares"/>
        /// <remark ref="http://www.cmar.csiro.au/csquares/Point_java.txt"/>
        /// </summary>
        public string ReturnCMapValueForResolution(double resolution)
        {
            int i, j;
            double llat, llon;
            StringBuilder sb = new StringBuilder();

            if (latitude.DegreeDecimal >= 0)
            {
                if (longitude.DegreeDecimal >= 0) sb.Append('1');
                else sb.Append('7');
            }
            else
            {
                if (longitude.DegreeDecimal >= 0) sb.Append('3');
                else sb.Append('5');
            }

            llat = Math.Abs(latitude.DegreeDecimal);

            if (llat >= 90) llat = 89.9;

            llon = Math.Abs(longitude.DegreeDecimal);

            if (llon >= 180) llon = 179.9;

            i = (int)(llat / 10);

            sb.Append(i);

            j = (int)(llon / 10);

            if (j < 10) sb.Append('0');

            sb.Append(j);

            if (resolution == 10)
            {
                return sb.ToString();
            }
            else
            {
                sb.Append(':');

                llat -= i * 10;

                llon -= j * 10;

                i = (int)llat;

                j = (int)llon;

                if (i < 5)
                {
                    if (j < 5) sb.Append('1');
                    else sb.Append('2');
                }
                else
                {
                    if (j < 5) sb.Append('3');
                    else sb.Append('4');
                }

                if (resolution == 5)
                {
                    return sb.ToString();
                }
                else
                {
                    sb.Append(i);

                    sb.Append(j);

                    if (resolution == 1)
                    {
                        return sb.ToString();
                    }
                    else
                    {
                        sb.Append(':');

                        i = (int)((llat - i) * 10);

                        j = (int)((llon - j) * 10);

                        if (i < 5)
                        {
                            if (j < 5) sb.Append('1');
                            else sb.Append('2');
                        }
                        else
                        {
                            if (j < 5) sb.Append('3');
                            else sb.Append('4');
                        }

                        if (resolution == 0.5)
                        {
                            return sb.ToString();
                        }
                        else
                        {
                            sb.Append(i);
                            sb.Append(j);

                            return sb.ToString();
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }


    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// <remark ref="http://en.wikipedia.org/wiki/Geographic_coordinate_conversion"/>
    /// </summary>
    public class Coordinate
    {
        private double degreeDecimal;
        private CoordinateType coordinateType { get; set; }

        private enum CoordinateType { Latitude, Longitude, Unknown };

        /// <summary/>
        public bool IsNegative { get; set; }

        /// <summary/>
        public int Degrees { get; set; }

        /// <summary/>
        public int Minutes { get; set; }

        /// <summary/>
        public int Seconds { get; set; }

        /// <summary/>
        public int Milliseconds { get; set; }

        /*
         * There are three basic forms of a coordinate.
         * 
         * Coordinate containing degrees (integer), minutes (integer), and seconds (integer, or real number) (DMS).
         * Coordinate containing degrees (integer) and minutes (real number) (MinDec).
         * Coordinate containing only degrees (real number) (DegDec).
         * 
         */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Coordinate()
        {
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Coordinate(double locationInDegreeDecimal)
        {
            degreeDecimal = locationInDegreeDecimal;

            // ensure the value will fall within the primary range [-180.0..+180.0]
            while (locationInDegreeDecimal < -180.0) locationInDegreeDecimal += 360.0;

            while (locationInDegreeDecimal > 180.0) locationInDegreeDecimal -= 360.0;

            // switch the value to positive
            IsNegative = locationInDegreeDecimal < 0;
            locationInDegreeDecimal = Math.Abs(locationInDegreeDecimal);

            // gets the degree
            Degrees = (int)Math.Floor(locationInDegreeDecimal);
            var delta = locationInDegreeDecimal - Degrees;

            // gets minutes and seconds
            var seconds = (int)Math.Floor(3600.0 * delta);
            Seconds = seconds % 60;
            Minutes = (int)Math.Floor(seconds / 60.0);
            delta = delta * 3600.0 - seconds;

            // gets fractions
            Milliseconds = (int)(1000.0 * delta);

            coordinateType = CoordinateType.Unknown;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Coordinate(string locationInDegreeMinuteDecimalSecond)
        {
            double d;
            string sign, r;
            Match m;

            sign = "";

            /*
             * All of the following are valid and acceptable ways to write geographic coordinates:
             * 40°26'47"N 79°58'36"W
             * 40°26′47″N 79°58′36″W
             * 40:26:46N,79:56:55W
             * 40:26:46.302N 79:56:55.903W

             * 40d 26′ 47″ N 79d 58′ 36″ W
             * 40.446195N 79.948862W
             * 40.446195, -79.948862
             * 40° 26.7717, -79° 56.93172
             */

            m = Regex.Match(locationInDegreeMinuteDecimalSecond, @"(\d{1,2})°(\d{1,2})['′](.{1,})[""″]([NSEW])");

            if (m.Success) { }
            else m = Regex.Match(locationInDegreeMinuteDecimalSecond, @"(\d{1,2})[: ](\d{1,2})[: ](.{1,})([NSEW])");

            if (m.Success)
            {
                try
                {
                    Degrees = int.Parse(m.Groups[1].Captures[0].Value);
                    Minutes = int.Parse(m.Groups[2].Captures[0].Value);

                    d = double.Parse(m.Groups[3].Captures[0].Value);

                    Seconds = (int)d;

                    d = d - Seconds;

                    Milliseconds = (int)(d * 1000);

                    // the sign
                    sign = m.Groups[4].Captures[0].Value;
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
            }
            else
            {
                m = Regex.Match(locationInDegreeMinuteDecimalSecond, @"(\d{1,2})°(\d{1,2})['′]([NSEW])");

                if (m.Success) { }
                else m = Regex.Match(locationInDegreeMinuteDecimalSecond, @"(\d{1,2})[: ](\d{1,2})[: ]([NSEW])");

                if (m.Success)
                {
                    try
                    {
                        Degrees = int.Parse(m.Groups[1].Captures[0].Value);
                        Minutes = int.Parse(m.Groups[2].Captures[0].Value);

                        Seconds = 0;
                        Milliseconds = 0;

                        // the sign
                        sign = m.Groups[3].Captures[0].Value;
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
                }
            }

            if (sign == "N" || sign == "E")
            {
                IsNegative = false;
                coordinateType = CoordinateType.Latitude;
            }
            else if (sign == "S" || sign == "W")
            {
                IsNegative = true;
                coordinateType = CoordinateType.Longitude;
            }
            else
            {
                coordinateType = CoordinateType.Unknown;
                //throw new Exception();
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public double DegreeDecimal
        {
            get
            {
                double d;

                //return degreeDecimal;

                // Calculate the total number of seconds:
                d = Milliseconds / 1000.0 + Seconds + Minutes * 60;

                // The fractional part is total number of seconds divided by 3600:
                d = d / 3600;

                // Add fractional degrees to whole degrees to produce the final result:
                d = Degrees + d;

                // switch the value to positive
                if (IsNegative) d = -d;

                return Math.Round(d, 6);
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string DegreeMinuteSecond()
        {
            switch (coordinateType)
            {
                case CoordinateType.Latitude:
                    return string.Format(
                        "{0}°{1:00}'{2:00}\"{3}",
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.IsNegative ? 'S' : 'N');

                case CoordinateType.Longitude:
                    return string.Format(
                        "{0}°{1:00}'{2:00}\"{3}",
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.IsNegative ? 'W' : 'E');

                default:
                    return string.Format(
                        "{0}°{1:00}'{2:00}\"",
                        this.Degrees,
                        this.Minutes,
                        this.Seconds);
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string DegreeMinuteDecimalSecond()
        {
            switch (coordinateType)
            {
                case CoordinateType.Latitude:
                    return string.Format(
                        "{0}°{1:00}'{2:00}.{3:000}\"{4}",
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.Milliseconds,
                        this.IsNegative ? 'S' : 'N');

                case CoordinateType.Longitude:
                    return string.Format(
                        "{0}°{1:00}'{2:00}.{3:000}\"{4}",
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.Milliseconds,
                        this.IsNegative ? 'W' : 'E');

                default:
                    return string.Format(
                        "{0}°{1:00}'{2:00}.{3:000}\"",
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.Milliseconds);
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string DegreeMinuteSecondDecimalMilliSecond()
        {
            switch (coordinateType)
            {
                case CoordinateType.Latitude:
                    return string.Format(
                        "{0}°{1:00}'{2:00}\"0.{3:000}{4}",
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.Milliseconds,
                        this.IsNegative ? 'S' : 'N');

                case CoordinateType.Longitude:
                    return string.Format(
                        "{0}°{1:00}'{2:00}\"0.{3:000}{4}",
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.Milliseconds,
                        this.IsNegative ? 'W' : 'E');

                default:
                    return string.Format(
                        "{0}°{1:00}'{2:00}\"0.{3:000}",
                        this.Degrees,
                        this.Minutes,
                        this.Seconds,
                        this.Milliseconds);
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /* CountryLookup.cs
     *
     * Copyright (C) 2008 MaxMind, Inc.  All Rights Reserved.
     *
     * This library is free software; you can redistribute it and/or
     * modify it under the terms of the GNU General Public
     * License as published by the Free Software Foundation; either
     * version 2 of the License, or (at your option) any later version.
     *
     * This library is distributed in the hope that it will be useful,
     * but WITHOUT ANY WARRANTY; without even the implied warranty of
     * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
     * General Public License for more details.
     *
     * You should have received a copy of the GNU General Public
     * License along with this library; if not, write to the Free Software
     * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
     */

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class GeoIp
    {
        private FileStream fileInput;
        private long COUNTRY_BEGIN = 16776960;
        private string[] countryCode = 
								{ "--","AP","EU","AD","AE","AF","AG","AI","AL","AM","AN","AO","AQ","AR","AS","AT","AU","AW","AZ","BA","BB","BD","BE","BF","BG","BH","BI","BJ","BM","BN","BO","BR","BS","BT","BV","BW","BY","BZ","CA","CC","CD","CF","CG","CH","CI","CK","CL","CM","CN","CO","CR","CU","CV","CX","CY","CZ","DE","DJ","DK","DM","DO","DZ",
									"EC","EE","EG","EH","ER","ES","ET","FI","FJ","FK","FM","FO","FR","FX","GA","GB","GD","GE","GF","GH","GI","GL","GM","GN","GP","GQ","GR","GS","GT","GU","GW","GY","HK","HM","HN","HR","HT","HU","ID","IE","IL","IN","IO","IQ","IR","IS","IT","JM","JO","JP","KE","KG","KH","KI","KM","KN","KP","KR","KW","KY","KZ",
									"LA","LB","LC","LI","LK","LR","LS","LT","LU","LV","LY","MA","MC","MD","MG","MH","MK","ML","MM","MN","MO","MP","MQ","MR","MS","MT","MU","MV","MW","MX","MY","MZ","NA","NC","NE","NF","NG","NI","NL","NO","NP","NR","NU","NZ","OM","PA","PE","PF","PG","PH","PK","PL","PM","PN","PR","PS","PT","PW","PY","QA",
									"RE","RO","RU","RW","SA","SB","SC","SD","SE","SG","SH","SI","SJ","SK","SL","SM","SN","SO","SR","ST","SV","SY","SZ","TC","TD","TF","TG","TH","TJ","TK","TM","TN","TO","TL","TR","TT","TV","TW","TZ","UA","UG","UM","US","UY","UZ","VA","VC","VE","VG","VI","VN","VU","WF","WS","YE","YT","RS","ZA","ZM","ME","ZW","A1","A2",
									"O1","AX","GG","IM","JE","BL","MF"
									};
        private string[] countryName = 
								{"N/A","Asia/Pacific Region","Europe","Andorra","United Arab Emirates","Afghanistan","Antigua and Barbuda","Anguilla","Albania","Armenia","Netherlands Antilles","Angola","Antarctica","Argentina","American Samoa","Austria","Australia","Aruba","Azerbaijan","Bosnia and Herzegovina","Barbados","Bangladesh","Belgium",
									"Burkina Faso","Bulgaria","Bahrain","Burundi","Benin","Bermuda","Brunei Darussalam","Bolivia","Brazil","Bahamas","Bhutan","Bouvet Island","Botswana","Belarus","Belize","Canada","Cocos (Keeling) Islands","Congo, The Democratic Republic of the","Central African Republic","Congo","Switzerland","Cote D'Ivoire",
									"Cook Islands","Chile","Cameroon","China","Colombia","Costa Rica","Cuba","Cape Verde","Christmas Island","Cyprus","Czech Republic","Germany","Djibouti","Denmark","Dominica","Dominican Republic","Algeria","Ecuador","Estonia","Egypt","Western Sahara","Eritrea","Spain","Ethiopia","Finland","Fiji","Falkland Islands (Malvinas)",
									"Micronesia, Federated States of","Faroe Islands","France","France, Metropolitan","Gabon","United Kingdom","Grenada","Georgia","French Guiana","Ghana","Gibraltar","Greenland","Gambia","Guinea","Guadeloupe","Equatorial Guinea","Greece","South Georgia and the South Sandwich Islands","Guatemala","Guam","Guinea-Bissau","Guyana",
									"Hong Kong","Heard Island and McDonald Islands","Honduras","Croatia","Haiti","Hungary","Indonesia","Ireland","Israel","India","British Indian Ocean Territory","Iraq","Iran, Islamic Republic of","Iceland","Italy","Jamaica","Jordan","Japan","Kenya","Kyrgyzstan","Cambodia","Kiribati","Comoros","Saint Kitts and Nevis",
									"Korea, Democratic People's Republic of","Korea, Republic of","Kuwait","Cayman Islands","Kazakstan","Lao People's Democratic Republic","Lebanon","Saint Lucia","Liechtenstein","Sri Lanka","Liberia","Lesotho","Lithuania","Luxembourg","Latvia","Libyan Arab Jamahiriya","Morocco","Monaco","Moldova, Republic of","Madagascar",
									"Marshall Islands","Macedonia","Mali","Myanmar","Mongolia","Macau","Northern Mariana Islands","Martinique","Mauritania","Montserrat","Malta","Mauritius","Maldives","Malawi","Mexico","Malaysia","Mozambique","Namibia","New Caledonia","Niger","Norfolk Island","Nigeria","Nicaragua","Netherlands",
									"Norway","Nepal","Nauru","Niue","New Zealand","Oman","Panama","Peru","French Polynesia","Papua New Guinea","Philippines","Pakistan","Poland","Saint Pierre and Miquelon","Pitcairn Islands","Puerto Rico","Palestinian Territory","Portugal","Palau","Paraguay","Qatar","Reunion","Romania","Russian Federation","Rwanda","Saudi Arabia",
									"Solomon Islands","Seychelles","Sudan","Sweden","Singapore","Saint Helena","Slovenia","Svalbard and Jan Mayen","Slovakia","Sierra Leone","San Marino","Senegal","Somalia","Suriname","Sao Tome and Principe","El Salvador","Syrian Arab Republic","Swaziland","Turks and Caicos Islands","Chad","French Southern Territories","Togo",
									"Thailand","Tajikistan","Tokelau","Turkmenistan","Tunisia","Tonga","Timor-Leste","Turkey","Trinidad and Tobago","Tuvalu","Taiwan","Tanzania, United Republic of","Ukraine","Uganda","United States Minor Outlying Islands","United States","Uruguay","Uzbekistan","Holy See (Vatican City State)","Saint Vincent and the Grenadines",
									"Venezuela","Virgin Islands, British","Virgin Islands, U.S.","Vietnam","Vanuatu","Wallis and Futuna","Samoa","Yemen","Mayotte","Serbia","South Africa","Zambia","Montenegro","Zimbabwe","Anonymous Proxy","Satellite Provider",
									"Other","Aland Islands","Guernsey","Isle of Man","Jersey","Saint Barthelemy","Saint Martin"};


        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public GeoIp(string fileName)
        {
            string path;

            try
            {
                path = global::Ia.Cl.Model.Default.AbsolutePath();

                fileInput = new FileStream(path + fileName, FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File " + fileName + " not found");
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string LookupCountryCode(string str)
        {
            IPAddress addr;

            try
            {
                addr = IPAddress.Parse(str);
            }
            catch (FormatException)
            {
                return "--";
            }

            return LookupCountryCode(addr);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private long IPAddressToNumber(IPAddress addr)
        {
            long ipnum = 0;
            //byte[] b = BitConverter.GetBytes(addr.Address);
            byte[] b = addr.GetAddressBytes();

            for (int i = 0; i < 4; ++i)
            {
                long y = b[i];
                if (y < 0)
                {
                    y += 256;
                }
                ipnum += y << ((3 - i) * 8);
            }
            Console.WriteLine(ipnum);
            return ipnum;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string LookupCountryCode(IPAddress addr)
        {
            return (countryCode[(int)SeekCountry(0, IPAddressToNumber(addr), 31)]);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string LookupCountryName(string str)
        {
            IPAddress addr;
            try
            {
                addr = IPAddress.Parse(str);
            }
            catch (FormatException)
            {
                return "N/A";
            }
            return LookupCountryName(addr);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string LookupCountryName(IPAddress addr)
        {
            return (countryName[(int)SeekCountry(0, IPAddressToNumber(addr), 31)]);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private long SeekCountry(long offset, long ipnum, int depth)
        {
            byte[] buf = new byte[6];
            long[] x = new long[2];
            if (depth == 0)
            {
                Console.WriteLine("Error seeking country.");
            }

            try
            {
                fileInput.Seek(6 * offset, 0);
                fileInput.Read(buf, 0, 6);
            }
            catch (IOException)
            {
                Console.WriteLine("IO Exception");
            }

            for (int i = 0; i < 2; i++)
            {
                x[i] = 0;
                for (int j = 0; j < 3; j++)
                {
                    int y = buf[i * 3 + j];
                    if (y < 0)
                    {
                        y += 256;
                    }
                    x[i] += (y << (j * 8));
                }
            }

            if ((ipnum & (1 << depth)) > 0)
            {
                if (x[1] >= COUNTRY_BEGIN)
                {
                    return x[1] - COUNTRY_BEGIN;
                }
                return SeekCountry(x[1], ipnum, depth - 1);
            }
            else
            {
                if (x[0] >= COUNTRY_BEGIN)
                {
                    return x[0] - COUNTRY_BEGIN;
                }
                return SeekCountry(x[0], ipnum, depth - 1);
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
