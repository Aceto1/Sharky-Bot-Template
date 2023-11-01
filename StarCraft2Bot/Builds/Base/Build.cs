using SC2APIProtocol;
using Sharky;
using Sharky.Builds.Terran;
using Sharky.DefaultBot;
using Sharky.Helper;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;
using StarCraft2Bot.Helper;

namespace StarCraft2Bot.Builds.Base
{
    public class Build : TerranSharkyBuild
    {
        private const int secondsPerMeasurement = 1;

        private readonly DefaultSharkyBot defaultBot;

        private int frame = 0;

        private readonly List<BuildAction> actions = new();

        public Build(DefaultSharkyBot defaultSharkyBot) : base(defaultSharkyBot)
        {
            this.defaultBot = defaultSharkyBot;
        }

        public void AddActionOnWorkerCount(ValueRange workerCount, UnitTypes desireUnitType, ValueRange desireCount, Dictionary<UnitTypes, ValueRange>? dataDict = null)
        {
            dataDict ??= MacroData.DesiredProductionCounts;

            actions.Add(new BuildAction(new WorkerCountCondition(workerCount, UnitCountService), new UnitDesire(desireUnitType, desireCount, dataDict)));
        }

        public void AddActionOnCompletedStructure(UnitTypes structureType, ValueRange structureCount, UnitTypes desireUnitType, ValueRange desireCount, Dictionary<UnitTypes, ValueRange>? dataDict = null)
        {
            dataDict ??= MacroData.DesiredProductionCounts;

            actions.Add(new BuildAction(new UnitCountCondition(structureType, structureCount, UnitCountService), new UnitDesire(desireUnitType, desireCount, dataDict)));
        }

        public void AddAction(BuildAction action)
        {
            actions.Add(action);
        }

        public void AddActions(params BuildAction[] actionArr)
        {
            actions.AddRange(actionArr);
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            ValueManager.CurrentBuild = this.GetType().Name;
        }

        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);

            frame++;

            if (frame % (defaultBot.SharkyOptions.FramesPerSecond * secondsPerMeasurement) == 0)
            {
                Measure();
            }

            foreach (var action in actions)
            {
                if (action.AreConditionsFulfilled())
                    action.EnforceDesires();
            }
        }

        private void Measure()
        {
            //TODO: Measure current state
        }
    }
}
   