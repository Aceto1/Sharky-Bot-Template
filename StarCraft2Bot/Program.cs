using SC2APIProtocol;
using Sharky;
using Sharky.DefaultBot;
using StarCraft2Bot.Helper;

namespace StarCraft2Bot
{
    internal class Program
    {
        private static int startupPort = 5000;

        public static Task StartSinglePlayerGame(List<Map> maps)
        {
            var gameConnection = new GameConnection();
            var defaultSharkyBot = new DefaultSharkyBot(gameConnection);
            defaultSharkyBot.BuildChoices[Race.Terran] = new BuildChoicesManager(defaultSharkyBot).GetBuildChoices();

            var exampleBot = defaultSharkyBot.CreateBot(defaultSharkyBot.Managers, defaultSharkyBot.DebugService);
            
            startupPort += 5;

            var map = maps.GetRandomEntry();

            return gameConnection.RunSinglePlayer(exampleBot, $"{Enum.GetName(map)}AIE.SC2Map", Race.Terran, Race.Terran, Difficulty.Hard, AIBuild.RandomBuild, startupPort, realTime: true);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting up...");

            Console.WriteLine("Setting up AI...");
            ValueCallbackService.Init(ValueManager.GetValue);

            Console.WriteLine("Starting bot(s)...");

            var games = new List<Task>();
            var instanceCount = 1;

            if (args.Length > 0 && args.FirstOrDefault() == "-instances")
            {
                instanceCount = Int32.Parse(args[1]);
            }

            var maps = new List<Map>()
            {
                Map.InsideAndOut,
                //Map.Stargazers,
                //Map.Hardwire,
                //Map.Waterfall,
                //Map.Berlingrad,
                //Map.Moondance
            };

            for (int i = 0; i < instanceCount; i++)
            {
                games.Add(StartSinglePlayerGame(maps));
            }

             Task.WaitAll(games.ToArray());
        }
    }
}