using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO.Pipes;
using OpenPop.Pop3;
using OpenPop.Mime;
using Message = OpenPop.Mime.Message;
using MessageHeader = OpenPop.Mime.Header.MessageHeader;
using System.Collections;
using System.Data;

namespace Ia.I.Test.Wa
{
    /// <summary>
    /// POP support class.
    /// </summary>
    /// <remarks>
    /// Copyright © 2008-2013 Jasem Y. Al-Shamlan (info@ia.com.kw), Internet Applications - Kuwait. All Rights Reserved.
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
    public class Pop
    {
        Ia.Cs.This.Pop t;
        Ia.Cs.This.Data d;
        //PCComm.CommunicationManager comm = new PCComm.CommunicationManager();

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Initialize()
        {
            string[] p;

            p = SerialPort.GetPortNames();

            t = new Ia.Cs.This.Pop("*", "*", "*");
            d = new Ia.Cs.This.Data();

            if (p.Length > 0)
            {
                // below: populate combobox with available serial ports

                //foreach (string s in p) port_cb.Items.Add(s);

                //port_cb.SelectedIndex = 0;
            }

            //c = new PCComm.CommunicationManager("115200", , , "8", , rtb);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region AT

        /*
        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            //Console.WriteLine("Data Received:");
            //Console.Write(indata);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void Send_Click(object sender, EventArgs e)
        {

            SerialPort mySerialPort = new SerialPort("COM14");

            mySerialPort.BaudRate = 9600;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;

            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            mySerialPort.Open();

            //Console.WriteLine("Press any key to continue...");
            //Console.WriteLine();
            //Console.ReadKey();

            mySerialPort.WriteLine("at");
            //mySerialPort.Close();




            //c.WriteData(tx_tb.Text);



            /*
            string s, dn;
            SerialPort sp;

            dn = "99746645";

            try
            {
                sp = new SerialPort();

                if (sp.IsOpen) sp.Close();

                sp.PortName = port_cb.SelectedItem.ToString();
                sp.BaudRate = 9600;
                sp.Parity = Parity.None;
                sp.DataBits = 8;
                sp.StopBits = StopBits.One;
                sp.Handshake = Handshake.None; //.XOnXOff;
                sp.DtrEnable = true;
                sp.RtsEnable = true;

                sp.Open();

                if (!sp.IsOpen)
                {
                    MessageBox.Show("Serial port is not opened");
                }
                else
                {
                    //MessageBox.Show("Serial port is opened");

                    //sp.WriteLine("AT" + Environment.NewLine);
                    //sp.WriteLine("ATD=" + dn + ";" + Environment.NewLine);

                    sp.WriteLine(tx_tb.Text);

                    rx_tb.Text += sp.ReadLine();
                    rx_tb.Text += sp.ReadLine();

                    if (sp.IsOpen) sp.Close();
                }
            }
            catch (Exception ex)
            {
            }
            * /
        }
             */

        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region Pop

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public class Pop
        {
            private bool use_ssl;
            private int port;
            private string host_name, user_name, password;

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public Pop(string _host_name, int _port, bool _use_ssl, string _user_name, string _password)
                : base()
            {
                host_name = _host_name;
                port = _port;
                use_ssl = _use_ssl;
                user_name = _user_name;
                password = _password;
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public Pop(string _host_name, string _user_name, string _password)
                : base()
            {
                host_name = _host_name;
                port = 110;
                use_ssl = false;
                user_name = _user_name;
                password = _password;
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public void Retrieve_New_Message_Update_Data(Data d)
            {
                Hashtable message_id_ht;
                List<Message> l_m;

                // below: collect Hashtable of all message ids already read in the database
                message_id_ht = d.Message_Id_Ht();

                l_m = Message(message_id_ht);

                foreach (Message m in l_m)
                {
                    d.Add(m.Headers.MessageId, m.Headers.Subject, (m.MessagePart.Body != null) ? m.MessagePart.GetBodyAsText() : "");
                }
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            private List<Message> Message(Hashtable ht)
            {
                MessageHeader mh;
                List<Message> l_m;

                mh = null;
                l_m = null;

                // below: The pc disconnects from the server when being disposed
                using (Pop3Client pc = new Pop3Client())
                {
                    try
                    {
                        // below: connect to the server
                        pc.Connect(host_name, port, use_ssl);

                        // below: authenticate ourselves towards the server
                        pc.Authenticate(user_name, password, OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword);

                        // below:
                        l_m = new List<Message>(pc.GetMessageCount());

                        // below: first get all message headers; message numbers are 1-based.
                        for (int i = 1; i <= pc.GetMessageCount(); i++)
                        {
                            mh = pc.GetMessageHeaders(i);

                            // below: now we check if the message id from the message header exists. If not we will download whole message

                            if (!ht.ContainsKey(mh.MessageId)) l_m.Add(pc.GetMessage(i));
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                return l_m;
            }

            ////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region Data

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        class Data
        {
            DataTable dt;

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public Data()
                : base()
            {
                dt = new DataTable();
                dt.Columns.Add("id");
                dt.Columns.Add("message_id");
                dt.Columns.Add("subject");
                dt.Columns.Add("body");
                dt.Columns.Add("created");
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public void Add(string _message_id, string _subject, string _body)
            {
                DataRow r = dt.NewRow();

                r["id"] = "";
                r["message_id"] = _message_id;
                r["subject"] = _subject;
                r["body"] = _body;
                r["created"] = "";

                dt.Rows.Add(r);
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public Hashtable Message_Id_Ht()
            {
                Hashtable ht = new Hashtable(dt.Rows.Count);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        ht[r["message_id"].ToString()] = 0;
                    }
                }

                return ht;
            }

            ////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        #region AT

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        class At
        {
            private bool use_ssl;
            private int port;
            private string host_name, user_name, password;

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public At(string _host_name, int _port, bool _use_ssl, string _user_name, string _password)
                : base()
            {
                host_name = _host_name;
                port = _port;
                use_ssl = _use_ssl;
                user_name = _user_name;
                password = _password;
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public At(string _host_name, string _user_name, string _password)
                : base()
            {
                host_name = _host_name;
                port = 110;
                use_ssl = false;
                user_name = _user_name;
                password = _password;
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public void Process()
            {
                Hashtable message_id_ht;
                List<Message> l_m;

                // below: collect Hashtable of all message ids already read in the database
                // ...

                message_id_ht = new Hashtable(100);

                l_m = Message(message_id_ht);
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            private List<Message> Message(Hashtable ht)
            {
                MessageHeader mh;
                List<Message> l_m;

                mh = null;
                l_m = null;

                // below: The pc disconnects from the server when being disposed
                using (Pop3Client pc = new Pop3Client())
                {
                    try
                    {
                        // below: connect to the server
                        pc.Connect(host_name, port, use_ssl);

                        // below: authenticate ourselves towards the server
                        pc.Authenticate(user_name, password, OpenPop.Pop3.AuthenticationMethod.UsernameAndPassword);

                        // below:
                        l_m = new List<Message>(pc.GetMessageCount());

                        // below: first get all message headers; message numbers are 1-based.
                        for (int i = 1; i <= pc.GetMessageCount(); i++)
                        {
                            mh = pc.GetMessageHeaders(i);

                            // below: now we check if the message id from the message header exists. If not we will download whole message

                            if (!ht.ContainsKey(mh.MessageId)) l_m.Add(pc.GetMessage(i));
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                return l_m;
            }

            ////////////////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////////////////
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
