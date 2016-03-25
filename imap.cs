using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using ActiveUp.Net.Imap4;
using ActiveUp.Net.Mail;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// IMAP support class.
    /// </summary>
    /// 
    /// <see cref="http://www.skytale.net/blog/archives/23-Manual-IMAP.html"/>
    /// <see cref="http://tools.ietf.org/html/rfc3501#page-51"/>
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
    public class Imap
    {
        private bool serverEnableSsl;
        private string serverHost, serverUser, serverPassword, defaultMailbox;
        private Imap4Client imap;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public struct Message
        {
            /// <summary/>
            public string MessageId { get; set; }
            /// <summary/>
            public string From { get; set; }
            /// <summary/>
            public string Subject { get; set; }
            /// <summary/>
            public string BodyText { get; set; }
            /// <summary/>
            public DateTime Date { get; set; }
            /// <summary/>
            public DateTime ReceivedDate { get; set; }

            /// <summary/>
            public Message(string messageId, string from, string subject, string bodyText)
                : this()
            {
                this.MessageId = messageId;
                this.From = from;
                this.Subject = subject;
                this.BodyText = bodyText;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public struct Header
        {
            /// <summary/>
            public string MessageId { get; set; }
            /// <summary/>
            public string From { get; set; }
            /// <summary/>
            public string Subject { get; set; }
            /// <summary/>
            public DateTime Date { get; set; }
            /// <summary/>
            public DateTime ReceivedDate { get; set; }

            /// <summary/>
            public Header(string messageId, string from, string subject)
                : this()
            {
                this.MessageId = messageId;
                this.From = from;
                this.Subject = subject;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Imap()
        {
            bool b;

            /*
             * app.config
             * <add key="imapServerHost" value="*" />
             * <add key="imapServerUser" value="*" />
             * <add key="imapServerPassword" value="*" />
             * <add key="imapServerEnableSsl" value="*" /> 
             */

            serverHost = ConfigurationManager.AppSettings["imapServerHost"].ToString();
            serverUser = ConfigurationManager.AppSettings["imapServerUser"].ToString();
            serverPassword = ConfigurationManager.AppSettings["imapServerPassword"].ToString();
            serverEnableSsl = bool.TryParse(ConfigurationManager.AppSettings["imapServerEnableSsl"].ToString(), out b) ? b : false;

            Initialize();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Imap(string _server, string _user, string _password, bool _useSsl)
        {
            this.serverHost = _server;
            this.serverUser = _user;
            this.serverPassword = _password;
            this.serverEnableSsl = _useSsl;

            Initialize();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        ~Imap()
        {
            Disconnect();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void Initialize()
        {
            defaultMailbox = "Inbox";

            imap = new Imap4Client();

            this.Log("Initialize(): Imap object created");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Connect()
        {
            if (serverEnableSsl) imap.ConnectSsl(serverHost);
            else imap.Connect(serverHost);

            this.Log("Connect(): Connection opened to " + serverHost);

            imap.Login(serverUser, serverPassword);

            this.Log(string.Format("Connect(): Login to '{0}' by user '{1}' successful", serverHost, serverUser));
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public bool IsConnected
        {
            get
            {
                this.Log("IsConnected: " + imap.IsConnected.ToString());

                return imap.IsConnected;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Disconnect()
        {
            if (imap.IsConnected)
            {
                try
                {
                    imap.Disconnect();

                    this.Log("Disconnect(): Disconnected");
                }
                catch (Exception ex)
                {
                    this.Log("Disconnect(): Exception: " + ex.ToString());
                }
            }
            else
            {
                this.Log("Disconnect(): Already disconnected");
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public List<string> MailboxList()
        {
            List<string> mailboxList;

            mailboxList = new List<string>(imap.Mailboxes.Count);

            foreach (Mailbox mailbox in imap.Mailboxes) mailboxList.Add(mailbox.Name);

            return mailboxList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void CreateMailbox(string mailboxName)
        {
            imap.CreateMailbox(mailboxName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string DeleteMailbox(string mailboxName)
        {
            return imap.DeleteMailbox(mailboxName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Header ReadHeader(Mailbox mailbox, int i)
        {
            Header header;
            ActiveUp.Net.Mail.Header aHeader;

            header = new Header();

            try
            {
                aHeader = mailbox.Fetch.HeaderObject(i);

                header.MessageId = aHeader.MessageId;
                header.From = aHeader.From.Email;
                header.Subject = aHeader.Subject;

                this.Log(string.Format("Success: Header read: {0},{1},{2}", header.MessageId, header.From, header.Subject));
            }
            catch (Imap4Exception iex)
            {
                this.Log(string.Format("Imap4 Error: {0}", iex.Message));
            }
            catch (Exception ex)
            {
                this.Log(string.Format("Failed: {0}", ex.Message));
            }
            finally
            {
            }

            return header;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public int MoveMessagesFromEmailToMailbox(string email, string destinationMailboxName)
        {
            int numberOfMessagesMoved;
            int[] messageOrdinalList;
            string searchPhrase;
            Mailbox mailbox;

            numberOfMessagesMoved = 0;

            mailbox = imap.SelectMailbox(defaultMailbox);

            searchPhrase = @"FROM """ + email + @"""";

            messageOrdinalList = mailbox.Search(searchPhrase);

            if (messageOrdinalList != null && messageOrdinalList.Length > 0)
            {
                // read message and check that from-email value before moving
                for (int i = messageOrdinalList.Length - 1; i >= 0; i--)
                {
                    MoveMessage(mailbox, messageOrdinalList[i], destinationMailboxName);
                    numberOfMessagesMoved++;
                }
            }

            return numberOfMessagesMoved;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void MoveMessage(string messageId, string destinationMailboxName)
        {
            int[] messageOrdinalList;
            string searchPhrase;
            Mailbox mailbox;
            ActiveUp.Net.Mail.Header header;

            mailbox = imap.SelectMailbox(defaultMailbox);

            searchPhrase = @"ALL";

            messageOrdinalList = mailbox.Search(searchPhrase);

            if (messageOrdinalList != null && messageOrdinalList.Length > 0)
            {
                for (int i = messageOrdinalList.Length - 1; i >= 0; i--)
                {
                    header = mailbox.Fetch.HeaderObject(messageOrdinalList[i]);

                    if (header.MessageId == messageId)
                    {
                        MoveMessage(mailbox, messageOrdinalList[i], destinationMailboxName);

                        break;
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void MoveMessage(Mailbox mailbox, int messageOrdinal, string destinationMailboxName)
        {
            mailbox.MoveMessage(messageOrdinal, destinationMailboxName);

            this.Log("Move message: " + messageOrdinal + " to mailbox '" + destinationMailboxName + "'");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Message list from Inbox
        /// </summary>
        public void MessageList(out List<Message> messageList)
        {
            string searchPhrase;

            searchPhrase = "ALL";

            SearchPhraseMessageList(searchPhrase, out messageList);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// List of messages from Inbox that were sent from email
        /// </summary>
        public void MessageList(string email, out List<Message> messageList)
        {
            string searchPhrase;

            searchPhrase = @"FROM """ + email + @"""";

            SearchPhraseMessageList(searchPhrase, out messageList);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void SearchPhraseMessageList(string searchPhrase, out List<Message> messageList)
        {
            Message message;
            Mailbox mailbox;
            MessageCollection messageCollection;

            messageList = new List<Message>();

            mailbox = imap.SelectMailbox(defaultMailbox);

            try
            {
                messageCollection = mailbox.SearchParse(searchPhrase);

                foreach (ActiveUp.Net.Mail.Message m in messageCollection)
                {
                    message = new Message();

                    message.MessageId = m.MessageId;
                    message.From = m.From.Email;
                    message.Subject = m.Subject;
                    message.BodyText = m.BodyText.TextStripped;
                    message.Date = m.Date;
                    message.ReceivedDate = m.ReceivedDate;

                    messageList.Add(message);

                    this.Log(string.Format("Success: Message read: {0},{1},{2}", m.MessageId, m.From, m.Subject));
                }
            }
            catch (Imap4Exception iex)
            {
                this.Log(string.Format("Imap4 Error: {0}", iex.Message));
            }
            catch (Exception ex)
            {
                this.Log(string.Format("Failed: {0}", ex.Message));
            }
            finally
            {
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public List<Header> HeaderList(Mailbox mailbox)
        {
            Header header;
            List<Header> headerList;
            ActiveUp.Net.Mail.Header aHeader;

            headerList = new List<Header>(mailbox.MessageCount);

            try
            {
                for (int i = 1; i <= mailbox.MessageCount; i++)
                {
                    header = new Header();

                    aHeader = mailbox.Fetch.MessageObject(i);

                    header.MessageId = aHeader.MessageId;
                    header.From = aHeader.From.Email;
                    header.Subject = aHeader.Subject;

                    this.Log(string.Format("Success: Header read: {0},{1},{2}", header.MessageId, header.From, header.Subject));
                }
            }
            catch (Imap4Exception iex)
            {
                this.Log(string.Format("Imap4 Error: {0}", iex.Message));
            }
            catch (Exception ex)
            {
                this.Log(string.Format("Failed: {0}", ex.Message));
            }
            finally
            {
            }

            return headerList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void DeleteMessage(string messageId)
        {
            int[] messageOrdinalList;
            string searchPhrase;
            Mailbox mailbox;
            ActiveUp.Net.Mail.Header header;

            mailbox = imap.SelectMailbox(defaultMailbox);

            searchPhrase = @"ALL";

            messageOrdinalList = mailbox.Search(searchPhrase);

            if (messageOrdinalList != null && messageOrdinalList.Length > 0)
            {
                for (int i = messageOrdinalList.Length - 1; i >= 0; i--)
                {
                    header = mailbox.Fetch.HeaderObject(messageOrdinalList[i]);

                    if (header.MessageId == messageId)
                    {
                        DeleteMessage(mailbox, messageOrdinalList[i]);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void DeleteMessage(Mailbox mailbox, int messageOrdinal)
        {
            mailbox.DeleteMessage(messageOrdinal, true);

            this.Log("Delete message: " + messageOrdinal);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void Log(string text)
        {
            string line;
            DateTime now;

            now = DateTime.UtcNow.AddHours(3);

            line = now.ToString("yyyy-MM-dd HH:mm:ss: ") + text;

            Console.WriteLine(line);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
