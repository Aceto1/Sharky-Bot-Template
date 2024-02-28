﻿using SC2APIProtocol;
using Sharky;
using Sharky.Pathing;

namespace StarCraft2Bot.Helper
{
    /// <summary>
    /// A Manager to approximate the enemys minerals and the safety of this Approximation
    /// </summary>
    public class EnemyInformationsManager
    {
        private UnitCountService UnitCountService;
        private MapDataService MapDataService;
        private EnemyUnitMemoryService EnemyUnitMemoryService;
        private SharkyUnitData SharkyUnitData;
        private FrameToTimeConverter FrameToTimeConverter;
        private MapMemoryService MapMemoryService;
        private EnemyUnitApproximationService EnemyUnitApproximationService;

        public EnemyInformationsManager(
            UnitCountService UnitCountService,
            MapDataService MapDataService,
            ActiveUnitData ActiveUnitData,
            EnemyUnitMemoryService enemyUnitMemoryService,
            SharkyUnitData sharkyUnitData,
            FrameToTimeConverter frameToTimeConverter,
            MapMemoryService mapMemoryService,
            EnemyUnitApproximationService enemyUnitApproximationService
        )
        {
            this.UnitCountService = UnitCountService;
            this.MapDataService = MapDataService;
            this.EnemyUnitMemoryService = enemyUnitMemoryService;
            this.SharkyUnitData = sharkyUnitData;
            this.FrameToTimeConverter = frameToTimeConverter;
            this.MapMemoryService = mapMemoryService;
            this.EnemyUnitApproximationService = enemyUnitApproximationService;

            //DatabaseContext database = new DatabaseContext();
        }

        public long GetCurrentEnemyUnitMineralCost()
        {
            long minerals = 0;
            foreach (var unitType in EnemyUnitMemoryService.CurrentTotalUnits.Keys)
            {
                minerals +=
                    SharkyUnitData.UnitData[unitType].MineralCost
                    * EnemyUnitMemoryService.CurrentTotalUnits[unitType];
            }

            return minerals;
        }

        public long GetCurrentEnemyUnitVespeneCost()
        {
            long minerals = 0;
            foreach (var unitType in EnemyUnitMemoryService.CurrentTotalUnits.Keys)
            {
                minerals +=
                    SharkyUnitData.UnitData[unitType].VespeneCost
                    * EnemyUnitMemoryService.CurrentTotalUnits[unitType];
            }

            return minerals;
        }

        public long GetCurrentEnemyArmyMineralCost()
        {
            long minerals = 0;
            foreach (var unitType in EnemyUnitMemoryService.CurrentTotalUnits.Keys)
            {
                if (unitType != UnitTypes.TERRAN_SCV)
                {
                    if (SharkyUnitData.UnitData[unitType].Weapons.Count > 0)
                    {
                        minerals +=
                            SharkyUnitData.UnitData[unitType].MineralCost
                            * EnemyUnitMemoryService.CurrentTotalUnits[unitType];
                    }
                }
            }

            return minerals;
        }

        public long GetCurrentEnemyArmyVespeneCost()
        {
            long minerals = 0;
            foreach (var unitType in EnemyUnitMemoryService.CurrentTotalUnits.Keys)
            {
                if (unitType != UnitTypes.TERRAN_SCV)
                {
                    if (SharkyUnitData.UnitData[unitType].Weapons.Count > 0)
                    {
                        minerals +=
                            SharkyUnitData.UnitData[unitType].VespeneCost
                            * EnemyUnitMemoryService.CurrentTotalUnits[unitType];
                    }
                }
            }

            return minerals;
        }

        public float GetInformationSafety(Observation observation)
        {
            // frame: observation.GameLoop;

            return 0;
        }

