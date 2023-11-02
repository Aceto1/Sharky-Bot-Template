using SC2APIProtocol;
using Sharky;
using Sharky.DefaultBot;
using Sharky.MicroTasks;
using StarCraft2Bot.Builds.Base;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds
{
    public class TvTOpener : Build
    {
        public Queue<BuildAction> BuildOrder { get; set; }

        public TvTOpener(DefaultSharkyBot defaultSharkyBot) : base(defaultSharkyBot)
        {
            BuildOrder = new Queue<BuildAction>();
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictSupplyCount = true;
            BuildOptions.StrictWorkerCount = true;

            MacroData.DesiredUnitCounts[UnitTypes.TERRAN_SCV] = 19;


            AddAction(new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_SUPPLYDEPOT, 1, UnitCountService),
                          new CustomDesire(() => {
                              MicroTaskData[typeof(WorkerScoutTask).Name].Enable();
                              MicroTaskData[typeof(AttackTask).Name].Enable();
                          })));

            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(14, MacroData),
                                               new SupplyDepotDesire(1, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(15, MacroData),
                                               new GasBuildingCountDesire(1, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(16, MacroData),
                                               new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), 
                                               new UnitDesire(UnitTypes.TERRAN_REAPER, 1, MacroData.DesiredUnitCounts),
                                               new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 1, MacroData)));

            // ENTWEDER: Gegner hat Baracke in der Base => CC bauen, ODER: Gegner hat keine Baracke in der Base => Gegner spielt Proxy => Transition
            // Wir tun mal so als spielt der Gegner nie Proxy, TODO: anderen Fall implementieren (Hausaufgaben yay)
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(20, MacroData),
                                               new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 2, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_COMMANDCENTER, 2, UnitCountService),
                                               new UnitDesire(UnitTypes.TERRAN_SCV, 24, MacroData.DesiredUnitCounts)));

            BuildOrder.Enqueue(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_COMMANDCENTER, 2, UnitCountService),
                                               new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, 1, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_REAPER, 1, UnitCountService),
                                               new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 1, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_FACTORY, 1, UnitCountService),
                                               new SupplyDepotDesire(2, MacroData),
                                               new AddonStructureDesire(UnitTypes.TERRAN_FACTORYTECHLAB, 1, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(22, MacroData),
                                               new GasBuildingCountDesire(2, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_FACTORY, 1, UnitCountService),
                                               new UnitDesire(UnitTypes.TERRAN_CYCLONE, 1, MacroData.DesiredUnitCounts)));

            BuildOrder.Enqueue(new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_BARRACKSREACTOR, 1, UnitCountService),
                                               new UnitDesire(UnitTypes.TERRAN_MARINE, 2, MacroData.DesiredUnitCounts)));

            BuildOrder.Enqueue(new BuildAction(new WorkerCountCondition(23, UnitCountService),
                                               new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 2, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_MARINE, 2, UnitCountService),
                                               new UnitDesire(UnitTypes.TERRAN_MARINE, 4, MacroData.DesiredUnitCounts)));

            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(29, MacroData),
                                               new ProductionStructureDesire(UnitTypes.TERRAN_STARPORT, 1, MacroData)));

            BuildOrder.Enqueue(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_STARPORT, 1, UnitCountService),
                                               new UnitDesire(UnitTypes.TERRAN_CYCLONE, 2, MacroData.DesiredUnitCounts)));
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
