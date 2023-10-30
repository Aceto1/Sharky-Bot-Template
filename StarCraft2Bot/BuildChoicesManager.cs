﻿using Sharky.DefaultBot;
using Sharky.MicroControllers;
using Sharky;
using SC2APIProtocol;
using Sharky.Builds;
using StarCraft2Bot.Builds;

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
            var currentOpener = new DefensiveOpener(defaultSharkyBot, scvMicroController);

            var builds = new Dictionary<string, ISharkyBuild>
            {
                [currentOpener.Name()] = currentOpener,
            };

            var defaultSequences = new List<List<string>>
            {
                new List<string> { currentOpener.Name() },
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
