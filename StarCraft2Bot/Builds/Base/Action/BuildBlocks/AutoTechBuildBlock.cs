using Microsoft.EntityFrameworkCore;
using Sharky;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds.Base.Action.BuildBlocks
{
    public class AutoTechBuildBlock(BaseBot bot) : BuildBlock([], [], [])
    {
        protected readonly BaseBot DefaultBot = bot;
        protected List<IAction> RequiredTechActions { get; set; } = [];

        public AutoTechBuildBlock WithConditions(List<ICondition> conditions)
        {
            Conditions = conditions;
            RequiredTechActions = GetRequiredTechActions();
            return this;
        }

        public AutoTechBuildBlock WithSerialActions(List<IAction> serial)
        {
            SerialBuildActions = serial;
            RequiredTechActions = GetRequiredTechActions();
            return this;
        }

        public AutoTechBuildBlock WithParallelActions(List<IAction> parallel)
        {
            SerialBuildActions = parallel;
            RequiredTechActions = GetRequiredTechActions();
            return this;
        }

        protected new int CalculateMineralCost()
        {
            return MineralCost + RequiredTechActions.Sum(a => a.MineralCost);
        }

        protected new int CalculateVespeneCost()
        {
            return VespeneCost + RequiredTechActions.Sum(a => a.VespeneCost);
        }

        protected new int CalculateTimeCost()
        {
            return TimeCost + RequiredTechActions.Sum(a => a.TimeCost);
        }

        public new void Enforce()
        {
            RequiredTechActions = GetRequiredTechActions();
            if (RequiredTechActions.Count == 0)
            {
                base.Enforce();
                return;
            }

            foreach (IAction requiredTechAction in RequiredTechActions)
            {
                if (requiredTechAction.AreConditionsFulfilled())
                {
                    requiredTechAction.Enforce();
                }
            }
        }

        private List<IAction> GetRequiredTechActions()
        {
            List<UnitTypes> requiredUnits = [];
            foreach (IAction action in SerialBuildActions)
            {
                foreach (IDesire desire in action.GetDesires())
                {
                    List<UnitTypes> partRequiredUnits = GetUnitTypeFromDesire(desire);
                    Console.WriteLine(string.Join(",", partRequiredUnits));

                    requiredUnits.AddRange(partRequiredUnits);
                }
            }

            //TODO reduce UnitTypes
            //requiredUnits = (List<UnitTypes>)requiredUnits.Distinct();

            return [];
        }

        private List<UnitTypes> GetUnitTypeFromDesire(IDesire desire)
        {
            return desire switch
            {
                UnitDesire d => TerranTechTree.GetListOfAllRequiredTechStructuresForUnit(d.Unit),
                ProductionStructureDesire d => TerranTechTree.GetListOfAllRequiredTechStructuresForUnit(d.StructureType),
                DefenseStructureDesire d => TerranTechTree.GetListOfAllRequiredTechStructuresForUnit(d.StructureType),
                TechStructureDesire d => TerranTechTree.GetListOfAllRequiredTechStructuresForUnit(d.StructureType),
                _ => []
            };
        /*
            string s = "";
            if (desire is UnitDesire uD)
            {
                s += uD.Unit + ": " + string.Join(",", TerranTechTree.GetListOfAllRequiredTechStructuresForUnit(uD.Unit));
            }
            else if (desire is ProductionStructureDesire pD)
            {
                s += pD.StructureType + ": " + string.Join(",", TerranTechTree.GetListOfAllRequiredTechStructuresForUnit(pD.StructureType));
            }
            else if (desire is DefenseStructureDesire dD)
            {
                s += dD.StructureType + ": " + string.Join(",", TerranTechTree.GetListOfAllRequiredTechStructuresForUnit(dD.StructureType));
            }
            else if (desire is TechStructureDesire tD)
            {
                s += tD.StructureType + ": " + string.Join(",", TerranTechTree.GetListOfAllRequiredTechStructuresForUnit(tD.StructureType));
            }
            Console.WriteLine(s);
        */
        }

        private class TerranTechTree
        {
            private static Dictionary<UnitTypes, HashSet<UnitTypes>> TechRequirementDict = new Dictionary<UnitTypes, HashSet<UnitTypes>>
            {
                {UnitTypes.TERRAN_COMMANDCENTER, [] },
                {UnitTypes.TERRAN_SUPPLYDEPOT, [] },
                {UnitTypes.TERRAN_REFINERY, [] },

                {UnitTypes.TERRAN_SCV, [UnitTypes.TERRAN_COMMANDCENTER] },
                {UnitTypes.TERRAN_ENGINEERINGBAY, [UnitTypes.TERRAN_COMMANDCENTER] },

                {UnitTypes.TERRAN_MISSILETURRET, [UnitTypes.TERRAN_ENGINEERINGBAY]},
                {UnitTypes.TERRAN_SENSORTOWER, [UnitTypes.TERRAN_ENGINEERINGBAY]},
                {UnitTypes.TERRAN_PLANETARYFORTRESS, [UnitTypes.TERRAN_ENGINEERINGBAY]},

                {UnitTypes.TERRAN_BARRACKS, [UnitTypes.TERRAN_SUPPLYDEPOT] },

                {UnitTypes.TERRAN_BARRACKSTECHLAB, [UnitTypes.TERRAN_BARRACKS] },
                {UnitTypes.TERRAN_ORBITALCOMMAND, [UnitTypes.TERRAN_BARRACKS] },
                {UnitTypes.TERRAN_FACTORY, [UnitTypes.TERRAN_BARRACKS] },
                {UnitTypes.TERRAN_GHOSTACADEMY, [UnitTypes.TERRAN_BARRACKS] },
                {UnitTypes.TERRAN_BUNKER, [UnitTypes.TERRAN_BARRACKS] },
                {UnitTypes.TERRAN_MARINE, [UnitTypes.TERRAN_BARRACKS] },
                {UnitTypes.TERRAN_REAPER, [UnitTypes.TERRAN_BARRACKS] },

                {UnitTypes.TERRAN_MARAUDER, [UnitTypes.TERRAN_BARRACKSTECHLAB] },

                {UnitTypes.TERRAN_GHOST, [UnitTypes.TERRAN_GHOSTACADEMY] },

                {UnitTypes.TERRAN_FACTORYTECHLAB, [UnitTypes.TERRAN_FACTORY] },
                {UnitTypes.TERRAN_ARMORY, [UnitTypes.TERRAN_FACTORY] },
                {UnitTypes.TERRAN_STARPORT, [UnitTypes.TERRAN_FACTORY] },
                {UnitTypes.TERRAN_HELLION, [UnitTypes.TERRAN_FACTORY] },
                {UnitTypes.TERRAN_WIDOWMINE, [UnitTypes.TERRAN_FACTORY] },
                {UnitTypes.TERRAN_CYCLONE, [UnitTypes.TERRAN_FACTORY] },

                {UnitTypes.TERRAN_SIEGETANK, [UnitTypes.TERRAN_FACTORYTECHLAB] },

                {UnitTypes.TERRAN_HELLIONTANK, [UnitTypes.TERRAN_FACTORY, UnitTypes.TERRAN_ARMORY] },
                {UnitTypes.TERRAN_THOR, [UnitTypes.TERRAN_FACTORYTECHLAB, UnitTypes.TERRAN_ARMORY] },
                {UnitTypes.TERRAN_THORAP, [UnitTypes.TERRAN_FACTORYTECHLAB, UnitTypes.TERRAN_ARMORY] },

                {UnitTypes.TERRAN_STARPORTTECHLAB, [UnitTypes.TERRAN_STARPORT] },
                {UnitTypes.TERRAN_FUSIONCORE, [UnitTypes.TERRAN_STARPORT] },
                {UnitTypes.TERRAN_VIKINGFIGHTER, [UnitTypes.TERRAN_STARPORT] },
                {UnitTypes.TERRAN_VIKINGASSAULT, [UnitTypes.TERRAN_STARPORT] },
                {UnitTypes.TERRAN_MEDIVAC, [UnitTypes.TERRAN_STARPORT] },
                {UnitTypes.TERRAN_LIBERATOR, [UnitTypes.TERRAN_STARPORT] },
                {UnitTypes.TERRAN_LIBERATORAG, [UnitTypes.TERRAN_STARPORT] },

                {UnitTypes.TERRAN_RAVEN, [UnitTypes.TERRAN_STARPORTTECHLAB] },
                {UnitTypes.TERRAN_BANSHEE, [UnitTypes.TERRAN_STARPORTTECHLAB] },

                {UnitTypes.TERRAN_BATTLECRUISER, [UnitTypes.TERRAN_STARPORTTECHLAB, UnitTypes.TERRAN_FUSIONCORE] },
            };

            public static HashSet<UnitTypes> GetUnlockTechStructuresForUnit(UnitTypes unit)
            {
                return TechRequirementDict[unit];
            }

            public static List<UnitTypes> GetListOfAllRequiredTechStructuresForUnit(UnitTypes unit)
            {
                List<UnitTypes> allTechStructures = TechRequirementDict[unit].ToList();

                bool stable = false;
                while (!stable)
                {
                    stable = true;
                    foreach (UnitTypes techStructure in allTechStructures)
                    {
                        HashSet<UnitTypes> recursiveTech = TechRequirementDict[techStructure];
                        if (!recursiveTech.IsSubsetOf(allTechStructures))
                        {
                            allTechStructures = [..recursiveTech, .. allTechStructures];
                            stable = false;
                        }
                    }
                }
                return allTechStructures;
            }
        }
    }
}
 