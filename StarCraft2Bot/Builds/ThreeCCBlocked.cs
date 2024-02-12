using Roy_T.AStar.Graphs;
using SC2APIProtocol;
using Sharky;
using Sharky.Builds;
using Sharky.Managers;
using Sharky.MicroControllers;
using Sharky.MicroTasks;
using Sharky.MicroTasks.Attack;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base;
using StarCraft2Bot.Builds.Base.Action;
using StarCraft2Bot.Builds.Base.Action.BuildBlocks;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds
{
    public class ThreeCCBlocked : Build
    {
        List<BuildBlock> BuildBlocks = [];
        List<BuildBlock> ActiveBuildBlocks = [];
        readonly bool SendDebugMessages = true;

        public ThreeCCBlocked(BaseBot defaultSharkyBot) : base(defaultSharkyBot)
        {
            defaultSharkyBot.MicroController = new AdvancedMicroController(defaultSharkyBot);
            defaultSharkyBot.SharkyOptions.GameStatusReportingEnabled = false;

            InitAttackManager(defaultSharkyBot);
        }

        public void InitAttackManager(BaseBot defaultSharkyBot)
        {
            var advancedAttackTask = new AdvancedAttackTask(defaultSharkyBot, new EnemyCleanupService(defaultSharkyBot.MicroController,
                defaultSharkyBot.DamageService), new List<UnitTypes> { UnitTypes.TERRAN_MARINE }, 1f, true);
            defaultSharkyBot.MicroTaskData[typeof(AttackTask).Name] = advancedAttackTask;
            var advancedAttackService = new AdvancedAttackService(defaultSharkyBot, advancedAttackTask);
            var advancedAttackDataManager = new AdvancedAttackDataManager(defaultSharkyBot, advancedAttackService, advancedAttackTask);
            defaultSharkyBot.AttackDataManager = advancedAttackDataManager;
            defaultSharkyBot.Managers.RemoveAll(m => m.GetType() == typeof(AttackDataManager));
            defaultSharkyBot.Managers.Add(advancedAttackDataManager);
        }

        private void SetInitialBuildSettings()
        {
            MicroTaskData[typeof(WorkerScoutTask).Name].Enable();
            MicroTaskData[typeof(AttackTask).Name].Enable();

            BuildOptions.StrictSupplyCount = true;
            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictWorkerCount = false;
            BuildOptions.StrictWorkersPerGas = true;
            BuildOptions.StrictWorkersPerGasCount = 3;
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);
            SetInitialBuildSettings();
            MacroData md = DefaultBot.MacroData;
            UnitCountService ucs = DefaultBot.UnitCountService;

            BuildBlocks = [
                new AutoTechBuildBlock("ExtendBasic", DefaultBot).WithActionNodes(root =>
                {
                    root.AddActionOnStart("BuildSupply", new SupplyCondition(13, md), new SupplyDepotDesire(1, md, ucs), node =>
                    {
                        node.AddActionOnResourcesSpend("BuildBarrack", new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, md, ucs), node =>
                        {
                            node.AddActionOnResourcesSpend("BuildGas", new GasBuildingCountDesire(1, md, ucs));
                        });
                    });
                }),
                new ScoutWithTrainedReaper(DefaultBot).WithActionNodes(root =>
                {
                    root.AddActionOnStart("Reduce Gas Worker", new UnitCompletedCountCondition(UnitTypes.TERRAN_REAPER, 1, DefaultBot.UnitCountService), new CustomDesire(() => BuildOptions.StrictWorkersPerGasCount = 1));
                }),
                new AutoTechBuildBlock("FirstOrbitalCommand", DefaultBot).WithActionNodes(root =>
                {
                    root.AddActionOnStart("BuildOrbitalCommand", new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 1, md, ucs));
                }),
                new AutoTechBuildBlock("OptionalBuildEnemyProxyDefense", DefaultBot).WithConditions(new CustomCondition(IsEnemyProxing), new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BARRACKS, 1, ucs)).WithActionNodes(root =>
                {
                    root.AddActionOnStart("Train4Marines", new UnitDesire(UnitTypes.TERRAN_MARINE, 4, md.DesiredUnitCounts, ucs));
                }),
                new AutoTechBuildBlock("OptionalBuildEnemyRushDefense", DefaultBot).WithConditions(new CustomCondition(IsEnemyRushing), new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BARRACKS, 1, ucs)).WithActionNodes(root =>
                {
                    root.AddActionOnStart("Train8Marines", new UnitDesire(UnitTypes.TERRAN_MARINE, 4, md.DesiredUnitCounts, ucs));
                    root.AddActionOnStart("SecondBarrack", new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 2, md, ucs), node =>
                    {
                        node.AddActionOnResourcesSpend("FirstBunker", new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BARRACKS, 2, ucs), new DefenseStructureDesire(UnitTypes.TERRAN_BUNKER, 1, md, ucs));
                    });
                }),
                new AutoTechBuildBlock("2CC+BarrackReactor", DefaultBot).WithConditions(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BARRACKS, 1, ucs)).WithActionNodes(root =>
                {
                    root.AddActionOnStart("Build2CC", new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 2, md, ucs), node =>
                    {
                        node.AddActionOnResourcesSpend("BuildBarrackReactor", new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_COMMANDCENTER, 2, ucs), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 1, md, ucs));
                        node.AddActionOnResourcesSpend("BuildSupplyDepot", new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_COMMANDCENTER, 2, ucs), new SupplyDepotDesire(2, md, ucs));
                    });
                }),
                new AutoTechBuildBlock("OptionalBuildMoreDefense", DefaultBot).WithConditions(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_COMMANDCENTER, 2, ucs), new CustomCondition(IsEnemyRushing)).WithActionNodes(root =>
                {
                    root.AddActionOnStart("BuildMoreMarines", new UnitDesire(UnitTypes.TERRAN_MARINE, 16, md.DesiredUnitCounts, ucs));
                    root.AddActionOnStart("BuildMoreBarracks", new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 3, md, ucs), node =>
                    {
                        node.AddActionOnResourcesSpend("BuildSecondBunker", new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BARRACKS, 3, ucs), new DefenseStructureDesire(UnitTypes.TERRAN_BUNKER, 2, md, ucs));
                    });
                    root.AddActionOnStart("BuildMoreSupplyDepots", new SupplyDepotDesire(4, md, ucs));
                }),
                new AutoTechBuildBlock("BuildThreeCC", DefaultBot).WithConditions(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_COMMANDCENTER, 2, ucs)).WithActionNodes(root =>
                {
                    root.AddActionOnStart("Build3CC", new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 3, md, ucs), node =>
                    {
                        node.AddActionOnResourcesSpend("BuildSecondOrbitalCenter", new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 2, md, ucs), node =>
                        {
                            node.AddActionOnResourcesSpend("BuildSecondGas", new GasBuildingCountDesire(2, md, ucs));
                        });
                    });
                }),
                new AutoTechBuildBlock("BuildTechFactoryAndStarport", DefaultBot).WithConditions(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_COMMANDCENTER, 3, ucs)).WithActionNodes(root =>
                {
                    root.AddActionOnStart("BuildFactory", new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, 1, md, ucs), node =>
                    {
                        node.AddActionOnResourcesSpend("BuildStarport", new ProductionStructureDesire(UnitTypes.TERRAN_STARPORT, 1, md, ucs));
                        node.AddActionOnResourcesSpend("BuildFactoryTech", new AddonStructureDesire(UnitTypes.TERRAN_FACTORYTECHLAB, 1, md, ucs), node =>
                        {
                            node.AddActionOnResourcesSpend("BuildCyclone", new UnitDesire(UnitTypes.TERRAN_CYCLONE, 1, md.DesiredUnitCounts, ucs));
                        });
                    });
                })
            ];
        }

        //TODO refine conditions
        private bool IsEnemyRushing() => UnitCountService.EnemyCount(UnitTypes.TERRAN_MARINE) >= 2 || UnitCountService.EnemyCount(UnitTypes.TERRAN_BARRACKS) >= 2;
        private bool IsEnemyProxing() => UnitCountService.EnemyCount(UnitTypes.TERRAN_REAPER) >= 1 || UnitCountService.EnemyCount(UnitTypes.TERRAN_BARRACKS) == 0;
        private bool IsEnemyAttacking() => DefaultBot.EnemyData.EnemyAggressivityData.IsHarassing || DefaultBot.EnemyData.EnemyAggressivityData.ArmyAggressivity > 0.8f;

        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);
            string debugMessage = "";

            //Activates new buildBlocks in order if no block is active or enough minerals and gas is present to start additional blocks
            int freeMinerals = MacroData.Minerals - ActiveBuildBlocks.Sum(a => a.MineralCost);
            int freeVespene = MacroData.VespeneGas - ActiveBuildBlocks.Sum(a => a.VespeneCost);
            foreach (BuildBlock block in BuildBlocks.ToList())
            {
                if (block.AreConditionsFulfilled())
                {
                    if (ActiveBuildBlocks.Count == 0 || block.MineralCost < freeMinerals && block.VespeneCost < freeVespene)
                    {
                        debugMessage += "**Activated** " + block.ToString();
                        BuildBlocks.Remove(block);
                        ActiveBuildBlocks.Add(block);
                    }
                }
            }

            //Enforces active buildBlocks
            foreach (BuildBlock block in ActiveBuildBlocks.ToList())
            {
                if (block.HasCompleted())
                {
                    debugMessage += "**Completed** " + block.ToString();
                    ActiveBuildBlocks.Remove(block);
                    continue;
                }

                string treeString = block.ToString();
                block.Enforce();
                if (!treeString.Equals(block.ToString()))
                    debugMessage += "**Updated** " + block.ToString();
            }

            if (SendDebugMessages && debugMessage != "")
            {
                Console.WriteLine(debugMessage);
            }
        }

        public override bool Transition(int frame)
        {
            if (IsEnemyAttacking() || ActiveBuildBlocks.All(a => a.HasSpendResources()) && BuildBlocks.All(a => !a.AreConditionsFulfilled() && a.Name.Contains("Optional")))
            {
                BuildOptions.StrictSupplyCount = false;
                BuildOptions.StrictGasCount = false;
                BuildOptions.StrictWorkerCount = false;
                BuildOptions.StrictWorkersPerGas = false;
                return true;
            }
            return false;
        }
    }
}
