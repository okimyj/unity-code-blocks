
namespace YJClient
{
    public class RedDotCheckerArena : RedDotCheckerBase
    {
        public RedDotCheckerArena() : base(EContentsType.Arena) { }
        public override void Refresh()
        {
            // 콘텐츠 별 레드닷 체크 로직.
            HasRedDot = true;
        }
    }

}
