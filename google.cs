using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;

namespace Ia.Cl.Model.Google
{
    /// <summary publish="true">
    /// Google support class.
    /// </summary>
    /// <value>
    /// Class to generate a static map using the Google StaticMaps API
    /// http://code.google.com/apis/maps/documentation/staticmaps/
    /// There is an article supporting this code available at 
    /// http://www.codeproject.com/KB/aspnet/csharp-google-static-map.aspx
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
    public class StaticMap
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Renders an image for display
        /// </summary>
        /// <returns></returns>
        /// </summary>
        /// <remarks> Primarily this just creates an ImageURL string</remarks>
        public string Render()
        {
            string qs = "http://maps.google.com/staticmap?center={0},{1}&zoom={2}&size={3}x{4}&maptype={5}";
            string mkqs = "";

            qs = string.Format(qs, LatCenter, LngCenter, Zoom, Width, Height, Type.ToString().ToLower());

            // add markers
            foreach (var marker in _markers)
            {

                mkqs += string.Format("{0},{1},{2}|",
                    marker.Lat,
                    marker.Lng,
                    GetMarkerParams(marker.Size, marker.Color, marker.Character));
            }

            if (mkqs.Length > 0)
            {
                qs += "&markers=" + mkqs.Substring(0, (mkqs.Length - 1));
            }

            qs += "&key=" + APIKey;

            return qs;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Build the correct string for marker parameters
        /// </summary>
        /// <returns></returns>
        /// </summary>
        /// <remarks> 
        /// (Some marker sizes such as 'tiny' won't accept character values, 
        ///    this function makes sure they'll be rendered even if the inputed parameters are wrong
        /// </remarks>
        private static string GetMarkerParams(mSize size, mColor color, string character)
        {
            string marker;

            // check if can render character
            if ((size == mSize.Normal) || (size == mSize.Mid))
            {
                if (size == mSize.Normal)
                {
                    marker = color.ToString().ToLower() + character;
                }
                else
                {
                    marker = size.ToString().ToLower() + color.ToString().ToLower() + character;
                }
            }
            else
            {
                // just render size and color - character not supported
                marker = size.ToString().ToLower() + color.ToString().ToLower();
            }

            return marker;

        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Defines a single map point to be added to a map
        /// </summary>
        public class Marker
        {

            private string _char = "";

            /// <summary>
            /// Optional single letter character
            /// </summary>
            public string Character
            {
                get { return _char; }
                set { _char = value; }
            }

            #region Auto-Properties

            /// <summary/>
            public Double Lat { get; set; }

            /// <summary/>
            public Double Lng { get; set; }

            /// <summary/>
            public StaticMap.mSize Size { get; set; }

            /// <summary/>
            public StaticMap.mColor Color { get; set; }

            #endregion

        }

        #region Marker enums

        /// <summary/>
        public enum mFormat
        {
            /// <summary/>
            gif = 0,

            /// <summary/>
            jpg = 1,

            /// <summary/>
            png = 2
        }

        /// <summary/>
        public enum mSize
        {
            /// <summary/>
            Normal = 0,

            /// <summary/>
            Mid = 1,

            /// <summary/>
            Small = 2,

            /// <summary/>
            Tiny = 3
        }

        /// <summary/>
        public enum mColor
        {
            /// <summary/>
            Black = 0,

            /// <summary/>
            Brown = 1,

            /// <summary/>
            Green = 2,

            /// <summary/>
            Purple = 3,

            /// <summary/>
            Yellow = 4,

            /// <summary/>
            Blue = 5,

            /// <summary/>
            Gray = 6,

            /// <summary/>
            Orange = 7,

            /// <summary/>
            Red = 8,

            /// <summary/>
            White = 9
        }

        /// <summary/>
        public enum mType
        {
            /// <summary/>
            Roadmap = 0,

            /// <summary/>
            Mobile = 1
        }

        #endregion

        /// <summary>
        /// StaticMap props
        /// </summary>
        #region Properties

        private List<Marker> _markers = new List<Marker>();
        private StaticMap.mType _type = StaticMap.mType.Roadmap;

        /// <summary>
        /// List of all markers to be displayed on the map
        /// </summary>
        public List<Marker> Markers
        {
            get { return _markers; }
            set { _markers = value; }
        }

        /// <summary/>
        public StaticMap.mType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// Google maps API key - required!
        /// </summary>
        private static string APIKey
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleAPIKey"]; ;
            }
        }

        #region Auto-Properties

        /// <summary/>
        public Double LatCenter { get; set; }

        /// <summary/>
        public Double LngCenter { get; set; }

        /// <summary/>
        public int Zoom { get; set; }

        /// <summary/>
        public int Width { get; set; }

        /// <summary/>
        public int Height { get; set; }

        #endregion

        #endregion

    }


    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Generic helper functions
    /// </summary>
    public class Tools
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Converts Integers to enum types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Enum int value</param>
        /// <returns></returns>
        /// <example>
        /// Enums.ConvertToEnum enum.type ([EnumAsInt]);
        /// </example>
        public static T ConvertToEnum<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Converts String to enum types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Enum string value</param>
        /// <returns></returns>
        /// <example>
        /// Enums.ConvertToEnum([EnumAsString]);
        /// </example>

        public static T ConvertToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
