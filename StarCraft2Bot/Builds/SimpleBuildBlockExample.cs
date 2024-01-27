using SC2APIProtocol;
using Sharky;
using Sharky.Builds;
using Sharky.MicroControllers;
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
        Queue<AutoTechBuildBlock> ActionQueue = [];

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

            var expandSupplyBarrackGasBlock = new AutoTechBuildBlock("ExpandSupplyBarrackGas",DefaultBot).WithConditions([]).WithActionNodes(root =>
            {
                root.AddActionOnStart("BuildSupply", new BuildAction(new SupplyCondition(13, MacroData), new SupplyDepotDesire(1, MacroData, UnitCountService)), n =>
                {
                    n.AddActionOnStart("BuildBarrack", new BuildAction(new SupplyCondition(14, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData, UnitCountService)), n =>
                    {
                        n.AddActionOnStart("BuildGas",new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_BARRACKS, 1, UnitCountService), new GasBuildingCountDesire(1, MacroData, UnitCountService)), n =>
                        {
                        });
                    });
                });
            });
            var scoutWithTrainedReaper = new ScoutWithTrainedReaper(DefaultBot);

            var testBlock = new AutoTechBuildBlock("TestBlock",DefaultBot).WithActionNodes(root =>
            {
                root.AddActionOnStart("BuildSupply",new BuildAction(new SupplyCondition(13, MacroData), new SupplyDepotDesire(1, MacroData, UnitCountService)), node =>
                {
                    node.AddActionOnStart("BuildBattleCruiser", new BuildAction(new SupplyCondition(13, MacroData), new UnitDesire(UnitTypes.TERRAN_BATTLECRUISER, 1, DefaultBot.MacroData.DesiredUnitCounts, DefaultBot.UnitCountService)));
                });
                root.AddActionOnStart("BuildBarrack", new BuildAction(new NoneCondition(), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, DefaultBot.MacroData, DefaultBot.UnitCountService)));
            });
            ActionQueue.Enqueue(expandSupplyBarrackGasBlock);
            ActionQueue.Enqueue(scoutWithTrainedReaper);
            ActionQueue.Enqueue(testBlock);
        }



        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);
            if (ActionQueue.Count == 0)
            {
                return;
            }

            var currentBlock = ActionQueue.Peek();
            if (!currentBlock.AreConditionsFulfilled()) return;

            if (!currentBlock.HasStarted()) currentBlock.PrintBuildBlock();
            if (currentBlock.HasCompleted())
            {
                ActionQueue.Dequeue();
            }
            else
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
