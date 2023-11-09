using Microsoft.EntityFrameworkCore;
using SC2APIProtocol;
using Sharky.Helper;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Database;
using StarCraft2Bot.Database.Entities;

namespace StarCraft2Bot.Helper
{
    public static class ValueManager
    {
        private static readonly Dictionary<string, int> values = new();
        private static readonly Random rnd = new();

        public static int GetValue(ValueRange range)
        {
            if (values.TryGetValue(range.Key, out var value))
                return value;

            var newValue = InternalGetValue(range);

            if(!String.IsNullOrEmpty(range.Key))
                values.Add(range.Key, newValue);

            return newValue;
        }

        private static int InternalGetValue(ValueRange range)
        {
            using var ctx = new DatabaseContext();

            var existingValues = ctx.GameValues.Where(m => m.Key == range.Key).Include(m => m.Game);

            int value;

            if(existingValues.Count() < 100)
                value = rnd.Next(range.Min, range.Max + 1);
            else
            {
                // Weigh every option by the number of wins
                var distinctValues = existingValues.DistinctBy(m => m.Value);
                var list = new WeightedList<int>(distinctValues.Select(m => new WeightedListItem<int>(m.Value, existingValues.Count(o => o.Value == m.Value && m.Game.Result == Result.Victory))).ToList(), rnd);

                value = list.Next();
            }

            ctx.GameValues.Add(new GameValue
            {
                Key = range.Key,
                Value = value,
                GameId = CustomSharkyBot.GameId,
            });
            ctx.SaveChanges();

            return value;
        }
    }
}
