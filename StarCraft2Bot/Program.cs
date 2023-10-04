using SC2APIProtocol;
using Sharky;
using Sharky.DefaultBot;

namespace StarCraft2Bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting up...");
            Console.WriteLine("Connecting to game...");
            // first we need to create a game connection for the SC2 api. The bot uses this to communicate with the game
            var gameConnection = new GameConnection();
            // We get a default bot that has everything setup.  You can manually create one instead if you want to more heavily customize it.  
            var defaultSharkyBot = new DefaultSharkyBot(gameConnection);

            Console.WriteLine("Loading builds...");
            //var buildMngr = new BuildChoicesManager(defaultSharkyBot);

            // we configure the bot with our own builds
            //defaultSharkyBot.BuildChoices[Race.Terran] = buildMngr.GetBuildChoices();

            // we create a bot with the modified default bot we made
            var sharkyExampleBot = defaultSharkyBot.CreateBot(defaultSharkyBot.Managers, defaultSharkyBot.DebugService);
            
            var myRace = Race.Terran;
            if (args.Length == 0)
            {
                // if there are no arguments passed we play against a comptuer opponent
                gameConnection.RunSinglePlayer(sharkyExampleBot, @"InsideAndOutAIE.SC2Map", myRace, Race.Terran, Difficulty.Easy, AIBuild.RandomBuild).Wait();
            }
            else
            {
                // when a bot runs on the ladder it will pass arguments for a specific map, enemy, etc.
                gameConnection.RunLadder(sharkyExampleBot, myRace, args).Wait();
            }
        }
    }
}