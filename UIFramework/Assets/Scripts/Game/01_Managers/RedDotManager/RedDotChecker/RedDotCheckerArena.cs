
namespace YJClient
{
    public class RedDotCheckerArena : RedDotCheckerBase
    {
        public RedDotCheckerArena() : base(EContentsType.Arena) { }
        public override void Refresh()
        {
            var hasTicket = HasArenaTicket();
            var hasReward = HasArenaReward();
            HasRedDot = hasTicket || hasReward;
        }
        private bool HasArenaTicket()
        {
            // 실제 티켓 소지 여부 로직.
            return true;
        }
        private bool HasArenaReward()
        {
            // 실제 수령 가능 보상 여부 로직.
            return false;
        }
    }

}
