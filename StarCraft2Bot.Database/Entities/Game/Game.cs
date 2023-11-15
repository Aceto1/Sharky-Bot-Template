using SC2APIProtocol;
using StarCraft2Bot.Database.Enum;

namespace StarCraft2Bot.Database.Entities
{
    public class Game
    {
        public int Id { get; set; }

        public Race MyRace { get; set; }

        public Race EnemyRace { get; set; }

        public Result Result { get; set; }

        public DateTime GameStart { get; set; }

        /// <summary>
        /// GameLength in Seconds
        /// </summary>
        public int GameLength { get; set; }

        public virtual ICollection<DataPoint>? DataPoints { get; set; }

        public virtual ICollection<GameValue>? Values { get; set; }

        public string MapName { get; set; }
    }
}
