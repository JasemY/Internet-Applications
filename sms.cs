using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Configuration;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// SMS API service support class. Handles sending and recieving SMS messages through the ClickATell.com SMS API Service gateway. Requires subscription.
    /// </summary>
    /// 
    /// <value> 
    /// I have updated the code to use "Send SMS with C#" see http://www.clickatell.com/developers/c.php 2010-02
    /// </value>
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
    public class Sms
    {
        private static string url;
        private static string api_id, user, password;
        private static WebClient wc;
        private static Stream s;
        private static StreamReader sr;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Sms() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static int Send(string to, string from, string text, out string result)
        {
            // for non-unicode sending on letter is about 160 chars. You need concat value that is at least (chars/160) + 1
            int op;

            op = 0;
            result = "";

            // unicode = 0, concat = 2;
            op = Send(to, from, text, 0, 2, out result);

            return op;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static int SendUtf8(string to, string from, string text, out string result)
        {
            int op;

            op = 0;
            result = "";

            // unicode = 1 (for Arabic), concat = 2;
            op = Send(to, from, ConvertUtf8ToUnicodeHex(text), 1, 2, out result);

            return op;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static int Send(string to, string from, string text, int unicode, int concat, out string result)
        {
            int op;
            string u;

            op = 0;
            result = "";
            wc = new WebClient();

            api_id = ConfigurationManager.AppSettings["clickatellHttpApiId"].ToString();
            user = ConfigurationManager.AppSettings["clickatellUser"].ToString();
            password = ConfigurationManager.AppSettings["clickatellPassword"].ToString();

            url = "http://api.clickatell.com/http/sendmsg";

            // remove all space chars
            to = Regex.Replace(to, @"\s", "");

            // check that to is all digits
            if (Regex.IsMatch(to, @"^\d{8,15}$"))
            {
                // remove the leading "00" if they existed
                to = Regex.Replace(to, "^00", "");

                if (to.Length == 8) // if number is 8 digits we will assume it is from Kuwait and add the country code
                {
                    // check if number is SMSable
                    to = "965" + to; // this adds the country code to the number
                }

                try
                {
                    wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                    wc.QueryString.Add("user", user);
                    wc.QueryString.Add("password", password);
                    wc.QueryString.Add("api_id", api_id);
                    wc.QueryString.Add("to", to);
                    wc.QueryString.Add("concat", concat.ToString());
                    wc.QueryString.Add("unicode", unicode.ToString());
                    wc.QueryString.Add("text", text);

                    s = wc.OpenRead(url);
                    sr = new StreamReader(s);

                    u = sr.ReadToEnd();

                    if (u.Contains("ERR"))
                    {
                        result = "SMS could not be sent (" + u + "). ";
                        op = -1;
                    }
                    else
                    {
                        result = u;
                        op = 1;
                    }
                }
                catch (Exception ex)
                {
                    result = "SMS could not be sent. " + ex.Message;
                    op = -1;
                }
                finally
                {
                    s.Close();
                    sr.Close();
                }
            }
            else
            {
                result = "Number has non-digit characters or number of characters is not within range. ";
                op = -1;
            }

            return op;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static int send_old(string to, string from, string text, out string result)
        {
            int op;
            string response, r1;

            op = 0;
            result = r1 = "";

            api_id = ConfigurationManager.AppSettings["clickatell_xml_api_id"].ToString();
            user = ConfigurationManager.AppSettings["clickatell_user"].ToString();
            password = ConfigurationManager.AppSettings["clickatell_password"].ToString();

            XmlDataDocument d = new XmlDataDocument();

            // remove all space chars
            to = Regex.Replace(to, @"\s", "");

            // check that to is all digits
            if (Regex.IsMatch(to, @"^\d{8,15}$"))
            {
                // remove the leading "00" if they existed
                to = Regex.Replace(to, "^00", "");

                if (to.Length <= 8) // if number is 8 or less digits we will assume it is from Kuwait and add the country code
                {
                    // check if number is SMSable

                    to = "965" + to; // this adds the country code to the number
                }

                url = "http://api.clickatell.com/xml/xml?data=<clickAPI><sendMsg><api_id>" + api_id + "</api_id><user>" + user + "</user><password>" + password + "</password><concat>" + concat + "</concat><unicode>" + unicode + "</unicode><to>" + to + "</to><text>" + HttpUtility.UrlEncode(text) + "</text><from>" + from + "</from></sendMsg></clickAPI>";

                op = get_page(url, out response, out r1);

                if (op > 0)
                {
                    try { d.LoadXml(response); }
                    catch (Exception) { result = "SMS could not be sent. Response is not in recognized XML format. "; }
                }
                else { result = r1; }
            }
            else { result = "Number has non-digit characters or number of characters is not within range"; op = -1; }

            return op;
        }
        */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static string ConvertUtf8ToUnicodeHex(string line)
        {
            // convert a utf8 string to a hex representation of unicode
            UTF8Encoding utf8 = new UTF8Encoding();

            // encode the string
            byte[] bytes = utf8.GetBytes(line);
            int count = utf8.GetCharCount(bytes);

            Decoder d = Encoding.UTF8.GetDecoder();
            int charCount = d.GetCharCount(bytes, 0, bytes.Length);
            char[] chars = new char[charCount];
            int charsDecodedCount = d.GetChars(bytes, 0, bytes.Length, chars, 0);

            line = "";
            foreach (char c in chars) line += ((ushort)c).ToString("X").PadLeft(4, '0');

            //result_l.Text += "\n<br><br>ΩΨΘ:03A903A80398";
            return line;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static int get_page(string url, out string text, out string result)
        {
            int op;

            op = 0;
            result = "";
            text = "";

            try
            {
                Uri ourUri = new Uri(url);
                // Creates an HttpWebRequest for the specified URL. 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(ourUri);
                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                Stream receiveStream = myHttpWebResponse.GetResponseStream();
                Encoding encode = System.Text.Encoding.GetEncoding("utf-8");

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, encode);

                Char[] read = new Char[256];
                // Reads 256 characters at a time.    
                int count = readStream.Read(read, 0, 256);

                while (count > 0)
                {
                    // Dumps the 256 characters on a string and displays the string to the console.
                    String str = new String(read, 0, count);
                    text += str;
                    count = readStream.Read(read, 0, 256);
                }

                // Releases the resources of the response.
                myHttpWebResponse.Close();

                op = 1;
            }
            catch (WebException e)
            {
                HttpWebResponse response = (HttpWebResponse)e.Response;
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        string challenge = null;
                        challenge = response.GetResponseHeader("WWW-Authenticate");
                        if (challenge != null) result = "The following challenge was raised by the server: " + challenge;
                    }
                    else result = "The following WebException was raised : " + e.Message;
                }
                else result = "Response Received from server was null";

                op = -1;
            }
            catch (Exception e)
            {
                result = "The following Exception was raised : " + e.Message;
                op = -1;
            }

            return op;
        }
        */

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
