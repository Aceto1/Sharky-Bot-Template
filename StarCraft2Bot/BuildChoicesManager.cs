﻿using Sharky.MicroControllers;
using Sharky;
using SC2APIProtocol;
using Sharky.Builds;
using StarCraft2Bot.Builds;
using StarCraft2Bot.Bot;

namespace StarCraft2Bot
{
    public class BuildChoicesManager
    {
        private readonly BaseBot defaultSharkyBot;
        private readonly IndividualMicroController scvMicroController;

        public BuildChoicesManager(BaseBot newDefaultSharkyBot)
        {
            defaultSharkyBot = newDefaultSharkyBot;
            scvMicroController = new IndividualMicroController(newDefaultSharkyBot, newDefaultSharkyBot.SharkyAdvancedPathFinder, MicroPriority.JustLive, false);
        }

        public BuildChoices GetBuildChoices()
        {

            var reaperCheese = new ReaperOpener(defaultSharkyBot, scvMicroController);
            var saltyMarines = new SaltyMarines(defaultSharkyBot);
            var threeCC = new ThreeCC(defaultSharkyBot);
            var tvtOpener = new TvTOpener(defaultSharkyBot);
            var jsonStandartTvT = new JsonBuild(defaultSharkyBot, "Standard TvT");
            var json3CCExample = new JsonBuild(defaultSharkyBot, "3CC-Example");
            var buildBlockExample = new SimpleBuildBlockExample(defaultSharkyBot);
            var buildBlockThreeCC = new ThreeCCBlocked(defaultSharkyBot);

            var builds = new Dictionary<string, ISharkyBuild>
            {
                [reaperCheese.Name()] = reaperCheese,
                [threeCC.Name()] = threeCC,
                [tvtOpener.Name()] = tvtOpener,
                [saltyMarines.Name()] = saltyMarines,
                [jsonStandartTvT.Name()] = jsonStandartTvT,
                [json3CCExample.Name()] = json3CCExample,
                [buildBlockExample.Name()] = buildBlockExample,
                [buildBlockThreeCC.Name()] = buildBlockThreeCC,
            };

            var transitions = new List<List<string>>
            {
                new() { saltyMarines.Name() }
            };

            var openers = new List<List<string>>
            {
                //new() { tvtOpener.Name() },
                //new() { reaperCheese.Name() },
                //new() { threeCC.Name() },
                //new() { jsonStandartTvT.Name() }
                //new() {buildBlockExample.Name()}
                new() {buildBlockThreeCC.Name() }
            };

            // INFO: The "Transition" entry should usually contain something other than the same builds over again
            var buildSequences = new Dictionary<string, List<List<string>>>
            {
                [Race.Terran.ToString()] = openers,
                ["Transition"] = transitions
            };

            return new BuildChoices { Builds = builds, BuildSequences = buildSequences };
        }
    }
}
