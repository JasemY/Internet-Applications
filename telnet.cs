using System;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Dart.PowerTCP.Telnet;
using System.Collections;
using System.Net.Sockets;

namespace Ia.Cl.Model
{
    /// <summary publish="true">
    /// Telnet communication support class.
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
    public class Telnet
    {
        private string hostname, username, password, unixPrompt;
        private Dart.PowerTCP.Telnet.Telnet telnet = new Dart.PowerTCP.Telnet.Telnet();

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Telnet()
        {
            // enable KeepAlive socket option
            //telnet.KeepAlive = true;

            // allow addresses to be reused
            //telnet.ReuseAddress = true;

            // do blocking connect
            telnet.ReceiveTimeout = 20000;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int Connect(string _hostname, string _username, string _password, string _prompt, out string result)
        {
            int op;

            op = 0;
            result = "";

            hostname = _hostname;
            username = _username;
            password = _password;
            unixPrompt = _prompt;

            try
            {
                Segment value = telnet.Login(hostname, username, password, unixPrompt);
                result = value.ToString();

                op = 1;
            }
            catch (SocketException ex)
            {
#if DEBUG
                result = "Ia.Cl.Model.Telnet.Telnet(): " + ex.ToString();
#else
                result = "Ia.Cl.Model.Telnet.Telnet(): " + ex.Message;
#endif
                op = -1;
            }
            catch (InvalidOperationException ex)
            {
#if DEBUG
                result = "Ia.Cl.Model.Telnet.Telnet(): " + ex.ToString();
#else
                result = "Ia.Cl.Model.Telnet.Telnet(): " + ex.Message;
#endif
                op = -1;
            }
            catch (Exception ex)
            {
#if DEBUG
                result = "Ia.Cl.Model.Telnet.Telnet(): " + ex.ToString();
#else
                result = "Ia.Cl.Model.Telnet.Telnet(): " + ex.Message;
#endif
                op = -1;
            }

            return op;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return telnet.Connected;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void SendLine(string line)
        {
            string result;
            StringBuilder sb;

            SendLine(line, out sb, out result);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int SendLine(string line, out StringBuilder data, out string result)
        {
            int i;
            string[] promptList;

            promptList = new string[1];
            promptList[0] = unixPrompt;

            i = SendLine(line, promptList, out data, out result);

            return i;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int SendLine(string line, string[] prompt, out StringBuilder data, out string result)
        {
            int op;

            op = 0;
            result = "";
            data = new StringBuilder(1000);
            data.Length = 0;

            if (telnet.Connected)
            {
                try
                {
                    telnet.Send(line + "\r\n");
                    data.Append(telnet.WaitFor(prompt));
                    op = 1;
                }
                catch (Exception ex)
                {
                    data.Length = 0;
#if DEBUG
                    result = "Ia.Cl.Model.Telnet.Send(): " + ex.ToString();
#else
                    result = "Ia.Cl.Model.Telnet.Send(): " + ex.Message;
#endif
                    op = -1;
                }
            }
            else
            {
                result = "Ia.Cl.Model.Telnet.Send(): Not connected";
                op = -1;
            }

            return op;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string FormatAndCleanData(string s)
        {
            // convert any \n\r to \r\n
            s = s.Replace("\n\r", "\r\n");

            // convert all naked \n to \r\n (2-step process)
            s = s.Replace("\r\n", "\n");
            s = s.Replace("\n", "\r\n");

            // replace all Tabs with spaces
            s = s.Replace("\t", "     ");

            // remove <esc>[0m (all attributes off)
            s = s.Replace("\x1b[0m", "");

            // remove <esc>[m (all attributes off)
            s = s.Replace("\x1b[m", "");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Disconnect()
        {
            if (telnet.Connected) telnet.Close();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            Disconnect();

            telnet.Dispose();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
