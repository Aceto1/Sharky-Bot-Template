namespace StarCraft2Bot.Helper
{
    public static class ListExtensions
    {
        public static T GetRandomEntry<T>(this List<T> list)
        {
            var rnd = new Random();

            var idx = rnd.Next(0, list.Count);

            return list[idx];
        }
    }
}
