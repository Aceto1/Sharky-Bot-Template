using System.Text.RegularExpressions;
using Sharky;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;
using Newtonsoft.Json;
using StarCraft2Bot.Builds.Base.Action;

namespace StarCraft2Bot.Helper
{
    public struct JsonBuildTemplate
    {
        private const string JSON_BUILD_FILEPATH = "Helper/";
        private const string JSON_BUILD_FILE = JSON_BUILD_FILEPATH + "BuildOrders.json";

        public string name;
        public string url;
        public string buildOrder;

        public static JsonBuildTemplate GetJsonBuildTemplateByName(string buildName)
        {
            string jsonText = File.ReadAllText(JSON_BUILD_FILE);
            JsonBuildTemplate[]? jsonBuildTemplates = JsonConvert.DeserializeObject<JsonBuildTemplate[]>(jsonText);
            return jsonBuildTemplates?.First(jb => jb.name == buildName) ?? throw new Exception($"Build {buildName} not found");
        }
    }

    public struct JsonBuildSettings
    {
        public readonly JsonBuildTemplate template;
        public readonly JsonBuildConditionType conditionType;
        public readonly MacroData data;
        public readonly UnitCountService unitCountService;
        public readonly SharkyUnitData unitData;

        public JsonBuildSettings(JsonBuildTemplate template, MacroData data, UnitCountService unitCountService, SharkyUnitData unitData, JsonBuildConditionType conditionType = JsonBuildConditionType.OnlySupplyCondition)
        {
            this.template = template;
            this.data = data;
            this.conditionType = JsonBuildConditionType.OnlySupplyCondition;
            this.unitCountService = unitCountService;
            this.unitData = unitData;

            //TODO enable time condition, fix observation error
            if (conditionType != JsonBuildConditionType.OnlySupplyCondition)
            {
                Console.WriteLine($"Unable to use ConditionType {conditionType}, TimeCondition not working");
                Console.WriteLine($"Falling back to ConditionType{this.conditionType}");
            }
        }
    }

    public static class JsonBuildParser
    {   
        public static List<BuildAction> GetBuildActionsFromJsonBuildTemplate(JsonBuildSettings buildSettings)
        {
            Console.WriteLine($"Parsing Build Order for build {buildSettings.template.name}");

            List<string> buildActionStringList = buildSettings.template.buildOrder.Trim().Split("\n").ToList();

            Dictionary<string, int> buildActionAmounts = new Dictionary<string, int>() { { "Command Center", 1 } };
            List<BuildAction> buildActionList = new();
            foreach (var buildActionString in buildActionStringList)
            {
                BuildAction buildAction = ParseBuildActionFromString(buildActionString, buildActionAmounts, buildSettings);
                buildActionList.Add(buildAction);
            }
            return buildActionList;
        }

        private static BuildAction ParseBuildActionFromString(string buildActionString, Dictionary<string, int> buildActionAmounts, JsonBuildSettings buildSettings)
        {
            buildActionString = Regex.Replace(buildActionString.Replace("\t", " "), @"\s+", " ").Trim();
            Console.WriteLine($"Parsing BuildAction '{buildActionString}'");

            int firstSpaceIndex = buildActionString.IndexOf(' ');
            int secondSpaceIndex = buildActionString.IndexOf(' ', firstSpaceIndex + 1);
            string supplyString = buildActionString[0..firstSpaceIndex].Trim();
            string timeString = buildActionString[firstSpaceIndex..secondSpaceIndex].Trim();
            string desiresString = buildActionString[secondSpaceIndex..].Trim();

            ICondition condition = ParseConditionFromString(supplyString, timeString, buildSettings);
            List<IDesire> desires = ParseDesiresFromStringList(desiresString, buildActionAmounts, buildSettings);
            return new BuildAction(condition, desires);
        }

        private static ICondition ParseConditionFromString(string supplyCount, string timeCount, JsonBuildSettings buildSettings)
        {
            int supplyCountValue = int.Parse(supplyCount);
            double timeCountValueInMinutes = int.Parse(timeCount[..timeCount.IndexOf(':')]) * 60;
            double timeCountValueInSeconds = 60 *  timeCountValueInMinutes + int.Parse(timeCount[(timeCount.IndexOf(':') + 1)..]);

            SupplyCondition supplyCondition = new SupplyCondition(supplyCountValue, buildSettings.data);
            TimeCondition timeCondition = new TimeCondition(timeCountValueInSeconds);

            return buildSettings.conditionType switch
            {
                JsonBuildConditionType.OnlySupplyCondition => supplyCondition,
                JsonBuildConditionType.OnlyTimeCondition => timeCondition,
                JsonBuildConditionType.AndCondition => new AndCondition(supplyCondition, timeCondition),
                JsonBuildConditionType.OrCondition => new OrCondition(new List<ICondition>() { supplyCondition, timeCondition }),
                _ => new NoneCondition(),
            };
        }

        private static List<IDesire> ParseDesiresFromStringList(string desiresString, Dictionary<string, int> buildActionAmounts, JsonBuildSettings buildSettings)
        {
            List<IDesire> desires = new();
            string[] desireStringList = desiresString.Split(',');
            foreach (string desireString in desireStringList)
            {
                int amount = 1;
                string desireActionString = desireString.Trim();

                int xTimesIndex = desireActionString.LastIndexOf(" x");
                if (xTimesIndex != -1) //handle shorthand "buildaction x<AMOUNT>"
                {
                    amount = int.Parse(desireActionString[(xTimesIndex + 2)..]);
                    desireActionString = desireActionString[..xTimesIndex];
                }
                buildActionAmounts[desireActionString] = buildActionAmounts.GetValueOrDefault(desireActionString, 0) + amount;

                IDesire parsedDesire = ParseDesireFromString(desireActionString, buildActionAmounts[desireActionString], buildSettings.data, buildSettings.unitCountService, buildSettings.unitData);
                desires.Add(parsedDesire);
            }
            return desires;
        }

