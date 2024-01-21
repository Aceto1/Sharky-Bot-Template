using System.Reflection;
using SC2APIProtocol;
using Sharky.DefaultBot;
using StarCraft2Bot.Bot;
using StarCraft2Bot.Builds.Base.Action;
using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds.Base
{

    public class AdvancedBuild : Build
    {
        private readonly Queue<BuildAction> buildOrder = new();
        private readonly List<BuildAction> shadowActions = new(); //duplicated list of actions ireBuild.class to allow debug messages for unordered buildActions

        public bool Verbose { get; set; } = true;

        public AdvancedBuild(BaseBot defaultSharkyBot) : base(defaultSharkyBot){}

        public new void AddAction(BuildAction action)
        {
            base.AddAction(action);
            EnqueueActionDebugMessage(action);
            shadowActions.Add(action);
        }

        public void AddAction(ICondition condition, params IDesire[] desires)
        {
            this.AddAction(new BuildAction(condition, desires));
        }

        public void AppendToBuildOrder(BuildAction action)
        {
            EnqueueActionDebugMessage(action);
            buildOrder.Enqueue(action);
        }

        public void AppendToBuildOrder(ICondition condition, params IDesire[] desires)
        {
            AppendToBuildOrder(new BuildAction(condition, desires));
        }

        public bool IsBuildOrderCompleted()
        {
            return buildOrder.Count == 0;
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);
        }

        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);

            //workaround to send debug message for simple action
            for (int i = shadowActions.Count - 1; i >= 0; i--)
            {
                var action = shadowActions[i];
                if (action.AreConditionsFulfilled())
                {
                    FullfillActionDebugMessage(action);
                    shadowActions.Remove(action);
                }
            }

            if (buildOrder.Count == 0) return;

            var nextAction = buildOrder.Peek();
            if (nextAction.AreConditionsFulfilled())
            {
                FullfillActionDebugMessage(nextAction);

                nextAction.Enforce();
                buildOrder.Dequeue();
            }
        }

        public override bool Transition(int frame)
        {
            return base.Transition(frame);
        }

        //send debug messages
        private void EnqueueActionDebugMessage(BuildAction action)
        {
            SendDebugMessage($"{GetActionAsString(action)}: Enqueued");
        }

        private void FullfillActionDebugMessage(BuildAction action)
        {
            SendDebugMessage($"{GetActionAsString(action)}: Fullfilled");
        }

        public void SendDebugMessage(string message, bool useDebugChat = false)
        {
            if (!Verbose) return;

            if (message != null)
            {
                if (useDebugChat)
                {
                    ChatService.SendDebugChatMessage(message);
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }

        private string GetActionAsString(BuildAction action)
        {
            string conditionString = String.Join(";", action.Conditions.ConvertAll(GetConditionAsString));
            string desireString =  String.Join(";", action.Desires.ConvertAll(GetDesireAsString));
            return $"Action({conditionString} :: {desireString})";
        }

        private string GetDesireAsString(IDesire desire)
        {
            return desire switch
            {
                SupplyDepotDesire => $"Build {(int) ((SupplyDepotDesire)desire).Count} SupplyDepots",
                GasBuildingCountDesire => $"Build {(int)((GasBuildingCountDesire)desire).Count} GasBuildings",
                ProductionStructureDesire => $"Build {(int) ((ProductionStructureDesire)desire).Count} {((ProductionStructureDesire)desire).StructureType}",
                AddonStructureDesire => $"Build {(int)((AddonStructureDesire)desire).Count} {((AddonStructureDesire)desire).AddonType}",
                MorphDesire => $"Build {(int)((MorphDesire)desire).Count} {((MorphDesire)desire).TargetType}",
                TechStructureDesire => $"Build{(int)((TechStructureDesire)desire).Count} {((TechStructureDesire)desire).StructureType}",
                UnitDesire => $"Train {(int)((UnitDesire)desire).Count} {((UnitDesire)desire).Unit}",
                UnitUpgradeDesire => $"Upgrade {((UnitUpgradeDesire)desire).TargetType}",
                DefenseStructureDesire => $"Build {((DefenseStructureDesire)desire).Count} {((DefenseStructureDesire)desire).StructureType}",
                CustomDesire => $"{((CustomDesire)desire).CustomEnforceFunc.Method}",
                _ => $"TODO Implement GetDesireAsString for {desire.GetType().Name}"
            };
        }

        private string GetConditionAsString(ICondition condition)
        {
            return condition switch
            {
                SupplyCondition => $"SupplyDepots {GetOperatorAsString(((SupplyCondition)condition).Operator)} {(int) ((SupplyCondition)condition).SupplyCount}",
                WorkerCountCondition => $"WorkerCount {GetOperatorAsString(((WorkerCountCondition)condition).Operator)} {(int)((WorkerCountCondition)condition).WorkerCount}",
                UnitCountCondition => $"{((UnitCountCondition)condition).Unit} {GetOperatorAsString(((UnitCountCondition)condition).Operator)} {(int) ((UnitCountCondition)condition).Count} Count",
                UnitCompletedCountCondition => $"{((UnitCompletedCountCondition)condition).Unit} {GetOperatorAsString(((UnitCompletedCountCondition)condition).Operator)} {(int)((UnitCompletedCountCondition)condition).Count} Completed",
                BuildingDoneOrInProgressCondition => $"{((BuildingDoneOrInProgressCondition)condition).Unit} {GetOperatorAsString(((BuildingDoneOrInProgressCondition)condition).Operator)} {(int)((BuildingDoneOrInProgressCondition)condition).Count} DoneOrProgress",
                PhaseCondition => $"On Phase {((PhaseCondition)condition).PhaseCount}",
                NoneCondition => "Always",
                CustomCondition => $"{((CustomCondition)condition).CustomConditionFunc.Method}",
                _ => $"TODO Implement GetConditionAsString for {condition.GetType().Name}"
            };;
        }

        private string GetOperatorAsString(ConditionOperator conditionOperator) {
            return conditionOperator switch
            {
                ConditionOperator.Smaller => "<",
                ConditionOperator.SmallerOrEqual => "<=",
                ConditionOperator.Equal => "==",
                ConditionOperator.GreaterOrEqual => ">=",
                ConditionOperator.Greater => ">",
                _ => ":"
            };
        }
    }
}