        public Tuple<float, float> GetApproximatedProducedEnemyMinerals(
            ResponseObservation observation
        )
        {
            // go trough the history of total units and calculate the total count of minerals

            float minimumMinerals = 0;
            float maximumMinerals = 0;

            if (EnemyUnitApproximationService.LastTotalUnits.ContainsKey(UnitTypes.TERRAN_SCV))
            {
                foreach (
                    var item in EnemyUnitApproximationService.LastTotalUnits[UnitTypes.TERRAN_SCV]
                )
                {
                    Console.WriteLine("List: " + item.Key + " " + item.Value);
                }

                var updateFrames = EnemyUnitApproximationService
                    .LastTotalUnits[UnitTypes.TERRAN_SCV]
                    .Keys.OrderBy(u => u);

                for (int i = 0; i < updateFrames.Count(); i++)
                {
                    uint currentUpdateFrame = updateFrames.ElementAt(i);
                    uint nextFrame = observation.Observation.GameLoop;

                    int knownSCVCount = EnemyUnitApproximationService.LastTotalUnits[
                        UnitTypes.TERRAN_SCV
                    ][currentUpdateFrame];

                    bool nextBreak = true;

                    if (
                        updateFrames.Count() > i + 1
                        && updateFrames.ElementAt(i + 1) <= currentUpdateFrame
                    )
                    {
                        nextFrame = updateFrames.ElementAt(i + 1);
                        nextBreak = false;
                    }

                    TimeSpan timeSpan =
                        FrameToTimeConverter.GetTime((int)nextFrame)
                        - FrameToTimeConverter.GetTime((int)currentUpdateFrame);

                    // float mapUnsafetyFactor = 0.3F;
                    float mineralsPerMinute = 50;
                    // float mineralsPerTwelveSeconds = 10;

                    // https://tl.net/forum/sc2-strategy/140055-scientifically-measuring-mining-speed
                    // SharkyUnitData.UnitData[UnitTypes.TERRAN_SCV]


                    minimumMinerals +=
                        (float)timeSpan.TotalMinutes * knownSCVCount * mineralsPerMinute;
                    // maximumMinerals +=
                    //     GetSCVCalculationSum((float)timeSpan.TotalSeconds, knownSCVCount, 1)
                    //     * mineralsPerTwelveSeconds;
                    // maximumMinerals += (float) timeSpan.TotalMinutes * knownSCVCount * (1 + MapMemoryService.GetUnexploredPercentage(nextFrame) * mapUnsafetyFactor) * mineralsPerMinute;
                    // Console.WriteLine("" + minimumMinerals + "  " + maximumMinerals + "  " + MapMemoryService.GetUnexploredPercentage(nextFrame));

                    if (nextBreak)
                        break;
                }
            }

            return new Tuple<float, float>(minimumMinerals, maximumMinerals);
        }

        private float GetSCVCalculationSum(float time, float startSCVs, float baseCount)
        {
            // float timeCeil = (float) Math.Ceiling(time / 12f);
            float timeCeil = (float)time / 12f;
            // Console.WriteLine("Time: " + time);
            // Console.WriteLine("Ceil: " + timeCeil);
            // Console.WriteLine("Sum: " + GetSum(timeCeil - 1));
            return GetSum(timeCeil - 1) * baseCount + startSCVs * (float)timeCeil;
        }

        private float GetSum(float n)
        {
            return 0.5f * n * (n + 1);
        }

        public float GetApproximatedProducedEnemyGas(ResponseObservation observation)
        {
            // go trough the history of total units and calculate the total count of gas

            float minimumGas = 0;

            if (EnemyUnitMemoryService.LastTotalUnits.ContainsKey(UnitTypes.TERRAN_SCV))
            {
                var updateFrames = EnemyUnitMemoryService
                    .LastTotalUnits[UnitTypes.TERRAN_SCV]
                    .Keys.OrderBy(u => u);

                for (int i = 0; i < updateFrames.Count(); i++)
                {
                    uint currentUpdateFrame = updateFrames.ElementAt(i);
                    uint nextFrame = observation.Observation.GameLoop;

                    int knownSCVCount = EnemyUnitMemoryService.LastTotalUnits[UnitTypes.TERRAN_SCV][
                        currentUpdateFrame
                    ];

                    if (updateFrames.Count() > i + 1)
                    {
                        nextFrame = updateFrames.ElementAt(i + 1);
                    }

                    TimeSpan timeSpan =
                        FrameToTimeConverter.GetTime((int)nextFrame)
                        - FrameToTimeConverter.GetTime((int)currentUpdateFrame);

                    minimumGas += (float)timeSpan.TotalMinutes * knownSCVCount * 39;
                }
            }

            return minimumGas;
        }

        public Dictionary<UnitTypes, int> GetApproximatedProducedEnemyUnits(float minerals)
        {
            Dictionary<UnitTypes, int> units = new Dictionary<UnitTypes, int>();
            float totalMinerals = 0;

            foreach (UnitTypes unit in EnemyUnitMemoryService.CurrentTotalUnits.Keys)
            {
                if (unit != UnitTypes.TERRAN_SCV)
                {
                    if (SharkyUnitData.UnitData[unit].Weapons.Count > 0)
                    {
                        units.Add(unit, EnemyUnitMemoryService.CurrentTotalUnits[unit]);
                        totalMinerals +=
                            SharkyUnitData.UnitData[unit].MineralCost
                            * EnemyUnitMemoryService.CurrentTotalUnits[unit];
                    }
                }
            }

            int factor = (int)Math.Floor(Math.Max(1, minerals / totalMinerals));

            foreach (UnitTypes unit in units.Keys)
            {
                units[unit] *= factor;
            }

            return units;
        }

        public float GetVisibleAreaPercentage()
        {
            float percentage = 0f;

            for (var x = 0; x < MapDataService.MapData.MapWidth; x++)
            {
                for (var y = 0; y < MapDataService.MapData.MapHeight; y++)
                {
                    if (MapDataService.MapData.Map[x, y].Visibility != 0)
                    {
                        percentage +=
                            100f
                            / (
                                MapDataService.MapData.MapWidth
                                * MapDataService.MapData.MapHeight
                                * 1f
                            );
                    }
                }
            }

            return percentage;
        }
    }
}
