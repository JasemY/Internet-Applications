using System;
using System.IO;
using System.Data;
using System.Globalization;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Deployment;
using System.Net;
using System.Linq;
using System.ComponentModel;

#if WINDOWS_FORM
#else
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Caching;
#endif

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// General use static class of common functions used by most applications.
    /// 
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
    public static class Default
    {
        private static Random random = new Random();

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Substring(string str, int len)
        {
            if (str.Length >= len - 3) str = str.Substring(0, len - 3) + "... ";
            else if (str.Length > 0) str = str.Substring(0, str.Length);
            else str = "";

            return str;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Url(string text, string url)
        {
            return "<a href=" + url + ">" + text + "</a>";
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string YesNo(bool checkValue)
        {
            string text;

            if (checkValue) text = "<span class=\"yes\">Yes</span>";
            else text = "<span class=\"no\">No</span>";

            return text;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string YesNoInArabic(bool checkValue)
        {
            string text;

            if (checkValue) text = "<span class=\"yes\">نعم</span>";
            else text = "<span class=\"no\">لا</span>";

            return text;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ActiveIdle(object o)
        {
            bool b;
            string s;

            if (o != null && o.ToString().Length > 0)
            {
                b = (bool)o;

                if (b) s = "<span style=\"color:Green\">Active</span>";
                else s = "<span style=\"color:DarkGoldenRod\">Idle</span>";
            }
            else s = "";

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ActiveIdleInArabic(object o)
        {
            bool b;
            string s;

            if (o != null && o.ToString().Length > 0)
            {
                b = (bool)o;

                if (b) s = "<span style=\"color:Green\">فعال</span>";
                else s = "<span style=\"color:DarkGoldenRod\">مطفأ</span>";
            }
            else s = "";

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return a random number n where maxValue > n >= 0
        /// <param name="maxValue">integer</param>
        /// <returns>int where maxValue > n >= 0</returns>
        /// </summary>
        public static int Random(int maxValue)
        {
            int num;

            if(random == null) random = new Random();

            num = random.Next(maxValue);

            return num;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return a random number n, with seed, where ra > num >= 0
        /// <param name="seed">integer</param>
        /// <param name="ra">integer</param>
        /// <returns>int where ra > num >=0</returns>
        /// </summary>
        public static int RandomWithSeed(int seed, int ra)
        {
            // return a random number num: ra > num >= 0
            int num;

            random = new Random(seed);

            num = random.Next(ra);
            return num;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return a random number n where r1 > num >= r0
        /// <param name="r1">integer max</param>
        /// <param name="r0">integer min</param>
        /// <returns>int where r1 >= num >= r0</returns>
        /// </summary>
        public static int Random(int r0, int r1)
        {
            // return a random number num: r1 >= num >= r0
            int num;

            if (random == null) random = new Random();

            num = random.Next(r1) + r0;
            return num;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return a pseudo-random item from list
        /// </summary>
        public static string Random(string s)
        {
            // take a list of comma seperated values in s and return one pseudo-random value
            int n;
            string[] sp;

            if (random == null) random = new Random();

            sp = s.Split(',');

            n = Random(sp.Length);

            return sp[n].ToString();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool RandomBoolean()
        {
            // return a random boolean value

            bool b;
            int n;

            if (random == null) random = new Random();

            n = random.Next(2);

            if (n == 1) b = true;
            else b = false;

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string RandomPassword(int num)
        {
            // return a random password of numbers and letters of length num
            return "12345678";
        }

        /*
        // Note, max is exclusive here!
        public static List<int> GenerateRandom(int count, int min, int max)
        {

            // initialize set S to empty
            // for J := N-M + 1 to N do
            //   T := RandInt(1, J)
            //   if T is not in S then
            //     insert T in S
            //   else
            //     insert J in S
            //
            // adapted for C# which does not have an inclusive Next(..)
            // and to make it from configurable range not just 1.

            if (max <= min || count < 0 ||
                // max - min > 0 required to avoid overflow
                    (count > max - min && max - min > 0))
            {
                // need to use 64-bit to support big ranges (negative min, positive max)
                throw new ArgumentOutOfRangeException("Range " + min + " to " + max +
                        " (" + ((Int64)max - (Int64)min) + " values), or count " + count + " is illegal");
            }

            // generate count random values.
            HashSet<int> candidates = new HashSet<int>();

            // start count values before max, and end at max
            for (int top = max - count; top < max; top++)
            {
                // May strike a duplicate.
                // Need to add +1 to make inclusive generator
                // +1 is safe even for MaxVal max value because top < max
                if (!candidates.Add(random.Next(min, top + 1)))
                {
                    // collision, add inclusive max.
                    // which could not possibly have been added before.
                    candidates.Add(top);
                }
            }

            // load them in to a list, to sort
            List<int> result = candidates.ToList();

            // shuffle the results because HashSet has messed
            // with the order, and the algorithm does not produce
            // random-ordered results (e.g. max-1 will never be the first value)
            for (int i = result.Count - 1; i > 0; i--)
            {
                int k = random.Next(i + 1);
                int tmp = result[k];
                result[k] = result[i];
                result[i] = tmp;
            }
            return result;
        }

        public static List<int> GenerateRandom(int count)
        {
            return GenerateRandom(count, 0, Int32.MaxValue);
        }
         */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<int> RandomList(int start, int end, int count)
        {
            int n;
            Random random;
            List<int> list;
            HashSet<int> hashset;
            
            random = new Random();
            hashset = new HashSet<int>();
            list = new List<int>();

            while (list.Count < count)
            {
                n = random.Next(start, end);

                if (!hashset.Contains(n))
                {
                    hashset.Add(n);
                    list.Add(n);
                }
            }

            list.Sort();

            return list;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string TickDateTime(string tick)
        {
            // reads a tick count and returns the date time as string
            string line;

            try
            {
                DateTime t = new DateTime(long.Parse(tick));
                line = t.ToString("dd/MM/yyyy HH:mm");
            }
            catch (Exception)
            {
                line = "";
            }

            return line;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static long DateTimeSecond(DateTime dt)
        {
            // return the number of seconds (total) in datetime
            long l;

            // A single tick represents one hundred nanoseconds or one ten-millionth of a second

            l = dt.Ticks / 10000000;

            return l;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the number of seconds (total) in datetime
        /// </summary>
        public static long DateTimeSecond()
        {
            long l;

            // A single tick represents one hundred nanoseconds or one ten-millionth of a second

            l = (long)(DateTime.Now.Ticks / 10000000);

            return l;
        }

#if WINDOWS_FORM
#else
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void InitializeDropDownList(DropDownList ddl, DataTable source_dt, string text_field, string value_field, int selected_index)
        {
            int index;

            if (selected_index == -1) index = ddl.SelectedIndex;
            else index = selected_index;

            ddl.Items.Clear();
            //ddl.Items.Add(new ListItem("XXX","0"));
            ddl.DataSource = source_dt;
            ddl.DataTextField = text_field;
            ddl.DataValueField = value_field;
            ddl.DataBind();
            ddl.SelectedIndex = index;
        }
#endif
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool IsFloat(string line)
        {
            // this check if line is floating point number. 
            bool ni = false;

            try { float x = float.Parse(line); ni = true; }
            catch (Exception) { ni = false; }

            return ni;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool IsDecimal(string line)
        {
            // this check if line is a decimal number. 
            bool ni = false;

            try { decimal x = decimal.Parse(line); ni = true; }
            catch (Exception) { ni = false; }

            return ni;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool IsInt(string line)
        {
            // this check if line is an integer
            bool ni = false;

            try { int x = int.Parse(line); ni = true; }
            catch (Exception) { ni = false; }

            return ni;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ByteToHex(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(2500);

            UTF8Encoding utf8 = new UTF8Encoding();
            foreach (byte b in bytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static byte[] HexToByte(string hex_str)
        {
            int i, n;
            byte[] bytes;
            char[] chars;
            string c_str = "";
            UTF8Encoding utf8 = new UTF8Encoding();

            chars = hex_str.ToCharArray();

            bytes = new byte[chars.Length / 2];  // since hex_str has two chars for every byte

            n = 0;
            for (i = 0; i < chars.Length; i += 2)
            {
                c_str = chars[i].ToString() + chars[i + 1].ToString();
                bytes[n++] = (byte)Convert.ToInt32(c_str, 16);
            }

            return bytes;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string DecToHex(int dec)
        {
            uint uiDecimal = 0;
            string hex;

            try
            {
                // convert text string to unsigned integer
                uiDecimal = checked((uint)System.Convert.ToUInt32(dec));

                hex = String.Format("{0:x2}", uiDecimal);
            }
            catch (System.OverflowException)
            {
                // Show overflow message and return
                hex = "";
            }

            return hex;

        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static int HexToDec(string hex)
        {
            // To hold our converted unsigned integer32 value
            uint dec;

            try
            {
                // Convert hex string to unsigned integer
                dec = System.Convert.ToUInt32(hex, 16);
            }
            catch (System.OverflowException)
            {
                // Show overflow message and return
                return 0;
            }

            return (int)dec;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static DataTable GenerateEmptyDataTable(DataTable dt)
        {
            // this function is used to produce a single empty DataTable line to make the GridView look nicer.
            // this will simply clone the in_dt and create a completly empty line
            DataRow dr;

            if (dt.Rows.Count == 0)
            {

                try
                {
                    dr = dt.NewRow();

                    foreach (DataColumn dc in dt.Columns)
                    {
                        dr[dc.ColumnName] = DBNull.Value;
                    }

                    dt.Rows.Add(dr);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return dt;
        }

#if WINDOWS_FORM
#else
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public static string GetPostBackControl(Page page)
        {
            // return the name of control that fired the postback
            // this is strange, sometimes it fills the ID with real name of control and sometimes the TEXT

            string s = "";
            Control c = null;

            string ctrlname = page.Request.Params.Get("__EVENTTARGET");
            if (ctrlname != null && ctrlname != string.Empty)
            {
                c = page.FindControl(ctrlname);
            }
            else
            {
                foreach (string ctl in page.Request.Form)
                {
                    Control ci = page.FindControl(ctl);
                    if (ci is System.Web.UI.WebControls.Button)
                    {
                        c = ci;
                        break;
                    }
                }
            }

            if (c != null)
            {
                if (c.GetType().ToString() == "System.Web.UI.WebControls.Button")
                {
                    s = ((System.Web.UI.WebControls.Button)c).ID;
                }
                else if (c.GetType().ToString() == "System.Web.UI.WebControls.DropDownList")
                {
                    s = ((System.Web.UI.WebControls.DropDownList)c).ID;
                }
                else s = "";
            }
            else s = "";

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////
#endif
        /// <summary>
        ///
        /// </summary>
        public static string Color(int count, int index)
        {
            string s;

            // rainbow:
            string[] color = { "#f00", "#f10", "#f20", "#f30", "#f40", "#f50", "#f60", "#f70", "#f80", "#f90", "#fa0", "#fb0", "#fc0", "#fd0", "#fe0", "#ff0", "#ef0", "#df0", "#cf0", "#bf0", "#af0", "#9f0", "#8f0", "#7f0", "#6f0", "#5f0", "#4f0", "#3f0", "#2f0", "#1f0", "#0f0", "#0f1", "#0f2", "#0f3", "#0f4", "#0f5", "#0f6", "#0f7", "#0f8", "#0f9", "#0fa", "#0fb", "#0fc", "#0fd", "#0fe", "#0ff", "#0ef", "#0df", "#0cf", "#0bf", "#0af", "#09f", "#08f", "#07f", "#06f", "#05f", "#04f", "#03f", "#02f", "#01f", "#00f", "#10f", "#20f", "#30f", "#40f", "#50f", "#60f", "#70f", "#80f", "#90f", "#a0f", "#b0f", "#c0f", "#d0f", "#e0f" };

            // random clear
            //string[] color = {"Black","Blue","BlueViolet","Brown","CadetBlue","CornFlowerBlue","Crimson","DarkBlue","DarkCyan","DarkGoldenRod","DarkGreen","DarkMagenta","DarkOliveGreen","DarkOrange","DarkOrchid","DarkRed","DarkSlateBlue","DarkSlateGray","DarkViolet","Firebrick","ForestGreen","Green","IndianRed","Indigo","LightGray","LightSeaGreen","LightSkyBlue","LightSlateGray","Maroon","MediumBlue","MediumOrchid","MediumPurple","MediumSeaGreen","MediumSlateBlue","MediumVioletRed","MidnightBlue","Navy","Olive","OliveDrab"," Orange","OrangeRed","Orchid","Purple","Red","RosyBrown","RoyalBlue","SaddleBrown","SlateBlue","SlateGray","Teal","Tomato","Transparent" };

            if (index > 0) index--;

            s = color[color.Length / count * index];

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string FormatHtml(string text)
        {
            text = Regex.Replace(text, @"[\n|\r]+", "</p><p>");
            text = "<p>" + text + "</p>";

            return text;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool IsValidIp(string ip)
        {
            bool isValid;

            IPAddress address;

            isValid = IPAddress.TryParse(ip, out address);

            //if (Regex.IsMatch(ip, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")) b = true;
            //else b = false;


            return isValid;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static long IpToLong(string ip)
        {
            string delimStr = ".";
            char[] delimiter = delimStr.ToCharArray();
            string[] ip_addr = null;
            long ip_num = 0;

            try
            {
                ip_addr = ip.Split(delimiter, 4);
                ip_num = (long.Parse(ip_addr[0]) * 16777216) + (long.Parse(ip_addr[1]) * 65536) + (long.Parse(ip_addr[2]) * 256) + (long.Parse(ip_addr[3]));
            }
            catch (Exception)
            {
            }

            return ip_num;

            /*
            long l;
            string r;
            Match m;

            m = Regex.Match(ip, @"(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})");

            if (m.Success)
            {
                r = m.Groups[1].Captures[0].Value.PadLeft(3, '0') + m.Groups[2].Captures[0].Value.PadLeft(3, '0') + m.Groups[3].Captures[0].Value.PadLeft(3, '0') + m.Groups[4].Captures[0].Value.PadLeft(3, '0');
                l = long.Parse(r);
            }
            else l = 0;

            return l;
            */
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string LongToIp(long l)
        {
            string s, s1, s2, s3, s4;

            s = l.ToString();
            s = s.PadLeft(12, '0');

            s1 = s.Substring(0, 3);
            s2 = s.Substring(3, 3);
            s3 = s.Substring(6, 3);
            s4 = s.Substring(9, 3);

            s = s1 + "." + s2 + "." + s3 + "." + s4;

            s = Regex.Replace(s, @"\.0+", ".");
            s = Regex.Replace(s, @"^0+", "");
            if (s[s.Length - 1] == '.') s = s + "0";

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string IpToHex(string ip)
        {
            string h;
            Match m;

            h = "";

            m = Regex.Match(ip, @"(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})");

            if (m.Success)
            {
                h = DecToHex(int.Parse(m.Groups[1].Captures[0].Value)) + DecToHex(int.Parse(m.Groups[2].Captures[0].Value)) + DecToHex(int.Parse(m.Groups[3].Captures[0].Value)) + DecToHex(int.Parse(m.Groups[4].Captures[0].Value));
            }
            else h = "";

            return h;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string HexToIp(string h)
        {
            string s, s1, s2, s3, s4;

            h = h.PadLeft(8, '0');

            s1 = h.Substring(0, 2);
            s2 = h.Substring(2, 2);
            s3 = h.Substring(4, 2);
            s4 = h.Substring(6, 2);

            s = HexToDec(s1) + "." + HexToDec(s2) + "." + HexToDec(s3) + "." + HexToDec(s4);

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string DecToIp(int i)
        {
            return HexToIp(DecToHex(i));
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static int IpToDec(string s)
        {
            return HexToDec(IpToHex(s));
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string NetworkNumberFromIp(string ip)
        {
            string[] ipp;
            string networkNumber;

            networkNumber = "";

            if (IsValidIp(ip))
            {
                ipp = ip.Split('.');

                networkNumber = (ipp[0] + "." + ipp[1] + "." + ipp[2] + ".0");
            }

            return networkNumber;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<string> NetworkNumberStrippedList(string startIp, string endIp)
        {
            int startIpI, endIpI;
            string s;
            List<long> li;
            List<string> list;

            li = new List<long>();
            list = new List<string>();

            // change network number to a real IP
            if (Regex.IsMatch(startIp, @"^.+\.0$")) startIp = Regex.Replace(startIp, @"^(.+)\.0$", "$1.1");
            if (Regex.IsMatch(endIp, @"^.+\.0$")) endIp = Regex.Replace(endIp, @"^(.+)\.0$", "$1.1");

            startIpI = IpToDec(startIp);
            endIpI = IpToDec(endIp);

            for (int i = startIpI; i <= endIpI; i += 256)
            {
                s = DecToIp(i);
                s = NetworkNumberFromIp(s);

                s = Regex.Replace(s, @"^(.+)\.0$", "$1");

                list.Add(s);
            }

            return list;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Hashtable DataTableToHashtable(DataTable dt)
        {
            // put the datatable first row value into a hashtable key, and second as value. if the table has only one column we will add it only to keys with 0 value
            Hashtable ht;

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {

                    ht = new Hashtable(dt.Rows.Count);

                    if (dt.Columns.Count == 1) foreach (DataRow r in dt.Rows) ht[r[0].ToString()] = "0";
                    else if (dt.Columns.Count > 1) foreach (DataRow r in dt.Rows) ht[r[0].ToString()] = r[1].ToString();
                }
                else ht = new Hashtable(1);
            }
            else ht = null;

            return ht;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string DataTableToString(DataTable dataTable)
        {
            var output = new StringBuilder();

            // write Column titles
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var text = dataTable.Columns[i].ColumnName;
                output.Append("\t" + text);
            }

            output.Append("|\n" + new string('=', output.Length) + "\n");

            // write rows
            foreach (DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var text = row[i].ToString();
                    output.Append("\t" + text);
                }

                output.Append("|\n");
            }

            return output.ToString();
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static ArrayList ArrayList_Limit_Randomize(ArrayList in_al, int max)
        {
            // 
            // parameter: ArrayList with any number of entries, an integer value indicating the max possible number of returned values
            // procedure: randomly select upto max values from al and return max >= num >= 0

            int n, o;
            ArrayList al;
            Hashtable ht;
            Random r;
            
            r = new Random();

            if (max > 0)
            {
                al = new ArrayList(max);
                ht = new Hashtable(max);

                o = 0;

                while (o < in_al.Count - 1 && o < max)
                {
                    foreach (string s in in_al)
                    {
                        n = r.Next(max);

                        if (!ht.ContainsKey(n))
                        {
                            al.Add(s);
                            ht[n] = 1;
                            o++;
                        }
                    }
                }

            }
            else al = null;

            return al;
        }
        */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static ArrayList SublistArrayList(ArrayList in_al, int n)
        {
            // return the first n values from all
            ArrayList al;

            if (n > 0)
            {
                al = new ArrayList(n);

                for (int i = 0; i < in_al.Count - 1 && i < n; i++)
                {
                    al.Add(in_al[i]);
                }
            }
            else al = null;

            return al;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static ArrayList ShuffleAndSublistArrayList(ArrayList in_al, int n)
        {
            // 

            ShuffleArrayList(in_al);

            return SublistArrayList(in_al, n);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static ArrayList KeyArrayHashtableToList(Hashtable ht)
        {
            // 
            ArrayList al;

            if (ht != null)
            {
                if (ht.Count > 0)
                {
                    al = new ArrayList(ht.Count);

                    foreach (string s in ht.Keys) al.Add(s);
                }
                else al = new ArrayList(1);
            }
            else al = null;

            al.Sort();

            return al;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static ArrayList KeyIntegerHashtableToArrayList(Hashtable ht)
        {
            // 
            ArrayList al;

            if (ht != null)
            {
                if (ht.Count > 0)
                {
                    al = new ArrayList(ht.Count);

                    foreach (int i in ht.Keys) al.Add(i);
                }
                else al = new ArrayList(1);
            }
            else al = null;

            al.Sort();

            return al;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Hashtable ReverseKeyValueInHashtable(Hashtable in_ht)
        {
            // 
            Hashtable ht;

            if (in_ht != null)
            {
                if (in_ht.Count > 0)
                {
                    ht = new Hashtable(in_ht.Count);

                    foreach (string s in in_ht.Keys) ht[in_ht[s].ToString()] = s;
                }
                else ht = new Hashtable(1);
            }
            else ht = null;

            return ht;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static ArrayList HashtableValueToArrayList(Hashtable ht)
        {
            // 
            ArrayList al;

            if (ht != null)
            {
                if (ht.Count > 0)
                {
                    al = new ArrayList(ht.Count);

                    foreach (string s in ht.Values) al.Add(s);
                }
                else al = new ArrayList(1);
            }
            else al = null;

            al.Sort();

            return al;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string HashtableKeyString(Hashtable ht)
        {
            // 
            string si;
            StringBuilder sb;

            if (ht != null)
            {
                if (ht.Count > 0)
                {
                    sb = new StringBuilder(ht.Count);
                    sb.Length = 0;

                    foreach (string s in ht.Keys) sb.Append(s + "|");
                }
                else sb = new StringBuilder(1);
            }
            else sb = null;

            si = sb.ToString();
            si = si.Remove(si.Length - 1, 1);

            return si;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Hashtable SortHashtableKey(Hashtable in_ht)
        {
            // sort the hashtable keys alphabetically

            ArrayList al;
            Hashtable ht;

            if (in_ht != null)
            {
                if (in_ht.Count > 0)
                {
                    al = new ArrayList(in_ht.Count + 1);
                    ht = new Hashtable(in_ht.Count + 1);

                    al.Clear();
                    foreach (string s in in_ht.Keys) al.Add(s);
                    al.Sort();
                    foreach (string s in al) ht.Add(s, in_ht[s].ToString());
                }
                else ht = in_ht;
            }
            else ht = in_ht;

            return ht;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string HashtableValueString(Hashtable ht)
        {
            // 
            string si;
            StringBuilder sb;

            if (ht != null)
            {
                if (ht.Count > 0)
                {
                    sb = new StringBuilder(ht.Count);
                    sb.Length = 0;

                    foreach (string s in ht.Keys) sb.Append(ht[s].ToString() + "|");
                }
                else sb = new StringBuilder(1);
            }
            else sb = null;

            si = sb.ToString();
            si = si.Remove(si.Length - 1, 1);

            return si;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public static string Match(string s, string regex)
        {
            string t;
            Match m;

            m = Regex.Match(s, regex);
            if (m.Groups[1].Success) t = m.Groups[1].Captures[0].Value;
            else t = null;

            return t;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public static string MatchToLower(string s, string regex)
        {
            string t;
            Match m;

            m = Regex.Match(s, regex);
            if (m.Groups[1].Success) t = m.Groups[1].Captures[0].Value.ToLower();
            else t = null;

            return t;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool IsRegexPatternValid(string pattern)
        {
            bool b;

            try
            {
                new Regex(pattern);

                b = true;
            }
            catch
            {
                b = false;
            }

            return b;
        }

#if WINDOWS_FORM
#else
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Store the current time in a cache variable
        /// </summary>
        public static void SetTimeSpan()
        {
            HttpRuntime httpRT = new HttpRuntime();
            Cache cache = HttpRuntime.Cache;

            cache["TimeSpan_Set"] = DateTime.UtcNow.AddHours(3).Ticks;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if sec seconds had passed since timespan_set
        /// </summary>
        public static bool CheckTimeSpan(int sec)
        {
            bool b;
            long l;
            HttpRuntime httpRT = new HttpRuntime();
            Cache cache = HttpRuntime.Cache;

            if (cache["TimeSpan_Set"] == null) b = true;
            else
            {
                l = (long)cache["TimeSpan_Set"];

                if (DateTime.UtcNow.AddHours(3).AddSeconds(-sec).Ticks > l) b = true;
                else b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if 1 sec seconds had passed since timespan_set
        /// </summary>
        public static bool CheckTimeSpan()
        {
            return CheckTimeSpan(1);
        }

#endif
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the absolute path
        /// </summary>
        public static string AbsolutePath()
        {
            return AbsolutePath(false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the absolute path to temp folder
        /// </summary>
        public static string AbsoluteTempPath()
        {
            return AbsolutePath(true);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the absolute path
        /// </summary>
        public static string AbsolutePath(bool temp_folder)
        {
            string path;

#if WINDOWS_FORM
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) path = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DataDirectory + @"\";
            else path = AppDomain.CurrentDomain.BaseDirectory;
#else
            if (temp_folder) path = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"app_data\temp\";
            else path = AppDomain.CurrentDomain.BaseDirectory.ToString();
#endif

            //if (path.IndexOf(@"\bin") >= 0) path = path.Remove(path.IndexOf(@"\bin"), path.Length - path.IndexOf(@"\bin"));

            return path;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the absolute path parent directory
        /// </summary>
        public static string AbsolutePathParent()
        {
            string s;

            s = AbsolutePath(false);

            s = s.Remove(s.Length - 1, 1);

            s = s.Substring(0, s.LastIndexOf(@"\")) + @"\";

            return s;
        }

#if WINDOWS_FORM
#else
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the absolute URL
        /// </summary>
        public static string AbsoluteUrl(Page p)
        {
            string url;
            Uri uri;

            uri = p.Request.Url;

            if (uri.Host == "localhost")
            {
                url = uri.Authority; // +@"/" + uri.Segments[1].Replace("/", "");
                url = url.ToLower();
                url = url.Replace("http://", "");
                url = url.Replace("www.", "");
            }
            else
            {
                url = uri.Host;
                url = url.ToLower();
                url = url.Replace("http://", "");
                url = url.Replace("www.", "");
            }

            return url;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the URL of a file from the path
        /// </summary>
        public static string AbsolutePathUrl(Page page, string file)
        {
            string s, absolute_url, absolute_path;

            absolute_url = AbsoluteUrl(page) + "/";
            absolute_path = AbsolutePath();

            s = file.Replace(absolute_path, absolute_url);

            s = s.Replace(@"\", @"/");

            return page.ResolveClientUrl("~/" + s);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Shows a client-side JavaScript alert in the browser.
        /// </summary>
        public static void JavasciptSrc(Page p, string relative_url_file)
        {
            // cleans the message to allow single quotation marks
            string script;

            relative_url_file = relative_url_file.Replace("'", "\\'");

            script = "<script type=\"text/javascript\" src=\"" + global::Ia.Cl.Model.Default.AbsoluteUrl(p) + @"/" + relative_url_file + "\"/>";

            // gets the executing web page
            Page page = HttpContext.Current.CurrentHandler as Page;

            // checks if the handler is a Page and that the script isn't allready on the Page
            if (page != null && !page.ClientScript.IsClientScriptBlockRegistered("alert"))
            {
                page.ClientScript.RegisterClientScriptBlock(typeof(string), "alert", script);
            }
        }
#endif
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Make the first letter of a word an upper letter
        /// </summary>
        public static string FirstLetterToUpper(string s)
        {
            string u;

            u = s.Substring(0, 1);
            return u.ToUpper() + s.Remove(0, 1);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Make the first letter of all words an upper letter and remove underscores
        /// </summary>
        public static string FirstWordLetterToUpperAndRemoveUnderscore(string line)
        {
            string u, v;

            v = "";

            line = RemoveUnderscore(line);

            foreach (string s in line.Split(' '))
            {
                u = s.Substring(0, 1);
                v += u.ToUpper() + s.Remove(0, 1) + " ";
            }

            if (v.Length > 0) v = v.Remove(v.Length - 1, 1);

            return v;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Remove underscores
        /// </summary>
        public static string RemoveUnderscore(string line)
        {
            string u;

            u = line.Replace('_', ' ');

            return u;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Regex that defines email
        /// </summary>
        public static string EmailRegex()
        {
            // http://www.regular-expressions.info/email.html
            string s;

            s = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+(?:[A-Z]{2}|com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum)\b";

            // Text="Invalid e-mail" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" runat="server" />

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool IsEmail(string s)
        {
            // return true if argument is an email
            bool b;

            b = false;

            if (Regex.IsMatch(s, global::Ia.Cl.Model.Default.EmailRegex(), RegexOptions.IgnoreCase)) b = true;

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Decimal(double d, int dec)
        {
            // 
            string s;

            if (dec == 3) s = string.Format("{0:0.000}", d);
            else if (dec == 2) s = string.Format("{0:0.00}", d);
            else if (dec == 1) s = string.Format("{0:0.0}", d);
            else s = d.ToString();

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Reverse a string
        /// </summary>
        public static string ReverseString(string t)
        {
            string s;

            s = "";

            foreach (char c in t.ToCharArray()) s = c + " " + s;

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if the passed IP address belongs to Kuwait
        /// </summary>
        public static bool IsKuwaitIp(string ip)
        {
            bool b;
            long l;

            l = global::Ia.Cl.Model.Default.IpToLong(ip);

            if (
            (l >= 1044742144 && l <= 1044750335)
            || (l >= 1050017792 && l <= 1050083327)
            || (l >= 1054277632 && l <= 1054343167)
            || (l >= 1075513152 && l <= 1075513183)
            || (l >= 1125110400 && l <= 1125110463)
            || (l >= 1161630912 && l <= 1161630919)
            || (l >= 1161641888 && l <= 1161641911)
            || (l >= 1163558016 && l <= 1163558028)
            || (l >= 1308094464 && l <= 1308096511)
            || (l >= 1314455552 && l <= 1314521087)
            || (l >= 1318764544 && l <= 1318780927)
            || (l >= 1319084032 && l <= 1319092223)
            || (l >= 1347219456 && l <= 1347223551)
            || (l >= 1347294424 && l <= 1347294431)
            || (l >= 1347321856 && l <= 1347323775)
            || (l >= 1347323904 && l <= 1347325183)
            || (l >= 1347325440 && l <= 1347325951)
            || (l >= 1354235904 && l <= 1354301439)
            || (l >= 1361035904 && l <= 1361035907)
            || (l >= 1361036764 && l <= 1361036767)
            || (l >= 1361036792 && l <= 1361036795)
            || (l >= 1365220792 && l <= 1365220799)
            || (l >= 1383273984 && l <= 1383276543)
            || (l >= 1383367168 && l <= 1383367679)
            || (l >= 1383368848 && l <= 1383368895)
            || (l >= 1383369120 && l <= 1383369231)
            || (l >= 1383369248 && l <= 1383369535)
            || (l >= 1383369600 && l <= 1383370751)
            || (l >= 1383371776 && l <= 1383374591)
            || (l >= 1385283584 && l <= 1385291775)
            || (l >= 1397006336 && l <= 1397014527)
            || (l >= 1398800384 && l <= 1398833151)
            || (l >= 1403658528 && l <= 1403658559)
            || (l >= 1410013696 && l <= 1410013727)
            || (l >= 1410013920 && l <= 1410013951)
            || (l >= 1410014016 && l <= 1410014047)
            || (l >= 1410014464 && l <= 1410014495)
            || (l >= 1410027008 && l <= 1410027263)
            || (l >= 1410035200 && l <= 1410035231)
            || (l >= 1410035264 && l <= 1410035295)
            || (l >= 1425426432 && l <= 1425428479)
            || (l >= 1441726464 && l <= 1441729023)
            || (l >= 1441729536 && l <= 1441734655)
            || (l >= 1475115008 && l <= 1475117055)
            || (l >= 1500186624 && l <= 1500188671)
            || (l >= 1506476032 && l <= 1506508799)
            || (l >= 1509642240 && l <= 1509644351)
            || (l >= 1509644384 && l <= 1509646335)
            || (l >= 1533419520 && l <= 1533419775)
            || (l >= 1533420032 && l <= 1533420287)
            || (l >= 1533420544 && l <= 1533421567)
            || (l >= 1533448192 && l <= 1533450239)
            || (l >= 1533470720 && l <= 1533472767)
            || (l >= 1533513728 && l <= 1533515775)
            || (l >= 1535934464 && l <= 1535967231)
            || (l >= 1536660016 && l <= 1536660019)
            || (l >= 1536663424 && l <= 1536663551)
            || (l >= 1539227648 && l <= 1539229695)
            || (l >= 1539466752 && l <= 1539467263)
            || (l >= 1539473920 && l <= 1539474431)
            || (l >= 1539737088 && l <= 1539737343)
            || (l >= 1540410112 && l <= 1540410367)
            || (l >= 1540467712 && l <= 1540467967)
            || (l >= 1540622336 && l <= 1540622591)
            || (l >= 1540628480 && l <= 1540628735)
            || (l >= 1540790272 && l <= 1540791295)
            || (l >= 1572814848 && l <= 1572816895)
            || (l >= 1578991616 && l <= 1579024383)
            || (l >= 1585446912 && l <= 1585577983)
            || (l >= 1589346304 && l <= 1589379071)
            || (l >= 1598160896 && l <= 1598193663)
            || (l >= 1603137536 && l <= 1603141631)
            || (l >= 1605320704 && l <= 1605328895)
            || (l >= 1925638656 && l <= 1925638911)
            || (l >= 1925639680 && l <= 1925639935)
            || (l >= 2341273600 && l <= 2341339135)
            || (l >= 2717646848 && l <= 2717712383)
            || (l >= 2830827520 && l <= 2830893055)
            || (l >= 3169255424 && l <= 3169271807)
            || (l >= 3169275904 && l <= 3169278991)
            || (l >= 3169279008 && l <= 3169279231)
            || (l >= 3169279256 && l <= 3169279263)
            || (l >= 3169279296 && l <= 3169279303)
            || (l >= 3169279320 && l <= 3169279743)
            || (l >= 3169279760 && l <= 3169281023)
            || (l >= 3169281280 && l <= 3169288191)
            || (l >= 3239285760 && l <= 3239286783)
            || (l >= 3239488512 && l <= 3239488767)
            || (l >= 3240222720 && l <= 3240223231)
            || (l >= 3240812288 && l <= 3240812543)
            || (l >= 3245088256 && l <= 3245088511)
            || (l >= 3249111552 && l <= 3249112063)
            || (l >= 3250335744 && l <= 3250339839)
            || (l >= 3250359808 && l <= 3250362879)
            || (l >= 3250364416 && l <= 3250372607)
            || (l >= 3251120128 && l <= 3251120639)
            || (l >= 3252483072 && l <= 3252483583)
            || (l >= 3252484096 && l <= 3252486143)
            || (l >= 3258368000 && l <= 3258384383)
            || (l >= 3262477220 && l <= 3262477223)
            || (l >= 3262478601 && l <= 3262478601)
            || (l >= 3263045632 && l <= 3263046847)
            || (l >= 3263046912 && l <= 3263047935)
            || (l >= 3263048192 && l <= 3263053823)
            || (l >= 3266341888 && l <= 3266342143)
            || (l >= 3274145792 && l <= 3274162175)
            || (l >= 3276114944 && l <= 3276115967)
            || (l >= 3276687872 && l <= 3276688383)
            || (l >= 3276858112 && l <= 3276858367)
            || (l >= 3277381120 && l <= 3277381631)
            || (l >= 3280580096 && l <= 3280580351)
            || (l >= 3285922816 && l <= 3285923327)
            || (l >= 3286425600 && l <= 3286433791)
            || (l >= 3286566656 && l <= 3286567423)
            || (l >= 3286568192 && l <= 3286568703)
            || (l >= 3286571008 && l <= 3286571775)
            || (l >= 3288417536 && l <= 3288418047)
            || (l >= 3340584704 && l <= 3340584959)
            || (l >= 3350042880 && l <= 3350043135)
            || (l >= 3453376848 && l <= 3453376887)
            || (l >= 3487969792 && l <= 3487970047)
            || (l >= 3509522432 && l <= 3509522687)
            || (l >= 3509559040 && l <= 3509559295)
            || (l >= 3512891232 && l <= 3512891263)
            || (l >= 3517438880 && l <= 3517438911)
            || (l >= 3518894096 && l <= 3518894103)
            || (l >= 3523593216 && l <= 3523593231)
            || (l >= 3559587840 && l <= 3559596031)
            || (l >= 3561742336 && l <= 3561750527)
            || (l >= 3575824384 && l <= 3575832575)
            || (l >= 3582255104 && l <= 3582263295)
            || (l >= 3585949696 && l <= 3585957887)
            || (l >= 3628153088 && l <= 3628153343)
            || (l >= 3630097664 && l <= 3630098175)
            || (l >= 3630100224 && l <= 3630100479)
            || (l >= 3632481760 && l <= 3632481767)
            || (l >= 3645222912 && l <= 3645227007)
            ) b = true;
            else b = false;

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convent the data content of a DataSet to an XmlDocument object for use in API Services.
        /// <param name="ds">DataSet to convert to XmlDocument</param>
        /// <returns>XmlDocument</returns>
        /// </summary>

        public static XmlDocument DataSetToXmlDocument(DataSet ds)
        {
            return DataSetToXmlDocument(ds, null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convent the data content of a DataSet to an XmlDocument object for use in API Services.
        /// </summary>
        public static XmlDocument DataSetToXmlDocument(DataSet ds, string item_name)
        {
            XmlText xt;
            XmlElement set_xe, table_xe, row_xe, xe;
            XmlDeclaration xde;
            XmlDocument xd;

            xd = new XmlDocument();

            if (ds != null)
            {
                xde = xd.CreateXmlDeclaration("1.0", "utf-8", null);

                // create root element
                if (ds.DataSetName.Length > 0) set_xe = xd.CreateElement(ds.DataSetName);
                else set_xe = xd.CreateElement("set");

                xd.InsertBefore(xde, xd.DocumentElement);
                xd.AppendChild(set_xe);

                if (ds.Tables.Count > 0)
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        // create table element
                        if (dt.TableName.Length > 0) table_xe = xd.CreateElement(dt.TableName);
                        else table_xe = xd.CreateElement("table");

                        set_xe.AppendChild(table_xe);

                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow r in dt.Rows)
                            {
                                // create a new row and add it to the root node
                                if (item_name == null) item_name = "row";
                                row_xe = xd.CreateElement(item_name);

                                table_xe.AppendChild(row_xe);

                                foreach (DataColumn dc in dt.Columns)
                                {
                                    xe = xd.CreateElement(dc.ColumnName);

                                    xt = xd.CreateTextNode(r[dc.ColumnName].ToString());

                                    xe.AppendChild(xt);

                                    row_xe.AppendChild(xe);
                                }
                            }
                        }
                    }
                }
                else
                {
                }
            }
            else
            {
            }

            return xd;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert the data content of a DataTable to an XmlDocument object for use in API Services.
        /// <param name="dt">DataTable to convert to XmlDocument</param>
        /// <returns>XmlDocument</returns>
        /// </summary>

        public static XmlDocument DataTableToXmlDocument(DataTable dt)
        {
            return DataTableToXmlDocument(dt, null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert the data content of a DataTable to an XmlDocument object for use in API Services.
        /// </summary>
        public static XmlDocument DataTableToXmlDocument(DataTable dt, string itemName)
        {
            XmlText xt;
            XmlElement table_xe, row_xe, xe;
            XmlDeclaration xde;
            XmlDocument xd;

            xd = new XmlDocument();

            if (dt != null)
            {
                xde = xd.CreateXmlDeclaration("1.0", "utf-8", null);

                // create root element
                if (dt.TableName.Length > 0) table_xe = xd.CreateElement(dt.TableName);
                else table_xe = xd.CreateElement("table");

                xd.InsertBefore(xde, xd.DocumentElement);
                xd.AppendChild(table_xe);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        // create a new row and add it to the root node
                        if (itemName == null) itemName = "row";
                        row_xe = xd.CreateElement(itemName);

                        table_xe.AppendChild(row_xe);

                        foreach (DataColumn dc in dt.Columns)
                        {
                            xe = xd.CreateElement(dc.ColumnName);

                            xt = xd.CreateTextNode(r[dc.ColumnName].ToString());

                            xe.AppendChild(xt);

                            row_xe.AppendChild(xe);
                        }
                    }
                }
            }
            else
            {
            }

            return xd;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert the data content of a DataTable to an XDocument object
        /// <param name="dt">DataTable to convert to XDocument</param>
        /// <returns>XDocument</returns>
        /// </summary>

        public static XDocument DataTableToXDocument(DataTable dt)
        {
            return DataTableToXDocument(dt, null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Convert the data content of a DataTable to an XDocument object.
        /// </summary>
        public static XDocument DataTableToXDocument(DataTable dt, string itemName)
        {
            XElement tableXElement, rowXElement, xe;
            XDeclaration xde;
            XDocument xd;

            if (dt != null)
            {
                xde = new XDeclaration("1.0", "utf-8", null);

                // create root element
                if (dt.TableName.Length > 0) tableXElement = new XElement(dt.TableName);
                else tableXElement = new XElement("table");

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        // create a new row and add it to the root node
                        if (itemName == null) itemName = "row";
                        rowXElement = new XElement(itemName, "");

                        tableXElement.Add(rowXElement);

                        foreach (DataColumn dc in dt.Columns)
                        {
                            xe = new XElement(dc.ColumnName, r[dc.ColumnName].ToString());

                            rowXElement.Add(xe);
                        }
                    }
                }

                xd = new XDocument(xde, tableXElement);
            }
            else
            {
                xd = null;
            }

            return xd;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Takes an XmlNodeList with text and value, and change them into a DataTable usable for databinding with controls
        /// </summary>
        public static DataTable XmlNodeListToDataTable(XmlNodeList xnl, string value, string text)
        {
            DataTable dt;
            DataRow dr;

            dt = new DataTable();
            dt.Columns.Add(value);

            if (value != text) dt.Columns.Add(text);

            foreach (XmlNode n in xnl)
            {
                dr = dt.NewRow();
                dr[value] = n.Attributes[value].Value;
                if (value != text) dr[text] = n.Attributes[text].Value;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Takes an XmlNodeList with text and value, and change them into a DataTable usable for databinding with controls
        /// </summary>
        public static DataTable XmlNodeListToDataTable(XmlNodeList xnl, string id, string text, string url)
        {
            DataTable dt;
            DataRow dr;

            dt = new DataTable();
            dt.Columns.Add(id);
            dt.Columns.Add(text);
            dt.Columns.Add(url);

            foreach (XmlNode n in xnl)
            {
                dr = dt.NewRow();
                dr[id] = n.Attributes[id].Value;
                dr[text] = n.Attributes[text].Value;
                dr[url] = n.Attributes[url].Value;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Takes an XmlNodeList with text and value, and extra initial entries, and change them into a DataTable usable for databinding with controls
        /// </summary>
        public static DataTable XmlNodeListToDataTable(XmlNodeList xnl, string value, string text, string init_value, string init_text)
        {
            DataTable dt;
            DataRow dr;

            dt = new DataTable();
            dt.Columns.Add(value);
            dt.Columns.Add(text);

            dr = dt.NewRow();
            dr[value] = init_value;
            dr[text] = init_text;
            dt.Rows.Add(dr);

            foreach (XmlNode n in xnl)
            {
                dr = dt.NewRow();
                dr[value] = n.Attributes[value].Value;
                dr[text] = n.Attributes[text].Value;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return domain name from page Uri
        /// </summary>
    /// <remarks> ReadDomainFromUri(this.Context.Request.Url, out url, out domain, out puny);</remarks>
        /// </summary>
        public static string ReadDomainFromUri(Uri uri)
        {
            string url, domain, puny, tld;

            if (uri.Host == "localhost")
            {
                url = uri.Segments[1].Replace("/", "");
                url = url.ToLower();
                url = url.Replace("http://", "");
                url = url.Replace("www.", "");
            }
            else
            {
                url = uri.Host;
                url = url.ToLower();
                url = url.Replace("http://", "");
                url = url.Replace("www.", "");
            }

            if (url.Contains("xn--"))
            {
                // URL is punycode
                if (url.Contains(".com")) tld = "com";
                else if (url.Contains(".net")) tld = "net";
                else if (url.Contains(".org")) tld = "org";
                else tld = "?";

                url = url.Replace(".com", "");
                url = url.Replace(".net", "");
                url = url.Replace(".org", "");

                domain = global::Ia.Cl.Model.Punycode.Decode(url) + "." + tld;
                puny = url + "." + tld;
            }
            else
            {
                // URL is not punycode
                domain = url;
                puny = url;
            }

            return domain;
        }

#if WINDOWS_FORM
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the MAC Address of the computer
        /// </summary>
        public static string Get_MAC_Address()
        {
            // This will support IPv4 and IPv6.

            string mac_address;
            System.Net.NetworkInformation.NetworkInterface[] nics;

            nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

            mac_address = string.Empty;

            foreach (System.Net.NetworkInformation.NetworkInterface adapter in nics)
            {
                if (mac_address == string.Empty)// only return MAC Address from first card  
                {
                    System.Net.NetworkInformation.IPInterfaceProperties properties = adapter.GetIPProperties();

                    mac_address = adapter.GetPhysicalAddress().ToString();
                }
            }

            return mac_address;
        }
#endif
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Download file
        /// </summary>
        public static bool DownloadFile(string url, string fileName)
        {
            // 
            bool b;

            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                wc.DownloadFile(url, fileName);

                b = true;
            }
            catch (Exception)
            {
                b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Regular Expression Guid Match
        /// </summary>
        /// </summary>
    /// <remarks> http://www.geekzilla.co.uk/view8AD536EF-BC0D-427F-9F15-3A1BC663848E.htm</remarks>
        public static bool IsGuid(string s)
        {
            bool b = false;

            if (!string.IsNullOrEmpty(s))
            {
                Regex r = new Regex(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$");

                b = r.IsMatch(s);
            }

            return b;
        }

#if !WINDOWS_FORM
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Checks the API Service State
        /// </summary>
        public static string ApiServiceState(System.Web.HttpApplicationState has, string name, TimeSpan ts)
        {
            bool b;
            string s;
            DateTime dti;

            b = false;
            s = "";

            // check if timespan variable is stored and is not null
            if (has[name] != null && has[name + "_ts"] != null)
            {
                // check if the timespan is cleared since stored value

                s = has[name].ToString();
                dti = DateTime.Parse(has[name + "_ts"].ToString());

                if (DateTime.UtcNow.AddHours(3) > dti + ts)
                {
                    // update API Service 
                    b = true;
                }
                else
                {
                    // do nothing
                    b = false;
                }
            }
            else b = true;

            if (b)
            {
                // update API Service values
                /*
                Ia.Ngn.kw.com.i.Ws_Default svc = new Ia.Ngn.kw.com.i.Ws_Default();
                svc.Timeout = 5000;

                try
                {
                    s = svc.Echo("Hello!");
                }
                catch (Exception)
                {
                    s = null;
                }
                finally
                {
                    has[name] = s;
                    has[name + "_ts"] = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-dd hh:mm:ss");
                }
                */
            }

            return has[name].ToString() + ":" + has[name + "_ts"].ToString();
        }
#endif

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// C# to convert a string to a byte array.
        /// </summary>
        private static byte[] ConvertStringToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);

            /*
        // C# to convert a byte array to a string.
        byte [] dBytes = ...
        string str;
        System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
        str = enc.GetString(dBytes);
             */
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static int IncrementArrayListIndexOrRestart(ArrayList list, int currentIndex)
        {
            int newIndex;

            if (currentIndex < list.Count - 1) newIndex = ++currentIndex;
            else newIndex = 0;

            return newIndex;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static int IncrementListIndexOrRestart<T>(List<T> list, int currentIndex)
        {
            int newIndex;

            if (currentIndex < list.Count - 1) newIndex = ++currentIndex;
            else newIndex = 0;

            return newIndex;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void IncrementIndexOrReset(int listCount, ref int currentIndex)
        {
            currentIndex = (currentIndex < listCount - 1) ? ++currentIndex : 0;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check if the this.Request.UserHostAddress from the webpage is from the development machine or IP.
        /// <param name="userHostAddress">The this.Request.UserHostAddress from the webpage</param>
        /// </summary>
        public static bool IaHostAddress(string userHostAddress)
        {
            bool isMyHostAddress;
            string iaHostAddress;

            iaHostAddress = ConfigurationManager.AppSettings["iaHostAddress"].ToString();

            if (userHostAddress == iaHostAddress || userHostAddress == "::1") isMyHostAddress = true;
            else isMyHostAddress = false;

            return isMyHostAddress;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void ShuffleArrayList(ArrayList al)
        {
            Random r = new Random();

            for (int inx = al.Count - 1; inx > 0; --inx)
            {
                int position = r.Next(inx);
                object temp = al[inx];
                al[inx] = al[position];
                al[position] = temp;
            }
        }
         */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static ArrayList ShuffleArrayList(ArrayList inputList)
        {
            ArrayList randomList = new ArrayList();

            if (random == null) random = new Random();

            int randomIndex = 0;

            while (inputList.Count > 0)
            {
                randomIndex = random.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static ArrayList IntegerListRange(ArrayList a_al)
        {
            // this will take an ArrayList of integer, remove duplicates, sort, then construct an ArrayList with ranges defined, if available. For example
            // a_al = [1,2,4,6,7,15,20,21,22,34,35,36,38]
            // b_al = [1,2,4,6,7,15,20-22,34-36,38]

            bool range, range_old;
            int u, v;
            int start, end;
            ArrayList b_al, c_al;
            Hashtable ht;

            // a_al = [1,2,4,6,7,15,20,21,22,34,35,36,38]
            // b_al = [1,2,4,6,7,15,20-22,34-36,38]

            start = end = 0;

            b_al = new ArrayList(a_al.Count + 1);
            c_al = new ArrayList(a_al.Count + 1);

            if (a_al.Count > 0)
            {
                // remove duplicates
                ht = new Hashtable(a_al.Count + 1);

                foreach (int i in a_al) ht[i] = 1;

                foreach (int i in ht.Keys) b_al.Add(i);

                // sort
                b_al.Sort();

                if (b_al.Count > 0)
                {
                    range = range_old = false;
                    u = (int)b_al[0];

                    for (int i = 1; i <= b_al.Count; i++)
                    {
                        if (i < b_al.Count) v = (int)b_al[i];
                        else v = (int)b_al[i - 1];

                        if (v - u == 1)
                        {
                            if (range) end = v;
                            else
                            {
                                start = u;
                                range = true;
                            }
                        }
                        else
                        {
                            if (range)
                            {
                                end = u;
                                range = false;
                            }
                            else c_al.Add(u.ToString());
                        }

                        if (range != range_old && range == false)
                        {
                            if (end - start == 1)
                            {
                                c_al.Add(start.ToString());
                                c_al.Add(end.ToString());
                            }
                            else c_al.Add(start + "-" + end);
                        }

                        u = v;
                        range_old = range;
                    }
                }
                else
                {

                }
            }
            else
            {
            }

            return c_al;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Generate a list of start-end count optimized to read around count number of values from passed list. 
        /// </summary>
        public static List<Tuple<int, int>> OptimizedStartEndRangeList(List<int> list, int count)
        {
            bool done;
            int c, start, end;
            Tuple<int, int> tuple;
            List<Tuple<int, int>> tupleList;

            tupleList = new List<Tuple<int, int>>();

            done = false;
            c = start = 0;

            if (list.Count > 0 && count > 0)
            {
                foreach (int i in list)
                {
                    if (c == 0)
                    {
                        start = i;
                        done = false;
                    }

                    if (c == count - 1)
                    {
                        end = i;

                        tuple = new Tuple<int, int>(start, end);

                        tupleList.Add(tuple);

                        done = true;
                        c = 0;
                    }
                    else c++;
                }

                if(!done)
                {
                    end = list[list.Count -1];

                    tuple = new Tuple<int, int>(start, end);

                    tupleList.Add(tuple);
                }
            }
            else throw new System.ArgumentException("List empty or range too short. ");

            tupleList.Sort();

            return tupleList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Generate a OptimizedStartEndRangeList() but with a range buffer before start and after end.
        /// </summary>
        public static List<Tuple<int, int>> OptimizedStartEndRangeBufferedList(List<int> list, int count, int bufferRange)
        {
            int start, end;
            List<Tuple<int, int>> tupleList;

            tupleList = OptimizedStartEndRangeList(list, count);

            if (tupleList != null && tupleList.Count > 0)
            {
                start = tupleList[0].Item1;
                end = tupleList[tupleList.Count - 1].Item1;

                tupleList.Insert(0, new Tuple<int, int>(start - bufferRange, start - 1));
                tupleList.Insert(tupleList.Count - 1, new Tuple<int, int>(end + 1, end + bufferRange + 1));
            }

            tupleList.Sort();

            return tupleList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Generate a list of start, end points of all integer ranges with given start, end and range. End included in range.
        /// </summary>
        public static List<Tuple<int, int>> StartEndRangeList(int start, int end, int range)
        {
            Tuple<int, int> tuple;
            List<Tuple<int, int>> tupleList;

            tupleList = new List<Tuple<int,int>>();

            if (end > start && range < (end - start))
            {
                for (int i = start; i <= end; i += range)
                {
                    if (i + range - 1 < end) tuple = new Tuple<int, int>(i, i + range - 1);
                    else tuple = new Tuple<int, int>(i, end);

                    tupleList.Add(tuple);
                }
            }
            else throw new System.ArgumentException("Start, end, or range is/are invalid. ");

            tupleList.Sort();

            return tupleList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Generate a StartEndRangeList() but with a range buffer before start and after end.
        /// </summary>
        public static List<Tuple<int, int>> StartEndRangeBufferedList(int start, int end, int range, int bufferRange)
        {
            List<Tuple<int, int>> tupleList;

            tupleList = StartEndRangeList(start, end, range);

            if(tupleList != null && tupleList.Count > 0)
            {
                tupleList.Insert(0, new Tuple<int, int>(start - bufferRange, start - 1));
                tupleList.Insert(tupleList.Count - 1, new Tuple<int, int>(end + 1, end + bufferRange + 1));
            }

            tupleList.Sort();

            return tupleList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<int> ConvertHyphenAndCommaSeperatedNumberStringToASortedNumberArrayList(string listStringAbbriviation)
        {
            int j, start, end;
            string abbriviation, u;
            List<int> list;
            MatchCollection matchCollection;

            // change number range (e.g. "1-5") to comma seperated numbers like "1,2,3,4,5"

            list = new List<int>();

            abbriviation = listStringAbbriviation;

            // 1-5;
            matchCollection = Regex.Matches(abbriviation, @"(\d{1,3})\-(\d{1,3})");

            foreach (Match match in matchCollection)
            {
                start = int.Parse(match.Groups[1].Value);
                end = int.Parse(match.Groups[2].Value);

                u = "";
                for (int i = start; i <= end; i++) u += i + ",";

                u = u.TrimEnd(',');

                // remove the matched string from the main string
                abbriviation = abbriviation.Replace(match.Groups[0].Value, u);
            }

            // convert to ArrayList

            foreach (string s in abbriviation.Split(','))
            {
                if (int.TryParse(s, out j)) list.Add(j);
                //else arrayList.Add(null);
            }

            // don't sort
            //arrayList.Sort();

            return list;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ConvertANumberArrayListToHyphenAndCommaSeperatedNumberString(List<int> numberList)
        {
            int start, end, j;
            string abbriviation;
            ArrayList al;

            if (numberList.Count > 0)
            {
                numberList.Sort();

                al = new ArrayList();

                start = numberList[0];
                end = numberList[numberList.Count - 1];

                j = 0;
                abbriviation = "";

                // add ',' where there are empty spaces
                for (int i = start; i <= end; i++)
                {
                    if (i == (int)numberList[j])
                    {
                        abbriviation += i + " ";
                        j++;
                    }
                    else abbriviation += ",";
                }

                // remove redundant ',' characters
                abbriviation = Regex.Replace(abbriviation, @",{2,}", @",");

                // replace 3 or more number sequences with a range
                abbriviation = Regex.Replace(abbriviation, @"(\d+?) [\d+ ]+ (\d+?)", @"$1-$2");

                // now replace 2 or more number sequences with a comma
                abbriviation = Regex.Replace(abbriviation, @"(\d+?) (\d+?)", @"$1,$2");

                abbriviation = abbriviation.Replace(" ", "");
            }
            else abbriviation = null;

            return abbriviation;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string NumberListToCommaSeperatedNumberString(List<int> numberList)
        {
            string s;

            if (numberList != null && numberList.Count > 0)
            {
                s = string.Join(",", numberList.Select(n => n.ToString()).ToArray());
            }
            else s = null;

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<int> CommaSeperatedNumberStringToNumberList(string numberListString)
        {
            List<int> numberList;

            if (!string.IsNullOrEmpty(numberListString))
            {
                numberList = (numberListString != string.Empty) ? numberListString.Split(',').Select(Int32.Parse).ToList() : null;
                numberList.Sort();
            }
            else numberList = null;

            return numberList;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Generate HTML table from list of generic class with specified properties
        /// <see cref="http://stackoverflow.com/questions/11126137/generate-html-table-from-list-of-generic-class-with-specified-properties"/>
        /// </summary>
        public static string GenerateHtmlTableFromListOfGenericClass<T>(IEnumerable<T> list, List<string> columnNameList, params Func<T, object>[] fxns)
        {
            StringBuilder sb;

            sb = new StringBuilder();

            sb.Append("<table>\n");

            // column names
            if (columnNameList.Count > 0)
            {
                sb.Append("<tr>");

                foreach (string column in columnNameList)
                {
                    sb.Append("<td>");
                    sb.Append(column);
                    sb.Append("</td>");
                }

                sb.Append("</tr>\n");
            }

            foreach (var item in list)
            {
                sb.Append("<tr>");

                foreach (var fxn in fxns)
                {
                    sb.Append("<td>");
                    sb.Append(fxn(item));
                    sb.Append("</td>");
                }

                sb.Append("</tr>\n");
            }

            sb.Append("</table>");

            return sb.ToString();
        }
         */ 

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// <see cref="http://stackoverflow.com/questions/564366/convert-generic-list-enumerable-to-datatable"/>
        /// </summary>
        public static DataTable GenerateDataTableFromListOfGenericClass<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props;
            DataTable table;

            table = new DataTable();
            props = TypeDescriptor.GetProperties(typeof(T));

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];

            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }

                table.Rows.Add(values);
            }

            return table;
        }
        
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Generate HTML table from DataTable
        /// <see cref="http://stackoverflow.com/questions/19682996/datatable-to-html-table"/>
        /// </summary>
        public static string GenerateHtmlTableFromDataTable(DataTable dataTable)
        {
            StringBuilder sb;

            sb = new StringBuilder();

            sb.Append("<table>\n");

            // header
            sb.Append("<tr>");

            for (int i = 0; i < dataTable.Columns.Count; i++) sb.Append("<td>" + dataTable.Columns[i].ColumnName + "</td>");

            sb.Append("</tr>");

            // row
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                sb.Append("<tr>");

                for (int j = 0; j < dataTable.Columns.Count; j++) sb.Append("<td>" + dataTable.Rows[i][j].ToString() + "</td>");

                sb.Append("</tr>");
            }

            sb.Append("</table>");

            return sb.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Generate tab separated text format from DataTable
        /// </summary>
        public static string GenerateTabSeparatedTextFromDataTable(DataTable dataTable)
        {
            StringBuilder sb;

            sb = new StringBuilder();

            for (int i = 0; i < dataTable.Columns.Count; i++) sb.Append(dataTable.Columns[i].ColumnName + "\t");

            sb.Append("\r\n");

            // row
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++) sb.Append(dataTable.Rows[i][j].ToString() + "\t");

                sb.Append("\r\n");
            }

            return sb.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Remove non-numeric characters
        /// http://stackoverflow.com/questions/3977497/stripping-out-non-numeric-characters-in-string
        /// </summary>
        public static string RemoveNonNumericCharacters(string line)
        {
            string s;

            if (line != null && line.Length != 0)
            {
                s = new string(line.Where(c => char.IsDigit(c)).ToArray());
                //s = Regex.Replace(line, "[^0-9]", "");
            }
            else s = line;

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// <remarks>http://stackoverflow.com/questions/2475795/check-for-missing-number-in-sequence</remarks>
        /// </summary>
        public static List<int> ExcludedNumberListFromNumberListWithinRange(List<int> list, int listSize)
        {
            // Check for missing number in sequence
            List<int> exclusionList;

            exclusionList = Enumerable.Range(1, listSize).Except(list).ToList();

            return exclusionList;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

#if WINDOWS_FORM
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class ListItem
    {
        // I created this because Windows Forms does not have the equivalent like System.Web ListItem

        private string name;
        private string value;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public ListItem(string _name, string _value)
        {
            this.name = _name;
            this.value = _value;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Value
        {

            get
            {
                return value;
            }
        }
    }
#else
#endif

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}
