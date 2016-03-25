using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Web.Security;

// for NewIdentity
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// ASP.NET Identity support class.
    /// </summary>
    /// <value>
    /// Identity: http://www.asp.net/identity/overview/getting-started/introduction-to-aspnet-identity
    /// </value>
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
    public class NewIdentity
    {
        private static Dictionary<Guid, string> userList;
        private static List<IdentityUser> identityUserList;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static IdentityUser CreateUser(string userName, string password, string email, DbContext context, out IdentityResult identityResult)
        {
            return CreateUser(userName, password, email, null, null, context, out identityResult);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static IdentityUser CreateUserWithNoEmailNoPassword(string userName, DbContext context, out IdentityResult identityResult)
        {
            return CreateUser(userName, null, null, null, null, context, out identityResult);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static IdentityUser CreateUser(string userName, string password, string email, string question, string answer, DbContext context, out IdentityResult identityResult)
        {
            // Default UserStore constructor uses the default connection string named: DefaultConnection
            IdentityUser identityUser;
            UserStore<IdentityUser> userStore;
            UserManager<IdentityUser> userManager;

            userStore = new UserStore<IdentityUser>(context);
            userManager = new UserManager<IdentityUser>(userStore);

            identityUser = new IdentityUser();

            identityUser.UserName = userName;
            if (email != null) identityUser.Email = email;

            identityResult = (password != null) ? userManager.Create(identityUser, password) : userManager.Create(identityUser);

            return identityUser;
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static MembershipUserCollection GetAllUsers(DbContext context)
        {
            MembershipUserCollection memberList;

            memberList = Membership.GetAllUsers();

            return memberList;
        }
         */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static IdentityUser IdentityUser(string userName, DbContext context)
        {
            UserStore<IdentityUser> userStore;
            IdentityUser identityUser;

            userStore = new UserStore<IdentityUser>(context);

            identityUser = (from q in userStore.Users where q.UserName == userName select q).SingleOrDefault();

            return identityUser;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<IdentityUser> IdentityUserList(DbContext context)
        {
            UserStore<IdentityUser> userStore;
            //UserManager<IdentityUser> userManager;

            userStore = new UserStore<IdentityUser>(context);
            //userManager = new UserManager<IdentityUser>(userStore);

            identityUserList = userStore.Users.ToList();

            return identityUserList.ToList();
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Dictionary<Guid, string> UserList(DbContext context)
        {
                MembershipUserCollection membershipUserList;

                if (userList == null || userList.Count == 0)
                {
                    membershipUserList = GetAllUsers(context);

                    userList = new Dictionary<Guid, string>(membershipUserList.Count);

                    userList.Add(Guid.Empty, "");

                    foreach(MembershipUser mu in membershipUserList)
                    {
                        userList.Add(Guid.Parse(mu.ProviderUserKey.ToString()), mu.UserName);
                    }
                }

                return userList.ToList();
        }
         */

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static MembershipUser GetUser(object o)
        {
            MembershipUser membershipUser;

            membershipUser = System.Web.Security.Membership.GetUser(o);

            return membershipUser;
        }
         */

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool UpdateUser(string userName, string oldPassword, string newPassword, string email, DbContext context, out string result)
        {
            bool b;
            MembershipUser membershipUser;

            b = false;
            result = "";
            userList = null; // to force refresh in UserList

            membershipUser = System.Web.Security.Membership.GetUser(userName);

            if (oldPassword.Length > 0 && newPassword.Length > 0)
            {
                b = membershipUser.ChangePassword(oldPassword, newPassword);
            }

            membershipUser.Email = email;

            System.Web.Security.Membership.UpdateUser(membershipUser);

            b = true;

            return b;
        }
         */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void DeleteUser(string userName, DbContext context, out IdentityResult identityResult)
        {
            IdentityUser identityUser;
            UserStore<IdentityUser> userStore;
            UserManager<IdentityUser> userManager;

            userStore = new UserStore<IdentityUser>(context);
            userManager = new UserManager<IdentityUser>(userStore);

            identityUser = userManager.FindByName(userName);

            identityResult = userManager.Delete(identityUser);
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool ValidateUser(string userName, string password, out string result)
        {
            bool b;

            result = "";
            b = System.Web.Security.Membership.ValidateUser(userName, password);

            if (b)
            {
            }
            else
            {
                result = "Error: User credentials invalid. ";
            }

            return b;
        }
         */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Translate MembershipCreateStatus enum value into text
        /// </summary>
        public static string MembershipCreateStatusText(MembershipCreateStatus membershipCreateStatus)
        {
            string s;

            switch (membershipCreateStatus)
            {
                case MembershipCreateStatus.DuplicateEmail: s = "The e-mail address already exists in the database for the application. "; break;
                case MembershipCreateStatus.DuplicateProviderUserKey: s = "The provider user key already exists in the database for the application. "; break;
                case MembershipCreateStatus.DuplicateUserName: s = "The user name already exists in the database for the application. "; break;
                case MembershipCreateStatus.InvalidAnswer: s = "The password retrieval answer provided is invalid. "; break;
                case MembershipCreateStatus.InvalidEmail: s = "The e-mail address is not formatted correctly. "; break;
                case MembershipCreateStatus.InvalidPassword: s = "The password is not formatted correctly. "; break;
                case MembershipCreateStatus.InvalidProviderUserKey: s = "The provider user key is of an invalid type or format. "; break;
                case MembershipCreateStatus.InvalidQuestion: s = "The password retrieval question provided is invalid. "; break;
                case MembershipCreateStatus.InvalidUserName: s = "The user name provided is invalid. "; break;
                case MembershipCreateStatus.ProviderError: s = "The authentication provider returned an error. "; break;
                case MembershipCreateStatus.Success: s = "The user was successfully created. "; break;
                case MembershipCreateStatus.UserRejected: s = "The user creation request has been canceled. "; break;
                default: s = ""; break;
            }

            /*
public string GetErrorMessage(MembershipCreateStatus status)
{
   switch (status)
   {
      case MembershipCreateStatus.DuplicateUserName:
        return "Username already exists. Please enter a different user name.";

      case MembershipCreateStatus.DuplicateEmail:
        return "A username for that e-mail address already exists. Please enter a different e-mail address.";

      case MembershipCreateStatus.InvalidPassword:
        return "The password provided is invalid. Please enter a valid password value.";

      case MembershipCreateStatus.InvalidEmail:
        return "The e-mail address provided is invalid. Please check the value and try again.";

      case MembershipCreateStatus.InvalidAnswer:
        return "The password retrieval answer provided is invalid. Please check the value and try again.";

      case MembershipCreateStatus.InvalidQuestion:
        return "The password retrieval question provided is invalid. Please check the value and try again.";

      case MembershipCreateStatus.InvalidUserName:
        return "The user name provided is invalid. Please check the value and try again.";

      case MembershipCreateStatus.ProviderError:
        return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

      case MembershipCreateStatus.UserRejected:
        return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

      default:
        return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
   }
}             */

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<IdentityRole> RoleList(DbContext context)
        {
            RoleStore<IdentityRole> roleStore;
            RoleManager<IdentityRole> roleManager;
            List<IdentityRole> identityRoleList;

            roleStore = new RoleStore<IdentityRole>(context);
            roleManager = new RoleManager<IdentityRole>(roleStore);

            identityRoleList = roleManager.Roles.ToList();

            return identityRoleList.ToList();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void RemoveUserFromRole(string userName, string roleName)
        {
            Roles.RemoveUserFromRole(userName, roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void CreateRole(string roleName)
        {
            Roles.CreateRole(roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool DeleteRole(string roleName)
        {
            return Roles.DeleteRole(roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void AddUserToRole(string userName, string roleName)
        {
            Roles.AddUserToRole(userName, roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void AddUserToRoles(string userName, string[] roleNames)
        {
            Roles.AddUserToRoles(userName, roleNames);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string[] UsersInRole(string roleName)
        {
            return Roles.GetUsersInRole(roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string[] RolesForUser(string userName)
        {
            return Roles.GetRolesForUser(userName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static string ExtractFirstNameAndChangeToLowerCase(string name)
        {
            int indexOfFirstSpace;
            string s;

            s = null;

            if (name != null && name != string.Empty)
            {
                name = name.Trim().ToLower();

                indexOfFirstSpace = name.IndexOf(' ');

                if (indexOfFirstSpace < 0) indexOfFirstSpace = name.Length - 1;

                s = name.Substring(0, indexOfFirstSpace + 1);
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////




    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///
    /// </summary>
    public class Identity
    {
        private static Dictionary<Guid, string> userNameDictionary;
        private static List<User> userList;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public class User
        {
            /// <summary/>
            public User() { }

            /// <summary/>
            public Guid ProviderUserKey { get; set; }
            /// <summary/>
            public string UserName { get; set; }
            /// <summary/>
            public string Email { get; set; }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Dictionary<Guid, string> UserNameDictionary
        {
            get
            {
                if (userNameDictionary == null || userNameDictionary.Count == 0)
                {
                    if (HttpContext.Current != null && HttpContext.Current.Application["userNameDictionary"] != null)
                    {
                        userNameDictionary = (Dictionary<Guid, string>)HttpContext.Current.Application["userNameDictionary"];
                    }
                    else
                    {
                        userNameDictionary = global::Ia.Cl.Model.Identity._UserNameDictionary;

                        if (HttpContext.Current != null) HttpContext.Current.Application["userNameDictionary"] = userNameDictionary;
                    }
                }

                return userNameDictionary;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static Dictionary<Guid, string> _UserNameDictionary
        {
            get
            {
                MembershipUserCollection memberList;

                if (userNameDictionary == null || userNameDictionary.Count == 0)
                {
                    memberList = GetAllUsers();

                    userNameDictionary = new Dictionary<Guid, string>(memberList.Count);
                    userNameDictionary.Clear();

                    userNameDictionary.Add(Guid.Empty, "");

                    foreach (MembershipUser mu in memberList)
                    {
                        userNameDictionary.Add(Guid.Parse(mu.ProviderUserKey.ToString()), mu.UserName);
                    }
                }

                return userNameDictionary;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static List<User> UserList
        {
            get
            {
                if (userList == null || userList.Count == 0)
                {
                    if (HttpContext.Current != null && HttpContext.Current.Application["userList"] != null)
                    {
                        userList = (List<User>)HttpContext.Current.Application["userList"];
                    }
                    else
                    {
                        userList = global::Ia.Cl.Model.Identity._UserList;

                        if (HttpContext.Current != null) HttpContext.Current.Application["userList"] = userList;
                    }
                }

                return userList;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static List<User> _UserList
        {
            get
            {
                if (userList == null || userList.Count == 0)
                {
                    User user;
                    MembershipUserCollection membershipUserCollection;
                    userList = new List<User>();

                    userList.Clear();
                    membershipUserCollection = Membership.GetAllUsers();

                    foreach (MembershipUser mu in membershipUserCollection)
                    {
                        user = new User();

                        user.ProviderUserKey = Guid.Parse(mu.ProviderUserKey.ToString());
                        user.UserName = mu.UserName;
                        user.Email = (!mu.Email.Contains("kuix.com")) ? mu.Email : null;

                        userList.Add(user);
                    }
                }

                return userList.ToList();
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static MembershipUserCollection GetAllUsers()
        {
            MembershipUserCollection memberList;

            memberList = Membership.GetAllUsers();

            return memberList;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static MembershipUser GetUser(object o)
        {
            MembershipUser membershipUser;

            membershipUser = System.Web.Security.Membership.GetUser(o);

            return membershipUser;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static MembershipUser CreateUser(string userName, string password, string email, out MembershipCreateStatus membershipCreateStatus)
        {
            return CreateUser(userName, password, email, "Q", "A", out membershipCreateStatus);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static MembershipUser CreateUser(string userName, string password, string email, string question, string answer, out MembershipCreateStatus membershipCreateStatus)
        {
            MembershipUser membershipUser;

            membershipUser = null;
            userList = null; // to force refresh in UserList

            //try
            //{
            membershipUser = System.Web.Security.Membership.CreateUser(userName, password, email, question, answer, true, out membershipCreateStatus);

            /*
            // If user created successfully, set password question and answer (if applicable) and 
            // redirect to login page. Otherwise return an error message.

            if (Membership.RequiresQuestionAndAnswer)
            {
                newUser.ChangePasswordQuestionAndAnswer(PasswordTextbox.Text,
                                                        PasswordQuestionTextbox.Text,
                                                        PasswordAnswerTextbox.Text);
            }

            Response.Redirect("login.aspx");
             */
            //}
            //catch (MembershipCreateUserException e)
            //{
            //Msg.Text = GetErrorMessage(e.StatusCode);
            //}
            //catch (HttpException e)
            //{
            //Msg.Text = e.Message;
            //}

            return membershipUser;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void UpdateUser(MembershipUser membershipUser)
        {
            System.Web.Security.Membership.UpdateUser(membershipUser);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool UpdateUser(string userName, string oldPassword, string newPassword, string email, out string result)
        {
            bool b;
            MembershipUser membershipUser;

            b = false;
            result = "";
            userList = null; // to force refresh in UserList

            membershipUser = System.Web.Security.Membership.GetUser(userName);

            if (oldPassword.Length > 0 && newPassword.Length > 0)
            {
                b = membershipUser.ChangePassword(oldPassword, newPassword);
            }

            membershipUser.Email = email;

            System.Web.Security.Membership.UpdateUser(membershipUser);

            b = true;

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool DeleteUser(string userName, out string result)
        {
            bool b;

            result = "";
            userList = null; // to force refresh in UserList

            b = System.Web.Security.Membership.DeleteUser(userName, true);

            if (b)
            {
            }
            else
            {
                result = "Error: Could not delete user. ";
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool ValidateUser(string userName, string password, out string result)
        {
            bool b;

            result = "";
            b = System.Web.Security.Membership.ValidateUser(userName, password);

            if (b)
            {
            }
            else
            {
                result = "Error: User credentials invalid. ";
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Translate MembershipCreateStatus enum value into text
        /// </summary>
        public static string MembershipCreateStatusText(MembershipCreateStatus membershipCreateStatus)
        {
            string s;

            switch (membershipCreateStatus)
            {
                case MembershipCreateStatus.DuplicateEmail: s = "The e-mail address already exists in the database for the application. "; break;
                case MembershipCreateStatus.DuplicateProviderUserKey: s = "The provider user key already exists in the database for the application. "; break;
                case MembershipCreateStatus.DuplicateUserName: s = "The user name already exists in the database for the application. "; break;
                case MembershipCreateStatus.InvalidAnswer: s = "The password retrieval answer provided is invalid. "; break;
                case MembershipCreateStatus.InvalidEmail: s = "The e-mail address is not formatted correctly. "; break;
                case MembershipCreateStatus.InvalidPassword: s = "The password is not formatted correctly. "; break;
                case MembershipCreateStatus.InvalidProviderUserKey: s = "The provider user key is of an invalid type or format. "; break;
                case MembershipCreateStatus.InvalidQuestion: s = "The password retrieval question provided is invalid. "; break;
                case MembershipCreateStatus.InvalidUserName: s = "The user name provided is invalid. "; break;
                case MembershipCreateStatus.ProviderError: s = "The authentication provider returned an error. "; break;
                case MembershipCreateStatus.Success: s = "The user was successfully created. "; break;
                case MembershipCreateStatus.UserRejected: s = "The user creation request has been canceled. "; break;
                default: s = ""; break;
            }

            /*
public string GetErrorMessage(MembershipCreateStatus status)
{
   switch (status)
   {
      case MembershipCreateStatus.DuplicateUserName:
        return "Username already exists. Please enter a different user name.";

      case MembershipCreateStatus.DuplicateEmail:
        return "A username for that e-mail address already exists. Please enter a different e-mail address.";

      case MembershipCreateStatus.InvalidPassword:
        return "The password provided is invalid. Please enter a valid password value.";

      case MembershipCreateStatus.InvalidEmail:
        return "The e-mail address provided is invalid. Please check the value and try again.";

      case MembershipCreateStatus.InvalidAnswer:
        return "The password retrieval answer provided is invalid. Please check the value and try again.";

      case MembershipCreateStatus.InvalidQuestion:
        return "The password retrieval question provided is invalid. Please check the value and try again.";

      case MembershipCreateStatus.InvalidUserName:
        return "The user name provided is invalid. Please check the value and try again.";

      case MembershipCreateStatus.ProviderError:
        return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

      case MembershipCreateStatus.UserRejected:
        return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

      default:
        return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
   }
}             */

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void RemoveUserFromRole(string userName, string roleName)
        {
            Roles.RemoveUserFromRole(userName, roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string[] GetAllRoles()
        {
            return Roles.GetAllRoles();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void CreateRole(string roleName)
        {
            Roles.CreateRole(roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool DeleteRole(string roleName)
        {
            return Roles.DeleteRole(roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void AddUserToRole(string userName, string roleName)
        {
            Roles.AddUserToRole(userName, roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void AddUserToRoles(string userName, string[] roleNames)
        {
            Roles.AddUserToRoles(userName, roleNames);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string[] UsersInRole(string roleName)
        {
            return Roles.GetUsersInRole(roleName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string[] RolesForUser(string userName)
        {
            return Roles.GetRolesForUser(userName);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static string ExtractFirstNameAndChangeToLowerCase(string name)
        {
            int indexOfFirstSpace;
            string s;

            s = null;

            if (name != null && name != string.Empty)
            {
                name = name.Trim().ToLower();

                indexOfFirstSpace = name.IndexOf(' ');

                if (indexOfFirstSpace < 0) indexOfFirstSpace = name.Length - 1;

                s = name.Substring(0, indexOfFirstSpace + 1);
            }

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
}