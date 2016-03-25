using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Data.Linq.Mapping;
using System.ComponentModel.DataAnnotations;

namespace Ia.Cl.Model.Life
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Individual object.
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

    /// <summary>
    ///
    /// </summary>
    public enum Gender
    {
        Male = 1, Female = 2, Unknown = 3, PreferNotToSay = 4
    }

    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public partial class Individual
    {
        //private Social_State social_state;
        //private Educational_State educational_state;
        //private string religion;
        //private Blood blood;
        //private int civil_id_number;

        /// <summary/>
        public Individual() { }

        /// <summary/>
        public int Id { get; set; }
        /// <summary/>
        public int TypeId { get; set; }
        /// <summary/>
        public int[] LanguageId { get; set; }
        /// <summary/>
        public string FirstName { get; set; }
        /// <summary/>
        public string MiddleName { get; set; }
        /// <summary/>
        public string LastName { get; set; }
        /// <summary/>
        public string Description { get; set; }
        /// <summary/>
        public int? GenderId { get; set; }
        /// <summary/>
        public DateTime? DateOfBirth { get; set; }
        /// <summary/>
        public string Note { get; set; }
        /// <summary/>
        public DateTime Created { get; set; }
        /// <summary/>
        public DateTime Updated { get; set; }
        /// <summary/>
        public DateTime Inspected { get; set; }
        /// <summary/>
        public Guid Guid { get; set; }

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
        ///
        /// </summary>
        public static bool Validate(string password, Uri url)
        {
            bool validated;
            string host;
            Authentication item;

            host = global::Ia.Cl.Model.Default.ReadDomainFromUri(url);

            using (var db = new global::Ia.Cl.Model.Ia())
            {
                item = (from q in db.Authentications where q.Password == password && q.Host == host select q).SingleOrDefault();

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
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}
