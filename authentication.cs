using System;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Manage and verify user logging and passwords. The administrator will define the user's password and logging website. The service will issue a true of false according to authentication.
    /// </summary>
    /// 
    /// <code>
    /// bool b;
    /// Uri uri = new Uri("http://*");
    /// b = Ia.Cl.Model.Authentication.Validate("name", uri);
    /// </code>
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

    ////////////////////////////////////////////////////////////////////////////

    public partial class Authentication
    {
        /*
        static int seed;
        static Random r;

        static string number = "1234567890";
        static string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static string lowercaseLetters = "abcdefghijklmnopqrstuvwxyz";

        private const string traceCookieName = "traceCookie";
         */

        /// <summary/>
        public Authentication() { }

        /// <summary/>
        public int Id { get; set; }
        /// <summary/>
        public string Password { get; set; }
        /// <summary/>
        public string Host { get; set; }
        /// <summary/>
        public DateTime Created { get; set; }
        /// <summary/>
        public DateTime Updated { get; set; }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Create(Authentication newItem, out string result)
        {
            bool b;

            b = false;
            result = "";

            using (var db = new global::Ia.Cl.Model.Ia())
            {
                db.Authentications.Add(newItem);
                db.SaveChanges();

                b = true;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Create(string password, Uri url, Guid guid)
        {
            bool newItemCreated;
            string result;
            Authentication newItem;

            newItem = new Authentication();

            // insert new item
            newItem.Password = password;
            newItem.Host = global::Ia.Cl.Model.Default.ReadDomainFromUri(url);
            newItem.Created = DateTime.UtcNow.AddHours(3);

            newItemCreated = Authentication.Create(newItem, out result);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Validate a password by sending it and the Uri of the website to this function
        /// <param name="password">Password of the user</param>
        /// <param name="uri">Uri of webpage</param>
        /// </summary>
        public static bool Validate(string password, Uri uri)
        {
            bool validated;
            string host;
            Authentication item;

            host = global::Ia.Cl.Model.Default.ReadDomainFromUri(uri);

            using (var db = new global::Ia.Cl.Model.Ia())
            {
                // the matching is case-insensitive
                item = (from q in db.Authentications where q.Password.ToLower() == password.ToLower() && q.Host == host select q).SingleOrDefault();

                validated = (item != null);
            }

            return validated;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Authentication Read(int id)
        {
            Authentication item;

            using (var db = new global::Ia.Cl.Model.Ia())
            {
                item = (from q in db.Authentications where q.Id == id select q).SingleOrDefault();
            }

            return item;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<Authentication> ReadList()
        {
            List<Authentication> list;

            using (var db = new global::Ia.Cl.Model.Ia())
            {
                list = (from q in db.Authentications select q).ToList();
            }

            return list;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Update(Authentication updatedItem, out string result)
        {
            bool b;

            b = false;
            result = "";

            using (var db = new global::Ia.Cl.Model.Ia())
            {
                updatedItem = (from q in db.Authentications where q.Id == updatedItem.Id select q).SingleOrDefault();

                updatedItem.Updated = DateTime.UtcNow.AddHours(3);

                db.Authentications.Attach(updatedItem);

                var v = db.Entry(updatedItem);
                v.State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                b = true;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Delete(int id, out string result)
        {
            bool b;

            b = false;
            result = "";

            using (var db = new global::Ia.Cl.Model.Ia())
            {
                var v = (from q in db.Authentications where q.Id == id select q).FirstOrDefault();

                db.Authentications.Remove(v);
                db.SaveChanges();

                b = true;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Returns randomly generated strings in password and authenticate sting formats for copy and past purposes.
        /// The format is variable according to user preference.
        /// </summary>
        public static string RandomPasswordOfLength(int passwordLength)
        {
            string password;

            password = Guid.NewGuid().ToString().ToLower().Substring(0, passwordLength);

            return password;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ReturnStringWithNumberAndUpperAndLowerLetters(int length)
        {
            int i;
            string range, s;

            // for a 64 long string
            range = number + uppercaseLetters + lowercaseLetters; s = "";
            for (i = 0; i < length; i++) s += RandomCharacter(range.ToCharArray(), r);

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ReturnStringWithNumberAndUpperLetters(int length)
        {
            int i;
            string range, s;

            range = number + uppercaseLetters; s = "";

            for (i = 0; i < length; i++)
            {
                s += RandomCharacter(range.ToCharArray(), r);
                if ((i + 1) % 5 == 0 && (i + 1) < 30) s += "-";
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ReturnStringWithNumberAndLowerLetters(int length)
        {
            int i;
            string range, s;

            range = number + lowercaseLetters; s = "";

            for (i = 0; i < length; i++) s += RandomCharacter(range.ToCharArray(), r);

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static char RandomCharacter(char[] line, Random r)
        {
            // return a random char from line
            int i;
            i = r.Next(line.Length);
            return line[i];
        }
         */

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
