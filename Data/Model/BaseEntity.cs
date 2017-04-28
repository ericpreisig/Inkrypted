/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entity
{
    public class BaseEntity
    {
        #region Public Properties

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        #endregion Public Properties
    }
}