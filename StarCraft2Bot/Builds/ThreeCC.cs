using System.Numerics;
using SC2APIProtocol;
using Sharky;
using Sharky.Builds;
using Sharky.Chat;
using Sharky.DefaultBot;
using Sharky.Extensions;
using Sharky.Helper;
using Sharky.Managers;
using Sharky.MicroControllers;
using Sharky.MicroTasks;
using Sharky.MicroTasks.Attack;
using Sharky.MicroTasks.Macro;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;
using StarCraft2Bot.Builds.Tasks;

namespace StarCraft2Bot.Builds
{
    public class ThreeCC : AdvancedBuild
    {
        /**
         * TODOS:
         * Bessere Enemy Detection Conditions
         * Drittes CC in Base bauen
         *      ->Wie CC in Base bauen?
         *      ->Wie CC zum Fliegen bringen (x)
         */

        public ThreeCC(BaseBot defaultSharkyBot) : base(defaultSharkyBot)
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

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);
            PhaseCondition.phase = 0;

            List<System.Action> phases = new List<System.Action>() {
                SetInitialBuildSettings,
                FirstBuildPhase,
                BuildDefenseForSecondBuildPhase,
                StartSecondBuildPhase,
                BuildDefenseForThirdBuildPhase,
                StartThirdBuildPhase };
            for (int i = 0; i < phases.Count; i++)
            {
                AddAction(new PhaseCondition(i), new CustomDesire(phases[i]));
            }
        }

        private void SetInitialBuildSettings()
        {
            BuildOptions.StrictSupplyCount = true;
            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictWorkerCount = false;
            BuildOptions.StrictWorkersPerGas = true;
            BuildOptions.StrictWorkersPerGasCount = 3;

            AttackData.CustomAttackFunction = false;
            AttackData.UseAttackDataManager = true;
            AttackData.RequireMaxOut = true;
            AttackData.AttackWhenMaxedOut = false;
            AttackData.RequireBank = false;
            AttackData.AttackTrigger = 1000f;
            AttackData.ArmyFoodAttack = 1000;
            AttackData.AttackWhenOverwhelm = false;
            AttackData.RetreatTrigger = 5f;
            AttackData.Attacking = false;

            NextPhase();
        }

        private void FirstBuildPhase()
        {
            MicroTaskData[typeof(WorkerScoutTask).Name].Enable();
            
            //if barack build -> train reaper
            AddAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_BARRACKS, 1, UnitCountService), 
                new UnitDesire(UnitTypes.TERRAN_REAPER, 1, MacroData.DesiredUnitCounts));
            //if enough gas for 1 reaper -> lower gas workers
            AddAction(new CustomCondition(() => MacroData.VespeneGas > DefaultBot.SharkyUnitData.UnitData[UnitTypes.TERRAN_REAPER].VespeneCost), 
                new CustomDesire(() => { BuildOptions.StrictWorkersPerGasCount = 1; }));
            //if reaper trained -> start reaper scout
            AddAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_REAPER, 1, UnitCountService), 
                new CustomDesire(StartReaperScouting));

            //builds 1 supply depot, 1 barrack, 1 gas refinery, 1 orbital commandcenter
            AppendToBuildOrder(new SupplyCondition(13, MacroData), 
                new SupplyDepotDesire(1, MacroData));
            AppendToBuildOrder(new SupplyCondition(14, MacroData), 
                new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData));
            AppendToBuildOrder(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BARRACKS, 1, UnitCountService), 
                new GasBuildingCountDesire(1, MacroData),
                new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 1, MacroData));
            AddAction(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_ORBITALCOMMAND, 1, UnitCountService), 
                new CustomDesire(NextPhase));
        }

        private void BuildDefenseForSecondBuildPhase()
        {
            //train one marine if enemy is not rushing or four to defend proxy 
            if (!IsEnemyRushing())
            {
                int marinesToTrain = IsEnemyProxing() ? 4 : 1;
                AddAction(new UnitCountCondition(UnitTypes.TERRAN_REAPER, 1, UnitCountService), 
                    new UnitDesire(UnitTypes.TERRAN_MARINE, marinesToTrain, MacroData.DesiredUnitCounts));
                NextPhase();
                return;
            }
            SendDebugMessage("Building Defense");
           
           //builds 8 marines, 1 additional barrack (2 total), 1 bunker
            AddAction(new UnitCountCondition(UnitTypes.TERRAN_REAPER, 1, UnitCountService), 
                new UnitDesire(UnitTypes.TERRAN_MARINE, 8, MacroData.DesiredUnitCounts));
            AppendToBuildOrder(new NoneCondition(), 
                new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 2, MacroData));
            AppendToBuildOrder(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BARRACKS, 2, UnitCountService), 
                new DefenseStructureDesire(UnitTypes.TERRAN_BUNKER, 1, MacroData));
            AppendToBuildOrder(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BUNKER, 1, UnitCountService), 
                new CustomDesire(NextPhase));
        }

        private void StartSecondBuildPhase()
        {
            //builds second command center, second supply depot, 1 barrack-reactor
            AppendToBuildOrder(new NoneCondition(), 
                new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 2, MacroData));
            AppendToBuildOrder(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_COMMANDCENTER, 2, UnitCountService), 
                new SupplyDepotDesire(2, MacroData),
                new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 1, MacroData));
            AppendToBuildOrder(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BARRACKSREACTOR, 1, UnitCountService), 
                new CustomDesire(NextPhase));
        }

        private void BuildDefenseForThirdBuildPhase()
        {
            //skip if enemy is not rushing
            if (!IsEnemyRushing())
            {
                NextPhase();
                return;
            }
            SendDebugMessage("Building more Defense");

            //builds 16 marines, 1 additional barrack (3 total), 1 additional bunker(2 total)
            AppendToBuildOrder(new NoneCondition(), 
                new UnitDesire(UnitTypes.TERRAN_MARINE, 16, MacroData.DesiredUnitCounts),
                new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 3, MacroData),
                new SupplyDepotDesire(3, MacroData));
            AppendToBuildOrder(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BARRACKS, 3, UnitCountService), 
                new DefenseStructureDesire(UnitTypes.TERRAN_BUNKER, 2, MacroData));
            AppendToBuildOrder(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_BUNKER, 2, UnitCountService),
                new CustomDesire(NextPhase));
        }

        private void StartThirdBuildPhase()
        {
            // if factory, upgrade factory-lab
            AddAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_FACTORY, 1, UnitCountService),
                new ProductionStructureDesire(UnitTypes.TERRAN_STARPORT, 1, MacroData),
                new AddonStructureDesire(UnitTypes.TERRAN_FACTORYTECHLAB, 1, MacroData));
            // if factory-lab, build cyclone
            AddAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_FACTORYTECHLAB, 1, UnitCountService), 
                new UnitDesire(UnitTypes.TERRAN_CYCLONE, 1, MacroData.DesiredUnitCounts));

            //builds third command center, 1 factory, second orbital commandcenter, first starport
            AppendToBuildOrder(new NoneCondition(),
                new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 3, MacroData));                                            
            AppendToBuildOrder(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_COMMANDCENTER, 3, UnitCountService),
                new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, 1, MacroData),
                new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 2, MacroData), 
                new GasBuildingCountDesire(2, MacroData),
                new UnitDesire(UnitTypes.TERRAN_MARINE, Math.Max(6, MacroData.DesiredUnitCounts[UnitTypes.TERRAN_MARINE]), MacroData.DesiredUnitCounts));
            AppendToBuildOrder(new BuildingDoneOrInProgressCondition(UnitTypes.TERRAN_STARPORT, 1, UnitCountService), 
                new CustomDesire(NextPhase));
        }
        private void NextPhase()
        {
            PhaseCondition.phase++;
            ChatService.SendDebugChatMessage($"Entering Phase {PhaseCondition.phase} with {MacroData.Minerals} Minerals and {MacroData.VespeneGas} Gas");
        }

        //TODO refine conditions
        private bool IsEnemyRushing()
        {
            if (UnitCountService.EnemyCount(UnitTypes.TERRAN_MARINE) >= 2)
            {
                return true;
            }
            if (UnitCountService.EnemyCount(UnitTypes.TERRAN_BARRACKS) >= 2)
            {
                return true;
            }
            return false;
        }

        private bool IsEnemyProxing()
        {
            if (UnitCountService.EnemyCount(UnitTypes.TERRAN_REAPER) >= 1)
            {
                return true;
            }
            if (UnitCountService.EnemyCount(UnitTypes.TERRAN_BARRACKS) == 0)
            {
                return true;
            }
            return false;
        }

        //TODO refine conditions
        private bool IsEnemyAttacking()
        {
            return DefaultBot.EnemyData.EnemyAggressivityData.IsHarassing || DefaultBot.EnemyData.EnemyAggressivityData.ArmyAggressivity > 0.8f;
        }

        private void StartReaperScouting()
        {
            ReaperScoutTask scoutTask = (ReaperScoutTask)MicroTaskData[typeof(ReaperScoutTask).Name];

            var reaperCommanders = ActiveUnitData.Commanders.Values.Where(c => c.UnitCalculation.Unit.UnitType == (uint)UnitTypes.TERRAN_REAPER);
            if (reaperCommanders.Count() == 0) return;

            //claim reaper for scout task
            UnitCommander nearestReaperToEnemyBase = reaperCommanders.OrderBy(p => Vector2.DistanceSquared(p.UnitCalculation.Position, BaseData.EnemyBaseLocations[0].Location.ToVector2())).First();
            nearestReaperToEnemyBase.Claimed = false;
            MicroTaskData.StealCommanderFromAllTasks(nearestReaperToEnemyBase);
            scoutTask.ClaimUnits(ActiveUnitData.Commanders.Where(c => c.Value == nearestReaperToEnemyBase).ToDictionary(c => c.Key, c => c.Value));
            scoutTask.Enable();

            //retreat worker scout
            MicroTaskData[typeof(WorkerScoutTask).Name].Disable();
            MicroTaskData[typeof(WorkerScoutTask).Name].ResetClaimedUnits();
        }

        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);
        }

        public override bool Transition(int frame)
        {
            if (new PhaseCondition(6).IsFulfilled() || IsEnemyAttacking())
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

    public class PhaseCondition : ICondition
    {
        public static int phase = 0;
        public PhaseCondition(int phaseCount)
        {
            PhaseCount = phaseCount;
        }

        public int PhaseCount { get; set; }

        public bool IsFulfilled()
        {
            return phase >= this.PhaseCount;
        }
    }
}
