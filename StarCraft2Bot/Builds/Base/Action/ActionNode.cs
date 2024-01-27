namespace StarCraft2Bot.Builds.Base.Action
{
    public class ActionNode
    {
        readonly IAction? nodeAction;
        readonly ParentConditionFunction nodeEnforceCondition;
        readonly ActionNode? parentNode;
        readonly List<ActionNode> childrenNodes = [];

        private delegate bool ParentConditionFunction(ActionNode node);
        private static readonly ParentConditionFunction parentStartCondition = n => n.nodeAction?.HasStarted() ?? true;
        private static readonly ParentConditionFunction parentCompletedCondition = n => n.nodeAction?.HasCompleted() ?? true;

        public delegate void PopulateActionTree(ActionNode root);

        private ActionNode(ActionNode? parent, IAction? action, ParentConditionFunction condition)
        {
            parentNode = parent;
            nodeAction = action;
            nodeEnforceCondition = condition;
        }
        internal static ActionNode GetRootNode() => new ActionNode(null, null, parentStartCondition);

        public ActionNode AddActionOnStart(IAction action, PopulateActionTree? populate = null) => AddActionToEnforce(action, parentStartCondition, populate);
        public ActionNode AddActionOnCompletion(IAction action, PopulateActionTree? populate = null) => AddActionToEnforce(action, parentCompletedCondition, populate);
        private ActionNode AddActionToEnforce(IAction action, ParentConditionFunction condition, PopulateActionTree? populate)
        {
            ActionNode newChild = new ActionNode(this, action, condition);
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

        /**
        internal List<ActionNode> GetParentNodes()
        {
            List<ActionNode> parentNodes = [];
            ActionNode? node = parentNode;
            while (node != null)
            {
                parentNodes.Add(node);
                node = node.parentNode;
            }
            return parentNodes;
        }

        internal List<ActionNode> GetRecursiveChildrenNodes()
        {
            List<ActionNode> recursiveChildrenNodes = [];
            foreach (ActionNode childNode in childrenNodes)
            {
                recursiveChildrenNodes.Add(childNode);
                recursiveChildrenNodes.AddRange(childNode.GetRecursiveChildrenNodes());
            }
            return recursiveChildrenNodes;
        }
        */
    }
}
