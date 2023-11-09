using SC2APIProtocol;
using Sharky;
using Sharky.Builds.Terran;
using Sharky.Helper;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;
using StarCraft2Bot.Database;
using StarCraft2Bot.Database.Entities;

namespace StarCraft2Bot.Builds.Base
{
    public class Build : TerranSharkyBuild
    {
        private const int framesBetweenMeasurements = 112;

        private int lastMeasurementFrame = 0;

        private readonly BaseBot defaultBot;

        private readonly List<BuildAction> actions = new();

        public bool DoTransition { get; set; }

        public Build(BaseBot defaultSharkyBot) : base(defaultSharkyBot)
        {
            defaultBot = defaultSharkyBot;
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

        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);

            if (lastMeasurementFrame == 0 && defaultBot.Frame > framesBetweenMeasurements * 2)
            {
                lastMeasurementFrame = defaultBot.Frame - defaultBot.Frame % framesBetweenMeasurements;
            }

            if (defaultBot.Frame >= lastMeasurementFrame + framesBetweenMeasurements)
            {
                lastMeasurementFrame = defaultBot.Frame;
                Measure(defaultBot.Frame);
            }

            foreach (var action in actions)
            {
                if (action.AreConditionsFulfilled())
                    action.EnforceDesires();
            }
        }

        public override bool Transition(int frame)
        {
            if (DoTransition)
                return true;

            return base.Transition(frame);
        }

        private void Measure(int frame)
        {
            var dataPoint = new DataPoint()
            {
                GameId = CustomSharkyBot.GameId,
                CurrentBuild = GetType().Name,
                IngameSeconds = (int)(frame / defaultBot.SharkyOptions.FramesPerSecond),

                CurrentMinerals = MacroData.Minerals,
                CurrentVespene = MacroData.VespeneGas,

                TotalMinerals = -1,
                TotalVespene = -1,

                Supply = MacroData.FoodUsed,
                WorkerCount = MacroData.FoodWorkers,

                LostVespene = defaultBot.ActiveUnitData.SelfVespeneLost,
                LostMinerals = defaultBot.ActiveUnitData.SelfMineralsLost,
                LostUnits = defaultBot.ActiveUnitData.SelfDeaths,
                LostBuildings = -1,

                KilledMinerals = defaultBot.ActiveUnitData.EnemyMineralsLost,
                KilledVespene = defaultBot.ActiveUnitData.EnemyVespeneLost,
                KilledUnits = defaultBot.ActiveUnitData.EnemyDeaths
            };

            using var ctx = new DatabaseContext();

            ctx.Datapoints.Add(dataPoint);
            ctx.SaveChanges();
        }
    }
}
   