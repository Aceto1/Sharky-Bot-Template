using Sharky;
using Sharky.DefaultBot;
using Sharky.Managers;

namespace StarCraft2Bot.Bot
{
    public class BaseBot : DefaultSharkyBot
    {
        public int Frame { get; set; }

        public BaseBot(GameConnection gameConnection) : base(gameConnection)
        {
        }

        public override SharkyBot CreateBot(List<IManager> managers, DebugService debugService)
        {
            return new CustomSharkyBot(managers, debugService, FrameToTimeConverter, SharkyOptions, PerformanceData, ChatService, TagService, this);
        }

        public override SharkyBot CreateBot()
        {
            return new CustomSharkyBot(Managers, DebugService, FrameToTimeConverter, SharkyOptions, PerformanceData, ChatService, TagService, this);
        }
    }
}
