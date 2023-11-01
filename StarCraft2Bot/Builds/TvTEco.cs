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

namespace StarCraft2Bot.Builds
{
    public class TvTEco : Build
    {
        private readonly ProxyLocationService proxyLocationService;
        private bool openingAttackChatSent;
        private readonly ProxyTask proxyTask;

        private Queue<BuildAction>? BuildOrder { get; set; }

        public TvTEco(DefaultSharkyBot defaultSharkyBot, IIndividualMicroController scvMicroController) : base(defaultSharkyBot)
        {
            defaultSharkyBot.MicroController = new AdvancedMicroController(defaultSharkyBot);
            var advancedAttackTask = new AdvancedAttackTask(defaultSharkyBot, new EnemyCleanupService(defaultSharkyBot.MicroController, defaultSharkyBot.DamageService), new List<UnitTypes> { UnitTypes.TERRAN_MARINE }, 100f, true);
            defaultSharkyBot.MicroTaskData[typeof(AttackTask).Name] = advancedAttackTask;
        }

        /*          
  14	  0:17	  Supply Depot	  
  15	  0:26	  Refinery Rally 15th SCV to Gas Gyser	  
  16	  0:44	  Barracks Fully Saturate Refinery with 16th SCV rally and 1 more from Mineral LIne. Then send 1 SCV to scout.	  
  19	  1:35	  Reaper, Orbital Command	  
  20	  1:47	  Command Center	  
  20	  1:57	  Factory IMPORTANT: Factory BEFORE 2nd Depot.	  
  21	  2:07	  Barracks Reactor Marine Prod. @ 100%	  
  22	  2:16	  Supply Depot	  
  22	  2:20	  Refinery Saturate @ 100%	  
  24	  2:43	  Factory Tech Lab	  
  26	  2:45	  Marine x2 Start Marine Prod. Stop @ 4	  
  30	  3:00	  Orbital Command, Cyclone Nat. CC, @ 100% Fac. Tech Lab	  
  33	  3:03	  Marine x2	  
  33	  3:11	  Starport @ 100 Gas. Prioritize SCV. // Retreat from Nat. & defend high ground until Cyclone is done. Pause Marine Prod. @ 4 Marines	  
  39	  3:37	  Siege Tank @ 100% Cyclone: Retake Nat. w/ units, transfer SCVs to Nat.	  
  41	  3:44	  Supply Depot, Refinery Cont. Supply Depot Prod.	  
  43	  3:51	  Marine x2 Cut Marine Prod. @ 6 Marines	  
  44	  3:52	  Starport Tech Lab @ 100% Starport	  
  51	  4:18	  Raven	  
  51	  4:29	  Command Center @ 100% Transfer to 3rd Base Loaction. Should line up w/ Nat. Full Saturation	  
  55	  4:34	  Siege Tank	  
  58	  4:41	  Marine x2 Start Marine Prod. Again // @ 5:00 - 5:30Scan Opp. Base to see what they're building.	  
  65	  5:01	  Raven As Gas Allows. Stop Raven Prod. @ 2 Ravens	  
  70	  5:06	  Siege Tank As Gas Allows	  
  70	  5:12	  Refinery Take when Nat. @ 16/16 Mineral Saturation	  
  70	  5:13	  Supply Depot Begin making 2 Supp. Depots at a time.	  
  78	  5:34	  Barracks x2	  
  79	  5:41	  Siege Tank Constant Siege Tank & Marine Prod.	  
  81	  5:44	  Starport Reactor Lift Starport & Replace w/ Barracks on Tech Lab for Stim.	  
  82	  5:54	  Engineering Bay x2, Stimpack	  
  83	  6:03	  Orbital Command @ 3rd CC on Location.	  
  90	  6:27	  Terran Infantry Weapons Level 1, Terran Infantry Armor Level 1 @ 100% Engi. Bays. | Armory @ 50% +1/+1	  
  92	  6:33	  Starport Reactor @ 100% Barracks: Lift Starport to build another Reactor. Lift Barracks to empty Reactors.	  
  104	  6:50	  Barracks x2	  
  108	  6:59	  Refinery x2 Grab both Gases @ 3rd when 16/16 Mineral Sat.	  
  118	  7:13	  Medivac x2	  
  121	  7:24	  Armory @ this point of the game just Macro Cycle. Spend Gas, then Minerals on Marines, then Depots. (Sidenote: HuShang stop SCV Prod.)	  
  135	  7:49	  Viking x2 Start Viking Prod. after 2 Medivacs OR Continue Medivac Prod. if AGGRO focused.	  
  140	  8:00	  Command Center 4th CC. | Turrets and Sensor Towers. "TvT Chess Games Begins"	  
  140	  8:17	  Terran Vehicle Weapons Level 1 @ 100 Armory	  
  145	  8:22	  Terran Infantry Armor Level 2, Terran Infantry Weapons Level 2 @ 100% +1/+1
         */

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            SendScvForFirstDepot(frame);
            
            //MicroTaskData[typeof(WorkerScoutTask).Name].Enable();
            MicroTaskData[typeof(AttackTask).Name].Enable();

            BuildOrder = new Queue<BuildAction>();
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(14, MacroData), new SupplyDepotDesire(1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(15, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(16, MacroData), new GasBuildingCountDesire(1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(18, MacroData), new UnitDesire(UnitTypes.TERRAN_MARINE, 100, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(18, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(22, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(22, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSTECHLAB, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(23, MacroData), new GasBuildingCountDesire(2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(34, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 5, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(35, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_ENGINEERINGBAY, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYARMORSLEVEL1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.STIMPACK, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.SHIELDWALL, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(36, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(35, MacroData), new GasBuildingCountDesire(2, MacroData)));
            //2nd part
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(40, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(42, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_FACTORYTECHLAB, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitDesire(UnitTypes.TERRAN_SIEGETANK, 3, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(42, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_ARMORY, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(45, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_STARPORT, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(48, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_STARPORTREACTOR, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(47, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(40, MacroData), new UnitDesire(UnitTypes.TERRAN_MEDIVAC, 10, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(50, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYARMORSLEVEL2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(50, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(70, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(70, MacroData), new UnitUpgradeDesire(Upgrades.TERRANINFANTRYARMORSLEVEL3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(66, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(50, MacroData), new MorphDesire(UnitTypes.TERRAN_PLANETARYFORTRESS, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(70, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 8, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(80, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 4, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(100, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 6, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(50, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(150, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 6, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(50, MacroData), new MorphDesire(UnitTypes.TERRAN_PLANETARYFORTRESS, 3, MacroData)));
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
                ChatService.SendChatType($"{nameof(ReaperOpener)}-FirstAttack");
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
        }

        private void ManageAttackCondition(ResponseObservation observation)
        {
            if (MacroData.FoodArmy >= AttackData.ArmyFoodAttack || MacroData.FoodUsed > 175)
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

            return false;
        }
    }
}
