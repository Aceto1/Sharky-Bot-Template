using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StarCraft2Bot.Database.Entities;

namespace StarCraft2Bot.Database.Maps
{
    internal class DataPointMap : IEntityTypeConfiguration<DataPoint>
    {
        public void Configure(EntityTypeBuilder<DataPoint> builder)
        {
            builder.ToTable("Datapoints");

            builder.HasKey(x => x.Id);

            builder.Property(m => m.Id).HasColumnName("Id").IsRequired();
            builder.Property(m => m.GameId).HasColumnName("GameId").IsRequired();
            builder.Property(m => m.CurrentBuild).HasColumnName("CurrentBuild").IsRequired();
            builder.Property(m => m.CurrentMinerals).HasColumnName("CurrentMinerals").IsRequired();
            builder.Property(m => m.CurrentVespene).HasColumnName("CurrentVespene").IsRequired();
            builder.Property(m => m.TotalMinerals).HasColumnName("TotalMinerals").IsRequired();
            builder.Property(m => m.TotalVespene).HasColumnName("TotalVespene").IsRequired();
            builder.Property(m => m.LostMinerals).HasColumnName("LostMinerals").IsRequired();
            builder.Property(m => m.LostVespene).HasColumnName("LostVespene").IsRequired();
            builder.Property(m => m.LostUnits).HasColumnName("LostUnits").IsRequired();
            builder.Property(m => m.LostBuildings).HasColumnName("LostBuildings").IsRequired();
            builder.Property(m => m.Supply).HasColumnName("Supply").IsRequired();
            builder.Property(m => m.WorkerCount).HasColumnName("WorkerCount").IsRequired();
            builder.Property(m => m.IngameSeconds).HasColumnName("IngameSeconds").IsRequired();

            builder.HasOne(m => m.Game).WithMany(m => m.DataPoints).HasForeignKey(m => m.GameId);
        }
    }
}
