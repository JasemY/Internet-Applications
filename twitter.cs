using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Configuration;
using TweetSharp;
using Hammock;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Twitter API support class.
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
    public class Twitter
    {
        private string consumerKey, consumerSecret, accessToken, accessTokenSecret;
        private TwitterService service;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Twitter()
        {
            /*
             * <appSettings>
             * <add key="twitterConsumerKey" value="*" />
             * <add key="twitterConsumerSecret" value="*" />
             * <add key="twitterAccessToken" value="*" />
             * <add key="twitterAccessTokenSecret" value="*" />
             */

            consumerKey = ConfigurationManager.AppSettings["twitterConsumerKey"].ToString();
            consumerSecret = ConfigurationManager.AppSettings["twitterConsumerSecret"].ToString();
            accessToken = ConfigurationManager.AppSettings["twitterAccessToken"].ToString();
            accessTokenSecret = ConfigurationManager.AppSettings["twitterAccessTokenSecret"].ToString();

            Initialize();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Twitter(string _consumerKey, string _consumerSecret, string _accessToken, string _accessTokenSecret)
        {
            consumerKey = _consumerKey;
            consumerSecret = _consumerSecret;
            accessToken = _accessToken;
            accessTokenSecret = _accessTokenSecret;

            Initialize();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private void Initialize()
        {
            // In v1.1, all API calls require authentication
            service = new TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(accessToken, accessTokenSecret);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public List<TwitterStatus> TweetListAboveId(long aboveTweetId)
        {
            List<TwitterStatus> tweetList;
            IEnumerable<TwitterStatus> iEnumerableTweetList;

            tweetList = new List<TwitterStatus>();
            iEnumerableTweetList = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());

            foreach (TwitterStatus tweet in iEnumerableTweetList)
            {
                if (tweet.Id > aboveTweetId) tweetList.Add(tweet);
            }

            return tweetList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public List<TwitterDirectMessage> DirectMessagesReceivedListAboveId(long aboveDmId)
        {
            List<TwitterDirectMessage> dmList;
            IEnumerable<TwitterDirectMessage> iEnumerableDmList;

            dmList = new List<TwitterDirectMessage>();
            iEnumerableDmList = service.ListDirectMessagesReceived(new ListDirectMessagesReceivedOptions());

            foreach (TwitterDirectMessage dm in iEnumerableDmList)
            {
                if (dm.Id > aboveDmId) dmList.Add(dm);
            }

            return dmList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public List<TwitterStatus> TweetList()
        {
            // 
            List<TwitterStatus> tweetList;
            IEnumerable<TwitterStatus> iEnumerableTweetList;

            tweetList = new List<TwitterStatus>();
            iEnumerableTweetList = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());

            foreach (TwitterStatus tweet in tweetList)
            {
                tweetList.Add(tweet);
            }

            return tweetList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public List<TwitterDirectMessage> DirectMessageList()
        {
            // 
            List<TwitterDirectMessage> dmList;
            IEnumerable<TwitterDirectMessage> iEnumerableTweetList;

            dmList = new List<TwitterDirectMessage>();
            iEnumerableTweetList = service.ListDirectMessagesReceived(new ListDirectMessagesReceivedOptions());

            foreach (TwitterDirectMessage dm in dmList)
            {
                dmList.Add(dm);
            }

            return dmList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public TwitterStatus ReadLastTweet()
        {
            // send tweet
            TwitterStatus ts;
            IEnumerable<TwitterStatus> tweetList;

            ts = null;
            tweetList = service.ListTweetsOnHomeTimeline(new ListTweetsOnHomeTimelineOptions());

            foreach (TwitterStatus tweet in tweetList)
            {
                ts = tweet;
                break; // to read only the first;
            }

            return ts;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Delete Tweet (this function is not working properly)
        /// <param name="tweetId">Tweet Id to delete</param>
        /// <see cref="http://stackoverflow.com/questions/4810076/tweetsharp-remove-undo-retweet"/>
        /// </summary>
        public TwitterStatus DeleteTweet(long tweetId)
        {
            // delete tweet
            TwitterStatus ts;

            ts = service.DeleteTweet(new DeleteTweetOptions() { Id = tweetId });

            return ts;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Delete DM
        /// </summary>
        public TwitterDirectMessage DeleteDirectMessage(long dmId)
        {
            // delete tweet
            TwitterDirectMessage dm;

            dm = service.DeleteDirectMessage(new DeleteDirectMessageOptions() { Id = dmId });

            return dm;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void SendTweet(string status)
        {
            // send tweet

            SendTweetOptions sto;

            sto = new SendTweetOptions();

            sto.Status = status; // "Testing (2)...";

            service.SendTweet(sto);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void SendDirectMessage(string screenName, string text)
        {
            // direct message

            SendDirectMessageOptions sdmo;

            sdmo = new SendDirectMessageOptions();

            sdmo.ScreenName = screenName; // "@abcd";
            sdmo.Text = text; // "Testing (2)...";

            service.SendDirectMessage(sdmo);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
