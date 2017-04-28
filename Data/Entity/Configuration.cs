/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Migrations;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace Data.Entity
{
    /// <summary>
    /// Set the configuration of the database (not for sqlite)
    /// </summary>
    public sealed class Configuration : DbMigrationsConfiguration<DbContextInkrypted>
    {
        #region Public Constructors

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <summary>
        /// Populate the database
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(DbContextInkrypted context)
        {
        }

        #endregion Protected Methods
    }

    /// <summary>
    /// Settings for sqlite
    /// </summary>
    public class SqLiteConfiguration : DbConfiguration
    {
        #region Public Constructors

        /// <summary>
        /// settings of the provider for SQLite
        /// </summary>
        public SqLiteConfiguration()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }

        #endregion Public Constructors
    }
}