﻿using StarCraft2Bot.Builds.Base.Action;

namespace StarCraft2Bot.Builds.Base.Condition
{
    public class ActionCompletedCondition : ICondition
    {
        public ActionCompletedCondition(IAction action)
        {
            Action = action;
        }

        public IAction Action { get; private set; }

        public bool IsFulfilled()
        {
            return Action.HasCompleted();
        }
    }
}
