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
using StarCraft2Bot.Bot;

namespace StarCraft2Bot.Builds
{
    public class ReaperOpener : Build
    {
        private readonly ProxyLocationService proxyLocationService;
        private bool openingAttackChatSent;
        private readonly ProxyTask proxyTask;

        private Queue<BuildAction>? BuildOrder { get; set; }

        public ReaperOpener(BaseBot defaultSharkyBot, IIndividualMicroController scvMicroController) : base(defaultSharkyBot)
        {
            proxyLocationService = defaultSharkyBot.ProxyLocationService;
            openingAttackChatSent = false;
            proxyTask = new ProxyTask(defaultSharkyBot, false, 0.9f, string.Empty, scvMicroController)
            {
                ProxyName = nameof(ReaperOpener)
            };
        }
        /**
            ByuN Proxy barracks
            14	  0:17	  Supply Depot	  
            15	  0:40	  Barracks	  
            16	  0:45	  Refinery	  
            16	  0:56	  Barracks	  
            17	  1:14	  Refinery	  
            18	  1:27	  Reaper	  
            18	  1:28	  Orbital Command	  
            19	  1:39	  Supply Depot	  
            19	  1:43	  Reaper
            21	  2:00	  Reaper
            23	  2:11	  Factory
            23	  2:19	  Reaper
            25	  2:26	  Supply Depot
            25	  2:34	  Reaper
            27	  2:52	  Reaper
            27	  2:57	  Hellion
            31	  3:10	  Command Center	  
            2nd Part
            33	  3:20	  Factory Tech Lab	  
            33	  3:22	  Barracks Reactor	  
            32	  3:27	  Starport	  
            33	  3:37	  Supply Depot	  
            33	  3:42	  Siege Tank	  
            37	  3:59	  Marine x2	  
            40	  4:04	  Medivac	  
            40	  4:08	  Refinery	  
            40	  4:19	  Marine x2	  
            41	  4:28	  Cyclone	  
            46	  4:46	  Orbital Command	  
            46	  4:48	  Marine x2, Starport Tech Lab	  
            49	  5:05	  Siege Tank	  
            49	  5:06	  Marine x2	  
            54	  5:11	  Raven	  
            58	  5:25	  Supply Depot	  
            58	  5:32	  Marine	  
            59	  5:42	  Marine	  
            55	  5:45	  Siege Tank	  
            66	  6:18	  Command Center
            **/

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictWorkerCount = true;

            MicroTaskData[GetType().Name] = proxyTask;
            var proxyLocation = proxyLocationService.GetCliffProxyLocation();
            MacroData.Proxies[proxyTask.ProxyName] = new ProxyData(proxyLocation, MacroData);
            proxyTask.Enable();

            BuildOrder = new Queue<BuildAction>();
            MacroData.DesiredUnitCounts[UnitTypes.TERRAN_SCV] = 18;
            SendScvForFirstDepot(frame);
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(15, MacroData), new ProxyProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData, proxyTask.ProxyName)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(16, MacroData), new GasBuildingCountDesire(1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(16, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(17, MacroData), new GasBuildingCountDesire(2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(18, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 1, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(18, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new UnitDesire(UnitTypes.TERRAN_SCV, 44, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 2, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 3, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(23, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(23, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 4, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(23, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 5, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(23, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 6, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(27, MacroData), new UnitDesire(UnitTypes.TERRAN_HELLION, 1, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(31, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 2, MacroData)));
            //2nd part
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(31, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 0, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_FACTORYTECHLAB, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(32, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_STARPORT, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(33, MacroData), new UnitDesire(UnitTypes.TERRAN_SIEGETANK, 1, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(37, MacroData), new UnitDesire(UnitTypes.TERRAN_MARINE, 2, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(40, MacroData), new UnitDesire(UnitTypes.TERRAN_MEDIVAC, 1, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(40, MacroData), new GasBuildingCountDesire(3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(40, MacroData), new UnitDesire(UnitTypes.TERRAN_MARINE, 4, MacroData.DesiredUnitCounts)));  
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(41, MacroData), new UnitDesire(UnitTypes.TERRAN_CYCLONE, 1, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(46, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 2, MacroData))); 
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(46, MacroData), new UnitDesire(UnitTypes.TERRAN_MARINE, 6, MacroData.DesiredUnitCounts)));  
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(46, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_STARPORTTECHLAB, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(49, MacroData), new UnitDesire(UnitTypes.TERRAN_SIEGETANK, 2, MacroData.DesiredUnitCounts)));  
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(49, MacroData), new UnitDesire(UnitTypes.TERRAN_MARINE, 8, MacroData.DesiredUnitCounts))); 
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(54, MacroData), new UnitDesire(UnitTypes.TERRAN_RAVEN, 1, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(58, MacroData), new UnitDesire(UnitTypes.TERRAN_MARINE, 9, MacroData.DesiredUnitCounts))); 
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(59, MacroData), new UnitDesire(UnitTypes.TERRAN_MARINE, 10, MacroData.DesiredUnitCounts))); 
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(55, MacroData), new UnitDesire(UnitTypes.TERRAN_SIEGETANK, 3, MacroData.DesiredUnitCounts))); 
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(66, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(18, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 3, MacroData)));
        }

        private int FrameFromTime(int minutes, int seconds)
        {
            return (int)((minutes * 60 + seconds) * 22.4);
        }

        /*void SetAttack()
        {
            AttackData.Attacking = true;
            if (!openingAttackChatSent)
            {
                ChatService.SendChatType($"{nameof(ReaperOpener)}-FirstAttack");
                openingAttackChatSent = true;
            }
        }*/
            
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

            //ManageAttackCondition(observation);
        }

        private void ManageAttackCondition(ResponseObservation observation)
        {
            if (UnitCountService.EquivalentTypeCount(UnitTypes.TERRAN_REAPER) >= 5)
            {
                //SetAttack();
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
                proxyTask.Disable();
                return true;
            }

            return base.Transition(frame);
        }
    }
}
