using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using SamuraiApp.Domain;
using System;
using System.Linq;

namespace SamuraiApp.Data
{
    public class SamuraiContext:DbContext
    {
        public SamuraiContext()
        {
            
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            
        }
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<SamuraiStat> SamuraiBattleStats { get; set; }
       
        public static readonly LoggerFactory MyConsoleLoggerFactory
         = new LoggerFactory(new[] {
              new ConsoleLoggerProvider((category, level)
                => category == DbLoggerCategory.Database.Command.Name
               && level == LogLevel.Information, true) });
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(MyConsoleLoggerFactory)
                .UseSqlServer(
                 "Server = .\\MSSQLSERVER2017; Database = SamuraiAppData; Trusted_Connection = True; ")
                .EnableSensitiveDataLogging(true); 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamuraiBattle>()
              .HasKey(s => new { s.SamuraiId, s.BattleId });
            modelBuilder.Entity<Battle>().Property(b => b.StartDate).HasColumnType("Date");
            modelBuilder.Entity<Battle>().Property(b => b.EndDate).HasColumnType("Date");

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.BaseType != typeof(DbView)
                    || entityType.ClrType.BaseType != typeof(JoinTable))
                {
                    modelBuilder.Entity(entityType.Name).Property<DateTime>("CreatedAt");
                    modelBuilder.Entity(entityType.Name).Property<DateTime>("LastModified");
                }
            }
            modelBuilder.Entity<Samurai>().OwnsOne(s => s.BetterName).Property(b => b.GivenName).HasColumnName("GivenName");
            modelBuilder.Entity<Samurai>().OwnsOne(s => s.BetterName).Property(b => b.SurName).HasColumnName("SurName");
            modelBuilder.Entity<SamuraiStat>().HasKey(s => s.SamuraiId);

            // base.OnModelCreating(modelBuilder);
        }
        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            var timestamp = DateTime.Now;
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => (e.State == EntityState.Added || e.State == EntityState.Modified)
                             && !e.Metadata.IsOwned()))
            {
                entry.Property("LastModified").CurrentValue = timestamp;

                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedAt").CurrentValue = timestamp;
                }
                if (entry.Entity is Samurai)
                {
                    if (entry.Reference("BetterName").CurrentValue == null)
                    {
                        entry.Reference("BetterName").CurrentValue = PersonFullName.Empty();
                    }
                    entry.Reference("BetterName").TargetEntry.State = entry.State;
                }
            }
            return base.SaveChanges();
        }
    }
}
