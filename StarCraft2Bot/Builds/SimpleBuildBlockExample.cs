using System.Numerics;
using SC2APIProtocol;
using Sharky;
using Sharky.Builds;
using Sharky.Extensions;
using Sharky.MicroControllers;
using Sharky.MicroTasks;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base;
using StarCraft2Bot.Builds.Base.Action;
using StarCraft2Bot.Builds.Base.Action.BuildBlocks;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds
{
    public class SimpleBuildBlockExample : Build
    {
        Queue<IAction>? ActionQueue;

        public SimpleBuildBlockExample(BaseBot defaultSharkyBot) : base(defaultSharkyBot)
        {
            defaultSharkyBot.MicroController = new AdvancedMicroController(defaultSharkyBot);
            defaultSharkyBot.SharkyOptions.GameStatusReportingEnabled = false;
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);
            BuildOptions.StrictSupplyCount = true;
            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictWorkerCount = false;

            ActionQueue = new Queue<IAction> {};

            //TerranTechTree.GetRequiredTechUnits(UnitTypes.TERRAN_REAPER).ToList().ForEach(unit => Console.WriteLine(unit));

            var expandSupplyBarrackGasBlock = new AutoTechBuildBlock(DefaultBot)
                .WithSerialActions([
                    new BuildAction(new SupplyCondition(13, MacroData), new SupplyDepotDesire(1, MacroData, UnitCountService)),
                    new BuildAction(new SupplyCondition(14, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData, UnitCountService)),
                    new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_BARRACKS, 1, UnitCountService), new GasBuildingCountDesire(1, MacroData, UnitCountService))
                ]);
            var scoutWithTrainedReaper = new ScoutWithTrainedReaper(DefaultBot);

            var testBlock = new AutoTechBuildBlock(DefaultBot)
               .WithSerialActions([
                   new BuildAction(new SupplyCondition(13, MacroData), new SupplyDepotDesire(1, MacroData, UnitCountService)),
                   new BuildAction(new NoneCondition(), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, DefaultBot.MacroData, DefaultBot.UnitCountService)),
                   new BuildAction(new SupplyCondition(13, MacroData), new UnitDesire(UnitTypes.TERRAN_BATTLECRUISER, 1, DefaultBot.MacroData.DesiredUnitCounts, DefaultBot.UnitCountService)),
               ]);

            //ActionQueue.Enqueue(expandSupplyBarrackGasBlock);
            //ActionQueue.Enqueue(scoutWithTrainedReaper);
            ActionQueue.Enqueue(testBlock);
        }

        

        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);
            if (ActionQueue == null)
            {
                throw new InvalidOperationException("BuildOrder has not been initialized.");
            }

            if (ActionQueue.Count == 0)
            {
                return;
            }

            var currentBlock = ActionQueue.Peek();
            if (currentBlock.HasCompleted())
            {
                ActionQueue.Dequeue();
            } else
            {
                currentBlock.Enforce();
            }
        }

        public override bool Transition(int frame)
        {
            if (ActionQueue?.Count == 0)
            {
                return true;
            }
            return false;
        }
    }
}
