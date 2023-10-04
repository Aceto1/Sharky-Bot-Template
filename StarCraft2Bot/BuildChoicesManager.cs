using Sharky.DefaultBot;
using Sharky.MicroControllers;
using Sharky;
using SC2APIProtocol;
using Sharky.Builds;
using StarCraft2Bot.Builds;

namespace StarCraft2Bot
{
    public static class BuildChoicesManager
    {
        private static DefaultSharkyBot defaultSharkyBot = null!;
        private static IndividualMicroController scvMicroController = null!;

        public static void Init(DefaultSharkyBot newDefaultSharkyBot)
        {
            defaultSharkyBot = newDefaultSharkyBot;
            scvMicroController = new IndividualMicroController(newDefaultSharkyBot, newDefaultSharkyBot.SharkyAdvancedPathFinder, MicroPriority.JustLive, false);
        }

        public static BuildChoices GetBuildChoices()
        {
            var reaperCheese = new ReaperOpener(defaultSharkyBot, scvMicroController);

            var builds = new Dictionary<string, ISharkyBuild>
            {
                [reaperCheese.Name()] = reaperCheese,
            };

            var defaultSequences = new List<List<string>>
            {
                new List<string> { reaperCheese.Name() },
            };

            // INFO: The "Transition" entry should usually contain something other than the same builds over again
            var buildSequences = new Dictionary<string, List<List<string>>>
            {
                [Race.Terran.ToString()] = defaultSequences,
                ["Transition"] = defaultSequences
            };

            return new BuildChoices { Builds = builds, BuildSequences = buildSequences };
        }
    }
}
