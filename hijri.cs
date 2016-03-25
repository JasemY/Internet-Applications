using System;
using System.Web;
using System.Diagnostics;
using System.Globalization;
using System.Data;
using System.Collections;

/*
 * Attention
 * This library has been written by: Anas Reslan Bahsas if you are going to use it please dont remove this line.
 * you have to add this class to a asp.net web project to work well. I will be grateful to receive any commments or 
 * suggestion to anasbahsas@hotmail.com
 * 
 * http://www.aawsat.com/
 */

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Hijri date handler class.
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
    public class Hijri
    {
        private static HttpContext cur;

        private const int startGreg = 1900;
        private const int endGreg = 2100;
        private static string[] allFormats = { "yyyy/MM/dd", "yyyy/M/d", "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy", "yyyy-MM-dd", "yyyy-M-d", "dd-MM-yyyy", "d-M-yyyy", "dd-M-yyyy", "d-MM-yyyy", "yyyy MM dd", "yyyy M d", "dd MM yyyy", "d M yyyy", "dd M yyyy", "d MM yyyy" };
        private static CultureInfo arCul;
        private static CultureInfo enCul;
        private static HijriCalendar h;
        private static GregorianCalendar g;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Hijri()
        {
            cur = HttpContext.Current;

            arCul = new CultureInfo("ar-SA");
            enCul = new CultureInfo("en-US");

            h = new HijriCalendar();
            g = new GregorianCalendar(GregorianCalendarTypes.USEnglish);

            arCul.DateTimeFormat.Calendar = h;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if string is hijri date and then return true 
        /// </summary>
        /// <param name="hijri"></param>
        /// <returns></returns>
        public static bool IsHijri(string hijri)
        {
            if (hijri.Length <= 0)
            {
                cur.Trace.Warn("IsHijri Error: Date String is Empty");
                return false;
            }
            try
            {
                DateTime tempDate = DateTime.ParseExact(hijri, allFormats, arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                if (tempDate.Year >= startGreg && tempDate.Year <= endGreg) return true;
                else return false;
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("IsHijri Error :" + hijri.ToString() + "\n" + ex.Message);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if string is Gregorian date and then return true 
        /// </summary>
        /// <param name="greg"></param>
        /// <returns></returns>
        public static bool IsGreg(string greg)
        {
            if (greg.Length <= 0)
            {
                cur.Trace.Warn("IsGreg :Date String is Empty");
                return false;
            }
            try
            {
                DateTime tempDate = DateTime.ParseExact(greg, allFormats, enCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                if (tempDate.Year >= startGreg && tempDate.Year <= endGreg) return true;
                else return false;
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("IsGreg Error :" + greg.ToString() + "\n" + ex.Message);
                return false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return Formatted Hijri date string 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatHijri(string date, string format)
        {
            if (date.Length <= 0)
            {
                cur.Trace.Warn("Format :Date String is Empty");
                return "";
            }
            try
            {
                DateTime tempDate = DateTime.ParseExact(date, allFormats, arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString(format, arCul.DateTimeFormat);

            }
            catch (Exception ex)
            {
                cur.Trace.Warn("Date :\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returned Formatted Gregorian date string
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatGreg(string date, string format)
        {
            if (date.Length <= 0)
            {
                cur.Trace.Warn("Format :Date String is Empty");
                return "";
            }
            try
            {
                DateTime tempDate = DateTime.ParseExact(date, allFormats, enCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString(format, enCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("Date :\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return Today Gregorian date and return it in yyyy/MM/dd format
        /// </summary>
        /// <returns></returns>
        public static string GDateNow()
        {
            try
            {
                return DateTime.Now.ToString("yyyy-MM-dd", enCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("GDateNow :\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return formatted today Gregorian date based on your format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GDateNow(string format)
        {
            try
            {
                return DateTime.Now.ToString(format, enCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("GDateNow :\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return Today Hijri date and return it in yyyy-MM-dd format
        /// </summary>
        /// <returns></returns>
        public string HDateNow()
        {
            try
            {
                return DateTime.Now.ToString("yyyy-MM-dd", arCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("HDateNow :\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return formatted today hijri date based on your format
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string HDateNow(string format)
        {
            try
            {
                return DateTime.Now.ToString(format, arCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("HDateNow :\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert Hijri Date to it's equivalent Gregorian Date
        /// </summary>
        /// <param name="hijri"></param>
        /// <returns></returns>
        public static string HijriToGreg(string hijri)
        {
            if (hijri.Length <= 0)
            {
                cur.Trace.Warn("HijriToGreg :Date String is Empty");
                return "";
            }
            try
            {
                DateTime tempDate = DateTime.ParseExact(hijri, allFormats, arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString("yyyy-MM-dd", enCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("HijriToGreg :" + hijri.ToString() + "\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert Hijri Date to it's equivalent Gregorian Date
        /// and return it in specified format
        /// </summary>
        /// <param name="hijri"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string HijriToGreg(string hijri, string format)
        {
            if (hijri.Length <= 0)
            {
                cur.Trace.Warn("HijriToGreg :Date String is Empty");
                return "";
            }
            try
            {
                DateTime tempDate = DateTime.ParseExact(hijri, allFormats, arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString(format, enCul.DateTimeFormat);

            }
            catch (Exception ex)
            {
                cur.Trace.Warn("HijriToGreg :" + hijri.ToString() + "\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert Gregoian Date to it's equivalent Hijir Date
        /// </summary>
        /// <param name="greg"></param>
        /// <returns></returns>
        public static string GregToHijri(string greg)
        {
            if (greg.Length <= 0)
            {
                cur.Trace.Warn("GregToHijri :Date String is Empty");
                return "";
            }
            try
            {
                DateTime tempDate = DateTime.ParseExact(greg, allFormats, enCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString("yyyy-MM-dd", arCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("GregToHijri :" + greg.ToString() + "\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert Hijri Date to it's equivalent Gregorian Date and
        /// return it in specified format
        /// </summary>
        /// <param name="greg"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GregToHijri(string greg, string format)
        {
            if (greg.Length <= 0)
            {
                cur.Trace.Warn("GregToHijri :Date String is Empty");
                return "";
            }
            try
            {
                DateTime tempDate = DateTime.ParseExact(greg, allFormats, enCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return tempDate.ToString(format, arCul.DateTimeFormat);
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("GregToHijri :" + greg.ToString() + "\n" + ex.Message);
                return "";
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return Gregrian Date Time as digit stamp
        /// </summary>
        /// <returns></returns>
        public static string GTimeStamp()
        {
            return GDateNow("yyyyMMddHHmmss");
        }

        ////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Return Hijri Date Time as digit stamp
        /// </summary>
        /// <returns></returns>
        public static string HTimeStamp()
        {
            return HDateNow("yyyyMMddHHmmss");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Compare two instaces of string date 
        /// and return indication of thier values 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns>positive d1 is greater than d2,
        /// negative d1 is smaller than d2, 0 both are equal</returns>
        public static int Compare(string d1, string d2)
        {
            try
            {
                DateTime date1 = DateTime.ParseExact(d1, allFormats, arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                DateTime date2 = DateTime.ParseExact(d2, allFormats, arCul.DateTimeFormat, DateTimeStyles.AllowWhiteSpaces);
                return DateTime.Compare(date1, date2);
            }
            catch (Exception ex)
            {
                cur.Trace.Warn("Compare :" + "\n" + ex.Message);
                return -1;
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}

