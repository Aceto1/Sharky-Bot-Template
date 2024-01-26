using System.Numerics;
using Sharky.MicroTasks;
using Sharky;
using Sharky.Extensions;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;
using StarCraft2Bot.Bot;

namespace StarCraft2Bot.Builds.Base.Action.BuildBlocks
{
    class ScoutWithTrainedReaper : AutoTechBuildBlock
    {
        public ScoutWithTrainedReaper(BaseBot bot) : base(bot)
        {
            base.WithConditions([
                new UnitCompletedCountCondition(UnitTypes.TERRAN_BARRACKS, 1, DefaultBot.UnitCountService),
                new UnitCompletedCountCondition(UnitTypes.TERRAN_REFINERY, 1, DefaultBot.UnitCountService),
            ]);
            base.WithSerialActions([
                new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_BARRACKS, 1, DefaultBot.UnitCountService), new UnitDesire(UnitTypes.TERRAN_REAPER, 1, DefaultBot.MacroData.DesiredUnitCounts, DefaultBot.UnitCountService)),
                new BuildAction(new UnitCompletedCountCondition(UnitTypes.TERRAN_REAPER, 1, DefaultBot.UnitCountService), new CustomDesire(EnableReaperScouting)),
            ]);
        }

        private void EnableReaperScouting()
        {
            ReaperScoutTask scoutTask = (ReaperScoutTask)DefaultBot.MicroTaskData[typeof(ReaperScoutTask).Name];

            var reaperCommanders = DefaultBot.ActiveUnitData.Commanders.Values.Where(c => c.UnitCalculation.Unit.UnitType == (uint)UnitTypes.TERRAN_REAPER);
            if (reaperCommanders.Count() == 0) return;

            //claim reaper for scout task
            UnitCommander nearestReaperToEnemyBase = reaperCommanders.OrderBy(p => Vector2.DistanceSquared(p.UnitCalculation.Position, DefaultBot.BaseData.EnemyBaseLocations[0].Location.ToVector2())).First();
            nearestReaperToEnemyBase.Claimed = false;
            DefaultBot.MicroTaskData.StealCommanderFromAllTasks(nearestReaperToEnemyBase);
            scoutTask.ClaimUnits(DefaultBot.ActiveUnitData.Commanders.Where(c => c.Value == nearestReaperToEnemyBase).ToDictionary(c => c.Key, c => c.Value));
            scoutTask.Enable();

            //retreat worker scout
            DefaultBot.MicroTaskData[typeof(WorkerScoutTask).Name].Disable();
            DefaultBot.MicroTaskData[typeof(WorkerScoutTask).Name].ResetClaimedUnits();
        }
    }
}
