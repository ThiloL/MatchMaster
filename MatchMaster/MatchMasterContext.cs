using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Data.Entity;
using SQLite.CodeFirst;

namespace MatchMaster
{
    class MatchMasterContext : DbContext
    {
        public MatchMasterContext() : base("MatchMaster.Properties.Settings.MatchMasterCon") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteDropCreateDatabaseWhenModelChanges<MatchMasterContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }

        public DbSet<Match> Matches {get;set;}
        public DbSet<Shooter> Shooters { get; set; }
    }
}
