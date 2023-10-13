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

        public ReaperOpener(DefaultSharkyBot defaultSharkyBot, IIndividualMicroController scvMicroController) : base(defaultSharkyBot)
        {
            proxyLocationService = defaultSharkyBot.ProxyLocationService;
            openingAttackChatSent = false;
            proxyTask = new ProxyTask(defaultSharkyBot, false, 0.9f, string.Empty, scvMicroController)
            {
                ProxyName = nameof(ReaperOpener)
            };
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictSupplyCount = true;
            BuildOptions.StrictWorkerCount = true;

            MacroData.DesiredUnitCounts[UnitTypes.TERRAN_SCV] = 15;

            var desiredUnitsClaim = new DesiredUnitsClaim(UnitTypes.TERRAN_REAPER, 1);
            if (MicroTaskData.ContainsKey("DefenseSquadTask"))
            {
                var defenseSquadTask = (DefenseSquadTask)MicroTaskData["DefenseSquadTask"];
                defenseSquadTask.DesiredUnitsClaims = new List<DesiredUnitsClaim> { desiredUnitsClaim };
                defenseSquadTask.Enable();

                if (MicroTaskData.ContainsKey("AttackTask"))
                {
                    MicroTaskData["AttackTask"].ResetClaimedUnits();
                }
            }

            MicroTaskData[nameof(ReaperOpener)] = proxyTask;
            var proxyLocation = proxyLocationService.GetCliffProxyLocation();
            MacroData.Proxies.Add(proxyTask.ProxyName, new ProxyData(proxyLocation, MacroData));
            proxyTask.Enable();

            AttackData.CustomAttackFunction = true;
            AttackData.UseAttackDataManager = false;
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

            AddAction(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_REAPER, 5, UnitCountService), new CustomDesire(SetAttack)));

            //SetAttack();

            AddAction(new BuildAction(new SupplyCondition(15, MacroData, ConditionOperator.GreaterOrEqual), new SupplyDepotDesire(1, MacroData)));
            
            //if (MacroData.FoodUsed >= 15)
            //{
            //    if (MacroData.DesiredSupplyDepots < 1)
            //    {
            //        MacroData.DesiredSupplyDepots = 1;
            //    }
            //}

            AddActionOnCompletedStructure(UnitTypes.TERRAN_SUPPLYDEPOT, 1, UnitTypes.TERRAN_BARRACKS, 1, MacroData.Proxies[proxyTask.ProxyName].DesiredProductionCounts);

            AddAction(new BuildAction(new AndCondition(new UnitCountCondition(UnitTypes.TERRAN_SUPPLYDEPOT, 0, UnitCountService),
                                                       new UnitCountCondition(UnitTypes.TERRAN_BARRACKS, 0, UnitCountService)),
                                      new UnitDesire(UnitTypes.TERRAN_BARRACKS, 3, MacroData.DesiredProductionCounts),
                                      new GasBuildingCountDesire(1, MacroData)));

            //if (UnitCountService.EquivalentTypeCompleted(UnitTypes.TERRAN_SUPPLYDEPOT) > 0 && UnitCountService.Count(UnitTypes.TERRAN_BARRACKS) > 0)
            //{
            //    if (MacroData.DesiredGases < 1)
            //    {
            //        MacroData.DesiredGases = 1;
            //    }
            //    if (MacroData.DesiredProductionCounts[UnitTypes.TERRAN_BARRACKS] < 3)
            //    {
            //        MacroData.DesiredProductionCounts[UnitTypes.TERRAN_BARRACKS] = 3;
            //    }
            //}

            AddAction(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_BARRACKS, 0, UnitCountService), 
                      new UnitDesire(UnitTypes.TERRAN_REAPER, 10, MacroData.DesiredUnitCounts),
                      new SupplyDepotDesire(2, MacroData)));

            //if (UnitCountService.Completed(UnitTypes.TERRAN_BARRACKS) > 0)
            //{
            //    if (MacroData.DesiredUnitCounts[UnitTypes.TERRAN_REAPER] < 10)
            //    {
            //        MacroData.DesiredUnitCounts[UnitTypes.TERRAN_REAPER] = 10;
            //    }

            //    if (MacroData.DesiredSupplyDepots < 2)
            //    {
            //        MacroData.DesiredSupplyDepots = 2;
            //    }
            //}

            AddAction(new BuildAction(new AndCondition(new UnitCountCondition(UnitTypes.TERRAN_COMMANDCENTER, 0, UnitCountService),
                                                       new UnitCountCondition(UnitTypes.TERRAN_BARRACKS, 0, UnitCountService),
                                                       new UnitCountCondition(UnitTypes.TERRAN_SUPPLYDEPOT, 2, UnitCountService, ConditionOperator.GreaterOrEqual)),
                      new UnitDesire(UnitTypes.TERRAN_ORBITALCOMMAND, 1, MacroData.DesiredMorphCounts),
                      new CustomDesire(() =>
                      {
                          BuildOptions.StrictSupplyCount = false;
                      })));

            //if (UnitCountService.Completed(UnitTypes.TERRAN_BARRACKS) > 0 && UnitCountService.Count(UnitTypes.TERRAN_COMMANDCENTER) > 0 && UnitCountService.EquivalentTypeCompleted(UnitTypes.TERRAN_SUPPLYDEPOT) >= 2)
            //{
            //    if (MacroData.DesiredMorphCounts[UnitTypes.TERRAN_ORBITALCOMMAND] < 1)
            //    {
            //        MacroData.DesiredMorphCounts[UnitTypes.TERRAN_ORBITALCOMMAND] = 1;
            //    }

            //    BuildOptions.StrictSupplyCount = false;
            //}

            AddAction(new BuildAction(new UnitCountCondition(UnitTypes.TERRAN_ORBITALCOMMAND, 1, UnitCountService, ConditionOperator.GreaterOrEqual),
                                      new CustomDesire(() =>
                                      {
                                          BuildOptions.StrictWorkerCount = false;
                                          BuildOptions.StrictGasCount = false;
                                      })));

            //if (UnitCountService.Count(UnitTypes.TERRAN_ORBITALCOMMAND) > 0)
            //{
            //    BuildOptions.StrictWorkerCount = false;
            //    BuildOptions.StrictGasCount = false;
            //}

            AddAction(new BuildAction(new MineralCondition(500, MacroData),
                                      new CustomDesire(() =>
                                      {
                                          if (MacroData.DesiredProductionCounts[UnitTypes.TERRAN_COMMANDCENTER] <= UnitCountService.EquivalentTypeCount(UnitTypes.TERRAN_COMMANDCENTER))
                                          {
                                              MacroData.DesiredProductionCounts[UnitTypes.TERRAN_COMMANDCENTER]++;
                                          }
                                      })));

            //if (MacroData.Minerals > 500)
            //{
            //    if (MacroData.DesiredProductionCounts[UnitTypes.TERRAN_COMMANDCENTER] <= UnitCountService.EquivalentTypeCount(UnitTypes.TERRAN_COMMANDCENTER))
            //    {
            //        MacroData.DesiredProductionCounts[UnitTypes.TERRAN_COMMANDCENTER]++;
            //    }
            //}
        }

        public override bool Transition(int frame)
        {
            if (UnitCountService.EquivalentTypeCount(UnitTypes.TERRAN_COMMANDCENTER) >= 2)
            {
                AttackData.UseAttackDataManager = true;
                proxyTask.Disable();
                return true;
            }

            return false;
        }
    }
}
