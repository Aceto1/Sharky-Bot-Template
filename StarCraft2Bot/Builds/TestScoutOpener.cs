using SC2APIProtocol;
using Sharky;
using Sharky.Builds;
using Sharky.MicroControllers;
using Sharky.MicroTasks;
using Sharky.MicroTasks.Attack;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base;
using StarCraft2Bot.Builds.Base.Action;
using StarCraft2Bot.Helper;

namespace StarCraft2Bot.Builds
{
    public class TestScoutOpener : Build
    {
        private EnemyInformationsManager EnemyInformationsManager;
        private EnemyUnitMemoryService UnitMemoryService;

        private Queue<BuildAction>? BuildOrder { get; set; }

        public TestScoutOpener(BaseBot defaultSharkyBot)
            : base(defaultSharkyBot)
        {
            defaultSharkyBot.MicroController = new AdvancedMicroController(defaultSharkyBot);
            var advancedAttackTask = new AdvancedAttackTask(
                defaultSharkyBot,
                new EnemyCleanupService(
                    defaultSharkyBot.MicroController,
                    defaultSharkyBot.DamageService
                ),
                new List<UnitTypes> { UnitTypes.TERRAN_MARINE },
                100f,
                true
            );
            defaultSharkyBot.MicroTaskData[typeof(AttackTask).Name] = advancedAttackTask;

            UnitMemoryService = defaultSharkyBot.EnemyUnitMemoryService;

            EnemyInformationsManager = new EnemyInformationsManager(
                UnitCountService,
                MapDataService,
                ActiveUnitData,
                UnitMemoryService,
                defaultSharkyBot.SharkyUnitData,
                FrameToTimeConverter,
                defaultSharkyBot.MapMemoryService,
                defaultSharkyBot.EnemyUnitApproximationService
            );
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictSupplyCount = true;
            BuildOptions.StrictWorkerCount = true;

            BuildOrder = new Queue<BuildAction>();
            MacroData.DesiredUnitCounts[UnitTypes.TERRAN_SCV] = 12;
            //MacroData.DesiredUnitCounts[UnitTypes.TERRAN_REAPER] = 5;

            MicroTaskData[typeof(WorkerScoutTask).Name].Enable();

            //foreach (UnitCommander commander in MicroTaskData[typeof(Micro).Name])
            //{
            //    if (commander.CommanderState == CommanderState.None)
            //    {
            //        commander.Claimed = false;
            //        MicroTaskData[typeof(WorkerScoutTask).Name].StealUnit(commander);
            //    }
            //}
        }

        public override void OnFrame(ResponseObservation observation)
        {
            //base.OnFrame(observation);
            //if (BuildOrder == null)
            //{
            //    throw new InvalidOperationException("BuildOrder has not been initialized.");
            //}

            //if (BuildOrder.Count == 0)
            //{
            //    return;
            //}

            //if (new TimeCondition(25.0).IsFulfilled(observation.Observation))
            //{
            //    proxyTask.DesiredWorkers = 2;
            //}


            Console.WriteLine("Frame: " + observation.Observation.GameLoop + "\n======");
            Console.WriteLine(
                "Mineralapproximation: "
                    + EnemyInformationsManager.GetApproximatedProducedEnemyMinerals(observation)
            );

            Console.WriteLine("Seen:\n=====");
            foreach (var key in UnitMemoryService.CurrentTotalUnits.Keys)
            {
                Console.WriteLine(UnitMemoryService.CurrentTotalUnits[key] + "x " + key.ToString());
            }

            var approx = EnemyInformationsManager.GetApproximatedProducedEnemyUnits(
                EnemyInformationsManager.GetApproximatedProducedEnemyMinerals(observation).Item2
            );

            Console.WriteLine("Approximated:\n=============");

            foreach (var key in approx.Keys)
            {
                Console.WriteLine(approx[key] + "x " + key.ToString());
            }

            // Console.WriteLine(EnemyInformationsManager.GetVisibleAreaPercentage() + "%");


            //var nextAction = BuildOrder.Peek();

            //if (nextAction.AreConditionsFulfilled())
            //{
            //    nextAction.EnforceDesires();
            //    BuildOrder.Dequeue();
            //}
        }

        public override bool Transition(int frame)
        {
            //if (BuildOrder == null)
            //{
            //    throw new InvalidOperationException("BuildOrder has not been initialized.");
            //}

            //if (BuildOrder.Count == 0)
            //{
            //    //MacroData.DesiredUnitCounts[UnitTypes.TERRAN_SCV] = 12;
            //    //MacroData.DesiredUnitCounts[UnitTypes.TERRAN_REAPER] = 0;

            //    //AttackData.UseAttackDataManager = true;
            //    //proxyTask.Disable();
            //    //return true;
            //}

            //return base.Transition(frame);
            return false;
        }
    }
}
