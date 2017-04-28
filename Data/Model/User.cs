/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

namespace Data.Entity
{
    public class User : BaseEntity
    {
        #region Public Properties

        public string Password { get; set; }
        public string Username { get; set; }

        #endregion Public Properties
    }
}