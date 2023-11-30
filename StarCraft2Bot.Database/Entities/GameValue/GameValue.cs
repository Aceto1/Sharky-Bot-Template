namespace StarCraft2Bot.Database.Entities
{
    public class GameValue
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public required string Key { get; set; }

        public int Value { get; set; }

        public virtual Game? Game { get; set; }
    }
}
