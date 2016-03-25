using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;

namespace Ia.Cl.Model.Azure
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Azure Cloud related support functions.
    /// </summary>
    /// <value>
    /// 
    /// Naming Stores:
    /// Experimentation suggests the rules for naming a queue include: (a) use only lower case letters, (b) digits are allowed anywhere, and (c) internal single hyphens are okay too, but (d) name should not contain any spaces (e) nor any punctuation (other than hyphen).
    /// http://blog.codingoutloud.com/2010/05/06/azure-error-one-of-the-request-inputs-is-out-of-range/
    /// 
    /// In web.config and app.config include:
    /// <appSettings>
    ///    <add key="applicationGuid" value="*" />
    ///    <add key="StorageConnectionString" value="DefaultEndpointsProtocol=https;AccountName=*;AccountKey=*;QueueEndpoint=http://*.queue.core.windows.net;" />
    /// </appSettings>
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

    ////////////////////////////////////////////////////////////////////////////

    public partial class Storage
    {
        private static CloudStorageAccount storageAccount;
        private static CloudQueueClient queueClient;
        private static CloudQueue queue;
        private static CloudQueueMessage message;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Storage() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void WriteContent(string queueName, string content)
        {
            // Retrieve storage account from connection string
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"].ToString());

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            message = new CloudQueueMessage(content);
            queue.AddMessage(message);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void WriteContent(string content)
        {
            string queueName;

            queueName = ConfigurationManager.AppSettings["applicationGuid"].ToString();

            WriteContent(queueName, content);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string PeekContent(string queueName)
        {
            // Retrieve storage account from connection string
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"].ToString());

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            message = null;

            if (queue.Exists())
            {
                // Peek at the next message
                message = queue.PeekMessage();
            }

            return (message != null) ? message.AsString : null;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string PeekContent()
        {
            string queueName;

            queueName = ConfigurationManager.AppSettings["applicationGuid"].ToString();

            return PeekContent(queueName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string UpdateContent(string queueName, string updatedContent)
        {
            // Retrieve storage account from connection string
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"].ToString());

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference(queueName);

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            // Get the message from the queue and update the message contents.
            message = queue.GetMessage();
            message.SetMessageContent(updatedContent);
            queue.UpdateMessage(message,
                TimeSpan.FromSeconds(0.0),  // Make it visible immediately.
                MessageUpdateFields.Content | MessageUpdateFields.Visibility);

            return message.AsString;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string UpdateContent(string updatedContent)
        {
            string queueName;

            queueName = ConfigurationManager.AppSettings["applicationGuid"].ToString();

            return UpdateContent(queueName, updatedContent);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void DeleteContent(string queueName)
        {
            // Retrieve storage account from connection string
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"].ToString());

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference(queueName);

            message = null;

            if (queue.Exists())
            {
                // Get the next message
                message = queue.GetMessage();

                // Process the message in less than 30 seconds, and then delete the message
                queue.DeleteMessage(message);
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void DeleteContent()
        {
            string queueName;

            queueName = ConfigurationManager.AppSettings["applicationGuid"].ToString();

            DeleteContent(queueName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void DeleteQueue(string queueName)
        {
            // Retrieve storage account from connection string
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"].ToString());

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference(queueName);

            if (queue.Exists())
            {
                // Delete the queue.
                queue.Delete();
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Test()
        {
            // Retrieve storage account from connection string
            storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"].ToString());

            // Create the queue client
            queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue
            queue = queueClient.GetQueueReference("myqueue");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            // Create a message and add it to the queue.
            message = new CloudQueueMessage("Hello, World");
            queue.AddMessage(message);


            // Peek at the next message
            message = queue.PeekMessage();
            // Display message.
            Console.WriteLine(message.AsString);


            // Get the message from the queue and update the message contents.
            message = queue.GetMessage();
            message.SetMessageContent("Updated contents.");
            queue.UpdateMessage(message,
                TimeSpan.FromSeconds(0.0),  // Make it visible immediately.
                MessageUpdateFields.Content | MessageUpdateFields.Visibility);

            // Get the next message
            message = queue.GetMessage();

            //Process the message in less than 30 seconds, and then delete the message
            queue.DeleteMessage(message);

            // Delete the queue.
            queue.Delete();
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
