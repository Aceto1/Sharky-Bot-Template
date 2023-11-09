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
    public class ReaperOpener : Build
    {
        private readonly ProxyLocationService proxyLocationService;
        private bool openingAttackChatSent;
        private readonly ProxyTask proxyTask;

        private Queue<BuildAction>? BuildOrder { get; set; }
        
        public ReaperOpener(DefaultSharkyBot defaultSharkyBot, IIndividualMicroController scvMicroController) : base(defaultSharkyBot)
        {
            proxyLocationService = defaultSharkyBot.ProxyLocationService;
            openingAttackChatSent = false;
            proxyTask = new ProxyTask(defaultSharkyBot, false, 0.9f, string.Empty, scvMicroController)
            {
                ProxyName = nameof(ReaperOpener)
            };

        }
        /*  
  12	  0:01	  SCV	  
  13	  0:13	  SCV	  
  14	  0:18	  Supply Depot	  
  14	  0:25	  SCV	  
  15	  0:41	  Barracks	  
  15	  0:45	  Refinery	  
  16	  1:00	  Barracks	  
  16	  1:01	  SCV	  
  16	  1:13	  SCV	  
  17	  1:29	  Reaper	  
  17	  1:30	  Barracks	  
  18	  1:33	  Refinery	  
  18	  1:36	  SCV	  
  19	  1:50	  Reaper	  
  20	  1:55	  Orbital Command	  
  20	  2:01	  Reaper	  
  21	  2:17	  Reaper	  
  21	  2:19	  Supply Depot	 
  21	  2:22	  Reaper	  x
  23	  2:40	  Reaper	  
  23	  2:49	  Reaper	  
  23	  2:50	  SCV	  
  24	  2:56	  Reaper	  
  25	  3:12	  Reaper	  
  25	  3:13	  SCV	  
  25	  3:19	  Factory	  
  26	  3:25	  SCV	  
  25	  3:38	  Reaper	  
  25	  3:39	  SCV	  
  28	  3:58	  Supply Depot	  
  28	  4:02	  Factory Tech Lab	  
  29	  4:21	  Command Center
        */

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
            MacroData.DesiredUnitCounts[UnitTypes.TERRAN_SCV] = 21;

            //MicroTaskData[typeof(WorkerScoutTask).Name].Enable();
            //MicroTaskData[typeof(AttackTask).Name].Enable();
            //MicroTaskData[typeof(ReaperWorkerHarassTask).Name].Enable();


            AddAction(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_REAPER, 1, UnitCountService),
                         new CustomDesire(() => {
                             proxyTask.Disable();
                             MicroTaskData[typeof(WorkerScoutTask).Name].Enable();
                             MicroTaskData[typeof(AttackTask).Name].Enable();
                             MicroTaskData[typeof(ReaperWorkerHarassTask).Name].Enable();
                         })));

            AddAction(new BuildAction(new EnemyUnitCountCondition(UnitTypes.TERRAN_MARINE, 2, UnitCountService),
                         new CustomDesire(() =>
                         {
                             MicroTaskData[typeof(WorkerScoutTask).Name].Disable();
                             MicroTaskData[typeof(ReaperWorkerHarassTask).Name].Disable();

                         })));

            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(14, MacroData), new SupplyDepotDesire(1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(15, MacroData), new ProxyProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData, proxyTask.ProxyName)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(15, MacroData), new GasBuildingCountDesire(1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(16, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData)));
            //BuildOrder.Enqueue(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_BARRACKS,1, UnitCountService), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(17, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER,1, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(17, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, 2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(18, MacroData), new GasBuildingCountDesire(2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(19, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 2, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(20, MacroData), new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(20, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 4, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(21, MacroData), new SupplyDepotDesire(2, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(21, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 5, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(23, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 7, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(24, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 9, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(25, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(25, MacroData), new UnitDesire(UnitTypes.TERRAN_REAPER, 10, MacroData.DesiredUnitCounts)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(28, MacroData), new SupplyDepotDesire(3, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(28, MacroData), new AddonStructureDesire(UnitTypes.TERRAN_FACTORYTECHLAB, 1, MacroData)));
            BuildOrder.Enqueue(new BuildAction(new SupplyCondition(29, MacroData), new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, 1, MacroData)));
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

        /*private void ManageAttackCondition(ResponseObservation observation)
        {
            if (UnitCountService.EquivalentTypeCount(UnitTypes.TERRAN_REAPER) >= 1)
            {
                SetAttack();
            }
        }*/

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
