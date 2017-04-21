using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Data.Entity;
using SQLite.CodeFirst;
using System.Data.Entity.ModelConfiguration;

namespace MatchMaster
{
    class MatchMasterContext : DbContext
    {
        public MatchMasterContext() : base("MatchMaster.Properties.Settings.SQLEXPRESS")
        {
            this.Configuration.LazyLoadingEnabled = false;
            Database.SetInitializer<MatchMasterContext>(new DropCreateDatabaseIfModelChanges<MatchMasterContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //var sqliteConnectionInitializer = new SqliteDropCreateDatabaseWhenModelChanges<MatchMasterContext>(modelBuilder);
            //Database.SetInitializer(sqliteConnectionInitializer);

            // === Match ===

            EntityTypeConfiguration<Match> MatchEntityConfig = modelBuilder.Entity<Match>();

            MatchEntityConfig.Property(p => p.EndDate).IsOptional();
            MatchEntityConfig.Property(p => p.StartDate).IsOptional();
            MatchEntityConfig.Property(p => p.Title).IsRequired();

            MatchEntityConfig
                .HasMany(s => s.MatchParticipations)
                .WithRequired(x => x.Match)
                .HasForeignKey(f => f.MatchID);

            // == MatchParticipation ===

            EntityTypeConfiguration<MatchParticipation> MatchParticipationsEntityConfig = modelBuilder.Entity<MatchParticipation>();

            MatchParticipationsEntityConfig
                .HasRequired<Shooter>(m => m.Shooter)
                .WithMany(s => s.MatchParticipations)
                .HasForeignKey(f => f.ShooterID);

            MatchParticipationsEntityConfig
                .HasRequired<Match>(s => s.Match)
                .WithMany(m => m.MatchParticipations)
                .HasForeignKey(f => f.MatchID);

            // === Shooter ===

            EntityTypeConfiguration<Shooter> ShooterEntityConfig = modelBuilder.Entity<Shooter>();

            ShooterEntityConfig.Property(p => p.Birthday).IsOptional();

            ShooterEntityConfig
                .HasMany(m => m.MatchParticipations)
                .WithRequired(x => x.Shooter)
                .HasForeignKey(f => f.ShooterID);

            // === *** ===

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Match> Matches {get;set;}
        public DbSet<MatchParticipation> MatchParticipations { get; set; }
        public DbSet<Shooter> Shooters {get;set;}
        public DbSet<Category> Categories { get; set; }
    }
}
