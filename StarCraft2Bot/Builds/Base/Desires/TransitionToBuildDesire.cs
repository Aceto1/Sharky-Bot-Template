using StarCraft2Bot.Bot;
using Type = System.Type;

namespace StarCraft2Bot.Builds.Base.Desires
{
    public class TransitionToBuildDesire : IDesire
    {
        public TransitionToBuildDesire(Build currentBuild, Type desiredBuild, BaseBot bot)
        {
            if (desiredBuild.BaseType != typeof(Build))
            {
                throw new ArgumentOutOfRangeException(nameof(desiredBuild), desiredBuild, $"Parameter {nameof(desiredBuild)} must have base type {nameof(Build)}!");
            }

            CurrentBuild = currentBuild;
            DesiredBuild = desiredBuild;
            Bot = bot;
        }

        public TransitionToBuildDesire(Build currentBuild, Type desiredBuild, BaseBot bot, int mineralCost, int vespeneCost, int timeCost) : this(currentBuild, desiredBuild, bot)
        {
            MineralCost = mineralCost;
            VespeneCost = vespeneCost;
            TimeCost = timeCost;
        }

        public Type DesiredBuild { get; set; }

        public Build  CurrentBuild { get; set; }

        public BaseBot Bot { get; set; }

        public bool Enforced { get; set; }

        public int MineralCost { get; }

        public int VespeneCost { get; }

        public int TimeCost { get; }

        public void Enforce()
        {
            if (Enforced)
                return;

            Bot.BuildChoices[Bot.MacroData.Race].BuildSequences["Transition"] = new List<List<string>>()
            {
                new List<string> { DesiredBuild.Name },
            };

            CurrentBuild.DoTransition = true;

            Enforced = true;
        }
    }
}
