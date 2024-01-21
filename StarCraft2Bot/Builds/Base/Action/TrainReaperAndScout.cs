using System.Numerics;
using Sharky.MicroTasks;
using Sharky;
using Sharky.Extensions;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds.Base.Action
{
    class ScoutWithTrainedReaper : BuildBlock
    {
        MicroTaskData MicroTaskData;
        MacroData MacroData;
        ActiveUnitData ActiveUnitData;
        UnitCountService UnitCountService;
        BaseData BaseData;

        public ScoutWithTrainedReaper(MicroTaskData microTaskData, MacroData macroData, ActiveUnitData activeUnitData, UnitCountService unitCountService ,BaseData baseData)
        {
            MicroTaskData = microTaskData;
            MacroData = macroData;
            ActiveUnitData = activeUnitData;
            UnitCountService = unitCountService;
            BaseData = baseData;

            Conditions.Add(new UnitCompletedCountCondition(UnitTypes.TERRAN_BARRACKS, 1, UnitCountService));
            Conditions.Add(new UnitCompletedCountCondition(UnitTypes.TERRAN_REFINERY, 1, UnitCountService));

            SerialBuildActions.Add(new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_BARRACKS, 1, UnitCountService),
                new UnitDesire(UnitTypes.TERRAN_REAPER, 1, MacroData.DesiredUnitCounts, UnitCountService)));
            SerialBuildActions.Add(new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_REAPER, 1, UnitCountService),
                new CustomDesire(EnableReaperScouting)));
        }

        private void EnableReaperScouting()
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
    }
}
