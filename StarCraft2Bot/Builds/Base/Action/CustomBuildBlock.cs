using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarCraft2Bot.Builds.Base.Condition;

namespace StarCraft2Bot.Builds.Base.Action
{
    public class CustomBuildBlock : BuildBlock
    {
        public CustomBuildBlock() {}

        public delegate void AppendBuildActionsFunc(List<IAction> serial, List<IAction> parallel);
        public delegate void AppendPreConditionsFunc(List<ICondition> conditions);
        public CustomBuildBlock WithBuildActions(AppendBuildActionsFunc appendFunction)
        {
            appendFunction(SerialBuildActions, ParallelBuildActions);
            return this;
        }
        
        public CustomBuildBlock WithConditions(AppendPreConditionsFunc appendFunction)
        {
            appendFunction(Conditions);
            return this;
        }
    }
}
