using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarCraft2Bot.Database.Entities;

namespace StarCraft2Bot.Database.Maps
{
    internal class GameValueMap : IEntityTypeConfiguration<GameValue>
    {
        public void Configure(EntityTypeBuilder<GameValue> builder)
        {
            builder.ToTable("GameValues");

            builder.HasKey(x => x.Id);

            builder.Property(m => m.Id).HasColumnName("Id").IsRequired();
            builder.Property(m => m.GameId).HasColumnName("GameId").IsRequired();
            builder.Property(m => m.Key).HasColumnName("Key").IsRequired();
            builder.Property(m => m.Value).HasColumnName("Value").IsRequired();
            
            builder.HasOne(m => m.Game).WithMany(m => m.Values).HasForeignKey(m => m.GameId);
        }
    }
}
