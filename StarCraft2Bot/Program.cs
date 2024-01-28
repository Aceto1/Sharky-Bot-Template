using System.Diagnostics;
using SC2APIProtocol;
using Sharky;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Helper;
using System.Net.NetworkInformation;

namespace StarCraft2Bot
{
    internal class Program
    {
        private static int startupPort = 5000;

        private static bool CheckPort(int port)
        {
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port >= port && tcpi.LocalEndPoint.Port <= port + 5)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }

        private static SharkyBot GetBot(GameConnection gameConnection)
        {
            var defaultSharkyBot = new BaseBot(gameConnection);
            defaultSharkyBot.BuildChoices[Race.Terran] = new BuildChoicesManager(defaultSharkyBot).GetBuildChoices();
            return defaultSharkyBot.CreateBot(defaultSharkyBot.Managers, defaultSharkyBot.DebugService);
        }

        private static Task<Process> StartSinglePlayerGame(List<Map> maps)
        {
            var gameConnection = new GameConnection();
            var exampleBot = GetBot(gameConnection);

            while (!CheckPort(startupPort))
                startupPort += 5;

            var map = maps.GetRandomEntry();

            return gameConnection.RunSinglePlayer(exampleBot, $"{Enum.GetName(map)}AIE.SC2Map", Race.Terran, Race.Terran, Difficulty.CheatInsane, AIBuild.RandomBuild, startupPort, realTime: false);
        }

        private static void RunLadderGame(string[] args)
        {
            var gameConnection = new GameConnection();
            var exampleBot = GetBot(gameConnection);

            gameConnection.RunLadder(exampleBot, Race.Terran, args).Wait();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting up...");

            Console.WriteLine("Setting up AI...");
            ValueCallbackService.Init(ValueManager.GetValue);

            Console.WriteLine("Starting bot...");

            var endless = args.Contains("-endless");

            var maps = new List<Map>()
            {
                Map.InsideAndOut,
                Map.Stargazers,
                // Map.Hardwire,
                Map.Waterfall,
                Map.Berlingrad,
                Map.Moondance
            };

            if (endless)
                Console.WriteLine("Running in endless mode...");

            if (endless || args.Length == 0)
            {
                do
                {
                    var game = StartSinglePlayerGame(maps);

                    while (!game.IsCompleted)
                    {
                        Thread.Sleep(50);
                    }

                    Thread.Sleep(500);
                    game.Result.Kill();
                } while (true);
            }
            else 
                RunLadderGame(args);
        }
    }
}