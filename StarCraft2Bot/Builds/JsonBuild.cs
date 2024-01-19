using SC2APIProtocol;
using Sharky;
using Sharky.Managers;
using Sharky.MicroTasks.Attack;
using Sharky.MicroTasks;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base;
using StarCraft2Bot.Helper;

namespace StarCraft2Bot.Builds
{
    //convert buildorder text to json string with https://toolsaday.com/text-tools/json-stringify
    public class JsonBuild : AdvancedBuild
    {
        readonly JsonBuildSettings JsonBuildSettings;

        public JsonBuild(BaseBot defaultSharkyBot, string buildName, JsonBuildConditionType jsonBuildConditionType = JsonBuildConditionType.OnlySupplyCondition) : base(defaultSharkyBot)
        {
            JsonBuildTemplate jsonBuildTemplate = JsonBuildTemplate.GetJsonBuildTemplateByName(buildName);
            JsonBuildSettings = new JsonBuildSettings(jsonBuildTemplate, MacroData, UnitCountService, SharkyUnitData, jsonBuildConditionType);
            InitAttackManager(defaultSharkyBot);
        }

        private void InitAttackManager(BaseBot defaultSharkyBot)
        {
            var advancedAttackTask = new AdvancedAttackTask(defaultSharkyBot, new EnemyCleanupService(defaultSharkyBot.MicroController,
                defaultSharkyBot.DamageService), new List<UnitTypes> { UnitTypes.TERRAN_MARINE }, 1f, true);
            defaultSharkyBot.MicroTaskData[typeof(AttackTask).Name] = advancedAttackTask;
            var advancedAttackService = new AdvancedAttackService(defaultSharkyBot, advancedAttackTask);
            var advancedAttackDataManager = new AdvancedAttackDataManager(defaultSharkyBot, advancedAttackService, advancedAttackTask);
            defaultSharkyBot.AttackDataManager = advancedAttackDataManager;
            defaultSharkyBot.Managers.RemoveAll(m => m.GetType() == typeof(AttackDataManager));
            defaultSharkyBot.Managers.Add(advancedAttackDataManager);
        }

        public new string Name()
        {
            return JsonBuildSettings.template.name;
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);
            BuildOptions.StrictSupplyCount = true;
            BuildOptions.StrictGasCount = true;
            BuildOptions.StrictWorkerCount = false;

            List<BuildAction> buildActions = JsonBuildParser.GetBuildActionsFromJsonBuildTemplate(JsonBuildSettings);
            buildActions.ForEach(AddAction);
        }

        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);
        }

        public override bool Transition(int frame)
        {
            if (actions.All(a => a.AreConditionsFulfilled()))
            {
                return true;
            }
            return base.Transition(frame);
        }


    }
}
