using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarCraft2Bot.Database.Entities;

namespace StarCraft2Bot.Database.Maps
{
    internal class GameMap : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).HasColumnName("Id").IsRequired();
            builder.Property(m => m.Result).HasColumnName("Result").IsRequired();
            builder.Property(m => m.MyRace).HasColumnName("MyRace").IsRequired();
            builder.Property(m => m.EnemyRace).HasColumnName("EnemyRace").IsRequired();
            builder.Property(m => m.GameStart).HasColumnName("GameStart").IsRequired();
            builder.Property(m => m.GameLength).HasColumnName("GameLength").IsRequired();

            builder.HasMany(m => m.DataPoints).WithOne(m => m.Game).HasForeignKey(m => m.GameId);
        }
    }
}
