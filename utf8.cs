using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.OleDb;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Handle UTF8 issues.
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
    public class Utf8 : System.Web.UI.Page
    {
        /// <summary/>
        protected DataSet ds;

        /// <summary/>
        protected OleDbDataAdapter da;

        /// <summary/>
        protected DataTable dt;

        /// <summary/>
        protected Label result_l;

        /// <summary/>
        public string connection_string = "Provider=MySQLProv;Location=localhost;Data Source=*;UID=;PWD=";

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            UTF8Encoding utf8 = new UTF8Encoding();

            result_l.Text += "<br><br>X: " + 123.ToString("X");

            string line = "This unicode string Pi (\u03a0) and Sigma (\u03a3) and (أهلاً و سهلاً) and (日本).";
            result_l.Text += "<br><br>Original: " + line;

            char[] chars;
            chars = line.ToCharArray();

            /////////////////////////

            result_l.Text += "<br><br>Chars: ";
            foreach (char c in chars)
            {
                result_l.Text += "|" + c;
            }

            /////////////////////////

            result_l.Text += "<br><br>Bytes inside Chars: ";
            string s, hex = "";
            foreach (char c in chars)
            {
                result_l.Text += "[" + c + "]: [";
                s = c.ToString();

                Byte[] line5 = utf8.GetBytes(s);
                hex = "";
                foreach (Byte b in line5)
                {
                    hex += b.ToString("x");
                    result_l.Text += "|" + (char)b + "(" + b + ")";
                }
                //   if(hex.Length == 2) hex = "00" + hex;
                result_l.Text += @"{|" + hex + "}";

                result_l.Text += "]<br>";

            }

            /////////////////////////

            Byte[] line2 = utf8.GetBytes(line);
            result_l.Text += "<br><br>Bytes: ";
            foreach (Byte b in line2)
            {
                result_l.Text += "|" + (char)b;
            }

            /////////////////////////

            // Decode bytes back to string.
            // Notice Pi and Sigma characters are still present.
            string line3 = utf8.GetString(line2);
            result_l.Text += "<br><br>Decoded: " + line3;

            /////////////////////////

            result_l.Text += "<br><br>Convert line with my function: " + Utf8ToHex(line);

            /////////////////////////

            result_l.Text += "<br><br>Reconstruct line with my function: " + HexToUtf8(Utf8ToHex(line));
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        protected string Utf8ToHex(string utf8_str)
        {
            byte[] bytes;
            string s = "";
            UTF8Encoding utf8 = new UTF8Encoding();

            bytes = utf8.GetBytes(utf8_str);

            foreach (byte b in bytes) s += b.ToString("x");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        protected string HexToUtf8(string hex_str)
        {
            int i, n;
            byte[] bytes;
            char[] chars;
            string c_str = "", s = "";
            UTF8Encoding utf8 = new UTF8Encoding();

            chars = hex_str.ToCharArray();

            bytes = new byte[chars.Length / 2];  // since hex_str has to chars for every byte

            n = 0;
            for (i = 0; i < chars.Length; i += 2)
            {
                c_str = chars[i].ToString() + chars[i + 1].ToString();
                bytes[n++] = (byte)Convert.ToInt32(c_str, 16);
            }

            s = utf8.GetString(bytes);

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        protected string Utf8ToAscii(string utf8_str)
        {
            byte[] bytes;
            string str = "";
            UTF8Encoding utf8 = new UTF8Encoding();
            ASCIIEncoding ascii = new ASCIIEncoding();

            bytes = utf8.GetBytes(utf8_str);

            foreach (byte b in bytes)
            {
                str += (char)b;
            }

            /*
               foreach(byte b in bytes)
               {
                Response.Write(" " + b +":"+ (char)b);
               }
            */

            //  str = ascii.GetString(bytes);
            //  str = utf8.GetString(bytes);

            return str;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        protected string AsciiToUtf8(string ascii_str)
        {
            char[] chars;
            string str = "";
            UTF8Encoding utf8 = new UTF8Encoding();
            ASCIIEncoding ascii = new ASCIIEncoding();

            chars = ascii_str.ToCharArray();

            int i = 0;
            byte[] bytes = new byte[chars.Length];

            foreach (char c in chars)
            {
                bytes[i++] += (byte)c;
            }

            //  bytes = ascii.GetBytes(ascii_str);
            //  bytes = utf8.GetBytes(ascii_str);

            str = utf8.GetString(bytes);

            return str;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        protected byte[] Utf8ToByte(string text)
        {
            UTF8Encoding utf8 = new UTF8Encoding();

            // Encode the string.
            byte[] encodedBytes = utf8.GetBytes(text);

            return encodedBytes;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        protected string ByteToUtf8(byte[] text)
        {
            string ichi;
            UTF8Encoding utf8 = new UTF8Encoding();
            // Decode bytes back to string.

            ichi = utf8.GetString(text);

            return ichi;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        protected string ByteToChar(byte[] text)
        {
            string ichi = "";

            foreach (byte b in text)
            {
                ichi += (char)b;
            }

            return ichi;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        protected string HtmlEncode(string encode)
        {

            // Response.Write("[[["+ encode);

            encode = Utf8ToAscii(encode);

            encode = Server.HtmlEncode(encode);

            encode = encode.Replace(@"'", @"&#039");
            encode = encode.Replace(@"\", @"&#092");
            encode = encode.Replace(@"?", @"&#063"); // the MySQL database has problems with the question mark '?'

            // Response.Write(":"+ encode);

            encode = encode.Replace(@"", @"&#142");

            // Response.Write(":"+ encode+"]]]");

            return encode;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        string HtmlDecode(string decode)
        {
            decode = decode.Replace(@"&#142", @"");

            decode = decode.Replace(@"&#039", @"'");
            decode = decode.Replace(@"&#092", @"\");
            decode = decode.Replace(@"&#063", @"?"); // the MySQL database has problems with the question mark '?'

            decode = Server.HtmlDecode(decode);
            decode = AsciiToUtf8(decode);

            return decode;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
