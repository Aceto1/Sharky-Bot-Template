using Sharky;

namespace StarCraft2Bot.Helper
{
    public static class TerranTechTree
    {
        private static Dictionary<UnitTypes, HashSet<UnitTypes>> TechTreeDict = new Dictionary<UnitTypes, HashSet<UnitTypes>>
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

        public static HashSet<UnitTypes> TechUnits => TechTreeDict.Values.SelectMany(v => v).ToHashSet();

        public static List<UnitTypes> GetRecursiveRequiredTechStructuresForUnit(UnitTypes unit)
        {
            List<UnitTypes> allTechStructures = TechTreeDict[unit].ToList();

            bool stable = false;
            while (!stable)
            {
                stable = true;
                foreach (UnitTypes techStructure in allTechStructures)
                {
                    HashSet<UnitTypes> recursiveTech = TechTreeDict[techStructure];
                    if (!recursiveTech.IsSubsetOf(allTechStructures))
                    {
                        allTechStructures = [.. recursiveTech, .. allTechStructures];
                        stable = false;
                    }
                }
            }
            return allTechStructures;
        }
    }
}
