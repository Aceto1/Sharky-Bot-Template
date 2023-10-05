using SC2APIProtocol;
using Sharky.Builds.Terran;
using Sharky.DefaultBot;
using StarCraft2Bot.Helper;

namespace StarCraft2Bot.Builds.Base
{
    public class Build : TerranSharkyBuild
    {
        private const int secondsPerMeasurement = 1;

        private readonly DefaultSharkyBot defaultBot;

        private int frame = 0;

        public Build(DefaultSharkyBot defaultSharkyBot) : base(defaultSharkyBot)
        {
            this.defaultBot = defaultSharkyBot;
        }

        public override void StartBuild(int frame)
        {
            base.StartBuild(frame);

            ValueManager.CurrentBuild = this.GetType().Name;
        }

        public override void OnFrame(ResponseObservation observation)
        {
            base.OnFrame(observation);

            frame++;

            if (frame % (defaultBot.SharkyOptions.FramesPerSecond * secondsPerMeasurement) == 0)
            {
                Measure();
            }
        }

        private void Measure()
        {
            //TODO: Measure current state
        }
    }
}
