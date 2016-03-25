using System;
using System.Web;
using System.Xml;
using System.Net;
using System.Text;
using System.IO;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Contains functions that relate to posting and receiving data from remote Internet/Intranet pages
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
    public class Http
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static bool range = false;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Http() { }

        // Note that "https://" and "http://" are different. wrong protocol could produce a "(403) Forbidden" response.

        // Include custom cookies, start and end points, and posting of data to remove server.

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Request(string url)
        {
            range = false;

            return ProcessRequest(url, 0, false, null, null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Request(string url, int start)
        {
            range = true;

            return ProcessRequest(url, start, false, null, null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Request2(string url)
        {
            range = true;

            return ProcessRequest2(url, false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Request_Utf8(string url, int start)
        {
            range = true;

            return ProcessRequest(url, start, true, null, null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Request(string url, int start, System.Net.Cookie c)
        {
            range = true;

            return ProcessRequest(url, start, false, c, null);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Request(string url, int start, System.Net.Cookie c1, System.Net.Cookie c2)
        {
            range = true;

            return ProcessRequest(url, start, false, c1, c2);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Post(string URI, string Parameters)
        {
            // for a "Request format is unrecognized" problem see: http://support.microsoft.com/default.aspx?scid=kb;en-us;819267

            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            //req.Proxy = new System.Net.WebProxy(ProxyString, true);

            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            //req.Timeout = 3000;

            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
            req.ContentLength = bytes.Length;

            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();

            System.Net.WebResponse resp = null;

            try
            {
                resp = req.GetResponse();

                if (resp == null) return null;

                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream(), Encoding.GetEncoding(1256));
                return sr.ReadToEnd().Trim();
            }
            catch (WebException ex)
            {
                string str = ex.Message;
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Post(string URI, string Parameters, int code_page)
        {
            // for a "Request format is unrecognized" problem see: http://support.microsoft.com/default.aspx?scid=kb;en-us;819267

            // Sometimes you need to POST in Windows 1256 code page for the process to run

            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            //req.Proxy = new System.Net.WebProxy(ProxyString, true);

            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            //req.Timeout = 3000;

            byte[] bytes = System.Text.Encoding.GetEncoding(code_page).GetBytes(Parameters);
            req.ContentLength = bytes.Length;

            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
            os.Close();

            System.Net.WebResponse resp = null;

            try
            {
                resp = req.GetResponse();

                if (resp == null) return null;

                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream(), Encoding.GetEncoding(code_page));
                return sr.ReadToEnd().Trim();
            }
            catch (WebException ex)
            {
                string str = ex.Message;
            }

            return null;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static string ProcessRequest(string url, int start, bool utf8, System.Net.Cookie c1, System.Net.Cookie c2)
        {
            string text = "";

            try
            {
                Uri ourUri = new Uri(url);
                // Creates an HttpWebRequest for the specified URL. 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(ourUri);

                // this code below is very important. It sends a request with a specific cookie in the collection
                // to demonstrate to the remote server that we have his cookie and we should skip his advertisement.
                if (c1 != null || c2 != null)
                {
                    myHttpWebRequest.CookieContainer = new CookieContainer();
                    if (c1 != null) myHttpWebRequest.CookieContainer.Add(c1);
                    if (c2 != null) myHttpWebRequest.CookieContainer.Add(c2);
                }

                myHttpWebRequest.Method = "POST";
                //myHttpWebRequest.Timeout = 5000; // 5 sec
                //myHttpWebRequest.MaximumResponseHeadersLength = 100; // *1024 (Kilobytes)

                // set the range of data to be returned if the start and end positions are given
                if (range) myHttpWebRequest.AddRange(start);

                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                myHttpWebRequest.ContentLength = 0;

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                if (myHttpWebRequest.HaveResponse)
                {
                    Stream receiveStream = myHttpWebResponse.GetResponseStream();
                    Encoding encode;

                    if (utf8) encode = System.Text.Encoding.GetEncoding("utf-8");
                    else encode = System.Text.Encoding.GetEncoding(1252); // 1252 best for western char

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(receiveStream, encode);

                    text = readStream.ReadToEnd().Trim();  // ONE

                    /*
                    // TWO
                    Char[] read = new Char[256];
                    // Reads 256 characters at a time.    
                    int count = readStream.Read( read, 0, 256 );
        
                    while (count > 0) 
                    {
                      // Dumps the 256 characters on a string and displays the string to the console.
                      String str = new String(read, 0, count);
                      text += str;
                      count = readStream.Read(read, 0, 256);
                    }
                    */

                    // Releases the resources of the response.
                    myHttpWebResponse.Close();
                }
                else
                {
                    text = "\nResponse not received from server";
                }
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
                        if (challenge != null) text = "\nThe following challenge was raised by the server: " + challenge;
                    }
                    else text = "\nThe following WebException was raised : " + e.Message;
                }
                else text = "\nResponse Received from server was null";
            }
            catch (Exception e)
            {
                text = "\nThe following Exception was raised : " + e.Message;
            }

            return text;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static string ProcessRequest2(string url, bool utf8)
        {
            string text = "";

            try
            {
                Uri ourUri = new Uri(url);
                // Creates an HttpWebRequest for the specified URL. 
                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(ourUri);

                //myHttpWebRequest.Method = "POST";
                //myHttpWebRequest.Timeout = 5000; // 5 sec
                //myHttpWebRequest.MaximumResponseHeadersLength = 100; // *1024 (Kilobytes)

                // set the range of data to be returned if the start and end positions are given
                //if (range) myHttpWebRequest.AddRange(start);

                myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
                myHttpWebRequest.ContentLength = 0;

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                if (myHttpWebRequest.HaveResponse)
                {
                    Stream receiveStream = myHttpWebResponse.GetResponseStream();
                    Encoding encode;

                    if (utf8) encode = System.Text.Encoding.GetEncoding("utf-8");
                    else encode = System.Text.Encoding.GetEncoding(1252); // 1252 best for western char

                    // Pipes the stream to a higher level stream reader with the required encoding format. 
                    StreamReader readStream = new StreamReader(receiveStream, encode);

                    text = readStream.ReadToEnd().Trim();  // ONE

                    /*
                    // TWO
                    Char[] read = new Char[256];
                    // Reads 256 characters at a time.    
                    int count = readStream.Read( read, 0, 256 );
        
                    while (count > 0) 
                    {
                      // Dumps the 256 characters on a string and displays the string to the console.
                      String str = new String(read, 0, count);
                      text += str;
                      count = readStream.Read(read, 0, 256);
                    }
                    */

                    // Releases the resources of the response.
                    myHttpWebResponse.Close();
                }
                else
                {
                    text = "\nResponse not received from server";
                }
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
                        if (challenge != null) text = "\nThe following challenge was raised by the server: " + challenge;
                    }
                    else text = "\nThe following WebException was raised : " + e.Message;
                }
                else text = "\nResponse Received from server was null";
            }
            catch (Exception e)
            {
                text = "\nThe following Exception was raised : " + e.Message;
            }

            return text;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static int Get(string url, out string text, out string result)
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

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
