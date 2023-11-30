namespace StarCraft2Bot.Database.Entities
{
    public class DataPoint
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public required string CurrentBuild { get; set; }

        public int IngameSeconds { get; set; }

        public int TotalMinerals { get; set; }

        public int CurrentMinerals { get; set; }

        public int LostMinerals { get; set; }

        public int KilledMinerals { get; set; }

        public int TotalVespene { get; set; }

        public int CurrentVespene { get; set; }

        public int LostVespene { get; set; }

        public int KilledVespene { get; set; }

        public int WorkerCount { get; set; }

        public int Supply { get; set; }

        public int LostUnits { get; set; }

        public int KilledUnits { get; set; }

        public int LostBuildings { get; set; }

        public virtual Game? Game { get; internal set; }
    }
}
