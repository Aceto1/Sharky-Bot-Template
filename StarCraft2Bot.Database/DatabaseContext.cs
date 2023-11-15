using Microsoft.EntityFrameworkCore;
using StarCraft2Bot.Database.Entities;
using StarCraft2Bot.Database.Maps;

namespace StarCraft2Bot.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Game> Games { get; set; }

        public DbSet<DataPoint> Datapoints { get; set; }

        public DbSet<GameValue> GameValues { get; set; }

        public static string DbPath { get; }

        static DatabaseContext()
        {
            var appdata = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(appdata);
            var folder = Path.Join(path, "StarCraft 2 Bot");

            Directory.CreateDirectory(folder);
            DbPath = Path.Join(folder, "sc2bot.db");
        }

        public DatabaseContext()
        {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new GameMap());
            modelBuilder.ApplyConfiguration(new GameValueMap());
            modelBuilder.ApplyConfiguration(new DataPointMap());
        }
    }
}