using SC2APIProtocol;
using Sharky.Builds;
using Sharky.Chat;
using Sharky.DefaultBot;
using Sharky.MicroControllers;
using Sharky.MicroTasks;
using Sharky.Proxy;
using Sharky;
using StarCraft2Bot.Builds.Base;
using StarCraft2Bot.Builds.Base.Desires;
using StarCraft2Bot.Builds.Base.Condition;
using Sharky.Builds.QuickBuilds;
using System.Dynamic;
using Sharky.MicroTasks.Attack;
using StarCraft2Bot.Bot;

namespace StarCraft2Bot.Builds
{
    public class SaltyMarines : Build
    {
        private readonly ProxyLocationService proxyLocationService;
        private bool openingAttackChatSent;
        private readonly ProxyTask proxyTask;

        private Queue<BuildAction>? BuildOrder { get; set; }

        public SaltyMarines(BaseBot defaultSharkyBot) : base(defaultSharkyBot)
        {
            defaultSharkyBot.MicroController = new AdvancedMicroController(defaultSharkyBot);
            var advancedAttackTask = new AdvancedAttackTask(defaultSharkyBot, new EnemyCleanupService(defaultSharkyBot.MicroController, defaultSharkyBot.DamageService), new List<UnitTypes> { UnitTypes.TERRAN_MARINE }, 100f, true);
            defaultSharkyBot.MicroTaskData[typeof(AttackTask).Name] = advancedAttackTask;
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            SendScvForFirstDepot(frame);

            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictWorkerCount = false;
            MicroTaskData[typeof(WorkerScoutTask).Name].Enable();
            MicroTaskData[typeof(AttackTask).Name].Enable();

            BuildOrder = new Queue<BuildAction>();
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(15, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(16, MacroData), new GasBuildingCountDesire(1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(18, MacroData), new UnitDesire(UnitTypes.TERRAN_MARINE, 100, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(18, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(22, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(22, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSTECHLAB, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(23, MacroData), new GasBuildingCountDesire(2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(34, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(35, MacroData), new TechStructureDesire(UnitTypes.TERRAN_ENGINEERINGBAY, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYARMORSLEVEL1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.STIMPACK, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.SHIELDWALL, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(36, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(32, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(35, MacroData), new GasBuildingCountDesire(3, MacroData)));

            //2nd part
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(40, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(42, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_FACTORYTECHLAB, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(40, MacroData), new UnitDesire(UnitTypes.TERRAN_SIEGETANK, 3, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_COMMANDCENTER, 3, UnitCountService), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 5, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(42, MacroData), new TechStructureDesire(UnitTypes.TERRAN_ARMORY, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(45, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_STARPORT, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(48, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_STARPORTREACTOR, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(47, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(40, MacroData), new UnitDesire(UnitTypes.TERRAN_MEDIVAC, 10, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(50, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYARMORSLEVEL2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(50, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(70, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(70, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYARMORSLEVEL3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(50, MacroData), new MorphDesire(UnitTypes.TERRAN_PLANETARYFORTRESS, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(70, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 8, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(80, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 4, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(100, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 6, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(50, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(80, MacroData), new UnitDesire(UnitTypes.TERRAN_SIEGETANK, 6, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(150, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 6, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(150, MacroData), new MorphDesire(UnitTypes.TERRAN_PLANETARYFORTRESS, 3, MacroData)));
        }

        private int FrameFromTime(int minutes, int seconds)
        {
            return (int)((minutes * 60 + seconds) * 22.4);
        }

        void SetAttack()
        {
            AttackData.Attacking = true;
            if (!openingAttackChatSent)
            {
                ChatService.SendChatType($"{nameof(SaltyMarines)}-FirstAttack");
                openingAttackChatSent = true;
            }
        }
            
        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);
            if (BuildOrder == null)
            {
                throw new InvalidOperationException("BuildOrder has not been initialized.");
            }

            if (BuildOrder.Count == 0)
            {
                return;
            }

            var nextAction = BuildOrder.Peek();

            if (nextAction.AreConditionsFulfilled())
            {
                nextAction.EnforceDesires();
                BuildOrder.Dequeue();
            }

            ManageAttackCondition(observation);
            // falls bestimmte struktur des Gegners nicht gefunden, setze scouting fort
            // MicroTaskData[typeof(ProxyScoutTask).Name].Enable();
        }

        private void ManageAttackCondition(ResponseObservation observation)
        {
            if (MacroData.FoodArmy >= 60 || MacroData.FoodUsed > 125)
            {
                SetAttack();
            }
        }

        public override bool Transition(int frame)
        {
            if (BuildOrder == null)
            {
                throw new InvalidOperationException("BuildOrder has not been initialized.");
            }

            if (BuildOrder.Count == 0)
            {
                AttackData.UseAttackDataManager = true;
                return true;
            }

            return base.Transition(frame);
        }
    }
}