        private static IDesire ParseDesireFromString(string desireString, int amount, MacroData data, UnitCountService unitCountService, SharkyUnitData unitData)
        {
            return desireString switch
            {
                "Armory" => new ProductionStructureDesire(UnitTypes.TERRAN_ARMORY, amount, data, unitCountService),
                "Barracks" => new ProductionStructureDesire(UnitTypes.TERRAN_BARRACKS, amount, data, unitCountService),
                "Barracks Tech Lab" => new AddonStructureDesire(UnitTypes.TERRAN_TECHLAB, amount, data, unitCountService),
                "Barracks Reactor" => new AddonStructureDesire(UnitTypes.TERRAN_BARRACKSREACTOR, amount, data, unitCountService),
                "Combat Shield" => new UnitUpgradeDesire(Upgrades.SHIELDWALL, data, unitData),
                "Command Center" => new ProductionStructureDesire(UnitTypes.TERRAN_COMMANDCENTER, amount, data, unitCountService),
                "Cyclone" => new UnitDesire(UnitTypes.TERRAN_CYCLONE, amount, data.DesiredUnitCounts, unitCountService),
                "Engineering Bay" => new ProductionStructureDesire(UnitTypes.TERRAN_ENGINEERINGBAY, amount, data, unitCountService),
                "Factory" => new ProductionStructureDesire(UnitTypes.TERRAN_FACTORY, amount, data, unitCountService),
                "Factory Tech Lab" => new AddonStructureDesire(UnitTypes.TERRAN_FACTORYTECHLAB, amount, data, unitCountService),
                "Hellion" => new UnitDesire(UnitTypes.TERRAN_HELLION, amount, data.DesiredUnitCounts, unitCountService),
                "Marauder" => new UnitDesire(UnitTypes.TERRAN_MARAUDER, amount, data.DesiredUnitCounts, unitCountService),
                "Marine" => new UnitDesire(UnitTypes.TERRAN_MARINE, amount, data.DesiredUnitCounts, unitCountService),
                "Medivac" => new UnitDesire(UnitTypes.TERRAN_MEDIVAC, amount, data.DesiredUnitCounts, unitCountService),
                "Missile Turret" => new DefenseStructureDesire(UnitTypes.TERRAN_MISSILETURRET, amount, data, unitCountService),
                "Orbital Command" => new MorphDesire(UnitTypes.TERRAN_ORBITALCOMMAND, amount, data, unitCountService),
                "Raven" => new UnitDesire(UnitTypes.TERRAN_RAVEN, amount, data.DesiredUnitCounts, unitCountService),
                "Siege Tank" => new UnitDesire(UnitTypes.TERRAN_SIEGETANK, amount, data.DesiredUnitCounts, unitCountService),
                "Reaper" => new UnitDesire(UnitTypes.TERRAN_REAPER, amount, data.DesiredUnitCounts, unitCountService),
                "Refinery" => new GasBuildingCountDesire(amount, data, unitCountService),
                "Sensor Tower" => new DefenseStructureDesire(UnitTypes.TERRAN_SENSORTOWER, amount, data, unitCountService),
                "Starport" => new ProductionStructureDesire(UnitTypes.TERRAN_STARPORT, amount, data, unitCountService),
                "Starport Tech Lab" => new AddonStructureDesire(UnitTypes.TERRAN_STARPORTTECHLAB, amount, data, unitCountService),
                "Starport Reactor" => new AddonStructureDesire(UnitTypes.TERRAN_STARPORTREACTOR, amount, data, unitCountService),
                "Stimpack" => new UnitUpgradeDesire(Upgrades.STIMPACK, data, unitData),
                "Supply Depot" => new SupplyDepotDesire(amount, data, unitCountService),
                "Terran Infantry Armor Level 1" => new UnitUpgradeDesire(Upgrades.TERRANINFANTRYARMORSLEVEL1, data, unitData),
                "Terran Infantry Armor Level 2" => new UnitUpgradeDesire(Upgrades.TERRANINFANTRYARMORSLEVEL2, data, unitData),
                "Terran Infantry Weapons Level 1" => new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL1, data, unitData),
                "Terran Infantry Weapons Level 2" => new UnitUpgradeDesire(Upgrades.TERRANINFANTRYWEAPONSLEVEL2, data, unitData),
                "Terran Vehicle And Ship Armor Level 1" => new UnitUpgradeDesire(Upgrades.TERRANVEHICLEANDSHIPARMORSLEVEL1, data, unitData),
                "Thor" => new UnitDesire(UnitTypes.TERRAN_THOR, amount, data.DesiredUnitCounts, unitCountService),
                "Widow Mine" => new UnitDesire(UnitTypes.TERRAN_WIDOWMINE, amount, data.DesiredUnitCounts, unitCountService),
                _ => throw new Exception($"TODO: Include {desireString} as switch case to ParseDesireFromString in JsonBuild.cs", null),
            };
        }
    }
    public enum JsonBuildConditionType
    {
        OnlyTimeCondition,
        OnlySupplyCondition,
        AndCondition,
        OrCondition
    }
}
