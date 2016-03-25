using System;
using System.Web;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Configuration;
using System.Net;
using System.Text;
using System.Net.Mail;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// SMTP send mail server suppot class.
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
    public class Smtp
    {
        private static bool b;
        private static SmtpClient smtpClient;
        private static MailAddress fromMailAddress, toMailAddress;
        private static MailMessage mailMessage;
        private enum MailType { Plain, Html };

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Smtp() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendPlain(string name, string email, string subject, string cc, string bcc, string content, out string result)
        {
            bool b;

            b = Send(name, email, subject, cc, bcc, content, MailType.Plain, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendHtml(string name, string email, string subject, string cc, string bcc, string content, out string result)
        {
            bool b;

            b = Send(name, email, subject, cc, bcc, content, MailType.Html, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendPlain(string name, string email, string subject, string cc, string content, out string result)
        {
            bool b;

            b = Send(name, email, subject, cc, null, content, MailType.Plain, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendHtml(string name, string email, string subject, string cc, string content, out string result)
        {
            bool b;

            b = Send(name, email, subject, cc, null, content, MailType.Html, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendPlain(string name, string email, string subject, string content, out string result)
        {
            bool b;

            b = Send(name, email, subject, null, null, content, MailType.Plain, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendHtml(string name, string email, string subject, string content, out string result)
        {
            bool b;

            b = Send(name, email, subject, null, null, content, MailType.Html, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendPlain(string name, string email, string subject, string content, string smtpServerHost, string smtpServerUserName, string smtpServerUser, string smtpServerPassword, bool enableSsl, out string result)
        {
            bool b;

            b = Send(name, email, subject, null, null, content, smtpServerHost, smtpServerUserName, smtpServerUser, smtpServerPassword, MailType.Plain, enableSsl, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendHtml(string name, string email, string subject, string content, string smtpServerHost, string smtpServerUserName, string smtpServerUser, string smtpServerPassword, bool enableSsl, out string result)
        {
            bool b;

            b = Send(name, email, subject, null, null, content, smtpServerHost, smtpServerUserName, smtpServerUser, smtpServerPassword, MailType.Html, enableSsl, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendPlain(string name, string email, string subject, string cc, string bcc, string content, string smtpServerHost, string smtpServerUserName, string smtpServerUser, string smtpServerPassword, bool enableSsl, out string result)
        {
            bool b;

            b = Send(name, email, subject, cc, bcc, content, smtpServerHost, smtpServerUserName, smtpServerUser, smtpServerPassword, MailType.Plain, enableSsl, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SendHtml(string name, string email, string subject, string cc, string bcc, string content, string smtpServerHost, string smtpServerUserName, string smtpServerUser, string smtpServerPassword, bool enableSsl, out string result)
        {
            bool b;

            b = Send(name, email, subject, cc, bcc, content, smtpServerHost, smtpServerUserName, smtpServerUser, smtpServerPassword, MailType.Html, enableSsl, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Send(string name, string email, string subject, string cc, string bcc, string content, string smtpServerHost, string smtpServerUserName, string smtpServerUser, string smtpServerPassword, string mailTypeString, bool enableSsl, out string result)
        {
            bool b;
            MailType mailType;

            result = "";

            if (mailTypeString == "html" || mailTypeString == "plain")
            {
                if (mailTypeString == "html") mailType = MailType.Html;
                else /*if (mail_type == "plain")*/ mailType = MailType.Plain;

                b = Send(name, email, subject, cc, bcc, content, smtpServerHost, smtpServerUserName, smtpServerUser, smtpServerPassword, mailType, enableSsl, out result);
            }
            else
            {
                b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static bool Send(string name, string email, string subject, string cc, string bcc, string content, MailType mail_type, out string result)
        {
            bool b, smtpServerEnableSsl;
            string smtpServerHost, smtpServerUserName, smtpServerUser, smtpServerPassword;

            /* 
             * app.config:
             * <appSettings>
             * <add key="smtpServerHost" value="*" />
             * <add key="smtpServerUserName" value="*" />
             * <add key="smtpServerUser" value="*" />
             * <add key="smtpServerPassword" value="*" />
             * <add key="smtpServerEnableSsl" value="*" />             
             */

            smtpServerUserName = ConfigurationManager.AppSettings["smtpServerUserName"].ToString();
            smtpServerHost = ConfigurationManager.AppSettings["smtpServerHost"].ToString();
            smtpServerUser = ConfigurationManager.AppSettings["smtpServerUser"].ToString();
            smtpServerPassword = ConfigurationManager.AppSettings["smtpServerPassword"].ToString();
            smtpServerEnableSsl = bool.TryParse(ConfigurationManager.AppSettings["smtpServerEnableSsl"].ToString(), out b) ? b : false;

            b = Send(name, email, subject, cc, bcc, content, smtpServerHost, smtpServerUserName, smtpServerUser, smtpServerPassword, mail_type, smtpServerEnableSsl, out result);

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static bool Send(string name, string email, string subject, string cc, string bcc, string content, string smtpServerHost, string smtpServerUserName, string smtpServerUser, string smtpServerPassword, MailType mailType, bool enableSsl, out string result)
        {
            //string send_state;

            b = false;

            smtpClient = new SmtpClient(smtpServerHost);

            smtpClient.EnableSsl = enableSsl;
            smtpClient.Port = 587;

            fromMailAddress = new MailAddress(smtpServerUser, smtpServerUserName, Encoding.UTF8);
            toMailAddress = new MailAddress(email, name, Encoding.UTF8);

            mailMessage = new MailMessage(fromMailAddress, toMailAddress);

            mailMessage.Subject = subject;

            if (cc != null)
            {
                foreach (string s in cc.Split(';'))
                {
                    // mm.CC.Add(new MailAddress(s, name, Encoding.UTF8));
                }
            }

            if (bcc != null)
            {
                foreach (string s in bcc.Split(';'))
                {
                    mailMessage.Bcc.Add(new MailAddress(s)); //, name, Encoding.UTF8));
                }
            }

            mailMessage.SubjectEncoding = Encoding.UTF8;

            smtpClient.UseDefaultCredentials = false;

            NetworkCredential c = new NetworkCredential(smtpServerUser, smtpServerPassword);

            smtpClient.Credentials = c;

            result = "";

            if (mailType == MailType.Html || mailType == MailType.Plain)
            {
                mailMessage.Body = content;

                if (mailType == MailType.Html) mailMessage.IsBodyHtml = true;
                else if (mailType == MailType.Plain) mailMessage.IsBodyHtml = false;

                mailMessage.BodyEncoding = Encoding.UTF8;

                try
                {
                    //send_state = "send without blocking but no guarantee code";
                    smtpClient.Send(mailMessage); //.SendAsync(mm,send_state);
                    b = true;
                }
                catch (SmtpException ex)
                {
#if DEBUG
                    result = "Error in global::Ia.Cl.Model.Mail.Send(): Your email couldn't be sent: " + ex.ToString();
#else
                    result = "Error in global::Ia.Cl.Model.Mail.Send(): Your email couldn't be sent: " + ex.Message;
#endif
                    b = false;
                }
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
