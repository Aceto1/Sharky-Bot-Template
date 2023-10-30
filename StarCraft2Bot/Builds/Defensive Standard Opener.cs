using SC2APIProtocol;
using Sharky.Builds;
using Sharky.Chat;
using Sharky.DefaultBot;
using Sharky.MicroControllers;
using Sharky.MicroTasks;
using Sharky;
using StarCraft2Bot.Builds.Base;
using StarCraft2Bot.Builds.Base.Desires;
using StarCraft2Bot.Builds.Base.Condition;

namespace StarCraft2Bot.Builds
{
    public class DefensiveOpener : Build
    {
        private bool openingAttackChatSent;

        private Queue<BuildAction>? BuildOrder { get; set; }

        public DefensiveOpener(DefaultSharkyBot defaultSharkyBot, IIndividualMicroController scvMicroController) : base(defaultSharkyBot)
        {
            openingAttackChatSent = false;
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            BuildOptions.StrictGasCount = true;
            // BuildOptions.StrictSupplyCount = true;
            // BuildOptions.StrictWorkerCount = true;

            BuildOrder = new Queue<BuildAction>();
            MacroData.DesiredUnitCounts[UnitTypes.TERRAN_SCV] = 18;
            // supply depot, 14, 0:18
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(14, MacroData), new SupplyDepotDesire(1, MacroData)));
            // Barrack
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(15, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData)));
            // 1 refinery, 16, 0:45/0:54
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(16, MacroData), new GasBuildingCountDesire(1, MacroData)));
            // Reaper, 19
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 3, MacroData.DesiredUnitCounts)));
            // Orbital Command, 19
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 2, MacroData)));
            // Supply depot, 19
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new SupplyDepotDesire(1, MacroData)));
            // Factory, 20
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(20, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, 1, MacroData)));
            // Reaper, 21
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(21, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 1, MacroData.DesiredUnitCounts)));
            // Command center, 23
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(23, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 3, MacroData)));
            // Hellion, 24
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(24, MacroData), new UnitDesire(UnitTypes.TERRAN_HELLION, 1, MacroData.DesiredUnitCounts)));
            // StarPort, 24
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(24, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_STARPORT, 1, MacroData)));
            // Barracks Reactor
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(24, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 1, MacroData)));
            // Supply Depot
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(24, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_SUPPLYDEPOT, 1, MacroData)));
            // Widow Mine
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(24, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_WIDOWMINE, 1, MacroData)));
            // Marine x2
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(24, MacroData), new UnitDesire(UnitTypes.TERRAN_MARINE, 2, MacroData.DesiredUnitCounts)));
            
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
                ChatService.SendChatType($"{nameof(DefensiveOpener)}-FirstAttack");
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
            if (UnitCountService.EquivalentTypeCount(UnitTypes.TERRAN_REAPER) >= 5)
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