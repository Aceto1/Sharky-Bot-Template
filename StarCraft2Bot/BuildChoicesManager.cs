using Sharky.DefaultBot;
using Sharky.MicroControllers;
using Sharky;
using SC2APIProtocol;
using Sharky.Builds;
using StarCraft2Bot.Builds;
using SharkyTerranExampleBot.Builds;

namespace StarCraft2Bot
{
    public class BuildChoicesManager
    {
        private DefaultSharkyBot defaultSharkyBot = null!;
        private IndividualMicroController scvMicroController = null!;

        public BuildChoicesManager(DefaultSharkyBot newDefaultSharkyBot)
        {
            defaultSharkyBot = newDefaultSharkyBot;
            scvMicroController = new IndividualMicroController(newDefaultSharkyBot, newDefaultSharkyBot.SharkyAdvancedPathFinder, MicroPriority.JustLive, false);
        }

        public BuildChoices GetBuildChoices()
        {
            var reaperCheese = new ReaperOpener(defaultSharkyBot, scvMicroController);
            var saltyMarines = new SaltyMarines(defaultSharkyBot, scvMicroController);
            var bansheesAndMarines = new BansheesAndMarines(defaultSharkyBot);

            var builds = new Dictionary<string, ISharkyBuild>
            {
                //[reaperCheese.Name()] = reaperCheese,
                [saltyMarines.Name()] = saltyMarines
            };
            var transitions = new List<List<string>>
            {
                new List<string> { saltyMarines.Name() },
            };

            var defaultSequences = new List<List<string>>
            {
                //new List<string> {
                //    reaperCheese.Name(),
                //    bansheesAndMarines.Name()
                //},
                new List<string> {
                    saltyMarines.Name()
                },
            };

            // INFO: The "Transition" entry should usually contain something other than the same builds over again
            var buildSequences = new Dictionary<string, List<List<string>>>
            {
                [Race.Terran.ToString()] = defaultSequences,
                ["Transition"] = transitions
            };

            return new BuildChoices { Builds = builds, BuildSequences = buildSequences };
        }
    }
}
