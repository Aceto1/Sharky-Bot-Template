using Sharky.Helper;

namespace StarCraft2Bot.Helper
{
    public static class ValueManager
    {
        private static Dictionary<string, int> values = new();
        private static Random rnd = new();

        public static string CurrentBuild { get; set; } = "";

        public static int GetValue(ValueRange range)
        {
            var key = $"{CurrentBuild}-{range.Key}"; 

            if (values.ContainsKey(key))
                return values[key];

            var value = InternalGetValue(range);

            if(!String.IsNullOrEmpty(range.Key))
                values.Add(key, value);

            return value;
        }

        private static int InternalGetValue(ValueRange range)
        {
            // TODO: Implement your real value-finding function here
            return rnd.Next(range.Min, range.Max + 1);
        }
    }
}
