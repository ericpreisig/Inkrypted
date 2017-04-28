/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using SQLite.CodeFirst;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Windows;

namespace Data.Entity
{
    [DbConfigurationType(typeof(SqLiteConfiguration))]
    public class DbContextInkrypted : DbContext
    {
        #region Public Constructors

        /// <summary>
        /// Init the database : do not work with SqLite
        /// </summary>
        public DbContextInkrypted() : base(new SQLiteConnection(@"Data Source = " + Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\InkryptedDb\InkryptedDb.db"), true)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<DbContextInkrypted>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DbContextInkrypted, Configuration>());
        }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<FileTrack> FileTrack { get; set; }
        public DbSet<User> Users { get; set; }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Model the database
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<DbContextInkrypted>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);

            modelBuilder.Entity<User>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("Users");
            });
            modelBuilder.Entity<AppSettings>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("AppSettings");
            });
            modelBuilder.Entity<FileTrack>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("FileTrack");
            });

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }

        #endregion Protected Methods
    }
}