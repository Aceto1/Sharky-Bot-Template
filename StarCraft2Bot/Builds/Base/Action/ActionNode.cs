namespace StarCraft2Bot.Builds.Base.Action
{
    public class ActionNode
    {
        public readonly string? Name = null;
        public readonly IAction? nodeAction;
        readonly ParentConditionFunction nodeEnforceCondition;
        readonly ActionNode? parentNode;
        readonly List<ActionNode> childrenNodes = [];

        private delegate bool ParentConditionFunction(ActionNode node);
        private static readonly ParentConditionFunction parentStartCondition = n => n.nodeAction?.HasStarted() ?? true;
        private static readonly ParentConditionFunction parentCompletedCondition = n => n.nodeAction?.HasCompleted() ?? true;

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
        public ActionNode AddActionOnCompletion(string name, IAction action, PopulateActionTree? populate = null) => AddActionToEnforce(name, action, parentCompletedCondition, populate);
        public ActionNode AddActionOnStart(IAction action, PopulateActionTree? populate = null) => AddActionToEnforce(null, action, parentStartCondition, populate);
        public ActionNode AddActionOnCompletion(IAction action, PopulateActionTree? populate = null) => AddActionToEnforce(null, action, parentCompletedCondition, populate);
        private ActionNode AddActionToEnforce(string? name, IAction action, ParentConditionFunction condition, PopulateActionTree? populate)
        {
            ActionNode newChild = new ActionNode(name, this, action, condition);
            populate?.Invoke(newChild);
            childrenNodes.Add(newChild);
            return this;
        }

        internal void EnforceChildrenNodes()
        {
            foreach (ActionNode child in childrenNodes)
            {
                if (child.nodeEnforceCondition.Invoke(this))
                {
                    child.nodeAction?.Enforce();
                    child.EnforceChildrenNodes();
                }
            }
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
            Console.WriteLine(indent + "+- " + node.Name + "(" + additionalInfos?.Invoke(node) + ")");
            indent += last ? "   " : "|  ";

            for (int i = 0; i < node.childrenNodes.Count; i++)
            {
                PrintNodeTree(node.childrenNodes[i], indent, i == node.childrenNodes.Count - 1, additionalInfos);
            }
        }
    }
}
