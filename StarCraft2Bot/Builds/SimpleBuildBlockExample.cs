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

            var expandSupplyBarrackGasBlock = new CustomBuildBlock().WithBuildActions((serial, parallel) =>
            {
                serial.Add(new BuildAction(new SupplyCondition(13, MacroData), 
                    new SupplyDepotDesire(1, MacroData, UnitCountService)));
                var buildBarrackAction = new BuildAction(new SupplyCondition(14, MacroData),
                    new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData, UnitCountService));
                serial.Add(buildBarrackAction);
                parallel.Add(new BuildAction(new ActionCompletedCondition(buildBarrackAction), new GasBuildingCountDesire(1, MacroData, UnitCountService)));
            });
            var scoutWithTrainedReaper = new ScoutWithTrainedReaper(MicroTaskData, MacroData, ActiveUnitData, UnitCountService, BaseData);

            ActionQueue.Enqueue(expandSupplyBarrackGasBlock);
            ActionQueue.Enqueue(scoutWithTrainedReaper);
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
