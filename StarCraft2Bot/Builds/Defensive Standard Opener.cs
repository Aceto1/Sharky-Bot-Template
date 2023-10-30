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

namespace StarCraft2Bot.Builds
{
    public class DefensiveOpener : Build
    {
        private readonly ProxyLocationService proxyLocationService;
        private bool openingAttackChatSent;
        private readonly ProxyTask proxyTask;

        private Queue<BuildAction>? BuildOrder { get; set; }

        public DefensiveOpener(DefaultSharkyBot defaultSharkyBot, IIndividualMicroController scvMicroController) : base(defaultSharkyBot)
        {
            proxyLocationService = defaultSharkyBot.ProxyLocationService;
            openingAttackChatSent = false;
            proxyTask = new ProxyTask(defaultSharkyBot, false, 0.9f, string.Empty, scvMicroController)
            {
                ProxyName = nameof(DefensiveOpener)
            };
        }
        /**
            14	  0:18	  Supply Depot	  
            15	  0:41	  Barracks	  
            16	  0:45	  Refinery	  
            16	  0:54	  Refinery	  
            19	  1:28	  Reaper, Orbital Command	  
            19	  1:31	  Supply Depot	  
            20    1:42    Factory	  
            21	  2:03	  Reaper	  
            23	  2:21	  Command Center	  
            24	  2:28	  Hellion
        
        Second part: defensive opener
        (not implemented yet)

  	        2:32	Starport	  //Prioritize this over Reactor, Supply Depot and even SCVs if necessary.
  	  	            Barracks Reactor	  
  	  	            Supply Depot	  
  	  	            Widow Mine	  
  	  	            Marine x2	  
  	  	            Factory Tech Lab	  
  	  	            Starport Tech Lab	  
  	        3:12	Refinery	  //To make sure the refinery finishes *with* the CC.
  	  	            Marine x2	
        **/

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictSupplyCount = true;
            BuildOptions.StrictWorkerCount = true;

            MicroTaskData[GetType().Name] = proxyTask;
            var proxyLocation = proxyLocationService.GetCliffProxyLocation();
            MacroData.Proxies[proxyTask.ProxyName] = new ProxyData(proxyLocation, MacroData);
            proxyTask.Enable();

            BuildOrder = new Queue<BuildAction>();
            MacroData.DesiredUnitCounts[UnitTypes.TERRAN_SCV] = 18;
            // supply depot, 14, 0:18
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(14, MacroData), new SupplyDepotDesire(1, MacroData)));
            // barracks, 15, 0:41
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(15, MacroData), new ProxyProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData, proxyTask.ProxyName)));
            // 2 refineries, 16, 0:45/0:54
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(16, MacroData), new GasBuildingCountDesire(1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(16, MacroData), new GasBuildingCountDesire(1, MacroData)));
            // Reaper, 19, 1:28
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 1, MacroData.DesiredUnitCounts)));
            // Orbital Command, 19, 1:28
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 2, MacroData)));
            // Supply depot, 19, 1:31
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new SupplyDepotDesire(1, MacroData)));
            // Factory, 20, 1:42
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(20, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, 1, MacroData)));
            // Reaper, 21, 2:03
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(21, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 1, MacroData.DesiredUnitCounts)));
            // Command center, 23, 2:21
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(23, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 3, MacroData)));
            // Hellion, 24, 2:28
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(24, MacroData), new UnitDesire(UnitTypes.TERRAN_HELLION, 1, MacroData.DesiredUnitCounts)));
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
                proxyTask.Disable();
                return true;
            }

            return false;
        }
    }
}