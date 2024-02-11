using StarCraft2Bot.Builds.Base.Condition;
using StarCraft2Bot.Builds.Base.Desires;

namespace StarCraft2Bot.Builds.Base.Action
{
    public class ActionNode
    {
        public readonly string? Name = null;
        public IAction? nodeAction;
        public List<ActionNode> childrenNodes = [];

        readonly ParentConditionFunction nodeEnforceCondition;
        internal ActionNode? parentNode;

        private delegate bool ParentConditionFunction(ActionNode node);
        private static readonly ParentConditionFunction parentStartCondition = n => n.nodeAction?.HasStarted() ?? true;
        private static readonly ParentConditionFunction parentCompletedCondition = n => n.nodeAction?.HasCompleted() ?? true;
        private static readonly ParentConditionFunction parentResourcesSpendCondition = n => n.nodeAction?.HasSpendResources() ?? true;

        public delegate void PopulateActionTree(ActionNode root);

        private ActionNode(string? name, ActionNode? parent, IAction? action, ParentConditionFunction condition)
        {
            this.Name = name;
            parentNode = parent;
            nodeAction = action;
            nodeEnforceCondition = condition;
        }

        internal static ActionNode GetRootNode(string name = "Root") => new ActionNode(name, null, null, parentStartCondition);

        public ActionNode AddActionOnStart(string name, IAction action, PopulateActionTree? populate = null) => AddActionToEnforce(name, action, parentStartCondition, populate);
        public ActionNode AddActionOnStart(string name, ICondition condition, IDesire desire, PopulateActionTree? populate = null) => AddActionToEnforce(name, new BuildAction(condition, desire), parentStartCondition, populate);
        public ActionNode AddActionOnStart(string name, IDesire desire, PopulateActionTree? populate = null) => AddActionToEnforce(name, new BuildAction(new NoneCondition(), desire), parentStartCondition, populate);

        public ActionNode AddActionOnResourcesSpend(string name, IAction action, PopulateActionTree? populate = null) => AddActionToEnforce(name, action, parentResourcesSpendCondition, populate);
        public ActionNode AddActionOnResourcesSpend(string name, ICondition condition, IDesire desire, PopulateActionTree? populate = null) => AddActionToEnforce(name, new BuildAction(condition, desire), parentResourcesSpendCondition, populate);
        public ActionNode AddActionOnResourcesSpend(string name, IDesire desire, PopulateActionTree? populate = null) => AddActionToEnforce(name, new BuildAction(new NoneCondition(), desire), parentResourcesSpendCondition, populate);

        public ActionNode AddActionOnCompletion(string name, IAction action, PopulateActionTree? populate = null) => AddActionToEnforce(name, action, parentCompletedCondition, populate);
        public ActionNode AddActionOnCompletion(string name, ICondition condition, IDesire desire, PopulateActionTree? populate = null) => AddActionToEnforce(name, new BuildAction(condition, desire), parentCompletedCondition, populate);
        public ActionNode AddActionOnCompletion(string name, IDesire desire, PopulateActionTree? populate = null) => AddActionToEnforce(name, new BuildAction(new NoneCondition(), desire), parentCompletedCondition, populate);
        private ActionNode AddActionToEnforce(string? name, IAction action, ParentConditionFunction condition, PopulateActionTree? populate)
        {
            ActionNode newChild = new ActionNode(name, this, action, condition);
            populate?.Invoke(newChild);
            childrenNodes.Add(newChild);
            return newChild;
        }

        internal void EnforceNode()
        {
            nodeAction?.Enforce();
            foreach (ActionNode child in childrenNodes)
            {
                if (child.nodeEnforceCondition.Invoke(this))
                {
                    child.EnforceNode();
                }
            }
        }

        internal void InjectActionBetweenParent(string name, IAction action)
        {
            if (this.parentNode == null) return;

            ActionNode nodeToInject = new ActionNode(name, null, action, parentStartCondition);

            nodeToInject.parentNode = this.parentNode;
            nodeToInject.parentNode.childrenNodes.Add(nodeToInject);

            this.parentNode.childrenNodes.Remove(this);

            this.parentNode = nodeToInject;
            nodeToInject.childrenNodes.Add(this);

        }

        internal List<IAction> GetRecursiveChildActions()
        {
            List<IAction> recursiveChildActions = [];
            foreach (ActionNode childNode in childrenNodes)
            {
                if (childNode.nodeAction != null)
                {
                    recursiveChildActions.Add(childNode.nodeAction);
                }
                recursiveChildActions.AddRange(childNode.GetRecursiveChildActions());
            }
            return recursiveChildActions;
        }

        public static void PrintNodeTree(ActionNode node, string indent = "", bool last = true, Func<ActionNode, string>? additionalInfos = null)
        {
            string treeBranch = node.nodeAction?.HasStarted()??false ? "*- " : (node.nodeAction?.HasCompleted() ?? false ? "#- " : "+- ");

            string additionalInfosString = additionalInfos?.Invoke(node) ?? "";
            additionalInfosString = additionalInfosString != "" ? "(" + additionalInfosString + ")" : "";

            Console.WriteLine(indent + treeBranch + node.Name + additionalInfosString);
            indent += last ? "   " : "|  ";

            for (int i = 0; i < node.childrenNodes.Count; i++)
            {
                PrintNodeTree(node.childrenNodes[i], indent, i == node.childrenNodes.Count - 1, additionalInfos);
            }
        }
    }
}
