/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Data
{
    /// <summary>
    /// Class that containes every thing about the users, sigin and login
    /// </summary>
    public static class Login
    {
        #region Private Properties

        private static Repository<User> Users => new Repository<User>();

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Check the password and username
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool CheckUserLogin(string userName, string password) => Users.GetList().Any(a => a.Username.ToLower() == userName.ToLower() && a.Password == HashPasword(password));

        /// <summary>
        /// Check if a login is already used
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool IsExistingLogin(string userName) => Users.GetList().Any(a => a.Username.ToLower() == userName.ToLower());

        /// <summary>
        /// Add an user to the database
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public static void SignIn(string userName, string password)
        {
            Users.Insert(new User
            {
                Username = userName,
                Password = HashPasword(password)
            });
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Hash a string
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static string HashPasword(string password)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(password));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }

        #endregion Private Methods
    }
}